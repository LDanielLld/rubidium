using FastDtw.CSharp;
using REVIREPanels.Componentes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace REVIREPanels
{
    /// <summary>
    /// Clase encargada de gestionar cada una de las trayectorias y calcular todos los parametros
    /// que definen una escala clinica a partir de una ponderacion de los valores
    /// </summary>
    public class Trial
    {
        //*********************************************Variables**********************************************//
        //****************************************************************************************************//
        #region [Public Variables] Parametros de la escala
        public double distanceTotal = 0.0f;
        public double percDistance = 0.0f;
        public double timeTotal = 0.0f;
        public double reactionTime = 0.0f;
        public double speedMax = 0.0f;
        public double errorInicial = 0.0f;
        public double razonInicial = 0.0f;
        public double similityDTW = 0.0f;
        public double distanceDTW = 0.0f;
        public bool isCompleted = false;
        #endregion

        #region [Public Variables] Parametros ideales
        public double distanceTotalIdeal = 0.0f;
        public double[] speedIdeal = new double[2];
        public double[] timeIdeal = new double[2];
        public double[] errorIdeal = new double[2];
        public double[] razonIdeal = new double[2];
        #endregion

        #region [private Variables] Almacenamiento de datos
        private DataRobot[] data; //Trayectoria real
        private List<double[]> ideal; //Trayectoria ideal

        private double[] t_ideal;
        private double[] t_real;
        #endregion

        #region [Private Variables] Condicionantes de la escala
        private double THRESHOLD_INIT_MOVEMENT = 0.0;
        private double[] center = new double[2] { 0.0f, -530.0f };
        private double[] game_points = new double[2] { -4.44f, 4.44f };
        private double[] ponderations = new double[] { 0.20f, 0.10f, 0.20f, 0.05f, 0.15f, 0.10f, 0.05f, 0.15f};
        #endregion
        //****************************************************************************************************//
        //****************************************************************************************************//



        //*****************************************Creacion de trials*****************************************//
        //****************************************************************************************************//
        #region [public Functions] Constructor y gestor del trial
        public Trial(DataRobot[] dataIn, TrialConditions condition, float time, float ampl, int id)
        {
            //Copia los datos recibidos
            data = dataIn.ToArray();

            //Crea ideal
            ideal = GenerateIdealPath(id, ampl, data.Length);

            //Separa datos de condiciones
            timeIdeal = condition.time;
            speedIdeal = condition.speed; 
            errorIdeal = condition.error; 
            razonIdeal = condition.razon; 

            //Calcular distancia total
            distanceTotal = CalculateDistanceTotal();

            //Calacular porcentaje de acierto de la distancia
            percDistance = CalculateSuccessDistance(id, ampl);

            //Calcular velocidad maxima
            speedMax = CalculateSpeedMax();

            //El movimiento inicial se identifica con el porcentaje del 5% de la velocidad maxima
            THRESHOLD_INIT_MOVEMENT = speedMax * 5 / 100.0;

            //Calcular tiempo de reaccion            
            reactionTime = CalculateReactionTime(out int start);

            //Calcular tiempo total del movimiento real
            timeTotal = CalculateTimeTotal() - reactionTime;

            //Calcula la razon de movimiento inicial, teniendo en cuenta que el
            //la fase de movimiento inicial se ha definido como el periodo desde el comienzo
            //del movimiento hasta el primer maximo local en la velocidad
            razonInicial = CalculateRazonInitial(start, out int end);

            //Calcula error inicial de movimiento
            errorInicial = CalculateErrorInitial(start, end);


            //Ausencia final de movimiento. No ha alcanzado el final de la trayectorio
            if (timeTotal + reactionTime < time)
                isCompleted = true;
            

            //Señal real de movimiento
            t_real = data.Skip(start + end).Select(p => Math.Sqrt(p.Xpr * p.Xpr + p.Ypr * p.Ypr)).ToArray(); //Distancias euclideas
            t_real = t_real.Where((punto, idp) => idp == 0 || punto != t_real[idp - 1]).ToArray(); //Filtrado de repetidos

            //Genera uno ideal
            ideal = GenerateIdealPath(id, ampl, t_real.Length);
            t_ideal = ideal.Select(p => Math.Sqrt(p[0] * p[0] + p[1] * p[1])).ToArray();


            //Calcula la componente DTW
            distanceDTW = Dtw.GetScore(t_ideal, t_real) / 1000;
            similityDTW = 1.0 / (1.0 + distanceDTW);


            //Calcula la distancia ideal total
            distanceTotalIdeal = CalculateDistanceTotalIdeal();
        }
        #endregion
        //****************************************************************************************************//
        //****************************************************************************************************//



        //*************************************Parametros Caracteristicos*************************************//
        //****************************************************************************************************//
        #region [Private Functions] Calculo de parametros caracteristicos
        /// <summary>
        /// Calcula la distancia total del trial
        /// </summary>
        /// <returns></returns>
        private double CalculateDistanceTotal()
        {
            /*double distance = 0.0f;

            //Extrae los valores del ejeX y del ejeY, y calcula el incremento
            for(int i=0; i<data.Length - 1; i++)  //Calcula la distancia total realizada
            {
                double delta =Math.Sqrt(Math.Pow(data[i + 1].Xpr - data[i].Xpr,2) + Math.Pow(data[i + 1].Ypr - data[i].Ypr, 2));
                distance += delta;                
            }*/

            double distance = data
                .Zip(data.Skip(1),(a, b) => Math.Sqrt(
                    Math.Pow(b.Xpr - a.Xpr, 2) +
                    Math.Pow(b.Ypr - a.Ypr, 2)))
                .Sum();
            return distance;
        }


        /// <summary>
        /// Calcula la distancia total considerada como acierto, que este cerca de un umbral con repecto al ideal
        /// para definir que porcentaje de trayectoria esta dentro de la parte blanca del juego. Asi, si no supera
        /// umbral, no se considera trayectoria buena (por si se ataja).            
        /// </summary>
        /// <returns></returns>
        private double CalculateSuccessDistance(int id, double ampl)
        {
            double percentage = 0;

            //Umbral para determinar si esta dentro 
            double threshold = (ampl / (game_points[1] - game_points[0])) / 2;


            if (id !=2 && id!=3) //Calcula distancia para rectas
            {
                // Definir segmento utilizado AB a partir de la trayectoria ideal            
                double[] A = { ideal.First()[0], ideal.First()[1] };
                double[] B = { ideal.Last()[0], ideal.Last()[1] };
                
                // Para cada punto real, comprobamos si alguno de los ideales está dentro del umbral
                var dentro = data
                    .Select(pReal => (DistancePointToSegment(A, B, new double[] { pReal.Xpr, pReal.Ypr }) <= threshold) ? 1 : 0).ToList();

                var dentro2 = data
                    .Select(pReal => DistancePointToSegment(A, B, new double[] { pReal.Xpr, pReal.Ypr })).ToList();
               

                percentage = (double)dentro.Sum() / (double)dentro.Count * 100;
            }
            else //Calcula distancias para circulos
            {
                // Para cada punto real, comprobamos si alguno de los ideales está dentro del umbral
                var dentro = data
                    .Select(pReal => (DistancePointToCircle(center, ampl/2, new double[] { pReal.Xpr, pReal.Ypr }) <= threshold) ? 1 : 0).ToList();

                percentage = (double)dentro.Sum() / (double)dentro.Count * 100;
            }

            //Calcular el porcentaje de los puntos que estan dentro
            return percentage;
        }

        /// <summary>
        /// Calcula la distancia total ideal del trial
        /// </summary>        
        private double CalculateDistanceTotalIdeal()
        {
            double distance = 0.0f;

            //Extrae los valores del ejeX y del ejeY, y calcula el incremento            
            for (int i = 0; i < ideal.Count - 1; i++)
            {
                double deltaIdeal = Math.Sqrt(Math.Pow(ideal[i + 1][0] - ideal[i][0], 2) + Math.Pow(ideal[i + 1][1] - ideal[i][1], 2));
                distance += deltaIdeal;
            }
            return distance;
        }        


        /// <summary>
        /// Devuelve el tiempo consumido para realizar la trayectoria completa
        /// </summary>
        /// <returns></returns>
        private double CalculateTimeTotal()
        {
            return data.Last().TimeStamp - data.First().TimeStamp;
        }


        /// <summary>
        /// Devuelve el tiempo que tarda al reaccionar
        /// </summary>
        /// <returns></returns>
        private double CalculateReactionTime(out int index)
        {
            //Filta los datos iniciales donde la velocidad es 0.
            List<double> vx = SelectAxisData(TypeData.TD_Vxr, data).ToList();
            List<double> vy = SelectAxisData(TypeData.TD_Vyr, data).ToList();

            double reactTime = -1f; //No hay reaccion
            
            //Comprueba hasta que el dispositivo empieza a moverse
            bool initReaction = false;
            index = 0;
            while( !initReaction && index < vx.Count)
            {
                double vel = Math.Sqrt(Math.Pow(vx[index], 2) + Math.Pow(vy[index], 2));
                if(vel > THRESHOLD_INIT_MOVEMENT)
                {
                    initReaction = true;
                    reactTime = data[index].TimeStamp - data.First().TimeStamp;
                }
                else
                    index++;
            }
            return reactTime;
        }


        /// <summary>
        /// Obtiene la velocidad maxima alcanzada por el movimiento del brazo
        /// </summary>
        /// <returns></returns>
        private double CalculateSpeedMax()
        {
            //Filta los datos iniciales donde la velocidad es 0.
            List<double> vx = SelectAxisData(TypeData.TD_Vxr, data).ToList();
            List<double> vy = SelectAxisData(TypeData.TD_Vyr, data).ToList();
            

            // Calcular la magnitud de la velocidad y luego obtener la máxima
            return vx.Zip(vy, (x, y) => Math.Sqrt(x * x + y * y)).Max();            
        }

        //Filtrado de la señal
        private IIRFilter filterAxis;

        /// <summary>
        /// Procesa el filtro
        /// </summary>
        private float filter(float x0, IIRFilter iirfilter)
        {
            float y = iirfilter.a0 * x0 + iirfilter.a1 * iirfilter.x1 + iirfilter.a2 * iirfilter.x2 + iirfilter.b1 * iirfilter.y1 + iirfilter.b2 * iirfilter.y2;

            iirfilter.x2 = iirfilter.x1;
            iirfilter.x1 = x0;
            iirfilter.y2 = iirfilter.y1;
            iirfilter.y1 = y;

            return y;
        }


        /// <summary>
        /// Obtiene la razon inicial del movimiento
        /// </summary>        
        private double CalculateRazonInitial(int start, out int end)
        {

            //Filta los datos iniciales donde la velocidad es 0.
            List<double> vx = SelectAxisData(TypeData.TD_Vxr, data).ToList();
            List<double> vy = SelectAxisData(TypeData.TD_Vyr, data).ToList();

            // Calcular la magnitud de la velocidad
            List<double> vel = vx.Zip(vy, (x, y) => Math.Sqrt(x * x + y * y)).ToList();


            //Selecciona solo, los datos que estan por debajo del umbral
            List<double> initial = vel.Skip(start).ToList();

            var initial_noRepeat = initial
           .Where((punto, id) => id == 0 || (punto != initial[id - 1]))
           .ToList();

            //Filtra la señal para calcular los maximos locales
            filterAxis = new IIRFilter(10, 5, 5);
            List<double> processed = new List<double>();
            for(int i=0; i< initial_noRepeat.Count; i++)
            {
                float value = (float)initial_noRepeat[i]; 
                //Filtrado del eje correspondiente
                float axisValue = filter(value, filterAxis);

                processed.Add(axisValue);
            }
            


            //Extrae los picos detectados
            double[] picos = initial_noRepeat.ToArray();

            List<double> list = new List<double>();


            //Comprueba de momento solo la primera accion - Derecha o izquierda
            for (int i = 0; i < picos.Length - 1; i++)
            {                
                if ( picos[i] > picos[i + 1] && picos[i] > picos[i - 1])                
                    list.Add(picos[i]);               
            }
            end = initial.FindIndex(x=> x == list.First());



            
            //Calcula distancia inicial de la fase de inicio hasta el indice calculado
            double distance = 0.0f;
            for (int j = start; j < start + end; j++)
            {
                double delta = Math.Sqrt(Math.Pow(data[j + 1].Xpr - data[j].Xpr, 2) + Math.Pow(data[j + 1].Ypr - data[j].Ypr, 2));
                distance += delta;
            }
            double razon = distance / distanceTotal;

            return razon;

        }


        /// <summary>
        /// Calcula el error en la direccion del movimiento inicial, desviacion en angulo entre la
        /// linea recta desde el punto central hasta la posicion del brazo despues de la fase
        /// de movimiento
        /// </summary>
        /// <returns></returns>
        private double CalculateErrorInitial(int start, int end)
        {                       
            //Regista dos vectores A(x,y) - real  y B(x,y) - ideal
            double Ax = data[end + start].Xpr - data[start].Xpr;
            double Ay = data[end + start].Ypr - data[start].Ypr;

            double Bx = ideal[end + start][0] - ideal[start][0];
            double By = ideal[end + start][1] - ideal[start][1];


            //Calcula el producto escalar para determinar el angulo entre la ideal y la calculada
            double escalar = Ax * Bx + Ay * By;
            double moduleReal = Math.Sqrt(Math.Pow(Ax, 2) + Math.Pow(Ay, 2));
            double moduleIdeal = Math.Sqrt(Math.Pow(Bx, 2) + Math.Pow(By, 2));


            //Extraer angulo
            double angle = Math.Acos(escalar / (moduleIdeal * moduleReal)) * (180.0 / Math.PI);
            return angle;
        }
        #endregion

        #region [Private Functions] Calculo de distancias entre puntos y segmentos 
        /// <summary>
        /// Calcula la distancia minima entre un punto P y un segmento AB utilizando la
        /// parametrizacion y el clamping
        /// </summary>
        /// <returns></returns>
        private double DistancePointToSegment(double[] A, double[] B, double[] P)
        {
            //1.Parametriza el segmento S(t) = A + t(B-A), t E [0,1]

            //Vector AB 
            double vx_AB = B[0] - A[0];
            double vy_AB = B[1] - A[1];

            //Vector AP
            double vx_AP = P[0] - A[0];
            double vy_AP = P[1] - A[1];


            //2. Proyectar P sobre la recta infinita que pasa por A y B usando 
            //la proyeccion escalar: t* = (AP · AB) / (AB · AB) 
            double dot = vx_AB * vx_AP + vy_AB * vy_AP; //Producto escalar
            double len2 = vx_AB * vx_AB + vy_AB * vy_AB; //Longitud

            //Se obtiene el parametro t
            double tStar = (len2 > 0) ? (dot / len2) : 0.0;

            //3. Clampea t a [0,1] para que la proyeccion caiga dentro del segmento
            double t = Math.Max(0, Math.Min(1, tStar));

            //4. El punto más cercano en el segmento es Q = A + t(B−A),
            double qx = A[0] + t * vx_AB;
            double qy = A[1] + t * vy_AB;

            //5. Calcular la distancia entre el punto P y el Q
            double dx = P[0] - qx;
            double dy = P[1] - qy;
            return Math.Sqrt(dx * dx + dy * dy);
        }

        /// <summary>
        /// Calcula la distancia minima entre un punto P y una circuferencia de
        /// centro R y radio r
        /// </summary>        
        /// <returns></returns>
        private double DistancePointToCircle(double[] C, double r, double[] P)
        {
            //1.Calcula la distancia euclidea desde el punto P hasta el centro            
            double dx = P[0] - C[0];
            double dy = P[1] - C[1];
            double dCentro = Math.Sqrt(dx * dx + dy * dy);

            //2. Calcula la distancia a la circunferencia            
            return Math.Abs(dCentro - r);
        }
        #endregion
        //****************************************************************************************************//
        //****************************************************************************************************//



        //****************************************Trayectorias ideales****************************************//
        //****************************************************************************************************//
        #region [Private Functions] Calculo de trayectorias ideales
        /// <summary>
        /// Metodo encargado de generar las trayectorias ideales a partir de la configuracion
        /// de los path clinicos realizados en el juego ClinicalScaleMeter
        /// </summary>
        /// <returns></returns>
        private List<double[]> GenerateIdealPath(int id, float ampl, int sample)
        {
            //Cada camino es diferente, por lo que se generaran a partir de una serie de ecuacion de recta y circulares
            List<double[]> list = new List<double[]>();

            //El primer camino es una recta. (izquierda a derecha)
            if (id == 0)
                list = GetArrayRectX(-ampl / 2, ampl / 2, sample);
            else if (id == 1)//Segundo camino (recta de derecha a izquierda)                           
                list = GetArrayRectX(ampl / 2, -ampl / 2, sample);  //Se junta y se añade            
            else if (id == 2) //Tercera trayectoria circular                            
                list = GetArrayCircle(ampl / 2, 225.0f, 0.0f, sample);
            else if (id == 3) //Cuarta trayectoria circular
                list = GetArrayCircle(ampl / 2, 315.0f, 270f + 315f, sample);
            else if (id == 4) //Quinta trayectoria recta (bajo a arriba)
                list = GetArrayRectY(0, ampl / 2, sample);
            else if (id == 5) //Sexta trayectoria recta (arriba a abajo)
                list = GetArrayRectY(ampl / 2, 0, sample);
            else if (id == 6) //Septima trayectoria recta (arriba a abajo)
                list = GetArrayRectY(0, -ampl / 2, sample);
            else if (id == 7) //Octava trayectoria recta (abajo a arriba)
                list = GetArrayRectY(-ampl / 2, ampl / 2, sample);
            else if (id == 8)//Novena trayectoria recta (arriba a abjo)
                list = GetArrayRectY(ampl / 2, -ampl / 2, sample);

            return list;
        }

        private List<double[]> GetArrayRectX(float init, float end, int sample)
        {
            double incr = (end - init) / (sample - 1);
            double[] x = Enumerable.Range(0, sample).Select(i => init + i * incr).ToArray();
            double[] y = Enumerable.Repeat(center[1], sample).ToArray();

            List<double[]> tmpList = x.Zip(y, (a, b) => new double[] { a, b }).ToList(); //Se junta y se añade
            return tmpList;
        }

        private List<double[]> GetArrayRectY(float init, float end, int sample)
        {
            double incr = (end - init) / (sample - 1);
            double[] x = Enumerable.Repeat(center[0], sample).ToArray();
            double[] y = Enumerable.Range(0, sample).Select(i => center[1] + (init + i * incr)).ToArray();

            List<double[]> tmpList = x.Zip(y, (a, b) => new double[] { a, b }).ToList(); //Se junta y se añade
            return tmpList;
        }

        /// <summary>
        /// Genera una trayectoria circular añadiendo angulo inicial y final
        /// </summary>                
        private List<double[]> GetArrayCircle(float radio, float init, float end, int sample)
        {
            double incr = ((end - init) / (sample - 1)) * (Math.PI / 180.0f);
            double initAngle = init * Math.PI / 180.0f;

            double[] x = Enumerable.Range(0, sample).Select(i => center[0] + radio * Math.Cos(initAngle + i * incr)).ToArray();
            double[] y = Enumerable.Range(0, sample).Select(i => center[1] + radio * Math.Sin(initAngle + i * incr)).ToArray();

            List<double[]> tmpList = x.Zip(y, (a, b) => new double[] { a, b }).ToList(); //Se junta y se añade
            return tmpList;
        }
        #endregion
        //****************************************************************************************************//
        //****************************************************************************************************//



        //*************************************Arrays de datos registrados************************************//
        //****************************************************************************************************//
        #region [Private Functions] Extraccion de array de datos
        private static IEnumerable<double> SelectAxisData(TypeData type, DataRobot[] datarobot)
        {

            IEnumerable<double> axisXData = null;

            try
            {
                //Generar vector con los datos
                switch (type)
                {
                    case TypeData.TD_TimeStamp:
                        axisXData = datarobot.Select(x => x.TimeStamp); break;
                    case TypeData.TD_Xpr: //Eje X filtrado a taskstate
                        axisXData = datarobot.Select(x => x.Xpr).ToList(); //Filtrado de taskstate
                        break;
                    case TypeData.TD_Ypr:
                        axisXData = datarobot.Select(x => x.Ypr).ToList(); //Filtrado de taskstate                        
                        break;
                    case TypeData.TD_Vxr:
                        axisXData = datarobot.Select(x => x.Vxr); break;
                    case TypeData.TD_Vyr:
                        axisXData = datarobot.Select(x => x.Vyr); break;
                    case TypeData.TD_Fxr:
                        axisXData = datarobot.Select(x => x.Fxr); break;
                    case TypeData.TD_Fyr:
                        axisXData = datarobot.Select(x => x.Fyr); break;
                    case TypeData.TD_EndeffAngle:
                        axisXData = datarobot.Select(x => x.EndeffAngle); break;
                    case TypeData.TD_RobotActivated:
                        axisXData = datarobot.Select(x => x.RobotActivated); break;
                    case TypeData.TD_RightArmSat:
                        axisXData = datarobot.Select(x => x.RightArmSat); break;
                    case TypeData.TD_LeftArmSat:
                        axisXData = datarobot.Select(x => x.LeftArmSat); break;
                    case TypeData.TD_Pulsed:
                        axisXData = datarobot.Select(x => x.Pulsed); break;
                    case TypeData.TD_NivelAsistencia:
                        axisXData = datarobot.Select(x => x.NivelAsistencia); break;
                    case TypeData.TD_Fuerza:
                        axisXData = datarobot.Select(x => x.Fuerza); break;
                    case TypeData.TD_TiempoMax:
                        axisXData = datarobot.Select(x => x.TiempoMax); break;
                    case TypeData.TD_Cx:
                        axisXData = datarobot.Select(x => x.Cx); break;
                    case TypeData.TD_Cy:
                        axisXData = datarobot.Select(x => x.Cy); break;
                    case TypeData.TD_GameState:
                        axisXData = datarobot.Select(x => x.GameState); break;
                    case TypeData.TD_TaskState:
                        axisXData = datarobot.Select(x => x.TaskState); break;
                    case TypeData.TD_Xpr0:
                        axisXData = datarobot.Select(x => x.Xpr0); break;
                    case TypeData.TD_Ypr0:
                        axisXData = datarobot.Select(x => x.Ypr0); break;
                    case TypeData.TD_XprF:
                        axisXData = datarobot.Select(x => x.XprF); break;
                    case TypeData.TD_YprF:
                        axisXData = datarobot.Select(x => x.YprF); break;
                    default:
                        axisXData = datarobot.Select(x => x.TimeStamp); break;
                }
            }
            catch (Exception)
            {
               
            }
            return axisXData;
        }        

        public double[] GetDTWReal() => t_real;

        public double[] GetDTWIdeal() => t_ideal;

        public List<double[]> GetIdealPath() => ideal;
        #endregion
        //****************************************************************************************************//
        //****************************************************************************************************//



        //*******************************************Escala clinicna******************************************//
        //****************************************************************************************************//
        #region [Private Functions] Calculo la ponderacion de la escala clinica
        /// <summary>
        /// Contruye el indice clinico ponderado para medir la similitud de la trayectoria con 
        /// respecto a la ideal
        /// </summary>
        /// <returns></returns>
        public double Score()
        {
            //Indice de distancia total                        
            double distanceScore = Normalize(distanceTotal, distanceTotalIdeal, distanceTotalIdeal * 2);

            //Indice del tiempo de reaccion                        
            double reactionScore = Normalize(reactionTime, 0, 1.5);

            //Indice del tiempo de trayectoria            
            double timeScore = Normalize(timeTotal, timeIdeal, 10);

            //Indice de velocidad maxima            
            double speedScore = Normalize(speedMax, speedIdeal, 0);

            //Indice de trayectoria completada.
            double completedScore = isCompleted ? 1 : 0;
            
            //Indice de error inicial            
            double errorScore = Normalize(errorInicial, 0, errorIdeal);

            //Indice de razon de movimiento inicial            
            double razonScore = Normalize(razonInicial, 0, razonIdeal);

            //Indice del DTW
            double dtwScore = similityDTW;

            //Calcula la escla clinica ponderada
            double clinicalScore =
                distanceScore * ponderations[0] +
                reactionScore * ponderations[1] +
                timeScore     * ponderations[2] +
                speedScore    * ponderations[3] +
                completedScore* ponderations[4] +
                errorScore    * ponderations[5] +
                razonScore    * ponderations[6] +
                dtwScore      * ponderations[7];  

            return clinicalScore;
        }       

        private double Normalize(double value, double max, double min)
        {
            //Conseguir puntuacion a partir de la funcion de primer grado y=a*x+b
            double a = 1 / (max - min);
            double b = 1 - a * max;
            double score = a * value + b;

            //Filtra resultados
            if (score > 1)
                score = 1;
            else if(score < 0)
                score = 0;           

            return score;
        }
        #endregion
        //****************************************************************************************************//
        //****************************************************************************************************//
    }

    /// <summary>
    /// Estructura contenedora de las condiciones para completar
    /// correctamente un trial
    /// </summary>
    public struct TrialCondition
    {        
        public double time;
        public double speed;
        public double error;       
        public double razon;

        public TrialCondition (double t, double s, double e, double r)
        {            
            time = t;
            speed = s;
            error = e;
            razon = r;
        }
    }

    public struct TrialConditions
    {
        public double[] time; //[Valor maxima puntuacion 1; Valor minima puntuacion0] 
        public double[] speed;
        public double[] error;
        public double[] razon;

        public TrialConditions(double[] t, double[] s, double[] e, double[] r)
        {
            time = t;
            speed = s;
            error = e;
            razon = r;
        }
    }
}
