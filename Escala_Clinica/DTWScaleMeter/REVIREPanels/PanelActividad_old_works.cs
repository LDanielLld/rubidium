using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Data.SqlClient;
using System.IO;

//Para la recepcion de datos de la actividad por parte de la interfaz
using System.Threading;
using System.Net;
using System.Net.Sockets;

namespace REVIREPanels
{
    public partial class PanelActividad : UserControl
    {        

        Actividad currentActivity;
        private PanelActividades actividades;
        public PanelActividades Actividades
        {
            get
            {
                if (actividades == null)
                {
                    actividades = new PanelActividades(currentActivity.UsuarioID);
                    actividades.Dock = DockStyle.Fill; //Para ajustar
                }
                return actividades;
            }
        }

        //Variables UDP
        public string localIP = "127.0.0.1";
        private UdpClient udpRead;
        public int localPort = 33333;
        private IPEndPoint localEndPoint;
        private static Thread receiveThread;
        private bool runningSession = false;

        List<Sesion> lista_sesiones = new List<Sesion>();

        //Paneles auxiliares
        SubPanelLevel subPanelLevel;
        public delegate void functioncall(int message);
        private event functioncall formFunctionPointer;

        public PanelActividad(Actividad actividad)
        {
            currentActivity = actividad;

            InitializeComponent();
            CheckForIllegalCrossThreadCalls = false;

            //Mostrar imagen de la actividad actual
            fondoActividad.BackgroundImage = Image.FromFile(actividad.Imagen);

            comboOrder.SelectedIndex = 0;
            //comboDificultad.SelectedIndex = 1;            

            //Habilitar los componentes en funcion de la actividad seleccionada
            for (int i=0; i<actividad.ActInterface.Count; i++)
            {
                bool currentCond = actividad.ActInterface[i];
                switch(i)
                {
                    case 0: //Tiempo total 
                        btnAddMin.Enabled = currentCond;
                        btnAddSec.Enabled = currentCond;
                        btnSubsMin.Enabled = currentCond;
                        btnSubsSec.Enabled = currentCond;
                        textMin.Enabled = currentCond;
                        textSec.Enabled = currentCond;
                        lblMin.Enabled = currentCond;
                        lblSec.Enabled = currentCond;
                        trackBarMin.Enabled = currentCond;
                        trackBarSec.Enabled = currentCond;
                        break;
                    case 1: //Repeticiones
                        btnAddRep.Enabled = currentCond;
                        btnSubsRep.Enabled = currentCond;
                        textRep.Enabled = currentCond;
                        lblRep.Enabled = currentCond;
                        trackBarRep.Enabled = currentCond;
                        break;
                    case 2: //Tiempo goal
                        btnAddTimeG.Enabled = currentCond;
                        btnSubsTimeG.Enabled = currentCond;
                        textTimeG.Enabled = currentCond;                        
                        lblTimeG.Enabled = currentCond;
                        trackBarTimeG.Enabled = currentCond;
                        break;
                    case 3: //Distancia goal
                        btnAddDistG.Enabled = currentCond;
                        btnSubsDistG.Enabled = currentCond;
                        textDistG.Enabled = currentCond;
                        lblDistG.Enabled = currentCond;
                        break;
                    case 4: //Orden goal
                        lblOrder.Enabled = currentCond;
                        comboOrder.Enabled = currentCond;
                        break;
                    case 5: //Panel auxiliar
                        if (actividad.NameAuxiliar == "SubPanelLevel")
                        {
                            subPanelLevel = new SubPanelLevel();
                            subPanelLevel.Dock = DockStyle.Fill; //Para ajustar
                            groupBoxAdi.Controls.Clear();
                            groupBoxAdi.Controls.Add(subPanelLevel);

                            //Metodo delegado para recibir cambios de valores en el UserControl hijo                                 
                            formFunctionPointer -= SubPanelLevelChanging;
                            formFunctionPointer += new functioncall(SubPanelLevelChanging);
                            subPanelLevel.UserSelectedIndexChangedPointer = formFunctionPointer;
                            subPanelLevel.SetSelectedIndex(1); //Dificultad normal por defecto
                        }
                        break;
                }
            }
            GetDataActivity();
        }
        
