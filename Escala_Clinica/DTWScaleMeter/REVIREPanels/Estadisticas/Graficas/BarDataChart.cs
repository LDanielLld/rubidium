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

namespace REVIREPanels.Estadisticas.Graficas
{
    public enum BarChartType
    {
        NONE,
        REPETITIONS,//Repeticiones
        REP_SUCCES_FAIL, //Repeticiones con los fallos y aciertos
        DISTANCE_TOTAL, //Distancia total recorrida
        TIME_TOTAL, //Tiempo total
    }

    class BarDataChart    
    {
        //Chart principal
        private Chart chart;

        //Tipo de grafica de barras
        BarChartType type = BarChartType.NONE;      
       

        public BarDataChart(Chart inChart, BarChartType tpe)
        {
            //Inicializar componente
            chart = inChart;

            //Tipo
            type = tpe;           

            //Inicializa la gráfica            
            InitChart();
        }

        private void InitChart()
        {
            chart.Series.Clear(); //Eliminar si existe alguna

            // Grafica inicial                
            chart.AntiAliasing = AntiAliasingStyles.All; // Set Antialiasing mode            
            chart.TextAntiAliasingQuality = TextAntiAliasingQuality.High;
            chart.ChartAreas[0].IsSameFontSizeForAllAxes = true;

            //Crea las series necesarias
            Series serie = new Series("SeriesPos");           
            chart.Series.Add(serie);
            serie = new Series("SeriesIdeal");
            chart.Series.Add(serie);

            // chart.Series["SeriesTarget"].Points.Clear();

            //Tipo de grafica 
            if (type != BarChartType.REP_SUCCES_FAIL)
            {
                chart.Series["SeriesPos"].ChartType = SeriesChartType.Bar;
                chart.Series["SeriesIdeal"].ChartType = SeriesChartType.Bar;
               // chart.Series["SeriesTarget"].ChartType = SeriesChartType.Bar;
            }           

                //Configurar ejes
                //  chart.ChartAreas[0].AxisX.LabelStyle.IntervalOffset = 1;
                //  chart.ChartAreas[0].AxisX.LabelStyle.IntervalOffsetType = DateTimeIntervalType.Number;
                chart.ChartAreas[0].AxisX.LabelStyle.Interval = 1;
            chart.ChartAreas[0].AxisX.LabelStyle.IntervalType = DateTimeIntervalType.Number;

         //   chart.ChartAreas[0].AxisY.LabelStyle.IntervalOffset = 1;
          //  chart.ChartAreas[0].AxisY.LabelStyle.IntervalOffsetType = DateTimeIntervalType.Number;
            chart.ChartAreas[0].AxisY.LabelStyle.Interval = 1;
            chart.ChartAreas[0].AxisY.LabelStyle.IntervalType = DateTimeIntervalType.Number;

            chart.ChartAreas[0].AxisX.IsMarginVisible = true;
            chart.ChartAreas[0].AxisY.IsMarginVisible = false;

            // Configurar etiquetas de la posiciones
            chart.Series["SeriesPos"].IsValueShownAsLabel = true;
            chart.Series["SeriesPos"]["LabelStyle"] = "Top";
            chart.Series["SeriesPos"].SmartLabelStyle.Enabled = false;
            chart.Series["SeriesPos"].LabelAngle = 0;
            //   chart.Series["SeriesPos"].LabelForeColor = Color.Black;
            //  chart.Series["SeriesPos"].LabelBackColor = Color.Transparent;
            // chart.Series["SeriesPos"].LabelBorderColor = Color.Transparent;

            //Configurar grid
            // Enable all elements
            chart.ChartAreas[0].AxisX.MajorGrid.Enabled = false;
            chart.ChartAreas[0].AxisX.MajorGrid.LineDashStyle = ChartDashStyle.Dash;
            chart.ChartAreas[0].AxisX.MajorGrid.LineColor = Color.Gray;            
            chart.ChartAreas[0].AxisX.MinorGrid.Enabled = false;

            chart.ChartAreas[0].AxisX.MajorTickMark.Enabled = false;
            chart.ChartAreas[0].AxisX.MinorTickMark.Enabled = false;


             chart.ChartAreas[0].AxisY.MajorGrid.Enabled = true;
            chart.ChartAreas[0].AxisY.MajorGrid.Interval = 1;
             chart.ChartAreas[0].AxisY.MajorGrid.LineDashStyle = ChartDashStyle.Dash;
             chart.ChartAreas[0].AxisY.MajorGrid.LineColor = Color.Gray;
            chart.ChartAreas[0].AxisY.MinorGrid.Enabled = false;

            chart.ChartAreas[0].AxisY.MajorTickMark.Enabled = true;
            chart.ChartAreas[0].AxisY.MajorTickMark.Interval = 1;
            chart.ChartAreas[0].AxisY.MinorTickMark.Enabled = false;


            //Fuente  para el titulo de los ejes
            Font fontref = chart.ChartAreas[0].AxisX.TitleFont;
            Font tittlefont = new Font(fontref.FontFamily, 11, FontStyle.Bold);
            
            //Titulos de los ejes            
            chart.ChartAreas[0].AxisY.TitleFont = tittlefont;            
            chart.ChartAreas[0].AxisX.TitleFont = tittlefont;

            switch (type)
            {
                case BarChartType.REPETITIONS: chart.ChartAreas[0].AxisX.Title = "Actividades";                    
                    chart.ChartAreas[0].AxisY.Title = "Repeticiones"; break;
                case BarChartType.REP_SUCCES_FAIL: ConfigureSuccesBar(); break;
                case BarChartType.DISTANCE_TOTAL: ConfigureDistanceBar();  break;
                case BarChartType.TIME_TOTAL: ConfigureTimeBar(); break;
                default: break;
            }
            

        }

