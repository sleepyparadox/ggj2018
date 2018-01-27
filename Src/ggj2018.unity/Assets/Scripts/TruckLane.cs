using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace Assets.Scripts
{
    public class TruckLane
    {
        public static Vector3 Forward = Vector3.right;

        public Vector3 Start;
        public Vector3 End;
        public List<Truck> Trucks;
        public Level Level;

        const int MaxTrucks = 20;

        public TruckLane(Level level, Transform t)
        {
            Start = t.TransformPoint(new Vector3(0, 0, -0.5f));
            End = Start + (Forward * level.LaneLength);
            Level = level;
            Trucks = new List<Truck>();
        }

        public void Update()
        {
            // Spawn Trucks
            if (Trucks.Count < MaxTrucks
                && ((Trucks.Count == 0 || Trucks.First().Position > Truck.SpawnLength)))
            {
                var car = new Truck(this, 0);
                Trucks.Insert(0, car);
            }

            // Update Trucks
            foreach (var car in Trucks)
            {
                car.Update(TruckParticles.S.ParticleCount);
                TruckParticles.S.ParticleCount++;
            }

            // Cleanup Trucks
            var trucksToRemove = Trucks.Where(t => t.Position > Level.LaneLength).ToList();
            foreach (var truck in trucksToRemove)
            {
                Trucks.Remove(truck);
            }
        }
    }
}
