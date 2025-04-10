using System;
using UnityEngine;

/// <summary>
/// Modelo de asistencia controlado por fuerzas, junto con tres modos de funcionamiento
/// (tunel, tunel con extremos y assistedasneeded)
/// </summary>
public class ForcesMode : RoboticMode
{

    //************************************Variables************************************//
    //*********************************************************************************//
    #region [Variables] Fuerzas         
    enum TypeForce
    {
        TUNEL,
        TUNEL_EXTREMES,
        ASSISTED_AS_NEEDED,
        NONE
    }
    private TypeForce Rol = TypeForce.NONE;
    ForceFields af; //Campos de fuerza
    #endregion
    //*********************************************************************************//
    //*********************************************************************************//



    //***********************************Constructor***********************************//
    //*********************************************************************************//   
    #region [Base] Constructor 
    public ForcesMode()
    {
        //Asigna tipo de fuerza
        if (InputManager.Fuerza == -1) //Tunel vacio
            Rol = TypeForce.TUNEL;
        else if (InputManager.Fuerza == 0)    //Tunel con extremos            
            Rol = TypeForce.TUNEL_EXTREMES;
        else if (InputManager.Fuerza > 0) //Assisted as needed
            Rol = TypeForce.ASSISTED_AS_NEEDED;

        //Inicializa el robot actual
        base.Configuration();


        //Inicializa los ForceFields       
        af = new ForceFields();



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

        //Registrar datos para almacenarlos en los ficheros de registro
        Rubidium.RecordOrigin_DesiredPosition();

        IsReady = true;

    }
    #endregion
    //*********************************************************************************//
    //*********************************************************************************//



    //**************************Actualizacion del dispositivo**************************//
    //*********************************************************************************//
    #region [Update Functions] Fuerzas
    public override float[] UpdateForces(float[] pos, float ang_eef)
    {
        //Variables de fuerza        
        float[] bowlCenter = new float[2] { 999, 999 };
        float[] f_ForceField = new float[2] { 999, 999 };
        float[] f_sensor = new float[2] { 999, 999 };
        float[] f = new float[2] { 999, 999 };


        // Compute ForceField
        //##############################
        bowlCenter[0] = pos[0];
        bowlCenter[1] = pos[1];
        f = af.ComputeForces(pos[0], pos[1], bowlCenter);

        //Envio de fuerzas al robot
        Rubidium.SendForces(f, 15); //int damping = 15;       


        //Registra los datos calculados de fuerzas en el array de fuerzas
        forces[0] = f_ForceField[0];
        forces[1] = f_ForceField[1];

        float[] tmp = new float[2];
        tmp = af.StartPoint;
        forces[2] = tmp[0];
        forces[3] = tmp[1];

        tmp = af.EndPoint;
        forces[4] = tmp[0];
        forces[5] = tmp[1];

        forces[6] = f_sensor[0];
        forces[7] = f_sensor[1];
        forces[8] = f[0];
        forces[9] = f[1];

        return forces;
    }
    #endregion

    #region [ForceFields] Campos de fuerza
    private void ConfigureBolField()
    {
        // Configurar Campo
        //float dist = 0.01f; // 0.01351f;
        //float force = 1.0f;
        //float F_fric = 0.0f;
        //float cGauss = af.CalcGaussC(dist, force, F_fric);

        float cGauss = InputManager.cGauss;

        af.ConfigBowlForce(cGauss); // cGauss
    }
    private void ConfigureTunnelField()
    {
        // Configurar Campo
        //float dist = 0.07f; // 0.01351f;
        //float force = 100.0f;
        //float F_fric = 0.0f;
        //float cGauss = af.CalcGaussC(dist, force, F_fric);

        float cGauss = InputManager.cGauss;

        af.ConfigTunnelForce(cGauss); // cGauss
        af.ConfigTunnelWithExtremes(cGauss); // cGauss
    }
    private void RemoveForceFields()
    {
        // Stop
        af.RemoveConstrictiveTunnel();
        af.RemoveTunnelWithExtremes();
    }

    public void SetConstrictiveTunnel(float[] origen, float[] destino, float speed)
    {
        RemoveForceFields();

        ConfigureTunnelField();
        ConfigureBolField();

        af.ConfigConstrictiveTunnel(50, speed);

        af.UpdateTrajectory(origen[0], origen[1], destino[0], destino[1]);

        // Iniciar
        af.SetConstrictiveTunnel();
    }
    public void SetTunnelExtremes(float[] origen, float[] destino)
    {
        RemoveForceFields();

        ConfigureTunnelField();
        ConfigureBolField();

        af.UpdateTrajectory(origen[0], origen[1], destino[0], destino[1]);

        // Iniciar
        af.SetTunnelWithExtremes();
    }
    public void SetTunnel(float[] origen, float[] destino)
    {
        RemoveForceFields();

        ConfigureTunnelField();
        //ConfigureBolField();

        af.UpdateTrajectory(origen[0], origen[1], destino[0], destino[1]);

        // Iniciar
        af.SetTunnelForce();
    }
    #endregion

    #region [Update Functions] Asistencia
    /// <summary>
    /// Proporciona asistencia al dispostivo
    /// </summary>
    /// <param name="x"></param>
    /// <param name="y"></param>
    public override void Assistance(float x, float y)
    {
        //La asistencia de este modo significa que se cambiarn los umbrales de fuerza.
        //Cambia los tuneles de fuerza

        try
        {
            //Registra cambios de posisciones
            Rubidium.RecordNextOrigin_DesiredPosition(x, y);

            //Primero obtiene las posiciones origen y deseada
            float[] origen = new float[2] { x, y }; //Punto actual del rubidium
            float[] desired = Rubidium.GetNextPos();


            //Calcula distancia entre los puntos
            Vector2 vModule = new Vector2(desired[0], desired[1]) - new Vector2(x, y);


            //Genera el cambio de fuerzas
            if (vModule.magnitude > 0.005)
            {
                // RobotPlayerController.sharedInstance.setConstrictiveTunnel(dataOrigin, dataDesired);
                //Comprueba los niveles de fuerza recibidos para ajustar el tipo de fuerza
                if (Rol == TypeForce.TUNEL) //Tunel vacio
                    SetTunnel(origen, desired);
                else if (Rol == TypeForce.TUNEL_EXTREMES)    //Tunel con extremos            
                    SetTunnelExtremes(origen, desired);
                else if (Rol == TypeForce.ASSISTED_AS_NEEDED) //Assisted as needed
                {
                    float speed = (InputManager.Amplitud / (1000.0f * 2.0f)) / (InputManager.TimeTotal * 0.5f);
                    SetConstrictiveTunnel(origen, desired, speed);
                }

            }
        }
        catch (Exception)
        { }
    }
    #endregion
    //*********************************************************************************//
    //*********************************************************************************//
}