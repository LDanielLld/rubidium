using System.Collections;
using UnityEngine;
using System;
using UnityEngine.UI;



#region [Enum] Comandos de teleoperacion
/// <summary>
/// Identificadores de la informacion compartida
/// </summary>
public enum MsgID
{ 
    MSG_INIT_DATA = 0x01,
    MSG_SET_SCENE = 0x02,
    MSG_GAME_STATE = 0x03,
    MSG_TASK_STATE = 0x04,    
    MSG_DATA_THERAPIST = 0x05,
    MSG_DATA_PATIENT=0x06
}
#endregion


/// <summary>
/// Controlador de todo el proceso de intercambio de datos entre el paciente y el terapeuta
/// </summary>
public class TeleOperationController : MonoBehaviour
{

    //Instancia compartida
    public static TeleOperationController sharedInstance;



    //************************************Variables************************************//
    //*********************************************************************************//
    #region [Variables] Gestion de interfaz
    [Header("Gestión Interfaz")]
    public Canvas canvas; //Panel con la informacion de la teleoperacion    
    public Image checkReady; //Tick de listo
    public Text lblCond1, lblCond2, lblCond3, lblCond4; //Etiquetas de condiciones
    public Text lblTimeCond1, lblTimeCond3; //Etiquetas de tiempo;
    public Text textEstado; //Para mostrar si existe algun error en la conexion
    #endregion    

    #region [Variables] Network
    [Header("NetWorking")]
    public string roomName = "LobbyMain"; //Nombre de la sala
    public TypeRol networkRole;    
    #endregion

    #region [Variables] Compensacion de Lag
    public Vector3 currentPosition = Vector3.zero;
    public double lastPacketTime = 0;
    public double currentPacketTime = 0;
    #endregion

    #region [Variables] Tiempos
    private float timeLoad = 0.0f;
    private readonly float desconexionTime = 5.0f;
    public float currentTime = 0f;
    private readonly int TIME_TO_RETRY = 5;
    #endregion

    #region [Variables] Estados
    private enum TO_State
    {
        READY,
        JOINING,
        STARTING,
        RETRY,
        CREATING,
        WAITING,
        IDLE,
        INIT_SLAVE, //Actualiza datos iniciales
        PROBLEM_WARNING, //Ha surgido un problema en el lado remoto (warning remoto)
        PROBLEM_DESCONEXION, //Indica un problema en el lado remoto (desconexion)
        NONE

    }
    TO_State operationState = TO_State.NONE;
    #endregion
    
    #region [Variables] Datos recibidos 
    public Vector2 rcv_vfSensor;
    public Vector3 rcv_vPosition;
    #endregion
    //*********************************************************************************//
    //*********************************************************************************//


        
    //************************Inicialización y bucle principal*************************//
    //*********************************************************************************//
    #region [Unity Functions] Inicializacion
    private void Awake()
    {
        //Instancia compartida
        if (sharedInstance == null)
            sharedInstance = this;
    }
    
    private void Start()
    {
        //Conectar a la red de Photon
        try
        {
            operationState = TO_State.JOINING;
            
        }
        catch(Exception e)
        {
            textEstado.text = e.Message;
        }
    }
    #endregion

    #region [Unity Functions] Bucle principal
    private void Update()
    {
        //Maquina de estados para controlar la interfaz
        if (operationState == TO_State.JOINING)
        {
            //En primer lugar espera que el usuario se conecte a la red de Photon
            timeLoad += Time.deltaTime;
            lblTimeCond1.text = UIManager.sharedInstance.CalculateStringTime(timeLoad, 1);
        }
        else if (operationState == TO_State.RETRY)
        {
            //Vuelve a intentar el conectarse al ejercicio
            timeLoad += Time.deltaTime;
            if (timeLoad <= TIME_TO_RETRY)
            {
                //Actuliza contador de segundos
                string seconds = UIManager.sharedInstance.CalculateStringTime(timeLoad, 0);
                int s = int.Parse(seconds);
                int restTime = TIME_TO_RETRY - s;
                lblCond2.text = "Ejercicio no iniciado. Reintentando: " + restTime;
            }
            else if (timeLoad > TIME_TO_RETRY)
            {
                
            }
        }
        else if (operationState == TO_State.WAITING)
        {
            
        }        
        else if (operationState == TO_State.PROBLEM_WARNING)
        {
            //Si existe el problema remoto de warning sólo espera hasta que se solucione
            timeLoad += Time.deltaTime;
            lblTimeCond3.text = UIManager.sharedInstance.CalculateStringTime(timeLoad, 1);
        }
        else if (operationState == TO_State.PROBLEM_DESCONEXION)
        {
            //Si existe el problema remoto de desconexion espera unos segundos para cerrar el ejercicio
            timeLoad += Time.deltaTime;

            //Cuenta atras y cierre de ejercicio
            float sTime = desconexionTime - timeLoad;
            if(sTime>0)
                lblTimeCond3.text = UIManager.sharedInstance.CalculateStringTime(sTime + 1, 0);
            else
                GameManager.sharedInstance.ClickExit();
        }
    }
    #endregion
    //*********************************************************************************//
    //*********************************************************************************//





