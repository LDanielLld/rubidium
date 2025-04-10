using System.Collections.Generic;
using UnityEngine;

using System;
using System.Net;
using System.Net.Sockets;
using System.Threading;


/// <summary>
/// Modulo de comunicacion entre el juego y el REVIRE para intercambiar datos 
/// al finalizar la actividad. El juego señala al REVIRE cuando ha acabado la actividad
/// y le traslada propiedades caracteristicas de cada tipo de juego
/// </summary>
public class SenderFinalData : MonoBehaviour
{


    //************************************Variables************************************//
    //*********************************************************************************//
    #region [Variables] UDP    
    public static string remoteIP = "127.0.0.1";

    private static UdpClient udpSend;    
    public static int remotePort = 33333;
    private static IPEndPoint remoteEndPoint;
    #endregion

    #region [Variables] Propiedades
    private static List<float> dataRecorded = new List<float>();
    #endregion
    //*********************************************************************************//
    //*********************************************************************************//


        
    //****************************Inicialización y cierre******************************//
    //*********************************************************************************//
    #region [Public Functions] Inicializacion    
    public static void InitCOM()
    {
        try 
        {           
            //Pipeline de  envio
            remoteEndPoint = new IPEndPoint(IPAddress.Parse(remoteIP), remotePort);
            udpSend = new UdpClient();      
        }
        catch(Exception es)
        {            
            Debug.Log(es.Message);
        }
    }   
    #endregion

    #region [Public Functions] Cierre    
    public static void Close()
    {
        udpSend.Close();  
    }
    #endregion
    //*********************************************************************************//
    //*********************************************************************************//



    //******************************Procesamiento de Datos*****************************//
    //*********************************************************************************//
    #region [Public Functions] Registro
    /// <summary>
    /// Añade un valor para enviar
    /// </summary>
    /// <param name="value"></param>
    public static void AddValue(float value)
    {
        dataRecorded.Add(value);
    }
    #endregion

    #region [Public Functions] Envio de datos    
    /// <summary>
    /// Funcion encargada de enviar datos
    /// </summary>
    public static void SendData()
    {
        byte[] data = GetByteData(dataRecorded);

        try
        {
            udpSend.Send(data, data.Length, remoteEndPoint);
            Thread.Sleep(100); //le damos un tiempo a la controladora
        }
        catch (Exception err)
        {
            Debug.Log(err.ToString());
        }
    }
    
    /// <summary>
    /// Tranforma una lista de float a un array de bytes
    /// </summary>
    /// <param name="sendata"></param>
    /// <returns></returns>
    private static byte[] GetByteData(List<float> sendata)
    {
        int width = sizeof(float);
        byte[] data = new byte[sendata.Count * width];

        for (int i = 0; i < sendata.Count; ++i)
        {
            byte[] converted = BitConverter.GetBytes(sendata[i]);

            for (int j = 0; j < width; ++j)
            {
                data[i * width + j] = converted[j];
            }
        }
        return data;
    }
    #endregion

    #region [Public Functions] Reinicio
    public static void ResetData()
    {
        dataRecorded.Clear();
        AddValue(-1.0f);
    }
    #endregion
    //*********************************************************************************//
    //*********************************************************************************//


}
