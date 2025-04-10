using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;
using System.Windows.Forms;
using System.Windows.Forms.DataVisualization.Charting;
using REVIREPanels.Modelos;

namespace REVIREPanels.Graficas
{
    //Clase para controlar el estado de la grafica proporcionada con forma de tarta
    //para el tiempo de sesion

    class SessionPieChart
    {
        //Elemento principal
        private Chart chart;

        //Cursor dentro de la grafica
        private System.Windows.Forms.Cursor cCursor;

        //Para que cuando se ejecute en el panel pase los datos al control padre
        public Delegate MouseMovementInChart;
        public Delegate MouseClickInChart;

        //Flags
        private bool haveLegend = false;
        private bool haveMouseEvent = false;

        /// <summary>
        /// Constructor del controlador de graficas con forma de tarta
        /// </summary>
        /// <param name="inChart">Elemento gráfico</param>
        /// <param name="cursor">Forma del cursor cuando pasa por encima</param>
        /// <param name="legend">Indica si se añade leyenda</param>
        /// <param name="mouseevent">Indica si se se tienen que generar evento del mouse</param>
        public SessionPieChart(Chart inChart, int tipo, System.Windows.Forms.Cursor cursor, bool legend, bool mouseevent)
        {
            //Inicializar componente
            chart = inChart;
            cCursor = cursor;

            //Flags
            haveLegend = legend;
            haveMouseEvent = mouseevent;

            //Inicializa gráfica visual            
            InitChartPie();
        }

