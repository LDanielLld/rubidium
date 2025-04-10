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
            sampleSelector.Maximum = (int)s;
            sampleSelector.Minimum = 100;
            sampleSelector.Value = 1500;

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
            cb_PosX.Checked = false;
            cb_PosY.Checked = false;
            cb_PosTrajectory.Checked = false;

            cb_PosIdeal.Checked = false;
            cb_posTargets.Checked = false;
            ch_PosTargetZone.Checked = false;
        }
        //**********************Visualizar datos de posicion**************************//
        //****************************************************************************//
        #region Datos Grafica posicion
        private void cb_PosX_CheckedChanged(object sender, EventArgs e)
        {
            
            if (cb_PosX.Checked)
            {
                cb_PosTrajectory.Checked = false; //Quita seleccion anterior

                //Desctivar complementarios
                cb_PosIdeal.Enabled = false;
                cb_posTargets.Enabled = false;
                ch_PosTargetZone.Enabled = false;                   

                //Configura grafica de datos para la visualizacion de posicion
                poschart = new PositionChart(chartData, 3);
                poschart.ConfigureChartView(new Vector2(0, 0), last, distance); //Establece la vista de la grafica   
                poschart.AddFeature(BinaryDataManager.GetAxisData(TypeData.TD_TimeStamp, 0),
                    BinaryDataManager.GetAxisData(TypeData.TD_Xpr, 0),
                    "PosX", (float)sampleSelector.Value);

            }
            else
            {
                poschart.RemoveFeature("PosX");
            }
           
        }

        private void cb_PosY_CheckedChanged(object sender, EventArgs e)
        {
            
            if (cb_PosY.Checked)
            {
                cb_PosTrajectory.Checked = false; //Quita seleccion anterior

                //Desctivar complementarios
                cb_PosIdeal.Enabled = false;
                cb_posTargets.Enabled = false;
                ch_PosTargetZone.Enabled = false;                    


                //Configura grafica de datos para la visualizacion de posicion
                poschart = new PositionChart(chartData, 3);
                poschart.ConfigureChartView(new Vector2(0, -53), last, distance); //Establece la vista de la grafica   
                poschart.AddFeature(BinaryDataManager.GetAxisData(TypeData.TD_TimeStamp, 0),
                    BinaryDataManager.GetAxisData(TypeData.TD_Ypr, 0), "PosY", (float)sampleSelector.Value);
            }
            else
            {
                poschart.RemoveFeature("PosY");
            }
            
        }

        private void cb_PosTrajectory_CheckedChanged(object sender, EventArgs e)
        {
            if (cb_PosTrajectory.Checked)
                {
                    //Primero quita seleccion anterior para que no se solape
                    cb_PosX.Checked = false;
                    cb_PosY.Checked = false;

                    //Activar complementarios
                    cb_PosIdeal.Enabled = true;
                    cb_posTargets.Enabled = true;
                    ch_PosTargetZone.Enabled = true;

                
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

                //  poschart.Update(ideal[8], ch_PosTargetZone.Checked, 1000); 

                //Actualiza trial
                int id = 3;
                poschart.Update(BinaryDataManager.GetAxisDataTrial(TypeData.TD_Xpr, 0, id), 
                  BinaryDataManager.GetAxisDataTrial(TypeData.TD_Ypr, 0, id),
                  ch_PosTargetZone.Checked, 1000);



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


                //Desctivar complementarios
                cb_PosIdeal.Enabled = false;
                cb_posTargets.Enabled = false;
                ch_PosTargetZone.Enabled = false;
            }
        }



        

        #endregion


        #region Caracteristicas
        private void cb_PosIdeal_CheckedChanged(object sender, EventArgs e)
        {
            if (cb_PosTrajectory.Checked)
            {
                if (cb_PosIdeal.Checked)
                {
                    //Pintar trajectoria ideal                                                        
                    IEnumerable<double> originX = BinaryDataManager.GetAxisData(TypeData.TD_Xpr0, 0);
                    IEnumerable<double> originY = BinaryDataManager.GetAxisData(TypeData.TD_Ypr0, 0);
                    Vector2 origin = new Vector2((float)originX.ElementAt(0), (float)originY.ElementAt(0));

                    poschart.SetIdealTrajectory(BinaryDataManager.GetAxisData(TypeData.TD_XprF, 0),
                        BinaryDataManager.GetAxisData(TypeData.TD_YprF, 0)
                        , origin); //Trajectorias ideales
                }
                else
                {
                    poschart.RemoveFeature("SeriesIdeal");
                }
            }
        }

        private void cb_posTargets_CheckedChanged(object sender, EventArgs e)
        {
            //Actualiza objetivos
            poschart.IsDrawTargetCircle(cb_posTargets.Checked);
        }

        private void ch_TargetZone_CheckedChanged(object sender, EventArgs e)
        {
            //Actualiza la zona de objetivos
            poschart.IsDrawTargetZone(ch_PosTargetZone.Checked);
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
                    cb_PosX.Checked = true;
                    cb_PosY.Checked = true;

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
                    cb_PosX.Checked = false;
                    cb_PosY.Checked = false;

                    //Resetea interfaz
                    trialSelector.Value = 0;
                    trialSelector.Enabled = false;

                    btnApplyTrials.Enabled = false;

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
            if (FinalFlagTrial != -1 && InitialFlagTrial != -1)
                btnApplyTrials.Enabled = true;
        }

        private void btnApplyTrials_Click(object sender, EventArgs e)
        {
            //Aplica los selectores de flag para separar los trials
            if (InitialFlagTrial != -1 && FinalFlagTrial != -1) //Se ha seleccionado un valor
            {
                trials = poschart.GetTrialIndex((string)cb_initial_flag.SelectedItem, (string)cb_final_flag.SelectedItem,
                    (string)cb_error_flag.SelectedItem,
                    BinaryDataManager.GetAxisData(TypeData.TD_TaskState, 0),
                    BinaryDataManager.GetAxisData(TypeData.TD_TimeStamp, 0));

                //Establece limite de trials para el selector
                trialSelector.Maximum = trials.Length - 1;
                trialSelector.Enabled = true;                

                //Seleccionar trial 0 en caso de existir
                poschart.Reset(); //Inicializa grafica
                poschart = new PositionChart(chartData, 3);
                poschart.ConfigureChartView(new Vector2(0, -53), distance); //Establece la vista de la grafica    

                poschart.SliceTrial(trials[(int)trialSelector.Value],
                    BinaryDataManager.GetAxisData(TypeData.TD_Xpr, 0),
                    BinaryDataManager.GetAxisData(TypeData.TD_Ypr, 0));


                //Deshabilita compoentnes
                ResetFlags();
                btnApplyTrials.Enabled = false;

                lblFocus.Focus();
            }
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
