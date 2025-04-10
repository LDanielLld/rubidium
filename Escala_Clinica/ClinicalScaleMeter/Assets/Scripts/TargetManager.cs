using UnityEngine;
using System;


#region [Enum] Estados de la tarea
/// <summary>
/// Indica los tipos de estados de la actividad
/// </summary>
public enum TypeState
{
    GAME_STATE,
    TASK_STATE,
    NONE
}

/// <summary>
/// Posibles estados del juego
/// </summary>
public enum TaskState
{
    INIT_PATH, //Va al punto de inicio
    WAITING_FOR_PATH, //Espera a llegar para iniciar el trial
    PERFORM_PATH, //Va a la caja
    WAIT, //Espera dos segundos para ir al objetivo
    FINISH_PATH, //Finaliza la trayectoria
    CHANGE_PATH, //Cambia la trayectoria
    NONE //Sin asignar
}
#endregion


/// <summary>
/// Clase que controla el estado actual de la tarea. Gestiona toda la interaccion visual y fisica de
/// todos los elementos que forman parte de la actividad. Solo trata los objetivos.
/// </summary>
public class TargetManager : MonoBehaviour
{
    //Instancia compartida
    public static TargetManager sharedInstance = null;


    //************************************Variables************************************//
    //*********************************************************************************//
    #region [Variables] Estado de la actividad
    public TaskState taskState = TaskState.NONE; // Variable para saber que estado del juego nos encontramos

    //Estado de la actividad
    public int nbErrors = 0;
    private int indexPath = 0;
    private int currentPosition;
    #endregion

    #region [Variables] Elementos  
    //Lista de trayectorias a realizar
    public GameObject[] listClinicalPathsOriginal;

    private GameObject[] listClinicalPaths;

    //Trayectoria actual
    private ClinicalPath cClinicalPath;
    #endregion

    #region [Variables] Tiempos
 
   //Tiempo de espera    
    private float timeWaiting = 0f;
    #endregion






    private void Awake()
    {
        if (sharedInstance == null)
            sharedInstance = this;
    }

   

    
    
    
  

    // Incialización
    void Start()
    {
        //Inicializa las trayectorias a realizar. Clona dos veces el array origial
        int n_rep = 2;
        listClinicalPaths = new GameObject[n_rep*listClinicalPathsOriginal.Length];
        int index_copy = 0;
        for(int i=0; i< n_rep; i++)
        {
            Array.Copy(listClinicalPathsOriginal, 0, listClinicalPaths, index_copy, listClinicalPathsOriginal.Length);
            index_copy += listClinicalPathsOriginal.Length;
        }
        UIManager.sharedInstance.SetRepLabel(0);

        InitGame();
    }

    public void InitGame()
    {  
        //Inicializar paths
        indexPath = -1;
        SetGameState(TaskState.CHANGE_PATH);
    }


    public int GetNPaths() => listClinicalPaths.Length;
   

    // Update is called once per frame
    void Update()
    {
        if (GameManager.sharedInstance.currentGameState == GameState.INGAME)
        {
            //Estados de la tarea. Existen 3 posibilidades
            if (taskState == TaskState.PERFORM_PATH)
            {
                //Actualiza registro
                bool isFinish = cClinicalPath.UpdatePath();

                //Al finalizar cambia el objetivo
                if (isFinish)
                {
                    //Incrementa contador de repeticiones
                    UIManager.sharedInstance.SetRepLabel(indexPath+1);

                    SetGameState(TaskState.WAIT);
                }
            }
            else if(taskState == TaskState.WAITING_FOR_PATH)
            {
                //Actualiza registro
                bool isFinish = cClinicalPath.UpdatePath();

                if(cClinicalPath.state == PathState.PS_PERFORMING_PATH)
                {
                    SetGameState(TaskState.PERFORM_PATH);
                }

            }
            else if (taskState == TaskState.WAIT)
            {
                timeWaiting += Time.deltaTime;
                if (timeWaiting >= 2f)
                {
                    //Esperar dos segundos para mandar                  
                    Destroy(cClinicalPath.gameObject);
                    SetGameState(TaskState.CHANGE_PATH);
                    SetGameState(TaskState.WAITING_FOR_PATH);
                }
            }

            /* else if (taskState == TaskState.GOTOBOX)
             {
                 //Comprueba distancia con el objetivo
                 bool isInBox = roulette.IsInsideTarget();

                 if (isInBox)
                     SetGameState(TaskState.GOTOZERO); //Cambia de estado a ir a centro con objetivo cumplido                    
             }
             else if (taskState == TaskState.WAITOBOX)
             {
                 timeWaiting += Time.deltaTime;
                 if (timeWaiting >= 2f)
                 {
                     //Esperar dos segundos para mandar objetivo                      
                     SetGameState(TaskState.GOTOBOX);                    
                 }
             }
             else if (taskState == TaskState.WAITOZERO)
             {
                 timeWaiting += Time.deltaTime;
                 if (timeWaiting >= 2f)
                 {
                     //Esperar dos segundos para mandar centro
                     SetGameState(TaskState.GOTOZERO);                    
                 }
             }*/
        }      
    }