        public void Dispose()
        {
            chart.MouseMove -= Chart_MouseMove;
            chart.MouseDown -= Chart_MouseDown;
            chart.Click -= Chart_Click;
        }           
            

        
        private void InitChartPie()
        {
            // Grafica inicial    
            Series series = chart.Series.ElementAt(0);// SeriesAdd("s1");
            ChartArea chartArea = chart.ChartAreas[0];

            //Tipo de gráfica
            series.ChartType = SeriesChartType.Pie;


            //Mismo tamaño de letra para ambos ejes
            //chartArea.IsSameFontSizeForAllAxes = true;

            // Disable X axis margin
            chartArea.AxisY.IsMarginVisible = true;

            //Deshabilitar etiquetas del eje X
            chartArea.AxisX.LabelStyle.Enabled = false;
           
            //Crear paleta de colores transparente - BrightPaste trasparente            
            string[] strColors = new string[] { "#418CF0", "#FCB441", "#E0400A", "#056492",
                "#BFBFBF", "#1A3B69", "#FFE382", "#129CDD", "#CA6B4B", "#005CDB",
                "#F3D288", "#506381", "#F1B9A8", "#E0830A", "#7893BE" };
            chart.PaletteCustomColors = new Color[strColors.Length];

            int i = 0;
            foreach (string colorstr in strColors)
            {
                Color color = ColorTranslator.FromHtml(colorstr);
                chart.PaletteCustomColors[i] = Color.FromArgb(220,color);
                i++;
            }


            // Set the threshold under which all points will be collected
            /* chart.Series[0]["CollectedThreshold"] = comboBoxCollectedThreshold.GetItemText(comboBoxCollectedThreshold.SelectedItem);

             // Set the label of the collected pie slice
             chart.Series[0]["CollectedLabel"] = textBoxCollectedLabel.Text;

             // Set the legend text of the collected pie slice
             chart.Series[0]["CollectedLegendText"] = textBoxCollectedLegend.Text;

             // Set the collected pie slice to be exploded
             chart.Series[0]["CollectedSliceExploded"] = checkBoxShowExploded.Checked.ToString();

             // Set collected color
             chart.Series[0]["CollectedColor"] = comboBoxCollectedColor.GetItemText(comboBoxCollectedColor.SelectedItem);*/


            //  chart.Palette = ChartColorPalette.BrightPastel;


            //Tamaño de la tarta dentro del lienzo
            /*  chart.ChartAreas[0].InnerPlotPosition.Auto = false;
              chart.ChartAreas[0].InnerPlotPosition.Height = 100F;
              chart.ChartAreas[0].InnerPlotPosition.Width = 100F;
              chart.ChartAreas[0].InnerPlotPosition.X = 0F;
              chart.ChartAreas[0].InnerPlotPosition.Y = 0F;*/

            chart.ChartAreas[0].Position.Auto = false;
            chart.ChartAreas[0].Position.X = 0F;
            chart.ChartAreas[0].Position.Y = 0F;
            chart.ChartAreas[0].Position.Width = 100F;
            chart.ChartAreas[0].Position.Height = 100F;


            //Establecer los registros de eventos     
            if (haveMouseEvent)
            {
                chart.MouseMove += new MouseEventHandler(Chart_MouseMove);            
            chart.MouseDown += new MouseEventHandler(Chart_MouseDown);
            chart.Click += new EventHandler(Chart_Click);

            //Caracteristicas
            chart.ChartAreas[0].CursorX.IsUserEnabled = true;
            chart.ChartAreas[0].CursorX.IsUserSelectionEnabled = true;
            chart.ChartAreas[0].CursorY.IsUserEnabled = true;
            chart.ChartAreas[0].CursorY.IsUserSelectionEnabled = true;
            }


            //Tamaño de la tarta dentro del lienzo
            chart.ChartAreas[0].InnerPlotPosition.Auto = false;
            chart.ChartAreas[0].InnerPlotPosition.Height = 70F;
            chart.ChartAreas[0].InnerPlotPosition.Width = 80F;
            chart.ChartAreas[0].InnerPlotPosition.X = 10F;
            chart.ChartAreas[0].InnerPlotPosition.Y = 15F;

            //chart.ChartAreas[0].AxisX.StripLines.Add(stripLine1);
            StripLine stripLine1 = new StripLine();
            chart.ChartAreas[0].AxisX.LabelStyle.Font = new System.Drawing.Font("Trebuchet MS", 8.25F, System.Drawing.FontStyle.Bold);
            chart.ChartAreas[0].AxisX.LineColor = System.Drawing.Color.FromArgb(((int)(((byte)(64)))), ((int)(((byte)(64)))), ((int)(((byte)(64)))), ((int)(((byte)(64)))));
            chart.ChartAreas[0].AxisX.MajorGrid.LineColor = System.Drawing.Color.FromArgb(((int)(((byte)(64)))), ((int)(((byte)(64)))), ((int)(((byte)(64)))), ((int)(((byte)(64)))));
            chart.ChartAreas[0].AxisX.ScaleView.Position = 3;
            chart.ChartAreas[0].AxisX.ScaleView.Size = 30;
            stripLine1.Interval = 20;
            stripLine1.StripWidth = 5;
            chart.ChartAreas[0].AxisX.StripLines.Add(stripLine1);
            chart.ChartAreas[0].AxisX2.Enabled = System.Windows.Forms.DataVisualization.Charting.AxisEnabled.False;
            chart.ChartAreas[0].AxisY.LabelStyle.Font = new System.Drawing.Font("Trebuchet MS", 8.25F, System.Drawing.FontStyle.Bold);
            chart.ChartAreas[0].AxisY.LineColor = System.Drawing.Color.FromArgb(((int)(((byte)(64)))), ((int)(((byte)(64)))), ((int)(((byte)(64)))), ((int)(((byte)(64)))));
            chart.ChartAreas[0].AxisY.MajorGrid.LineColor = System.Drawing.Color.FromArgb(((int)(((byte)(64)))), ((int)(((byte)(64)))), ((int)(((byte)(64)))), ((int)(((byte)(64)))));
            chart.ChartAreas[0].AxisY.ScaleView.Position = 5;
            chart.ChartAreas[0].AxisY.ScaleView.Size = 10;
            chart.ChartAreas[0].AxisY2.Enabled = System.Windows.Forms.DataVisualization.Charting.AxisEnabled.False;

            //Gestiona la leyenda en caso de ser necesaria
            if (haveLegend)
            {
                Legend legend1 = new Legend();
                legend1.BackColor = System.Drawing.Color.Transparent;
                legend1.Font = new System.Drawing.Font("Trebuchet MS", 8.25F, System.Drawing.FontStyle.Bold);
                legend1.Alignment = StringAlignment.Center;
                legend1.Docking = Docking.Right;
                legend1.IsTextAutoFit = false;
                legend1.Name = "Default";
                chart.Legends.Add(legend1);

                chart.ChartAreas[0].InnerPlotPosition.Height = 80F;
                chart.ChartAreas[0].InnerPlotPosition.Y = 10F;
            }
            else
            {
                // Set labels style
                chart.Series[0]["PieLabelStyle"] = "Outside";
            }

            //Esatblecer modo de dibujo
            chart.Series[0]["PieDrawingStyle"] = "SoftEdge";


            
            chart.Series[0].SmartLabelStyle.Enabled = true;
            chart.Series[0].SmartLabelStyle.AllowOutsidePlotArea = LabelOutsidePlotAreaStyle.No;
            chart.Series[0].IsValueShownAsLabel = true;

            chart.Series[0].XValueType = ChartValueType.String;
            chart.Series[0].YValueType = ChartValueType.Double;
        }

        public void UpdatePie(List<Actividad> listact)
        {
            //Convertir lista a datos
            DataPoint[] piedata = ConvertActToDataPoint(listact);

            //Añadir todos los puntos
            foreach (DataPoint point in piedata)
                chart.Series[0].Points.Add(point);

            // Invalidate chart
            chart.Invalidate();



        }
              

        

