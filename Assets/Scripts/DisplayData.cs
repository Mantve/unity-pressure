using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO.Ports;
using UnityEngine.UI;


public class DisplayData : MonoBehaviour
{

    SerialPort sp = new SerialPort("COM7",9600);

    public Text data;

    // Start is called before the first frame update
    void Start()
    {
        sp.Open();    
        //sp.Close();
        sp.ReadTimeout = 1;
    }

    // Update is called once per frame
    void Update()
    {
         if (sp.IsOpen)
         {
         Debug.Log("Open!");

            try
            {
                string read = sp.ReadLine();
                data.text = read;
                Debug.Log(read);
            }
            catch(System.Exception)
            {

            }
        }
    }
}
