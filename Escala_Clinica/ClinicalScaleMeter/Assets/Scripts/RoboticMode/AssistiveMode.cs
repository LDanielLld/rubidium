
/// <summary>
/// Modelo de asistencia total.
/// </summary>
public class AssistiveMode : RoboticMode
{

    //***********************************Constructor***********************************//
    //*********************************************************************************//   
    #region [Base] Constructor 
    public AssistiveMode()
    {
        Configuration();
    }
    #endregion
    //*********************************************************************************//
    //*********************************************************************************//



    //*************************Inicializacion y configuración**************************//
    //*********************************************************************************//
    #region [Unity Functions] Configuracion
    public override void Configuration()
    {
        //Inicializa el robot actual
        base.Configuration();

        //Configura el modo de control                
        Rubidium.SendLevelAndForce(200); //Inicializar fuerza y nivel de asistencia
        Rubidium.CurrentPositionRubidium(); //Envia a la posicion actual

        IsReady = true;
    }
    #endregion
    //*********************************************************************************//
    //*********************************************************************************//



    //**************************Actualizacion del dispositivo**************************//
    //*********************************************************************************//
    #region [Update Functions] Asistencia
    public override void Assistance(float x, float y)
    {
        //Envia a la posicion objetivo
        Rubidium.SendDataPosAssisted(x, y);
    }
    #endregion
    //*********************************************************************************//
    //*********************************************************************************//
}

