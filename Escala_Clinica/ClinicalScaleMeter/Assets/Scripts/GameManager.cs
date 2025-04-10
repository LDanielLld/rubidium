using System;
using System.Collections;
using System.Collections.Generic;
using System.Numerics;
using UnityEngine;
using UnityEngine.UI;

#region [Enum] Estados generales

//Posibles estados generales del juego
public enum GameState
{
    STARTING, //Al inicio
    INGAME, //Debe esquivar objetos    
    FINISH, //Finaliza todos los obejtivos       
    SAFETYMODE, //Se produce un error en el dispositivo robotico
    SAFETYMODE_REMOTE, //Estado de pausa debido a que el equipo remoto tiene un warning
    CANCEL, //Estado para indicar que se ha cancelado la actividad (escape)
    NONE
}
#endregion


/// <summary>
/// Clase central de la actividad que gestiona el estado general de la actividad. Indica cuando
/// se está iniciando, cuando se tiene que realizar el ejercicio, cuando entra en modo warning y
/// cuando finaliza. General para todos los juegos
/// </summary>
public class GameManager : MonoBehaviour
{
    //Instancia compartida
    public static GameManager sharedInstance = null;


    //************************************Variables************************************//
    //*********************************************************************************//
    #region [Variables] Escena
    //Variable para saber que estado del juego nos encontramos
    public GameState currentGameState = GameState.NONE;

    //Modo debug
    public bool ModeDebug = false;
    #endregion

    #region [Variables] Elementos visuales 
    public Canvas startCanvas, gameCanvas, warningCanvas; //referencia del canvas
    #endregion   

    #region [Variables] Tiempos
    //Tiempo
    public float TimeTotal = 0f;

    //Fecha
    public DateTime InitDate;
    #endregion
    //*********************************************************************************//
    //*********************************************************************************//



    //***********************Inicializacion y bucle de control*************************//
    //*********************************************************************************//
    #region [Unity Functions] Inicializacion      

    private void Awake()
    {
        if (sharedInstance == null)
        {
            sharedInstance = this;
        }


        //Establecer fecha de inicio del ejercicio
        DateTime datetime = DateTime.Now;
        int day = 1;
        int mouth;
        int year;
        if (datetime.Month != 1)
        {
            mouth = datetime.Month - 1;
            year = datetime.Year;
        }
        else
        {
            mouth = 12;
            year = datetime.Year - 1;
        }

        InitDate = new System.DateTime(year, mouth, day);
    }

    private void Start()
    {
        //Desconecta todas las interfaces
        startCanvas.enabled = false;
        gameCanvas.enabled = false;
        warningCanvas.enabled = false;

        //Estado inicial del juego
        if(InputManager.NivelAsistencia != 5)
            SetGameState(GameState.STARTING);
        else
            SetGameState(currentGameState);      
    }
    #endregion

    #region [Functions] Control de estados
    void Update()
    {
        //Tiempo total del juego        
        TimeTotal += Time.deltaTime;
        CheckError();


        if (Input.GetKey("escape"))
        {
            //Si se cierrra el programa anular el envio de resultados
            ClickExit();
        }
        else if (Input.GetKeyDown(KeyCode.F12))
        {
            //Conseguir un nombre único
            DateTime dateTime = DateTime.Now;

            string name = Application.productName + "_screenshot_" + dateTime.Day + "_" + dateTime.Month
                + "_" + dateTime.Year + "_" + dateTime.Hour + "_" + dateTime.Minute + "_" + dateTime.Second;
            ScreenCapture.CaptureScreenshot(name + ".png");
        }
        else if (Input.GetKeyDown(KeyCode.Tab))
            ModeDebug = !ModeDebug;
        else if (Input.GetKey(KeyCode.Space) && currentGameState == GameState.SAFETYMODE)
        {
            //Solucionar el problema del robot
            bool isFixed = 
                RobotPlayerController.sharedInstance.modeControl.ClearProblem();  

            //Si esta solucionado arregla la interfaz
            if (isFixed)
            {
                //Vuelve a activar la actividad      
                startCanvas.enabled = false;
                gameCanvas.enabled = true;
                warningCanvas.enabled = false;
                currentGameState = GameState.INGAME; //Asignar el estado del juego actual   
                TargetManager.sharedInstance.ReStart(); //Reinicia el juego      

                //Envia el gameState en caso de ser necesario        
                RobotPlayerController.sharedInstance.modeControl.Auxiliar(TypeState.GAME_STATE);
            }
        }        
    }

