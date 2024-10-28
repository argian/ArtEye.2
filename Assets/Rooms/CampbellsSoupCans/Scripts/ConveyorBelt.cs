using UnityEngine;

namespace ArtEye.CampbellsSoupCans
{
    public class ConveyorBelt : MonoBehaviour
    {
        [SerializeField] private Vector3 direction;
        [SerializeField] private float speed;
        [SerializeField] private bool isOn = true;
        [SerializeField] private Collider c;

        private void OnTriggerStay(Collider other)
        {
            if (!isOn)
                return;

            Vector3 otherPosition = other.bounds.center;

            Vector3 movement = otherPosition + direction;
            Vector3 dragToCenter = (ClosestPointOnLine(otherPosition, c.bounds.center, direction) - otherPosition) *
                                   DistancePointToLine(otherPosition, c.bounds.center, direction);

            // Debug.DrawLine(otherPosition, movement, Color.red, 0.1f);
            // Debug.DrawLine(otherPosition, otherPosition + dragToCenter, Color.blue, 0.1f);

            Vector3 targetPosition = movement + dragToCenter;
            Vector3 newPosition =
                Vector3.MoveTowards(other.attachedRigidbody.position, targetPosition, speed * Time.deltaTime);
            other.attachedRigidbody.MovePosition(newPosition);
        }

        public void Activated(bool state)
        {
            isOn = state;
        }

        static Vector3 ClosestPointOnLine(Vector3 point, Vector3 origin, Vector3 direction)
        {
            Vector3 lineVector = direction.normalized;
            Vector3 pointVector = point - origin;

            float projection = Vector3.Dot(pointVector, lineVector);

            Vector3 closestPoint = origin + lineVector * projection;

            // Debug.DrawLine(closestPoint, closestPoint + Vector3.up, Color.yellow, 0.1f);
            return closestPoint;
        }

        static float DistancePointToLine(Vector3 point, Vector3 origin, Vector3 direction)
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