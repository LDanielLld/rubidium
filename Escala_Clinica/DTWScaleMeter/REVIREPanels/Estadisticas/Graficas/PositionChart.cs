using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;
using System.Windows.Forms;
using System.Windows.Forms.DataVisualization.Charting;
using System.Numerics;
using System.Drawing.Drawing2D;
using System.Threading;
using REVIREPanels.Componentes;
using System.Data;

namespace REVIREPanels.Graficas
{
     
    class PositionChart
    {
        #region Atributos
        private double valueXMaxChart; //Valor maximo que alcanza la gráfica
        private double valueXMinChart; //Valor minimo que alcanza la gráfica
        private double valueYMaxChart; //Valor maximo que alcanza la gráfica
        private double valueYMinChart; //Valor minimo que alcanza la gráfica
        private double valueXMidChart; //Valor medio eje X
        private double valueYMidChart; //Valor medio eje Y

        public double GetXMaxChart() { return valueYMaxChart; }
        public double GetYMinChart() { return valueYMinChart; }
        public double GetXMidChart() { return valueXMidChart; }
        public double GetYMidChart() { return valueYMidChart; }

        //Punto central de movimiento
        private Vector2 centralPoint;
        private bool crossCenter; //Indica si los ejes deben cruzar el centro

        //Chart principal
        private Chart chart;        

        
        float distInsideZone; //Distancia localizada dentro del area
        float distanceTrial; //Distancia total recorrida por la grafica

        //Umbral para que se vea mas parte de la grafica
        private double threshold;

        //Guardar polingonos para pintar
        List<PointF[]> poligons = new List<PointF[]>();
        List<PointF[]> cpoligons = new List<PointF[]>(); //Coeficientes de la ecuacion de los lados
        List<PointF[]> circles = new List<PointF[]>();
        float widthZone = 1.0f; //cm - radio

        private const int nXLines = 10; //Numero de incrementos en el eje X
        private const int nYLines = 10; //Numero de incrementos en el eje X
        private const float margin = 1.0f; //Margen de la grafica

        private bool isDrawTargetZone = false; //Por defecto no se debe pintar la zona objetivo
        private bool isDrawTargetCircle = false; //Por defecto no se debe pintar el circulo objetivo

      

        #endregion

        //*******************************Inicialización*********************************//
        //******************************************************************************//
        #region Inicializacion
        public PositionChart(Chart inChart, double trhold)
        {
            //Inicializar componente
            chart = inChart;
           

            //Umbral
            threshold = trhold;

            //Inicializa la gráfica            
            InitChart();
        }

        private void InitChart()
        {
            // Grafica inicial                
            chart.AntiAliasing = AntiAliasingStyles.All; // Set Antialiasing mode            
            chart.TextAntiAliasingQuality = TextAntiAliasingQuality.High;
            chart.ChartAreas[0].IsSameFontSizeForAllAxes = true;

            

            //Borde del area            
            chart.ChartAreas[0].BorderColor = Color.FromArgb(150, 10, 10, 10); //Color
            chart.ChartAreas[0].BorderWidth = 1; //Anchura
            chart.ChartAreas[0].BorderDashStyle = ChartDashStyle.Solid; //Estilo

            //Handle paint
            chart.Paint += new System.Windows.Forms.PaintEventHandler(Chart_Paint);

            //*********Configurar Zoom y cursor seleccionabel******//
            //Habilita la utlizacion del cursor
            chart.ChartAreas[0].CursorX.IsUserEnabled = true;
            chart.ChartAreas[0].CursorX.IsUserSelectionEnabled = true;
           // chart.ChartAreas[0].CursorX.SelectionColor = SystemColors.Highlight;
            chart.ChartAreas[0].CursorY.IsUserEnabled = true;
            chart.ChartAreas[0].CursorY.IsUserSelectionEnabled = true;
           // chart.ChartAreas[0].CursorY.SelectionColor = SystemColors.Highlight;

            chart.ChartAreas[0].CursorX.Interval = 0;  //Para que se pueda seleccionar el movimiento exacto del cursor          
            chart.ChartAreas[0].CursorY.Interval = 0;
            

            //Tamaño minimo del zoom
            chart.ChartAreas[0].AxisX.ScaleView.MinSize = 1;
            chart.ChartAreas[0].AxisY.ScaleView.MinSize = 1;

            //Permitir que haga el autozoom pero no el autoscroll, para que no se desplace la grafica
            chart.ChartAreas[0].AxisX.ScaleView.Zoomable = true; 
            chart.ChartAreas[0].AxisY.ScaleView.Zoomable = true;
            chart.ChartAreas[0].CursorX.AutoScroll = false;// AutoScrollCursor.Checked;
            chart.ChartAreas[0].CursorY.AutoScroll = false;// AutoScrollCursor.Checked;

            chart.ChartAreas[0].AxisX.ScrollBar.IsPositionedInside = false;
            chart.ChartAreas[0].AxisY.ScrollBar.IsPositionedInside = false;
            chart.ChartAreas[0].AxisX.ScaleView.SmallScrollSize = 0.1;
            chart.ChartAreas[0].AxisY.ScaleView.SmallScrollSize = 0.1;

            //Evento del scroll
            chart.AxisScrollBarClicked += new EventHandler<ScrollBarEventArgs>(Chart_AxisScrollBarClicked);
            chart.AxisViewChanged += new EventHandler<ViewEventArgs>(this.Chart_AxisViewChanged);

            

        }
        #endregion

        #region Configuracion Vista
        // <summary>
        /// Configura la vista de la grafica de posicion
        /// </summary>
        /// <param name="initPos"></param>
        /// <param name="targetPos"></param>
        /// <param name="distancia">Distancia total del movimiento</param>
        public void ConfigureChartView(Vector2[] initPos, Vector2[] targetPos)
        {

            //Establecer valores maximos y minimos de la grafica, así como
            //los puntos objetivos            
            IEnumerable<float> axisXDataT = targetPos.Select(x => x.X);
            IEnumerable<float> axisYDataT = targetPos.Select(y => y.Y);

            IEnumerable<float> axisXDataI = targetPos.Select(x => x.X);
            IEnumerable<float> axisYDataI = targetPos.Select(y => y.Y);

            //Registra valores maximo de ambos tipos de objetivos
            float[] valuesXMax = new float[2] { axisXDataI.Max(), axisXDataT.Max() };
            float[] valuesXMin = new float[2] { axisXDataI.Min(), axisXDataT.Min() };
            float[] valuesYMax = new float[2] { axisYDataI.Max(), axisYDataT.Max() };
            float[] valuesYMin = new float[2] { axisYDataI.Min(), axisYDataT.Min() };

            //Primero, calcular los limites de la grafica
            valueXMaxChart = valuesXMax.Max();
            valueXMinChart = valuesXMin.Min();

            valueYMaxChart = valuesYMax.Max();
            valueYMinChart = valuesYMin.Min();

            //Valores medio de los ejes de posicion
            valueXMidChart = (valueXMaxChart + valueXMinChart) / 2.0;
            valueYMidChart = (valueYMaxChart + valueYMinChart) / 2.0; //Centro del rubidium

            ConfigureAxis(true);

            chart.Invalidate();
        }

        /// <summary>
        /// Configura punto de vista para la trayectoria
        /// </summary>
        /// <param name="center"></param>
        /// <param name="distancia"></param>
        public void ConfigureChartView(Vector2 center, float distancia)
        {
            //Primero, calcular los limites de la grafica
            valueXMaxChart = /*center.X +*/ distancia / 2.0f;
            valueXMinChart = /*center.X*/ -distancia / 2.0f;

            valueYMaxChart = /*center.Y*/ +distancia / 2.0f;
            valueYMinChart = /*center.Y */-distancia / 2.0f;

            //Valores medio de los ejes de posicion
            valueXMidChart = 0;// (valueXMaxChart + valueXMinChart) / 2.0;
            valueYMidChart = 0;// (valueYMaxChart + valueYMinChart) / 2.0; //Centro del rubidium

            ConfigureAxis(true);

            //Punto central
            centralPoint = center;

            chart.Invalidate();
        }

        /// <summary>
        /// Configura punto de vista para la posicion x o y
        /// </summary>
        /// <param name="center"></param>
        /// <param name="distancia"></param>
        public void ConfigureChartView(Vector2 center, double xData, float distancia)
        {
            //Primero, calcular los limites de la grafica
            valueXMaxChart = xData;
            valueXMinChart = 0;

            valueYMaxChart = distancia / 2;
            valueYMinChart = -distancia / 2;

            ConfigureAxis(false);

            //Punto central
            centralPoint = center;

            chart.Invalidate();
        }


        public void ConfigureAxis(bool cross)
        {

            //Configurar Axes de la grafica            
            chart.ChartAreas[0].AxisX.Interval = (valueXMaxChart - valueXMinChart) / (double)nXLines;
            chart.ChartAreas[0].AxisX.IntervalType = DateTimeIntervalType.Number;
            chart.ChartAreas[0].AxisX.IntervalOffset = margin;
            chart.ChartAreas[0].AxisX.IntervalOffsetType = DateTimeIntervalType.Number;
            chart.ChartAreas[0].AxisY.Interval = (valueYMaxChart - valueYMinChart) / (double)nYLines;
            chart.ChartAreas[0].AxisY.IntervalType = DateTimeIntervalType.Number;
            chart.ChartAreas[0].AxisY.IntervalOffset = margin;
            chart.ChartAreas[0].AxisY.IntervalOffsetType = DateTimeIntervalType.Number;

            chart.ChartAreas[0].AxisX.Minimum = valueXMinChart - margin;
            chart.ChartAreas[0].AxisX.Maximum = valueXMaxChart + margin;
            chart.ChartAreas[0].AxisY.Minimum = valueYMinChart - margin;
            chart.ChartAreas[0].AxisY.Maximum = valueYMaxChart + margin;

            // Enable all elements
            chart.ChartAreas[0].AxisX.MajorGrid.Enabled = true;
            chart.ChartAreas[0].AxisX.MajorGrid.LineDashStyle = ChartDashStyle.Dash;
            chart.ChartAreas[0].AxisX.MajorGrid.LineColor = Color.Gray;
            chart.ChartAreas[0].AxisX.MajorTickMark.Enabled = true;
            chart.ChartAreas[0].AxisX.MinorGrid.Enabled = false;
            chart.ChartAreas[0].AxisX.MinorTickMark.Enabled = false;
            // chartArea.AxisX.MinorTickMark.Interval = valueXMaxChart/10;

            chart.ChartAreas[0].AxisY.MajorGrid.Enabled = true;
            chart.ChartAreas[0].AxisY.MajorGrid.LineDashStyle = ChartDashStyle.Dash;
            chart.ChartAreas[0].AxisY.MajorGrid.LineColor = Color.Gray;
            chart.ChartAreas[0].AxisY.MajorTickMark.Enabled = true;
            chart.ChartAreas[0].AxisY.MinorGrid.Enabled = false;
            chart.ChartAreas[0].AxisY.MinorTickMark.Enabled = false;
            // chart.ChartAreas[0].AxisY.MinorTickMark.Interval = valueYMaxChart / 10;

            //Cruza los ejes
            crossCenter = cross;
            if (crossCenter)
            {
                chart.ChartAreas[0].AxisX.Crossing = valueXMidChart; //Valor medio
                chart.ChartAreas[0].AxisY.Crossing = valueYMidChart; //Valor medio
                chart.ChartAreas[0].AxisX.IsMarksNextToAxis = true;

                // Position the Y axis labels and tick marks next to the axis
                chart.ChartAreas[0].AxisY.IsMarksNextToAxis = true;
            }

            //Estilo de las etoquetas
            chart.ChartAreas[0].AxisX.LabelStyle.IntervalType = DateTimeIntervalType.Number;
            chart.ChartAreas[0].AxisX.LabelStyle.Format = "0";
            chart.ChartAreas[0].AxisY.LabelStyle.IntervalType = DateTimeIntervalType.Number;
            chart.ChartAreas[0].AxisY.LabelStyle.Format = "0";

            //Margen en los ejes
            chart.ChartAreas[0].AxisX.IsMarginVisible = true;
            chart.ChartAreas[0].AxisY.IsMarginVisible = true;

        }
        #endregion
        //******************************************************************************//
        //******************************************************************************//


