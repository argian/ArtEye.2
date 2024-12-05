using System;
using System.Collections.Generic;
using UnityEngine;

namespace ArtEye.Warhol
{
    public class ConveyorBelt : MonoBehaviour
    {
        [field: SerializeField] public bool IsOn { get; set; } = true;
        
        [SerializeField] private float speed = 1f;
        
        private void OnTriggerStay(Collider other)
        {
            if (!IsOn)
                return;

            CampbellCan can = other.attachedRigidbody.GetComponent<CampbellCan>();
            if (can && can.PickedUp)
                return;

            Vector3 otherPosition = other.bounds.center;
        
            Vector3 movement = otherPosition + transform.forward;
            Vector3 dragToCenter = (ClosestPointOnLine(otherPosition, transform.position, transform.forward) - otherPosition) * DistancePointToLine(otherPosition, transform.position, transform.forward);
        
            // Debug.DrawLine(otherPosition, movement, Color.red, 0.1f);
            // Debug.DrawLine(otherPosition, otherPosition + dragToCenter, Color.blue, 0.1f);

            Vector3 targetPosition = movement + dragToCenter;
            Vector3 newPosition = Vector3.MoveTowards(other.attachedRigidbody.position, targetPosition, speed * Time.deltaTime);
            other.attachedRigidbody.MovePosition(newPosition);
        }
        
        private void OnDrawGizmosSelected()
        {
            Gizmos.color = Color.red;

            float offset = Time.realtimeSinceStartup / 2f % 1f;
            offset -= .5f;

            for (int i = 0; i < 3; i++)
            {
                Gizmos.DrawSphere(transform.position + transform.forward * offset, .05f);
                offset += .33f;
                if (offset > .5f)
                    offset -= 1f;
            }
        }

        private static Vector3 ClosestPointOnLine(Vector3 point, Vector3 origin, Vector3 direction)
        {
            Vector3 lineVector = direction.normalized;
            Vector3 pointVector = point - origin;
        
            float projection = Vector3.Dot(pointVector, lineVector);
        
            Vector3 closestPoint = origin + lineVector * projection;
            
            return closestPoint;
        }

        private static float DistancePointToLine(Vector3 point, Vector3 origin, Vector3 direction)
        {
            Vector3 lineVector = direction.normalized;
            Vector3 pointVector = point - origin;
        
            float projection = Vector3.Dot(pointVector, lineVector);
        
            Vector3 closestPoint = origin + lineVector * projection;
        
            float distance = Vector3.Distance(point, closestPoint);

            return distance;
        }
    }
}
