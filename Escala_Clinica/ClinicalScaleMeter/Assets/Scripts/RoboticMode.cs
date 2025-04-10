using System.Collections;
using System.Collections.Generic;
using UnityEngine;


/// <summary>
/// Clase padre del modo de control del robot. Engloba los metodos que cambian 
/// dependiendo del modo control
/// </summary>
public class RoboticMode: MonoBehaviour
{


    //************************************Variables************************************//
    //*********************************************************************************//
    #region [Variables] Estado general   
    public const int nSignals = 10; //Numero de señales de fuerzas, que se pueden calcular
    public float[] forces = new float[nSignals];

    public bool IsReady = false;
    #endregion
    //*********************************************************************************//
    //*********************************************************************************//



    //***********************************Constructor***********************************//
    //*********************************************************************************//   
    #region [Base] Constructor         
    public RoboticMode(){}
    #endregion
    //*********************************************************************************//
    //*********************************************************************************//



    //*************************Inicializacion y configuración**************************//
    //*********************************************************************************//
    #region [Unity Functions] Configuración
    public virtual void Configuration()
    {
        // Inicializa conexion con el robot Rubidium       
        Rubidium.InitCOM(); //UDPs      
        Rubidium.InitRecvThread(); //Hilo de recepcion      
    }
    #endregion
    //*********************************************************************************//
    //*********************************************************************************//



    //*****************************Cierre del dispositivo******************************//
    //*********************************************************************************//
    #region [Public Functions] Cerrar sistema
    /// <summary>
    /// Desconecta el dispositivo
    /// </summary>
    public virtual void Close() { Rubidium.Close(); }
    #endregion
    //*********************************************************************************//
    //*********************************************************************************//



    //**************************Actualizacion del dispositivo**************************//
    //*********************************************************************************//
    #region [Update Functions] Posicion
    /// <summary>
    /// Actualiza la posicion del jugador en pantalla
    /// </summary>
    /// <param name="values"></param>
    /// <returns></returns>
    public virtual float[] UpdatePose(float[] values)
    {
        // RUBIDIUM
        //###############################                
        float[] dataRb = Rubidium.GetDataRubidium();        

        // Convertir datos del robot a la posicion en pantalla
        float[] currentPosition = RobotPlayerController.sharedInstance.ConvertDataRobotToScene(dataRb);

        // Cambiar posicion del player
        transform.position = new Vector3(currentPosition[0], currentPosition[1], 0);        

        // Rotación potenciometro
        float endeff_angle = dataRb[7];
        transform.rotation = Quaternion.Euler(0, 0, Mathf.Rad2Deg * endeff_angle);

        //Duelve posicion del player en pantalla         
        return currentPosition;
    }
    #endregion

    #region [Update Functions] Fuerzas
    /// <summary>
    /// Actualiza las fuerzas del dispositivo
    /// </summary>
    /// <param name="pos"></param>
    /// <param name="ang_eef"></param>
    /// <returns></returns>
    public virtual float[] UpdateForces(float[] pos, float ang_eef){ return forces; }
    #endregion

    #region [Public Function] Datos
    public virtual float[] GetDataRobot()
    {
        // Actualiza posicion del robot
        float[] dataRb = Rubidium.GetDataRubidium();
        return dataRb;
    }
    #endregion

    #region [Public Function] Asistencia
    /// <summary>
    /// Metodo para proporcionar asistencia en caso de ser necesario
    /// </summary>
    public virtual void Assistance(float x, float y) { }
    #endregion
    
    #region [Public Functions] Gestion de errores
    /// <summary>
    /// Metodo para eliminar los posibles errores del dispositivo
    /// </summary>
    /// <returns></returns>
    public virtual bool ClearProblem()
    {
        //Resetear punto actual y envio de nuevo a cero

        //Cambiar esto para que no dependa solo de la clase Rubidium, 
        //que se encargue el RoboticMode            
        Rubidium.RequestError(1500);
        Rubidium.ClearError(1500);
        Rubidium.ResetModeControl(1500);
        Rubidium.CurrentPositionRubidium();

        return !Rubidium.HasError();
    }
    #endregion
    //*********************************************************************************//
    //*********************************************************************************//



    //*********************************Datos auxiliares********************************//
    //*********************************************************************************//
    #region [Send Functions] Datos auxiliares
    /// <summary>
    /// Metodo para añadir procesos adicionales. Cada RoboticMode puede tener el suyo propio
    /// </summary>
    public virtual void Auxiliar(TypeState type) { }
    #endregion
    //*********************************************************************************//
    //*********************************************************************************//
    


    //***************************Simulacion del dispositivo****************************//
    //*********************************************************************************//
    #region [Simulation Functions] Simulacion
    /// <summary>
    /// Simulacion del robot y transmision de datos
    /// </summary>
    public virtual void Simulation()
    {
        PlayerController.sharedInstance.MovementWithKeyboard();
    }
    #endregion
    //*********************************************************************************//
    //*********************************************************************************//

}