using UnityEngine;

using System;
using System.Text;
using System.Net;
using System.Net.Sockets;
using System.Threading;


/// <summary>
/// Clase envoltorio del dispositivo robotico que controla la inicializacion y la gestion
/// de toda la informacion transmitida por UDP
/// </summary>
public class Rubidium : MonoBehaviour
{

    //************************************Variables************************************//
    //*********************************************************************************//
    #region [Variables] General
    private static bool isCOMInit = false;
    private static bool isRubidiumInit = false;
    private static bool isCOMRecvInit = false;

    private static float[] dataRubidium;
    private static float[] dataDesired = new float[2] { 0, 0 };
    private static float[] dataOrigin = new float[2] { 0, 0 };

    private static string currentTrial = ""; //Trama actual de recpecion
    private static string error = "No error!!";//Errores en la programacion
    private static string statusInitRubidium = "Waiting initialization!!"; //Inicializacion string
    private static string processError = ""; //Errores enviados por el robot
    private static bool hasError = false;

    private static int workingMode = -1; //Modo de control actual       
    #endregion

    #region [Variables] Network
        public static string localIP = "172.16.0.10", remoteIP = "172.16.0.1";
        public static int localPort = 20222, remotePort = 10111;
        private static UdpClient udpRead, udpSend;
        private static IPEndPoint remoteEndPoint, localEndPoint;
        private static Thread receiveThread;
    #endregion
    //*********************************************************************************//
    //*********************************************************************************//



    //**********************Inicialización de la comunicacion**************************//
    //*********************************************************************************//
    #region [Public Functions] Gestion de la comunicación
    public static string InitCOM()
    {

        string state = "";
        try
        {
            //Enviar
            remoteEndPoint = new IPEndPoint(IPAddress.Parse(remoteIP), remotePort);
            udpSend = new UdpClient(remotePort);

            //Recibir
            localEndPoint = new IPEndPoint(IPAddress.Parse(localIP), localPort);
            udpRead = new UdpClient(localPort);
            udpRead.Client.ReceiveTimeout = 5000;

            state = "UDP initialized!!";
            isCOMInit = true;
        }
        catch (Exception e)
        {
            state = e.Message.ToString();
        }
        return state;
    }

    public static string InitRecvThread()
    {
        string state = "";

        try
        {
            //Hilo para recibir datos
            receiveThread = new Thread(new ThreadStart(ReceiveData));
            receiveThread.IsBackground = true;
            receiveThread.Start();
            state = "Received thread initialized!!";
            isCOMRecvInit = true;
        }
        catch (Exception e)
        {
            state = e.Message.ToString();
        }

        return state;
    }
    #endregion

    #region [Public Function] Cierre
    public static string Close()
    {
        string state = "";
        try
        {
            //Cerrar el tipo de modo
            ExitModeControl(100);
            isRubidiumInit = false;

            //Eliminar puerto COM
            if (udpSend != null)
            {
                udpSend.Close();
                udpSend = null;
            }

            if (udpRead != null)
            {
                udpRead.Close();
                udpRead = null;

                //Borrar hilo de recepcion
                receiveThread.Abort();
                receiveThread = null;
                isCOMRecvInit = false;
            }
            isCOMInit = false;

            state = "System disconnected!!";
        }
        catch (Exception e)
        {
            state = e.Message.ToString();
        }
        return state;
    }
    #endregion
    //*********************************************************************************//
    //*********************************************************************************//



    //******************Obtener/Establecer datos del dispositivo***********************//
    //*********************************************************************************//
    #region [Public Functions] Estado general del robot
    public static bool IsCOMInit() => isCOMInit;

    public static bool IsRecvCOMInit() => isCOMRecvInit;

    public static bool IsRubidiumInit() => isRubidiumInit;

    public static float[] GetDataRubidium() => dataRubidium;

    public static string GetProcessError() => processError;

    public static string GetCurrentTrial() => currentTrial;

    public static string GetStatusInit() => statusInitRubidium;

    public static float[] GetNextPos()
    {
        return (float[])dataDesired.Clone();
    }