        private void btnSalir_Click(object sender, EventArgs e)
        {
            if (runningSession)
                CloseCOM();

            //Application.Exit();
            REVIREPanels.PanelPrincipal.sharedInstance.GetPanelREVIRE().Controls.Clear();
            REVIREPanels.PanelPrincipal.sharedInstance.GetPanelREVIRE().Controls.Add(Actividades);

            
        }

        //-------------------Lineas de separacion-------------------//
        private void linea1_Paint(object sender, PaintEventArgs e)
        {
            ControlPaint.DrawBorder(e.Graphics, e.ClipRectangle,
                SystemColors.ControlLightLight, 0, ButtonBorderStyle.Outset,
                SystemColors.ControlLightLight, 0, ButtonBorderStyle.Outset,
                SystemColors.ControlLightLight, 1, ButtonBorderStyle.Outset,
                SystemColors.ControlLightLight, 0, ButtonBorderStyle.Outset);
        }        

        private void linea2_Paint(object sender, PaintEventArgs e)
        {
            ControlPaint.DrawBorder(e.Graphics, e.ClipRectangle,
                SystemColors.ControlLightLight, 0, ButtonBorderStyle.Outset,
                SystemColors.ControlLightLight, 0, ButtonBorderStyle.Outset,
                SystemColors.ControlLightLight, 0, ButtonBorderStyle.Outset,
                SystemColors.ControlLightLight, 1, ButtonBorderStyle.Outset);
        }

        private void linea3_Paint(object sender, PaintEventArgs e)
        {
            ControlPaint.DrawBorder(e.Graphics, e.ClipRectangle,
               SystemColors.ControlLightLight, 0, ButtonBorderStyle.Outset,
               SystemColors.ControlLightLight, 0, ButtonBorderStyle.Outset,
               SystemColors.ControlLightLight, 0, ButtonBorderStyle.Outset,
               SystemColors.ControlLightLight, 1, ButtonBorderStyle.Outset);
        }
        //----------------------------------------------------------//


        //--------------------Tiempo total--------------------------//
        private void trackBarMin_Scroll(object sender, EventArgs e)
        {
            int value = SetDataTrackBar(trackBarMin, textMin);
            currentActivity.Minutos = value;
        }

        private void btnAddMin_Click(object sender, EventArgs e)
        {
            int value = trackBarMin.Value;

            if (value < trackBarMin.Maximum)
                value++;

            currentActivity.Minutos = value;
            trackBarMin.Value = value;
            textMin.Text = value.ToString();

            labelFocus.Focus();
        }

        private void btnSubsMin_Click(object sender, EventArgs e)
        {
            int value = trackBarMin.Value;
                        
            if (value > trackBarMin.Minimum)
                value--;

            currentActivity.Minutos = value;
            trackBarMin.Value = value;
            textMin.Text = value.ToString();

            labelFocus.Focus();
        }

        private void trackBarSec_Scroll(object sender, EventArgs e)
        {
            int value = SetDataTrackBar(trackBarSec, textSec);
            currentActivity.Segundos = value;
        }

        private void btnAddSec_Click(object sender, EventArgs e)
        {
            int value = trackBarSec.Value;

            if (value < trackBarSec.Maximum)
                value++;

            currentActivity.Segundos = value;
            trackBarSec.Value = value;
            textSec.Text = value.ToString();

            labelFocus.Focus();
        }

        private void btnSubsSec_Click(object sender, EventArgs e)
        {
            int value = trackBarSec.Value;

            if (value > trackBarSec.Minimum)
                value--;

            currentActivity.Segundos = value;
            trackBarSec.Value = value;
            textSec.Text = value.ToString();

            labelFocus.Focus();
        }
        //----------------------------------------------------------//

        //--------------------Repeticiones--------------------------//
        private void trackBarRep_Scroll(object sender, EventArgs e)
        {
            int value = trackBarRep.Value;
            currentActivity.Repeticiones = value;
            textRep.Text = value.ToString();
        }