        public void Reset()
        {
            // Remove data points 
            chart.Series[0].Points.Clear();

            // Invalidate chart
            chart.Invalidate();
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


        /// <summary>
        /// Mouse Down Event
        /// </summary>
        private void Chart_MouseDown(object sender, System.Windows.Forms.MouseEventArgs e)
        {
            // Call Hit Test Method
            HitTestResult result = chart.HitTest(e.X, e.Y);

            // Exit event if no item was clicked on (PointResult will be negative one)
            if (result.PointIndex < 0)
                return;

            // Reset Data Point Attributes
            foreach (DataPoint point in chart.Series[0].Points)
            {
                point.LabelBackColor = Color.Transparent;
            }

            // Check if data point is already exploded.
            bool exploded = (chart.Series[0].Points[result.PointIndex].CustomProperties == "Exploded=true") ? true : false;

            // Remove all exploded attributes
            foreach (DataPoint point in chart.Series[0].Points)
            {
                point.CustomProperties = "";
            }

            // If data point is already exploded get out.
            if (exploded)
            {
                chart.Series[0].Points[result.PointIndex].LabelBackColor = Color.Transparent;
                MouseClickInChart.DynamicInvoke(result.PointIndex, 0);
                return;
            }

            // If data point is selected
            if (result.ChartElementType == ChartElementType.DataPoint)
            {
                // Set Attribute
                DataPoint point = chart.Series[0].Points[result.PointIndex];
                point.CustomProperties = "Exploded = true";
            }

            if (result.ChartElementType == ChartElementType.DataPointLabel)
            {
                // Set Attribute
                DataPoint point = chart.Series[0].Points[result.PointIndex];
                point.CustomProperties = "Exploded = true";
            }

            // If legend item is selected
            if (result.ChartElementType == ChartElementType.LegendItem)
            {
                // Set Attribute
                DataPoint point = chart.Series[0].Points[result.PointIndex];
                point.CustomProperties = "Exploded = true";
            }

            
            chart.Series[0].Points[result.PointIndex].LabelBackColor = Color.Green;
            

            MouseClickInChart.DynamicInvoke(result.PointIndex, 1);
        }

        /// <summary>
        /// Mouse Move Event
        /// </summary>
        private void Chart_MouseMove(object sender, System.Windows.Forms.MouseEventArgs e)
        {
            // Call Hit Test Method
            HitTestResult result = chart.HitTest(e.X, e.Y);

            bool touching = false;
            // Reset Data Point Attributes
            foreach (DataPoint point in chart.Series[0].Points)
            {
                point.BackSecondaryColor = Color.Black;
                point.BackHatchStyle = ChartHatchStyle.None;
                point.BorderWidth = 1;
            }

            // If a Data Point or a Legend item is selected.
            if
            (result.ChartElementType == ChartElementType.DataPoint ||
                result.ChartElementType == ChartElementType.LegendItem ||
                result.ChartElementType == ChartElementType.DataPointLabel)
            {
                // Set cursor type 
                cCursor = Cursors.Hand;

                // Find selected data point
                DataPoint point = chart.Series[0].Points[result.PointIndex];

                // Set End Gradient Color to White
                point.BackSecondaryColor = Color.White;

                // Set selected hatch style
                point.BackHatchStyle = ChartHatchStyle.Percent25;

                // Increase border width
                point.BorderWidth = 5;
                    point.BorderColor = Color.Black;

                touching = true;
            }
            else
            {
                // Set default cursor
                cCursor = Cursors.Default;
                touching = false;
            }

            MouseMovementInChart.DynamicInvoke(touching);

        }

        private void Chart_Click(object sender, System.EventArgs e)
        {

        }

        /// <summary>
        /// Convertir lista de actividadades en puntos para la grafica
        /// </summary>
        /// <param name="activities">Lista de actividades</param>
        /// <returns>Datos para la grafica</returns>
        private DataPoint[] ConvertActToDataPoint(List<Actividad> activities)
        {
            DataPoint[] data = new DataPoint[activities.Count]; //Almacena memoria
            List<string> nametask = GetListNameTask(activities);

            for (int i = 0; i < activities.Count; i++)
            {


                //Segundos
                string timeused = activities[i].DatosRecibidos.Split(' ')[0];
                float sec = float.Parse(timeused); 

                //Definir el punto
                DataPoint cpoint = new DataPoint(0, sec);
                cpoint.CustomProperties = "OriginalPointIndex=" + i;
                cpoint.LegendText = nametask[i];
                cpoint.Label = nametask[i];
                cpoint.BorderWidth = 1;
                cpoint.BorderColor = Color.Black;

                //Guardar
                data[i] = cpoint;
            }
            return data;
        }

        /// <summary>
        /// Obtiene una lista con los nombre de los ejercicios realizados en una sesion
        /// </summary>
        /// <param name="activities"></param>
        /// <returns></returns>
        private List<string> GetListNameTask(List<Actividad> activities)
        {
            List<string> listtask = new List<string>(); //Almacena memoria

            for (int i = 0; i < activities.Count; i++)
            {
                //Definir el punto                                                
                string name = activities[i].Nombre.Trim() + "_" + activities[i].Fecha.ToShortTimeString(); ;

                //Guardar
                listtask.Add(name);
            }
            return listtask;
        }
    }
}
