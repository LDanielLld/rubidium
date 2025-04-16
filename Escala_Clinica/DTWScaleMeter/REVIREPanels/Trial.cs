using FastDtw.CSharp;
using REVIREPanels.Componentes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace REVIREPanels
{
    class Trial
    {
        public double distanceTotal = 0.0f;
        public double timeTotal = 0.0f;
        public double reactionTime = 0.0f;
        public double speedMax = 0.0f;
        public double errorInicial = 0.0f;
        public double razonInicial = 0.0f;
        public double similityDTW = 0.0f;
        public double distanceDTW = 0.0f;



        public bool isCompleted = false;

        private const double THRESHOLD_INIT_MOVEMENT = 0.01;

        private DataRobot[] data; //Trayectoria real
        private List<float[]> ideal; //Trayectoria ideal


        public Trial(DataRobot[] dataIn, List<float[]> idealIn, float time, float ampl)
        {
            //Copia los datos recibidos
            data = dataIn.ToArray();
            ideal = idealIn.ToList();

            //Calcular distancia total
            distanceTotal = CalculateDistanceTotal();

            //Calcular tiempo de reaccion            
            reactionTime = CalculateReactionTime();

            //Calcular tiempo total del movimiento real
            timeTotal = CalculateTimeTotal() - reactionTime;

            //Calcular velocidad maxima
            speedMax = CalculateSpeedMax();

            //Ausencia final de movimiento. No ha alcanzado el final de la trayectorio
            if (timeTotal + reactionTime < time)
                isCompleted = true;

            //Calcula error inicial de movimiento
            CalculateErrorInitial(out errorInicial, out razonInicial);


            //Calcular DTW, distancia y similitud
           /* var dtwMatrix = ComputeDTW(ideal, data);
            float dtwDistance = dtwMatrix[ideal.Count - 1, data.Length - 1];

            //Cuanto más cercana a 1, más parecidas son las trayectorias.
            float maxLength = Math.Max(ideal.Count, data.Length);
            float similarity = 1f / (1f + dtwDistance / maxLength); // Da un valor entre 0 y 1
            */

            //Preparo los dos vectores             
            double[] t_ideal = ideal
            .Select(p => (Math.Sqrt(p[0] * p[0] + p[1] * p[1]))/1000)
            .ToArray();

            var sinRepetidos = data
                .Where((punto, id) => id == 0 || !PuntosIguales(punto, data[id - 1]))
                .ToList();

            //Quita los inferiores al tiempo de reaccion
            var filtradoreact = sinRepetidos.SkipWhile(p => Math.Sqrt(Math.Pow(p.Vxr, 2) + Math.Pow(p.Vyr, 2)) < THRESHOLD_INIT_MOVEMENT).ToList();

            //Quita repetidos
            double[] t_real = filtradoreact
                .Select(p => (Math.Sqrt(p.Xpr * p.Xpr + p.Ypr * p.Ypr))/1000)
                .ToArray();

            


            distanceDTW = Dtw.GetScore(t_real, t_ideal);
            similityDTW = 1.0 / (1.0 + distanceDTW);

        }

        bool PuntosIguales(DataRobot a, DataRobot b)
        {
            return a.Xpr == b.Xpr && a.Ypr == b.Ypr;
        }


        /// <summary>
        /// Calcula la distancia total del trial
        /// </summary>
        /// <returns></returns>
        private double CalculateDistanceTotal()
        {
            double distance = 0.0f;

            //Extrae los valores del ejeX y del ejeY, y calcula el incremento
            for(int i=0; i<data.Length - 1; i++)
            {
                double delta =Math.Sqrt( Math.Pow(data[i + 1].Xpr - data[i].Xpr,2) + Math.Pow(data[i + 1].Ypr - data[i].Ypr, 2));
                distance += delta;
            }
            return distance;
        }


        /// <summary>
        /// Devuelve el tiempo consumido para realizar la trayectoria completa
        /// </summary>
        /// <returns></returns>
        public double CalculateTimeTotal()
        {
            return data.Last().TimeStamp - data.First().TimeStamp;
        }


        /// <summary>
        /// Devuelve el tiempo que tarda al reaccionar
        /// </summary>
        /// <returns></returns>
        public double CalculateReactionTime()
        {
            //Filta los datos iniciales donde la velocidad es 0.
            List<double> vx = SelectAxisData(TypeData.TD_Vxr, data).ToList();
            List<double> vy = SelectAxisData(TypeData.TD_Vyr, data).ToList();

            double reactTime = -1f; //No hay reaccion

            //Comprueba hasta que el dispositivo empieza a moverse
            bool initReaction = false;
            int i = 0;
            while( !initReaction && i < vx.Count)
            {
                double vel = Math.Sqrt(Math.Pow(vx[i], 2) + Math.Pow(vy[i], 2));
                if(vel > THRESHOLD_INIT_MOVEMENT)
                {
                    initReaction = true;
                    reactTime = data[i].TimeStamp - data.First().TimeStamp;
                }
                i++;
            }
            return reactTime;
        }


        /// <summary>
        /// Obtiene la velocidad maxima alcanzada por el movimiento del brazo
        /// </summary>
        /// <returns></returns>
        public double CalculateSpeedMax()
        {
            //Filta los datos iniciales donde la velocidad es 0.
            List<double> vx = SelectAxisData(TypeData.TD_Vxr, data).ToList();
            List<double> vy = SelectAxisData(TypeData.TD_Vyr, data).ToList();

            // Calcular la magnitud de la velocidad y luego obtener la máxima
            return vx.Zip(vy, (x, y) => Math.Sqrt(x * x + y * y)).Max();            
        }


        /// <summary>
        /// Calcula el error en la direccion del movimiento inicial, desviacion en angulo entre la
        /// linea recta desde el punto central hasta la posicion del brazo despues de la fase
        /// de movimiento
        /// </summary>
        /// <returns></returns>
        public void CalculateErrorInitial(out double angle, out double razon)
        {
            //Registrar dataRobot seleccionados
            List<DataRobot> tmp = new List<DataRobot>();

            
            double total = 0.0f;
            double oldtotal = 0.0f;
            int index = -1; //Indice registrado

            //Calcula pendientes
            double[] pendientes = new double[data.Length - 1];
            bool isCalculate = false;

            int i = 0;
            while (!isCalculate && i<data.Length - 1)
            {
                double dy = data[i + 1].Ypr - data[i].Ypr;
                double dx = data[i + 1].Xpr - data[i].Xpr;


                if (dx != 0)
                {
                    double m = dy / dx;
                    pendientes[i] = m;
                    tmp.Add(data[i]);

                    total += m;

                    if (Math.Abs(total) < Math.Abs(oldtotal))
                    {
                        isCalculate = true;
                        index = i;
                    }
                    
                    oldtotal = total;                    
                }

                i++;
            }


            //Regista dos vectores A(x,y) - real  y B(x,y) - ideal
            double Ax = data[index].Xpr - data.First().Xpr;
            double Ay = data[index].Ypr - data.First().Ypr;

            double Bx = ideal.Last()[0] - ideal.First()[0];
            double By = ideal.Last()[1] - ideal.First()[1];


            //Calcula el producto escalar para determinar el angulo entre la ideal y la calculada
            double escalar = Ax * Bx + Ay * By;
            double moduleReal = Math.Sqrt(Math.Pow(Ax, 2) + Math.Pow(Ay, 2));
            double moduleIdeal = Math.Sqrt(Math.Pow(Bx, 2) + Math.Pow(By, 2));


            //Extraer angulo
            angle = Math.Acos(escalar/(moduleIdeal * moduleReal)) * (180.0 / Math.PI);


            //Extraer razon de movimiento inicial

            //Calcula distancia inicial desde el principio hasta el indice calculado
            double distance = 0.0f;
            for (int j = 0; j < index; j++)
            {
                double delta = Math.Sqrt(Math.Pow(data[j + 1].Xpr - data[j].Xpr, 2) + Math.Pow(data[j + 1].Ypr - data[j].Ypr, 2));
                distance += delta;
            }           

            razon = distance/distanceTotal;
            
        }



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
                        double[] result = datarobot.Where(x => x.TaskState == 2).Select(x => x.Xpr).ToArray(); //Filtrado de taskstate
                        axisXData = result.Where((val, index) => index == 0 || val != result[index - 1]);//Filtrado de repetidos

                        break;
                    case TypeData.TD_Ypr:
                        double[] result2 = datarobot.Where(x => x.TaskState == 2).Select(x => x.Ypr).ToArray(); //Filtrado de taskstate
                        axisXData = result2.Where((val, index) => index == 0 || val != result2[index - 1]);//Filtrado de repetidos
                        break;
                    case TypeData.TD_Vxr:
                        /* double[] result3 = datarobot.Where(x => x.TaskState == 2).Select(x => x.Vxr).ToArray(); //Filtrado de taskstate
                         axisXData = result3.Where((val, index) => index == 0 || val != result3[index - 1]);//Filtrado de repetidos
                         */
                        axisXData = datarobot.Select(x => x.Vxr); break;
                    case TypeData.TD_Vyr:

                        /*double[] result4 = datarobot.Where(x => x.TaskState == 2).Select(x => x.Vyr).ToArray(); //Filtrado de taskstate
                        axisXData = result4.Where((val, index) => index == 0 || val != result4[index - 1]);//Filtrado de repetidos
                        */
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


        float[,] ComputeDTW(List<float[]> ideal, DataRobot[] real)
        {
            int n = ideal.Count;
            int m = real.Length;
            float[,] dtw = new float[n, m];

            for (int i = 0; i < n; i++)
                for (int j = 0; j < m; j++)
                    dtw[i, j] = float.PositiveInfinity;

            dtw[0, 0] = EuclideanDistance(ideal[0], real[0]);

            for (int i = 0; i < n; i++)
            {
                for (int j = 0; j < m; j++)
                {
                    float cost = EuclideanDistance(ideal[i], real[j]);
                    if (i > 0) cost += dtw[i - 1, j];
                    if (j > 0) cost = Math.Min(cost, dtw[i, j - 1] + EuclideanDistance(ideal[i], real[j]));
                    if (i > 0 && j > 0) cost = Math.Min(cost, dtw[i - 1, j - 1] + EuclideanDistance(ideal[i], real[j]));
                    dtw[i, j] = cost;
                }
            }

            return dtw;
        }

        float EuclideanDistance(float[] p1, DataRobot p2)
        {
            return (float)Math.Sqrt(Math.Pow(p1[0] - p2.Xpr, 2) + Math.Pow(p1[1] - p2.Ypr, 2));
        }

    }
}
