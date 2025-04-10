using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Line : MonoBehaviour
{
    LineRenderer linea;

    public List<Vector2> puntos = new List<Vector2>();

    Vector2 ultimo;

    public int[] indexLinePoint = new int[] {-1, -1}; //Indice inicial y final 

    

    private void Awake()
    {
        linea = GetComponent<LineRenderer>();
        
    }

    public void DibujarLinea(Vector2 Pos)
    {
        if (puntos == null)
        {
            
            DrawPoint(Pos);
            return;
        }

        if(Vector2.Distance(ultimo, Pos) >= 0.05f)
            DrawPoint(Pos);

    }

    public void SetIndexLine(int index, int value)
    {
        indexLinePoint[index] =  value;
    }

    public void DrawPoint(Vector2 pos)
    {
        puntos.Add(pos);
        linea.positionCount = puntos.Count;
        linea.SetPosition(puntos.Count - 1, pos);
        ultimo = pos;
    }

    public void UpdatePoint(int index, Vector2 pos)
    {
        linea.SetPosition(index, pos);
    }

    public void SaveLinePoint()
    {
        for(int i=0; i<linea.positionCount; i++)
            puntos[i] = linea.GetPosition(i);        
    }

    // Start is called before the first frame update
    void Start()
    {
        
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