        private void btnAddRep_Click(object sender, EventArgs e)
        {
            int value = trackBarRep.Value;

            if (value < trackBarRep.Maximum)
                value++;

            currentActivity.Repeticiones = value;
            trackBarRep.Value = value;
            textRep.Text = value.ToString();

            labelFocus.Focus();
        }

        private void btnSubsRep_Click(object sender, EventArgs e)
        {
            int value = trackBarRep.Value;

            if (value > trackBarRep.Minimum)
                value--;

            currentActivity.Repeticiones = value;
            trackBarRep.Value = value;
            textRep.Text = value.ToString();

            labelFocus.Focus();
        }
        //----------------------------------------------------------//

        //--------------------Tiempo Objetivo-----------------------//
        private void trackBarTimeG_Scroll(object sender, EventArgs e)
        {
            int value = SetDataTrackBar(trackBarTimeG, textTimeG);
            currentActivity.TiempoObjetivo = value;
        }

        private void btnAddTimeG_Click(object sender, EventArgs e)
        {
            int value = trackBarTimeG.Value;

            if (value < trackBarTimeG.Maximum)
                value++;

            currentActivity.TiempoObjetivo = value;
            trackBarTimeG.Value = value;
            textTimeG.Text = value.ToString();

            labelFocus.Focus();
        }

        private void btnSubsTimeG_Click(object sender, EventArgs e)
        {
            int value = trackBarTimeG.Value;

            if (value > trackBarTimeG.Minimum)
                value--;

            currentActivity.TiempoObjetivo = value;
            trackBarTimeG.Value = value;
            textTimeG.Text = value.ToString();

            labelFocus.Focus();
        }
        //----------------------------------------------------------//

        //------------------Distancia Objetivo----------------------//
        private void trackBarDistG_Scroll(object sender, EventArgs e)
        {
            int value = SetDataTrackBar(trackBarDistG, textDistG);
            currentActivity.Distancia = value;
        }

        private void btnAddDistG_Click(object sender, EventArgs e)
        {
            int value = trackBarDistG.Value;

            if (value < trackBarDistG.Maximum)
                value++;

            currentActivity.Distancia = value;
            trackBarDistG.Value = value;
            textDistG.Text = value.ToString();

            labelFocus.Focus();
        }

        private void btnSubsDistG_Click(object sender, EventArgs e)
        {
            int value = trackBarDistG.Value;

            if (value > trackBarDistG.Minimum)
                value--;

            currentActivity.Distancia = value;
            trackBarDistG.Value = value;
            textDistG.Text = value.ToString();

            labelFocus.Focus();
        }
        //----------------------------------------------------------//

        //----------------------Orden Objetivo----------------------//
        private void comboOrder_SelectedIndexChanged(object sender, EventArgs e)
        {
            switch(comboOrder.SelectedIndex)
            {
                case 0:
                    currentActivity.OrdenObjetivo = Actividad.Orden.Random;
                    break;
                case 1:
                    currentActivity.OrdenObjetivo = Actividad.Orden.Clockwise;
                    break;
                case 2:
                    currentActivity.OrdenObjetivo = Actividad.Orden.AntiClockwise;
                    break;
            }
            
        }
        //----------------------------------------------------------//

        //------------------------Adicionales-----------------------//
        private void SubPanelLevelChanging(int value)
        {
            //Si esta seleccionado actualmente una actividad con panel auxiliar, 
            //se almacena antes de cambiar
            if (currentActivity.NameAuxiliar == "SubPanelLevel")
            {
                currentActivity.DatosAuxiliares = subPanelLevel.GetDataBaseString();
            }
        }
        //----------------------------------------------------------//

