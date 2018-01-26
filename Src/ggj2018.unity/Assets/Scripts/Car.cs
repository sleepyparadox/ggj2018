using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace Assets.Scripts
{
    public class Car : MonoBehaviour
    {
        public Device Device;

        public float MaxSpeed = 10;
        public float Acceleration = 100;

        public float CarSpeed;
        public Lane Lane;

        public void Reset()
        {
            transform.position = Lane.Start;
            transform.forward = Lane.Direction;
        }

        public void Update()
        {
            var targetSpeed = Device.Input.Speed * MaxSpeed;
            CarSpeed = Mathf.Lerp(CarSpeed, targetSpeed, Acceleration * Time.deltaTime);

            var targetPos = transform.position + ( Lane.Direction * CarSpeed * Time.deltaTime);
            transform.position = Lane.Clamp(targetPos);
        }
    }
}
