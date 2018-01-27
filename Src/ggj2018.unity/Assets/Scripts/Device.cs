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

        public Device(int deviceId)
        {
            DeviceId = deviceId;
            Input = new InputMap();
            Color = Color.red;
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
}