        //****************************Visualización de datos****************************//
        //******************************************************************************//
        #region Trayectorias ideales
        public void SetIdealTrajectory(IEnumerable<double> axisXData, IEnumerable<double> axisYData, Vector2 origin)
        {
            //Metodo para mostrar las trajectorias ideales                     

            //*****Configurar la muestra de trayectorias ideales*****//
            Series serieIdeal = new Series("SeriesIdeal");
            serieIdeal.ChartType = SeriesChartType.Line;
            serieIdeal.Color = Color.FromArgb(50, 0, 255, 0);
            serieIdeal.BorderWidth = 3;
            chart.Series.Add(serieIdeal);

            //El punto principal, hacia donde convergen todos los demas puntos    
            List<Vector2> points = new List<Vector2>();
            points.Add(origin);

            //Añadir trayectoria            
            for (int i = 0; i < axisXData.Count(); i++)
            {
                Vector2 current = new Vector2((float)axisXData.ElementAt(i) * 100 - centralPoint.X, (float)axisYData.ElementAt(i) * 100 - centralPoint.Y);

                if (!points.Contains(current))
                {
                    //Ida y vuelta
                    serieIdeal.Points.AddXY(origin.X, origin.Y);
                    serieIdeal.Points.AddXY(current.X, current.Y);
                    serieIdeal.Points.AddXY(origin.X, origin.Y);

                    //Lo añade a la lista de puntos pintados
                    points.Add(current);
                }
            }
            points.Clear();
            chart.Invalidate();
        }        
        #endregion

        #region Objetivos

        #endregion
        //******************************************************************************//
        //******************************************************************************//

        //********************************Pintar grafica********************************//
        //******************************************************************************//
        void Chart_Paint(object sender, PaintEventArgs e)
        {
            //Pintar zona objetivo si hace falta
            if (isDrawTargetZone)
            {
                DrawTargetZone(e.Graphics);
            }

            //Pintar circulos ojetivos objetivos
            if (isDrawTargetCircle)
            {
                DrawTargetCircle(e.Graphics);
            }
        }


        public void Dispose()
        {
            chart.Paint -= Chart_Paint;
            chart.AxisScrollBarClicked -= Chart_AxisScrollBarClicked;
            chart.AxisViewChanged -= Chart_AxisViewChanged;
        }

        

        
        /// <summary>
        /// Calcula la posicion y forma de las zonas objetivo
        /// </summary>
        /// <param name="events">Eventos actuales</param>        
        public void DrawTargets(IEnumerable<double> xorigin, IEnumerable<double> yorigin, IEnumerable<double> xtarget, IEnumerable<double> ytarget)
        {
            //Extraer la direccion de los trials para dibujar las zonas
           /* List<TrialPacket> trials = events.Trials;
            List<Vector2> targets = events.Target;
            List<Vector2> init_target = events.PosInit;*/

            List<string> code_zones = new List<string>();

            //El punto principal, hacia donde convergen todos los demas puntos    
            List<Vector2> pointsOrigin = new List<Vector2>();
            List<Vector2> pointsTarget = new List<Vector2>();

            //ELiminar duplicados            
            //Obtener cuando se produce un cambio de valor en los arrays  
            List<double> xlist = xtarget.ToList();
            List<double> ylist = ytarget.ToList();
            int[] indexes_changesX = Enumerable.Range(0, xlist.Count() - 1).Where(x => xlist[x] != xlist[x+1]).ToArray();
            int[] indexes_changesY = Enumerable.Range(0, ylist.Count() - 1).Where(x => ylist[x] != ylist[x + 1]).ToArray();
            
            //Procesa las dos listas
            for (int i=0; i<indexes_changesX.Count(); i++)
            {                
                for (int j = 0; j < 2; j++) //Para colocar el indice actual y uno superior
                {
                    Vector2 current = new Vector2((float)xlist[indexes_changesX[i] + j] * 100 - centralPoint.X, 
                        (float)ylist[indexes_changesX[i] + j] * 100 - centralPoint.Y);
                                        

                    if (!pointsTarget.Contains(current))
                    {
                        //Lo añade a la lista de puntos pintados
                        pointsTarget.Add(current);
                        Vector2 target = new Vector2((float)xorigin.ElementAt(indexes_changesX[i] + j) * 100 - centralPoint.X,
                            (float)yorigin.ElementAt(indexes_changesX[i] + j) * 100 - centralPoint.Y);
                        pointsOrigin.Add(target);
                    }
                }
            }

            //Procesa las dos listas
            for (int i = 0; i < indexes_changesY.Count(); i++)
            {
                for (int j = 0; j < 2; j++) //Para colocar el indice actual y uno superior
                {
                    Vector2 current = new Vector2((float)xlist[indexes_changesX[i] + j] * 100 - centralPoint.X,
                        (float)ylist[indexes_changesX[i] + j] * 100 - centralPoint.Y);


                    if (!pointsTarget.Contains(current))
                    {
                        //Lo añade a la lista de puntos pintados
                        pointsTarget.Add(current);
                        Vector2 target = new Vector2((float)xorigin.ElementAt(indexes_changesX[i] + j) * 100 - centralPoint.X,
                            (float)yorigin.ElementAt(indexes_changesX[i] + j) * 100 - centralPoint.Y);
                        pointsOrigin.Add(target);
                    }
                }
            }


          


            //Comprueba los puntos iniciales y evita los repetidos
            /*   foreach (TrialPacket trial in trials)
               {
                   int[] idTarget = trial.idTarget;
                   byte[] typeTarget = trial.typeTarget;

                   if (idTarget[0] != -1 && idTarget[1] != -1) //Para no procesar el inicio
                   {
                       int initT = idTarget[0];
                       int endT = idTarget[1];

                       TypeTarget initTT = (TypeTarget)typeTarget[0];
                       TypeTarget endTT = (TypeTarget)typeTarget[1];

                       string code = "";
                       //Ordenar objetivos
                       if (initTT == TypeTarget.TT_TARGET && endTT == TypeTarget.TT_INIT) //Si el objetivo final es una posicion inicial
                           code = $"{endT}/{endTT.ToString()}&{initT}/{initTT.ToString()}";
                       else if (initTT == TypeTarget.TT_INIT && endTT == TypeTarget.TT_TARGET)
                           code = $"{initT}/{initTT.ToString()}&{endT}/{endTT.ToString()}"; //Si el objetivo final es una posicion objetivo
                       else if ((initTT == TypeTarget.TT_INIT && endTT == TypeTarget.TT_INIT)
                           || (initTT == TypeTarget.TT_TARGET && endTT == TypeTarget.TT_TARGET))
                       {
                           //Lo comprueba en las dos direcciones
                           string code1 = $"{initT}/{initTT.ToString()}&{endT}/{endTT.ToString()}";
                           string code2 = $"{endT}/{endTT.ToString()}&{initT}/{initTT.ToString()}";

                           if (!code_zones.Contains(code1) && !code_zones.Contains(code2))
                               code = code1;
                           else if (code_zones.Contains(code1) && !code_zones.Contains(code2))
                               code = code1;
                           else if (!code_zones.Contains(code1) && code_zones.Contains(code2))
                               code = code2;
                           else if (code_zones.Contains(code1) && code_zones.Contains(code2))
                               code = code1;
                       }

                       if (!code_zones.Contains(code))
                           code_zones.Add(code);
                   }
               }


               */
            //Crear las zonas objetivos (por donde debera ir la trayectoria principalmente) 
            /*     for (int i = 0; i < code_zones.Count; i++)
                 {
                     string code = code_zones[i];

                     //Analiza el codigo para generar la zona ideal
                     string[] data = code.Split('&'); //Obtiene las dos partes

                     string[] zoneinit = data[0].Split('/');
                     string[] zoneend = data[1].Split('/');

                     //Obtiene las posiciones a partir de la decodificacion
                     Vector2 initPos = Vector2.Zero;

                     if (zoneinit[1] == TypeTarget.TT_INIT.ToString())
                         initPos = init_target[int.Parse(zoneinit[0])];
                     else if (zoneinit[1] == TypeTarget.TT_TARGET.ToString())
                         initPos = targets[int.Parse(zoneinit[0])];

                     Vector2 targetPos = Vector2.Zero;
                     if (zoneend[1] == TypeTarget.TT_INIT.ToString())
                         targetPos = init_target[int.Parse(zoneend[0])];
                     else if (zoneend[1] == TypeTarget.TT_TARGET.ToString())
                         targetPos = targets[int.Parse(zoneend[0])];

                     //Crea la zona
                     CreateZoneIdealTrajectory(i, initPos, targetPos, widthZone);

                 }*/

            try
            {
                for (int i = 1; i < pointsTarget.Count(); i++)
                {
                    //Crea la zona
                    CreateZoneIdealTrajectory(i, pointsOrigin[i], pointsTarget[i], widthZone); //Solo para la ruleta
                }
            }
            catch(Exception)
            {

            }

            chart.Invalidate();
        }




        public void SetRangeIdealTrajectory(Vector2[] position)
        {
            //Dibuja el rango valido dentro de una zona
            //IEnumerable<double> axisXData = position.Select(x => x.X);
           // IEnumerable<double> axisYData = position.Select(y => y.Y);

            //El punto principal, hacia donde convergen todos los demas puntos (//TEMPORAL)
            //MODIFICAR CUANDO SE ESTRABLEZCA EN LOS JUEGOS EL FICHERO DE EVENTOS y PODER VER LAS TRAYECTORIAS IDEALES
            //DE MOMENTO TODOS VAN AL CENTRO
            //Vector objetivo 1
            Vector2 mainpoint = position[0];
            Vector2 main2 = position[2];

            Vector2 x = new Vector2() { X = main2.X - mainpoint.X, Y = main2.Y - mainpoint.Y}; //Vector al obejtivo 2

            //Calculo el angulo de incidencia en el eje X
            double grados = Math.Atan( x.X/x.Y);
            double degrees = grados * 180 / Math.PI;

            double incx = Math.Cos(grados) * 5;
            double incy = Math.Sin(grados) * 5;

            double xend1 = main2.X + incx;
            double yend1 = main2.Y - incy;
            double xend2 = main2.X - incx;
            double yend2 = main2.X + incy;

            double xstart1 = mainpoint.X + incx;
            double ystart1 = mainpoint.Y - incy;
            double xstart2 = mainpoint.X - incx;
            double ystart2 = mainpoint.X + incy;


            double[] yValue11 = { ystart1, 74, 45, 59, 34, 87, 50, 87, 64, 34 };
            double[] yValue12 = { ystart1, 65, 30, 42, 25, 47, 40, 70, 34, 20 };
            //  chart.Series["Default"].Points.DataBindY(yValue11, yValue12);


            
            //Añadir trayectoria            
            /* for (int i = 1; i < position.Length; i++)
             {
                 //Ida y vuelta
                 chart.Series["SeriesRange"].Points.AddXY(position[0].X, position[0].Y);
                 chart.Series["SeriesIdeal"].Points.AddXY(position[i].X, position[i].Y);
                 chart.Series["SeriesIdeal"].Points.AddXY(position[0].X, position[0].Y);
             }
             */
             chart.Invalidate();
        }