        private void btnStart_Click(object sender, EventArgs e)
        {
            InitCOM();
            InitRecvThread();
            btnStart.Enabled = false;
            runningSession = true;

            /*   string nameActivity = currentActivity.Nombre + ".exe";

               //Obtener los argumentos de entrada del juego por orden
               int repeticiones = currentActivity.Repeticiones; //1
               int amplitud = currentActivity.Distancia * 10; //2
               int orden = (int)currentActivity.OrdenObjetivo; //3
               int nivel_asistencia = (int)currentActivity.TipoAsistencia; //4
               int fuerza = currentActivity.Nivel; //5

               int min = currentActivity.Minutos * 60;
               int sec = currentActivity.Segundos;
               string tiempo = (min + sec).ToString(); //6

               string path = Directory.GetCurrentDirectory() + @"\Activities\"+ currentActivity.Nombre+"\\" + nameActivity;

               //string ruta = @"D:\JuegoHRehabBuild\" + nameActivity;

               if (nameActivity == "FactoryTask.exe" || nameActivity == "HitTask.exe" || nameActivity == "DesPickAppleBird.exe")
               {
                   tiempo = textTimeG.Text;
               }

               int tmp_orden = orden;

               //Temporal para el juego imposible donde cambio orden por nivel dificultad
               if(nameActivity=="Impossible.exe")
               {
                   int s = subPanelLevel.GetSelectedIndex();
                   if (s == -1 || s == 0)
                   {
                       tmp_orden = 1;
                   }
                   else if(s==1)
                   {
                       tmp_orden = 2;
                   }
                   else if(s==2)
                   {
                       tmp_orden = 4;
                   }
               }

               //Temporal para el juego imposible donde cambio orden por nivel dificultad
               if (nameActivity == "ObjectsMemory.exe")
               {
                   int s = subPanelLevel.GetSelectedIndex();
                   if (s == -1 || s == 0)
                   {
                       tmp_orden = 0;
                   }
                   else if (s == 1)
                   {
                       tmp_orden = 1;
                   }
                   else if (s == 2)
                   {
                       tmp_orden = 2;
                   }
               }
               //Ejecutar la tarea
               System.Diagnostics.Process pStart = new System.Diagnostics.Process();
               pStart.StartInfo.FileName = path;
               pStart.StartInfo.Arguments = repeticiones + " " + amplitud + " " + tmp_orden + " " + ConvertModeControlRb(nivel_asistencia) + " " + fuerza + " " + tiempo;
             //  pStart.Start();

               //Inicializar la recepcion de los datos de valoracion
               //   InitCOM();
               //  List<float> values = ReceiveData();
               //  CloseCOM();

               //Guardar los datos finales de la tarea en el base de datos del REVIRE
             /*  List<float> values = new List<float>();
               values.Add(15);
               values.Add(2);

               //Primero compurba si ha recibido todos los valores
               if (values.Count > 1)
               {
                   currentActivity.Aciertos = (int)values[0];                
                   currentActivity.Fecha = DateTime.Now;

                   //Almacenar actividad en la base de datos
                   DBConnection.AddActivityToDataBase(currentActivity);

                   //Almacenar sesion en caso de ser necesario                
                   bool loading = DBConnection.LoadSesionsFromDataBase(currentActivity.UsuarioID);
                   if (loading)
                       lista_sesiones = DBConnection.GetListSesions();

                   //Comprobar si hay sesion del dia actual
                   bool sesExist = false;
                   foreach (Sesion ses in lista_sesiones)
                   {
                       if (currentActivity.Fecha.ToShortDateString().CompareTo(ses.Fecha.ToShortDateString()) == 0)
                       {
                           sesExist = true;
                           break;
                       }
                   }
                   if (!sesExist)
                   {
                       //Añadir una sesion nueva
                       Sesion sesion = new Sesion()
                       {
                           UsuarioID = currentActivity.UsuarioID,
                           Fecha = currentActivity.Fecha,
                           Comentarios = ""
                       };                                   
                       DBConnection.AddSesionToDataBase(sesion);
                   }
               }*/

        }

        private int ConvertModeControlRb(int level)
        {
            int mode = 1;

            if (level == 3)
                mode = 1;
            else if (level == 5 || level == 4)
                mode = 2;


            return mode;
        }

        private int SetDataTrackBar(TrackBar trackBar, Label label)
        {
            int value = trackBar.Value;
            label.Text = value.ToString();
            return value;
        }