        private void SetBarFeatures()
        {
            switch(type)
            {
                case BarChartType.REPETITIONS: chart.ChartAreas[0].AxisX.Title = "Actividades"; break;
                
                case BarChartType.DISTANCE_TOTAL: chart.ChartAreas[0].AxisX.Title = "Distancia"; break;
                case BarChartType.TIME_TOTAL: chart.ChartAreas[0].AxisX.Title = "Tiempo"; break;
                default: break;
            }
        }

        public void Dispose()
        {
            
           
        }

        //

        public void ConfigureAxis()
        {
            //Configurar Axes de la grafica
            chart.ChartAreas[0].AxisX.Interval = 5;
            chart.ChartAreas[0].AxisX.IntervalType = DateTimeIntervalType.Number;
            chart.ChartAreas[0].AxisY.Interval = 5;
            chart.ChartAreas[0].AxisY.IntervalType = DateTimeIntervalType.Number;

       /*     chart.ChartAreas[0].AxisX.Minimum = valueXMinChart;
            chart.ChartAreas[0].AxisX.Maximum = valueXMaxChart;
            chart.ChartAreas[0].AxisY.Minimum = valueYMinChart;
            chart.ChartAreas[0].AxisY.Maximum = valueYMaxChart;*/

            // Enable all elements
            chart.ChartAreas[0].AxisX.MajorGrid.Enabled = true;
            chart.ChartAreas[0].AxisX.MajorGrid.LineDashStyle = ChartDashStyle.Dash;
            chart.ChartAreas[0].AxisX.MajorGrid.LineColor = Color.Gray;
            chart.ChartAreas[0].AxisX.MajorTickMark.Enabled = true;
            chart.ChartAreas[0].AxisX.MinorGrid.Enabled = false;
            chart.ChartAreas[0].AxisX.MinorTickMark.Enabled = true;
            // chartArea.AxisX.MinorTickMark.Interval = valueXMaxChart/10;

            chart.ChartAreas[0].AxisY.MajorGrid.Enabled = true;
            chart.ChartAreas[0].AxisY.MajorGrid.LineDashStyle = ChartDashStyle.Dash;
            chart.ChartAreas[0].AxisY.MajorGrid.LineColor = Color.Gray;
            chart.ChartAreas[0].AxisY.MajorTickMark.Enabled = true;
            chart.ChartAreas[0].AxisY.MinorGrid.Enabled = false;
            chart.ChartAreas[0].AxisY.MinorTickMark.Enabled = true;
            //chart.ChartAreas[0].AxisY.MinorTickMark.Interval = valueYMaxChart / 10;

            

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
            chart.ChartAreas[0].RecalculateAxesScale();

            // Invalidate chart
            chart.Invalidate();

        }





