using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace Assets.Scripts
{
    public class Truck
    {
        public const float Width = 2;
        public const float Length = 8;
        public const float SpawnLength = Length + 6f;
        public const float ThreeSecondRule = Length * 1.5f;

        Color AIColor;

        public float Position;

        const float MaxSpeed = 10;
        const float Acceleration = 10;

        public float CurrentSpeed;
        public TruckLane Lane;

        public Truck(TruckLane lane, float position)
        {
            Lane = lane;
            Position = position;

            AIColor = Color.Lerp(Color.black, Color.white, UnityEngine.Random.Range(0.3f, 0.7f));
        }

        public void Update(int particleId)
        {
            var nextIndex = Lane.Trucks.IndexOf(this) + 1;
            var nextTruck = nextIndex < Lane.Trucks.Count ? Lane.Trucks[nextIndex] : null;

            float targetSpeed;

            // braking choices
            {
                var shouldBrake = nextTruck != null && Position + ThreeSecondRule > nextTruck.Position;

                // AI
                if (shouldBrake)
                    targetSpeed = 0f;
                else
                    targetSpeed = 1f * MaxSpeed;
            }

            CurrentSpeed = Mathf.Lerp(CurrentSpeed, targetSpeed, Acceleration * Time.deltaTime);

            Position += CurrentSpeed * Time.deltaTime;
            if (Position < 0)
                Position = 0f;

            if (nextTruck != null && Position + Length > nextTruck.Position)
            {
                nextTruck.Position = Position + Length;

                var sharedSpeed = (CurrentSpeed + nextTruck.CurrentSpeed) / 2f;
                CurrentSpeed = sharedSpeed;
                nextTruck.CurrentSpeed = sharedSpeed;

            }

            var worldPos = Lane.Start + (TruckLane.Forward * Position);

            // hit cars
            foreach (var carLane in Lane.Level.CarLanes)
            {
                var distToLane = Mathf.Abs(worldPos.x - carLane.Start.x);
                const float CarLaneTouchDist = (Truck.Length / 2f) + (Car.Width / 2f);

                if (distToLane > CarLaneTouchDist)
                    continue;

                const float CarHitDist = (Car.Length / 2f) + (Truck.Width / 2f);

                foreach (var car in carLane.Cars)
                {
                    var distToCar = Mathf.Abs(worldPos.z - (carLane.Start.z + car.Position));
                    if (distToCar > CarHitDist)
                        continue;

                    car.Hurt();
                }
            }


            TruckParticles.S.Particles[particleId].startColor = AIColor;
            TruckParticles.S.Particles[particleId].position = worldPos;
            TruckParticles.S.Particles[particleId].remainingLifetime = 10f;
            TruckParticles.S.Particles[particleId].startSize = 1f;
        }
    }
}
