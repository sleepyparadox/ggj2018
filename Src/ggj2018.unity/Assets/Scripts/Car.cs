using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace Assets.Scripts
{
    public class Car 
    {
        public Device Device;

        public float Position;

        public float MaxSpeed = 10;
        public float Acceleration = 100;

        public float CarSpeed;
        public Lane Lane;

        public Car(Lane lane, float position)
        {
            Lane = lane;
            Position = position;
        }

        public void Update(int particleId)
        {
            float targetSpeed;

            if(Device != null)
            {
                // Button
                targetSpeed = Device.Input.Speed * MaxSpeed;
            }
            else
            {
                // AI
                targetSpeed = 1 * MaxSpeed;
            }

            CarSpeed = Mathf.Lerp(CarSpeed, targetSpeed, Acceleration * Time.deltaTime);

            var targetPos = Position + ( CarSpeed * Time.deltaTime);
            if (targetPos < 0 || targetPos > Lane.Level.LaneLength)
                targetPos = 0f;

            Position = targetPos;

            CarParticles.S.Particles[particleId].startColor = Device != null ? Device.Color : Color.grey;
            CarParticles.S.Particles[particleId].position = Lane.Start + (Lane.Forward * Position);
            CarParticles.S.Particles[particleId].remainingLifetime = 10f;
            CarParticles.S.Particles[particleId].startSize = 1f;
        }
    }
}
