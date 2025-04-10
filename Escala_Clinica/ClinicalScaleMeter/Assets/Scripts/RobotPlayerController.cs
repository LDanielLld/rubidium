using System;
using UnityEngine;


#region [Enum] Tipos de fuerzas
public enum TypeForces
{
    FORCE_FIEDLS,
    SENSOR,
    ORIGIN,
    DESTINY,
    TOTAL
}
#endregion


/// <summary>
/// Gestor que relaciona los datos del dispositivo robotico externo con
/// su representante en pantalla dentro de la actividad de Unity
/// </summary>
public class RobotPlayerController : MonoBehaviour
{
    //Instancia compartida
    public static RobotPlayerController sharedInstance;



    //************************************Variables************************************//
    //*********************************************************************************//
    #region [Variables] Configuracion de escena
    //Puntos para transformar coordenadas robot-escenario 3D
    public float[] limScene; //Obtiene los limites de los objetivos del juego (Xmin;Xmax;Ymin;Ymax) 

    [Header("Movimiento player")]
    [Tooltip("Puntos que limitan el movimiento del jugador por pantalla" +
        "(Xmin, Xmax, Ymin, Ymax)")]
    public float[] limSceneVisual; //Obtiene los limites visual del juagor en pantalla (Xmin;Xmax;Ymin;Ymax,Zmin;Zmax)   

    [Header("Asistencia")]
    [Tooltip("Puntos de asistencia del robot")]
    public Transform[] SupportedPositions;
    [Tooltip("Punto inicial de asistencia del robot")]
    public Vector2 InitialSupportedPosition;
    [Tooltip("Datos RAW del centro del robot")]
    public float[] CenterRubidium = new float[2] { 0f, -530f }; //Centro del rubidium
    public float[] CenterUnity = new float[2] { 0f, 0f }; //Centro del juego

    //Valores de  asistencia y fuerza
    [Header("Almacenamiento")]
    public bool RecoderData = false;
    
    public float factorX; // Factor de escala en funcion de la amplitud de movimiento del robot
    #endregion

    #region Calculo de fuerzas    
    public RoboticMode modeControl; //Modo de control del dispositivo
    #endregion

    #region [Variables] Almacenamiento de datos
    //Valores de  asistencia y fuerza
    const int num_signals = 31;
    float nivel_asistencia;
    float fuerza;
    float tiempoMax;

    //Datos a registrar del robot
    float working_mode; //Modo de funcionamiento
    float x, y; //Posicion
    float vx, vy;   //Velocidad (m/s)    
    float fx, fy; //Fuerza
    float endeff_angle; //Angulo entre el brazon del usuario y la barra del effector final
    float robot_activated; //Robot habilitado
    float right_arm_sat, left_arm_sat; //Estado del motor derecho e izquierdo 
    float pulsed; //Estado de la pulsacion del boton situado en el efector final

    //Datos adicionales
    float xpr0, ypr0; //Posicion origen       
    float xprF, yprF; //Posicion destino

    float currentX, currentY; //Posicion player en el mundo

    //Datos de fuerza    
    float fx_ff, fy_ff; //Fuerza campos de fuerza
    float ff_origen_x, ff_origen_y;
    float ff_destino_x, ff_destino_y;
    float fx_sensor, fy_sensor; //Fuerza del sensor
    float fx_total, fy_total; //Suma total de fuerzas calculadas (Campos de fuerza + sensor)
    #endregion    
    //*********************************************************************************//
    //*********************************************************************************//



    //*********************Inicializacion, configuración y cierre**********************//
    //*********************************************************************************//
    #region [Unity Function] Inicializacion
    void Awake()
    {
        //Instancia compartida
        if (sharedInstance == null)
            sharedInstance = this;

        //Inicializar valores de asistencia del rubidium
        InputManager.Init();
        float[] tmplimScene = SupportManager.Init(InitialSupportedPosition, SupportedPositions, CenterRubidium, CenterUnity);
        if (tmplimScene.Length == limScene.Length)
            Array.Copy(tmplimScene, limScene, tmplimScene.Length);


        //Datos de asistencia
        nivel_asistencia = InputManager.NivelAsistencia;
        fuerza = InputManager.Fuerza / 100.0f;
        tiempoMax = InputManager.TimeTotal;
        factorX = (limScene[1] - limScene[0]) / InputManager.Amplitud;

        //Crear un modo de control
        modeControl = CreateRoboticMode();

        //Inicializar registro de datos
        if (RecoderData)
            DataManager.Init();       

        xpr0 = -1; //Ausencia de valor objetivo
        ypr0 = -1;
        xprF = -1; //Ausencia de valor objetivo
        yprF = -1;
    }
    #endregion

