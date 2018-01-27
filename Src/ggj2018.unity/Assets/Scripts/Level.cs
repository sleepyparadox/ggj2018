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

        public float LaneLength = 10;

        List<Lane> _lanes = new List<Lane>();
        Dictionary<Device, Car> _cars = new Dictionary<Device, Car>();

        void Awake()
        {
            for (int i = 0; i < LaneParent.childCount; i++)
            {
                var laneChild = LaneParent.GetChild(i);
                _lanes.Add(new Lane(this, laneChild));
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
                foreach (var lane in _lanes)
                    lane.Update();

                yield return null;
            }
        }
        
        void OnDrawGizmos()
        {
            for (int i = 0; i < LaneParent.childCount; i++)
            {
                var laneChild = LaneParent.GetChild(i);
                Debug.DrawLine(laneChild.position, laneChild.position + (Lane.Forward * LaneLength) , Color.green);
            }

            for (int i = 0; i < TrafficLightParent.childCount; i++)
            {
                var trafficLightChild = TrafficLightParent.GetChild(i);
                Debug.DrawLine(trafficLightChild.position, trafficLightChild.position + (Vector3.up * LaneLength), Color.red);
            }
        }
    }
}
