using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using NDream.AirConsole;
using Newtonsoft.Json.Linq;

public class MainApp : MonoBehaviour
{

    void Start()
    {
        AirConsole.instance.onMessage += OnMessage;
    }

    void OnMessage(int from, JToken data)
    {
        AirConsole.instance.Message(from, "Full of pixels!");
    }

}
