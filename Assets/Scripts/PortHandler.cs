using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO.Ports;
using UnityEngine.UI;
using System;

public class PortHandler : MonoBehaviour
{

    public int kg;
    public string Port;
    public Text data;
    public Text MReceived;
    public Text PortNotif;
    

    private SerialPort sp;

    private CsvWrite csv;
    private bool Connected;
    private bool read;
    private bool bullet;

    private int count;


    private byte[] buffer;
    Graph grafas;

    // Start is called before the first frame update
    void Start()
    {
        grafas = GameObject.Find("Canvas").GetComponent<Graph>();
        csv = GameObject.Find("CSV").GetComponent<CsvWrite>();
        Connected = false;
        read = false;
        bullet = false;

        count = 0;
        buffer = new byte[2];
        try
        {
            sp = new SerialPort(Port, 9600);
        }
        catch(System.Exception)
        {
            PortNotif.text = "Porto nėra";
        }

    }

    // Update is called once per frame
    void Update()
    {
       //string d;
         if(count < 10 && sp.IsOpen && read && sp.BytesToRead > 1)
         {
            count++;
            buffer[0] = (byte)sp.ReadByte();
            buffer[1] = (byte)sp.ReadByte();
         }
         else if (sp.IsOpen && read && sp.BytesToRead > 1 )
         {
            Debug.Log("Open!");
            buffer[0] = (byte)sp.ReadByte();
            buffer[1] = (byte)sp.ReadByte();

            short bait = BitConverter.ToInt16(buffer, 0);

            data.text = bait.ToString();
            kg = (int)bait;

            if ( (bullet || kg > 700) && count < 100)
            {
                count++;
                bullet = true;
                csv.Write(kg.ToString());
                grafas.data.RemoveAt(0);
                grafas.data.Add(kg);
                grafas.Draw(grafas.data);
            } else if (count == 100)
            {
                csv.Finish();
                count++;
            }








            //if (buffer.Contains("M") && buffer.Contains("D"))
            //{
            //if (buffer.Contains("+"))
            //    d = buffer.Split('+')[1].Split('k')[0];
            //else if (buffer.Contains("-"))
            //    d = buffer.Split('-')[1].Split('k')[0];
            //else d = "0";

            //try
            //{
            //    kg = Convert.ToInt32(d.Split(',')[0]);

            //}
            //catch
            //{
            //    kg = 0;
            //}
            //Debug.Log(kg);
            //data.text = "M = " + d + "kg";
            //buffer = "";
            //}
        }
    }

    public void Connect()
    {
        if (sp.IsOpen)
        {
            sp.Write("AT");
            MReceived.text = "Connected";
            Connected = true;
            // MReceived.text = sp.ReadLine();

        }
        else
        {
            MReceived.text = "Port is not connected!";
        }
    }

    public void Read()
    {
        string Mac = "A4DA32678BDC";
        if (sp.IsOpen && Connected)
        {
            sp.Write("AT+CON"+Mac);
            read = true;
            MReceived.text = "Reading";
            // MReceived.text = sp.ReadLine();
            sp.DiscardInBuffer();
        }
        else
        {
            MReceived.text = "Port is not connected!";
        }
    }

    public void Close()
    {
        if (sp.IsOpen && Connected)
        {
            Connected = false;
            read = false;
            sp.Write("AT");
            //read = sp.ReadLine();
        }
        else
        {
            MReceived.text = "Module is not connected!";
        }
    }

    public void OpenPort()
    {
        try
        {
            sp.Open();
            PortNotif.text = "Open";
            sp.ReadTimeout = 1;
        }
        catch
        {
            PortNotif.text = "Port'o vis dar nėra!";
        }
    }


    public void ClosePort()
    {
        try
        {
            sp.Close();
            PortNotif.text = "Closed";
            sp.ReadTimeout = 0;
            Connected = false;
        }
        catch 
        {
            //PortNotif.text = "Nėra ką uždaryti";
        }
    }
}
