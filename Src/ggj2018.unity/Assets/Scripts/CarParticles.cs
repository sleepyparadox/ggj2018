using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace Assets.Scripts
{
    public class CarParticles : MonoBehaviour
    {
        public static CarParticles S;

        public int ParticleCount;
        public ParticleSystem.Particle[] Particles;

        public ParticleSystem.Particle A;
        public ParticleSystem.Particle B;


        ParticleSystem _system;

        void Awake()
        {
            S = this;
            _system = gameObject.GetComponent<ParticleSystem>();
            Particles = new ParticleSystem.Particle[_system.main.maxParticles];
            _system.GetParticles(Particles);
        }

        void Update()
        {
            _system.SetParticles(Particles, ParticleCount);

            for (int i = 0; i < ParticleCount; i++)
            {
                foreach (var field in typeof(ParticleSystem.Particle).GetFields())
                {
                    Console.WriteLine(field.Name + " " + field.GetValue(Particles[i]));
                }
            }
        }
    }
}