    /// <summary>
    /// Metodo encargado de cambiar el estado del juego
    /// </summary>
    /// <param name="newGameState">GameState enum variable</param>
    public void SetGameState(TaskState newGameState)
    {

        if (newGameState == TaskState.CHANGE_PATH)
        {
            //Cambia la rutina
            SetNextPath();

           /* timeWaiting = 0f;
            roulette.ChangeTargetColour(true, false, currentPosition - 1); //Centro

            //Reiniciar repeticiones
            UIManager.sharedInstance.SetRepLabel(iterationRand);

            if (iterationRand == InputManager.Repeticiones - 1)
                timeWaiting = 2f;*/
        }
        else if (newGameState == TaskState.WAIT)
        {
            //Lanzar corrutina de dos segundos de espera    
            timeWaiting = 0f;
        }

       /* else if (newGameState == TaskState.WAITOZERO)
        {
            //Lanzar corrutina de dos segundos de espera    
            timeWaiting = 0f;
            roulette.ChangeTargetColour(false, false, currentPosition - 1); //Centro            
        }
        else if (newGameState == TaskState.GOTOBOX)
        {            
            SetNextTarget();
        }
        else if (newGameState == TaskState.GOTOZERO)
        {
            //Resetea valores de interaccion
            roulette.ChangeTargetColour(true, true, currentPosition - 1); //Centro
            roulette.ChangeTargetColour(false, false, currentPosition - 1); //Objetivo

            //Reiniciar timer
            UIManager.sharedInstance.ResetTimer();

            //Enviar posicion robot cero
            //Rubidium.SendDataPos(SupportManager.supportInitPos.x, SupportManager.supportInitPos.y);

            //Asistencia para la posicion cero
            RobotPlayerController.sharedInstance.modeControl.Assistance(
                SupportManager.supportInitPos.x, SupportManager.supportInitPos.y);
        }   */     
        taskState = newGameState; //Asignar el estado del juego actual al que nos ha llegado

        

    }
    

    public void ResetGame()
    {
        indexPath = 0;
      //  currentPosition = roulette.Target(iterationRand);
    }

    private void SetNextPath()
    {
        indexPath++;
        //Establecer la siguiente iteracion de objetivo
        //Primero comprueba si quedan mas trayectorias por realizar
        if (indexPath < listClinicalPaths.Length)
        {
            //Crea el path actual
            GameObject cPath = Instantiate(listClinicalPaths[indexPath]);

            //Registra el path actual
            cClinicalPath = cPath.GetComponent<ClinicalPath>();            
        }
        else
        {
            //Finaliza la valoracion           
            GameManager.sharedInstance.SetGameState(GameState.FINISH);
        }

       
    }


    /// <summary>
    /// Metodo auxiliar para modificar los objetivos desde fuera (caso teleoperacion)
    /// </summary>
    public void ChangeTargets(byte[] data)
    {
       
    }

    /// <summary>
    /// Gestion externa de las repeticiones actuales
    /// </summary>
    /// <returns></returns>
    public float GetRemainRep() => indexPath;
    public void SetRemainRep(int value) => indexPath = value;

    //*****************************************Gestion con GameManager****************************************//
    //********************************************************************************************************//
    public void Init()
    {        
       SetGameState(TaskState.WAITING_FOR_PATH);
    }

    public void ReSet()
    {
      /*  try
        {
            //Reiniciar colores de los elementos de interaccion
            if (taskState == TaskState.GOTOZERO)                            
                roulette.ChangeTargetColour(true, false, currentPosition - 1); //Quitar señalizacion del centro              
            else if(taskState == TaskState.GOTOBOX)
                roulette.ChangeTargetColour(false, false, currentPosition - 1); //Quitar señalizacion delObjetivo 

            
        }
        catch (Exception)
        { }*/
    }

    public void ReStart()
    {

     /*   if (taskState != TaskState.WAITOBOX)
        {
            //Reiniciar objetivo actual
            if (iterationRand > -1)
            {
                iterationRand--;
            }
            
        }
        SetGameState(TaskState.GOTOZERO); //Cambia de estado para esperar dos segundos
       */


       
    }

    public void Finish()
    {
        //En caso contrario se cierra el programa
        //Enviar datos al REVIRE en caso de ser necesario
        SenderFinalData.AddValue(GameManager.sharedInstance.TimeTotal);
        SenderFinalData.AddValue(InputManager.Repeticiones - nbErrors); //Numero de aciertos
        SenderFinalData.AddValue(nbErrors); //Numero de fallos
                                                //Enviar datos de las valoraciones            
        SenderFinalData.InitCOM();
        SenderFinalData.SendData();
        SenderFinalData.Close();
    }
    //********************************************************************************************************//
    //********************************************************************************************************//
}