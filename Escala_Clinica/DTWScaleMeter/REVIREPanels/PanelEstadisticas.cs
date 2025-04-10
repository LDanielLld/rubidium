using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Windows.Forms.DataVisualization.Charting;
using REVIREPanels.Graficas;
using REVIREPanels.Componentes;
using System.Numerics;
using REVIREPanels.Modelos;
using System.IO;
using System.Runtime.Remoting;

using REVIREPanels.Estadisticas.Graficas;


namespace REVIREPanels
{
    public enum StadisticType
    {
        NULL,
        DIARY,
        EVOLUTIVE
    }

    public partial class PanelEstadisticas : UserControl
    {


        StadisticType type = StadisticType.NULL;        
       

        IEnumerable<double> axisXDataS1 = null;
        IEnumerable<double> axisYDataS1 = null;

        IEnumerable<double> axisXDataS2 = null;        
        IEnumerable<double> axisYDataS2 = null;

        //Lista de actividades total y actual
        List<Actividad> lista_actividades = new List<Actividad>();
        List<Actividad> current_lista_actividades1 = new List<Actividad>();
        List<Actividad> current_lista_actividades2 = new List<Actividad>();

        //Lista de las propiedades
        List<PropActividades> lista_propactividades = new List<PropActividades>();

        Actividad currentAct1 = null;
        Actividad currentAct2 = null;

        //Fechas actuales
        DateTime dateInit;
        DateTime dateEnd;

        //Metodos delegados para recibir info desde la clase de control de la grafica        
        public delegate void functioncall(bool state);
        private event functioncall MouseMovement;
        public delegate void functioncall2(int index, int action);
        private event functioncall2 MouseClick;

        //Datos de usuario
        private int idUser;
        private Usuario cUser;

        public PanelEstadisticas(int usuario)
        {
            InitializeComponent();

            idUser = usuario;            

            



            //Inicializar pick date  en funcion de las sesiones que existan       
            dateInit = GetMinDate();
            dateEnd = DateTime.Now;

            dateTimeEvolutiveInit.Value = dateInit;
            dateTimeEvolutiveEnd.Value = dateEnd;

            //Tab de control            
            radioButtonDi.PerformClick(); //Seleccionar esta diaria por defecto

            //Tab de control
            tabControlStats.Appearance = TabAppearance.FlatButtons;
            tabControlStats.ItemSize = new Size(0, 1);
            tabControlStats.SizeMode = TabSizeMode.Fixed;
            radioButtonDi.PerformClick(); //Seleccionar esta diaria por defecto

            //Tab de visualizacion de graficas
            tabChartData.Appearance = TabAppearance.FlatButtons;
            tabChartData.ItemSize = new Size(0, 1);
            tabChartData.SizeMode = TabSizeMode.Fixed;
            


            // labelfocu
            focusLabel.Focus();

            DoubleBuffered = true;
            
            //Seleccionar la ultima sesion registrada
            comboSesionDiary.SelectedIndex = comboSesionDiary.Items.Count - 1;

            //Cargar fichero                            
            int nLoaded = BinaryDataManager.LoadDataRobotSession();

            if (nLoaded == 0)
            {
                MessageBox.Show("Error de carga: " + BinaryDataManager.errorMsg);
            }
            else
            {
                tableLayoutViewData.Enabled = true; //Habilita panel de visualizacion de datos
                

                panelInfoPosition.IsSelectedPie = true;
                panelInfoPosition.Setting(
                    2500, //Tiempo maximo
                    350, //Distancia
                    BinaryDataManager.listDataRobot[0].Count()); //Numero total de muestras

                //Selecciona el tiempo del juego
                //  axisXDataS1 = BinaryDataManager.sharedInstance.GetTimeAxisData(0);
                //  ftypeX = 0;
                //  dataRobotS1 = BinaryDataManager.sharedInstance.listDataRobot[0];
            }
        }

        PositionChart poschart;
        BarDataChart datachart;

