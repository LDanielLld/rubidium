using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using REVIREPanels.Modelos;
using System.Data;
using System.ComponentModel;
using System.Numerics;

namespace REVIREPanels.Componentes
{

    public enum TypeData //Tipos de datos
    {
        TD_TimeStamp,
        TD_WorkingMode,
        TD_Xpr,
        TD_Ypr,
        TD_Vxr,
        TD_Vyr,
        TD_Fxr,
        TD_Fyr,
        TD_EndeffAngle,
        TD_RobotActivated,
        TD_RightArmSat,
        TD_LeftArmSat,
        TD_Pulsed,
        TD_NivelAsistencia,
        TD_Fuerza,
        TD_TiempoMax,
        TD_Cx,
        TD_Cy,
        TD_GameState,
        TD_TaskState,
        TD_Xpr0,
        TD_Ypr0,
        TD_XprF,
        TD_YprF,
        TD_Fx_ff,
        TD_Fy_ff,
        TD_Ff_origen_x,
        TD_Ff_origen_y,
        TD_Ff_destino_x,
        TD_Ff_destino_y,
        TD_CGauss,
        TD_None
    }

    /// <summary>
    /// Clase encargada de gestionar la carga de los datos binarios
    /// </summary>
    class BinaryDataManager
    {
        // private int numSignals = 24;
        private static Vector2 centralPoint = new Vector2(0f,-53f);
        private static int numSignals = 31;
        public static string errorMsg = "";

        public static string DataPath { get; set; } = "";
        

        //Datos de carga
        public static List<DataRobot[]> listDataRobot = new List<DataRobot[]>();
        public static List<List<DataRobot[]>> listTrialDataRobot = new List<List<DataRobot[]>>();
        
        //************************Carga de los datos**********************************//
        //****************************************************************************//
        /// <summary>
        /// Carga los ficheros de datos del robot y los almacena en una lista
        /// </summary>
        /// <param name="fileNames">Nombre de los ficheros</param>
        /// <returns></returns>
        public static int LoadDataRobotSession()
        {
            int nLoadSucces = 0;
            listDataRobot.Clear();

            string errortmp = "";
            errorMsg = ""; //Resetea string de error

           
                //Carga nombre del fichero binario de datos                
                string name = @"data\data.bin";

            DataRobot[] cDataRobot = GetDataRobotFromBinaryFile(name);
            listTrialDataRobot.Add(SeparateTrial(cDataRobot));

            if (cDataRobot.Length > 1)
                {
                    listDataRobot.Add(cDataRobot);
                    nLoadSucces++;
                }
                else                                    
                    errortmp += errorMsg; //Registra todos los errores                
            

            //Errores totales
            errorMsg = errortmp;            

            return nLoadSucces;
        }               
             

        private static DataRobot[] GetDataRobotFromBinaryFile(string fileName)
        {
            //Leer fichero binario con los datos del robot           
            try
            {
                FileStream fs = new FileStream(fileName, FileMode.Open, FileAccess.Read);
                BinaryReader br = new BinaryReader(fs, new ASCIIEncoding());

                double dis = fs.Length;
                int cantDoubles = (int)dis / sizeof(double);
                double[] chunk = new double[cantDoubles];
                int numsignals = (int)(dis / (sizeof(double) * numSignals));



                DataRobot[] dataRobot = new DataRobot[numsignals];

                //Separa todas las señales del fichero               
                for (int i = 0; i < numsignals; i++)
                {
                    DataRobot dataCurrent = new DataRobot
                    {
                        //Tiempo
                        TimeStamp = br.ReadDouble(),

                        //Datos robot                        
                        WorkingMode = br.ReadDouble(),
                        Xpr = br.ReadDouble(),
                        Ypr = br.ReadDouble(),
                        Vxr = br.ReadDouble(),
                        Vyr = br.ReadDouble(),
                        Fxr = br.ReadDouble(),
                        Fyr = br.ReadDouble(),
                        EndeffAngle = br.ReadDouble(),
                        RobotActivated = br.ReadDouble(),
                        RightArmSat = br.ReadDouble(),
                        LeftArmSat = br.ReadDouble(),
                        Pulsed = br.ReadDouble(),

                        //Adicionales
                        NivelAsistencia = br.ReadDouble(),
                        Fuerza = br.ReadDouble(),
                        TiempoMax = br.ReadDouble(),
                        Cx = br.ReadDouble(),
                        Cy = br.ReadDouble(),                        
                        TaskState = br.ReadDouble(),
                        GameState = br.ReadDouble(),
                        Xpr0 = br.ReadDouble(),
                        Ypr0 = br.ReadDouble(),
                        XprF = br.ReadDouble(),
                        YprF = br.ReadDouble(),

                        //Fuerzas
                        Fx_ff = br.ReadDouble(),
                        Fy_ff = br.ReadDouble(),
                        Ff_origen_x = br.ReadDouble(),
                        Ff_origen_y = br.ReadDouble(),
                        Ff_destino_x = br.ReadDouble(),
                        Ff_destino_y = br.ReadDouble(),
                        CGauss = br.ReadDouble()
                    };                   

                    dataRobot[i] = dataCurrent;
                }

                return dataRobot;

            }
            catch (Exception ex)
            {
                errorMsg = ex.Message;                
                return new DataRobot[0];
            }
            
        }

