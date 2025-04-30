using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Numerics;
using REVIREPanels.Estadisticas.Graficas;
using REVIREPanels.Graficas;
using REVIREPanels.Componentes;
using System.Windows.Forms.DataVisualization.Charting;

namespace REVIREPanels.Estadisticas.Paneles
{
    public partial class PanelInfoPosition : UserControl
    {
        public bool IsSelectedPie { get; set; } //Flag si se ha seleccionado una actividad

        private PositionChart poschart; //Controlador de la grafica

        private Trial[] trials;

        private double last; //Ultimo valor de la grafica
        private float distance; //Distancia maxima de movimiento objetivo   

        List<List<float[]>> ideal = new List<List<float[]>>();
        public PanelInfoPosition()
        {
            InitializeComponent();

            // Set Antialiasing mode
            chartData.AntiAliasing = AntiAliasingStyles.All;            
            chartData.TextAntiAliasingQuality = TextAntiAliasingQuality.High;


            ideal = DTW.GenerateIdealPath(350, 1000);

            
           
        }

        public void Setting(double c, float d, float s)
        {
            last = c;
            distance = d;


            //Calcular distancias totales
            trials = new Trial[BinaryDataManager.listTrialDataRobot[0].Count];
            List<DataRobot[]> lista = BinaryDataManager.listTrialDataRobot[0];
            for (int i = 0; i < lista.Count; i++)
            {
                int id_ideal = i % 9;
                Trial cTrial = new Trial(lista[i], ideal[id_ideal], 10f, 350f);
                trials[i] = cTrial;
            }
        }

        private void chartData_AxisScrollBarClicked(object sender, ScrollBarEventArgs e)
        {
            if (e.ButtonType == ScrollBarButtonType.ZoomReset)
            {
                // Event is handled, no more processing required
                e.IsHandled = true;

                // Reset zoom on X and Y axis
                chartData.ChartAreas["ChartArea1"].AxisX.ScaleView.ZoomReset();
                chartData.ChartAreas["ChartArea1"].AxisY.ScaleView.ZoomReset();
            }
        }

       
        //**********************Visualizar datos de posicion**************************//
        //****************************************************************************//
        #region Datos Grafica posicion       
        

       

        #endregion


      
        #region Trials     

        private int contTrialIdeal = -1;
        private int contTrial = -1;

        private void btnApplyTrials_Click(object sender, EventArgs e)
        {
            contTrial++;
            if (contTrial > 17)
                contTrial = 0;


            contTrialIdeal++;
            if (contTrialIdeal > 8)
                contTrialIdeal = 0;


            poschart = new PositionChart(chartData, 3);
            poschart.RemoveFeature("SeriesIdeal");
            poschart.RemoveFeature("SeriesPos");            
            poschart.ClearTargets();
           


                //Configura grafica de datos para la visualizacion de posicion
                
                poschart.ConfigureChartView(new Vector2(0, -53), distance); //Establece la vista de la grafica    

                
                poschart.Update(ideal[contTrialIdeal], false, 1000);

                //Actualiza trial
                poschart.Update(BinaryDataManager.GetAxisDataTrial(TypeData.TD_Xpr, 0, contTrial),
                  BinaryDataManager.GetAxisDataTrial(TypeData.TD_Ypr, 0, contTrial),
                  false, 1000);


            /* poschart.AddCircleTargets(BinaryDataManager.sharedInstance.GetAxisData(TypeData.TD_XprF, 0),
                 BinaryDataManager.sharedInstance.GetAxisData(TypeData.TD_YprF, 0));*/


            UpdateDataTrial(contTrial);

            trials[contTrial].Score();

            lblFocus.Focus();          
        }


        private void UpdateDataTrial(int index)
        {
            Trial trial = trials[index];

            lblTrial.Text = index.ToString("0");
            lblCompleted.Text = trial.isCompleted.ToString();
            lblDistance.Text = trial.distanceTotal.ToString("0.0000");
            lblTime.Text = trial.timeTotal.ToString("0.0000");
            lblReactionT.Text = trial.reactionTime.ToString("0.0000");
            lblSpeed.Text = trial.speedMax.ToString("0.0000");
            lblError.Text = trial.errorInicial.ToString("0.0000");
            lblRazon.Text = trial.razonInicial.ToString("0.0000");
            lblDTW.Text = trial.distanceDTW.ToString("0.0000")+"/"+trial.similityDTW.ToString("0.0000");
        }
        #endregion
        //****************************************************************************//
        //****************************************************************************//
    }
}