        /// <summary>
        /// 
        /// </summary>
        /// <param name="xvalues">Valores eje X</param>
        /// <param name="yvalues">Valores eje Y</param>
        /// <param name="limits">Indicar si se tiene que señalar el estar dentro de una zona objetivo</param>
        public void Update(IEnumerable<double> xvalues, List<string> yvalues)
        {
            //Actualizar la graficas de barras
            Series serie = chart.Series["SeriesPos"];            

           

            

//            chart.ChartAreas[0].AxisX.Minimum = -1;
      //      chart.ChartAreas[0].AxisX.Maximum = xvalues.Count();
            chart.ChartAreas[0].AxisY.Minimum = 0;
            chart.ChartAreas[0].AxisY.Maximum = xvalues.Max() + 1; 

             

            
            int pos = 0;
            string lastValue = "";
            for (int i = 0; i < xvalues.Count(); i++)
            {

                string xValue = yvalues[i];
                DataPoint pt = new DataPoint(pos, (float)Math.Ceiling(xvalues.ElementAt(i)));
                pt.AxisLabel = xValue;
              //  pt.CustomProperties = "Exploded = true";
                serie.Points.Add(pt);

                
                if (lastValue != xValue)
                    pos++;

                //  Series
                lastValue = xValue;
                
            }

           
            

                /* double[] actualStart = { 3, 1, 6, 0, 3, 2, 5.5, 2, 4 };
                 double[] actualEnd = { 4.5, 4.5, 6, 4.5, 4, 3.5, 5.5, 4.5, 4.5 };
                 ser = chart.Series[1];
                 pos = 1;
                 lastValue = "";
                 for (int i = 0; i < start.Length - 1; i++)
                 {
                     string xValue = task[i];
                     if (lastValue != xValue)
                         pos++;

                     string yValues = (dStartDate + actualStart[i]).ToString() + "," + (dStartDate + actualEnd[i]).ToString();
                     DataPoint pt = new DataPoint(pos, yValues);
                     pt.AxisLabel = xValue;
                     ser.Points.Add(pt);

                     if (dStartDate + dToday > actualStart[i])
                     {
                         if (start[i] < actualStart[i] || end[i] < actualEnd[i])
                             pt.Color = Color.Red;
                         else if (dStartDate + dToday < end[i])
                             pt.Color = Color.Lime;
                         else if (end[i] == actualEnd[i])
                             pt.Color = Color.Gray;
                     }

                     lastValue = xValue;
                 }*/

                /*     StripLine stripLine = new StripLine();
                     stripLine.StripWidth = 1;
                     stripLine.Font = new Font("Arial", 8.25F, FontStyle.Bold);
                     stripLine.Text = "Today";
                     stripLine.TextOrientation = TextOrientation.Rotated90;
                     stripLine.BorderColor = Color.Black;
                     stripLine.BackColor = Color.PaleTurquoise;
                     stripLine.IntervalOffset = dStartDate + dToday;
                     stripLine.TextAlignment = StringAlignment.Center;
                     stripLine.TextLineAlignment = StringAlignment.Near;

                     chart.ChartAreas[0].AxisY.StripLines.Add(stripLine);*/
                /*
                            foreach (DataPoint pt in chart.Series["SeriesPos"].Points)
                            {
                                if (pt.YValues[0] == pt.YValues[1])
                                    pt.Color = Color.Transparent;
                            }*/

                // Invalidate chart
                chart.Invalidate();

        }


