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

        public Vector3 Position;

        public float MaxSpeed = 10;
        public float Acceleration = 100;

        public float CarSpeed;
        public Lane Lane;

        public void Reset()
        {
            Position = Lane.Start;
        }

        public void Update(int particleId)
        {
            var targetSpeed = Device.Input.Speed * MaxSpeed;
            CarSpeed = Mathf.Lerp(CarSpeed, targetSpeed, Acceleration * Time.deltaTime);

            var targetPos = Position + ( Lane.Direction * CarSpeed * Time.deltaTime);
            Position = Lane.Clamp(targetPos);

            CarParticles.S.Particles[particleId].position = Position;
            CarParticles.S.Particles[particleId].remainingLifetime = 10f;
            CarParticles.S.Particles[particleId].startSize = 1f;

        }
    }
}
