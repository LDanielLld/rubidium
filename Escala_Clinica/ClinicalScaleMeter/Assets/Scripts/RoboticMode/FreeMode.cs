
/// <summary>
/// Modelo de asistencia libre. No genera fuerzas.
/// </summary>
public class FreeMode : RoboticMode
{

    //***********************************Constructor***********************************//
    //*********************************************************************************//   
    #region [Base] Constructor 
    public FreeMode()
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
        Rubidium.RecordOrigin_DesiredPosition(); //Registra solo las posiciones para almacenarlas        

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
        //Solo registra las posiciones actuales de origen y destino para almacenarlas en el fichero de registro
        Rubidium.RecordNextOrigin_DesiredPosition(x, y);
    }
    #endregion
    //*********************************************************************************//
    //*********************************************************************************//
}