    public static float[] GetOriginPos()
    {
        return (float[])dataOrigin.Clone();
    }

    public static bool GetSafety()
    {
        try
        {
            if (dataRubidium[0] > 0.0f)
                return true;
            else
                return false;
        }
        catch (Exception err)
        {
            error = err.Message.ToString();
            return false;
        }
    }
    #endregion
    //*********************************************************************************//
    //*********************************************************************************//



    //********************************Enviar datos**************************************//
    //*********************************************************************************//
    #region [Public Functions] Envio de datos - Control

    public static void ExitModeControl(int timewait)
    {
        //Salir del modo de control
        string msg = "";
        if (workingMode >= 2 && workingMode <= 4)
        {
            msg = "-1/0/0/";
        }
        else
        {
            msg = "-1/";
        }

        SendData(msg);
        Thread.Sleep(timewait);
    }
    

    public static void CurrentPositionRubidium()
    {
        try
        {
            //Mandar posicion actual obtenida desde el hilo paralelo                
            string cx_string = dataRubidium[1].ToString();
            string cy_string = dataRubidium[2].ToString();

            //Registrar datos para almacenarlos en los ficheros de registro
            RecordOrigin_DesiredPosition();

            //Instruccion de envio al rubidium
            string s_data = workingMode + "/" + cx_string + "/" + cy_string;
            SendData(s_data);            
        }
        catch (Exception)
        { }
    }

    public static void RecordOrigin_DesiredPosition()
    {
        try
        {
            dataOrigin[0] = dataRubidium[1];
            dataOrigin[1] = dataRubidium[2];

            dataDesired[0] = dataRubidium[1];
            dataDesired[1] = dataRubidium[2];
        }
        catch(Exception)
        { }
    }

    public static void RecordNextOrigin_DesiredPosition(float x, float y)
    {
        dataOrigin[0] = dataDesired[0];
        dataOrigin[1] = dataDesired[1];

        dataDesired[0] = x;
        dataDesired[1] = y;
    }


    /// <summary>
    /// Enviar robot a la posicion actual. Asistido total
    /// </summary>
    /// <param name="x"></param>
    /// <param name="y"></param>
    public static void SendDataPosAssisted(float x, float y)
    {
        //Convierte datos recibidos a un string
        string cx_string = x.ToString();
        string cy_string = y.ToString();

        //Registrar datos para almacenarlos en los ficheros de registro
        RecordNextOrigin_DesiredPosition(x, y);

        //Instruccion de envio al rubidium
        string s_data = workingMode + "/" + cx_string + "/" + cy_string;
        SendData(s_data);        
    }

   

    /// <summary>
    /// Envia posición (x,y) en el espacio del robot al Rubidium. 
    /// La función no realiza la conversión de las coordenadas, tienen 
    /// que introducirse en el espacio de trabajo del robot. El Rubidium debe estar configurado en modo control en posición.
    /// </summary>
    /// <param name="rp">Posición rp[0]=x e rp[1]=y objetivo</param>
    public static void SendSlavePos(float[] rp)
    {
        if (workingMode == 2 || workingMode == 22) // Position Control
        {
            string rpx_string = Decimal.Round(((decimal)rp[0]), 3).ToString();
            string rpy_string = Decimal.Round(((decimal)rp[1]), 3).ToString();

            string s_data = workingMode + "/" + rpx_string + "/" + rpy_string + "/";

            //Debug.Log(s_data);

            SendData(s_data);
        }

    }

    /// <summary>
    /// Envia las fuerzas a aplicar en el efector final del Rubidium. El Rubidium debe estar configurado en modo control fuerza. 
    /// </summary>
    /// <param name="f">Fuerzas f[0]=x e f[1]=y objetivo</param>
    /// <param name="damping">Efecto Damping del robot</param>
    public static void SendForces(float[] f, int damping)
    {
        if (workingMode == 4) // Force Control
        {
            string fx_string = Decimal.Round(((decimal)f[0]), 3).ToString();
            string fy_string = Decimal.Round(((decimal)f[1]), 3).ToString();

            string s_data = workingMode + "/" + fx_string + "/" + fy_string + "/" + damping.ToString();

            SendData(s_data);
        }

    }

