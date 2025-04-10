using System;
using System.IO;
using System.Threading;
using UnityEngine;

//- Meta Data
//	modo de juego(telerehab, local, asistivo fijo)
//	fecha y hora
//	params de inputManager
//	Params force field

//- GAME
//	data Rubidium
//	estado del juego(iendo a centro, esperando, iendo a target, etc.)
//	ID target a alganzar(-1 si no hay ninguno, 0 centro, y el resto los targets)
//	tiempo del juego
//	score
//	fuerza terapeuta medida por el onRobot
//	fuerza aplicada por forcefield
//	fuerza resultante aplicada al efector final

//- Multijugador: Data Events snd
//- Multijugador: Data Events rcv

//- Multijugador: Data Real Time snd
//- Multijugador: Data Real Time rcv

public class DataManager : MonoBehaviour
{
    #region General Variables

    private static bool b_isRec = false;
    private static string saveFolder;
    private static string filegamemode;

    #endregion General Variables

    #region DATA Robot

    private static FileStream data_file;
    private static BinaryWriter writer;
    private static byte[] data_bufferFileByte; //Buffer para guardar bytes
    private static int data_i = 0;
    private static int data_i_byte = 0;
    private static int data_packetSize = 5;
    //private static int data_num_signals = 1 + 12 + 22;
    private static int data_num_signals = 1 + 12 + 18;

    #endregion DATA Robot

    #region [Private Functions]

    private static void InitDataBuffer(int t)
    {
        data_bufferFileByte = new byte[data_packetSize * data_num_signals * sizeof(double)];
    }

    private static void OpenRecordData()
    {

        filegamemode = "data";// GameManager.sharedInstance.gameMode.ToString();        

        //Directorio temporal
        string tempath = Application.dataPath + "/temp/";
        string tempathtotal = Application.dataPath + "/temp/" + filegamemode + ".bin";

        //Crear fichero temporal para guardar datos si hace falta
        if (!Directory.Exists(tempath))
        {
            try
            {
                Directory.CreateDirectory(tempath);
                FileStream f = File.Open(tempath + filegamemode + ".bin", FileMode.CreateNew);
                f.Close();
            }
            catch (Exception err)
            {
                print(err.ToString());
            }
        }

        try
        {
            //Abrir archivo y escritor binario
            data_file = File.Open(tempathtotal, FileMode.Create);
            writer = new BinaryWriter(data_file);

            b_isRec = true; //Para que empiece a grabar
        }
        catch (Exception err)
        {
            print(err.ToString());
        }
    }

    //Eliminar fichero y carpeta
    public static void DeleteFileData()
    {
        try
        { 
            
            /*string completePath = Application.dataPath + saveFolder;
            DirectoryInfo dir = new DirectoryInfo(completePath);

            //Borra ficheros
            foreach (FileInfo fi in dir.GetFiles())
                fi.Delete();

            //Borra directorio actual
            dir.Delete();*/
        }
        catch (Exception err)
        {
            print(err.ToString());
        }
    }

    private static void ResetData()
    {
        //reset data files
        data_file.Close();
        writer.Close();

        //Abrir otra vez
        OpenRecordData();

        //Inicializacion del buffer de datos
        InitDataBuffer(data_packetSize);
    }

    private static void ConvertAndInsertDataByte(int index, double data)
    {
        //Añadir bytes al buffer de registro
        byte[] temp = BitConverter.GetBytes(data);
        temp.CopyTo(data_bufferFileByte, index);
        data_i_byte += 8;
    }

    #endregion [Private Functions]

    #region [Public Functions]

    public static void StopRecordData()
    {
        if (b_isRec)
        {
            b_isRec = false;

            Thread.Sleep(1000);

            data_file.Close();
            writer.Close();

            //Copiar fichero temporal al definitivo
            string sourcefilename = Application.dataPath + "/temp/" + filegamemode + ".bin";
            string destfilename = Application.dataPath + saveFolder + "/" + filegamemode + ".bin";
            File.Copy(sourcefilename, destfilename);
        }
    }

    public static bool isRecord()
    {
        return b_isRec;
    }

