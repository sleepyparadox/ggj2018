using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace Assets.Scripts
{
    public class PlayerIcon : MonoBehaviour
    {
        public RectTransform Space;
        public Dictionary<Device, PlayerIcon> IconsCollection;

        public UnityEngine.UI.Text NameText;
        public UnityEngine.UI.Text ScoreText;

        const float MinDistance = 100f;
        const float MinDistanceSqr = MinDistance * MinDistance;

        Vector3 CurrentVelocity;
        const float Acceleration = 10f;

        public void Update()
        {
            const float WeightAvoid = 2f;
            const float WeightEdge = 10f;

            var desiredDirection = Vector3.zero;

            foreach (var otherIcon in IconsCollection.Values)
            {
                if (otherIcon == this)
                    continue;

                // avoid other icons
                var dist = transform.position - otherIcon.transform.position;
                var distSqr = dist.sqrMagnitude;
                if(distSqr < MinDistanceSqr)
                {
                    desiredDirection += dist / MinDistance * WeightAvoid;
                }
            }

            // Avoid edges
            if (transform.localPosition.x < Space.rect.xMin)
                desiredDirection += Vector3.right * WeightEdge;
            if (transform.localPosition.x > Space.rect.xMax)
                desiredDirection += Vector3.left * WeightEdge;
            if (transform.localPosition.y < Space.rect.yMin)
                desiredDirection += Vector3.right * WeightEdge;
            if (transform.localPosition.y > Space.rect.yMax)
                desiredDirection += Vector3.left * WeightEdge;

            // Apply
            CurrentVelocity = Vector3.Lerp(CurrentVelocity, desiredDirection.normalized, Acceleration * Time.deltaTime);

            transform.position += new Vector3(CurrentVelocity.x, CurrentVelocity.y, 0);
        }
    }
}
