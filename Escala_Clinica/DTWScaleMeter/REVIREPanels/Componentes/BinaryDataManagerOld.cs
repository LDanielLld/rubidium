using REVIREPanels.Modelos;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace REVIREPanels.Componentes
{
    //Bytes de control del paquete
    enum EventCommands
    {
        PREFIX = 3, // Number of bytes that make up the Frame header
        SUFIX = 1, // Number of bytes that make up the Trailer

        STX = 0XAA, // Start of frame delimiter
        ETX = 0XBB, // End of frame delimiter

        EC_INIT_TARGETS = 0x01, //Flag para guardar las posiciones iniciales
        EC_TARGETS = 0x02, //Flag para guardar las posiciones objetivos
        EC_TRIAL = 0x03, //Indicador de trial    
    }

    public enum TypeTarget
    {
        NONE,
        TT_TARGET = 0xCC,
        TT_INIT = 0xDD
    }

        

    /// <summary>
    /// Clase encargada de gestionar la carga de los datos binarios
    /// </summary>
    class BinaryDataManagerOld
        {
            public static BinaryDataManagerOld sharedInstance = null;

            public string errorMsg = "";

            public BinaryDataManagerOld()
            {
                if (sharedInstance == null)
                    sharedInstance = this;
            }

            //Datos de carga
            public List<DataRobotOld[]> listDataRobot = new List<DataRobotOld[]>();
            public List<EventRobot> listEventRobot = new List<EventRobot>();


            //************************Carga de los datos**********************************//
            //****************************************************************************//
            /// <summary>
            /// Carga los ficheros de datos del robot y los almacena en una lista
            /// </summary>
            /// <param name="fileNames">Nombre de los ficheros</param>
            /// <returns></returns>
            public int LoadDataRobotSession(List<Actividad> fileNames)
            {
                int nLoadSucces = 0;
                listDataRobot.Clear();

                string errortmp = "";
                errorMsg = ""; //Resetea string de error

                foreach (Actividad act in fileNames)
                {
                    //Carga nombre del fichero binario de datos
                    string name = GetPathFile(act, 0);

                    DataRobotOld[] cDataRobot = GetDataRobotFromBinaryFile(name);

                    if (cDataRobot.Length > 1)
                    {
                        listDataRobot.Add(cDataRobot);
                        nLoadSucces++;
                    }
                    else
                        errortmp += errorMsg; //Registra todos los errores                
                }

                errorMsg = errortmp;

                //Ahora carga todos los eventos
                foreach (Actividad act in fileNames)
                {
                    //Carga nombre del fichero binario de datos
                    string name = GetPathFile(act, 1);

                    EventRobot cEventRobot = GetEventRobotFromBinaryFile(name);

                    if (cEventRobot != null)
                    {
                        listEventRobot.Add(cEventRobot);
                        nLoadSucces++;
                    }
                    else
                        errortmp += errorMsg; //Registra todos los errores                
                }

                return nLoadSucces;
            }



            private string GetPathFile(Actividad act, int type)
            {
                //Seleccionar carpeta donde estan los ficheros
                string folderpath = Directory.GetCurrentDirectory() +
                    @"\Activities\" + act.Nombre.Trim(' ') + @"\" + act.Nombre.Trim(' ') + @"_Data\data\";

                DateTime datetime = act.Fecha;
                string filepath = datetime.Year + "_" + datetime.Month + "_" + datetime.Day + "_" + datetime.Hour +
                    "_" + datetime.Minute + "_" + datetime.Second + @"\data.bin";

                string path = folderpath + filepath;

                //Temporal
                if (type == 0)
                    path = Directory.GetCurrentDirectory() + @"\Activities\data.bin";
                else
                    path = Directory.GetCurrentDirectory() + @"\Activities\events.bin";



                return path;
            }

            private DataRobotOld[] GetDataRobotFromBinaryFile(string fileName)
            {
                //Leer fichero binario con los datos del robot           
                try
                {
                    FileStream fs = new FileStream(fileName, FileMode.Open, FileAccess.Read);
                    BinaryReader br = new BinaryReader(fs, new ASCIIEncoding());

                    double dis = fs.Length;
                    int cantDoubles = (int)dis / 8;
                    double[] chunk = new double[cantDoubles];
                    int numsignals = (int)(dis / (8 * 17));


                    DataRobotOld[] dataRobot = new DataRobotOld[numsignals];

                    //Separa todas las señales del fichero               
                    for (int i = 0; i < numsignals; i++)
                    {
                        DataRobotOld dataCurrent = new DataRobotOld
                        {
                            TimeStamp = br.ReadDouble(),
                            NivelAsistencia = br.ReadDouble(),
                            Fuerza = br.ReadDouble(),
                            TiempoMax = br.ReadDouble(),
                            Xpr0 = br.ReadDouble(),
                            Ypr0 = br.ReadDouble(),
                            Safety = br.ReadDouble(),
                            Xpr = br.ReadDouble(),
                            Ypr = br.ReadDouble(),
                            Zpr = br.ReadDouble(),
                            Rpr = br.ReadDouble(),
                            Vxr = br.ReadDouble(),
                            Vyr = br.ReadDouble(),
                            Vzr = br.ReadDouble(),
                            Fxr = br.ReadDouble(),
                            Fyr = br.ReadDouble(),
                            MagAssist = br.ReadDouble()
                        };
                        dataRobot[i] = dataCurrent;
                    }

                    return dataRobot;

                }
                catch (Exception ex)
                {
                    errorMsg = ex.Message;
                    return new DataRobotOld[0];
                }

            }

            private EventRobot GetEventRobotFromBinaryFile(string fileName)
            {
                //Leer fichero binario con los eventos del ejercicio           
                try
                {
                    FileStream fs = new FileStream(fileName, FileMode.Open, FileAccess.Read);
                    BinaryReader br = new BinaryReader(fs, new ASCIIEncoding());

                    //Longitud final del fichero
                    double dis = fs.Length;

                    //Procesar datos
                    EventRobot eventsRobot = new EventRobot();

                    int idByte = 0;
                    while (idByte < dis)
                    {
                        byte cByte = br.ReadByte();
                        idByte += sizeof(byte);

                        if (cByte == Convert.ToByte(EventCommands.STX)) //Secuencia de entrada
                        {


                            //Lee es siguiente byte para comprobar que tipo de evento es
                            cByte = br.ReadByte();
                            idByte += sizeof(byte);


                            //Comprueba el tipo de evento
                            if (cByte == Convert.ToByte(EventCommands.EC_TARGETS))
                            {
                                //Comprueba cuantos objetivos hay
                                cByte = br.ReadByte();
                                idByte += sizeof(byte);

                                int nT = Convert.ToInt32(cByte) / (2 * sizeof(float));

                                for (int i = 0; i < nT; i++)
                                {

                                    float valueX = br.ReadSingle() * 100; idByte += sizeof(float);
                                    float valueY = br.ReadSingle() * 100; idByte += sizeof(float);
                                    eventsRobot.Target.Add(new System.Numerics.Vector2(valueX, valueY));
                                }

                                //Procesa byte fin de trama
                                cByte = br.ReadByte();
                                idByte += sizeof(byte);

                            }
                            else if (cByte == Convert.ToByte(EventCommands.EC_INIT_TARGETS))
                            {
                                //Comprueba cuantos posiciones iniciales hay
                                cByte = br.ReadByte();
                                idByte += sizeof(byte);

                                int nT = Convert.ToInt32(cByte) / (2 * sizeof(float));

                                for (int i = 0; i < nT; i++)
                                {

                                    float valueX = br.ReadSingle() * 100; idByte += sizeof(float);
                                    float valueY = br.ReadSingle() * 100; idByte += sizeof(float);
                                    eventsRobot.PosInit.Add(new System.Numerics.Vector2(valueX, valueY));
                                }

                                //Procesa byte fin de trama
                                cByte = br.ReadByte();
                                idByte += sizeof(byte);
                            }
                            else if (cByte == Convert.ToByte(EventCommands.EC_TRIAL))
                            {
                                //Cantidad de bytes
                                cByte = br.ReadByte();
                                idByte += sizeof(byte);

                                TrialPacket trial = new TrialPacket();

                                //Obtiene secuencia, numero de trial
                                uint sec = br.ReadUInt32(); idByte += sizeof(uint);
                                trial.sequence = sec;

                                //Obtiene objetivos
                                int targeti = br.ReadInt32(); idByte += sizeof(int);
                                int targetf = br.ReadInt32(); idByte += sizeof(int);
                                trial.idTarget = new int[2] { targeti, targetf };

                                //Obtiene tipo de objetivos
                                byte ttargeti = br.ReadByte(); idByte += sizeof(byte);
                                byte ttargetf = br.ReadByte(); idByte += sizeof(byte);
                                trial.typeTarget = new byte[2] { ttargeti, ttargetf };

                                //Obtiene tiempos
                                double tinit = br.ReadDouble(); idByte += sizeof(double);
                                double ttarget = br.ReadDouble(); idByte += sizeof(double);
                                double tendt = br.ReadDouble(); idByte += sizeof(double);
                                trial.timeTarget = new double[3] { tinit, ttarget, tendt };

                                //Procesa byte fin de trama
                                cByte = br.ReadByte();
                                idByte += sizeof(byte);

                                eventsRobot.Trials.Add(trial);
                            }
                        }


                    }
                    return eventsRobot;

                }
                catch (Exception ex)
                {
                    errorMsg = ex.Message;
                    return null;
                }
            }

            //****************************************************************************//

            //***************************Gestionar datos**********************************//
            //****************************************************************************//
            public IEnumerable<double> GetAxisData(int type, int index)
            {
                IEnumerable<double> axisData = SelectAxisData(type, listDataRobot[index]);
                return axisData;
            }

            private IEnumerable<double> SelectAxisData(int type, DataRobotOld[] datarobot)
            {

                IEnumerable<double> axisXData = null;

                try
                {
                    //Generar vector con los datos del eje y     
                    switch (type) //Eje Y
                    {
                        case 0:
                            axisXData = datarobot.Select(x => x.Xpr); break;
                        case 1:
                            axisXData = datarobot.Select(x => x.Ypr); break;
                        case 2:
                            axisXData = datarobot.Select(x => x.TimeStamp); break;
                        case 3:
                            axisXData = datarobot.Select(x => x.Vxr); break;
                        case 4:
                            axisXData = datarobot.Select(x => x.Vyr); break;
                        case 5:
                            axisXData = datarobot.Select(x => x.Fxr); break;
                        case 6:
                            axisXData = datarobot.Select(x => x.Fyr); break;
                        default:
                            axisXData = datarobot.Select(x => x.Xpr); break;
                    }
                }
                catch (Exception)
                {
                    errorMsg = "Datos inaccesibles";
                }
                return axisXData;
            }
            //****************************************************************************//
            //****************************************************************************//
        }
   
}