        void GetDataActivity()
        {
            //Recolectar datos de inicio de la actividad
            currentActivity.Repeticiones = trackBarRep.Value;
            currentActivity.Minutos = trackBarMin.Value;
            currentActivity.Segundos = trackBarSec.Value;
            currentActivity.Distancia = trackBarDistG.Value;
            currentActivity.TiempoObjetivo = trackBarTimeG.Value;
            currentActivity.OrdenObjetivo = (Actividad.Orden)comboOrder.SelectedIndex;
        }

        //*****************Recibir datos desde la valoracion**************************//
        // Use this for initialization
        public void InitCOM()
        {
            try
            {
                //UDP de recepción
                localEndPoint = new IPEndPoint(IPAddress.Any, 0);
                udpRead = new UdpClient(localPort);

                //InitRecvThread();
            }
            catch (Exception err)
            {
                MessageBox.Show(err.ToString());
            }
        }

        public void InitRecvThread()
        {
            try
            {
                //Hilo para recibir datos
                receiveThread = new Thread(new ThreadStart(RunActivitiesThread));
                receiveThread.IsBackground = true;
                receiveThread.Start();

            }
            catch (Exception e)
            {
                MessageBox.Show(e.ToString());
            }
        }

        private void RunActivitiesThread()
        {
            string nameActivity = currentActivity.Nombre + ".exe";

            //Obtener los argumentos de entrada del juego por orden
            int repeticiones = currentActivity.Repeticiones; //1
            int amplitud = currentActivity.Distancia * 10; //2
            int orden = (int)currentActivity.OrdenObjetivo; //3
            int nivel_asistencia = (int)currentActivity.TipoAsistencia; //4
            int fuerza = currentActivity.Nivel; //5

            int min = currentActivity.Minutos * 60;
            int sec = currentActivity.Segundos;
            string tiempo = (min + sec).ToString(); //6

            string path = Directory.GetCurrentDirectory() + @"\Activities\" + currentActivity.Nombre + "\\" + nameActivity;

            //Si tiene tiempo por objetivo se cambia la variable de tiempo
            if(!btnAddMin.Enabled && btnAddTimeG.Enabled)
            { 
                tiempo = textTimeG.Text;
            }

            int tmp_orden = orden;

            //Temporal para el juego imposible donde cambio orden por nivel dificultad
            if (nameActivity == "Impossible.exe")
            {
                int s = subPanelLevel.GetSelectedIndex();
                if (s == -1 || s == 0)
                {
                    tmp_orden = 1;
                }
                else if (s == 1)
                {
                    tmp_orden = 2;
                }
                else if (s == 2)
                {
                    tmp_orden = 4;
                }
            }

            //Temporal para el juego imposible donde cambio orden por nivel dificultad
            if (nameActivity == "ObjectsMemory.exe")
            {
                int s = subPanelLevel.GetSelectedIndex();
                if (s == -1 || s == 0)
                {
                    tmp_orden = 0;
                }
                else if (s == 1)
                {
                    tmp_orden = 1;
                }
                else if (s == 2)
                {
                    tmp_orden = 2;
                }
            }
            

            SchedulerActividad.SetCurrentActivity(currentActivity);
            string path2 = SchedulerActividad.GetPathActivity();
            string command2 = ;

            //Ejecutar la tarea
            System.Diagnostics.Process pStart = new System.Diagnostics.Process();
            pStart.StartInfo.FileName = path;
            pStart.StartInfo.Arguments = repeticiones + " " + amplitud + " " + tmp_orden + " " + ConvertModeControlRb(nivel_asistencia) + " " + fuerza + " " + tiempo;
            //  pStart.Start();

            //Inicializar la recepcion de los datos de valoracion
            // InitCOM();
            List<float> values = ReceiveData();
                //  CloseCOM();

                // List<float> values = new List<float>();
                //                  values.Add(15);


                //Primero compurba si ha recibido todos los valores
                /* if (values.Count >= 1)
                 {
                     actividad.Aciertos = (int)values[0];
                     actividad.Fecha = DateTime.Now;

                     //Almacenar actividad en la base de datos
                     DBConnection.AddActivityToDataBase(actividad);

                     //Almacenar sesion en caso de ser necesario                
                     bool loading = DBConnection.LoadSesionsFromDataBase(userID);

                     List<Sesion> lista_sesiones;
                     if (loading)
                     {
                         lista_sesiones = DBConnection.GetListSesions();

                         //Comprobar si hay sesion del dia actual
                         bool sesExist = false;
                         foreach (Sesion ses in lista_sesiones)
                         {
                             if (actividad.Fecha.ToShortDateString().CompareTo(ses.Fecha.ToShortDateString()) == 0)
                             {
                                 sesExist = true;
                                 break;
                             }
                         }
                         if (!sesExist)
                         {
                             //Añadir una sesion nueva
                             Sesion sesion = new Sesion()
                             {
                                 UsuarioID = userID,
                                 Fecha = actividad.Fecha,
                                 Comentarios = ""
                             };
                             DBConnection.AddSesionToDataBase(sesion);
                         }
                     }
                 }*/
            

            //Si sale significa que se han completado todas las actividades, entonces
            CloseCOM(); //Cierra udp y el hilo adicional
            runningSession = false;
            btnStart.Enabled = true;

        }