    public static void SendLevelAndForce(int waitTime)
    {
        //Envio del modo de control
        int mode = (int)InputManager.NivelAsistencia;
        SendData(mode + "/");
        Thread.Sleep(waitTime);
    }

    private static void SendData(string msg)
    {
        try
        {
            byte[] data = Encoding.ASCII.GetBytes(msg.Replace(',', '.'));

            udpSend.Send(data, data.Length, remoteEndPoint);

            //Thread.Sleep(100); //le damos un tiempo a la controladora
        }
        catch (Exception err)
        {
            Debug.Log(err.ToString());
        }
    }
    #endregion

    #region [Public Functions] Gestión de errores

    public static bool HasError() => hasError;

    public static string GetError() => error;

    public static void RequestError(int waitTime)
    {
        string mssg = "5/";
        SendData(mssg);
        Thread.Sleep(waitTime); //Espera un tiempo marcado
    }

    public static void ClearError(int waitTime)
    {
        string mssg = "6/";
        SendData(mssg);
        Thread.Sleep(waitTime); //Espera un tiempo marcado
        hasError = false;
    }

    public static void ResetModeControl(int waitTime)
    {
        if (workingMode >= 1 && workingMode <= 4)
        {
            string mssg = workingMode + "/";
            SendData(mssg);
            Thread.Sleep(waitTime); //Espera un tiempo marcado
        }
    }

    #endregion
    //*********************************************************************************//
    //*********************************************************************************//



    //****************************Hilo de recepción************************************//
    //*********************************************************************************//
    #region [Functions] Recibir datos

    /// <summary>
    /// Hilo paralelo que recibe todos los datos del rubidium en formato string
    /// </summary>
    private static void ReceiveData()
    {
        //double tic = 0, toc = 0, telapse = 0;

        //double tloop = 1.0 / 50.0; // 50Hz

        //Proces de recepcion de datos enviados por el robot
        while (isCOMRecvInit)
        {
            //tic = (System.DateTime.UtcNow.Subtract(GameManager.sharedInstance.datetime)).TotalMilliseconds;

            try
            {
                //Recepcion de bytes
                byte[] data = udpRead.Receive(ref localEndPoint);

                string returnData = Encoding.ASCII.GetString(data);
                currentTrial = returnData;

                //Separar datos dependiendo de los bytes recibidos
                dataRubidium = SplitRobotData(returnData);

            }
            catch (Exception e)
            {
                string es = e.Message;
            }

            //toc = (System.DateTime.UtcNow.Subtract(GameManager.sharedInstance.datetime)).TotalMilliseconds;

            //telapse = toc - tic;
            //if (telapse < tloop)
            ///Thread.Sleep((int)((tloop - telapse) * 1000.0));
        }
    }


    /// <summary>
    /// Procesa el string recibido por el rubidium y lo convierte a comandos de control
    /// </summary>
    /// <param name="data"></param>
    /// <returns></returns>
    private static float[] SplitRobotData(string data)
    {
        //Separa el string
        string[] datos = data.Split('/');

        //Comando de control
        float comando = float.Parse(datos[0]);

        //Array de vuelta
        float[] arraydata = new float[datos.Length - 1];
        arraydata[0] = comando;


        //Analiza el comando recibido
        if ((comando >= 1 && comando <= 4) || comando == 22) //Modos de control
        {
            workingMode = (int)comando;
            //Convertir strings en un array de variables de info del robot       
            for (int i = 1; i < datos.Length - 1; i++)
                arraydata[i] = float.Parse(datos[i].Replace('.', ','));

        }
        else if (comando == 5)
        {
            //Convertir strings en un array de variables de error del robot       
            arraydata[0] = comando;
            arraydata[1] = float.Parse(datos[1].Replace('.', ','));
            processError = datos[2];
        }
        else if (comando == 7)
        {
            hasError = true;
        }

        return arraydata;
    }   
    #endregion
    //*********************************************************************************//
    //*********************************************************************************//

}
