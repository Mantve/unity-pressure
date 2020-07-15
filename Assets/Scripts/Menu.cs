using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Menu : MonoBehaviour
{
    [SerializeField] GameObject PortRenderer ;
    // Start is called before the first frame update
    void Start()
    {
        PortRenderer = GameObject.Find("PortRenderer");
    }

    // Update is called once per frame
    void Update()
    {
        if(Input.GetButtonDown("Cancel"))
        {
            PortRenderer.active = !PortRenderer.active;
        }
    }
}
