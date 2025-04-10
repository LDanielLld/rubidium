using UnityEngine;


/// <summary>
/// Gestor que recolecta todos los argumentos de entrada de la actividad
/// cuando se ejecuta desde la ventana de comandos
/// </summary>
public class InputManager : MonoBehaviour {

    /**** 
     * Modo de funcionamiento - Parametro: NivelAsistencia
     * 1 - Modo libre
     * 2 - Modo asistido total (control por posicion)
     * 3 - Modo velocidad (no se utiliza)     
     * 4 - Modo fuerza: Aplica fuerzas calculadas desde el juego
     *                  Tiene 3 modos dependiendo parametro Fuerza
     *                  (-1: Tunel vacio 0:Tunel con extremos, >0:Assisted as needed) 
     * 5 - Modo Teleoperacion: Comunicacion online
     *                         Tiene en cuenta el modo (maestro esclavo). Parametro Fuerza
     *                         (0: Maestro, !0: Esclavo)
     *  
     *****/



    //******************************Parámetros de entrada******************************//
    //*********************************************************************************//
    #region [Parametros] Registro
    //Valores de entrada para modificar criterios de terapia
    private static string[] args = System.Environment.GetCommandLineArgs();
    #endregion

    #region [Parametros] Valores
    public static int Repeticiones { get; set; } = 0;
    public static int Amplitud { get; set; } = 0;
    public static int Orden { get; set; } = 0;
    public static float NivelAsistencia { get; set; } = 0.0f; //
    public static float Fuerza { get; set; } = 0.0f;
    public static float TimeTotal { get; set; } = 0.0f;
    public static float cGauss { get; set; } = 0.0f;
    #endregion
    //*********************************************************************************//
    //*********************************************************************************//



    //*********************************Inicialización**********************************//
    //*********************************************************************************//
    #region [Public Function] Inicializacion
    public static void Init()    
    {
        cGauss = 0.03f;
#if !UNITY_EDITOR
        if (args.Length > 1)
        {
            Repeticiones = int.Parse(args[1]);
            Amplitud = int.Parse(args[2]);
            Orden = int.Parse(args[3]);
            NivelAsistencia = float.Parse(args[4]);           
            Fuerza = float.Parse(args[5]);
            TimeTotal = float.Parse(args[6]);
        }
        else
        {
            Repeticiones = 15;
            Amplitud = 350;
            Orden = 0;
            NivelAsistencia = 1.0f;
            Fuerza = 0.0f;
            TimeTotal = 10.0f;     
        }
#else
        Repeticiones = 15;
        Amplitud = 350;
        Orden = 0;
        NivelAsistencia = 1.0f;
        Fuerza = 0.0f;
        TimeTotal = 10.0f;        
#endif        

    }
    #endregion
    //*********************************************************************************//
    //*********************************************************************************//



    //**********************Control externo de características*************************//
    //*********************************************************************************//
    #region [Public Function] Lectura
    /// <summary>
    /// Devuelve el numero de argumentos de entrada
    /// </summary>
    /// <returns></returns>
    public static int GetLength()
    {
#if !UNITY_EDITOR
        return  args.Length;
#else
        return 7;
#endif
        
    }

    /// <summary>
    /// Obtiene todos los valores de las entradas
    /// </summary>
    /// <returns></returns>
    public static float[] GetInputs()
    {
        float[] values = new float[GetLength()];

        values[0] = Repeticiones;
        values[1] = Amplitud;
        values[2] = Orden;
        values[3] = NivelAsistencia;
        values[4] = Fuerza;
        values[5] = TimeTotal;
        values[6] = cGauss;

        return values;
    }
    #endregion

    #region [Public Function] Asignación
    /// <summary>
    /// Establece el valor de las variables de entrada a partir de un
    /// array
    /// </summary>
    /// <param name="value"></param>
    public static void SetInputs(float[] value)
    {
        Repeticiones = (int)value[0];
        Amplitud = (int)value[1];
        Orden = (int)value[2];
        //NivelAsistencia = (float) value[3]; //No se modifican estos, ya que se
        //Fuerza = (float) value[4];          //configuran al principio
        TimeTotal = (float) value[5];
        cGauss = (float)value[6];
    }

    
#endregion
    //*********************************************************************************//
    //*********************************************************************************//
}
