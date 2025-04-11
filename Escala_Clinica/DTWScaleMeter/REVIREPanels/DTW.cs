using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Numerics;
using System;
using System.Collections.Generic;

namespace REVIREPanels
{

    public class DTWResult
    {
        public float Distance { get; set; }
        public List<int[]> Path { get; set; } = new List<int[]>();
    }

    public static class DTW
    {

        private static float[] center = new float[2] {0.0f, -530.0f};

        /// <summary>
        /// Metodo encargado de generar las trayectorias ideales a partir de la configuracion
        /// de los path clinicos realizados en el juego ClinicalScaleMeter
        /// </summary>
        /// <returns></returns>
        public static List<List<float[]>> GenerateIdealPath(float ampl, int sample)
        {
            List<List<float[]>> list = new List<List<float[]>>();
            //Cada camino es diferente, por lo que se generaran a partir de una serie de ecuacion de recta y circulares
            float incr = ampl / sample; //Incremento

            //El primer camino es una recta. (izquierda a derecha)

            float[] x = Enumerable.Range(0, sample).Select(i => -ampl / 2 + i * incr).ToArray();
            float[] y = Enumerable.Repeat(center[1], sample).ToArray();

            List<float[]> tmpList = GetArrayRectX(-ampl/2, ampl/2, sample);
            list.Add(tmpList);

            //Segundo camino (recta de derecha a izquierda)
            incr = ampl / sample;
            x = Enumerable.Range(0, sample).Select(i => ampl / 2 - i * incr).ToArray();
            y = Enumerable.Repeat(center[1], sample).ToArray();

            tmpList = GetArrayRectX(ampl/2, -ampl/2, sample);  //Se junta y se añade
            list.Add(tmpList);

            //Trayectoria circular
            float radio = ampl/2;
            //  x = Enumerable.Range(0, sample).Select(i => initValue + i * incr * direction).ToArray();
            //  y = Enumerable.Repeat(center[1], sample).ToArray();

            //float initAngle = 225;
            //float endAngle = 0;

            //float incremento = ((endAngle - initAngle) / sample) * ((float)(Math.PI) / 180.0f);

            //x = new float[sample];
            //y = new float[sample];
            //float[] angles = new float[sample];

            //for (int i = 0; i < sample; i++)
            //{
            //    float radAngle = initAngle * (float)Math.PI / 180.0f;
            //    float angulo = radAngle + i * incremento;

            //    //  float angulo = (float)(2 * Math.PI * i / sample);
            //    x[i] = center[0] + radio * (float)Math.Cos(angulo);
            //    y[i] = center[1] + radio * (float)Math.Sin(angulo);
            //    angles[i] = angulo;
            //}

            //Tercera trayectoria circular
            tmpList = GetArrayCircle(ampl/2, 225.0f, 0.0f, sample);
            list.Add(tmpList);

            //Cuarta trayectoria circular
            tmpList = GetArrayCircle(ampl / 2, 315.0f, 270f + 315f, sample);
            list.Add(tmpList);

            //Quinta trayectoria recta (bajo a arriba)
            tmpList = GetArrayRectY(0, ampl/2, sample);
            list.Add(tmpList);

            //Sexta trayectoria recta (arriba a abajo)
            tmpList = GetArrayRectY(ampl/2, 0, sample);
            list.Add(tmpList);

            //Septima trayectoria recta (arriba a abajo)
            tmpList = GetArrayRectY(0, -ampl/2, sample);
            list.Add(tmpList);

            //Octava trayectoria recta (abajo a arriba)
            tmpList = GetArrayRectY(-ampl/2, ampl/2, sample);
            list.Add(tmpList);

            //Novena trayectoria recta (arriba a abjo)
            tmpList = GetArrayRectY(ampl/2, -ampl/2, sample);
            list.Add(tmpList);

            return list;
        }

        private static List<float[]> GetArrayRectX(float init, float end, int sample)
        {
            float incr = (end - init) / sample;
            float[] x = Enumerable.Range(0, sample).Select(i => init + i * incr).ToArray();
            float[] y = Enumerable.Repeat(center[1], sample).ToArray();

            List<float[]> tmpList = x.Zip(y, (a, b) => new float[] { a, b }).ToList(); //Se junta y se añade
            return tmpList;
        }

        private static List<float[]> GetArrayRectY(float init, float end, int sample)
        {
            float incr = (end - init) / sample;
            float[] x = Enumerable.Repeat(center[0], sample).ToArray(); 
            float[] y = Enumerable.Range(0, sample).Select(i =>center[1] + (init + i * incr)).ToArray();

            List<float[]> tmpList = x.Zip(y, (a, b) => new float[] { a, b }).ToList(); //Se junta y se añade
            return tmpList;
        }


