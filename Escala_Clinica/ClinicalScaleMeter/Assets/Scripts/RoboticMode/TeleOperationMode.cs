using UnityEngine;

using DSP;
using COM_OnRobot_HEX_H;

using System;
using System.Threading;


#region [Enum] Tipos de usuarios
/// <summary>
/// Tipo de rol
/// </summary>
public enum TypeRol
{
    THERAPISTH,
    PATIENT,
    NONE
}
#endregion

/// <summary>
/// Modelo de asistencia de teleoperacion bilateral
/// </summary>
public class TeleOperationMode : RoboticMode
{    

    //************************************Variables************************************//
    //*********************************************************************************//
    #region [Variables] Teleoperacion         
    private TeleOperationController toController;  //Interfaz adicional  
    private TypeRol Rol = TypeRol.NONE; //Tipo de rol
    #endregion    

    #region [Variables] Gestion de fuerzas    
    ForceFields af; //Campos de fuerza
    OnRobotHEX_H onRobot; //Sensor

    //Filtrado
    private LowpassFilterButterworthImplementation filterx;
    private LowpassFilterButterworthImplementation filtery;        

    //Crear clase para la comunicacion online
    Vector2 rcv_vfSensor = Vector2.zero; //De momento variable 0 fuerza recibida
    #endregion    

    #region [Variables] Estado general    
    private Thread goToInitThread; //Condicion de inicialización
    private bool isRunning = false;
    public bool isPosInit = false;
    #endregion
    //*********************************************************************************//
    //*********************************************************************************//



    //***********************************Constructor***********************************//
    //*********************************************************************************//   
    #region [Base] Constructor         
    public TeleOperationMode()
    {
        //Inicializa el modo de control
        Configuration();
    }
    #endregion
    //*********************************************************************************//
    //*********************************************************************************//



    //*************************Inicializacion y configuración**************************//
    //*********************************************************************************//
    #region [Unity Functions] Inicializacion
    private void Start()
    {
        //Crea el controlador para mostra una interfaz de espera y gestionar toda la comunicacion
        var controller = Resources.Load("TeleOperationController");
        var obj = Instantiate(controller, transform) as GameObject;

        toController = obj.GetComponent<TeleOperationController>();
        toController.networkRole = Rol;
    }
    #endregion

    #region [Functions] Configuracion
    public override void Configuration()
    {        
        //Asigna rol
        if (InputManager.Fuerza == 0)
            Rol = TypeRol.THERAPISTH;
        else
            Rol = TypeRol.PATIENT;

        //Inicializa el robot actual
        base.Configuration();

        //Inicializa el sensor de fuerza        
        if (Rol == TypeRol.THERAPISTH) //Inicializa cuando es maestro
        {
            onRobot = new OnRobotHEX_H(); //Sensor de fuerza            

            onRobot.Connect();
            onRobot.Init();
            onRobot.Start(0);
            onRobot.SetSoftwareBias(true);


            //Establece que la interfaz se actualice por los valores que recibe el paciente
            UIManager.sharedInstance.freeUpdating = false;
        }

        //Inicializa los ForceFields       
        af = new ForceFields();

        //Configura el modo de control                        
        GoToInitPosition();

        //Inicializa filtrado
        filterx = new LowpassFilterButterworthImplementation(2, 1, 50); // double cutoffFrequencyHz, int numSections, double Fs
        filtery = new LowpassFilterButterworthImplementation(2, 1, 50); // double cutoffFrequencyHz, int numSections, double Fs
    }
    #endregion
    
    #region [Functions] Ajuste de dispositivos
    /// <summary>
    /// Inicializa el hilo paralelo para establecer los dispositivos
    /// </summary>
    private void GoToInitPosition()
    {
        isRunning = true;

        //Crea hilo de conexion para decidir establecer el rol        
        goToInitThread = new Thread(new ThreadStart(t_goToInit));
        goToInitThread.IsBackground = true;
        goToInitThread.Start();
    }

