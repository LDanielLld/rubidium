using UnityEngine;
using UnityEngine.UI;


/// <summary>
/// Clase que gestiona la actualización de los elementos visuales de la interfaz gráfica
/// </summary>
public class UIManager : MonoBehaviour
{     
    //Instancia compartida
    public static UIManager sharedInstance;



    //************************************Variables************************************//
    //*********************************************************************************//
    #region [Variables] Elementos visuales
    public Text startLabel; //Etiqueta cuenta inicial
    public Text repLabel; //Etiqueta con las repeticiones     
    public Image mProgressBar; //Barra de progreso para tiempo por objetivos
    public Text instructionLabel; //Texto de instrucciones
    public Text instructionTimerLabel; //Texto de instrucciones
    public GameObject instructionPanel; //Panel de instrucciones

    public ClinicalPath path; //Trayectoria actual
    #endregion

    #region [Variables] Tiempos    
    private float timeTotalBar; //Variable de entrada para indicar tiempo maximo  
    public float timerBar; //Tiempo actual de la barra de tiempo
    public float timerTotal; //Tiempo trascurrido
    private const int SEC_TO_START = 3; //Cuenta atrás para iniciar tarea    
    #endregion

    #region [Variables] Control remoto
    /// <summary>
    /// Flag para indicar si la interfaz se actualiza libre a traves del juego,
    /// o restrictiva a partir del juego remoto
    /// </summary>
    public bool freeUpdating = true;
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
        //Actualizar variables de tiempo y repeticiones
        timerTotal = 0f;
        timerBar = 0f;
        timeTotalBar = InputManager.TimeTotal;
        repLabel.text = InputManager.Repeticiones.ToString();
    }
    #endregion

    #region [Unity Functions] Bucle principal
    void Update()
    {
        if (freeUpdating) //Actualizacion interfaz local y libre
        {
            if (GameManager.sharedInstance.currentGameState == GameState.STARTING)
            {
                //Cuenta atras para empezar el juego
                bool countdown = CountDown();
                if (countdown)
                {
                    GameManager.sharedInstance.SetGameState(GameState.INGAME);
                    timerTotal = 0.0f;
                }

            }
            else if (GameManager.sharedInstance.currentGameState == GameState.INGAME)
            {
                //Actualizar elementos durante el desarrollo de la actividad
               /* if (TargetManager.sharedInstance.taskState == TaskState.PERFORM_PATH)
                {
                    //Si el path actual esta establecido
                    if (path.state == PathState.PS_PERFORMING_PATH)
                    {
                        //Actualizar barra de tiempo para alcanzar el objetivo
                        timerBar += Time.deltaTime;

                        if (timerBar <= timeTotalBar)
                        {
                            ProgressBar();
                        }
                        else
                        {
                            //Resetea valores para la siguiente iteracion de tiempo
                            timerBar = 0f;
                            mProgressBar.fillAmount = 1f;
                            TargetManager.sharedInstance.nbErrors++;
                            TargetManager.sharedInstance.SetGameState(TaskState.WAIT);
                        }
                    }
                }*/

            }
            else if (GameManager.sharedInstance.currentGameState == GameState.FINISH)
                startLabel.text = "Finalizado!!"; //Señala la finalizacion del ejercicio
        }
        else
            UpdateRemoting(); //Actualizacion remota
    }
    #endregion
    //*********************************************************************************//
    //*********************************************************************************//



    //**************************Actualizacion de la interfaz***************************//
    //*********************************************************************************//
    #region [Update Functions] Elementos visuales

    public void SetTimeTotalBar(float timeTotal) => timeTotalBar = timeTotal;

   

    public void SetRepLabel(int value) =>
        repLabel.text = value.ToString() + "/" + TargetManager.sharedInstance.GetNPaths();

    public void SetIntruction(string data) => instructionLabel.text = data;

    
    public void ProgressBar(float timerbar)
    {
        float factorT = 1f / timeTotalBar;
        float offsetT = (1f - timeTotalBar * factorT);
        float sizeBar = (timerbar * factorT) + offsetT;
        mProgressBar.fillAmount = 1f - sizeBar;
    }
    #endregion

    #region [Update Functions] Remoto
    /// <summary>
    /// Actualizar interfaz de manera remota
    /// </summary>
    public void UpdateRemoting()
    {
        if (GameManager.sharedInstance.currentGameState == GameState.STARTING)
        {
            //Cuenta atras para empezar el juego
            bool countdown = CountDown();
        }
        else if (GameManager.sharedInstance.currentGameState == GameState.INGAME)
        {
            //ProgressBar();
        }
    }

    #endregion
    //*********************************************************************************//
    //*********************************************************************************//



    //**************************Funcionalidades adicionales****************************//
    //*********************************************************************************//
    #region [Functions] Control de tiempos
    /// <summary>
    /// Cuenta atras para iniciar la tarea
    /// </summary>
    private bool CountDown()
    {
        bool state = false;

        //Actualizar barra de tiempo para alcanzar el objetivo
        timerTotal += Time.deltaTime;

        //Cuenta atras para empezar el juego
        if (timerTotal < SEC_TO_START)
        {
            string seconds = CalculateStringTime(timerTotal, 0);

            int s = int.Parse(seconds);
            int restTime = SEC_TO_START - s;

            startLabel.text = restTime.ToString();
        }
        else if (timerTotal >= SEC_TO_START && timerTotal < SEC_TO_START + 1)
        {
            startLabel.text = "Empieza!!";
            state = true; //Acaba la cuenta atras
        }

        return state;
    }


    /// <summary>
    /// Tiempo contador de instrucciones
    /// </summary>
    /// <param name="data"></param>
    public void SetTimeInstruction(int timeLimit, float data)
    {
        string seconds = CalculateStringTime(data, 0);

        int s = int.Parse(seconds);
        int restTime = timeLimit - s;         

        instructionTimerLabel.text = restTime.ToString();
    }

    public void ResetTimeInstruction() => instructionTimerLabel.text = "";


    /// <summary>
    /// Reinicia la barra de progreso
    /// </summary>
    public void ResetTimer()
    {
        timerBar = 0f;
        mProgressBar.fillAmount = 1f;
    }

    /// <summary>
    /// Metodo que obtiene un string de tiempo con formato hh:mm a partir
    /// de un valor float de entrada
    /// </summary>
    /// <param name="timeValueLoop"></param>
    /// <param name="type"></param>
    /// <returns></returns>
    public string CalculateStringTime(float timeValueLoop, int type)
    {
        //Calcular string de tiempo para su visualizacion
        System.TimeSpan timespan = System.TimeSpan.FromSeconds(timeValueLoop);
        int hour = timespan.Hours;
        int min = timespan.Minutes;
        int sec = timespan.Seconds;
        string timerString = "";

        if (type == 0)
            timerString = sec.ToString("D1");
        else if (type == 1)
        {
            string mins = ((int)timeValueLoop / 60).ToString("00");
            float segundos = timeValueLoop % 60;
            float segc = Mathf.Ceil(segundos);
            string segs = ((int)segc).ToString("00");

            //string segs = ((int)timerControl % 60).ToString("00");
            if (segs == "60")
            {
                segs = "00";
                mins = (int.Parse(mins) + 1).ToString("00");
            }

            timerString = string.Format("{00}:{01}", mins, segs);
        }

        return timerString;
    }
    #endregion
    //*********************************************************************************//
    //*********************************************************************************//







}
