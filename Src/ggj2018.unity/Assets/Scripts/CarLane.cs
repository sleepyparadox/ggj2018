using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace Assets.Scripts
{
    [Serializable]
    public class CarLane
    {
        public static Vector3 Forward = new Vector3(0, 0, 1);
        public Vector3 Start;
        public Vector3 End;
        public List<Car> Cars;
        public Level Level;

        const int MaxCars = 20;

        public CarLane(Level level, Transform t)
        {
            Start = t.TransformPoint(new Vector3(0, 0, -0.5f));
            End = Start + (Forward * level.LaneLength);
            Level = level;
            Cars = new List<Car>();
        }

        public void Update()
        {
            // Spawn Cars
            if (Cars.Count < MaxCars
                && ((Cars.Count == 0 || Cars.First().Position > Car.SpawnLength)))
            {
                var car = new Car(this, 0);
                Cars.Insert(0, car);
            }

            // Update Cars
            foreach (var car in Cars)
            {
                car.Update(CarParticles.S.ParticleCount);
                CarParticles.S.ParticleCount++;
            }

            // Cleanup Cars
            var carsToRemove = Cars.Where(c => c.Position > Level.LaneLength).ToList();
            foreach (var car in carsToRemove)
            {
                Cars.Remove(car);
                Level.CarDeleted(car);
            }
        }
    }
}
