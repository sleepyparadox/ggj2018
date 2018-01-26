using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace Assets.Scripts
{
    public class Lane
    {
        public Vector3 Start;
        public Vector3 End;
        public Vector3 Direction;

        public Lane(Transform t)
        {
            Start = t.TransformPoint(new Vector3(0, 0, -0.5f));
            End = t.TransformPoint(new Vector3(0, 0, 0.5f));
            Direction = t.forward;
        }

        public Vector3 Clamp(Vector3 point)
        {
            Debug.DrawLine(Start + Vector3.up, End, Color.white);
            Debug.DrawLine(Start + Vector3.up, Start + Vector3.up + Direction, Color.magenta);

            if (Vector3.Dot((point - Start).normalized, Direction) >= 0)
            {
                // forward

                if ((Start - point).sqrMagnitude > (Start - End).sqrMagnitude)
                {
                    // gone past
                    // loop back to start

                    Debug.DrawLine(point, point + new Vector3(0, 10, 0), Color.red, 10f);

                    return Start;
                }
                else
                {
                    // still within line
                    return point;
                }
            }
            else
            {
                // backing off too far
                Debug.DrawLine(point, point + new Vector3(0, 10, 0), Color.red, 10f);
                return Start;
            }
        }
    }
}