        /// <summary>
        /// Genera una trayectoria circular añadiendo angulo inicial y final
        /// </summary>                
        private static List<float[]> GetArrayCircle(float radio, float init, float end, int sample)
        {
            float incr = ((end - init) / sample) * ((float)(Math.PI) / 180.0f); 
            float initAngle = init * (float)Math.PI / 180.0f;

            float[] x = Enumerable.Range(0, sample).Select(i => center[0] + radio * (float)Math.Cos(initAngle + i * incr)).ToArray();
            float[] y = Enumerable.Range(0, sample).Select(i => center[1] + radio * (float)Math.Sin(initAngle + i * incr)).ToArray();

            List<float[]> tmpList = x.Zip(y, (a, b) => new float[] { a, b }).ToList(); //Se junta y se añade
            return tmpList;
        }


        public static DTWResult Calculate(List<float[]> seq1, Vector3[] seq2, int windowSize = -1)
        {
            //Obtiene las longitudes de los vectores
            int n = seq1.Count;
            int m = seq2.Length;

            // Si no se define ventana, se permite comparar todo
            int w = (windowSize < 0) ? Math.Max(n, m) : windowSize;

            //Matriz de coste
            float[,] cost = new float[n + 1, m + 1];



            // Inicializar con infinito
            for (int i = 0; i <= n; i++)
                for (int j = 0; j <= m; j++)
                    cost[i, j] = float.PositiveInfinity;

            cost[0, 0] = 0;

            // Llenar matriz DTW
            for (int i = 1; i <= n; i++)
            {
                int jStart = Math.Max(1, i - w);
                int jEnd = Math.Min(m, i + w);

                for (int j = jStart; j <= jEnd; j++)
                {
                    float[] cSeq1 = seq1[i - 1];

                    float dist = Vector3.Distance( new Vector3(cSeq1[0], cSeq1[1], 0), seq2[j - 1]);
                    float minPrev = Math.Min(
                        cost[i - 1, j],      // inserción
                        cost[i, j - 1]);      // eliminación
                    minPrev = Math.Min(minPrev, cost[i - 1, j - 1]);   // coincidencia
                    //);
                    cost[i, j] = dist + minPrev;
                }
            }


            int x = n;
            int y = m;

            // Reconstruir el camino óptimo
            List<int[]> path = new List<int[]>();

            while (x > 0 && y > 0)
            {
                path.Add(new int[] { x - 1, y - 1 });

                float diag = cost[x - 1, y - 1];
                float left = cost[x, y - 1];
                float up = cost[x - 1, y];

                if (diag <= left && diag <= up)
                {
                    x--; y--;
                }
                else if (left < up)
                {
                    y--;
                }
                else
                {
                    x--;
                }
            }

            path.Reverse();


            return new DTWResult
            {
                Distance = cost[n, m],
                Path = path
            };
        }


        public static DTWResult Calculate(Vector3[] seq1, Vector3[] seq2, int windowSize = -1)
        {
            //Obtiene las longitudes de los vectores
            int n = seq1.Length;
            int m = seq2.Length;

            // Si no se define ventana, se permite comparar todo
            int w = (windowSize < 0) ? Math.Max(n, m) : windowSize;

            //Matriz de coste
            float[,] cost = new float[n + 1, m + 1];

            

            // Inicializar con infinito
            for (int i = 0; i <= n; i++)
                for (int j = 0; j <= m; j++)
                    cost[i, j] = float.PositiveInfinity;

            cost[0, 0] = 0;

            // Llenar matriz DTW
            for (int i = 1; i <= n; i++)
            {
                int jStart = Math.Max(1, i - w);
                int jEnd = Math.Min(m, i + w);

                for (int j = jStart; j <= jEnd; j++)
                {
                    float dist = Vector3.Distance(seq1[i - 1], seq2[j - 1]);
                    float minPrev = Math.Min(
                        cost[i - 1, j],      // inserción
                        cost[i, j - 1]);      // eliminación
                    minPrev = Math.Min(minPrev, cost[i - 1, j - 1]);   // coincidencia
                    //);
                    cost[i, j] = dist + minPrev;
                }
            }


            int x = n;
            int y = m;

            // Reconstruir el camino óptimo
            List<int[]> path = new List<int[]>();

            while (x > 0 && y > 0)
            {
                path.Add(new int[] { x - 1, y - 1 });

                float diag = cost[x - 1, y - 1];
                float left = cost[x, y - 1];
                float up = cost[x - 1, y];

                if (diag <= left && diag <= up)
                {
                    x--; y--;
                }
                else if (left < up)
                {
                    y--;
                }
                else
                {
                    x--;
                }
            }

            path.Reverse();


            return new DTWResult
            {
                Distance = cost[n, m],
                Path = path
            };
        }
    }
}