        public void UpdateStackedBar(IEnumerable<double> xvalues, IEnumerable<double> xvalues2, List<string> yvalues)
        {
            //Actualizar la graficas de barras
            Series serieS = chart.Series["SeriesPos"];
            Series serieF = chart.Series["SeriesIdeal"];
          //  Series serieN = chart.Series["SeriesTarget"];





            //            chart.ChartAreas[0].AxisX.Minimum = -1;
            //      chart.ChartAreas[0].AxisX.Maximum = xvalues.Count();
            chart.ChartAreas[0].AxisY.Minimum = 0;
            chart.ChartAreas[0].AxisY.Maximum = 100;


            
            for (int i = 0; i < xvalues.Count(); i++)
            {

                string xValue = yvalues[i];
                DataPoint pt = new DataPoint(i, xvalues.ElementAt(i));
                pt.AxisLabel = xValue;               

                //Calcular fallos dependiento de la cantidad de repeticiones
                double fails = Math.Abs(xvalues2.ElementAt(i) - xvalues.ElementAt(i));

                //Añadir los puntos
                Random d = new Random();

                //serieS.AxisLabel = xValue;
                serieS.Points.Add(pt);
                pt = new DataPoint(i, fails);
                pt.AxisLabel = xValue;
                serieF.Points.Add(pt);

                pt = new DataPoint(i, 0);
                pt.AxisLabel = xValue;

               // serieN.Points.Add(pt);

            }

            //serieS.AxisLabel = yvalues;




            /* double[] actualStart = { 3, 1, 6, 0, 3, 2, 5.5, 2, 4 };
             double[] actualEnd = { 4.5, 4.5, 6, 4.5, 4, 3.5, 5.5, 4.5, 4.5 };
             ser = chart.Series[1];
             pos = 1;
             lastValue = "";
             for (int i = 0; i < start.Length - 1; i++)
             {
                 string xValue = task[i];
                 if (lastValue != xValue)
                     pos++;

                 string yValues = (dStartDate + actualStart[i]).ToString() + "," + (dStartDate + actualEnd[i]).ToString();
                 DataPoint pt = new DataPoint(pos, yValues);
                 pt.AxisLabel = xValue;
                 ser.Points.Add(pt);

                 if (dStartDate + dToday > actualStart[i])
                 {
                     if (start[i] < actualStart[i] || end[i] < actualEnd[i])
                         pt.Color = Color.Red;
                     else if (dStartDate + dToday < end[i])
                         pt.Color = Color.Lime;
                     else if (end[i] == actualEnd[i])
                         pt.Color = Color.Gray;
                 }

                 lastValue = xValue;
             }*/

            /*     StripLine stripLine = new StripLine();
                 stripLine.StripWidth = 1;
                 stripLine.Font = new Font("Arial", 8.25F, FontStyle.Bold);
                 stripLine.Text = "Today";
                 stripLine.TextOrientation = TextOrientation.Rotated90;
                 stripLine.BorderColor = Color.Black;
                 stripLine.BackColor = Color.PaleTurquoise;
                 stripLine.IntervalOffset = dStartDate + dToday;
                 stripLine.TextAlignment = StringAlignment.Center;
                 stripLine.TextLineAlignment = StringAlignment.Near;

                 chart.ChartAreas[0].AxisY.StripLines.Add(stripLine);*/
            /*
                        foreach (DataPoint pt in chart.Series["SeriesPos"].Points)
                        {
                            if (pt.YValues[0] == pt.YValues[1])
                                pt.Color = Color.Transparent;
                        }*/

            // Invalidate chart
            chart.Invalidate();

        }

        public void UpdateBarDistance(List<IEnumerable<double>> distX, List<IEnumerable<double>> distY, List<string> yvalues)
        {
            //Actualizar la graficas de barras
            Series serieS = chart.Series["SeriesPos"];
            Series serieF = chart.Series["SeriesIdeal"];
            //  Series serieN = chart.Series["SeriesTarget"];


            //Calcular distancia recorrida por el usuario
            List<float> distances = new List<float>();
            for(int i=0; i< yvalues.Count(); i++)
            {

            }
/*
valor = size(x, 1);

            value = 0;

% Punto inicial
 ptoi = [x(1) y(1)];

            for i = 2:valor
            
                ptof = [x(i) y(i)];
                d = norm(ptof - ptoi);
            

                % Suma el incremento
            
                value = value + d;


    % Actualiza punto inicial
    ptoi = ptof;
            end




            //            chart.ChartAreas[0].AxisX.Minimum = -1;
            //      chart.ChartAreas[0].AxisX.Maximum = xvalues.Count();
            chart.ChartAreas[0].AxisY.Minimum = 0;
            chart.ChartAreas[0].AxisY.Maximum = 100;



            for (int i = 0; i < xvalues.Count(); i++)
            {

                string xValue = yvalues[i];
                DataPoint pt = new DataPoint(i, xvalues.ElementAt(i));
                pt.AxisLabel = xValue;

                //Calcular fallos dependiento de la cantidad de repeticiones
                double fails = Math.Abs(xvalues2.ElementAt(i) - xvalues.ElementAt(i));

                //Añadir los puntos
                Random d = new Random();

                //serieS.AxisLabel = xValue;
                serieS.Points.Add(pt);
                pt = new DataPoint(i, fails);
                pt.AxisLabel = xValue;
                serieF.Points.Add(pt);

                pt = new DataPoint(i, 0);
                pt.AxisLabel = xValue;

                // serieN.Points.Add(pt);

            }

            //serieS.AxisLabel = yvalues;




     */

            // Invalidate chart
            chart.Invalidate();

        }

     