        private void CreateZoneIdealTrajectory(int index, Vector2 init, Vector2 end, double hold)
        {
            //Puntos donde se pintara en el componente, el actual rectangulo de zona de objetivos
            PointF[] ptospoligon = new PointF[4]; //Cuatro puntos que delimitan el area
            PointF[] ptoscircle = new PointF[4]; //Cuatro puntos,punto inicial, final y para calcular el diametro
            PointF[] coefpoligon = new PointF[4]; //Cuatro posibles lados


            //Obtiene el vector principal en funcion delos dos puntos de entrada (direccion)
            Vector2 final = end - init; 

            //Registra la posicion donde estaran las zonas de circulo
            ptoscircle[0] = new PointF(end.X, end.Y);
            ptoscircle[1] = new PointF(init.X, init.Y);            

            //Obtener vector perperndicular con laa longitud de hold
            double Ax = 1;
            double Ay = -final.X/final.Y;

            float incx = 0; //Componente X del vector perpendicular
            float incy = 0; //Componente Y del vector perpendicular

            if (!double.IsInfinity(Ay))
            {             

                double A = Math.Pow(Ax, 2) + Math.Pow(Ay, 2);

                double lambda = Math.Sqrt(Math.Pow(hold, 2) / A);

                //Calculo el angulo de incidencia en el eje X
                double grados = Math.Atan(final.X / final.Y);
                incx = (float)(Ax * lambda);
                incy = (float)(Ay * lambda);
            }
            else
            {
                //Calculo el angulo de incidencia en el eje X                
                incx = 0;
                incy = (float)hold;
            }

            //Calcular incremento de todos los puntos                        
            Vector2[] points = new Vector2[4]; //Cada segmento objetivo tiene 4 puntos            

            //Calcular posiciones punto inicial
            points[0] = new Vector2(init.X - incx, init.Y - incy);
            points[1] = new Vector2(init.X + incx, init.Y + incy);

            //Calcular posiciones punto fina
            points[2] = new Vector2(end.X + incx, end.Y + incy);
            points[3] = new Vector2(end.X - incx, end.Y - incy);


            //Registra la posicion de los puntos para el calculo del tamaño del radio
            ptoscircle[2] = new PointF(points[0].X, points[0].Y);
            ptoscircle[3] = new PointF(points[1].X, points[1].Y);

            //ptoscircle[2] = new PointF(widthZone, 0);
           // ptoscircle[3] = new PointF(-widthZone, 0);



            //Calcular la ecuacion de cada uno de los lados - parte inferior (Elementos situado en 90 grados, paralela al eje Y)
            float pendiente_inf = (points[2].Y - points[3].Y) / (points[2].X - points[3].X);
            float offset_inf = 0;
            if (float.IsInfinity(pendiente_inf))
                offset_inf = points[2].X;
            else
                offset_inf = points[2].Y - points[2].X * pendiente_inf;
            coefpoligon[0] = new PointF(pendiente_inf, offset_inf);

            //Parte izquierda
            float pendiente_izq = (points[3].Y - points[0].Y) / (points[3].X - points[0].X);
            float offset_izq = 0;
            if (float.IsInfinity(pendiente_izq))
                offset_izq = points[3].X;
            else
                offset_izq = points[3].Y - points[3].X * pendiente_izq;
            coefpoligon[1] = new PointF(pendiente_izq, offset_izq);

            //Calcular funcion superior
            float pendiente_sup = (points[1].Y - points[0].Y) / (points[1].X - points[0].X);
            float offset_sup = 0;
            if (float.IsInfinity(pendiente_sup))
                offset_sup = points[1].X;
            else
                offset_sup = points[1].Y - points[1].X * pendiente_sup;
            coefpoligon[2] = new PointF(pendiente_sup, offset_sup);

            //Parte derecha
            float pendiente_der = (points[2].Y - points[1].Y) / (points[2].X - points[1].X);
            float offset_der = 0;
            if (float.IsInfinity(pendiente_der))
                offset_der = points[2].X;
            else
                offset_der = points[2].Y - points[2].X * pendiente_izq;
            coefpoligon[3] = new PointF(pendiente_der, offset_der);

            //****Ordenar los vertices para que se dibujen en la misma direccion (hacia arriba paralelo al eje Y)
            //Angulo entre el vector ideal y el actual normalizado
            Vector2 ideal = new Vector2(0, 1);
            Vector2 current = new Vector2(final.X/ Util.GetModulo(final), final.Y / Util.GetModulo(final));

            float s = ideal.X * current.X + ideal.Y * current.Y;
            float s1 = (float) Math.Sqrt((float)Math.Pow(ideal.X,2) + (float)Math.Pow(ideal.Y, 2)) * (float)Math.Sqrt((float)Math.Pow(current.X, 2) + (float)Math.Pow(current.Y, 2));
            float anglei = (float)(Math.Acos(s / s1)); //Angulo

            //Calcula los nuevos puntos con la rotacion de transformacion para extraer indices de cada uno de los vertices       
            Vector2[] newpoints = new Vector2[4];
            Vector2[] newpoints1 = new Vector2[4];
            for (int i = 0; i < newpoints.Length; i++)
            {
               // newpoints[i].X = (float)(points[i].X * Math.Cos(anglei) - points[i].Y * Math.Sin(anglei));
               // newpoints[i].Y = (float)(points[i].X * Math.Sin(anglei) + points[i].Y * Math.Cos(anglei));

                newpoints[i].X = (float)(Math.Ceiling(1000*(points[i].X * Math.Cos(anglei) - points[i].Y * Math.Sin(anglei))))/1000.0f;
                newpoints[i].Y = (float)(Math.Ceiling(1000*(points[i].X * Math.Sin(anglei) + points[i].Y * Math.Cos(anglei))))/1000.0f;
            }

            

            //Ordena los puntos (esquina inferior izquierda - punto 0, esquina inferior derecha - punto 1
            // List<Vector2> listpoints = new List<Vector2>(); //Registro temporal de vectores
            List<int> indpoints = new List<int>(); //Indice de los vectores

            //Metodo para mostrar las trajectorias ideales                     
            IEnumerable<float> axisXData = newpoints.Select(x => x.X);
            IEnumerable<float> axisYData = newpoints.Select(y => y.Y);

            Vector2[] listpoints = new Vector2[4];
            /*listpoints[0] = new Vector2((float)Math.Round(axisXData.Max(), 2), (float)Math.Round(axisYData.Min(), 2));
            listpoints[1] = new Vector2((float)Math.Round(axisXData.Min(), 2), (float)Math.Round(axisYData.Min(), 2));
            listpoints[2] = new Vector2((float)Math.Round(axisXData.Min(), 2), (float)Math.Round(axisYData.Max(), 2));
            listpoints[3] = new Vector2((float)Math.Round(axisXData.Max(), 2), (float)Math.Round(axisYData.Max(), 2));*/

            listpoints[0] = new Vector2((float)axisXData.Max(), (float)axisYData.Min());
            listpoints[1] = new Vector2((float)axisXData.Min(), (float)axisYData.Min());
            listpoints[2] = new Vector2((float)axisXData.Min(), (float)axisYData.Max());
            listpoints[3] = new Vector2((float)axisXData.Max(), (float)axisYData.Max());


            //Calcula cual es el mas problable de ser

           // Vector2[] porcentaje = new Vector2[4];
            for (int i = 0; i < newpoints.Length; i++)
            {
               /* float[] valuesX = new float[4];
                float[] valuesY = new float[4];*/

                for (int j = 0; j < listpoints.Length; j++)
                {
                       float roundvalX = newpoints[i].X;
                       float roundvalY = newpoints[i].Y;
                       //
                       float currentX = listpoints[j].X;
                       float currentY = listpoints[j].Y;

                    //   valuesX[j] = Math.Abs(roundvalX - currentX);
                    //   valuesY[j] = Math.Abs(roundvalY - currentY);*/

                    //Busca el indice de los puntos deseados
                    //  float roundvalX = (float)Math.Round(newpoints[i].X, 2);
                    //  float roundvalY = (float)Math.Round(newpoints[i].Y, 2);

                    /*float roundvalX = newpoints[i].X;
                    float roundvalY = newpoints[i].Y;
//
                    float currentX = listpoints[j].X;
                    float currentY = listpoints[j].Y;

                     if ((roundvalX == currentX) && (roundvalY == currentY))
                     {
                         indpoints.Add(j);
                    }*/

                    // Define the tolerance for variation in their values

                    /*double difference = Math.Abs(roundvalX * .1 );
                    double difference2 = Math.Abs(roundvalY * .1);

                    

                    double se1 = Math.Abs(roundvalX - currentX);
                    double se2 = Math.Abs(roundvalY - currentY);
                    // Compare the values
                    // The output to the console indicates that the two values are equal
                    if (Math.Abs(roundvalX - currentX) <= difference && Math.Abs(roundvalY - currentY) <= difference2)
                    {
                        indpoints.Add(j);
                    }*/

                    /* if (HasMinimalDifference(roundvalX, currentX, 1) && HasMinimalDifference(roundvalY, currentY, 1))
                     {
                         indpoints.Add(j);
                     }*/
                   // Math.Abs(x - point.X) < 0.01

                    if (Math.Abs(roundvalX - currentX) <= 0.01 && Math.Abs(roundvalY - currentY) <= 0.01)
                    {
                        indpoints.Add(j);
                    }

                    
                }

                
                

                //Busca el valor minimo
                //         int ind = valuesX2.SequenceEqual( FindIndex(item => item.X == value);
            }       

            //Ordena los puntos
            ptospoligon[0] = new PointF(points[indpoints[0]].X, points[indpoints[0]].Y);
            ptospoligon[1] = new PointF(points[indpoints[1]].X, points[indpoints[1]].Y);
            ptospoligon[2] = new PointF(points[indpoints[2]].X, points[indpoints[2]].Y);
            ptospoligon[3] = new PointF(points[indpoints[3]].X, points[indpoints[3]].Y);
            
            
            

            //Registra los puntos para despues pintar la grafica
            poligons.Add(ptospoligon);
            cpoligons.Add(coefpoligon);
            circles.Add(ptoscircle);             
        }

   