    #region [Unity Function] Bucle de registro  
    void Update()
    {
/*#if UNITY_EDITOR

        Vector3 position = transform.position;
        float rotation = transform.rotation.eulerAngles.z;
        Rigidbody2D body = GetComponent<Rigidbody2D>();
        Vector3 velocity = body.velocity;

        float[] datos = new float[num_signals] {GameManager.sharedInstance.TimeTotal,
            nivel_asistencia, position.x, position.z, velocity.x, velocity.z, 0, 0, rotation, 0, 0, 0, 0,
            nivel_asistencia, fuerza, tiempoMax, position.x, position.z, (float)TargetManager.sharedInstance.taskState,
            (float)GameManager.sharedInstance.currentGameState, xpr0, ypr0, xprF, yprF, fx_ff, fy_ff, ff_origen_x,
            ff_origen_y, ff_destino_x, ff_destino_y,InputManager.cGauss
        };

#else*/
        //Primero los datos del robot, despues datos adicionales
        /*float[] datos = new float[num_signals] {working_mode, x, y, vx, vy, fx, fy,
            endeff_angle, robot_activated, right_arm_sat, left_arm_sat, pulsed,
            GameManager.sharedInstance.TimeTotal, fuerza, tiempoMax, xpr0, ypr0, (float)TargetManager.sharedInstance.taskState};*/

         float[] datos = new float[num_signals] {GameManager.sharedInstance.TimeTotal, //Tiempo
                working_mode, x, y, vx, vy, fx, fy, endeff_angle, robot_activated, right_arm_sat, left_arm_sat, pulsed, //Robot
                nivel_asistencia, fuerza, tiempoMax, currentX, currentY, (float)TargetManager.sharedInstance.taskState, //Adicionales
                (float)GameManager.sharedInstance.currentGameState, xpr0, ypr0, xprF, yprF, fx_ff, fy_ff, ff_origen_x,
                ff_origen_y, ff_destino_x, ff_destino_y,InputManager.cGauss }; 
//#endif
        DataManager.SaveRubidiumData(datos);
    }
    #endregion

    #region [Unity Function] Bucle de registro  
    void FixedUpdate()
    {        
        //Actualizar posicion del player en funcion del modo de control        
        if (modeControl.IsReady)
        {
            try
            {
                // Modelo de control: Rubidium
                //Obtener datos del robot
                float[] dataRb = modeControl.GetDataRobot();

                //Actualizar jugador en pantalla
                float[] currentPosition = modeControl.UpdatePose(dataRb);               

                // Rotación potenciometro
                endeff_angle = dataRb[7];

                //Actualiza fuerzas que se envian al robot
                float[] forces = modeControl.UpdateForces(new float[] { dataRb[1], dataRb[2]}, endeff_angle);

                //Actualiza valores para guardar
                UpdateDataRubidium(dataRb);
                UpdateDataForces(forces);                             
            }
            catch (Exception err)
            {

                //Si no encuentra rubidium
                Debug.LogWarning("Rubidium not found: " + err.Message);

                modeControl.Simulation();
                
            }
        }
    }
    #endregion

    #region [Unity Function] Cierre del control
    void OnApplicationQuit()
    {
        //Rubidium.Close();
        //Cierra el modelo de control
        modeControl.Close();


        //Cerrar el registro de datos
        DataManager.StopRecordData();

        //Si se ha cerrado por teclado borra el fichero genero
        if (GameManager.sharedInstance.currentGameState != GameState.FINISH)
            DataManager.DeleteFileData();
    }
    #endregion
    //*********************************************************************************//
    //*********************************************************************************//



    //******************************Registro de datos**********************************//
    //*********************************************************************************//
    #region Almacenamiento
    private void UpdateDataRubidium(float[] values)
    {
        //Variables de info del robot
        working_mode = values[0];

        vx = values[3];
        vy = values[4];

        fx = values[5];
        fy = values[6];

        endeff_angle = values[7];
        robot_activated = values[8];

        right_arm_sat = values[9];
        left_arm_sat = values[10];

        pulsed = values[11];

        //Datos adicionales
        //Registro de la posicion donde se dirige el robot
        float[] nextdata = Rubidium.GetNextPos();
        xprF = nextdata[0];
        yprF = nextdata[1];

        //Posicion origen
        float[] origindata = Rubidium.GetOriginPos();
        xpr0 = origindata[0];
        ypr0 = origindata[1];
    }

