using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;
using System.Windows.Forms;
using System.Windows.Forms.DataVisualization.Charting;

namespace REVIREPanels.Componentes
{
    //Clase para controlar el estado de la grafica proporcionada
    class ChartControl
    {
        private Chart chart;

        private double valueXMaxChart; //Valor maximo que alcanza la gráfica
        private double valueXMinChart; //Valor minimo que alcanza la gráfica
        private double valueYMaxChart; //Valor maximo que alcanza la gráfica
        private double valueYMinChart; //Valor minimo que alcanza la gráfica
        private double valueXMidChart; //Valor medio eje X
        private double valueYMidChart; //Valor medio eje Y

        private int pointIndex = 0;
        // Define some variables
        private int numberOfPointsInChart = 25;
        private int numberOfPointsAfterRemoval = 25;

        private int typeChart;

        public ChartControl(Chart inChart, int tipo, double[] values)
        {
            chart = inChart;

            //Valores limites
            valueXMaxChart = values[0];
            valueXMinChart = values[1];

            valueYMaxChart = values[2];
            valueYMinChart = values[3];

            //Valores medio de los ejes de posicion
            valueXMidChart = (valueXMaxChart + valueXMinChart) / 2.0;
            valueYMidChart = (valueYMaxChart + valueYMinChart) / 2.0;

            typeChart = tipo;

            //Inicializa tipo de grafica
            if (tipo == 0)
                InitChartBubble();
            else if (tipo == 1)
                InitChartLine();
        }

        public double GetXMaxChart() { return valueYMaxChart; }
        public double GetYMinChart() { return valueYMinChart; }
        public double GetXMidChart() { return valueXMidChart; }
        public double GetYMidChart() { return valueYMidChart; }

        private void InitChartLine()
        {
            // Grafica inicial
            chart.BorderSkin.SkinStyle = BorderSkinStyle.None;

            Series series = chart.Series.ElementAt(0);// SeriesAdd("s1");
            ChartArea chartArea = chart.ChartAreas[0];

            series.ChartType = SeriesChartType.Spline;
            series.BorderWidth = 2;


            chartArea.IsSameFontSizeForAllAxes = true;


            // Disable X axis margin
            chartArea.AxisY.IsMarginVisible = false;

            /*chartArea.AxisX.Interval = 5;
            chartArea.AxisX.IntervalType = DateTimeIntervalType.Number;*/
            chartArea.AxisY.Interval = valueYMaxChart / 2;
            chartArea.AxisY.IntervalType = DateTimeIntervalType.Number;


            /* chartArea.AxisX.Minimum = valueXMinChart;
             chartArea.AxisX.Maximum = valueXMaxChart;*/
            chartArea.AxisY.Minimum = valueYMinChart;
            chartArea.AxisY.Maximum = valueYMaxChart;

            //Deshabilitar etiquetas del eje X
            chartArea.AxisX.LabelStyle.Enabled = false;

            // Enable all elements
            chartArea.AxisX.MajorGrid.Enabled = false;
            chartArea.AxisX.MajorGrid.LineDashStyle = ChartDashStyle.Dash;
            chartArea.AxisX.MajorGrid.LineColor = Color.Gray;
            chartArea.AxisX.MajorTickMark.Enabled = false;
            chartArea.AxisX.MinorGrid.Enabled = false;
            chartArea.AxisX.MinorTickMark.Enabled = false;

            chartArea.AxisY.MajorGrid.Enabled = true;
            chartArea.AxisY.MajorGrid.LineDashStyle = ChartDashStyle.Dash;
            chartArea.AxisY.MajorGrid.LineColor = Color.Gray;
            chartArea.AxisY.MajorTickMark.Enabled = true;
            chartArea.AxisY.MinorGrid.Enabled = false;
            chartArea.AxisY.MinorTickMark.Enabled = false;

            //Titulo ejes
            /*chartArea.AxisY.Title = units;
            chartArea.AxisY.TitleAlignment = StringAlignment.Near;*/

            /*chartArea.AxisX.TitleFont = new Font("Times New Roman", 12, FontStyle.Bold);
            chartArea.AxisY.TitleFont = new Font("Times New Roman", 12, FontStyle.Bold);*/
        }

