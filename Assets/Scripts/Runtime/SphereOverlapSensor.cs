using UnityEngine;

namespace ThreeDISevenZeroR.SensorKit
{
    /// <summary>
    /// Sensor which checks for colliders inside its volume, sphere or capsule, depending on your settings.
    /// Behaves like a Physics.OverlapSphereNonAlloc, Physics.OverlapCapsuleNonAlloc respectively
    /// </summary>
    public class SphereOverlapSensor : OverlapSensor
    {
        /// <summary>
        /// Radius of sphere
        /// </summary>
        public float radius;

        /// <summary>
        /// With of sphere, when non zero, makes this sensor behave like a capsule
        /// </summary>
        public float width;

        protected override int DoOverlapCheck(Vector3 center, Collider[] colliders)
        {
            var scale = transform.lossyScale;

            if (width != 0)
            {
                Vector3 p1;
                Vector3 p2;

                PhysicsSensorUtils.GetCapsulePoints(center, transform.rotation, width, scale.x, out p1, out p2);
                return Physics.OverlapCapsuleNonAlloc(p1, p2, PhysicsSensorUtils.GetScaledCapsuleRadius(radius, scale),
                    colliders, layerMask, queryTriggerInteraction);
            }

            return Physics.OverlapSphereNonAlloc(center, PhysicsSensorUtils.GetScaledSphereRadius(radius, scale),
                colliders, layerMask, queryTriggerInteraction);
        }

#if UNITY_EDITOR
        protected override void DrawColliderShape(Vector3 position, Quaternion rotation, Vector3 scale)
        {
            if (width > 0)
            {
                PhysicsSensorUtils.DrawCapsuleGizmo(position, rotation, scale, width, radius);
            }
            else
            {
                PhysicsSensorUtils.DrawSphereGizmo(position, rotation, scale, radius);
            }
        }
#endif
    }
}
