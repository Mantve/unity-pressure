using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Graph : MonoBehaviour
{

    [SerializeField] public float graphWidth;
    [SerializeField] public float graphHeight;

    LineRenderer newLineRenderer;
    List<int> decibels;
    int vertexAmount = 50;
    [SerializeField] float xInterval;

    [SerializeField] GameObject parentCanvas;
    [SerializeField] Transform line;
    [SerializeField] List<int> data = new List<int>();
   public  int maxV=100;
    [SerializeField] int timer;

    // Use this for initialization
    [System.Obsolete]
    
    void Start()
    {
        parentCanvas = GameObject.Find("Canvas");
        line = transform.Find("Linerenderer");
        graphWidth = transform.Find("Linerenderer").GetComponent<RectTransform>().rect.width;
        graphHeight = transform.Find("Linerenderer").GetComponent<RectTransform>().rect.height;
        newLineRenderer = GetComponentInChildren<LineRenderer>();
        newLineRenderer.SetVertexCount(vertexAmount);

        xInterval = graphWidth / vertexAmount;

        for(int i=0;i<50;i++)
            data.Add( 10);
        
    }

    void Update()
    {
        Draw(data);
      data.RemoveAt(0);
        int num = Mathf.Abs(data[data.Count-1] + Random.Range(-10, 10));
      /*  timer--;
        if (num > maxV)
        {
            maxV = num;
            timer = 50;
        }
        if(timer<0)
        {
             maxV = Mathf.Max(data.ToArray());
            for(int i=0;i<data.Count;i++)
            {
                if (maxV == data[i])
                {
                    timer = i;
                    i = data.Count;
                }
            }
            
        }*/
        data.Add(num);
    }

    public void Draw(List<int> decibels)
    {
        if (decibels.Count == 0)
            return;

        float x = 0;

        for (int i = 0; i < vertexAmount && i < decibels.Count; i++)
        {
            int _index = decibels.Count - i - 1;

            float y = decibels[_index] * (graphHeight / maxV ); //(Divide grapheight with the maximum value of decibels.
            x = i * xInterval;

            newLineRenderer.SetPosition(i, new Vector3(x - graphWidth / 2, y - graphHeight / 2, 0));

        }
    }

}