        private void InitChartBubble()
        {

            // Grafica inicial
            Series series = chart.Series.ElementAt(0);// SeriesAdd("s1");
            ChartArea chartArea = chart.ChartAreas[0];

            series.ChartType = SeriesChartType.Bubble;
            series.MarkerStyle = MarkerStyle.Circle;
            series["BubbleMaxSize"] = "10";
            series.BorderWidth = 2;


            series.Points.Add(new DataPoint(valueXMidChart, valueYMidChart));


            chartArea.IsSameFontSizeForAllAxes = true;

            /*
            for (int i = 0; i < 10; i++)
                chart1.Series["s2"].Points.AddXY(i, i + 1);*/
            // Disable X axis margin
            //chart1.ChartAreas["ChartArea1"].AxisX.IsMarginVisible = true;

            chartArea.AxisX.Interval = 5;
            chartArea.AxisX.IntervalType = DateTimeIntervalType.Number;
            chartArea.AxisY.Interval = valueYMaxChart / 2;
            chartArea.AxisY.IntervalType = DateTimeIntervalType.Number;
            /* chart1.ChartAreas["ChartArea1"].AxisX.IntervalOffset = 1;
               chart1.ChartAreas["ChartArea1"].AxisX.IntervalType = DateTimeIntervalType.Number;*/

            chartArea.AxisX.Minimum = valueXMinChart;
            chartArea.AxisX.Maximum = valueXMaxChart;
            chartArea.AxisY.Minimum = valueYMinChart;
            chartArea.AxisY.Maximum = valueYMaxChart;

            // Enable all elements
            chartArea.AxisX.MajorGrid.Enabled = true;
            chartArea.AxisX.MajorGrid.LineDashStyle = ChartDashStyle.Dash;
            chartArea.AxisX.MajorGrid.LineColor = Color.Gray;
            chartArea.AxisX.MajorTickMark.Enabled = true;
            chartArea.AxisX.MinorGrid.Enabled = false;
            chartArea.AxisX.MinorTickMark.Enabled = true;
            // chartArea.AxisX.MinorTickMark.Interval = valueXMaxChart/10;

            chartArea.AxisY.MajorGrid.Enabled = true;
            chartArea.AxisY.MajorGrid.LineDashStyle = ChartDashStyle.Dash;
            chartArea.AxisY.MajorGrid.LineColor = Color.Gray;
            chartArea.AxisY.MajorTickMark.Enabled = true;
            chartArea.AxisY.MinorGrid.Enabled = false;
            chartArea.AxisY.MinorTickMark.Enabled = true;
            chartArea.AxisY.MinorTickMark.Interval = valueYMaxChart / 10;

            //Cruza los ejes
            chartArea.AxisX.Crossing = valueXMidChart; //Valor medio
            chartArea.AxisY.Crossing = valueYMidChart; //Valor medio
            chartArea.AxisX.IsMarksNextToAxis = true;

            // Position the Y axis labels and tick marks next to the axis
            chartArea.AxisY.IsMarksNextToAxis = true;

            //Etiquetas personalizadas en el eje X  
            double axisdouble = -1.5;
            for (int i = -10; i <= 10; i = i + 5)
            {
                chartArea.AxisX.CustomLabels.Add(axisdouble, axisdouble + 5, i.ToString());
                axisdouble = axisdouble + 5;
            }

            //Titulo ejes
            chartArea.AxisX.Title = "X-Axis (cm)";
            chartArea.AxisX.TitleAlignment = StringAlignment.Near;
            chartArea.AxisY.Title = "Y-Axis (cm)";
            chartArea.AxisY.TitleAlignment = StringAlignment.Near;

            chartArea.AxisX.TitleFont = new Font("Times New Roman", 12, FontStyle.Bold);
            chartArea.AxisY.TitleFont = new Font("Times New Roman", 12, FontStyle.Bold);

        }

