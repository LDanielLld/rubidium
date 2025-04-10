using REVIREPanels.Modelos;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace REVIREPanels
{
    /// <summary>
    /// Clase con funciones de apoyo
    /// </summary>
    public class Util
    {
        /// <summary>
        /// Devuelve un string formato de tiempo corto con los segundos incorporados
        /// </summary>
        /// <param name="timeValueLoop"></param>
        /// <param name="type"></param>
        /// <returns></returns>
        public static string CalculateStringTime(float timeValueLoop, int type)
        {
            //Calcular string de tiempo para su visualizacion
            TimeSpan timespan = TimeSpan.FromSeconds(timeValueLoop);
            int hour = timespan.Hours;
            int min = timespan.Minutes;
            int sec = timespan.Seconds;
            string timerString = "";

            if (type == 0)
                timerString = sec.ToString("D1");
            else if (type == 1)
            {
                string mins = ((int)timeValueLoop / 60).ToString("00");
                float segundos = timeValueLoop % 60;
                float segc = (float)Math.Ceiling(segundos);
                string segs = ((int)segc).ToString("00");

                //string segs = ((int)timerControl % 60).ToString("00");
                if (segs == "60")
                {
                    segs = "00";
                    mins = (int.Parse(mins) + 1).ToString("00");
                }

                timerString = string.Format("{00}:{01}", mins, segs);
            }

            return timerString;
        }

        /// <summary>
        /// Extrae el array diferencia de un array de entrada
        /// </summary>
        /// <param name="flags"></param>
        /// <returns></returns>
        public static List<float> DiffAlgorithm(IEnumerable<double> flags)
        {
            //Descartar la primera variacion ya que siempre ira a la posicion inicial al iniciar el juego
            List<float> diff = new List<float>();
            bool first = false;
            for (int i = 0; i < flags.Count() - 1; i++)
            {
                float cmp = (float)(flags.ElementAt(i + 1) - flags.ElementAt(i));
                if(!first && cmp != 0)
                {
                    first = true;
                    cmp = 0;
                }

                diff.Add(cmp);
            }           

            return diff;
        }

        /// <summary>
        /// Extrae el array diferencia de un array de entrada y lo procesa para obtener los trials
        /// </summary>
        /// <param name="flags"></param>
        /// <returns></returns>
        public static List<int> DiffAlgorithm(IEnumerable<double> flags, string sflags)
        {
            List<int> tmp_start = new List<int>();
            try
            {
                //Procesa stado inicial y final del cambio
                string[] ind = sflags.Split('-');
                int s1 = int.Parse(ind[1]) - int.Parse(ind[0]);//-2); //Variacion de flags

                //Descartar la primera variacion ya que siempre ira a la posicion inicial al iniciar el juego
                List<float> diff = new List<float>();
                bool first = false;
                for (int i = 0; i < flags.Count() - 1; i++)
                {
                    float cmp = (float)(flags.ElementAt(i + 1) - flags.ElementAt(i));
                    if (!first && cmp != 0)
                    {
                        first = true;
                        cmp = 0;
                    }

                    diff.Add(cmp);
                }

                //Se seleccionan los indices donde se produce el cambio de evento de inicio de trial
                List<int> tmp = new List<int>();
                int index = 0;
                while (index != -1)
                {
                    index = diff.ToList().FindIndex(x => x == s1);
                    if (index != -1)
                    {
                        tmp.Add(index + 1);
                        diff[index] = 0; //Lo anula para el siguiente elemento
                    }
                }

                //Filtra los que no corresponden al flag de inicio del trial                
                for (int i = 0; i < tmp.Count(); i++)
                {
                    double value = flags.ElementAt(tmp[i]);
                    if (flags.ElementAt(tmp[i]) == int.Parse(ind[1]))//1)
                    {
                        tmp_start.Add(tmp[i]);
                    }
                }
            }
            catch(Exception){}
            return tmp_start;
        }       

        /// <summary>
        /// Elimina los elementos repetidos dentro de un array
        /// </summary>
        /// <param name="array"></param>
        /// <returns></returns>
        public static List<dynamic> RemoveDuplicates(IEnumerable<dynamic> array)
        {
            HashSet<dynamic> hashWithoutDuplicates = new HashSet<dynamic>(array);
            List<dynamic> listWithoutDuplicates = hashWithoutDuplicates.ToList();
            return listWithoutDuplicates;
        }

        /// <summary>
        /// Calcula el producto escalar de dos vectores (angulo entre vectores)
        /// </summary>
        /// <param name="p0"></param>
        /// <param name="p1"></param>
        /// <returns></returns>
        public static double PEscalar(PointF p0, PointF p1)
        {
            double s = p1.X * p0.X + p1.Y * p0.Y;
            double s1 = Math.Sqrt(Math.Pow(p1.X, 2) + Math.Pow(p1.Y, 2)) * Math.Sqrt(Math.Pow(p0.X, 2) + Math.Pow(p0.Y, 2));
            double angle = (Math.Acos(s / s1)); //Angulo
            return angle;
        }

        /// <summary>
        /// Calcula el modulo de un vector (distancia)
        /// </summary>
        /// <param name="vect"></param>
        /// <returns></returns>
        public static float GetModulo(Vector2 vect)
        {
            float module = (float)Math.Sqrt((float)Math.Pow(vect.X, 2) + (float)Math.Pow(vect.Y, 2));
            return module;
        }

        /// <summary>
        /// Distancia entre dos puntos
        /// </summary>
        /// <param name="v1"></param>
        /// <param name="v2"></param>
        /// <returns></returns>
        public static float DistanceBetweenPoints(Vector2 v1, Vector2 v2)
        {
            double distance = Math.Sqrt(Math.Pow(v2.X - v1.X, 2) + Math.Pow(v2.Y - v1.Y, 2));
            return (float)distance;

        }


        //*******************Extraer información de las sesiones*************************//
        //*******************************************************************************//
        #region Extraer informacion de las sesiones
        /// <summary>
        /// Obtiene una lista con los nombre de los ejercicios realizados en una sesion
        /// </summary>
        /// <param name="activities"></param>
        /// <returns></returns>
        public static List<string> GetListNameTask(List<Actividad> activities)
        {
            List<string> listtask = new List<string>(); //Almacena memoria

            for (int i = 0; i < activities.Count; i++)
            {
                //Definir el punto                                                
                string name = activities[i].Nombre.Trim() + "_" + activities[i].Fecha.ToShortTimeString(); ;

                //Guardar
                listtask.Add(name);
            }
            return listtask;
        }

        /// <summary>
        /// Extrae las caracteristicas requeridas de una lista de actividades
        /// </summary>
        /// <param name="type"></param>
        /// <param name="activities"></param>
        /// <returns></returns>
        public static IEnumerable<double> GetNumericInfoTasks(int type, List<Actividad> activities)
        {
            Actividad[] arrayact = activities.ToArray();

            //Extrae las caracteristicas requeridas
            IEnumerable<double> tasksData = null;

            try
            {
                //Generar vector con los datos de las actividades     
                switch (type)
                {
                    case 1: //Repeticiones
                        tasksData = arrayact.Select(x => (double)x.Repeticiones); break;
                    case 2: //Aciertos                        
                        tasksData = arrayact.Select(x => double.Parse(x.DatosRecibidos.Split(' ')[1])); break;
                    case 3: //Distancia 
                        tasksData = arrayact.Select(x => (double)x.Distancia); break;
                    case 4: //Tiempo
                        tasksData = arrayact.Select(x => double.Parse(x.DatosRecibidos.Split(' ')[0])); break;
                    case 5: //Errores 
                        tasksData = arrayact.Select(x => double.Parse(x.DatosRecibidos.Split(' ')[2])); break;

                    default: break;
                }
            }
            catch (Exception)
            {

                MessageBox.Show("Error al acceder a los datos de la sesión.");
            }
            return tasksData;
        }


        /// <summary>
        /// Elimina fichero y carpeta de la actividad borrada de la base de datos
        /// </summary>
        /// <param name="path"></param>
        //Eliminar fichero y carpeta
        public static void DeleteFileData(string path)
        {
            try
            {                
                DirectoryInfo dir = new DirectoryInfo(path);

                //Borra ficheros
                foreach (FileInfo fi in dir.GetFiles())
                    fi.Delete();

                //Borra directorio actual
                dir.Delete();
            }
            catch (Exception)
            {}
        }

        public static string GetPathFile(Actividad act, int type)
        {
            //Seleccionar carpeta donde estan los ficheros
            string folderpath = "";

#if !DEBUG            
            folderpath = Directory.GetCurrentDirectory() +
                @"\Activities\" + act.Nombre.Trim(' ') + @"\" + act.Nombre.Trim(' ') + @"_Data\data\";
#else
            folderpath = @"D:\UnityProjects\rubidium_games\" + act.Nombre.Trim(' ') + @"\" +
                act.Nombre.Trim(' ') + @"_Data\data\";

#endif

            DateTime datetime = act.Fecha;
            string filepath = datetime.Year + "_" + datetime.Month + "_" + datetime.Day + "_" + datetime.Hour +
                "_" + datetime.Minute + "_" + datetime.Second;

            string path = folderpath + filepath;
            
            return path;
        }

        public static DateTime GetLastDateFile(string act)
        {
            //Añadir la fecha en la que se han guardado los datos de la actividad en el juego. 
#if !DEBUG
            DirectoryInfo dinfo = new DirectoryInfo(@"Activities\" + act.Trim(' ') + @"\" + act.Trim(' ') + @"_Data\data\");
#else
            DirectoryInfo dinfo = new DirectoryInfo(@"D:\UnityProjects\rubidium_games\" + act.Trim(' ') + @"\" + act.Trim(' ') + @"_Data\data\");
#endif

            DirectoryInfo[] darray = dinfo.GetDirectories();

            DateTime datet = new DateTime(1000, 1, 1, 0, 0, 0);
            foreach (DirectoryInfo d in darray)
            {
                string[] dn = d.Name.Split('_');
                DateTime newDateTime = new DateTime(Int32.Parse(dn[0]), Int32.Parse(dn[1]), Int32.Parse(dn[2]),
                    Int32.Parse(dn[3]), Int32.Parse(dn[4]), Int32.Parse(dn[5]));
                if (DateTime.Compare(datet, newDateTime) < 0)
                {
                    datet = newDateTime;
                }
            }
            return datet;
        }
        #endregion
        //*******************************************************************************//
        //*******************************************************************************//

        public static Size PredefinedSizeScreen()
        {
            Size sizescreen = Screen.PrimaryScreen.Bounds.Size;
            float newsizeh = (float)sizescreen.Height * 91f / 100f;
            Size size = new Size(sizescreen.Width, (int)newsizeh);//
            return size;
        }
    }
}
