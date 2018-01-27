using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace Assets.Scripts
{
    public class Prefabs : MonoBehaviour
    {
        public static Prefabs S { get; private set; }

        public Truck Truck;
        public Level Level1;

        void Awake()
        {
            S = this;
        }
    }
}
