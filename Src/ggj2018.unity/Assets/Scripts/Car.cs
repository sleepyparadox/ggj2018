using System;
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
        public const float ThreeSecondRule = Length * 2f;
        public const float OneSecondRule = Length * 0.5f;

        public Device Device;

        public float Position;

        const float MaxSpeed = 10;
        const float Acceleration = 10;

        public float CurrentSpeed;
        public CarLane Lane;

        Color AIColor;

        float HurtFor = 0f;
        const float MaxHurtFor = 0.25f;

        float? KillingDt;
        const float MaxKillingDuration = 1f;
        public bool Dead { get; private set; }

        public object Log { get; private set; }

        public int ValueOfCar { get { return Device != null ? 10 : 1; } }

        public AudioClip HonkSfx;

        public Car(CarLane lane, float position)
        {
            Lane = lane;
            Position = position;

            AIColor = Color.Lerp(Color.black, Color.white, UnityEngine.Random.Range(0.3f, 0.7f));
            HonkSfx = MainApp.S.AllHonksSfk[UnityEngine.Random.Range(0, MainApp.S.AllHonksSfk.Length)];
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
                var shouldBrake = nextCar != null && Position + ThreeSecondRule > nextCar.Position && nextCar.KillingDt.HasValue == false;
                var hurt = HurtFor > 0f;

                var lightCheckStart = Lane.Start.z + Position;
                var lightCheckEnd = Lane.Start.z + Position + OneSecondRule;

                var readLightAhead = Lane.Level.TrafficLights.Any(t => t.Mode != TrafficMode.CarGo
                                                                    && t.transform.position.z >= lightCheckStart
                                                                    && t.transform.position.z <= lightCheckEnd);

                // AI
                if (shouldBrake || hurt || readLightAhead)
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

            if (HurtFor > 0f)
            {
                HurtFor -= Time.deltaTime;
                color = Color.Lerp(color, Color.red, 0.2f);
            }



            CarParticles.S.Particles[particleId].startColor = color;
            CarParticles.S.Particles[particleId].position = Lane.Start + (CarLane.Forward * Position);
            CarParticles.S.Particles[particleId].remainingLifetime = 10f;
            CarParticles.S.Particles[particleId].startSize = GetHonkSize();
            CarParticles.S.Particles[particleId].rotation3D = Vector3.zero;
        }

        public void Honk()
        {
            HonkingDt = 0;
            MainApp.S.AudioSource.PlayOneShot(HonkSfx);
        }

        void KillingUpdate(int particleId)
        {
            const float JumpHeight = 15f;
            const float SpinDist = 360 * 4f;
            const float BounceBackDist = MaxSpeed * MaxKillingDuration * 2f;

            KillingDt += Time.deltaTime;
            var normalizedDeath = Mathf.Clamp01(KillingDt.Value / MaxKillingDuration);
            var sineDeath = Mathf.Sin(normalizedDeath * Mathf.PI / 2f);

            var lanePos = Lane.Start + (CarLane.Forward * Position);

            var jumpOffset = (Vector3.up * JumpHeight * sineDeath);
            var bounceBack = (CarLane.Forward * -1f * BounceBackDist * sineDeath);

            CarParticles.S.Particles[particleId].position = lanePos + jumpOffset + bounceBack;

            var color = Device != null ? Color.red : AIColor;

            CarParticles.S.Particles[particleId].startColor = color;
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

            MainApp.S.AudioSource.PlayOneShot(MainApp.S.KillSfk);

            Lane.Level.ConductorScore += ValueOfCar;

            var conductor = MainApp.S.Devices.Values.FirstOrDefault(d => d.Connected && d.Role == DeviceRole.Conductor);
            if (conductor != null)
                conductor.Score += ValueOfCar;

            KillingDt = 0f;
        }

        float? HonkingDt;
        const float HonkingDuration = 0.5f;

        float GetHonkSize()
        {
            if (HonkingDt .HasValue == false)
                return 1f;

            HonkingDt += Time.deltaTime;

            var normalizedHonk = HonkingDt.Value / HonkingDuration;
            var sineHonk = Mathf.Sin(normalizedHonk * Mathf.PI);

            if (HonkingDt.Value > HonkingDuration)
                HonkingDt = null;

            return 1f + ( 1 * sineHonk);
        }
    }
}
