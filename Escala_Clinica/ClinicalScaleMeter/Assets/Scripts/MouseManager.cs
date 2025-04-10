using UnityEngine;


/// <summary>
/// Gestor del puntero del raton encargado de esconderlo de la pantalla
/// para evitar distracciones
/// </summary>
public class MouseManager : MonoBehaviour
{

    //************************************Variables************************************//
    //*********************************************************************************//
    #region [Variables] Tiempos    
    public readonly float timeMouse = 2f; //Tiempo para que desaparezca el raton
    private float timeToHideMouse = 0f; //Tiempo actual
    #endregion
    //*********************************************************************************//
    //*********************************************************************************//



    //************************Inicialización y bucle principal*************************//
    //*********************************************************************************//
    #region [Unity Functions] Bucle principal
    void Update()
    {
        timeToHideMouse += Time.deltaTime;

        //Cuando pasan dos segundos, el puntero desaparece
        if (Cursor.visible)
        {
            if (timeToHideMouse >= timeMouse)
            {
                Cursor.visible = mouseMoved();
                timeToHideMouse = 0;
            }
        }
        else
        {
            //Al mover el raton, vuelve a aparecer
            Cursor.visible = mouseMoved();
            timeToHideMouse = 0;
        }
    }
    #endregion
    //*********************************************************************************//
    //*********************************************************************************//



    //********************************Control de mouse*********************************//
    //*********************************************************************************//
    #region [Public Functions] Mouse
    private bool mouseMoved()
    {
        return (Input.GetAxis("Mouse X") != 0) || (Input.GetAxis("Mouse Y") != 0);
    }
    #endregion
    //*********************************************************************************//
    //*********************************************************************************//
}
