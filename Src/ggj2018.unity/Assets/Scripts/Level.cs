using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace Assets.Scripts
{
    public class Level : MonoBehaviour
    {
        public List<Transform> LaneObjects;
        public Transform CarsParent;

        List<Lane> _lanes = new List<Lane>();
        Dictionary<Device, Car> _cars = new Dictionary<Device, Car>();

        void Awake()
        {
            foreach (var laneObject in LaneObjects)
            {
                _lanes.Add(new Lane(laneObject));
                laneObject.gameObject.SetActive(false);
            }
        }

        public IEnumerator Run()
        {
            while(true)
            {
                RespawnCars();
                UpdateCars();
                yield return null;
            }
        }

        void RespawnCars()
        {
            foreach (var device in MainApp.S.Devices.Values)
            {
                if (_cars.ContainsKey(device))
                    continue;

                var car = new Car();
                car.Device = device;
                car.Lane = _lanes[UnityEngine.Random.Range(0, _lanes.Count)];
                car.Reset();
                _cars.Add(device, car);
            }
        }

        void UpdateCars()
        {
            CarParticles.S.ParticleCount = 0;
            foreach (var car in _cars.Values)
            {
                car.Update(CarParticles.S.ParticleCount);
                CarParticles.S.ParticleCount++;
            }
        }
    }
}
