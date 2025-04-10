using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Windows.Forms.DataVisualization.Charting;
using System.Drawing.Drawing2D;

namespace REVIREPanels.Estadisticas.Graficas
{
    class ChartValoration    
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
        
        //Chart principal
        private Chart chart;     
              
        private float margin = 1.0f; //Margen de la grafica

        private List<string> series_name;

        #endregion

        //*******************************Inicialización*********************************//
        //******************************************************************************//
        #region Inicializacion
        public ChartValoration(Chart inChart)
        {
            //Inicializar componente
            chart = inChart;

            //Inicializa nombre series
            series_name = new List<string> { "TrialRight", "TrialUp", "TrialLeft", "TrialDown" };

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
            chart.ChartAreas[0].BorderWidth = 0; //Anchura            
            chart.Margin = new Padding(0);

            //Color fondo
            chart.BackColor = Color.Transparent;
            

            //Elimina leyendas
            chart.Legends.Clear();

           

            

          

            //Configura la imagen de fondo
            chart.ChartAreas[0].BackColor = Color.Transparent;
            chart.ChartAreas[0].BackImage = null;
            chart.ChartAreas[0].BackImageAlignment = ChartImageAlignmentStyle.Center;
            chart.ChartAreas[0].BackImageWrapMode = ChartImageWrapMode.Scaled;

            //Configurar series
            ConfigureSeries();

            //Tamaño de la grafica dentro del lienzo
            chart.ChartAreas[0].InnerPlotPosition.Auto = false;
            chart.ChartAreas[0].InnerPlotPosition.Height = 100F;
            chart.ChartAreas[0].InnerPlotPosition.Width = 100F;
            chart.ChartAreas[0].InnerPlotPosition.X = 0F;
            chart.ChartAreas[0].InnerPlotPosition.Y = 0F;

            chart.ChartAreas[0].Position.Auto = false;
            chart.ChartAreas[0].Position.X = 0F;
            chart.ChartAreas[0].Position.Y = 0F;
            chart.ChartAreas[0].Position.Width = 100F;
            chart.ChartAreas[0].Position.Height = 100F;
                    
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
            margin = distancia / (2f * 10f); 

            //Primero, calcular los limites de la grafica
            valueXMaxChart = distancia / 2.0f;
            valueXMinChart = -distancia / 2.0f;

            valueYMaxChart = +distancia / 2.0f;
            valueYMinChart = -distancia / 2.0f;

            //Valores medio de los ejes de posicion
            valueXMidChart = 0;
            valueYMidChart = 0;

            ConfigureAxis();

            //Punto central
            centralPoint = center;

            chart.Invalidate();
        }
        
        public void ConfigureAxis()
    {
        //Configuracion de ejes      
        chart.ChartAreas[0].AxisX.Enabled = AxisEnabled.False;
        chart.ChartAreas[0].AxisY.Enabled = AxisEnabled.False;
        chart.ChartAreas[0].AxisX.IsMarginVisible = false;
        chart.ChartAreas[0].AxisY.IsMarginVisible = false;
           
        //Configurar Axes de la grafica   
        chart.ChartAreas[0].AxisX.Minimum = valueXMinChart - margin;
        chart.ChartAreas[0].AxisX.Maximum = valueXMaxChart + margin;
        chart.ChartAreas[0].AxisY.Minimum = valueYMinChart - margin;
        chart.ChartAreas[0].AxisY.Maximum = valueYMaxChart + margin;

        // Disable malla de fondo
        chart.ChartAreas[0].AxisX.MajorGrid.Enabled = false;            
        chart.ChartAreas[0].AxisX.MajorTickMark.Enabled = false;
        chart.ChartAreas[0].AxisX.MinorGrid.Enabled = false;
        chart.ChartAreas[0].AxisX.MinorTickMark.Enabled = false;            

        chart.ChartAreas[0].AxisY.MajorGrid.Enabled = false;            
        chart.ChartAreas[0].AxisY.MajorTickMark.Enabled = false;
        chart.ChartAreas[0].AxisY.MinorGrid.Enabled = false;
        chart.ChartAreas[0].AxisY.MinorTickMark.Enabled = false;

        //Cruza los ejes
        chart.ChartAreas[0].AxisX.Crossing = valueXMidChart; //Valor medio
        chart.ChartAreas[0].AxisY.Crossing = valueYMidChart; //Valor medio
        chart.ChartAreas[0].AxisX.IsMarksNextToAxis = true;

        // Position the Y axis labels and tick marks next to the axis
        chart.ChartAreas[0].AxisY.IsMarksNextToAxis = true;   
    }
        
        /// <summary>
        /// Configura una serie por cada trial
        /// </summary>
        private void ConfigureSeries()
    {
        //Crea 4 series            
        List<Color> colores = new List<Color>
        {
            Color.ForestGreen, Color.GreenYellow,
            Color.DarkOliveGreen, Color.DarkSeaGreen
        };
            
        for(int i=0; i< series_name.Count; i++)
        {
            Series serie = new Series(series_name[i]);
            serie.ChartType = SeriesChartType.Spline;
            serie.Color = colores[i];
            serie.BorderWidth = 3;

            chart.Series.Add(serie);
        }     
    }
        #endregion
        //******************************************************************************//
        //******************************************************************************//


        //****************************Visualización de datos****************************//
        //******************************************************************************//               
        /// <summary>
        /// 
        /// </summary>
        /// <param name="xvalues">Valores eje X</param>
        /// <param name="yvalues">Valores eje Y</param>        
        public void Update(List<double[]> xvalues, List<double[]> yvalues)
        {
            
            for(int i=0; i<series_name.Count; i++)
            {
                //Actualizar grafica comprobando si se tiene que señalar las zonas objetivo            
                Series serie = chart.Series[series_name[i]];

                double[] valuesx = xvalues[i];
                double[] valuesy = yvalues[i];

                for (int j = 0; j < valuesx.Count(); j++) 
                    serie.Points.AddXY(valuesx[j] * 100 - centralPoint.X, valuesy[j] * 100 - centralPoint.Y); 

                // Invalidate chart
                chart.Invalidate();
            }

            

        }
        //******************************************************************************//
        //******************************************************************************//

            
        //****************************Estado de los datos*******************************//
        //******************************************************************************//
        public void SetVisibleTrajectory(bool state)
        {
            
            //Establece la visibilidad de la trayectoria
            try
            {                
                for(int i=0; i<series_name.Count; i++)
                    chart.Series[series_name[i]].Enabled = state;

            }
            catch (Exception){}            
        }

        public void SetImage(Bitmap image)
        {
            try
            {
                chart.Images.Clear();
                chart.Images.Add(new NamedImage("img", image));
                chart.ChartAreas[0].BackImage = chart.Images[0].Name;
            }
            catch (Exception) { }
        }

        public void SetImage(string name)
        {
            try
            {                
                chart.ChartAreas[0].BackImage = name;
            }
            catch (Exception) { }
        }

        public void Reset()
        {
            // Elimina los puntos todas las series           
            try
            {
                for (int i = 0; i < series_name.Count; i++)
                    chart.Series[series_name[i]].Points.Clear();

            }
            catch (Exception) { }

            // Invalidate chart
            chart.Invalidate();
        }
        //******************************************************************************//
        //******************************************************************************//  



    }
}
