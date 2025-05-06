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

namespace REVIREPanels.Estadisticas.Graficas
{
    class DTWChart     
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


        //Umbral para que se vea mas parte de la grafica
        private double threshold;



        private const int nXLines = 10; //Numero de incrementos en el eje X
        private const int nYLines = 10; //Numero de incrementos en el eje X
        private const float margin = 1.0f; //Margen de la grafica       




        #endregion

        //*******************************Inicialización*********************************//
        //******************************************************************************//
        #region Inicializacion
        public DTWChart(Chart inChart)
        {
            //Inicializar componente
            chart = inChart;

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



        //********************************Pintar grafica********************************//
        //******************************************************************************//
        void Chart_Paint(object sender, PaintEventArgs e)
        {

        }


        public void Dispose()
        {
            chart.Paint -= Chart_Paint;
            chart.AxisScrollBarClicked -= Chart_AxisScrollBarClicked;
            chart.AxisViewChanged -= Chart_AxisViewChanged;
        }





        
        public void Update(double[] valuesR, double[] valuesI)
        {
            //Actualizar grafica comprobando si se tiene que señalar las zonas objetivo
            Series serieR = new Series("SeriesReal");
            serieR.ChartType = SeriesChartType.Spline;
            chart.Series.Add(serieR);

            //Muestras que se seleccionaran para visualizar
            int sized = valuesR.Count();
           // float incremento = sized / samples;
           // IEnumerable<int> indices = Enumerable.Range(0, (int)samples);

            using (DataTable dt = new DataTable("RobotData"))
            {
                dt.Columns.Add("X", typeof(double));
                dt.Columns.Add("Y", typeof(double));

                for (int i = 0; i < sized; i++)
                {
                    //int index = indices.ElementAt(i);

                    //Crea fila
                    DataRow row = dt.NewRow();
                    row["X"] = i;
                    row["Y"] = valuesR.ElementAt(i);

                    dt.Rows.Add(row);
                }

                //Visualizar señal
                serieR.Points.DataBindXY(dt.Rows, "X", dt.Rows, "Y");
            }


            //Actualizar grafica comprobando si se tiene que señalar las zonas objetivo
            Series serieI = new Series("SeriesIdeal");
            serieI.ChartType = SeriesChartType.Spline;
            serieI.BorderWidth = 5;
            chart.Series.Add(serieI);

            //Muestras que se seleccionaran para visualizar
            sized = valuesI.Count();
            //incremento = sized / samples;
           // indices = Enumerable.Range(0, (int)samples);

            using (DataTable dt = new DataTable("RobotData"))
            {
                dt.Columns.Add("X", typeof(double));
                dt.Columns.Add("Y", typeof(double));

                for (int i = 0; i < sized; i++)
                {
                    //int index = indices.ElementAt(i);

                    //Crea fila
                    DataRow row = dt.NewRow();
                    row["X"] = i;
                    row["Y"] = valuesI.ElementAt(i);

                    dt.Rows.Add(row);
                }

                //Visualizar señal
                serieI.Points.DataBindXY(dt.Rows, "X", dt.Rows, "Y");
            }




            // Invalidate chart
            chart.Invalidate();

        }





        public void Reset()
        {
            // Elimina todas las series
            List<string> seriesname = new List<string>();

            foreach (Series serie in chart.Series)
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
            chart.ChartAreas[0].AxisX.IsMarksNextToAxis = false;

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




        //********************************Pintar datos**********************************//
        //******************************************************************************//

        public void RemoveFeature(string type)
        {
            //Genera una nueva serie de datos 
            try
            {
                Series serie = chart.Series[type];
                chart.Series.Remove(serie);
                chart.ChartAreas[0].RecalculateAxesScale();
            }
            catch (Exception)
            {
                Reset();
            }
            // Invalidate chart
            chart.Invalidate();

        }
        //******************************************************************************//
        //******************************************************************************//




    }
}