        private PointF ConvertPointChartToPixel(PointF point)
        {
            PointF pointdef = new PointF(0,0);
            pointdef.X = (float)chart.ChartAreas[0].AxisX.ValueToPixelPosition(point.X);
            pointdef.Y = (float)chart.ChartAreas[0].AxisY.ValueToPixelPosition(point.Y);
            return pointdef;
        }


      
        /// <summary>
        /// 
        /// </summary>
        /// <param name="xvalues">Valores eje X</param>
        /// <param name="yvalues">Valores eje Y</param>
        /// <param name="limits">Indicar si se tiene que señalar el estar dentro de una zona objetivo</param>
        public void Update(IEnumerable<double> xvalues, IEnumerable<double> yvalues, bool limits)
        {
            //Actualizar grafica comprobando si se tiene que señalar las zonas objetivo
            Series serie = new Series("SeriesPos");
            serie.ChartType = SeriesChartType.Spline;
            chart.Series.Add(serie);

            isDrawTargetZone = limits;

            if (limits)
            {
                int npoint = -1; //Indice de puntos en la grafica
                PointF oldPoint = new PointF(float.NaN, float.NaN); //Punto aun sin asignar
                PointF currentPoint = new PointF(float.NaN, float.NaN); //Punto aun sin asignar

                int d = xvalues.Count();

                for (int i = 0; i < xvalues.Count(); i++)
                {
                    //Punto actual
                    currentPoint = new PointF((float)xvalues.ElementAt(i)* 100 - centralPoint.X, (float)yvalues.ElementAt(i)* 100 - centralPoint.Y);

                    //Calcular recta por donde pasan dos puntos, iterados de manera secuencial
                    if (float.IsNaN(oldPoint.X) && float.IsNaN(oldPoint.Y)) //Primer elemento de la grafica
                    {
                        //currentPoint = new PointF((float)xvalues.ElementAt(i) * 250, (float)yvalues.ElementAt(i) * 100 + 50);
                        serie.Points.AddXY(currentPoint.X,currentPoint.Y);
                        npoint++;

                        //Añade el primer punto comprobando si esta fuera o dentro de alguna zona
                        if (IsInsidePolyies(cpoligons, currentPoint))//Si esta dentro empieza a pintarlo de color verde
                            serie.Points.Last().Color = Color.Green;
                        else //Si no, de color rojo
                            serie.Points.Last().Color = Color.Red;                        
                        
                    }
                    else //Si ya ha iterado dos elementos entonces calcula las posibles intersecciones con la zona
                    {

                        if (oldPoint != currentPoint)
                        {
                            //Incremento de distancia para estadisticas generales
                            distanceTrial += (float)Math.Sqrt(Math.Pow(currentPoint.X - oldPoint.X, 2) + Math.Pow(currentPoint.Y - oldPoint.Y, 2));

                            //Calcula coeficientes de la recta que pasa por estos dos puntos
                            float pendiente = (currentPoint.Y - oldPoint.Y)/(currentPoint.X - oldPoint.X);
                            float offset = currentPoint.Y - currentPoint.X * pendiente;

                            //Calcular puntos donde intersecta con las rectas del cuadrilatero
                            List<PointF> listInterseccion = new List<PointF>(); //Registra los puntos donde interseccionan con los laterales                            
                            List<PointF> disListIntersection = new List<PointF>();

                            //Comprueba los coeficiente de todos los lados de los cuadrilateros para ver si hace falta generar un punto de interseccion    
                            for (int j = 0; j < cpoligons.Count; j++)
                            {   
                                //Calcular los puntos que intersectan los lados de los cuadrilateros
                                for(int k=0; k<cpoligons[j].Length; k++)
                                {
                                    //Coeficiente actuales
                                    PointF coef = cpoligons[j][k];

                                    //Se asegura que los valores de pendiente no sean infinitos, lo que significa que es una recta
                                    float x = 0;
                                    float y = 0;

                                    //Se puede dar 3 posibles casos
                                    if (coef.X == 0) //Si la pendiente es 0 es que es una recta en un punto y
                                    {
                                        x = (coef.Y - offset) / pendiente;
                                        y = coef.Y;

                                        if (float.IsInfinity(Math.Abs(pendiente)))
                                            x = currentPoint.X;
                                    }
                                    else if (float.IsInfinity(coef.X)) //Si la pendiente es infinita es que es una recta en un punto x
                                    {
                                        x = coef.Y;
                                        y = x * pendiente + offset;
                                    }
                                    else //Casos normales
                                    {
                                        if (float.IsInfinity(Math.Abs(pendiente)))
                                        {
                                            x = currentPoint.X;
                                            y = coef.X * x + coef.Y;
                                        }
                                        else
                                        {
                                            x = (coef.Y - offset) / (pendiente - coef.X);
                                            y = x * pendiente + offset;
                                        }
                                    }

                                    //Prueba si esta dentro del poligono actual                                    
                                    bool insidepol = IsInQuadrilateral(cpoligons[j], new PointF(x, y), circles[j]);

                                    if (insidepol)
                                    {
                                        //Comprueba que la interseccion se encuentra entre los dos puntos
                                        //Primero hay que ordenar los limites en el eje X e Y
                                        float[] limitXAxes = new float[2] { oldPoint.X, currentPoint.X };
                                        float[] limitYAxes = new float[2] { oldPoint.Y, currentPoint.Y };

                                        //if (x >= oldPoint.X && x <= currentPoint.X) //Primero si esta en el rango X
                                        if (x >= limitXAxes.Min() && x <= limitXAxes.Max()) //Primero si esta en el rango X
                                        {
                                            if (y >= limitYAxes.Min() && y <= limitYAxes.Max()) //Si esta en el rango Y
                                            {
                                                listInterseccion.Add(new PointF(x, y));                                               
                                            }
                                        }
                                    } 
                                }

                                //Calcular los puntos que intersectan los circulos
                                for (int k = 0; k < circles[j].Length/2; k++)
                                {                                  

                                    //Punto de la circunferencia
                                    PointF cpoint = circles[j][k];                                                                        

                                    //Actualizar valor offset si la pendiente es infinita
                                    if(float.IsInfinity(pendiente))
                                        offset = currentPoint.X;

                                    //Obtiene los puntos de interseccion
                                    PointF[] ptoscirc = CircunfIntersection(pendiente, offset, cpoint, widthZone);
                                    

                                    //Primero hay que ordenar los limites en el eje X e Y
                                    float[] limitXAxes = new float[2] { oldPoint.X, currentPoint.X };
                                    float[] limitYAxes = new float[2] { oldPoint.Y, currentPoint.Y };

                                    //Prueba si esta dentro del poligono actual                
                                    for (int h = 0; h < ptoscirc.Length; h++)
                                    {
                                        if (ptoscirc[h].X >= limitXAxes.Min() && ptoscirc[h].X <= limitXAxes.Max()) //Primero si esta en el rango X
                                        {
                                            if (ptoscirc[h].Y >= limitYAxes.Min() && ptoscirc[h].Y <= limitYAxes.Max()) //Si esta en el rango Y
                                            {
                                                listInterseccion.Add(ptoscirc[h]);
                                            }
                                        }
                                    }
                                }
                            }


                            //Una vez calculados todos los puntos de interseccion se discriminan los que comparten dos o mas zonas objeivos                            
                            for (int j = 0; j < listInterseccion.Count; j++)
                            {
                                PointF currentP = listInterseccion[j];
                                int cont_rep = 0; //Contador de repeticiones para ver en cuantas zonas coincide                                
                                for (int k = 0; k < cpoligons.Count; k++)
                                {
                                    if (IsInQuadrilateral(cpoligons[k], currentP, circles[k]))
                                        cont_rep++; //Incrementa uno si lo encuentra en una zona                                        
                                }

                                //Determinar la zona donde se encuentra
                                bool inside = IsInsidePolyies(cpoligons, currentP);
                                

                                if (cont_rep == 1 && (currentZone == Zone.RECTANGLE || currentZone == Zone.CIRCLE_INIT)
                                    || currentZone == Zone.CIRCLE_END) //Solo lo ha encontrado en una zona
                                {                                    
                                    disListIntersection.Add(currentP); //Lo guardada
                                }
                                //Encuentra la interseccion entre el rectangulo y un circulo
                                else if(cont_rep == 1 && (currentZone == Zone.RECTANGLE_CIRCLE_END || currentZone == Zone.RECTANGLE_CIRCLE_INIT))
                                {
                                    //Comprueba que no exista el valor dentro
                                    if(!disListIntersection.Contains(currentP))
                                        disListIntersection.Add(currentP); //Lo guardada
                                }
                            }
                            //Calcular la posicion de donde van las intersecciones
                            //Clasificar orden dependiendo del punto inicial y final (Eje X)


                            //IEnumerable<double> axisX = puntos.Select(x => (double)x.X);
                            //IEnumerable<double> axisY = puntos.Select(y => (double)y.Y);

                            
                            
                            float initpointX = oldPoint.X;
                            float endpointX = currentPoint.X;
                            

                            float direction = (endpointX - initpointX) /Math.Abs(endpointX - initpointX);
                            if (direction == -1 || direction == 1) //Es direrente a una linea recta hacia abajo
                            {
                                IEnumerable<float> axisX1 = disListIntersection.ToArray().Select(x => x.X);
                                //Debe ordenar en funcion de X
                                List<float> arrayX = axisX1.ToList();
                                if (direction == 1)
                                    arrayX.Sort();
                                else if (direction == -1)
                                {
                                    arrayX.Sort();
                                    arrayX.Reverse();
                                }
                                    

                                List<int> indexX = new List<int>();

                                //Establecer el color de la interseccion
                                //Como posiblemente no vaya a haber nunca dos intersecciones, 
                                //establecer el color dependiendo de sus coordenadas iniciales
                                bool colororder = false;
                                for (int l = 0; l < arrayX.Count; l++)
                                {
                                    double value = arrayX[l];
                                    int ind = disListIntersection.ToList().FindIndex(item => item.X == value);

                                    indexX.Add(ind);

                                    PointF point = disListIntersection[ind];
                                    serie.Points.AddXY(point.X, point.Y); //Pinta el punto final

                                    //Comprueba el color del anterior
                                    Direction dir = GetDirectionPoint(currentPoint, oldPoint);

                                    if(dir == Direction.INPUT)
                                    {
                                        serie.Points.Last().Color = Color.Red;
                                    }
                                    else if (dir == Direction.OUTPUT)
                                    {
                                        serie.Points.Last().Color = Color.Green;

                                        //Si es verde se guarda en la variable de dentro 
                                        //de distancia para estadisticas generales
                                        distInsideZone += (float)Math.Sqrt(Math.Pow(point.X - oldPoint.X, 2) + Math.Pow(point.Y - oldPoint.Y, 2));
                                    }
                                   /* Color oldcolor = serie.Points[serie.Points.Count - 2].Color;
                                    if (oldcolor == Color.Green) //Si el anterior es verde, significa que estaba dentro. Por lo tanto se cambia                                                                            
                                        serie.Points.Last().Color = Color.Red;
                                    else if (oldcolor == Color.Red)
                                        serie.Points.Last().Color = Color.Green;*/

                                   /* if (!colororder)
                                    {
                                        colororder = true;
                                        serie.Points.Last().Color = Color.Red;
                                    }
                                    else
                                    {
                                        colororder = false;
                                        serie.Points.Last().Color = Color.Green;
                                    }*/
                                }
                            }
                            else if (float.IsNaN(direction)) //Direccion perperdicular al eje X
                            {
                                float initpointY = oldPoint.Y;
                                float endpointY = currentPoint.Y;


                                float newdirection = (endpointY - initpointY) / Math.Abs(endpointY - initpointY);
                                IEnumerable<float> axisY = disListIntersection.ToArray().Select(x => x.Y);
                                //Debe ordenar en funcion de Y
                                List<float> arrayY = axisY.ToList();
                                if (newdirection == 1)
                                    arrayY.Sort();
                                else if (newdirection == -1)
                                {
                                    arrayY.Sort();
                                    arrayY.Reverse();
                                }

                                List<int> indexY = new List<int>();
                               
                                for (int l = 0; l < arrayY.Count; l++)
                                {
                                    double value = arrayY[l];
                                    int ind = disListIntersection.ToList().FindIndex(item => item.Y == value);

                                    indexY.Add(ind);

                                    PointF point = disListIntersection[ind];
                                    serie.Points.AddXY(point.X, point.Y); //Pinta el punto final

                                    //Comprueba el color del anterior
                                    //Comprueba el color del anterior
                                    Direction dir = GetDirectionPoint(currentPoint, oldPoint);

                                    if (dir == Direction.INPUT)
                                    {
                                        serie.Points.Last().Color = Color.Red;
                                    }
                                    else if (dir == Direction.OUTPUT)
                                    {
                                        serie.Points.Last().Color = Color.Green;

                                        //Si es verde se guarda en la variable de dentro 
                                        //de distancia para estadisticas generales
                                        distInsideZone += (float)Math.Sqrt(Math.Pow(point.X - oldPoint.X, 2) + Math.Pow(point.Y - oldPoint.Y, 2));
                                    }
                                    /*Color oldcolor = serie.Points[serie.Points.Count - 2].Color;
                                    if (oldcolor == Color.Green) //Si el anterior es verde, significa que estaba dentro. Por lo tanto se cambia                                                                            
                                        serie.Points.Last().Color = Color.Red;
                                    else if(oldcolor == Color.Red)
                                        serie.Points.Last().Color = Color.Green;*/

                                }
                            }


                            //Comprobar si el punto final esta dentro o fuera  
                            // serie.Points.AddXY(currentPoint.X, currentPoint.Y);
                            // if (dir == LineDirection.INPUT)

                            serie.Points.AddXY(currentPoint.X, currentPoint.Y); //Pinta el punto final
                            if (IsInsidePolyies(cpoligons, currentPoint))//Si esta dentro empieza a pintarlo de color verde
                            {
                                serie.Points.Last().Color = Color.Green;

                                //Si es verde se guarda en la variable de dentro 
                                //de distancia para estadisticas generales
                                distInsideZone += (float)Math.Sqrt(Math.Pow(currentPoint.X - oldPoint.X, 2) + Math.Pow(currentPoint.Y - oldPoint.Y, 2));
                            }
                            else //Si no, de color rojo
                                serie.Points.Last().Color = Color.Red;

                        }
                        
                        
                /* foreach(Vector2 vect in list_point)
                 {
                     chart.Series["SeriesPos"].Points.AddXY(vect.X, vect.Y);

                 }*/

               

                

                    }

                  

                   // chart.Series["SeriesPos"].Points.AddXY(xvalues.ElementAt(i) * 250, xvalues.ElementAt(i) * 100 + 50);

                    oldPoint = currentPoint;
                    npoint++;
                }
            }
            else
            {  
                 for (int i = 0; i < xvalues.Count(); i++)
                    serie.Points.AddXY(xvalues.ElementAt(i) * 100 - centralPoint.X, yvalues.ElementAt(i) * 100 - centralPoint.Y);
                   
               
            }

 
          

            // Invalidate chart
            chart.Invalidate();

        }


