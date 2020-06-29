using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class CsvWrite : MonoBehaviour
{

    private List<string> Data;
    [SerializeField] private string file;
    public StreamWriter writer;

    private int counter;

    // Start is called before the first frame update
    void Start()
    {
        Data = new List<string>();
        file = Application.dataPath + "/CSV/" + "FPS.csv";
        writer = new StreamWriter(file);
        counter = 0;
    }

    public void Write(string line)
    {
        //if (counter < 100)
        //{
          //  writer.WriteLine(line + "," + Time.timeSinceLevelLoad);
          //  counter++;
        //}
        //else if (counter++ == 100)
        //    Finish();
    }

    public void Finish()
    {
        writer.Flush();
        writer.Close();
    }
}
