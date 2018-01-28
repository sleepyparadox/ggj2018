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
        const float MaxTimePerLevel = 60f;

        public Transform LaneParent;
        public Transform TrafficLightParent;
        public Transform TrucksParent;

        public float LaneLength = 100;

        public List<CarLane> CarLanes = new List<CarLane>();
        public List<TruckLane> TruckLanes = new List<TruckLane>();
        public List<TrafficLight> TrafficLights = new List<TrafficLight>();

        Dictionary<Device, Car> _cars = new Dictionary<Device, Car>();

        TrafficMode _trafficMode;
        float _trafficModeElapsed;

        const float MaxTimePerTrafficMode = 1f;

        public int CarScore = 0;
        public int ConductorScore = 0;

        float LevelTimeElapsed = 0f;
        
        void Awake()
        {
            for (int i = 0; i < LaneParent.childCount; i++)
            {
                var laneChild = LaneParent.GetChild(i);
                laneChild.gameObject.SetActive(false);

                CarLanes.Add(new CarLane(this, laneChild));
            }

            for (int i = 0; i < TrucksParent.childCount; i++)
            {
                var laneChild = TrucksParent.GetChild(i);
                laneChild.gameObject.SetActive(false);

                TruckLanes.Add(new TruckLane(this, laneChild));
            }

            for (int i = 0; i < TrafficLightParent.childCount; i++)
            {
                var lightChild = TrafficLightParent.GetChild(i);
                lightChild.gameObject.SetActive(false);

                var light = TrafficLight.Instantiate<TrafficLight>(Prefabs.S.TrafficLight);
                light.transform.position = lightChild.position;
                TrafficLights.Add(light);
            }
        }

        public void SetTrafficMode(TrafficMode mode)
        {
            _trafficMode = mode;
            _trafficModeElapsed = 0f;
            foreach (var light in TrafficLights)
                light.SetMode(mode);
        }

        public IEnumerator Run()
        {
            Canvas.S.SetScreen(Screen.Game);

            // Choose a conductor
            var conductor = MainApp.S.Devices.Values.FirstOrDefault(d => d.Connected && d.Role == DeviceRole.Ready);
            if (conductor != null)
                conductor.SetRole(DeviceRole.Conductor);

            while (LevelTimeElapsed < MaxTimePerLevel  && HasConductor())
            {
                LevelTimeElapsed += Time.deltaTime;

                // Update Traffic modes
                _trafficModeElapsed += Time.deltaTime;
                if(_trafficModeElapsed > MaxTimePerTrafficMode 
                    && (_trafficMode == TrafficMode.CarSlow))
                {
                    SetTrafficMode(TrafficMode.TruckGo);
                }
                
                // Update car particles
                CarParticles.S.ParticleCount = 0;
                foreach (var lane in CarLanes)
                    lane.Update();

                // Update truck particles
                TruckParticles.S.ParticleCount = 0;
                foreach (var lane in TruckLanes)
                    lane.Update();

                RepopulateDevices();

                var timeRemaining = MaxTimePerLevel - LevelTimeElapsed;
                Canvas.S.UpdateScores(CarScore, ConductorScore, timeRemaining);

                yield return null;
            }
        }

        bool HasConductor()
        {
            return MainApp.S.Devices.Values.Any(d => d.Connected && d.Role == DeviceRole.Conductor);
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

                if (_cars.ContainsKey(device) || device.Connected == false || device.Role != DeviceRole.Ready)
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

                pair.Key.SetRole(DeviceRole.Ready);
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