        public void Update(IEnumerable<double> xvalues, IEnumerable<double> yvalues, bool limits, float samples)
        {
            /* List<List<double>> lst = new List<List<double>>();
             lst.Add(xvalues.ToList());
             lst.Add(yvalues.ToList());

             double[][] arrays = lst.Select(a => a.ToArray()).ToArray();

             List<Vector2> v = new List<Vector2>();
             for (int i = 0; i < xvalues.Count(); i++)
             {
                 v.Add(new Vector2((float)xvalues.ElementAt(i), (float)yvalues.ElementAt(i)));
             }



             var das = v.Distinct().ToList();

             var re2s = from i in Enumerable.Range(0, xvalues.Count())
                        select (new Vector2((float)xvalues.ElementAt(i), (float)yvalues.ElementAt(i)));
             Vector2[] ssd = re2s.ToArray();

             var rerw3 = from i in Enumerable.Range(0, xvalues.Count() - 1)
                         where ssd[i] != ssd[i + 1]
                         select ssd[i];
             int d2 = rerw3.Count();

             List<double> xlist = xvalues.ToList();
             List<double> ylist = yvalues.ToList();
             int[] indexes_changesX = Enumerable.Range(0, xlist.Count() - 1).Where(x => xlist[x] != xlist[x + 1]).ToArray();
             int[] indexes_changesY = Enumerable.Range(0, ylist.Count() - 1).Where(x => ylist[x] != ylist[x + 1]).ToArray();

             var res = from i in indexes_changesX
                       select( new Vector2((float)xlist[i], (float)ylist[i]));

             List<Vector2> ly = res.ToList();*/
            //List<Vector2> s = Util.RemoveDuplicates(v));

            //Actualizar grafica comprobando si se tiene que señalar las zonas objetivo
            Series serie = new Series("SeriesPos");
            serie.ChartType = SeriesChartType.Spline;
            chart.Series.Add(serie);

            isDrawTargetZone = limits;


            //Crear una datatable con los ejes asignados y las muestas a visualizar

            //Muestras que se seleccionaran para visualizar
            float sized = xvalues.Count();
            float incremento = sized / samples;
            IEnumerable<int> indices = Enumerable.Range(0, (int)samples).Select(x => (int)(x * incremento));

            using (DataTable dt = new DataTable("RobotData"))
            {
                dt.Columns.Add("X", typeof(double));
                dt.Columns.Add("Y", typeof(double));

                for (int i = 0; i < indices.Count(); i++)
                {
                    int index = indices.ElementAt(i);

                    //Crea fila
                    DataRow row = dt.NewRow();
                    row["X"] = (xvalues.ElementAt(index) /10 - centralPoint.X)*9;
                    row["Y"] = (yvalues.ElementAt(index) /10 - centralPoint.Y)*9;
                    dt.Rows.Add(row);
                }    

                //Visualizar señal
                serie.Points.DataBindXY(dt.Rows, "X", dt.Rows, "Y");
            }

            // Invalidate chart
            chart.Invalidate();

        }


        public void Update(List<float[]> values, bool limits, float samples)
        {           
            //Actualizar grafica comprobando si se tiene que señalar las zonas objetivo
            Series serie = new Series("SeriesIdeal");
            serie.ChartType = SeriesChartType.Spline;
            serie.BorderWidth = 5;
            chart.Series.Add(serie);

            isDrawTargetZone = limits;


            //Crear una datatable con los ejes asignados y las muestas a visualizar

            //Muestras que se seleccionaran para visualizar
            float sized = values.Count();
            float incremento = sized / samples;
            IEnumerable<int> indices = Enumerable.Range(0, (int)samples);

            using (DataTable dt = new DataTable("RobotData"))
            {
                dt.Columns.Add("X", typeof(double));
                dt.Columns.Add("Y", typeof(double));

                for (int i = 0; i < indices.Count(); i++)
                {
                    int index = indices.ElementAt(i);

                    //Crea fila
                    DataRow row = dt.NewRow();
                    row["X"] = (values[index][0] / 10 - centralPoint.X) * 10 + 0.00001;
                    row["Y"] = (values[index][1] / 10 - centralPoint.Y) * 10;
                    dt.Rows.Add(row);
                }

                //Visualizar señal
                serie.Points.DataBindXY(dt.Rows, "X", dt.Rows, "Y");
            }

            // Invalidate chart
            chart.Invalidate();

        }

        /// <summary>
        /// Calcular los puntos de una recta que cruzan una circunferencia 
        /// </summary>
        /// <param name="a">pendiente de la recta</param>
        /// <param name="b">offset de la recta</param>
        /// <param name="hk">coordenadas de la circunferencia</param>
        /// <param name="r">radio</param>
        /// <returns></returns>
        private PointF[] CircunfIntersection(float a, float b, PointF hk, float r)
        {
            //Despejar funcion de circuferencia en el plaano (x - h)^2 + (y - k)^2 = r
            PointF[] cpoints = new PointF[2]; //Dos puntos de una circuferencia cruzada por una recta            

            //Valores x e y de la ecuacion de segundo grado
            float x1, x2, y1, y2 = 0;

            //Dependiendo de la pendiente realiza un calculo u otro
            if (float.IsInfinity(a)) //Recta paralela al eje Y, ya se conoce x=a
            {
                float c1 = 1;
                float c2 = -2*hk.Y;
                float c3 = (float)Math.Pow(b, 2) + (float)Math.Pow(hk.X, 2) + (float)Math.Pow(hk.Y, 2) - 2*b*hk.X - (float)Math.Pow(r, 2);

                //Despejar la x de la ecuacion de segundo grado
                y1 = (-c2 + (float)Math.Sqrt(Math.Pow(c2, 2) - 4 * c1 * c3)) / (2 * c1);
                y2 = (-c2 - (float)Math.Sqrt(Math.Pow(c2, 2) - 4 * c1 * c3)) / (2 * c1);

                //Despejar Y
                x1 = b;
                x2 = b;
            }           
            else
            {
                //Despejar los componentes de una ecuacion de segundo grado,
                //al igualar la funcion de la recta con la circunferencia c1*x^2 + c2*x + c3 = 0 
                float c1 = (float)Math.Pow(a, 2) + 1;
                float c2 = 2 * a * b - 2 * a * hk.Y - 2 * hk.X;
                float c3 = (float)Math.Pow(b, 2) + (float)Math.Pow(hk.Y, 2) + (float)Math.Pow(hk.X, 2) - 2 * b * hk.Y - (float)Math.Pow(r, 2);

                //Despejar la x de la ecuacion de segundo grado
                x1 = (-c2 + (float)Math.Sqrt(Math.Pow(c2, 2) - 4 * c1 * c3)) / (2 * c1);
                x2 = (-c2 - (float)Math.Sqrt(Math.Pow(c2, 2) - 4 * c1 * c3)) / (2 * c1);

                //Despejar Y
                y1 = a * x1 + b;
                y2 = a * x2 + b;
            }

            

            cpoints[0] = new PointF(x1, y1);
            cpoints[1] = new PointF(x2, y2);

            return cpoints;
        }

        private enum Direction
        {
            NONE,
            OUTPUT, //Atraviesa un borde hacia afura
            INPUT, //Atraviesa un borde hacia dentro
            OUT, //Todo fuera
            IN //Todo dentro
        }

        private Direction GetDirectionPoint(PointF ipoint, PointF opoint )
        {
            Direction dir = Direction.NONE;

            bool init = IsInsidePolyies(cpoligons, opoint);
            bool end = IsInsidePolyies(cpoligons, ipoint);

            if (init && end)
                dir = Direction.IN; 
            else if (!init && !end)
                dir = Direction.OUT;
            else if (!init && end)
                dir = Direction.INPUT;
            else if (init && !end)
                dir = Direction.OUTPUT;

            return dir;
        }

        //Determinar la zona donde se encuentra el punto dentro de una zona objetivo
        private enum Zone
        {
            NONE, //No esta situado dentro de la zona objetivo
            CIRCLE_END, //Está en el circulo final
            CIRCLE_INIT, //Esta en el circulo inicial
            RECTANGLE, //Esta dento del rectangulo
            RECTANGLE_CIRCLE_END, //Esta dentro del rectangulo y el circulo final a la vez
            RECTANGLE_CIRCLE_INIT  //Esta dentro del rectangulo y el circulo inicial a la vez 

        }