        public void Update(double y)
        {
            // Simulate adding new data points
            chart.Series[0].Points.AddXY(pointIndex + 1, y);
            ++pointIndex;

            // Adjust Y & X axis scale
            chart.ResetAutoValues();

            // Keep a constant number of points by removing them from the left
            while (chart.Series[0].Points.Count > numberOfPointsInChart)
            {
                // Remove data points on the left side
                while (chart.Series[0].Points.Count > numberOfPointsAfterRemoval)
                {
                    chart.Series[0].Points.RemoveAt(0);
                }

                // Adjust X axis scale
                //    chartVelX.ChartAreas["Default"].AxisX.Minimum = pointIndex - numberOfPointsAfterRemoval;
                //    chartVelX.ChartAreas["Default"].AxisX.Maximum = chartVelX.ChartAreas["Default"].AxisX.Minimum + numberOfPointsInChart;

                //  chartVelY.ChartAreas["Default"].AxisX.Minimum = pointIndex - numberOfPointsAfterRemoval;
                //  chartVelY.ChartAreas["Default"].AxisX.Maximum = chartVelY.ChartAreas["Default"].AxisX.Minimum + numberOfPointsInChart;
            }



            // Invalidate chart
            chart.Invalidate();

        }

        public void UpdateRandom()
        {
            // Simulate adding new data points
            chart.Series[0].Points.AddXY(pointIndex + 1, Math.Tan(pointIndex));
            ++pointIndex;

            // Adjust Y & X axis scale
            chart.ResetAutoValues();

            // Keep a constant number of points by removing them from the left
            while (chart.Series[0].Points.Count > numberOfPointsInChart)
            {
                // Remove data points on the left side
                while (chart.Series[0].Points.Count > numberOfPointsAfterRemoval)
                {
                    chart.Series[0].Points.RemoveAt(0);
                }

                // Adjust X axis scale
                //    chartVelX.ChartAreas["Default"].AxisX.Minimum = pointIndex - numberOfPointsAfterRemoval;
                //    chartVelX.ChartAreas["Default"].AxisX.Maximum = chartVelX.ChartAreas["Default"].AxisX.Minimum + numberOfPointsInChart;

                //  chartVelY.ChartAreas["Default"].AxisX.Minimum = pointIndex - numberOfPointsAfterRemoval;
                //  chartVelY.ChartAreas["Default"].AxisX.Maximum = chartVelY.ChartAreas["Default"].AxisX.Minimum + numberOfPointsInChart;
            }
            // Invalidate chart
            chart.Invalidate();
        }

        public void Reset()
        {
            // Remove data points 
            chart.Series[0].Points.Clear();

            // Invalidate chart
            chart.Invalidate();

            //Añade un punto extra para que no desaparezcan los axis
            chart.Series[0].Points.AddY(0);

            //Reiniciar indice X
            pointIndex = 0;
        }

        public double[] GetMousePositionInChart(MouseEventArgs e)
        {
            int coordinate = e.Y;
            if (coordinate < 0)
                coordinate = 0;
            if (coordinate > chart.Size.Height - 1)
                coordinate = chart.Size.Height - 1;

            int coordinateX = e.X;
            if (coordinateX < 0)
                coordinateX = 0;
            if (coordinateX > chart.Size.Width - 1)
                coordinateX = chart.Size.Width - 1;

            // Calculate new Y value from current cursor position
            ChartArea chartArea = chart.ChartAreas[0];
            double yValue = chartArea.AxisY.PixelPositionToValue(coordinate);
            yValue = Math.Min(yValue, chartArea.AxisY.Maximum);
            yValue = Math.Max(yValue, chartArea.AxisY.Minimum);

            // Calculate new x value from current cursor position
            double xValue = chartArea.AxisX.PixelPositionToValue(coordinateX);
            xValue = Math.Min(xValue, chartArea.AxisX.Maximum);
            xValue = Math.Max(xValue, chartArea.AxisX.Minimum);

            double[] values = new double[2] { xValue, yValue };
            return values;

        }

        public void UpdateBubble(double x, double y)
        {
            double valueX = x + valueXMidChart;
            double valueY = y + valueYMidChart;
            //Ajuste en caso de sobrepasar valores limite visuales
            if (valueX < valueXMinChart)
                valueX = valueXMinChart;
            else if (valueX > valueXMaxChart)
                valueX = valueXMaxChart;

            if (valueY < valueYMinChart)
                valueY = valueYMinChart;
            else if (valueY > valueYMaxChart)
                valueY = valueYMaxChart;

            //Actualizar grafica
            chart.Series[0].Points.Clear();
            chart.Series[0].Points.Add(new DataPoint(valueX, valueY));

            // Invalidate chart
            chart.Invalidate();

        }
    }
}
