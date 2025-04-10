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
    public partial class PanelProgramacion : UserControl
    {
        private PanelRobot probot;
        public PanelRobot Probot
        {
            get
            {
                if (probot == null)
                {
                    probot = new PanelRobot(true, userID);
                    probot.Dock = DockStyle.Fill; //Para ajustar
                }
                return probot;
            }
        }

        //Variables UDP
        public string localIP = "127.0.0.1";
        private UdpClient udpRead;
        public int localPort = 33333;
        private IPEndPoint localEndPoint;
        private static Thread receiveThread;
        private bool runningSession = false;

        List<PropActividades> lista_propactividades = new List<PropActividades>();
        List<Actividad> actividades_programadas = new List<Actividad>();
        Actividad currentActivity;
        ImageList imgs = new ImageList();

        int userID;

        int contActividades = 0;

        //Paneles auxiliares y metodos delegados para recibir del usercontrol hijo
        SubPanelLevel subPanelLevel;        
        public delegate void functioncall(int message);
        private event functioncall formFunctionPointer;


        public PanelProgramacion(int usuario)
        {
            userID = usuario;

            InitializeComponent();

            CheckForIllegalCrossThreadCalls = false;

            //Configurar las imágenes de los botones
            imgs.ImageSize = new Size(100, 100);

            //Carga todas las propiedades de las actividades           
            LoadActivitiesFromDataBase();

            //Crear botones
            CreateButtons();
            
            listBoxAct.DrawMode = System.Windows.Forms.DrawMode.OwnerDrawVariable;

            //Bloquear elementos
            ResetInterface(false);
        }

        private void btnBack_Click(object sender, EventArgs e)
        {
            //Application.Exit();
            REVIREPanels.PanelPrincipal.sharedInstance.GetPanelREVIRE().Controls.Clear();
            REVIREPanels.PanelPrincipal.sharedInstance.GetPanelREVIRE().Controls.Add(Probot);

            //Si la sesion de ehercicios se esta ejecutando se corta el udp
            if (runningSession)
                CloseCOM();

        //Controls.Add();
    }

        private void listBoxAct_DrawItem(object sender, DrawItemEventArgs e)
        {

            if (e.Index != -1)
            {
                int d = (int)listBoxAct.Items[e.Index];
                /* var item = listBox1.Items[e.Index];
                 Font customFont = e.Font;
                 Brush customBrush = new SolidBrush(e.ForeColor);

                 e.DrawBackground();
                 /*e.Graphics.DrawString("hola",
                      customFont, customBrush, e.Bounds, StringFormat.GenericDefault);*/
                //e.DrawFocusRectangle();
                /*   Image img = Image.FromFile(@"D:\UnityProjects\DeveloperVisualCSharp\REVIRE\Resources\iconos\A2D10_ico.png"); 
                   e.Graphics.DrawImage(img, new PointF(e.Bounds.Left, e.Bounds.Top));*/

                // Draw the background of the ListBox control for each item.
                e.DrawBackground();

                // Draw the current item text based on the current Font 
                // and the custom brush settings.            

                e.Graphics.DrawImage(imgs.Images[d], new PointF(e.Bounds.Left + 5, e.Bounds.Top + 5));
                /*e.Graphics.DrawString(listBox1.Items[e.Index].ToString(),
                    e.Font, myBrush, e.Bounds, StringFormat.GenericDefault);*/
                // If the ListBox has focus, draw a focus rectangle around the selected item.
                e.DrawFocusRectangle();
            }
        }
       

        private void btnDelete_Click(object sender, EventArgs e)
        {
            int index = listBoxAct.SelectedIndex;
            if(index != -1)
            {
                //Elimina el elemento seleccionado
                listBoxAct.Items.RemoveAt(index);
                actividades_programadas.RemoveAt(actividades_programadas.Count - index - 1);

                //Señala la reduccion de la actividad en la etiqueta        
                contActividades--;
                lblNumAct.Text = contActividades.ToString();

                //Seleccionar el siguiente en la lista
                if (contActividades > 0)
                {
                    if (index == listBoxAct.Items.Count)
                        index--;

                    listBoxAct.SelectedIndex = index;

                    int cant_activities = actividades_programadas.Count;
                    currentActivity = actividades_programadas[cant_activities - index - 1];
                    UpdateParamActivity(currentActivity);

                }
                else
                {
                    ResetParamActivityInterface();
                    ResetInterface(false);
                }
                
                
            }
        }

        private void listBoxAct_MeasureItem(object sender, MeasureItemEventArgs e)
        {
            e.ItemHeight = 110;
        }

        //Crear botones
        private void CreateButtons()
        {
            int cont = 0;
            foreach(Image img in imgs.Images)
            {
                cont++;
                Button button = new Button();

                // Set Button properties
                button.AutoSize = true;                
                button.Text = ">>";
                button.Name = lista_propactividades[cont-1].Nombre;// "Actividad" + cont;
                button.Font = new Font("Microsoft Sans Serif", 16);
                button.Image = img;
                button.Tag = cont-1;
                button.TextImageRelation = TextImageRelation.ImageBeforeText;
                button.Click += new EventHandler(button_Click);
                flowLayoutPanel1.Controls.Add(button);
            }            
        }

        private void button_Click(object sender, EventArgs e)
        {
            //Generar una actividad y guardarla
            Actividad actividad = new Actividad();
            actividades_programadas.Add(actividad);
            currentActivity = actividad;
            InitParamActivity();
            ResetParamActivityInterface();

            // Process the panel clicks here
            Button index = (Button) sender;

            //Condiciones de bloqueo de la interfaz
            currentActivity.Nombre = index.Name;
            currentActivity.Imagen = Directory.GetCurrentDirectory() + @"\Resources\fondos\" +
                lista_propactividades[(int)index.Tag].Fondo;
            currentActivity.ActInterface = lista_propactividades[(int)index.Tag].ActInterface;
            currentActivity.ActInterface.Add(lista_propactividades[(int)index.Tag].Asistido);
            currentActivity.ActInterface.Add(lista_propactividades[(int)index.Tag].AsistidoReg);
            currentActivity.ActInterface.Add(lista_propactividades[(int)index.Tag].Libre);
            currentActivity.ActInterface.Add(lista_propactividades[(int)index.Tag].ResistidoReg);
            currentActivity.ActInterface.Add(lista_propactividades[(int)index.Tag].Resistido);

            currentActivity.NameAuxiliar = lista_propactividades[(int)index.Tag].NameAuxiliar;

            //Añadir actividad en la lista
            listBoxAct.Items.Insert(0, index.Tag);
            listBoxAct.SelectedIndex = 0;            

            //MessageBox.Show("Button "+index.Name +" is clicked");
            contActividades++;
            lblNumAct.Text = contActividades.ToString();               
        }

       
        private void LoadActivitiesFromDataBase()
        {
            //Extraer propiedades de la actividad dentro de la base de datos
            bool dbLoaded = DBConnection.LoadPropActFromDataBase();
            if (dbLoaded)
            {
                lista_propactividades = DBConnection.GetListPropActivities();

                string path = Directory.GetCurrentDirectory() + @"\Resources\iconos\";
                foreach (PropActividades prop in lista_propactividades)
                    imgs.Images.Add(Image.FromFile(path + prop.Icono));               
            }
        }

        //--------------Botones nivel de asistencia-----------------//
        private void btn1Asistido_Click(object sender, EventArgs e)
        {
            //Condiciones barra de nivel
            SetCondTrackBarLevel(false, 100, Actividad.TypeAsistencia.Assisted);
            SetTypeButtonsSelection(btn1Asistido);
        }

        private void btn2AsistidoReg_Click(object sender, EventArgs e)
        {
            //Condiciones barra de nivel
            SetCondTrackBarLevel(true, 50, Actividad.TypeAsistencia.AdjustAssisted);
            SetTypeButtonsSelection(btn2AsistidoReg);
            
        }

        private void btn3Libre_Click(object sender, EventArgs e)
        {
            //Condiciones barra de nivel
            SetCondTrackBarLevel(false, 0, Actividad.TypeAsistencia.Free);
            SetTypeButtonsSelection(btn3Libre);            
        }

        private void btn4ResistidoReg_Click(object sender, EventArgs e)
        {
            //Condiciones barra de nivel
            SetCondTrackBarLevel(true, 50, Actividad.TypeAsistencia.AdjustResisted);
            SetTypeButtonsSelection(btn4ResistidoReg);
        }

        private void btn5Resistido_Click(object sender, EventArgs e)
        {
            //Condiciones barra de nivel
            SetCondTrackBarLevel(false, 100, Actividad.TypeAsistencia.Resisted);
            SetTypeButtonsSelection(btn5Resistido);
        }       

        private void SetTypeButtonsSelection(Button currentButton)
        {
            //Eliminar seleccion de los botones
            btn1Asistido.FlatAppearance.BorderSize = 1;
            btn2AsistidoReg.FlatAppearance.BorderSize = 1;
            btn3Libre.FlatAppearance.BorderSize = 1;
            btn4ResistidoReg.FlatAppearance.BorderSize = 1;
            btn5Resistido.FlatAppearance.BorderSize = 1;

            currentButton.FlatAppearance.BorderSize = 6;

            labelLevel.Focus();
        }

        

        private void trackBarLevel_Scroll(object sender, EventArgs e)
        {
            currentActivity.Nivel = trackBarLevel.Value;
            labelLevel.Text = trackBarLevel.Value.ToString();
        }

        private void SetCondTrackBarLevel(bool enabled, int value, Actividad.TypeAsistencia type)
        {
            //Establecer valor en los elementos
            if (!enabled)
            {
                trackBarLevel.Value = value;
                labelLevel.Text = value.ToString();
            }

            currentActivity.TipoAsistencia = type;
            currentActivity.Nivel = trackBarLevel.Value;
            trackBarLevel.Enabled = enabled;

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
            switch (comboOrder.SelectedIndex)
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

        private int SetDataTrackBar(TrackBar trackBar, Label label)
        {
            int value = trackBar.Value;
            label.Text = value.ToString();
            return value;
        }

        private void linea1_Paint(object sender, PaintEventArgs e)
        {
            ControlPaint.DrawBorder(e.Graphics, e.ClipRectangle,
                SystemColors.ControlLightLight, 0, ButtonBorderStyle.Outset,
                SystemColors.ControlLightLight, 0, ButtonBorderStyle.Outset,
                SystemColors.ControlLightLight, 1, ButtonBorderStyle.Outset,
                SystemColors.ControlLightLight, 0, ButtonBorderStyle.Outset);
        }

        //-------Modificar parametros dentro de la interfaz---------//
        private void ResetParamActivityInterface()
        {
            //Resetear labels y trackbars
            textMin.Text = "3";
            trackBarMin.Value = 3;
            textSec.Text = "0";
            trackBarSec.Value = 0;
            textRep.Text = "5";
            trackBarRep.Value = 5;
            textDistG.Text = "20";
            trackBarDistG.Value = 20;
            textTimeG.Text = "10";
            trackBarTimeG.Value = 10;
            comboOrder.SelectedIndex = 0;
        }

        private void InitParamActivity()
        {
            //Inicializar actividad. Atributos comunes
            currentActivity.Minutos = 3;
            currentActivity.Segundos = 0;
            currentActivity.Repeticiones = 5;            
            currentActivity.Distancia = 20;
            currentActivity.TiempoObjetivo = 10;
            currentActivity.TipoAsistencia = Actividad.TypeAsistencia.Free;
            currentActivity.Nivel = 0;
            currentActivity.OrdenObjetivo = Actividad.Orden.Random;            

            //Establece por defecto el nivel de asistencia libre
            btn3Libre.PerformClick();
        }

        private void UpdateParamActivity(Actividad currentAct)
        {
            //Resetear labels y trackbars
            textMin.Text = currentAct.Minutos.ToString();
            trackBarMin.Value = currentAct.Minutos;
            textSec.Text = currentAct.Segundos.ToString();
            trackBarSec.Value = currentAct.Segundos;
            textRep.Text = currentAct.Repeticiones.ToString();
            trackBarRep.Value = currentAct.Repeticiones;
            textDistG.Text = currentAct.Distancia.ToString();
            trackBarDistG.Value = currentAct.Distancia;
            textTimeG.Text = currentAct.TiempoObjetivo.ToString();
            trackBarTimeG.Value = currentAct.TiempoObjetivo;

            //Cajas de seleccion            
            comboOrder.SelectedIndex = (int)currentAct.OrdenObjetivo;

            //Actualizar panel auxiliar
            if (currentAct.NameAuxiliar == "SubPanelLevel")
            {
                string data_string = currentAct.DatosAuxiliares;
                float[] data_array = SplitAuxiliarData(data_string);                
                subPanelLevel.SetSelectedIndex((int)data_array[0]);
            }

            //Actualizar imagen de la actividad         
            fondoActividad.BackgroundImage = Image.FromFile(currentAct.Imagen);

            //Actualizar tipo y nivel de asistencia
            switch (currentAct.TipoAsistencia)
            {
                case Actividad.TypeAsistencia.Assisted:                   
                    trackBarLevel.Enabled = false;
                    SetTypeButtonsSelection(btn1Asistido);                   
                    break;
                case Actividad.TypeAsistencia.AdjustAssisted:                   
                    trackBarLevel.Enabled = true;
                    SetTypeButtonsSelection(btn2AsistidoReg);                    
                    break;
                case Actividad.TypeAsistencia.Free:                    
                    trackBarLevel.Enabled = false;
                    SetTypeButtonsSelection(btn3Libre);                    
                    break;
                case Actividad.TypeAsistencia.AdjustResisted:                    
                    trackBarLevel.Enabled = true;
                    SetTypeButtonsSelection(btn4ResistidoReg);                    
                    break;
                case Actividad.TypeAsistencia.Resisted:                    
                    trackBarLevel.Enabled = false;
                    SetTypeButtonsSelection(btn5Resistido);                    
                    break;                
                default:
                    break;
            }
            trackBarLevel.Value = currentAct.Nivel;
            labelLevel.Text = currentAct.Nivel.ToString();
            
        }

        private void ResetInterface(bool currentCond)
        {
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
            //Repeticiones
            btnAddRep.Enabled = currentCond;
            btnSubsRep.Enabled = currentCond;
            textRep.Enabled = currentCond;
            lblRep.Enabled = currentCond;
            trackBarRep.Enabled = currentCond;
            //Tiempo goal
            btnAddTimeG.Enabled = currentCond;
            btnSubsTimeG.Enabled = currentCond;
            textTimeG.Enabled = currentCond;
            lblTimeG.Enabled = currentCond;
            trackBarTimeG.Enabled = currentCond;
            //Distancia goal
            btnAddDistG.Enabled = currentCond;
            btnSubsDistG.Enabled = currentCond;
            textDistG.Enabled = currentCond;
            lblDistG.Enabled = currentCond;

            //Orden goal
            lblOrder.Enabled = currentCond;
            comboOrder.Enabled = currentCond;

            btn1Asistido.Enabled = currentCond;
            btn2AsistidoReg.Enabled = currentCond;
            btn3Libre.Enabled = currentCond;
            btn4ResistidoReg.Enabled = currentCond;
            btn5Resistido.Enabled = currentCond;
            trackBarLevel.Enabled = currentCond;

            if (!currentCond)
            {
                btn1Asistido.FlatAppearance.BorderSize = 1;
                btn2AsistidoReg.FlatAppearance.BorderSize = 1;
                btn3Libre.FlatAppearance.BorderSize = 1;
                btn4ResistidoReg.FlatAppearance.BorderSize = 1;
                btn5Resistido.FlatAppearance.BorderSize = 1;
            }

            //Borra el panel auxiliar
            groupBoxAdi.Controls.Clear();
        }
        //----------------------------------------------------------//

        //*********************Procesamineto del panel auxiliar************************//
        private void SubPanelLevelChanging(int value)
        {
            //Si esta seleccionado actualmente una actividad con panel auxiliar, 
            //se almacena antes de cambiar
            if (currentActivity.NameAuxiliar == "SubPanelLevel")
            {
                currentActivity.DatosAuxiliares = subPanelLevel.GetDataBaseString();
            }           
        }        

        private float[] SplitAuxiliarData(string data)
        {
            float[] arraydata = new float[1] {1}; //Para inicar nivel normal
            if (data != null)
            {
                //Separa el string
                string[] datos = data.Split('_');

                //Array de vuelta
                arraydata = new float[datos.Length - 1];

                //Convertir strings en un array de variables de info del robot       
                for (int i = 1; i < datos.Length; ++i)
                    arraydata[i-1] = float.Parse(datos[i].Replace('.', ','));
            }
            return arraydata;
        }        
        //****************************************************************************//


        //************************Seleccion actividad creada**************************//
        private void listBoxAct_SelectedIndexChanged(object sender, EventArgs e)
        {           
            int index = listBoxAct.SelectedIndex;
            if (index != -1)
            {
                int cant_activities = actividades_programadas.Count;
                currentActivity = actividades_programadas[cant_activities - index - 1];

                //Bloquear elementos desactivados en funcion de la actividad
                //Habilitar los componentes en funcion de la actividad seleccionada
                for (int i = 0; i < currentActivity.ActInterface.Count; i++)
                {
                    bool currentCond = currentActivity.ActInterface[i];
                    switch (i)
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
                        case 5:
                            groupBoxAdi.Controls.Clear();
                            if (currentActivity.NameAuxiliar == "SubPanelLevel")
                            {
                                subPanelLevel = new SubPanelLevel();
                                subPanelLevel.Dock = DockStyle.Fill; //Para ajustar                                
                                groupBoxAdi.Controls.Add(subPanelLevel);

                                //Datos auxiliares e iniciales de la actividad
                                string datos = currentActivity.DatosAuxiliares;


                                //Metodo delegado para recibir cambios de valores en el UserControl hijo                                 
                                formFunctionPointer -= SubPanelLevelChanging;
                                formFunctionPointer += new functioncall(SubPanelLevelChanging);
                                subPanelLevel.UserSelectedIndexChangedPointer = formFunctionPointer;
                            }
                            break;
                        case 6:
                            btn1Asistido.Enabled = currentCond;
                            break;
                        case 7:
                            btn2AsistidoReg.Enabled = currentCond;
                            break;
                        case 8:
                            btn3Libre.Enabled = currentCond;
                            break;
                        case 9:
                            btn4ResistidoReg.Enabled = currentCond;
                            break;
                        case 10:
                            btn5Resistido.Enabled = currentCond;
                            break;
                    }
                }

                //Actualizar los parametros 
                UpdateParamActivity(currentActivity);
            }
        }
        //****************************************************************************//


        //********************Iniciar la sesion programada****************************//
        private void btnStartRoutine_Click(object sender, EventArgs e)
        {
            if (actividades_programadas.Any())
            {
                InitCOM();
                InitRecvThread();
                btnStartRoutine.Enabled = false;
                runningSession = true;

            /*    foreach (Actividad actividad in actividades_programadas)
                {
                    //Nombre de la actividad  
             /*       string nameActivity = actividad.Nombre + ".exe";

                    //Obtener los argumentos de entrada del juego por orden
                    int repeticiones = actividad.Repeticiones; //1
                    int amplitud = actividad.Distancia * 10; //2
                    int orden = (int)actividad.OrdenObjetivo; //3
                    int nivel_asistencia = (int)actividad.TipoAsistencia; //4
                    int fuerza = actividad.Nivel; //5

                    int min = actividad.Minutos * 60;
                    int sec = actividad.Segundos;
                    string tiempo = (min + sec).ToString(); //6

                    string path = Directory.GetCurrentDirectory() + @"\Activities\" + actividad.Nombre + "\\" + nameActivity;

                    //string ruta = @"D:\JuegoHRehabBuild\" + nameActivity;

                    if (nameActivity == "FactoryTask.exe" || nameActivity == "HitTask.exe" || nameActivity == "DesPickAppleBird.exe")
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


                    //Ejecutar la tarea
                    System.Diagnostics.Process pStart = new System.Diagnostics.Process();
                    pStart.StartInfo.FileName = path;
                    pStart.StartInfo.Arguments = repeticiones + " " + amplitud + " " + tmp_orden + " " + ConvertModeControlRb(nivel_asistencia) + " " + fuerza + " " + tiempo;
                   // pStart.Start();

                /*    //Inicializar la recepcion de los datos de valoracion
                    InitCOM();
                    List<float> values = ReceiveData();
                    CloseCOM();*/

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
              //  }
            }
            else
            {
                MessageBox.Show("No se ha programado ninguna actividad.");
            }

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
        //****************************************************************************//


        //*****************Recibir datos desde la valoracion**************************//
        // Use this for initialization
        public void InitCOM()
        {
            try
            {
                //UDP de recepción
                localEndPoint = new IPEndPoint(IPAddress.Any, 0);
                udpRead = new UdpClient(localPort);
                udpRead.Client.ReceiveTimeout = 0;                
            }
            catch (Exception e)
            {
                MessageBox.Show(e.ToString());
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
            foreach (Actividad actividad in actividades_programadas)
            {
                //Nombre de la actividad  
                string nameActivity = actividad.Nombre + ".exe";

                //Obtener los argumentos de entrada del juego por orden
                int repeticiones = actividad.Repeticiones; //1
                int amplitud = actividad.Distancia * 10; //2
                int orden = (int)actividad.OrdenObjetivo; //3
                int nivel_asistencia = (int)actividad.TipoAsistencia; //4
                int fuerza = actividad.Nivel; //5

                int min = actividad.Minutos * 60;
                int sec = actividad.Segundos;
                string tiempo = (min + sec).ToString(); //6

                string path = Directory.GetCurrentDirectory() + @"\Activities\" + actividad.Nombre + "\\" + nameActivity;

                //string ruta = @"D:\JuegoHRehabBuild\" + nameActivity;

                //Si tiene tiempo por objetivo se cambia la variable de tiempo
                if (!btnAddMin.Enabled && btnAddTimeG.Enabled)
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
            }

            //Si sale significa que se han completado todas las actividades, entonces
            CloseCOM(); //Cierra udp y el hilo adicional
            runningSession = false;
            btnStartRoutine.Enabled = true;

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


        //*******************Cerrar panel mediante teclado****************************//


        //****************************************************************************//

    }
}