        private Zone GetFinalZone(Zone zonerect, Zone zonecirc)
        {
            Zone finalzone = Zone.NONE;

            if (zonerect == Zone.RECTANGLE && zonecirc == Zone.CIRCLE_END)
                finalzone = Zone.RECTANGLE_CIRCLE_END;
            else if(zonerect == Zone.RECTANGLE && zonecirc == Zone.CIRCLE_INIT)
                finalzone = Zone.RECTANGLE_CIRCLE_INIT;
            else if (zonerect == Zone.RECTANGLE && zonecirc == Zone.NONE)
                finalzone = Zone.RECTANGLE;
            else if (zonerect == Zone.NONE && zonecirc == Zone.CIRCLE_INIT)
                finalzone = Zone.CIRCLE_INIT;
            else if (zonerect == Zone.NONE && zonecirc == Zone.CIRCLE_END)
                finalzone = Zone.CIRCLE_END;
            return finalzone;                        
        }

        public void UpdateWithoutIntersect(IEnumerable<double> xvalues, IEnumerable<double> yvalues, bool limits, float samples)
        {
            //Actualizar grafica comprobando si se tiene que señalar las zonas objetivo
            Series serie = chart.Series["SeriesPos"];

            if (limits)
            {


                PointF currentPoint = new PointF(float.NaN, float.NaN); //Punto aun sin asignar

                for (int i = 0; i < xvalues.Count(); i++)
                {
                    //Punto actual
                    currentPoint = new PointF((float)xvalues.ElementAt(i) * 250, (float)yvalues.ElementAt(i) * 100 + 60);

                    //Calcular puntos donde intersecta con las rectas del cuadrilatero                            
                    serie.Points.AddXY(currentPoint.X, currentPoint.Y); //Pinta el punto final
                    if (IsInsidePolyies(cpoligons, currentPoint))//Si esta dentro empieza a pintarlo de color verde
                        serie.Points.Last().Color = Color.Green;
                    else //Si no, de color rojo
                        serie.Points.Last().Color = Color.Red;
                }
            }
            else
            {
                //Crear una datatable con los ejes asignados y las muestas a visualizar

                //Muestras que se seleccionaran para visualizar
                float sized = xvalues.Count();
                float incremento = sized / samples;
                IEnumerable<int> indices = Enumerable.Range(0, (int)samples).Select(x => (int)(x * incremento));

                using (DataTable dt = new DataTable("RobotData"))
                {
                    dt.Columns.Add("X", typeof(double));
                    dt.Columns.Add("Y", typeof(double));

                    for (int i = 0; i < indices.Count(); i++)
                    {
                        int index = indices.ElementAt(i);

                        //Crea fila
                        DataRow row = dt.NewRow();
                        row["X"] = xvalues.ElementAt(index) * 100f - centralPoint.X;
                        row["Y"] = yvalues.ElementAt(index) * 100f - centralPoint.Y;
                        dt.Rows.Add(row);
                    }

                    //Visualizar señal
                    serie.Points.DataBindXY(dt.Rows, "X", dt.Rows, "Y");

                    /*for (int i = 0; i < xvalues.Count(); i++)
                    {
                        serie.Points.AddXY(xvalues.ElementAt(i), yvalues.ElementAt(i));
                        //chart.Series["SeriesPos"].Points.AddXY(xvalues.ElementAt(i) * 250, xvalues.ElementAt(i) * 100 + 50);
                    }*/
                }

                // Invalidate chart
                chart.Invalidate();
            }

        }        

        

        public void IsDrawTargetZone(bool state)
        {
            isDrawTargetZone = state;
            chart.Invalidate(); //Vuelva a pintar
        }

        public void IsDrawTargetCircle(bool state)
        {
            isDrawTargetCircle = state;
            chart.Invalidate(); //Vuelva a pintar
        }

        private void DrawTargetCircle(Graphics e)
        {
            //Color zona
            SolidBrush zoneBrush = new SolidBrush(Color.FromArgb(150, 255, 0, 0)); //Rojo
            

            //Genera una region para generar un circulo con borde variable
            GraphicsPath graphPathOut = new GraphicsPath();
            graphPathOut.FillMode = FillMode.Winding; //Modo para rellenar las zonas que coincidan
            GraphicsPath graphPathInner = new GraphicsPath();
            graphPathInner.FillMode = FillMode.Winding; //Modo para rellenar las zonas que coincidan



            int cont = 0; //Contador de poligonos

            //Obtener limites del dibujo
            PointF[] limits = GetLimitsPlot();

            //Pintar las zonas objetivo
            foreach (PointF[] points in poligons)
            {
                //Pinta los circulos                
                PointF[] ptoscircles = new PointF[2] { circles[cont][0], //Punto final
                                                       circles[cont][1]}; //Punto inicial

                //Primero generar puntos circulares a partir del centro de los puntos objetivos
                for (int i = 0; i < ptoscircles.Length; i++)
                {
                    //Circulo externo
                    PointF[] outcircle = CalculateCircle(ptoscircles[i], widthZone * 0.80f, 100);
                    graphPathOut.AddPolygon(outcircle);
                    PointF[] innercircle = CalculateCircle(ptoscircles[i], widthZone * 0.7f, 100);
                    graphPathInner.AddPolygon(innercircle);
                }
                cont++;
            }
            
            //e.DrawPath()
            // Crear una region con los circulos
            Region regionCircles = new Region(graphPathOut);

            //Obtener el area de insterseccion de la zon dibujable con la zona final            
            regionCircles.Exclude(graphPathInner);
           
            //Zona dibujable del control
             GraphicsPath graphLimit = new GraphicsPath();            
             graphLimit.AddPolygon(GetLimitsPlot());            

             // Crear una region con la zona de dibujo del control
             Region myRegion = new Region(graphLimit);

             //Obtener el area de insterseccion de la zon dibujable con la zona final            
             myRegion.Intersect(regionCircles);

             //Rellenar esta interseccion de zonas
             e.FillRegion(zoneBrush, myRegion);
        }

        private PointF[] CalculateCircle(PointF center, float radius, int npts)
        {
            //Calcula una serie de puntos para generar una esfera a partir de un punto central y un radio
            //asi como se indica la cantidad de puntos que tendra el circulo
            PointF[] circle = new PointF[npts];
            float[] angles = new float[npts];
            float incangle = 360f / (npts - 1); //Rango de calculo
            float tmpangle = 360f;
            for (int j = 0; j < circle.Length; j++)
            {
                float compx = (float)Math.Cos(tmpangle * Math.PI / 180f) * radius;
                float compy = (float)Math.Sin(tmpangle * Math.PI / 180f) * radius;

                circle[j] = ConvertPointChartToPixel(new PointF(center.X + compx, center.Y + compy));
                angles[j] = tmpangle;
                tmpangle -= incangle;
            }

            return circle;
        }

        private void DrawTargetZone(Graphics e)
        {
            //Color zona
            SolidBrush zoneBrush = new SolidBrush(Color.FromArgb(100, 192, 192, 0));

            //Genera un path para mostrar las zonas 
            GraphicsPath graphPath = new GraphicsPath();
            graphPath.FillMode = FillMode.Winding; //Modo para rellenar las zonas que coincidan

            int cont = 0; //Contador de poligonos

            //Pintar las zonas objetivo
            foreach (PointF[] points in poligons)
            {
                //Pinta los cuadrilateroes
                PointF[] newpointf = new PointF[4];
                for (int i = 0; i < newpointf.Length; i++)
                    newpointf[i] = ConvertPointChartToPixel(points[i]);
                graphPath.AddPolygon(newpointf);

                //Pinta los circulos                
                PointF[] ptoscircles = new PointF[2] { circles[cont][0], //Punto final
                                                       circles[cont][1]}; //Punto inicial

                //Primero generar puntos circulares a partir del centro de los puntos objetivos
                for (int i = 0; i < ptoscircles.Length; i++)
                {
                    int npts = 100; //Cantidad de putnos del circulo
                    PointF[] circle = new PointF[npts];
                    float[] angles = new float[npts];
                    float incangle = 360f / (npts - 1); //Rango de calculo
                    float tmpangle = 360f;
                    for (int j = 0; j < circle.Length; j++)
                    {
                        float compx = (float)Math.Cos(tmpangle * Math.PI / 180f) * widthZone;
                        float compy = (float)Math.Sin(tmpangle * Math.PI / 180f) * widthZone;
                        circle[j] = ConvertPointChartToPixel(new PointF(ptoscircles[i].X + compx, ptoscircles[i].Y + compy));

                        angles[j] = tmpangle;
                        tmpangle -= incangle;
                    }
                    graphPath.AddPolygon(circle);//Añadir el circulo al dibujo
                }
                cont++;
            }


            //Zona dibujable del control
            GraphicsPath graphLimit = new GraphicsPath();
            graphLimit.AddPolygon(GetLimitsPlot());

            // Crear una region con la zona de dibujo del control
            Region myRegion = new Region(graphLimit);

            //Obtener el area de insterseccion de la zon dibujable con la zona final            
            myRegion.Intersect(graphPath);

            //Rellenar esta interseccion de zonas
            e.FillRegion(zoneBrush, myRegion);
        }

        private PointF ConvertPixelToPointChart(PointF point)
        {
            PointF pointdef = new PointF(0, 0);
            pointdef.X = (float)chart.ChartAreas[0].AxisX.PixelPositionToValue(point.X);
            pointdef.Y = (float)chart.ChartAreas[0].AxisY.PixelPositionToValue(point.Y);
            return pointdef;
        }

        private bool IsInsideLimitPlot(PointF[] limits, PointF point)
        {
            //Convierte a pixel
            PointF tmppto = ConvertPointChartToPixel(point);

            bool inside = false;
            if (tmppto.X >= limits[0].X && tmppto.X <= limits[1].X) //Esta dentro del eje X
                if (tmppto.Y >= limits[0].Y && tmppto.Y <= limits[1].Y) //Dentro del eje Y
                inside = true;

            return inside;
        }

        public void Reset()
        {
            // Elimina todas las series
            List<string> seriesname = new List<string>();

            foreach(Series serie in chart.Series)
            {
                seriesname.Add(serie.Name);
            }

            foreach (string name in seriesname)
            {
                Series serie = chart.Series[name];

                chart.Series.Remove(serie);                
            }
            chart.Annotations.Clear();
            chart.ChartAreas[0].RecalculateAxesScale();

            // Invalidate chart
            chart.Invalidate();

        }

        public void ClearTargets()
        {
            poligons.Clear();
            cpoligons.Clear(); //Coeficientes de la ecuacion de los lados
            circles.Clear();

            isDrawTargetZone = false;
            isDrawTargetCircle = false;
    }

        

        private void Chart_AxisScrollBarClicked(object sender, System.Windows.Forms.DataVisualization.Charting.ScrollBarEventArgs e)
        {
            // Handle zoom reset button
            if (e.ButtonType == ScrollBarButtonType.ZoomReset)
            {
                // Event is handled, no more processing required
                e.IsHandled = true;

                // Reset zoom on X and Y axis
                chart.ChartAreas[0].AxisX.ScaleView.ZoomReset(0);
                chart.ChartAreas[0].AxisY.ScaleView.ZoomReset(0);

                chart.ChartAreas[0].AxisX.Interval = 5;
                chart.ChartAreas[0].AxisY.Interval = 5;


                chart.ChartAreas[0].AxisX.IsMarksNextToAxis = true;

                // Position the Y axis labels and tick marks next to the axis
                chart.ChartAreas[0].AxisY.IsMarksNextToAxis = true;

                ConfigureAxis(crossCenter);
            }
        }