    // Use this for initialization
    public static void Init()
    {
        //Abrir fichero temporal
        OpenRecordData();

        //Crear carpeta de almacenaje si se quiere grabar
        if (b_isRec)
        {
            //Crear carpeta para guardar datos actuales
            DateTime dateTime = DateTime.Now;
            saveFolder = "/data/";
            string c_date = dateTime.Year + "_" + dateTime.Month + "_" + dateTime.Day +
                "_" + dateTime.Hour + "_" + dateTime.Minute + "_" + dateTime.Second;
            saveFolder = saveFolder + c_date;

            //Current path
            string completePath = Application.dataPath + saveFolder;

            Debug.Log("SAVE: " + completePath);

            Directory.CreateDirectory(completePath);

            //Inicializar buffer de grabacion
            InitDataBuffer(data_packetSize);
        }
    }



    public static void SaveRubidiumData(float[] data)
    {
        if (b_isRec)
        {
            float timeStamp = data[0];

            //Convertir strings en las variables de info del robot
            float working_mode = data[1];
            float x = data[2];
            float y = data[3];
            float vx = data[4];
            float vy = data[5];
            float fx = data[6];
            float fy = data[7];
            float endeff_angle = data[8];
            float robot_activated = data[9];
            float right_arm_sat = data[10];
            float left_arm_sat = data[11];
            float pulsed = data[12];

            //Datos adicionales
            float nivel_asistencia = data[13];
            float fuerza = data[14];
            float tiempoMax = data[15];
            float cpx = data[16]; // CurrentPosition
            float cpy = data[17];
            float gameState = data[18];
            float taskState = data[19];
            float xpr0 = data[20];
            float ypr0 = data[21];
            float xprF = data[22];
            float yprF = data[23];

            float fx_ff = data[24];
            float fy_ff = data[25];
            float ff_origen_x = data[26];
            float ff_origen_y = data[27];
            float ff_destino_x = data[28];
            float ff_destino_y = data[29];
            float cGauss = data[30];

            //Gestion de guardado de los valores		
            ConvertAndInsertDataByte(data_i_byte, timeStamp);
            ConvertAndInsertDataByte(data_i_byte, working_mode);
            ConvertAndInsertDataByte(data_i_byte, x);
            ConvertAndInsertDataByte(data_i_byte, y);
            ConvertAndInsertDataByte(data_i_byte, vx);
            ConvertAndInsertDataByte(data_i_byte, vy);
            ConvertAndInsertDataByte(data_i_byte, fx);
            ConvertAndInsertDataByte(data_i_byte, fy);
            ConvertAndInsertDataByte(data_i_byte, endeff_angle);
            ConvertAndInsertDataByte(data_i_byte, robot_activated);
            ConvertAndInsertDataByte(data_i_byte, right_arm_sat);
            ConvertAndInsertDataByte(data_i_byte, left_arm_sat);
            ConvertAndInsertDataByte(data_i_byte, pulsed);

            ConvertAndInsertDataByte(data_i_byte, nivel_asistencia);
            ConvertAndInsertDataByte(data_i_byte, fuerza);
            ConvertAndInsertDataByte(data_i_byte, tiempoMax);
            ConvertAndInsertDataByte(data_i_byte, cpx);
            ConvertAndInsertDataByte(data_i_byte, cpy);
            ConvertAndInsertDataByte(data_i_byte, gameState);
            ConvertAndInsertDataByte(data_i_byte, taskState);
            ConvertAndInsertDataByte(data_i_byte, xpr0);
            ConvertAndInsertDataByte(data_i_byte, ypr0);
            ConvertAndInsertDataByte(data_i_byte, xprF);
            ConvertAndInsertDataByte(data_i_byte, yprF);

            ConvertAndInsertDataByte(data_i_byte, fx_ff);
            ConvertAndInsertDataByte(data_i_byte, fx_ff);            
            ConvertAndInsertDataByte(data_i_byte, ff_origen_x);
            ConvertAndInsertDataByte(data_i_byte, ff_origen_y);
            ConvertAndInsertDataByte(data_i_byte, ff_destino_x);
            ConvertAndInsertDataByte(data_i_byte, ff_destino_y);
            ConvertAndInsertDataByte(data_i_byte, cGauss);


            data_i++;
            if (data_i == data_packetSize)
            {
                try
                {
                    //Grabar datos
                    writer.Write(data_bufferFileByte);
                    data_i = 0;
                    data_i_byte = 0;
                }
                catch (Exception err)
                {
                    print(err.Data.ToString());
                }
            }
        }
    }
    #endregion [Public Functions]
}