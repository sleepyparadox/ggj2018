﻿using NDream.AirConsole;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace Assets.Scripts
{
    public class Device
    {
        public readonly int DeviceId;
        public bool Connected;
        public InputMap Input;
        public Color Color;
        public DeviceRole Role { get; private set;}
        public string Name;
        public bool Ready;
        public int Score;

        public float PlayerPitch;
        public Device(int deviceId)
        {
            DeviceId = deviceId;
            Input = new InputMap();
            Color = Color.Lerp(Color.blue, Color.green, UnityEngine.Random.Range(0f, 1f));

            PlayerPitch = UnityEngine.Random.Range(0.8f, 1.2f);
        }

        public void SetRole(DeviceRole role)
        {
            Role = role;
            var msg = new
            {
                type = "setrole",
                data = role.ToString()
            };

            AirConsole.instance.Message(DeviceId, msg);
        }

        public class InputMap
        {
            public float Speed;
            public bool Honking;

            public void Clear()
            {
                Speed = 0f;
                Honking = false;
            }
        }
    }

    public enum DeviceRole
    {
        Wait,
        Ready,
        Conductor,
        Car,
    }
}