    /// <summary>
    /// Metodo encargado de cambiar el estado del juego
    /// </summary>
    public void SetGameState(GameState newGameState)
    {
        if (newGameState == GameState.STARTING)
        {
            //Prepara escena para la cuenta atras antes de empezar    
            startCanvas.enabled = true;
            gameCanvas.enabled = false;
            warningCanvas.enabled = false;
        }
        if (newGameState == GameState.INGAME)
        {
            //Prepara escena para mostrar tiempo y puntuación
            gameCanvas.enabled = true;
            startCanvas.enabled = false;
            warningCanvas.enabled = false;

            //Inicia tarea
            TargetManager.sharedInstance.Init();
        }
        else if (newGameState == GameState.FINISH)
        {
            //Prepara escena para la cuenta atras antes de empezar    
            startCanvas.enabled = true;
            gameCanvas.enabled = true;

            //Finaliza tarea
            TargetManager.sharedInstance.Finish();

            //Lanzar corrutina de dos segundos de espera              
            StartCoroutine("FinishGame");
        }
        else if (newGameState == GameState.SAFETYMODE)
        {
            //Prepara escena mostrar el error
            startCanvas.enabled = false;
            gameCanvas.enabled = false;
            warningCanvas.enabled = true;

            //Bloquea la tarea
            TargetManager.sharedInstance.ReSet();
        }
        this.currentGameState = newGameState; //Asignar el estado del juego actual al que nos ha llegado

        //Envia el gameState en caso de ser necesario        
        RobotPlayerController.sharedInstance.modeControl.Auxiliar(TypeState.GAME_STATE);


    }
    #endregion
    //*********************************************************************************//
    //*********************************************************************************//



    //***************************Funcionalidades adcionales****************************//
    //*********************************************************************************//
    #region [Functions] Comprobación de información

    private void OnGUI()
    {
       

        if (ModeDebug)
        {
            var resor = Resources.Load("Prefabs/TeleOperationC");

            string path = "Prefabs/TeleOperationC";
            print(path);
            var animatedWordPrefab = Resources.Load(path) as GameObject;
            var reso2r = Resources.Load("Assets/Prefabs/TeleOperationC");

            GameObject prefabToInstantiate = Resources.Load<GameObject>("Prefabs/" + "CanvasTeleOp");

            Rect rectObj = new Rect(40, 250, 350, 600);
            GUIStyle style = new GUIStyle();
            style.alignment = TextAnchor.UpperLeft;

            String txt;

            // GAME MODE
            if (InputManager.NivelAsistencia == 1 )//GameMode.singleplayer)
             {
                 txt = "# [Single Player] \n";
             }
             else if (InputManager.NivelAsistencia == 4)//GameMode.localMS)
            {
                 txt = "# [localMS] \n";
             }
             else if (InputManager.NivelAsistencia == 2)// == GameMode.totalassistive)
            {
                 txt = "# [Total Assistive] \n";
             }
            else if (InputManager.NivelAsistencia == 3)// == GameMode.assistive)
            {
                txt = "# [Assistive] \n";
            }
            else
             {
                 txt = "[ ... ] \n";
             }

            //Numero de argumentos de entrada
            txt = "Numero entradas: " + InputManager.GetLength() + "\n";

            // GAME STATE
            txt = txt + "currentGameState: " + currentGameState + "\n";

             

            //
            txt = txt + "cGauss: " + InputManager.cGauss.ToString() + "\n";

            txt = txt + "TimerTotal: " + UIManager.sharedInstance.timerTotal + "\n";

            GUI.Box(rectObj, txt, style);
        }
    }
   
    private void CheckError()
    {
        //Comprueba si entra en modo seguro para bloquear el ejercicio        

#if !UNITY_EDITOR
        if (Rubidium.HasError() && currentGameState != GameState.SAFETYMODE)
        //if (Input.GetKeyDown(KeyCode.M) && currentGameState != GameState.SAFETYMODE)
#else
        if (Rubidium.HasError() && currentGameState != GameState.SAFETYMODE)
            //if (Input.GetKeyDown(KeyCode.M) && currentGameState != GameState.SAFETYMODE)
#endif
        {
            SetGameState(GameState.SAFETYMODE);
        }
    }

    #endregion
    //*********************************************************************************//
    //*********************************************************************************//



    //*************************Cierre de la actividad**********************************//
    //*********************************************************************************//
    #region [Functions] Gestion de cierre
    IEnumerator FinishGame()
    {
        yield return new WaitForSecondsRealtime(3.0f);
        ExitGame();
    }

    /// <summary>
    /// Pulsar boton de salir
    /// </summary>
    public void ClickExit()
    {
        //Si se cierrra el programa anular el envio de resultados
        //Enviar datos de las valoraciones            
        SenderFinalData.InitCOM();
        SenderFinalData.ResetData();
        SenderFinalData.SendData();
        SenderFinalData.Close();

        //Estado de salir 
        SetGameState(GameState.CANCEL);

        ExitGame();
    }

    /// <summary>
    /// Metodo para finalizar la aplicacion
    /// </summary>
    public void ExitGame()
    {
        //Si existe error, cerrarlo antes de acabar con el programa 
        if (Rubidium.HasError() && currentGameState == GameState.SAFETYMODE)
        {

            Rubidium.RequestError(1500);
            Rubidium.ClearError(1500);
            if (currentGameState != GameState.CANCEL)
                currentGameState = GameState.FINISH;

        }

#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false; //No para moviles
#else
        Application.Quit();
#endif        
    }
    #endregion    
    //*********************************************************************************//
    //*********************************************************************************//
}
