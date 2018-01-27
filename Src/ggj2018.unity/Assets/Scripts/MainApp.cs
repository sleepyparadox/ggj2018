using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using NDream.AirConsole;
using Newtonsoft.Json.Linq;
using System;

namespace Assets.Scripts
{
    public class MainApp : MonoBehaviour
    {
        public static MainApp S { get; private set; }
        public Dictionary<int, Device> Devices = new Dictionary<int, Device>();

        void Awake()
        {
            S = this;
        }

        void Start()
        {
            if(AirConsole.instance != null)
            {
                AirConsole.instance.onConnect += OnConnect;
                AirConsole.instance.onDisconnect += OnDisconnect;
                AirConsole.instance.onMessage += OnMessage;
            }
          
            TinyCoro.SpawnNext(Run);
        }

        public IEnumerator Run()
        {
            while (true)
            {
                var level = Level.Instantiate<Level>(Prefabs.S.Level1);

                var runLevel = TinyCoro.SpawnNext(level.Run);
                yield return TinyCoro.Join(runLevel);

                Level.Destroy(level);
            }
        }

        void OnConnect(int device_id)
        {
            if (Devices.ContainsKey(device_id) == false)
                Devices.Add(device_id, new Device(device_id));

            Devices[device_id].Connected = true;
        }

        void OnDisconnect(int device_id)
        {
            var device = Devices[device_id];
            
            device.Connected = false;
            device.Input.Clear();
        }

        void OnMessage(int device_id, JToken message)
        {
            const string TypeKey = "type";
            const string DataKey = "data";

            var device = Devices[device_id];
            var msgType = (string)message[TypeKey];
            if (msgType == null)
                return;

            switch(msgType)
            {
                case "move":
                    OnMove(device, (int)message[DataKey]);
                    break;
                default:
                    throw new NotImplementedException("message type " + msgType);
            }

            Logs.Log(Newtonsoft.Json.JsonConvert.SerializeObject(message));
        }

        void OnMove(Device device, float acceleration)
        {
            device.Input.Speed = acceleration;
        }

        void Update()
        {
            TinyCoro.StepAllCoros();
        }
    }
}