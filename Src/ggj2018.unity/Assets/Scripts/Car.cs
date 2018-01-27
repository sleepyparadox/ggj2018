using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace Assets.Scripts
{
    public class Car 
    {
        public const float Length = 4;
        public const float SpawnLength = Length + 6f;

        public Device Device;

        public float Position;

        const float MaxSpeed = 10;
        const float Acceleration = 60;

        public float CarSpeed;
        public Lane Lane;

        Color DefaultColor;
        float DefaultInputSpeed;

        float HurtFor = 0f;
        const float MaxHurtFor = 0.1f;

        public Car(Lane lane, float position)
        {
            Lane = lane;
            Position = position;

            var defaultValue = UnityEngine.Random.Range(0f, 1f);

            DefaultInputSpeed = Mathf.Lerp(0.5f, 1f, defaultValue);
            DefaultColor = Color.Lerp(Color.black, Color.white, Mathf.Lerp(0.1f, 0.7f, defaultValue));
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
                if (HurtFor > 0f)
                    targetSpeed = 0f;
                else
                    targetSpeed = DefaultInputSpeed * MaxSpeed;
            }

            CarSpeed = Mathf.Lerp(CarSpeed, targetSpeed, Acceleration * Time.deltaTime);

            Position += CarSpeed * Time.deltaTime;
            if (Position < 0)
                Position = 0f;

            var nextIndex = Lane.Cars.IndexOf(this) + 1;
            var nextCar = nextIndex < Lane.Cars.Count ? Lane.Cars[nextIndex] : null;

            if(nextCar != null && Position + Length > nextCar.Position)
            {
                Debug.Log("bump! " + (nextIndex - 1) + " and " + nextIndex);

                nextCar.Position = Position + Length;

                var sharedSpeed = (CarSpeed + nextCar.CarSpeed) / 2f;
                CarSpeed = sharedSpeed;
                nextCar.CarSpeed = sharedSpeed;

                HurtFor = MaxHurtFor;
                //nextCar.HurtFor = MaxHurtFor;
            }

            var color = Device != null ? Device.Color : DefaultColor;
            if(HurtFor > 0f)
            {
                HurtFor -= Time.deltaTime;
                color = Color.Lerp(color, Color.red, HurtFor / MaxHurtFor);
            }

            CarParticles.S.Particles[particleId].startColor = color;
            CarParticles.S.Particles[particleId].position = Lane.Start + (Lane.Forward * Position);
            CarParticles.S.Particles[particleId].remainingLifetime = 10f;
            CarParticles.S.Particles[particleId].startSize = 1f;
        }
    }
}