    private void UpdateDataForces(float[] forces)
    {

        //Variables de fuerzas del robot                                
        fx_ff = forces[0];
        fy_ff = forces[1];

        //Fuerzas de origen
        ff_origen_x = forces[2];
        ff_origen_y = forces[3];

        //Fuerzas de destino
        ff_destino_x = forces[4];
        ff_destino_y = forces[5];

        //Fuerzas del sensor
        fx_sensor = forces[6];
        fy_sensor = forces[7];

        //Fuerzas totales
        fx_total = forces[8];
        fy_total = forces[9];
    }
    #endregion
    //*********************************************************************************//
    //*********************************************************************************//



    //**************************Funcionalidades adicionales****************************//
    //*********************************************************************************//       
    #region [Public Functions] Conversion robot-unity
    /// <summary>
    /// Convierte los datos de posicion recididos por el dispositivo robotico en 
    /// coordenadas visuales de Unity del jugador dentro de la actividad
    /// </summary>
    /// <param name="values"></param>
    /// <returns></returns>
    public float[] ConvertDataRobotToScene(float[] values)
    {
        //Coordenadas del robot
        x = values[1] * 1000f; //Pasa de metros a milimetros
        y = values[2] * 1000f;

        //Recta de primer grado: y = m*x + offset, para calcular el cambio de coordenadas rubidium a unity
        float m = (limScene[1] - limScene[0]) / (float)InputManager.Amplitud; //Factor general

        //Position eje X del robot a la escena  teniendo en cuenta centros
        float offsetx = (limScene[1] + CenterUnity[0]) - m * ((float)InputManager.Amplitud / 2f + CenterRubidium[0]);
        float valuex = x * m + offsetx;

        //Position eje Y del robot a la escena  teniendo en cuenta centros        
        float offsety = (limScene[1] + CenterUnity[1]) - m * ((float)InputManager.Amplitud / 2f + CenterRubidium[1]);
        float valuey = y * m + offsety;
        

        //Ajuste en caso de sobrepasar valores limite visuales
        if (valuex < limSceneVisual[0])
            valuex = limSceneVisual[0];
        else if (valuex > limSceneVisual[1])
            valuex = limSceneVisual[1];

        if (valuey < limSceneVisual[2])
            valuey = limSceneVisual[2];
        else if (valuey > limSceneVisual[3])
            valuey = limSceneVisual[3];

        //Actualizar variable con la posicion del player
        //Solo para esta actividad se cambia el signo ya que esta diseñada al reves
        float[] current = new float[2] { 0f, 0f };
        current[0] = valuex;
        current[1] = valuey;

        return current;
    }
    #endregion

    #region [Public Functions] Fuerzas
    /// <summary>
    /// Construye el modo de funcionamiento del dispositivo
    /// </summary>
    /// <returns></returns>
    private RoboticMode CreateRoboticMode()
    {
        //Crear modo de control del dispostivo y asigna la asistencia correspondiente
        RoboticMode mode = null;

        if (InputManager.NivelAsistencia == 1) //Modo libre
        {
            gameObject.AddComponent<FreeMode>();            
        }
        else if (InputManager.NivelAsistencia == 2) //Modo asistido
            mode = new AssistiveMode();
        else if (InputManager.NivelAsistencia == 4) //Modo fuerza
            mode = new ForcesMode();
        else if (InputManager.NivelAsistencia == 5) //Modo teleoperacion        
        {
            // mode = Instantiate<TeleOperationMode>();
            mode = gameObject.AddComponent<TeleOperationMode>();
        }

        mode = gameObject.GetComponent<RoboticMode>();
        return mode;
    }


    /// <summary>
    /// Devuelve las fuerzas calculadas
    /// </summary>
    /// <param name="type"></param>
    /// <returns></returns>
    public float[] GetForces(TypeForces type)
    {
        float[] backf = new float[2];
        switch(type)
        {
            case TypeForces.SENSOR:
                backf[0] = fx_sensor; backf[1] = fy_sensor;
                break;
            case TypeForces.FORCE_FIEDLS:
                backf[0] = fx_ff; backf[1] = fy_ff;
                break;
            case TypeForces.ORIGIN:
                backf[0] = ff_origen_x; backf[1] = ff_origen_y;
                break;
            case TypeForces.DESTINY:
                backf[0] = ff_destino_x; backf[1] = ff_destino_y;
                break;
            case TypeForces.TOTAL:
                backf[0] = fx_total; backf[1] = fy_total;
                break;
        }
        return backf;
                
    
       
    }
    #endregion
    //*********************************************************************************//
    //*********************************************************************************//
}