        //**********************Configurar tipo de grafica****************************//
        //****************************************************************************//
        private void ConfigureSuccesBar()
        {
            //Titulo del eje Y
            chart.ChartAreas[0].AxisY.Title = "Aciertos/Fallos (%)";
            chart.ChartAreas[0].AxisX.Title = "Actividades";

            //Configura tipo de grafica
            chart.Series["SeriesPos"].ChartType = SeriesChartType.StackedBar100;
            chart.Series["SeriesIdeal"].ChartType = SeriesChartType.StackedBar100;
         //   chart.Series["SeriesTarget"].ChartType = SeriesChartType.StackedBar100;

            // Configurar etiquetas de los aciertos y fallos
            chart.Series["SeriesPos"].IsValueShownAsLabel = true;
            chart.Series["SeriesPos"]["LabelStyle"] = "Top";
            chart.Series["SeriesPos"].SmartLabelStyle.Enabled = false;
            chart.Series["SeriesPos"].LabelAngle = 0;
            chart.Series["SeriesIdeal"].IsValueShownAsLabel = true;
            chart.Series["SeriesIdeal"]["LabelStyle"] = "Top";
            chart.Series["SeriesIdeal"].SmartLabelStyle.Enabled = false;
            chart.Series["SeriesIdeal"].LabelAngle = 0;
         //   chart.Series["SeriesTarget"].IsValueShownAsLabel = false;            

            //Color
            chart.Series["SeriesPos"].Color = Color.Green;
            chart.Series["SeriesIdeal"].Color = Color.Red;
           // chart.Series["SeriesTarget"].Color = Color.Black;

            //Configurar ejes           
            chart.ChartAreas[0].AxisY.LabelStyle.Interval = 10;
            chart.ChartAreas[0].AxisY.LabelStyle.IntervalType = DateTimeIntervalType.Number;
            
            chart.ChartAreas[0].AxisX.IsMarginVisible = true;
            chart.ChartAreas[0].AxisY.IsMarginVisible = false;

            //Configurar grid            
            chart.ChartAreas[0].AxisX.MajorGrid.Enabled = false;            
            chart.ChartAreas[0].AxisX.MajorGrid.LineDashStyle = ChartDashStyle.Dash;
            chart.ChartAreas[0].AxisX.MajorGrid.LineColor = Color.Gray;
            chart.ChartAreas[0].AxisX.MinorGrid.Enabled = false;


            chart.ChartAreas[0].AxisY.MajorGrid.Enabled = true;
            chart.ChartAreas[0].AxisY.MajorGrid.Interval = 10;
            chart.ChartAreas[0].AxisY.MajorGrid.LineDashStyle = ChartDashStyle.Dash;
            chart.ChartAreas[0].AxisY.MajorGrid.LineColor = Color.Gray;
            chart.ChartAreas[0].AxisY.MinorGrid.Enabled = false;

            //Configurar ticks de señalización
            chart.ChartAreas[0].AxisX.MajorTickMark.Enabled = false;
            chart.ChartAreas[0].AxisX.MinorTickMark.Enabled = false;

            chart.ChartAreas[0].AxisY.MajorTickMark.Enabled = true;
            chart.ChartAreas[0].AxisY.MajorTickMark.Interval = 10;
            chart.ChartAreas[0].AxisY.MinorTickMark.Enabled = false;
            


        }