    /// <summary>
    /// Configura los dispositivos en los modos paciente y terapeuta, y se espera a 
    /// que se coloquen en la posicion inicial
    /// </summary>
    private void t_goToInit()
    {
        // Establece modo de control asistido del Rubidium para simular bloqueo
        InputManager.NivelAsistencia = 2;
        Rubidium.SendLevelAndForce(1000);

        //Lo envia al centro para que ambos esten en el mismo punto
        Rubidium.SendDataPosAssisted(SupportManager.supportInitPos.x, SupportManager.supportInitPos.y);

        // Comprobar conexion entre dispositivos
        float distance = 100.0f;
        float[] dataRb;

        double tic = 0, toc = 0, telapse = 0, tmax = 10;
        double tloop = 1.0 / 10.0; //100Hz
        double duration = 0;

        while (distance > 0.01f && isRunning)
        {
            tic = (DateTime.UtcNow.Subtract(GameManager.sharedInstance.InitDate)).TotalSeconds;

            if (duration > tmax)
            {
                Debug.LogError("Init position not found!");
                return;
            }

            try
            {
                dataRb = Rubidium.GetDataRubidium();
                distance = 0;// Mathf.Sqrt(Mathf.Pow(dataRb[1], 2) + Mathf.Pow(dataRb[2] + 0.47f, 2));
            }
            catch (Exception err)
            {
                Debug.LogWarning("[Init] " + err.ToString());
            }

            toc = (DateTime.UtcNow.Subtract(GameManager.sharedInstance.InitDate)).TotalSeconds;
            telapse = toc - tic;
            if (telapse < tloop)
                Thread.Sleep((int)((tloop - telapse) * 1000.0));

            duration += tloop;
        }

        // Establecer modo de control dependiendo del rol (cliente-maestro)
        if (Rol == TypeRol.PATIENT) //Rol de cliente
        {
            InputManager.NivelAsistencia = 4; // Modo fuerza ya que el cliente puede mover el dispostivo

            Rubidium.ExitModeControl(1000); //Cierra el modo actual e inicia el nuevo
            Rubidium.SendLevelAndForce(1000);
        }
        else if (Rol == TypeRol.THERAPISTH) //Rol de maestro
        {
            InputManager.NivelAsistencia = 22; // Modo de control asisitido modificado para bombardear 
                                               // con señales de control constantes, y simular al cliente
            Rubidium.ExitModeControl(1000); //Cierra el modo actual y activa el nuevo
            Rubidium.SendLevelAndForce(1000);
        }
        isPosInit = true;        
    }
    #endregion
    //*********************************************************************************//
    //*********************************************************************************//



    //*****************************Cierre del dispositivo******************************//
    //*********************************************************************************//
    #region [Public Functions] Cerrar sistema
    public override void Close()
    {
        //Desconecta el dispostivo
        base.Close();

        
    }
    #endregion
    //*********************************************************************************//
    //*********************************************************************************//
           
        

    //**************************Actualizacion del dispositivo**************************//
    //*********************************************************************************//
    #region [Update Functions] Posicion
    /// <summary>
    /// Actualiza la posicion del jugador dependiendo de si tiene el rol 
    /// de paciente o terapetua
    /// </summary>
    /// <param name="values"></param>
    /// <returns></returns>
    public override float[] UpdatePose(float[] values)
    {
        //Si es el paciente se mueve de manera normal. Calcula la posicion por el robot
        if (Rol == TypeRol.PATIENT)
            return base.UpdatePose(values);
        else //Si es el terapeuta, la posicion se calcula a partir de los datos recibidos
        {
            // Convertir datos del paciente a la posicion en pantalla            
            float[] currentPosition = new float[] { toController.rcv_vPosition.x, toController.rcv_vPosition.y };

            // Cambiar posicion del player
            transform.position = new Vector3(currentPosition[0], currentPosition[1], 0);

            //La rotacion es la misma
            float endeff_angle = values[7];
            transform.rotation = Quaternion.Euler(0, 0, Mathf.Rad2Deg * endeff_angle);

            //Duelve posicion del player en pantalla            
            return currentPosition;
        }
    }
    #endregion