        private static List<DataRobot[]> SeparateTrial(DataRobot[] original)
        {            
            var result = new List<DataRobot[]>();

            if (original.Length > 0)
            {
                List<DataRobot> currentGroup = new List<DataRobot> { original[0] };

                for (int i = 1; i < original.Length; i++)
                {
                    if (original[i].TaskState == original[i - 1].TaskState)
                    {
                        currentGroup.Add(original[i]);
                    }
                    else
                    {
                        result.Add(currentGroup.ToArray());
                        currentGroup = new List<DataRobot> { original[i] };
                    }
                }

                result.Add(currentGroup.ToArray()); // Añadir el último grupo
            }

            
            var seriesDe1 = result.Where(group => group.First().TaskState == 2).ToList();

            return seriesDe1;

        }

        //****************************************************************************//

        //***************************Gestionar datos**********************************//
        //****************************************************************************//
        public static IEnumerable<double> GetAxisData(TypeData type, int index)
        {
            IEnumerable<double> axisData = SelectAxisData(type, listDataRobot[index]);
            return axisData;
        }

        private static IEnumerable<double> SelectAxisData(TypeData type, DataRobot[] datarobot)
        {

            IEnumerable<double> axisXData = null;

            try
            {
                //Generar vector con los datos
                switch (type)
                {
                    case TypeData.TD_TimeStamp:
                        axisXData = datarobot.Select(x => x.TimeStamp); break;
                    case TypeData.TD_Xpr: //Eje X filtrado a taskstate
                        axisXData = datarobot.Where(x => x.TaskState == 2).Select(x => x.Xpr); //Filtrado de taskstate
                       // axisXData = result.Where((val, index) => index == 0 || val != result[index - 1]);//Filtrado de repetidos
                        
                        break;                    
                    case TypeData.TD_Ypr:
                        axisXData = datarobot.Where(x => x.TaskState == 2).Select(x => x.Ypr); //Filtrado de taskstate
                        //axisXData = result2.Where((val, index) => index == 0 || val != result2[index - 1]);//Filtrado de repetidos
                        break;
                    case TypeData.TD_Vxr:
                       /* double[] result3 = datarobot.Where(x => x.TaskState == 2).Select(x => x.Vxr).ToArray(); //Filtrado de taskstate
                        axisXData = result3.Where((val, index) => index == 0 || val != result3[index - 1]);//Filtrado de repetidos
                        */
                        axisXData = datarobot.Where(x => x.TaskState == 2).Select(x => x.Vxr);break;
                    case TypeData.TD_Vyr:

                        /*double[] result4 = datarobot.Where(x => x.TaskState == 2).Select(x => x.Vyr).ToArray(); //Filtrado de taskstate
                        axisXData = result4.Where((val, index) => index == 0 || val != result4[index - 1]);//Filtrado de repetidos
                        */
                        axisXData = datarobot.Where(x => x.TaskState == 2).Select(x => x.Vyr); break;
                    case TypeData.TD_Fxr:
                        axisXData = datarobot.Select(x => x.Fxr); break;
                    case TypeData.TD_Fyr:
                        axisXData = datarobot.Select(x => x.Fyr); break;
                    case TypeData.TD_EndeffAngle:
                        axisXData = datarobot.Select(x => x.EndeffAngle); break;
                    case TypeData.TD_RobotActivated:
                        axisXData = datarobot.Select(x => x.RobotActivated); break;
                    case TypeData.TD_RightArmSat:
                        axisXData = datarobot.Select(x => x.RightArmSat); break;
                    case TypeData.TD_LeftArmSat:
                        axisXData = datarobot.Select(x => x.LeftArmSat); break;
                    case TypeData.TD_Pulsed:
                        axisXData = datarobot.Select(x => x.Pulsed); break;
                    case TypeData.TD_NivelAsistencia:
                        axisXData = datarobot.Select(x => x.NivelAsistencia); break;
                    case TypeData.TD_Fuerza:
                        axisXData = datarobot.Select(x => x.Fuerza); break;
                    case TypeData.TD_TiempoMax:
                        axisXData = datarobot.Select(x => x.TiempoMax); break;
                    case TypeData.TD_Cx:
                        axisXData = datarobot.Select(x => x.Cx); break;
                    case TypeData.TD_Cy:
                        axisXData = datarobot.Select(x => x.Cy); break;
                    case TypeData.TD_GameState:
                        axisXData = datarobot.Select(x => x.GameState); break;
                    case TypeData.TD_TaskState:
                        axisXData = datarobot.Select(x => x.TaskState); break;
                    case TypeData.TD_Xpr0:
                        axisXData = datarobot.Select(x => x.Xpr0); break;
                    case TypeData.TD_Ypr0:
                        axisXData = datarobot.Select(x => x.Ypr0); break;
                    case TypeData.TD_XprF:
                        axisXData = datarobot.Select(x => x.XprF); break;
                    case TypeData.TD_YprF:
                        axisXData = datarobot.Select(x => x.YprF); break;
                    default:
                        axisXData = datarobot.Select(x => x.TimeStamp); break;
                }
            }
            catch (Exception)
            {
                errorMsg = "Datos inaccesibles";
            }
            return axisXData;
        }


        public static IEnumerable<double> GetAxisDataTrial(TypeData type, int index, int trial)
        {
            IEnumerable<double> axisData = SelectAxisData(type, listTrialDataRobot[index][trial]);
            return axisData;
        }
        //****************************************************************************//
        //****************************************************************************//
    }
}