    //***********************Control del módulo de teleoperacion***********************//
    //*********************************************************************************// 
    #region [Public Functions] Complementos
    /// <summary>
    /// Inicializa la actividad dependiendo del rol actual
    /// </summary>
    /// <returns></returns>
    IEnumerator InitPatient()
    {
        //El terapeuta envia los datos de inicializacií de la actividad al paciente
        if (networkRole == TypeRol.THERAPISTH)
        {
           
        }

        //Espera dos segundos 
        yield return new WaitForSecondsRealtime(2.0f);        

        //Empieza la actividad
        operationState = TO_State.READY;

        //Activa que se pueda usar el robot
        RobotPlayerController.sharedInstance.modeControl.IsReady = true;
        


        //Desconecta interfaz de visualizacion de estado
        canvas.enabled = false;

        //El paciente indica al terapeuta cuando debe empezar la actividad
        if (networkRole == TypeRol.PATIENT) //Envia para que se conecte            
            GameManager.sharedInstance.SetGameState(GameState.STARTING); //Inicia el juego, con la cuenta atrás
        
    }
    
    
    /// <summary>
    /// Envia los datos auxiliares a través del modulo de comunicacion. Pueden ser el estado
    /// general del juego y el estado concreto de la tarea. Hace de puente entre el componente
    /// RoboticMode y el que se encarga de enviar los datos via online.
    /// </summary>
    /// <param name="type"></param>
    public void SendAuxiliarData(TypeState type)
    {
       
    }
    #endregion
    //*********************************************************************************//
    //*********************************************************************************//
       


    //**********************Gestion de la interfaz teleoperacion***********************//
    //*********************************************************************************//
    #region [UI] Interfaz Teleoperacion       
    /// <summary>
    /// Configura la interfaz de telerehabilitacion para indicar 
    /// que se ha producido un warning en el sistema remoto
    /// </summary>
    public void WarningProblem()
    {
        //Conecta los elementos de la interfaz
        string type = ConfigureUI(TO_State.PROBLEM_WARNING);

        //Actualiza el texto de la interfaz
        lblCond2.text = "Se ha producido un WARNING \n" +
            "en el sistema del " + type;
        lblCond3.text = "Esperando respuesta: ";
        lblTimeCond3.text = "00:00";
        timeLoad = 0.0f;

        //Cambia el estado actual para bloquear el avance de los objetivos
        GameManager.sharedInstance.currentGameState = GameState.SAFETYMODE_REMOTE;
    }

    /// <summary>
    /// Elimina el estado actual de bloqueo por warning del sistema remoto
    /// </summary>
    public void DisconectWarningProblem()
    {
        canvas.enabled = false;

        GameManager.sharedInstance.currentGameState = GameState.INGAME; //Asignar el estado del juego actual   
        TargetManager.sharedInstance.ReStart(); //Reinicia el juego  
    }

    /// <summary>
    /// Configura la interfaz de telerehabilitacion para indicar 
    /// que se ha producido una desconexion en el sistema remoto
    /// </summary>
    public void DesconexionProblem()
    {
        //Conecta los elementos de la interfaz
        string type = ConfigureUI(TO_State.PROBLEM_DESCONEXION);

        //Actualiza el texto de la interfaz
        lblCond2.text = "El " + type + " se ha desconectado.";            
        lblCond3.text = "Cerrando ejercicio en: ";
        lblTimeCond3.text = desconexionTime.ToString();
        timeLoad = 0.0f;
        
    }
    
    /// <summary>
    /// Inicializa los elementos visuales de la interfaz y asigna el estado del problema
    /// </summary>
    /// <param name="type"></param>
    /// <returns></returns>
    private string ConfigureUI(TO_State type)
    {
        string srol = "";
        if (networkRole == TypeRol.PATIENT)
            srol = "Terapeuta";
        else
            srol = "Paciente";

        //Elementos visuales de la interfaz
        canvas.enabled = true;        
        lblCond1.enabled = false; lblTimeCond1.enabled = false;
        lblCond2.enabled = true;
        lblCond3.enabled = true; lblTimeCond3.enabled = true;
        lblCond4.enabled = false;
        checkReady.enabled = false;       

        //Estado actual
        operationState = type;

        return srol;
    }
    #endregion
    //*********************************************************************************//
    //*********************************************************************************//
}
