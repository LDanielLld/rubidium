using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;


/// <summary>
/// Estados de la prueba actual
/// </summary>
public enum PathState
{
    PS_NONE,
    PS_WAITING,    
    PS_INIT_POINT,
    PS_PERFORMING_PATH,
    PS_FINISHING_PATH
}


/// <summary>
/// Clase con la ruta que debe seguir el paciente.
/// </summary>
public class ClinicalPath : MonoBehaviour
{

    public GameObject lineMain;//Linea que se crea


    public StartPoint startP; //Punto inicial
    public StartPoint finishP; //Punto inicial

    public float cTime = 0f;   
    private readonly int TIME_TO_SHOW = 3;

    /// <summary>
    /// Estado actual
    /// </summary>
    public PathState state = PathState.PS_NONE;
   

    // Start is called before the first frame update
    void Start()
    {

        SetState(PathState.PS_INIT_POINT);
    }

    // Update is called once per frame
    Line linea;

    public bool UpdatePath()
    {
        bool isFinish = false;

        if (state == PathState.PS_INIT_POINT)
        {
            if (startP.IsTouching)
                SetState(PathState.PS_WAITING);            
        }
       
        else if (state == PathState.PS_WAITING)
        {
            //Si permanece dentro del centro suma dos segundos, si sale se reinicia
            if (startP.IsTouching)
            {
                cTime += Time.deltaTime;
                UIManager.sharedInstance.SetTimeInstruction(TIME_TO_SHOW, cTime);

                if (cTime >= TIME_TO_SHOW)
                {                    
                    //Cambia al estado de realizacion de trayectoria
                    SetState(PathState.PS_PERFORMING_PATH);
                }   
            }
            else
                SetState(PathState.PS_INIT_POINT);
        }      
        
        else if(state == PathState.PS_PERFORMING_PATH)
        {

            cTime += Time.deltaTime;            

            if (cTime <= InputManager.TimeTotal)
            {
                UIManager.sharedInstance.ProgressBar(cTime);

                if (linea != null)
                {
                    Vector2 pos = PlayerController.sharedInstance.transform.position;
                    linea.DibujarLinea(pos);
                }

                if (finishP.IsTouching)
                {
                    cTime = 0f;
                    UIManager.sharedInstance.ProgressBar(cTime);

                    SetState(PathState.PS_FINISHING_PATH);
                    isFinish = true;
                }
            }
            else
            {
                //Resetea valores para la siguiente iteracion de tiempo
                cTime = 0f;
                UIManager.sharedInstance.ProgressBar(cTime);

                TargetManager.sharedInstance.nbErrors++;
                SetState(PathState.PS_FINISHING_PATH);
                isFinish = true;
            }            
        }
        return isFinish;
    }

    /// <summary>
    /// Realiza acciones dependiendo del estado que se vaya a asignar
    /// </summary>
    /// <param name="newState"></param>
    private void SetState(PathState newState)
    {
        if (newState == PathState.PS_INIT_POINT)
        {
            //Conecta el panel
            UIManager.sharedInstance.instructionPanel.SetActive(true);

            UIManager.sharedInstance.SetIntruction("Dirige el cursor al punto inicial");
            UIManager.sharedInstance.ResetTimeInstruction();
        }
        else if (newState == PathState.PS_WAITING)
        {
            UIManager.sharedInstance.SetIntruction("Espera en el punto inicial");
        }
        else if (newState == PathState.PS_PERFORMING_PATH)
        {            

            UIManager.sharedInstance.SetIntruction("Realiza la trayectoria");
            UIManager.sharedInstance.ResetTimeInstruction();

            //Quita la notificacion a los dos segundos
            StartCoroutine("RemoveNotification");

            //Posicion player
            Transform transform = PlayerController.sharedInstance.transform;

            //Crea la primera linea                        
            GameObject lineaActual = Instantiate(lineMain, transform);
            lineaActual.transform.SetParent(this.transform);
            linea = lineaActual.GetComponent<Line>();

            //Invierte funcionalidad de los puntos
            startP.gameObject.SetActive(false);
            finishP.gameObject.SetActive(true);
        }
        else if (newState == PathState.PS_FINISHING_PATH)
        {
            //Conecta el panel
            UIManager.sharedInstance.instructionPanel.SetActive(true);

            UIManager.sharedInstance.SetIntruction("Trayectoria finalizada!!");
            UIManager.sharedInstance.ResetTimeInstruction();

            finishP.gameObject.SetActive(false);
        }

        cTime = 0f;
        state = newState;
    }

    

    IEnumerator RemoveNotification()
    {
        yield return new WaitForSecondsRealtime(3.0f);
        //Desconecta panel
        UIManager.sharedInstance.instructionPanel.SetActive(false);
    }

    /// <summary>
    /// Cuando esta dentro pinta la linea de verde
    /// </summary>
    /// <param name="collision"></param>
    private void OnTriggerEnter2D(Collider2D collision)
    {
        //Si se esta realizando la trayectoria, se crea una linea nueva con otro color
        if(state == PathState.PS_PERFORMING_PATH)
        {
            //Posicion player
            Transform transform = PlayerController.sharedInstance.transform;
            //Crea la primera linea                        

            //Crea la primera linea                        
            GameObject lineaActual = Instantiate(lineMain, transform);
            lineaActual.transform.SetParent(this.transform);
            linea = lineaActual.GetComponent<Line>();

            linea.GetComponent<LineRenderer>().startColor = Color.green;
            linea.GetComponent<LineRenderer>().endColor = Color.green;
        }
        
    }


    /// <summary>
    /// Cuando sale pinta la linea de rojo
    /// </summary>
    /// <param name="collision"></param>
    private void OnTriggerExit2D(Collider2D collision)
    {
        //Si se esta realizando la trayectoria, se crea una linea nueva con otro color
        if (state == PathState.PS_PERFORMING_PATH)
        {            
            //Posicion player
            Transform transform = PlayerController.sharedInstance.transform;
                             

            //Crea la primera linea                        
            GameObject lineaActual = Instantiate(lineMain, transform);
            lineaActual.transform.SetParent(this.transform);
            linea = lineaActual.GetComponent<Line>();

            linea.GetComponent<LineRenderer>().startColor = Color.red;
            linea.GetComponent<LineRenderer>().endColor = Color.red;
        }
    }
}