        protected override CreateParams CreateParams
        {
            get
            {
                CreateParams cp = base.CreateParams;
                cp.ExStyle |= 0x02000000;
                return cp;
            }
        }

        //************************Botones principales*********************************//
        private void btBack_Click(object sender, EventArgs e)
        {
            Application.Exit();
        }        

        private void btnReport_Click(object sender, EventArgs e)
        {

            
        }
        //****************************************************************************//

        //************************Filtrado de fechas**********************************//
        private void dateTimeEvolutiveInit_ValueChanged(object sender, EventArgs e)
        {
            if (dateTimeEvolutiveInit.Value.CompareTo(dateEnd) > 0)
            {
                dateTimeEvolutiveInit.Value = dateInit;
            }
            else
            {
                dateInit = dateTimeEvolutiveInit.Value;
                FilterRangeOfDates();
            }
        }

        private void dateTimeEvolutiveEnd_ValueChanged(object sender, EventArgs e)
        {
            if (dateTimeEvolutiveEnd.Value.CompareTo(dateInit) < 0)
            {
                dateTimeEvolutiveEnd.Value = dateEnd;
            }
            else
            {
                dateEnd = dateTimeEvolutiveEnd.Value;
                FilterRangeOfDates();
            }
        }

        DateTime GetMinDate()
        {
            DateTime minDate = DateTime.Now;            

            foreach (Actividad act in lista_actividades)
            {
                DateTime currentTime = new DateTime(act.Fecha.Year, act.Fecha.Month, act.Fecha.Day);
                
                if (currentTime.CompareTo(minDate) < 0)
                {
                    minDate = currentTime;
                }
            }
            return minDate;
        }

        DateTime FilterRangeOfDates()
        {
            DateTime minDate = DateTime.Now;

            List<DateTime> list_dates = new List<DateTime>();
            comboSesionDiary.Items.Clear();
                 

            foreach (Actividad act in lista_actividades)
            {
                DateTime currentTime = new DateTime(act.Fecha.Year, act.Fecha.Month, act.Fecha.Day);

                 //Comprueba si esta sesion esta dentro de los limites de fechas
                if (currentTime.CompareTo(dateInit) >= 0 && currentTime.CompareTo(dateEnd) <= 0)
                {
                    if(!list_dates.Contains(currentTime))
                    {                        
                        list_dates.Add(currentTime);
                        comboSesionDiary.Items.Add(currentTime.ToShortDateString());       
                        
                    }                    
                }                
            }
            
            return minDate;
        }
        //****************************************************************************//
        
        //************************Selección de tipo de estadistica********************//
        private void radioButtonDi_CheckedChanged(object sender, EventArgs e)
        {
            panelDiary.Enabled = true;
           
           

            type = StadisticType.DIARY;

            ResetStadistictSession();

            tabControlStats.SelectedIndex = 0;
        }

        private void radioButtonEv_CheckedChanged(object sender, EventArgs e)
        {
            panelDiary.Enabled = false;
           
            

            type = StadisticType.EVOLUTIVE;

            ResetStadistictSession();

            tabControlStats.SelectedIndex = 1;
        }

        private void ResetStadistictSession()
        {
            comboSesionDiary.SelectedIndex = -1;
                     

            //Borrar elementos de la lista de actividades actual
            current_lista_actividades1.Clear();
            current_lista_actividades2.Clear();
           

            currentAct1 = null;
            currentAct2 = null;

         

            // mainChart.Series.Clear();
        }
        //****************************************************************************//