        private void Chart_AxisViewChanged(object sender, System.Windows.Forms.DataVisualization.Charting.ViewEventArgs e)
        {
            // Change label format when showing hours
            chart.ChartAreas[0].RecalculateAxesScale();
            chart.ChartAreas[0].AxisX.LabelStyle.IntervalType = DateTimeIntervalType.Number;
            chart.ChartAreas[0].AxisX.LabelStyle.Format = "0";
            chart.ChartAreas[0].AxisY.LabelStyle.IntervalType = DateTimeIntervalType.Number;
            chart.ChartAreas[0].AxisY.LabelStyle.Format = "0";
            chart.ChartAreas[0].AxisX.Interval = 1;            
            chart.ChartAreas[0].AxisY.Interval = 1;
            chart.ChartAreas[0].AxisX.IsMarksNextToAxis =  false;

            // Position the Y axis labels and tick marks next to the axis
            chart.ChartAreas[0].AxisY.IsMarksNextToAxis = false;

            chart.ChartAreas[0].AxisX.MajorGrid.Enabled = true;
            chart.ChartAreas[0].AxisX.MajorGrid.LineDashStyle = ChartDashStyle.Dash;
            chart.ChartAreas[0].AxisX.MajorGrid.LineColor = Color.Gray;
            chart.ChartAreas[0].AxisX.MajorTickMark.Enabled = false;
            chart.ChartAreas[0].AxisX.MinorGrid.Enabled = false;
            chart.ChartAreas[0].AxisX.MinorTickMark.Enabled = false;
          //   chartArea.AxisX.MinorTickMark.Interval = valueXMaxChart/10;

            chart.ChartAreas[0].AxisY.MajorGrid.Enabled = true;
            chart.ChartAreas[0].AxisY.MajorGrid.LineDashStyle = ChartDashStyle.Dash;
            chart.ChartAreas[0].AxisY.MajorGrid.LineColor = Color.Gray;
            chart.ChartAreas[0].AxisY.MajorTickMark.Enabled = false;
            chart.ChartAreas[0].AxisY.MinorGrid.Enabled = false;
            chart.ChartAreas[0].AxisY.MinorTickMark.Enabled = false;
            chart.ChartAreas[0].AxisY.MinorTickMark.Interval = 1;

            /*chart.ChartAreas[0].AxisX.Minimum = 1;
            chart.ChartAreas[0].AxisX.Maximum = 1;
            chart.ChartAreas[0].AxisY.Minimum = 1;
            chart.ChartAreas[0].AxisY.Maximum = 1;*/

            chart.ChartAreas[0].RecalculateAxesScale();

           /* if (chart.ChartAreas[0].AxisX.LabelStyle.IntervalType == DateTimeIntervalType.Hours)
            {
                chart.ChartAreas[0].AxisX.LabelStyle.Format = "MMM d, hh tt";
                chart.ChartAreas[0].RecalculateAxesScale();
            }*/
        }

        private Zone currentZone = Zone.NONE;

        //*******************************Separar trials*********************************//
        //******************************************************************************//
        #region Trials       

        public Vector2[] GetTrialIndex(string initFlag, string endFlag, string errorFlag,
            IEnumerable<double> flags, IEnumerable<double> times)
        {
            //Indices inicial y final de los trials
            List<Vector2> listTrialIndex = new List<Vector2>();

            //Extraer vector diferencia del taskstate inicial
            string[] indinit = initFlag.Split('-');
            List<int> tmp_start = Util.DiffAlgorithm(flags, initFlag);//-2);

            

            //Extraer vector diferencia del taskstate final
            indinit = endFlag.Split('-');
            List<int> tmp_end = Util.DiffAlgorithm(flags, endFlag);//-1);
            List<int> tmp_error = Util.DiffAlgorithm(flags, errorFlag);//-1);
           
            //Si existen errores mezclarlo con los finales
            if(tmp_error.Count()>0)
            {
                tmp_end.AddRange(tmp_error);
                //Entonces ordena elementos
                tmp_end.Sort();
            }

            //Selecciona los trials que tienen inicio y final

            int length = Math.Min(tmp_end.Count(), tmp_start.Count());
            for(int i=0; i<length; i++)
            {
                Vector2 indexs = new Vector2(tmp_start[i], tmp_end[i]);
                listTrialIndex.Add(indexs);
            }

            return listTrialIndex.ToArray();
        }

       

        public void SliceTrial(Vector2 trials, IEnumerable<double> x, IEnumerable<double> y)
        {
            //Coge indices inicial y final            
            int indexInf = (int)trials.X;
            int indexSup = (int)trials.Y;            

            //Extraer valores marcados por los indices
            int sizen = indexSup - indexInf;

            double[] sliceddataXr = new double[sizen];
            double[] sliceddataYr = new double[sizen];
            
            Array.Copy(x.ToArray(), indexInf, sliceddataXr, 0, sizen);
            Array.Copy(y.ToArray(), indexInf, sliceddataYr, 0, sizen);                
            
            Update(sliceddataXr, sliceddataYr, false, 1000);
        }        
        #endregion
        //******************************************************************************//
        //******************************************************************************//


        //****************************Calculo con vectores******************************//
        //******************************************************************************//
        /// <summary>
        /// Metodo para calcular si un punto esta dentro de un cuadrilatero determinado por
        /// las ecuaciones de sus lados
        /// </summary>
        private bool IsInQuadrilateral(PointF[] coefficients, PointF point, PointF[] circles)
        {            
            bool isInside = false;//Flag global

            Zone zone1 = Zone.NONE; //Variables temporales para almacenar zonas y determinar la zona final
            Zone zone2 = Zone.NONE;

            bool isInside1 = false;

            //Contar vertices superiores o inferiores
            int[] indices = new int[4];

            if (!float.IsNaN(point.X) && !float.IsNaN(point.Y))
            {
                //Registra los puntos que generan en todas las funciones de los lados
                PointF[] points = new PointF[indices.Length];
                //Coeficientes: 0-Arriba, 1-Izquierda 2-Abajo 3-Derecha
                for (int i = 0; i < coefficients.Length; i++)
                {
                    //Coeficiente actuales
                    PointF coef = coefficients[i];

                    //Se asegura que los valores de pendiente no sean infinitos, lo que significa que es una recta
                    float x = 0;
                    float y = 0;

                    //Se puede dar 3 posibles casos
                    if (coef.X == 0) //Si la pendiente es 0 es que es una recta en un punto y, paralelo al eje X
                    {
                        x = float.PositiveInfinity; // (coef.Y - offset) / pendiente;
                        y = coef.Y;
                    }
                    else if (float.IsInfinity(coef.X)) //Si la pendiente es infinita es que es una recta en un punto x, paralelo al eje Y
                    {
                        x = coef.Y; // (coef.Y - offset) / pendiente;
                        y = float.PositiveInfinity;
                    }
                    else //Casos normales  
                    {
                        y = point.X * coef.X + coef.Y;
                        x = (point.Y - coef.Y) / coef.X;
                    }

                    points[i] = new PointF(x, y);
                }

                //Calcula la cantidad de indices que indican si esta dentro del cuadrilatero, ya que deben tener 
                //lab misma cantidaqd e indices en todas sus direcciones
                for (int i = 0; i < points.Length; i++)
                {
                    float x = points[i].X;
                    float y = points[i].Y;

                    //Incrementa indices //Casos normales
                    if (!float.IsInfinity(Math.Abs(x)) && !float.IsInfinity(Math.Abs(y)))
                    {
                        //   if (point.X == x)
                        //     return true;                    
                        if (Math.Abs(x - point.X) < 0.00001) //Gestiona que un valor sea igual
                        {
                            //Contar cuantos tiene por la derecha o izquierdaarriba y por debajo                        
                            int cont_right = 0;
                            int cont_left = 0;
                            for (int j = 0; j < points.Length; j++)
                            {
                                float currentx = points[j].X;
                                //Extraer el otro valor de X
                                if (Math.Abs(x - currentx) > 0.00001)
                                {
                                    if (x > currentx)
                                        cont_left++;
                                    else if (x < currentx)
                                        cont_right++;
                                }
                            }

                            //En funcion de los contadores de derecha e izquierda incrementara
                            //el indice correspondiente                       
                            if (cont_right == 2)
                                indices[0]++;
                            else if (cont_left == 2)
                                indices[1]++;
                        }
                        else if (x < point.X)
                            indices[0]++;
                        else if (x > point.X)
                            indices[1]++;

                        //if (point.Y == y)
                        //    return true;                    
                        if (Math.Abs(y - point.Y) < 0.00001)
                        {
                            //Contar cuantos tiene por arriba y por debajo                        
                            int cont_up = 0;
                            int cont_down = 0;
                            for (int j = 0; j < points.Length; j++)
                            {
                                float currenty = points[j].Y;
                                //Contar donde estan situadas
                                if (Math.Abs(y - currenty) > 0.00001)
                                {
                                    if (y > currenty)
                                        cont_down++;
                                    else if (y < currenty)
                                        cont_up++;
                                }
                            }

                            //En funcion de los contadores de derecha e izquierda incrementara
                            //el indice correspondiente                        
                            if (cont_up == 2)
                                indices[3]++;
                            else if (cont_down == 2)
                                indices[2]++;
                        }
                        else if (y > point.Y)
                            indices[2]++;
                        else if (y < point.Y)
                            indices[3]++;

                    }
                    else if (float.IsInfinity(Math.Abs(x)) && !float.IsInfinity(Math.Abs(y)))
                    {
                        if (y > point.Y)
                            indices[2]++;
                        else if (y < point.Y)
                            indices[3]++;
                        else if (y == point.Y)
                        {
                            //Lo compara con las otras Y para ver que indices es
                            float anothery = 0;
                            for (int j = 0; j < points.Length; j++)
                            {
                                //Extraer el otro valor de Y
                                float currenty = points[j].Y;
                                if (!float.IsInfinity(Math.Abs(currenty))) //Si es diferente de infinito, lo registra
                                {
                                    if (y != currenty) //Seria el unico diferente
                                        anothery = currenty;
                                }
                            }

                            //Calcula indices
                            if (y < anothery)
                                indices[3]++;
                            else
                                indices[2]++;
                        }
                    }
                    else if (!float.IsInfinity(Math.Abs(x)) && float.IsInfinity(Math.Abs(y)))
                    {
                        if (x < point.X)
                            indices[0]++;
                        else if (x > point.X)
                            indices[1]++;
                        else if (x == point.X)
                        {
                            //Lo compara con las otras X para ver que indices es
                            float anotherx = 0;
                            for (int j = 0; j < points.Length; j++)
                            {
                                //Extraer el otro valor de X
                                float currentx = points[j].X;
                                if (!float.IsInfinity(Math.Abs(currentx))) //Si es diferente de infinito, lo registra
                                {
                                    if (x != currentx) //Seria el unico diferente
                                        anotherx = currentx;
                                }
                            }

                            //Calcula indices
                            if (x < anotherx)
                                indices[0]++;
                            else
                                indices[1]++;
                        }
                    }
                }

                //Para determinar si esta dentro o fuera, los indices deben ser iguales
                int old = indices[0];
                for (int i = 1; i < indices.Length; i++)
                {
                    int current = indices[i];
                    if (current == old)
                    {
                        isInside1 = true;
                        zone1 = Zone.RECTANGLE;
                    }
                    else
                    {
                        isInside1 = false;
                        zone1 = Zone.NONE;
                        break;
                    }
                    old = current;
                }


                //Testea dentro de los circulos
                bool isInside2 = false;
                for (int i = 0; i < 2; i++)
                {
                    Vector2 ptocircle = new Vector2(circles[i].X, circles[i].Y);
                    Vector2 vector = new Vector2(point.X - ptocircle.X, point.Y - ptocircle.Y);

                    //Calcula la distancia y se compara con el radio para ver si esta dentro
                    float distancia = Util.GetModulo(vector);

                    //Redondear a 4 decimales
                    if(!float.IsInfinity(distancia))
                        distancia = (float) Decimal.Round((decimal)distancia, 4);

                    if (distancia <= widthZone)
                    {
                        isInside2 = true;
                        if (i == 0) //Circulo final
                            zone2 = Zone.CIRCLE_END;
                        else if (i == 1)
                            zone2 = Zone.CIRCLE_INIT;

                        break;
                    }
                }

                if (isInside2 || isInside1)
                {
                    isInside = true;

                    
                }
            }

            //Determinar donde está situado
            currentZone = GetFinalZone(zone1, zone2);

            return isInside;
        }

