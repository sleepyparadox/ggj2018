﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace Assets.Scripts
{
    public class Car 
    {
        public const float Width = 2;
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

        float? KillingDt;
        const float MaxKillingDuration = 1f;
        public bool Dead { get; private set; }

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
            if(KillingDt.HasValue)
            {
                KillingUpdate(particleId);
                return;
            }

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

                Hurt();
                //nextCar.Hurt();
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
            CarParticles.S.Particles[particleId].rotation3D = Vector3.zero;

        }

        void KillingUpdate(int particleId)
        {
            const float JumpHeight = 15f;
            const float SpinDist = 360 * 4f;
            const float BounceBackDist = MaxSpeed * MaxKillingDuration * 2f;

            KillingDt += Time.deltaTime;
            var normalizedDeath = Mathf.Clamp01(KillingDt.Value / MaxKillingDuration);
            var sineDeath = Mathf.Sin(normalizedDeath * Mathf.PI / 2f);

            var pos = Position - (BounceBackDist * sineDeath);

            var lanePos = Lane.Start + (CarLane.Forward * Position);

            var jumpOffset = (Vector3.up * JumpHeight * sineDeath);
            var bounceBack = (CarLane.Forward * -1f * BounceBackDist * sineDeath);

            CarParticles.S.Particles[particleId].position = lanePos + jumpOffset + bounceBack;

            CarParticles.S.Particles[particleId].startColor = Color.red;
            CarParticles.S.Particles[particleId].remainingLifetime = 10f;
            CarParticles.S.Particles[particleId].startSize = 1f - sineDeath;
            CarParticles.S.Particles[particleId].rotation3D = new Vector3(0, sineDeath * SpinDist, 0);

            if (KillingDt > MaxKillingDuration)
            {
                Dead = true;
                return;
            }
        }

        public void Hurt()
        {
            HurtFor = MaxHurtFor;
        }

        public void Kill()
        {
            if (KillingDt.HasValue)
                return;

            KillingDt = 0f;   
        }
    }
}
