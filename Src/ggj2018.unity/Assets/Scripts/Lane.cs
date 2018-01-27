using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace Assets.Scripts
{
    [Serializable]
    public class Lane
    {
        public static Vector3 Forward = new Vector3(0, 0, 1);
        public Vector3 Start;
        public Vector3 End;
        public List<Car> Cars = new List<Car>();
        public Level Level;

        public Lane(Level level, Transform t)
        {
            Start = t.TransformPoint(new Vector3(0, 0, -0.5f));
            End = Start + (Forward * level.LaneLength);
            Level = level;

            SpawnDefaultCars();
        }

        public void SpawnDefaultCars()
        {
            for (var position = 0f; position < Level.LaneLength; position += 5f)
            {
                var car = new Car(this, position);
                Cars.Add(car);
            }
        }

        public void Update()
        {
            foreach (var car in Cars)
            {
                car.Update(CarParticles.S.ParticleCount);
                CarParticles.S.ParticleCount++;
            }
        }
    }
}
