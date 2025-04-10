using System;
using System.Linq;
using UnityEngine;


/// <summary>
/// Gestor de soporte para registrar y calcular posiciones de asistencia en coordenadas
/// del robot a partir de datos de posicion del mundo de la activida en Unity
/// </summary>
public class SupportManager : MonoBehaviour
{


    //************************************Variables************************************//
    //*********************************************************************************//
    #region [Variables] Limites escena Unity
    //Puntos para transformar coordenadas robot-escenario 3D
    public static float[] limSceneX; //Xmax y Xmin de los objetivos
    public static float[] limSceneY; //Ymax y Ymin de los objetivos
    #endregion

    #region [Variables] Centros
    //El rubidium tiene el cnetro y puede trabjara hasta un radio de 200 milimetros desde el centro
    public static float[] centroRubidium;
    public static float[] centroUnity;
    #endregion

    #region [Variables] Posiciones de asistencia
    public static Vector2[] supportPositionRobot;
    public static Vector2 supportInitPos;
    #endregion
    //*********************************************************************************//
    //*********************************************************************************//



    //****************************Inicializacion del gestor****************************//
    //*********************************************************************************//
    #region [Functions] Inicializacion

    /// <summary>
    /// Convierte una lista de posicion de Unity en las coordendas correspondientes
    /// a la posicion del robot, y devuelve los limites de movimiento del mundo Unity
    /// </summary>
    /// <param name="initialPos"></param>
    /// <param name="supportPosition"></param>
    /// <param name="cRubidium"></param>
    /// <param name="cUnity"></param>
    /// <returns></returns>
    public static float[] Init(Vector2 initialPos, Transform[] supportPosition, float[] cRubidium, float[] cUnity)
    {
        try
        {
            centroRubidium = cRubidium;
            centroUnity = cUnity;

            //Calcular distancia maxima recorrida en ambos ejes
            float[] vectory_tmp = new float[supportPosition.Length];
            float[] vectorx_tmp = new float[supportPosition.Length];
            for (int i = 0; i < supportPosition.Length; i++)
            {
                vectorx_tmp[i] = supportPosition[i].position.x;
                vectory_tmp[i] = supportPosition[i].position.y;
            }

            //Obtener distancias
            limSceneY = new float[] { vectory_tmp.Min(), vectory_tmp.Max() };
            limSceneX = new float[] { vectorx_tmp.Min(), vectorx_tmp.Max() };

            //Comprueba si esta centrado en 0, si no lo ajusta
            float med = (limSceneX[1] - limSceneX[0]) / 2f;
            float center = limSceneX[1] - med;
            if (center != 0)
            {
                limSceneX[1] -= center;
                limSceneX[0] -= center;
            }

            if (limSceneY[0] == limSceneY[1])
                CheckLimits(ref limSceneY);
            if (limSceneX[0] == limSceneX[1])
                CheckLimits(ref limSceneX);


            //Convertir posiciones escena en robot             
            supportPositionRobot = new Vector2[supportPosition.Length];
            for (int i = 0; i < supportPosition.Length; i++)
                supportPositionRobot[i] = SetAssistivePosition(supportPosition[i].position);
            

            //Convertir posiciones escena en robot           
            supportInitPos = SetAssistivePosition(initialPos);

            float[] limits = new float[] { limSceneX[0], limSceneX[1], limSceneY[0], limSceneY[1] };            

            return limits;
        }catch(Exception)
        {
            return new float[2];
        }

    }


    /// <summary>
    /// Comprueba que los valores no superen un limite
    /// </summary>
    /// <param name="limits"></param>
    private static void CheckLimits(ref float[] limits)
    {
        if (limits[0] > 0)
            limits[0] = limits[0] * -1f;
        else if (limits[1] < 0)
            limits[1] = limits[1] * -1f;
    }
    #endregion
    //*********************************************************************************//
    //*********************************************************************************//



    //*****************************Conversion Unity-Robot******************************//
    //*********************************************************************************//
    #region [Public Function] Asistencia
    /// <summary>
    /// Convierte un punto del mundo del juego a su equivalente posicion en el 
    /// mundo del robot. Obtiene una posicion de asistencia a partir de un punto del juego
    /// </summary>
    /// <param name="point"></param>
    /// <returns></returns>
    public static Vector2 SetAssistivePosition(Vector3 point)
    {        
        //Calcular los puntos dentro del espacio del robot
        Vector2 n = Vector2.zero;

        //Calcular eje X - Centro
        float factorx =  (float)InputManager.Amplitud / (limSceneX[1] - limSceneX[0]); //Incremento de eje Rubidium por 1 unidad de Unity
        n.x = (point.x - centroUnity[0]) * factorx + centroRubidium[0];
        n.y = (point.y - centroUnity[1]) * factorx + centroRubidium[1];

        return n / 1000f;
    }
    #endregion
    //*********************************************************************************//
    //*********************************************************************************//


}
