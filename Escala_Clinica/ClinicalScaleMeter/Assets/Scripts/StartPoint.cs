using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum TypePoint
{
    START,
    FINISH
}

public class StartPoint : MonoBehaviour
{
    
    public float cTime = 0f; //Tiempo actual tocando el trigger
    public bool IsTouching = false;
    public TypePoint type;
    


    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.tag == "Player")
        {
            IsTouching = true;
            cTime = 0f;
        }
    }


    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.tag == "Player")
        {
            IsTouching = false;
            cTime = 0f;
        }
    }

}