        private void ConfigureTimeBar()
        {
            //Titulo del eje Y            
            chart.ChartAreas[0].AxisX.Title = "Actividades";
            chart.ChartAreas[0].AxisY.Title = "Tiempo";
            

            // Configurar etiquetas de los aciertos y fallos
            chart.Series["SeriesPos"].IsValueShownAsLabel = true;
            chart.Series["SeriesPos"]["LabelStyle"] = "Top";
            chart.Series["SeriesPos"].SmartLabelStyle.Enabled = false;
            chart.Series["SeriesPos"].LabelAngle = 0;
            chart.Series["SeriesIdeal"].IsValueShownAsLabel = true;           
            //   chart.Series["SeriesTarget"].IsValueShownAsLabel = false;            

           
           
            // chart.Series["SeriesTarget"].Color = Color.Black;

            //Configurar ejes           
            chart.ChartAreas[0].AxisY.LabelStyle.Interval = 10;
            chart.ChartAreas[0].AxisY.LabelStyle.IntervalType = DateTimeIntervalType.Number;

            chart.ChartAreas[0].AxisX.IsMarginVisible = true;
            chart.ChartAreas[0].AxisY.IsMarginVisible = false;

            //Configurar grid            
            chart.ChartAreas[0].AxisX.MajorGrid.Enabled = false;
            chart.ChartAreas[0].AxisX.MajorGrid.LineDashStyle = ChartDashStyle.Dash;
            chart.ChartAreas[0].AxisX.MajorGrid.LineColor = Color.Gray;
            chart.ChartAreas[0].AxisX.MinorGrid.Enabled = false;


            chart.ChartAreas[0].AxisY.MajorGrid.Enabled = true;
            chart.ChartAreas[0].AxisY.MajorGrid.Interval = 10;
            chart.ChartAreas[0].AxisY.MajorGrid.LineDashStyle = ChartDashStyle.Dash;
            chart.ChartAreas[0].AxisY.MajorGrid.LineColor = Color.Gray;
            chart.ChartAreas[0].AxisY.MinorGrid.Enabled = false;

            //Configurar ticks de señalización
            chart.ChartAreas[0].AxisX.MajorTickMark.Enabled = false;
            chart.ChartAreas[0].AxisX.MinorTickMark.Enabled = false;

            chart.ChartAreas[0].AxisY.MajorTickMark.Enabled = true;
            chart.ChartAreas[0].AxisY.MajorTickMark.Interval = 10;
            chart.ChartAreas[0].AxisY.MinorTickMark.Enabled = false;



        }

        private void ConfigureDistanceBar()
        {
            //Titulo del eje Y            
            chart.ChartAreas[0].AxisX.Title = "Actividades";
            chart.ChartAreas[0].AxisY.Title = "Distancia de Movimiento";


            // Configurar etiquetas de los aciertos y fallos
            chart.Series["SeriesPos"].IsValueShownAsLabel = true;
            chart.Series["SeriesPos"]["LabelStyle"] = "Top";
            chart.Series["SeriesPos"].SmartLabelStyle.Enabled = false;
            chart.Series["SeriesPos"].LabelAngle = 0;
            chart.Series["SeriesIdeal"].IsValueShownAsLabel = true;
            //   chart.Series["SeriesTarget"].IsValueShownAsLabel = false;            



            // chart.Series["SeriesTarget"].Color = Color.Black;

            //Configurar ejes           
            chart.ChartAreas[0].AxisY.LabelStyle.Interval = 5;
            chart.ChartAreas[0].AxisY.LabelStyle.IntervalType = DateTimeIntervalType.Number;

            chart.ChartAreas[0].AxisX.IsMarginVisible = true;
            chart.ChartAreas[0].AxisY.IsMarginVisible = false;

            //Configurar grid            
            chart.ChartAreas[0].AxisX.MajorGrid.Enabled = false;
            chart.ChartAreas[0].AxisX.MajorGrid.LineDashStyle = ChartDashStyle.Dash;
            chart.ChartAreas[0].AxisX.MajorGrid.LineColor = Color.Gray;
            chart.ChartAreas[0].AxisX.MinorGrid.Enabled = false;


            chart.ChartAreas[0].AxisY.MajorGrid.Enabled = true;
            chart.ChartAreas[0].AxisY.MajorGrid.Interval = 5;
            chart.ChartAreas[0].AxisY.MajorGrid.LineDashStyle = ChartDashStyle.Dash;
            chart.ChartAreas[0].AxisY.MajorGrid.LineColor = Color.Gray;
            chart.ChartAreas[0].AxisY.MinorGrid.Enabled = false;

            //Configurar ticks de señalización
            chart.ChartAreas[0].AxisX.MajorTickMark.Enabled = false;
            chart.ChartAreas[0].AxisX.MinorTickMark.Enabled = false;

            chart.ChartAreas[0].AxisY.MajorTickMark.Enabled = true;
            chart.ChartAreas[0].AxisY.MajorTickMark.Interval = 5;
            chart.ChartAreas[0].AxisY.MinorTickMark.Enabled = false;



        }
        //****************************************************************************//
        //****************************************************************************//






    }
}
