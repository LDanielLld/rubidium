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
        private SpeedChart spdchart; //Controlador de la grafica de velocidad
        private DTWChart dtwchart; //Controlador de la grafica de dtw

        private Trial[] trials;

        private double last; //Ultimo valor de la grafica
        private float distance; //Distancia maxima de movimiento objetivo   

        #region Condition
        private TrialCondition[] condition = new TrialCondition[] {
            new TrialCondition(2.5, 0.30, 75, 0.25),
            new TrialCondition(6, 0.30, 80, 0.25),
            new TrialCondition(6, 0.30, 80, 0.25),
            new TrialCondition(1.5, 0.30, 60, 0.25)}; //Condiciones
        private int[] pattern = new int[]{0, 0, 1 ,2, 3 ,3, 3, 0, 0};//Indice de condiciones
        #endregion

      //  List<List<float[]>> ideal = new List<List<float[]>>();
        public PanelInfoPosition()
        {
            InitializeComponent();

            // Set Antialiasing mode
            chartData.AntiAliasing = AntiAliasingStyles.All;            
            chartData.TextAntiAliasingQuality = TextAntiAliasingQuality.High;


          //  ideal = DTW.GenerateIdealPath(350, 1000);

            
           //Crear condiciones para cada trial
           

        }

        private float[] ponderacion = new float[] {
            0.075f, 0.075f, 0.025f, 0.025f, 0.05f, 0.05f, 0.05f, 0.075f, 0.075f,  0.075f, 0.075f, 0.025f, 0.025f, 0.05f, 0.05f, 0.05f, 0.075f, 0.075f};

        private float[] ponderacion2 = new float[] {
            0.1f, 0.1f, 0.0125f, 0.0125f, 0.025f, 0.025f, 0.025f, 0.1f, 0.1f, 0.1f, 0.1f, 0.0125f, 0.0125f, 0.025f, 0.025f, 0.025f, 0.1f, 0.1f};

        public void Setting(double c, float d, float s)
        {
            last = c;
            distance = d;

            double valor = 0;
            double valor2 = 0;

            //Calcular distancias totales
            trials = new Trial[BinaryDataManager.listTrialDataRobot[0].Count];
            List<DataRobot[]> lista = BinaryDataManager.listTrialDataRobot[0];
            for (int i = 0; i < lista.Count; i++)
            {
                int id_ideal = i % 9;
                Trial cTrial = new Trial(lista[i], condition[pattern[id_ideal]], 10f, 350f, id_ideal);
                trials[i] = cTrial;

                valor += cTrial.Score() * ponderacion2[i];
                valor2 += cTrial.Score(); 
            }

            //lblEscala.Text = (valor2 * 100/18).ToString("0.00");
            lblEscala.Text = (valor * 100).ToString("0.00"); //

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


            //Configura grafica de datos para la visualizacion de posicion                
            poschart = new PositionChart(chartData);
            poschart.RemoveFeature("SeriesIdeal");
            poschart.RemoveFeature("SeriesPos");            
            poschart.ConfigureChartView(new Vector2(0, -53), distance); //Establece la vista de la grafica   

            //Visualiza datos de posicion
            poschart.Update(trials[contTrialIdeal].GetIdealPath(), trials[contTrialIdeal].GetIdealPath().Count); //Ideal
            poschart.Update(BinaryDataManager.GetAxisDataTrial(TypeData.TD_Xpr, 0, contTrial),
              BinaryDataManager.GetAxisDataTrial(TypeData.TD_Ypr, 0, contTrial),
              1000); //Real

            //Configura grafica de datos para la visualizacion de la velocidad                
            /*spdchart = new SpeedChart(chartSpeed);
            spdchart.RemoveFeature("SeriesPos");
            //spdchart.ConfigureChartView(new Vector2(0, -53), distance); //Establece la vista de la grafica   


            //Visualiza datos de velocidad            
            spdchart.Update(
                BinaryDataManager.GetAxisDataTrial(TypeData.TD_Vxr, 0, contTrial),
                BinaryDataManager.GetAxisDataTrial(TypeData.TD_Vyr, 0, contTrial),
                BinaryDataManager.GetAxisDataTrial(TypeData.TD_TimeStamp, 0, contTrial), 1000); //Real*/


            //Configura grafica de datos para la visualizacion de la velocidad                
            dtwchart = new DTWChart(chartSpeed);
            dtwchart.RemoveFeature("SeriesReal");
            dtwchart.RemoveFeature("SeriesIdeal");
            //spdchart.ConfigureChartView(new Vector2(0, -53), distance); //Establece la vista de la grafica   


            //Visualiza datos de velocidad            
            dtwchart.Update(
                trials[contTrialIdeal].GetDTWReal(),
                trials[contTrialIdeal].GetDTWIdeal());


            //Actualiza datos del trial
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
            lblpercSuccess.Text = trial.percDistance.ToString("0.00");
        }
        #endregion
        //****************************************************************************//
        //****************************************************************************//
    }
}
