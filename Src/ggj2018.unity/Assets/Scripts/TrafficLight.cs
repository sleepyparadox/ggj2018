using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace Assets.Scripts
{
    public class TrafficLight : MonoBehaviour
    {
        public Material RedMaterial;
        public Material YellowMaterial;
        public Material GreenMaterial;
        public Renderer LightRender;

        public TrafficMode Mode { get; private set; }

        public void SetMode(TrafficMode mode)
        {
            if (mode == TrafficMode.CarGo)
                LightRender.sharedMaterial = GreenMaterial;
            if ( mode == TrafficMode.CarSlow)
                LightRender.sharedMaterial = YellowMaterial;
            if (mode == TrafficMode.TruckGo || mode == TrafficMode.TruckSlow)
                LightRender.sharedMaterial = RedMaterial;
        }
    }

    public enum TrafficMode
    {
        CarGo,
        CarSlow,
        TruckGo,
        TruckSlow,
        COUNT
    }
}
