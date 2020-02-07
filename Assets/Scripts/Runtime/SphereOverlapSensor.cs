using UnityEngine;

namespace ThreeDISevenZeroR.SensorKit
{
    /// <summary>
    /// <para>Sensor which checks for colliders inside its volume, sphere or capsule, depending on your settings.<para>
    /// <para>Behaves like a Physics.OverlapSphereNonAlloc, Physics.OverlapCapsuleNonAlloc respectively<para>
    /// </summary>
    public class SphereOverlapSensor : OverlapSensor
    {
        /// <summary>
        /// <para>Radius of sphere</para>
        /// </summary>
        [Tooltip("Radius of sphere")]
        public float radius;

        /// <summary>
        /// <para>Width of sphere, when non zero, makes this sensor behave like a capsule</para>
        /// </summary>
        [Tooltip("Width of sphere, when non zero, makes this sensor behave like a capsule")]
        public float width;

        protected override int DoOverlapCheck(Vector3 center, Collider[] colliders)
        {
            var scale = transform.lossyScale;

            if (width != 0)
            {
                Vector3 p1;
                Vector3 p2;

                PhysicsSensorUtils.GetCapsulePoints(center, transform.rotation, width, scale.x, out p1, out p2);

#if UNITY_2019_1_OR_NEWER
                return PhysicsScene.OverlapCapsule
#else
                return Physics.OverlapCapsuleNonAlloc
#endif
                (p1, p2, PhysicsSensorUtils.GetScaledCapsuleRadius(radius, scale),
                    colliders, layerMask, queryTriggerInteraction);
            }

#if UNITY_2019_1_OR_NEWER
            return PhysicsScene.OverlapSphere
#else
            return Physics.OverlapSphereNonAlloc
#endif
            (center, PhysicsSensorUtils.GetScaledSphereRadius(radius, scale), 
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
