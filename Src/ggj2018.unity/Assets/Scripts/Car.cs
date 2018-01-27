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
        public const float ThreeSecondRule = Length * 1.5f;

        public Device Device;

        public float Position;

        const float MaxSpeed = 10;
        const float Acceleration = 10;

        public float CurrentSpeed;
        public CarLane Lane;

        Color AIColor;
        float AIBreakElapsed;
        float AIBreakFor;
        float AiBreakCycle;


        float HurtFor = 0f;
        const float MaxHurtFor = 0.25f;

        public object Log { get; private set; }

        public Car(CarLane lane, float position)
        {
            Lane = lane;
            Position = position;

            // Ai breaking
            var aiBrakeFrequence = UnityEngine.Random.Range(0f, 1f);

            AiBreakCycle = Mathf.Lerp(2.4f, 12f, aiBrakeFrequence);
            AIBreakFor = Mathf.Lerp(1f, 0.2f, aiBrakeFrequence);
            AIBreakElapsed = AIBreakFor;

            AIColor = Color.Lerp(Color.black, Color.white, UnityEngine.Random.Range(0.3f, 0.7f));
        }

        public void Update(int particleId)
        {
            var nextIndex = Lane.Cars.IndexOf(this) + 1;
            var nextCar = nextIndex < Lane.Cars.Count ? Lane.Cars[nextIndex] : null;

            float targetSpeed;

            if(Device != null)
            {
                // Button
                targetSpeed = Device.Input.Speed * MaxSpeed;
            }
            else
            {
                var shouldBrake = nextCar != null && Position + ThreeSecondRule > nextCar.Position;
                var hurt = HurtFor > 0f;

                AIBreakElapsed += Time.deltaTime;
                if (AIBreakElapsed > AiBreakCycle)
                    AIBreakElapsed -= AiBreakCycle;
                var aiBraking = AIBreakElapsed < AIBreakFor;

                // AI
                if (shouldBrake || hurt || aiBraking)
                    targetSpeed =  0f;
                else
                    targetSpeed = 1f * MaxSpeed;
            }

            CurrentSpeed = Mathf.Lerp(CurrentSpeed, targetSpeed, Acceleration * Time.deltaTime);

            Position += CurrentSpeed * Time.deltaTime;
            if (Position < 0)
                Position = 0f;

            if(nextCar != null && Position + Length > nextCar.Position)
            {
                nextCar.Position = Position + Length;

                var sharedSpeed = (CurrentSpeed + nextCar.CurrentSpeed) / 2f;
                CurrentSpeed = sharedSpeed;
                nextCar.CurrentSpeed = sharedSpeed;

                HurtFor = MaxHurtFor;
                //nextCar.HurtFor = MaxHurtFor;
            }

            var color = Device != null ? Device.Color : AIColor;
            if(HurtFor > 0f)
            {
                HurtFor -= Time.deltaTime;
                color = Color.Lerp(color, Color.red, HurtFor / MaxHurtFor);
            }

            CarParticles.S.Particles[particleId].startColor = color;
            CarParticles.S.Particles[particleId].position = Lane.Start + (CarLane.Forward * Position);
            CarParticles.S.Particles[particleId].remainingLifetime = 10f;
            CarParticles.S.Particles[particleId].startSize = 1f;
        }
    }
}