        //************************Selección de la sesion******************************//
        private void comboSesionDiary_SelectedIndexChanged(object sender, EventArgs e)
        {
            try
            {

                int index = comboSesionDiary.SelectedIndex;
                if (index != -1)
                {
                    current_lista_actividades1.Clear();
                    string currentDateTime = comboSesionDiary.SelectedItem.ToString();

                    foreach (Actividad act in lista_actividades)
                    {
                        //Comprueba si esta sesion esta dentro de los limites de fechas
                        if (currentDateTime.CompareTo(act.Fecha.ToShortDateString()) == 0)
                        {
                            current_lista_actividades1.Add(act);
                        }
                    }

                    //Muestra las actividades en forma de tarta
                    CreateSessionPieData(index, current_lista_actividades1);

                   
                }
                else
                {
                    sessionChart.Series.Clear();
                    sessionChart.ResetAutoValues();
                }
            }
            catch (Exception err)
            {
                tableLayoutViewData.Enabled = false; //Deshabilita panel de visualizacion de datos
                MessageBox.Show("Datos de versiones antiguas: "+err.Message, "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
        }

       

        void LoadNameActivities(ListBox list, List<Actividad> current_list)
        {
            //Primero borra los que existen
            list.Items.Clear();

            foreach (Actividad act in current_list)
            {
                string name = act.Nombre;
                list.Items.Add(name);
            }
        }
        //****************************************************************************//        

        //************************Selección de activad********************************//
        private void listActivitiesD_SelectedIndexChanged(object sender, EventArgs e)
        {
           /* int index = listActivitiesE1.SelectedIndex;
            if (index != -1)
            {
                currentAct1 = current_lista_actividades1[index];

                //Cargar fichero
                List<Actividad> l_act = new List<Actividad>() { currentAct1 };
                int nLoaded = BinaryDataManager.LoadDataRobotSession();

                if (nLoaded != l_act.Count)
                {
                    MessageBox.Show("Error de carga: " + BinaryDataManager.errorMsg);
                }
            }*/
        }

       
        //****************************************************************************//

       


        //********************Gestion de la gráfica sesion****************************//
        //****************************************************************************//
        #region Grafica Sesion
        private SessionPieChart ctrlInfoPos;        
        private void CreateSessionPieData(int type, List<Actividad> listact)
        {
            //Inicializar grafica con una tarta de prueba
            sessionChart.Series[0].Points.Clear();
            sessionChart.Legends.Clear();
            if(ctrlInfoPos!=null)
                ctrlInfoPos.Dispose();

            ctrlInfoPos = new SessionPieChart(sessionChart, 0, this.Cursor, false, true);

            //Evento  movimiento de raton
            MouseMovement -= CursorChanging;
            MouseMovement += new functioncall(CursorChanging);
            ctrlInfoPos.MouseMovementInChart = MouseMovement;
            //Evento pulsacion de raton
            MouseClick -= SelectPieSlice;
            MouseClick += new functioncall2(SelectPieSlice);
            ctrlInfoPos.MouseClickInChart = MouseClick;

            //Convertir dias en datapoints            
            ctrlInfoPos.UpdatePie(listact);
        }

        private void CursorChanging(bool state)
        {
            //Cambia el cursor si toca la grafica
            if (state)
                this.Cursor = Cursors.Hand;
            else
                this.Cursor = Cursors.Default;
        }

        private void SelectPieSlice(int index, int action)
        {
            try
            {
                //Generar y rellenar panel para mostrar infomacion                
                if (action == 1) //Selecciona trozo de tarta
                {
                    //Recolectar informacion sobre las actividades para visualizarla            
                    currentAct1 = current_lista_actividades1[index];
                    PropActividades prop = lista_propactividades.Find(x => x.Nombre == currentAct1.Nombre);                    

                   
                    //Borra las estadisticas mostradas actualmente
                    panelInfoPosition.Reset();

                    //Rellenar asistencia
                    FillAsistenceInfo(currentAct1);

                    //Rellenar tabla duracion
                    FillDurationInfo(currentAct1, prop);

                    //Rellenar tabla objetivos
                    FillTargetInfo(currentAct1, prop);

                    //Rellenar tabla adicional
                    FillAditionalInfo(currentAct1, prop);

                    //Carga fichero binario
                    List<Actividad> l_act = new List<Actividad>() { currentAct1 };
                    int nLoaded = BinaryDataManager.LoadDataRobotSession();

                    if (nLoaded != l_act.Count)
                    {
                        MessageBox.Show("Error de carga: " + BinaryDataManager.errorMsg);
                    }
                    else
                    {
                        tableLayoutViewData.Enabled = true; //Habilita panel de visualizacion de datos
                        

                        panelInfoPosition.IsSelectedPie = true;
                        panelInfoPosition.Setting(
                            double.Parse(currentAct1.DatosRecibidos.Split(' ')[0]), //Tiempo maximo
                            currentAct1.Distancia, //Distancia
                            BinaryDataManager.listDataRobot[0].Count()); //Numero total de muestras

                        //Selecciona el tiempo del juego
                        //  axisXDataS1 = BinaryDataManager.sharedInstance.GetTimeAxisData(0);
                        //  ftypeX = 0;
                        //  dataRobotS1 = BinaryDataManager.sharedInstance.listDataRobot[0];
                    }



                }
                else if(action == 0) //Deselecciona trozo
                {
                                  
                    panelInfoPosition.IsSelectedPie = false;

                    //Borrar datos de sesion
                    ClearChartSession();
                }
            }
            catch (Exception)
            {
                tableLayoutViewData.Enabled = false; //Deshabilita panel de visualizacion de datos
                MessageBox.Show("Datos de versiones antiguas.", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
        }

        //*****************************Mostrar informacion****************************//
        //****************************************************************************//
        #region Informacion de sesion
        private void FillAsistenceInfo(Actividad act)
        {

            
        }

        private void FillDurationInfo(Actividad cActividad, PropActividades prop)
        {
            string[] datareceived = cActividad.DatosRecibidos.Split(' ');

            //Informacion con la duracion 
            List<string> values = new List<string>();
            values.Add(cActividad.Fecha.ToShortTimeString());

            float timetotal = cActividad.Minutos * 60 + cActividad.Segundos;
            float timereach = float.Parse(datareceived[0]);
           /* if (timereach > timetotal)
                timereach = timetotal;*/

            values.Add(Util.CalculateStringTime(timereach, 1));

            if (prop.ActInterface[0]) //Tiempo total
            {
                int min = cActividad.Minutos;
                int seg = cActividad.Segundos;
                string tiempoTotal = min.ToString("00") + ":"
                    + seg.ToString("00");
                values.Add(tiempoTotal);
            }
            else
                values.Add("---------------------");

            if (prop.ActInterface[1]) //Repeticiones
                values.Add(cActividad.Repeticiones.ToString());
            else
                values.Add("---------------------");
           
        }

        private void FillTargetInfo(Actividad cActividad, PropActividades prop)
        {
            List<string> values = new List<string>();

            //Informacion de los objetivos            
            if (prop.ActInterface[2]) //Tiempo
                values.Add(cActividad.TiempoObjetivo.ToString());
            else
                values.Add("---------------------");

            if (prop.ActInterface[3]) //Distancia
                values.Add(cActividad.Distancia.ToString());
            else
                values.Add("---------------------");

            if (prop.ActInterface[4]) //Orden
            {
                values.Add(cActividad.OrdenObjetivo.ToString());
            }
            else
                values.Add("---------------------");

          


        }

        private void FillAditionalInfo(Actividad cActividad, PropActividades prop)
        {
           
        }

        private void ClearChartSession()
        {           

            //Resetea panel
            panelInfoPosition.Reset();
        }
        #endregion
        //****************************************************************************//
        //****************************************************************************//
        #endregion
        //****************************************************************************//
        //****************************************************************************//


       
        

        //************************Gestion de la gráfica*******************************//
        //****************************************************************************//
        

        /// <summary>
        /// Cambia el estilo de la grafica dependiendo de las caracteristicas a mostrar
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void cb_SelectorTypeChart_SelectedIndexChanged(object sender, EventArgs e)
        {
            int index = 0;
            switch (index)
            {               
                case 10: //Gráfica de la posicion antigua

//                    axisXDataS1 = BinaryDataManager.sharedInstance.GetAxisData(0, 0);
 //                   axisYDataS1 = BinaryDataManager.sharedInstance.GetAxisData(1, 0);

                    //Temporal, objetivos a colocar con un circulo
                    /* Vector2[] vectort = new Vector2[9];

                     vectort[0] = new Vector2(0, 0); //Punto inicial
                     vectort[1] = new Vector2(0, 20);
                     vectort[2] = new Vector2(15, 15);
                     vectort[3] = new Vector2(20, 0);
                     vectort[4] = new Vector2(15, -15);
                     vectort[5] = new Vector2(0, -20);
                     vectort[6] = new Vector2(-15, -15);
                     vectort[7] = new Vector2(-20, 0);
                     vectort[8] = new Vector2(-15, 15);*/
                    /* vectort[9] = new Vector2(50, 50);
                     vectort[10] = new Vector2(50, -50);
                     vectort[11] = new Vector2(-50, 50);
                     vectort[12] = new Vector2(-50, -50);*/

                    //Configurar la vista del chart
 //                   Vector2[] vectort = BinaryDataManager.sharedInstance.listEventRobot[0].Target.ToArray();
 //                   Vector2[] vectori = BinaryDataManager.sharedInstance.listEventRobot[0].PosInit.ToArray();

                    //Crear grafica de posicion del robot
//                    poschart = new PositionChart(chartData, 3);
 //                   poschart.ConfigureChartView(vectori, vectort); //Configurar la vista de la grafica

                    //Pintar elementos
//                    poschart.DrawTargets(BinaryDataManager.sharedInstance.listEventRobot[0]); //Pintar zona objetivo                    

                    //poschart.SetIdealTrajectory(vectort);

                    //  poschart.SetRangeIdealTrajectory(vectort);
                    //Pinta la grafica
 //                   poschart.Update(axisXDataS1, axisYDataS1, ch_TargetZone.Checked);


                    break;
               
                
                case 5:
                    //Tiempo de reaccion                    
                   // poschart = new PositionChart(chartData, 3);

                    

  /*                  for (int i=0; i<BinaryDataManager.sharedInstance.listEventRobot[0].Trials.Count; i++)
                    {
                        comboTrials.Items.Add("Trial_"+i);
                    }

                    //Configurar la vista del chart
                    Vector2[] vectort2 = BinaryDataManager.sharedInstance.listEventRobot[0].Target.ToArray();
                    Vector2[] vectori2 = BinaryDataManager.sharedInstance.listEventRobot[0].PosInit.ToArray();

                    //Crear grafica de posicion del robot
                    poschart = new PositionChart(chartData, 3);
                    poschart.ConfigureChartView(vectori2, vectort2); //Configurar la vista de la grafica

                    //Pintar elementos
                    poschart.DrawTargets(BinaryDataManager.sharedInstance.listEventRobot[0]); //Pintar zona objetivo   
*/
                    break;

            }

            

            //En caso de evolutiva se selecciona
          //  if (type == StadisticType.EVOLUTIVE)
            //    axisXDataS2 = BinaryDataManager.sharedInstance.GetAxisData((TypeData)ftypeX, 1);

        }

        
        //****************************************************************************//
        //****************************************************************************//
        


        //*******************************Modo Debug***********************************//
        //****************************************************************************//
        private int pulseToDebub = 0;
        private void panelDebug_MouseDown(object sender, MouseEventArgs e)
        {
            pulseToDebub++;
          
        }

        private void btnPathData_Click(object sender, EventArgs e)
        {
            string path = BinaryDataManager.DataPath;
            if (!string.IsNullOrEmpty(path))
                System.Diagnostics.Process.Start(path);
            else
                MessageBox.Show("No se ha seleccionado una actividad!","Warning", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
            focusLabel.Focus();
        }
        
        //****************************************************************************//
        //****************************************************************************//
    }
}
