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
        public Level CurrentLevel;

        public AudioSource AudioSource;

        public AudioClip[] AllHonksSfk;
        public AudioClip KillSfk;


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
                var lobby = TinyCoro.SpawnNext(Canvas.S.RunLobby);
                yield return TinyCoro.Join(lobby);

                CurrentLevel = Level.Instantiate<Level>(Prefabs.S.Level1);

                var runLevel = TinyCoro.SpawnNext(CurrentLevel.Run);
                yield return TinyCoro.Join(runLevel);

                foreach (var device in Devices.Values)
                {
                    device.SetRole(DeviceRole.Wait);
                }

                var winner = TinyCoro.SpawnNext(Canvas.S.RunWinner);
                yield return TinyCoro.Join(winner);

                Level.Destroy(CurrentLevel.gameObject);
                CurrentLevel = null;
            }
        }

        void OnConnect(int device_id)
        {
            if (Devices.ContainsKey(device_id) == false)
                Devices.Add(device_id, new Device(device_id));

            var device = Devices[device_id];
            device.Connected = true;
            device.SetRole(device.Role);
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
                case "honk":
                    OnHonk(device);
                    break;
                case "ready":
                    OnReady(device, (string)message[DataKey]);
                    break;
                case "setlights":
                    OnSetLights(device, (bool)message[DataKey]);
                    break;
                case "calltrain":
                    OnCallTrain(device);
                    break;
                default:
                    Debug.LogWarning("message type " + msgType);
                    break;
            }

            Logs.Log(Newtonsoft.Json.JsonConvert.SerializeObject(message));
        }

        void OnHonk(Device device)
        {
            if(CurrentLevel != null && CurrentLevel.Cars.ContainsKey(device))
            {
                CurrentLevel.Cars[device].Honk();
            }
        }

        void OnMove(Device device, float acceleration)
        {
            device.Input.Speed = acceleration;
        }

        void OnReady(Device device, string name)
        {
            device.Name = string.IsNullOrEmpty(name) ? null : name;

            if (device.Role == DeviceRole.Wait)
                device.SetRole(DeviceRole.Ready);
        }

        void OnSetLights(Device device, bool greenToGo)
        {
            if (CurrentLevel == null)
                return;

            if (greenToGo)
                CurrentLevel.SetTrafficMode(TrafficMode.CarGo);
            else
                CurrentLevel.SetTrafficMode(TrafficMode.CarSlow);
        }

        void OnCallTrain(Device device)
        {

        }

        void Update()
        {
            TinyCoro.StepAllCoros();
        }
    }
}