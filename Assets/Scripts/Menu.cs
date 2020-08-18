using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Menu : MonoBehaviour
{
    [SerializeField]
    private GameObject PortRenderer;
    // Start is called before the first frame update
    void Start()
    {
        if(PortRenderer == null)
            PortRenderer = GameObject.Find("PortRenderer");
        PortRenderer.SetActive(false);
    }

    void OnGUI()
    {
        if (Event.current.Equals(Event.KeyboardEvent(KeyCode.Escape.ToString())))
            PortRenderer.SetActive(!PortRenderer.activeSelf);
    }
}
