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

        private double last; //Ultimo valor de la grafica
        private float distance; //Distancia maxima de movimiento objetivo
        private float samples; //Numero de samples totales
        private bool initialized = false;

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
            initialized = true;

            //Configura el selector de muestras
            samples = s;
           

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

        public void Reset()
        {
            //Resetear toda la interfaz           
            cb_PosTrajectory.Checked = false;            
        }
        //**********************Visualizar datos de posicion**************************//
        //****************************************************************************//
        #region Datos Grafica posicion       
        

        private void cb_PosTrajectory_CheckedChanged(object sender, EventArgs e)
        {
            if (cb_PosTrajectory.Checked)
                {
                  
                
                
                    //Configura grafica de datos para la visualizacion de posicion
                    poschart = new PositionChart(chartData, 3);
                    poschart.ConfigureChartView(new Vector2(0, -53), distance); //Establece la vista de la grafica    

#if DEBUG
                    //Calcula objetivos
                    poschart.DrawTargets(BinaryDataManager.GetAxisData(TypeData.TD_Xpr0, 0),
                    BinaryDataManager.GetAxisData(TypeData.TD_Ypr0, 0),
                    BinaryDataManager.GetAxisData(TypeData.TD_XprF, 0),
                    BinaryDataManager.GetAxisData(TypeData.TD_YprF, 0));

                //Actualiza grafica
                /*poschart.Update(BinaryDataManager.GetAxisData(TypeData.TD_Xpr, 0), 
                     BinaryDataManager.GetAxisData(TypeData.TD_Ypr, 0),
                     ch_PosTargetZone.Checked, (float)sampleSelector.Value);*/

                int id = 0;
                poschart.Update(ideal[id], false, 1000); 

                //Actualiza trial
                
                poschart.Update(BinaryDataManager.GetAxisDataTrial(TypeData.TD_Xpr, 0, id), 
                  BinaryDataManager.GetAxisDataTrial(TypeData.TD_Ypr, 0, id),
                  false, 1000);

                List<float[]> d = ideal[id];

                Vector3[] result =
                    BinaryDataManager.GetAxisDataTrial(TypeData.TD_Xpr, 0, id).Zip(BinaryDataManager.GetAxisDataTrial(TypeData.TD_Ypr, 0, id), (x, y) => new Vector3((float)x, (float)y, 0)).ToArray();



                var data = DTW.Calculate(d, result, 1);
                float dist = data.Distance;


                //Calcular distancias totales
                Trial[] trial = new Trial[BinaryDataManager.listTrialDataRobot[0].Count];
                List<DataRobot[]> lista = BinaryDataManager.listTrialDataRobot[0];
                for (int i=0; i<lista.Count; i++)
                {
                    Trial cTrial = new Trial(lista[i], d, 10f, 350f);
                    trial[i] = cTrial;
                }

               
#else

                    //Calcula objetivos
                    poschart.DrawTargets(BinaryDataManager.GetAxisData(TypeData.TD_Xpr0, 0),
                    BinaryDataManager.GetAxisData(TypeData.TD_Ypr0, 0),
                    BinaryDataManager.GetAxisData(TypeData.TD_XprF, 0),
                    BinaryDataManager.GetAxisData(TypeData.TD_YprF, 0));

                    //Actualiza grafica
                    poschart.Update(BinaryDataManager.GetAxisData(TypeData.TD_Xpr, 0), 
                        BinaryDataManager.GetAxisData(TypeData.TD_Ypr, 0),
                        ch_PosTargetZone.Checked, (float)sampleSelector.Value);
#endif


                /* poschart.AddCircleTargets(BinaryDataManager.sharedInstance.GetAxisData(TypeData.TD_XprF, 0),
                     BinaryDataManager.sharedInstance.GetAxisData(TypeData.TD_YprF, 0));*/
            }
            else
            {
                poschart.RemoveFeature("SeriesPos");
                poschart.ClearTargets();


               
            }
        }

        #endregion


      
        #region Trials        

        private int InitialFlagTrial = -1;
        private int FinalFlagTrial = -1;
        private int ErrorFlagTrial = -1;

        private Vector2[] trials;

        private List<int> FlagIndex = new List<int>(); //Indices para señalar los flags

        private void cb_TrialsSegmention_CheckedChanged(object sender, EventArgs e)
        {
            if (initialized)
            {
                if (cb_TrialsSegmention.Checked)
                {
                    // Primero quita seleccion anterior para que no se solape
                    cb_PosTrajectory.Checked = false;
                   

                    //Activa selectores de  flag
                    cb_final_flag.Enabled = true;
                    cb_initial_flag.Enabled = true;
                    cb_error_flag.Enabled = true;

                    //Añade los tipos de gamestate para decidir cual es el que genera los trials               
                    poschart.AddTaskState(BinaryDataManager.GetAxisData(TypeData.TD_TimeStamp, 0),
                        BinaryDataManager.GetAxisData(TypeData.TD_TaskState, 0), "GameState", distance);

                    //Muestra los posibles flags en los respectivos combobox 
                    List<int> indexes = new List<int>();
                    List<string> sFlags = PossibleFlags(BinaryDataManager.GetAxisData(TypeData.TD_TaskState, 0), ref indexes);


                    for (int i = 0; i < sFlags.Count(); i++)
                    {
                        string v = sFlags[i];

                        if (!cb_initial_flag.Items.Contains(v))
                        {
                            cb_initial_flag.Items.Add(v);
                            cb_final_flag.Items.Add(v);
                            cb_error_flag.Items.Add(v);
                            FlagIndex.Add(indexes[i]);
                        }
                    }
                }
                else
                {
                    poschart.Reset();
                    

                    //Resetea interfaz
                    trialSelector.Value = 0;
                    trialSelector.Enabled = false;

                    //btnApplyTrials.Enabled = false;

                    ResetFlags();

                    trials = new Vector2[0];
                }
            }
            else
            {

                cb_TrialsSegmention.Checked = false;
                MessageBox.Show("Selecciona una actividad!!", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
        }

        /// <summary>
        /// Gestiona los posibles flags que se pueden seleccionar (incrementos y decrementos de la señal step)
        /// </summary>
        /// <param name="flags"></param>
        /// <returns></returns>
        private List<string> PossibleFlags(IEnumerable<double> flags, ref List<int> indexes)
        {
            List<string> possibles = new List<string>();            

            //Extrae info donde se produce el cambio de estado
            List<float> steps = Util.DiffAlgorithm(flags);            

            //Revisa donde se produce el cambio de evento de inicio de trial            
            int index = 0;
            while (index != -1)
            {
                index = steps.ToList().FindIndex(x => x != 0);
                if (index != -1)
                {
                    indexes.Add(index + 1);
                    possibles.Add(flags.ElementAt(index) + " - " + flags.ElementAt(index + 1));
                    steps[index] = 0; //Lo anula para el siguiente elemento
                }
            }           

            return possibles;
        }

        private void cb_initial_flag_SelectedIndexChanged(object sender, EventArgs e)
        {
            //Comprueba que no se pode el flag inicial delante del flag final
            int cIndex = cb_initial_flag.SelectedIndex;
            if (cIndex != -1)
            {
                if (cIndex < FinalFlagTrial && cIndex!=ErrorFlagTrial)
                {
                    //Seleccionar el flag que indicara el inicio del trial
                    InitialFlagTrial = cb_initial_flag.SelectedIndex;

                    //Señala el elemento seleccionado en la grafica
                    poschart.RemarkPoint("init", FlagIndex[cb_initial_flag.SelectedIndex], "GameState");
                }
                else if (FinalFlagTrial == -1 && cIndex != ErrorFlagTrial)
                {
                    //Seleccionar el flag que indicara el inicio del trial
                    InitialFlagTrial = cb_initial_flag.SelectedIndex;

                    //Señala el elemento seleccionado en la grafica
                    poschart.RemarkPoint("init", FlagIndex[cb_initial_flag.SelectedIndex], "GameState");
                }
                else
                { 
                    //Vuelve al estado anterior
                    cb_initial_flag.SelectedIndex = InitialFlagTrial;
                }
            }

            //Comprueba si activa el boton apply
            CheckButtonApply();

        }

        private void cb_final_flag_SelectedIndexChanged(object sender, EventArgs e)
        {
            int cIndex = cb_final_flag.SelectedIndex;
            if (cIndex != -1)
            {
                if (cIndex > InitialFlagTrial && cIndex != ErrorFlagTrial)
                {
                    //Seleccionar el flag que indicara el final del trial
                    FinalFlagTrial = cb_final_flag.SelectedIndex;

                    //Señala el elemento seleccionado en la grafica
                    poschart.RemarkPoint("end", FlagIndex[cb_final_flag.SelectedIndex], "GameState");
                }
                else
                {
                    //Vuelve al estado anterior
                    cb_final_flag.SelectedIndex = FinalFlagTrial;
                }
            }

            //Comprueba si activa el boton apply
            CheckButtonApply();
        }

        private void cb_error_flag_SelectedIndexChanged(object sender, EventArgs e)
        {
            //Comprueba que no se pode el flag inicial delante del flag final
            int cIndex = cb_error_flag.SelectedIndex;
            if (cIndex != -1)
            {
                //Si es diferente de los flags anteriores
                if (cIndex != FinalFlagTrial && cIndex != InitialFlagTrial)
                {
                    //Seleccionar el flag que indicara el error en el trial (por lo tanto el final)
                    ErrorFlagTrial = cIndex;

                    //Señala el elemento seleccionado en la grafica
                    poschart.RemarkPoint("error", FlagIndex[cb_error_flag.SelectedIndex], "GameState");
                }                
                else
                {
                    //Vuelve al estado anterior
                    cb_error_flag.SelectedIndex = ErrorFlagTrial;
                }
            }            
        }

        private void CheckButtonApply()
        {
         //   if (FinalFlagTrial != -1 && InitialFlagTrial != -1)
              //  btnApplyTrials.Enabled = true;
        }

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



                List<float[]> d = ideal[contTrialIdeal];
                //Calcular distancias totales
                Trial[] trial = new Trial[BinaryDataManager.listTrialDataRobot[0].Count];
                List<DataRobot[]> lista = BinaryDataManager.listTrialDataRobot[0];
                for (int i = 0; i < lista.Count; i++)
                {
                    Trial cTrial = new Trial(lista[i], d, 10f, 350f);
                    trial[i] = cTrial;
                }





                /* poschart.AddCircleTargets(BinaryDataManager.sharedInstance.GetAxisData(TypeData.TD_XprF, 0),
                     BinaryDataManager.sharedInstance.GetAxisData(TypeData.TD_YprF, 0));*/
            
            

            



            lblFocus.Focus();
          
        }

        private void trialSelector_ValueChanged(object sender, EventArgs e)
        {
            try
            {
                //Selecciona el trial y lo muestra en la grafica
                Vector2 trial = trials[(int)trialSelector.Value];

                //Seleccionar trial 0 en caso de existir
                poschart.Reset(); //Inicializa grafica      
                poschart = new PositionChart(chartData, 3);
                poschart.ConfigureChartView(new Vector2(0, -53), distance); //Establece la vista de la grafica    
                poschart.SliceTrial(trial,
                    BinaryDataManager.GetAxisData(TypeData.TD_Xpr, 0),
                    BinaryDataManager.GetAxisData(TypeData.TD_Ypr, 0));
            }
            catch(Exception)
            { }
        }

        private void comboTrials_SelectedIndexChanged(object sender, EventArgs e)
        {
         //               TrialPacket trial = 
            //                BinaryDataManagerOld.sharedInstance.listEventRobot[0].Trials[comboTrials.SelectedIndex];            
            //            poschart.SliceTrial(trial, BinaryDataManagerOld.sharedInstance.listDataRobot[0]);
        }


        private void ResetFlags()
        {
            cb_final_flag.Enabled = false;
            cb_initial_flag.Enabled = false;
            cb_error_flag.Enabled = false;

            cb_final_flag.Items.Clear();
            cb_initial_flag.Items.Clear();
            cb_error_flag.Items.Clear();

            InitialFlagTrial = -1;
            FinalFlagTrial = -1;
            ErrorFlagTrial = -1;
        }

        

        #endregion
        //****************************************************************************//
        //****************************************************************************//
    }
}
