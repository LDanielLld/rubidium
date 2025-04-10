using UnityEngine;


/// <summary>
/// Clase encargada de gestionar toda la interaccion del jugador con los elementos que 
/// se encuentran dentro de la actividad, desde el movimiento o animaciones del jugador hasta actuar
/// con otros elementos visuales
/// </summary>
public class PlayerController : MonoBehaviour
{   
    //Instancia compartida
    public static PlayerController sharedInstance;

    

    //************************************Variables************************************//
    //*********************************************************************************//
    #region [Variables] Movimiento
    public float speed; //Velocidad del personaje
    #endregion
    //*********************************************************************************//
    //*********************************************************************************//



    //*********************************Inicialización**********************************//
    //*********************************************************************************//
    #region [Unity Function] Inicialización
    private void Awake()
    {
        //Instancia compartida
        if (sharedInstance == null)
            sharedInstance = this;   
    }    
    #endregion
    //*********************************************************************************//
    //*********************************************************************************//



    //***************************Control visual del jugador****************************//
    //*********************************************************************************//
    #region [Public Function] Movimiento del jugador
    /// <summary>
    /// Mueve el jugador utilizando el teclado
    /// </summary>
    public void MovementWithKeyboard()
    {
        //Calculamos el nuevo punto donde hay que ir en base a la variable destino
        if (Input.GetKey(KeyCode.UpArrow) || Input.GetKey(KeyCode.W))
            transform.Translate(Vector3.up * speed * Time.deltaTime);
        if (Input.GetKey(KeyCode.RightArrow) || Input.GetKey(KeyCode.D))
            transform.Translate(Vector3.right * speed * Time.deltaTime);
        if (Input.GetKey(KeyCode.DownArrow) || Input.GetKey(KeyCode.S))
            transform.Translate(Vector3.down * speed * Time.deltaTime);
        if (Input.GetKey(KeyCode.LeftArrow) || Input.GetKey(KeyCode.A))
            transform.Translate(Vector3.left * speed * Time.deltaTime);        
    }    
    #endregion
    //*********************************************************************************//
    //*********************************************************************************//

}