    #region [Update Functions] Fuerzas
    /// <summary>
    /// Actualiza las fuerzas del dispostivo dependiendo del rol asignado
    /// </summary>
    /// <param name="pos"></param>
    /// <param name="ang_eef"></param>
    /// <returns></returns>
    public override float[] UpdateForces(float[] pos, float ang_eef)
    {
        //Variables de fuerza        
        float[] bowlCenter = new float[2];
        bowlCenter[0] = 999;
        bowlCenter[1] = 999;

        float[] f_ForceField = new float[2];
        f_ForceField[0] = 999;
        f_ForceField[1] = 999;

        float[] f_sensor = new float[2];
        f_sensor[0] = 999;
        f_sensor[1] = 999;

        float[] f = new float[2];
        f[0] = 999;
        f[1] = 999;


        if (Rol == TypeRol.THERAPISTH) // TERAPEUTA
        {
            // Copy slave position                            
            float[] sp = new float[2] { 0f, 0f };
            sp[0] = toController.rcv_vPosition.x;
            sp[1] = toController.rcv_vPosition.y;

            //float[] spf = new float[2]; // posicion filtrada
            //spf[0] = (float)filterx.compute(sp[0]);
            //spf[1] = (float)filtery.compute(sp[1]);

            Vector2 rp = SupportManager.SetAssistivePosition(new Vector3(sp[0], sp[1], 0)); // posicion robot

            Rubidium.SendSlavePos(new float[] { rp.x, rp.y }); // enviar posicion filtrada

            // Compute Forces from sensor
            //##############################

            Vector2 fs = GetForceSensor(ang_eef);

            f_sensor[0] = fs.x;
            f_sensor[1] = fs.y;

        }
        else if (Rol == TypeRol.PATIENT) // PACIENTE
        {
            // Compute Forces rcv from Master
            //##############################

            f_sensor[0] = rcv_vfSensor.x;
            f_sensor[1] = rcv_vfSensor.y;

            // Compute ForceField
            //##############################
            bowlCenter[0] = pos[0];
            bowlCenter[1] = pos[1];

            f_ForceField = af.ComputeForces(pos[0], pos[1], bowlCenter);

            // Resultant Force
            //##############################
            f[0] = f_sensor[0] + f_ForceField[0];
            f[1] = f_sensor[1] + f_ForceField[1];

            // PLOT
            //Vector2 origen = new Vector2(currentPosition[0], currentPosition[1]);
            //ArrowControl.plot_coordinates(origen, origen + new Vector2(f[0], f[1]));

            Rubidium.SendForces(f, 15); //int damping = 15;
        }


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


    /// <summary>
    /// Recibe datos del sensor de fuerza colocado en el efector final del Rubidium
    /// </summary>
    /// <param name="ang_eef"></param>
    /// <returns></returns>
    public Vector2 GetForceSensor(float ang_eef)
    {
        // Read OnRobot
        float[] f = onRobot.Force;

        f[0] = (float)filterx.compute(f[0]);
        f[1] = (float)filtery.compute(f[1]);

        //Vector2 origen = transform.position;
        float module = Mathf.Sqrt(Mathf.Pow(f[0], 2.0f) + Mathf.Pow(f[1], 2.0f));

        if (module <= 1.0f)
        {
            f[0] = 0.0f;
            f[1] = 0.0f;
            module = 0.0f;
        }
        else
        {
            module = module - 1;
        }

        //if (Mathf.Abs(f[0]) < 1.0f)
        //f[0] = 0.0f;

        //if (Mathf.Abs(f[1]) < 1.0f)
        //f[1] = 0.0f;

        // Compute Force Vector onRobot Sensor

        float ang_forceVector = Mathf.Atan2(f[1], f[0]);
        //ArrowControl.plot_angle(origen, module, (ang_eef + ang_forceVector));

        Vector2 vf;
        if (module >= 30.0f)
        {
            vf.x = Mathf.Cos(ang_eef + ang_forceVector) * 30.0f;
            vf.y = Mathf.Sin(ang_eef + ang_forceVector) * 30.0f;
        }
        else
        {
            vf.x = Mathf.Cos(ang_eef + ang_forceVector) * module;
            vf.y = Mathf.Sin(ang_eef + ang_forceVector) * module;
        }

        // FILTER
        //Vector2 ffilt = new Vector2(); // posicion filtrada
        //ffilt.x = (float)filterx.compute(vf.x);
        //ffilt.y = (float)filtery.compute(vf.y);

        return vf;

    }
    #endregion

    #region [Update Functions] Asistencia
    public override void Assistance(float x, float y)
    {
        //Solo registra las posiciones actuales de origen y destino para almacenarlas en el fichero de registro
        Rubidium.RecordNextOrigin_DesiredPosition(x, y);
    }
    #endregion
    //*********************************************************************************//
    //*********************************************************************************//



    //**********************************Teleoperacion**********************************//
    //*********************************************************************************//
    #region [Send Functions] Datos auxiliares
    /// <summary>
    /// Metodo auxiliar para enviar datos adicionales
    /// </summary>
    public override void Auxiliar(TypeState type)
    {
        //Paciente y terapeuta mandan el estado de sus juegos
        toController.SendAuxiliarData(type);

    }
    #endregion
    //*********************************************************************************//
    //*********************************************************************************//



    //***************************Simulacion del dispositivo****************************//
    //*********************************************************************************//
    #region [Simulation Functions] Simulacion
    /// <summary>
    /// Realiza una simulacion de movimiento del player
    /// </summary>
    public override void Simulation()
    {
        if (Rol == TypeRol.THERAPISTH)
        {
            //Posicion actual
            // Vector3 oldPosition = transform.position;
            // Vector3 networkPosition = new Vector3(toController.rcv_vPosition.x, toController.rcv_vPosition.y, 0);

            //transform.position = Vector3.MoveTowards(oldPosition, networkPosition, Time.deltaTime * 150);
            //  transform.rotation = Quaternion.RotateTowards(transform.rotation, networkRotation, Time.deltaTime * 100);


            //Lag compensation
            double timeToReachGoal = toController.currentPacketTime - toController.lastPacketTime;
            toController.currentTime += Time.fixedDeltaTime;

            //Update remote player
            transform.position = Vector3.Lerp(toController.currentPosition, toController.rcv_vPosition, (float)(toController.currentTime / timeToReachGoal));
            //  transform.rotation = Quaternion.Lerp(rotationAtLastPacket, latestRot, (float)(currentTime / timeToReachGoal));





            // Convertir datos del paciente a la posicion en pantalla            
            // float[] currentPosition = new float[] { toController.rcv_vPosition.x, toController.rcv_vPosition.y };

            // Cambiar posicion del player
            //  transform.position = new Vector3(currentPosition[0], currentPosition[1], 0);           

        }
        else if (Rol == TypeRol.PATIENT)
        {
            //Si es el paciente, se controla normal y envia solo los datos de posicion 
            PlayerController.sharedInstance.MovementWithKeyboard();
        }
    }
    #endregion
    //*********************************************************************************//
    //*********************************************************************************//
}
