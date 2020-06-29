using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Graph : MonoBehaviour
{

    [SerializeField] public float graphWidth;
    [SerializeField] public float graphHeight;

    LineRenderer newLineRenderer;
    public int vertexAmount = 100;
    [SerializeField] float xInterval;

    [SerializeField] GameObject parentCanvas;
    [SerializeField] Transform line;
   public List<int> data = new List<int>();
   public  int maxV=40000;
    [SerializeField] int timer;
    PortHandler portdata;
    // Use this for initialization
    [System.Obsolete]
    
    void Start()
    {
        Debug.LogError("This message will make the console appear in Development Builds");
        portdata = GameObject.Find("EventSystem").GetComponent<PortHandler>();
        parentCanvas = GameObject.Find("Canvas");
        line = transform.Find("Linerenderer");
        graphWidth = transform.Find("Linerenderer").GetComponent<RectTransform>().rect.width;
        graphHeight = transform.Find("Linerenderer").GetComponent<RectTransform>().rect.height;
        newLineRenderer = GetComponentInChildren<LineRenderer>();
        newLineRenderer.SetVertexCount(vertexAmount);

        xInterval = graphWidth / vertexAmount;

        for(int i=0;i<101;i++)
           data.Add( 0);
        
    }

    void FixedUpdate()
    {
                                                                                   //   Draw(data);
                                                                                     // data.RemoveAt(0);
        //  int num = Mathf.Abs(data[data.Count-1] + Random.Range(-10, 10));
      //  int num = portdata.kg;
       // int num = Mathf.Abs((int)Input.mousePosition.y/10);

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
                                                                                    // data.Add(num);
    }

    public void Draw(List<int> decibels)
    {
        if (decibels.Count == 0)
            return;
        for (int i = 0; i < vertexAmount && i < decibels.Count; i++)
        {
            int _index = i;
            float y = decibels[_index] * (graphHeight / maxV ); //(Divide grapheight with the maximum value of decibels.
            float x = i * xInterval;
            newLineRenderer.SetPosition(i, new Vector3(x - graphWidth / 2, y - graphHeight / 2, 0));

        }
    }

}