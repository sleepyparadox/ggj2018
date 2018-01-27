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
        public Transform LaneParent;
        public Transform TrafficLightParent;
        public Transform TrucksParent;

        public float LaneLength = 100;

        public List<CarLane> CarLanes = new List<CarLane>();
        public List<TruckLane> TruckLanes = new List<TruckLane>();
        Dictionary<Device, Car> _cars = new Dictionary<Device, Car>();

        void Awake()
        {
            for (int i = 0; i < LaneParent.childCount; i++)
            {
                var laneChild = LaneParent.GetChild(i);
                CarLanes.Add(new CarLane(this, laneChild));
                laneChild.gameObject.SetActive(false);
            }

            for (int i = 0; i < TrucksParent.childCount; i++)
            {
                var laneChild = TrucksParent.GetChild(i);
                TruckLanes.Add(new TruckLane(this, laneChild));
                laneChild.gameObject.SetActive(false);
            }
        }

        public IEnumerator Run()
        {
            while(true)
            {
                // Todo give players cars
                
                // Update car particles
                CarParticles.S.ParticleCount = 0;
                foreach (var lane in CarLanes)
                    lane.Update();

                // Update truck particles
                TruckParticles.S.ParticleCount = 0;
                foreach (var lane in TruckLanes)
                    lane.Update();

                RepopulateDevices();

                yield return null;
            }
        }
        
        void RepopulateDevices()
        {
            foreach (var device in MainApp.S.Devices.Values)
            {
                if(device.Connected == false && _cars.ContainsKey(device))
                {
                    // Cleanup disconnected car
                    _cars[device].Device = null;
                    _cars.Remove(device);
                }

                if (_cars.ContainsKey(device) || device.Connected == false)
                    continue;

                const float MinDistance = Car.SpawnLength * 3f;
                var suitableHost = CarLanes.SelectMany(l => l.Cars)
                    .OrderBy(c => c.Position)
                    .Where(c => c.Position >= MinDistance && c.Device == null)
                    .FirstOrDefault();

                if (suitableHost == null)
                {
                    Debug.Log("no car found for " + device.DeviceId);
                    continue;
                }

                Debug.Log("car found for " + device.DeviceId);
                suitableHost.Device = device;
                device.SetRole(DeviceRole.Car);
                _cars.Add(device, suitableHost);
            }
        }

        public void CarDeleted(Car car)
        {
            if(_cars.Any(p => p.Value == car))
            {
                var pair = _cars.First(p => p.Value == car);

                pair.Key.SetRole(DeviceRole.Wait);
                _cars.Remove(pair.Key);
            }
        }

        void OnDrawGizmos()
        {
            for (int i = 0; i < LaneParent.childCount; i++)
            {
                var laneChild = LaneParent.GetChild(i);
                Debug.DrawLine(laneChild.position, laneChild.position + (CarLane.Forward * LaneLength) , Color.green);
            }

            for (int i = 0; i < TrucksParent.childCount; i++)
            {
                var truckChild = TrucksParent.GetChild(i);
                Debug.DrawLine(truckChild.position, truckChild.position + (Vector3.right * LaneLength), Color.red);
            }

            for (int i = 0; i < TrafficLightParent.childCount; i++)
            {
                var trafficLightChild = TrafficLightParent.GetChild(i);
                Debug.DrawLine(trafficLightChild.position, trafficLightChild.position + (Vector3.up * LaneLength), Color.red);
            }

           
        }
    }
}