        private bool IsInsidePolyies(List<PointF[]> polyies, PointF pnt)
        {
            //Se le pasa varios poligonos
            bool inside = false;

            int i = 0;
            foreach (PointF[] poly in polyies)
            {
                inside = IsInQuadrilateral(poly, pnt, circles[i]);

                if (inside)
                    break;
                i++;
            }
            return inside;
        }

        

        private PointF[] GenerateCircle(int npoints, PointF center, PointF[] sides)
        {
            //Genera un vector de puntos que forman un semicirculo
            PointF[] points = new PointF[npoints];

            return points;
        }

        private PointF[] GetLimitsPlot()
        {
            Size chartsize = chart.Size; //Tamaño en pixeles del componente principal

            ElementPosition posgen = chart.ChartAreas[0].Position; //Posicion relativa del chart dentro del componente de control
            ElementPosition posinner = chart.ChartAreas[0].InnerPlotPosition; //Posicion relativa interna

            //Calcula el tamaño en pixeles del chart con respecto al componente principal
            float[] sizepixelsChart = new float[4];
            sizepixelsChart[0] = (posgen.Right) * (float)chartsize.Width / 100f; //Anchura 
            sizepixelsChart[1] = (posgen.X) * (float)chartsize.Width / 100f;

            sizepixelsChart[2] = (posgen.Bottom) * (float)chartsize.Height / 100f; //Altura 
            sizepixelsChart[3] = (posgen.Y) * (float)chartsize.Height / 100f;


            //Ahora se calcula el tamaño en pixeles de la zona interna del chart
            float[] sizeinnerpixelsChart = new float[4];
            sizeinnerpixelsChart[0] = sizepixelsChart[1] + (posinner.Right) * (float)(sizepixelsChart[0] - sizepixelsChart[1]) / 100f;
            sizeinnerpixelsChart[1] = sizepixelsChart[1] + (posinner.X) * (float)(sizepixelsChart[0] - sizepixelsChart[1]) / 100f;

            sizeinnerpixelsChart[2] = sizepixelsChart[3] + (posinner.Bottom) * (float)(sizepixelsChart[2] - sizepixelsChart[3]) / 100f;
            sizeinnerpixelsChart[3] = sizepixelsChart[3] + (posinner.Y) * (float)(sizepixelsChart[2] - sizepixelsChart[3]) / 100f;

            //Posicion completa
            PointF[] limits = new PointF[4];         

            //Lista del cuadrilatero central
            limits[0] = new PointF(sizeinnerpixelsChart[1], sizeinnerpixelsChart[3]); //Punto izq superior
            limits[1] = new PointF(sizeinnerpixelsChart[0], sizeinnerpixelsChart[3]); //Punto der superior
            limits[2] = new PointF(sizeinnerpixelsChart[0], sizeinnerpixelsChart[2]); //Punto der superior
            limits[3] = new PointF(sizeinnerpixelsChart[1], sizeinnerpixelsChart[2]); //Punto izq inferior

            return limits;
        }
        //******************************************************************************//
        //******************************************************************************//

        //********************************Pintar datos**********************************//
        //******************************************************************************//
        public void AddTaskState(IEnumerable<double> time, IEnumerable<double> state, string type, float distance)
        {
            //Genera una nueva serie de datos
            Series serie = new Series(type);
            serie.ChartType = SeriesChartType.StepLine;            
            serie.SmartLabelStyle.Enabled = true;
            serie.SmartLabelStyle.AllowOutsidePlotArea = LabelOutsidePlotAreaStyle.Yes;
            serie.SmartLabelStyle.CalloutLineAnchorCapStyle = LineAnchorCapStyle.Arrow;
            serie.SmartLabelStyle.CalloutLineColor = Color.Yellow;
            serie.SmartLabelStyle.CalloutLineWidth = 2;
            serie.SmartLabelStyle.CalloutStyle = LabelCalloutStyle.Underlined;
            chart.Series.Add(serie);

            //Estado valor maximo
            double maximo = state.Max();            

            for (int i = 0; i < time.Count(); i++)
            {
                //Valor tiempo
                double valueX = time.ElementAt(i);

                //Selecciona datos del eje y
                double valueY = (state.ElementAt(i)/maximo) * distance/2f; //Para que se ajuste a la altura total de la grafica (normalizado al maximo)

                DataPoint p = new DataPoint(valueX, valueY);

                //Pinta el punto final
                serie.Points.Add(p);//.AddXY(valueX, valueY);
            }


            //Extraer donde se produce el cambio de flag para señalar valores
            List<float> diff = Util.DiffAlgorithm(state);
            //Revisa donde se produce el cambio de evento de inicio de trial            
            int index = 0;
            while (index != -1)
            {
                index = diff.ToList().FindIndex(x => x != 0);
                if (index != -1)
                {
                    //Etiquetas con los valores de tags                   
                    serie.Points[index].Label = state.ElementAt(index).ToString("0");                    
                    serie.Points[index+1].Label = state.ElementAt(index+1).ToString("0");
                    diff[index] = 0; //Lo anula para el siguiente elemento
                }

            }

            // Invalidate chart
            chart.Invalidate();

        }

        /// <summary>
        /// Señala un punto con una anotacion
        /// </summary>
        public void RemarkPoint(string selector, int index, string type)
        {
            // Find selected data point
            DataPoint point1 = chart.Series[type].Points[index];
            DataPoint point0 = chart.Series[type].Points[index - 1];

            Annotation annotation_flag = chart.Annotations.FindByName("Flag" + selector);             

            if (annotation_flag == null) //Crea uno nuevo
            {
                // create a line annotation
                LineAnnotation annotation = new LineAnnotation();

                // setup visual attributes
                annotation.StartCap = LineAnchorCapStyle.None;
                annotation.EndCap = LineAnchorCapStyle.Arrow;
                annotation.Name = "Flag" + selector;

                annotation.LineWidth = 5;
                annotation.LineColor = Color.OrangeRed;
                annotation.ClipToChartArea = "Default";

                

                // prevent moving or selecting
                annotation.AllowMoving = false;
                annotation.AllowAnchorMoving = false;
                annotation.AllowSelecting = false;


                // Use the Anchor Method to anchor to points 8 and 10...
               // annotation.SetAnchor(new DataPoint(point1.XValue, point0.YValues[0]), point1);
                annotation.SetAnchor(point0, point1);



                chart.Annotations.Add(annotation);
            }
          else //Actualiza posicion
            {
                // Use the Anchor Method to anchor to points 8 and 10...
               // annotation_flag.SetAnchor(new DataPoint(point1.XValue, point0.YValues[0]), point1);
                annotation_flag.SetAnchor(point0, point1);
            }
            
         
        }        

        public void AddFeature(IEnumerable<double> time, IEnumerable<double> pos,  string type, float samples)
        {
            //Genera una nueva serie de datos
            Series serie = new Series(type);
            serie.ChartType = SeriesChartType.Spline;
            chart.Series.Add(serie);

            //Crear una datatable con los ejes asignados y las muestas a visualizar

            //Muestras que se seleccionaran para visualizar            
            float sized = pos.Count();
            float incremento = sized / samples;
            IEnumerable<int> indices = Enumerable.Range(0, (int)samples).Select(x => (int)(x * incremento));

            using (DataTable dt = new DataTable("Data" + type))
            {
                dt.Columns.Add("X", typeof(double));
                dt.Columns.Add("Y", typeof(double));

                for (int i = 0; i < indices.Count(); i++)
                {
                    int index = indices.ElementAt(i);

                    //Crea fila
                    DataRow row = dt.NewRow();
                    row["X"] = time.ElementAt(index);
                    row["Y"] = pos.ElementAt(index) * 100f - centralPoint.Y;
                    dt.Rows.Add(row);
                }

                //Visualizar señal
                serie.Points.DataBindXY(dt.Rows, "X", dt.Rows, "Y");

                /* for (int i = 0; i < time.Count(); i++)
                 {
                     //Valor tiempo
                     double valueX = time.ElementAt(i);

                     //Selecciona datos del eje y
                     double valueY = pos.ElementAt(i) * 100 - centralPoint.Y;              

                     //Pinta el punto final
                     serie.Points.AddXY(valueX, valueY);
                 }*/
            }
                // Invalidate chart
                chart.Invalidate();

        }

        public void RemoveFeature(string type)
        {
            //Genera una nueva serie de datos 
            try
            {
                Series serie = chart.Series[type];
                chart.Series.Remove(serie);
                chart.ChartAreas[0].RecalculateAxesScale();
            }
            catch(Exception)
            {
                Reset();
            }
            // Invalidate chart
            chart.Invalidate();

        }
        //******************************************************************************//
        //******************************************************************************//

        public void AddCircleTargets(IEnumerable<double> xtarget, IEnumerable<double> ytarget)
        {                        
            //El punto principal, hacia donde convergen todos los demas puntos                
            List<Vector2> pointsTarget = new List<Vector2>();

            for (int i = 0; i < xtarget.Count(); i++)
            {
                Vector2 current = new Vector2((float)xtarget.ElementAt(i) * 100 - centralPoint.X, (float)ytarget.ElementAt(i) * 100 - centralPoint.Y);

                if (!pointsTarget.Contains(current))
                {
                    //Lo añade a la lista de puntos pintados
                    pointsTarget.Add(current);                    
                }
            }

            Series bubbles = new Series("Targets");
            bubbles.ChartType = SeriesChartType.Bubble;
            bubbles.MarkerBorderWidth = 10;
            bubbles.MarkerBorderColor = Color.Black;
            bubbles.MarkerColor = Color.Transparent;
            bubbles.MarkerStyle = MarkerStyle.Circle;
            bubbles.MarkerSize = 1;
            bubbles["BubbleScaleMin"] = "5";
                // bubbles.DeleteCustomProperty("BubbleScaleMin");
            chart.Series.Add(bubbles);
            

            for (int i = 0; i < pointsTarget.Count(); i++)
            {
                //Crea la zona
                bubbles.Points.AddXY(pointsTarget[i].X, pointsTarget[i].Y);
                
            }

            chart.Invalidate();
        }


    }
}