        private void RunActivitiesThread2()
        {
            SchedulerActividad.SetCurrentActivity(currentActivity);

            //Ejecutar la tarea
            System.Diagnostics.Process pStart = new System.Diagnostics.Process();
            pStart.StartInfo.FileName = SchedulerActividad.GetPathActivity();
            pStart.StartInfo.Arguments = SchedulerActividad.GetRunActivityName();
            //  pStart.Start();

            //Inicializar la recepcion de los datos de valoracion
            // InitCOM();
            List<float> values = ReceiveData();
            //  CloseCOM();

            // List<float> values = new List<float>();
            //                  values.Add(15);


            //Primero compurba si ha recibido todos los valores
            /* if (values.Count >= 1)
             {
                 actividad.Aciertos = (int)values[0];
                 actividad.Fecha = DateTime.Now;

                 //Almacenar actividad en la base de datos
                 DBConnection.AddActivityToDataBase(actividad);

                 //Almacenar sesion en caso de ser necesario                
                 bool loading = DBConnection.LoadSesionsFromDataBase(userID);

                 List<Sesion> lista_sesiones;
                 if (loading)
                 {
                     lista_sesiones = DBConnection.GetListSesions();

                     //Comprobar si hay sesion del dia actual
                     bool sesExist = false;
                     foreach (Sesion ses in lista_sesiones)
                     {
                         if (actividad.Fecha.ToShortDateString().CompareTo(ses.Fecha.ToShortDateString()) == 0)
                         {
                             sesExist = true;
                             break;
                         }
                     }
                     if (!sesExist)
                     {
                         //Añadir una sesion nueva
                         Sesion sesion = new Sesion()
                         {
                             UsuarioID = userID,
                             Fecha = actividad.Fecha,
                             Comentarios = ""
                         };
                         DBConnection.AddSesionToDataBase(sesion);
                     }
                 }
             }*/


            //Si sale significa que se han completado todas las actividades, entonces
            CloseCOM(); //Cierra udp y el hilo adicional
            runningSession = false;
            btnStart.Enabled = true;

        }
        //Hilo de recepcion
        private List<float> ReceiveData()
        {
            List<float> values = new List<float>();
            try
            {
                //Recepcion de bytes
                byte[] data = udpRead.Receive(ref localEndPoint);

                if (data.Length > 4)
                {
                    for (int i = 0; i < 8; i++)
                    {
                        float d = BitConverter.ToSingle(data, (4 * i));
                        values.Add(d);
                    }
                }
                else
                {
                    float d = BitConverter.ToSingle(data, 0);
                    values.Add(d);
                }

            }
            catch (Exception err)
            {
               // MessageBox.Show(err.Message);
            }

            return values;

        }

        public void CloseCOM()
        {
            try
            {
                //Eliminar puerto COM
                if (udpRead != null)
                {
                    udpRead.Close();
                    udpRead = null;

                    //Borrar hilo de recepcion
                    receiveThread.Abort();
                    receiveThread = null;
                }
            }
            catch (Exception e)
            {
                MessageBox.Show(e.Message.ToString());
            }
        }
        //****************************************************************************//

 
    }
}
