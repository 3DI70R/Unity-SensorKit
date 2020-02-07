using UnityEngine;

namespace ThreeDISevenZeroR.SensorKit
{
    /// <summary>
    /// <para>Sensor which casts ray, sphere or capsule.</para>
    /// <para>Behaves like a Physics.Raycast, Physics.RaycastNonAlloc, Physics.SphereCast, Physics.SphereCastNonAlloc,
    /// Physics.CapsuleCast, Physics.CapsuleCastNonAlloc, depending on your settings</para>
    /// </summary>
    public class SphereCastSensor : CastSensor
    {
        /// <summary>
        /// <para>Radius of sphere, when sphere radius is zero, behaves like a ray</para>
        /// </summary>
        [Tooltip("Radius of sphere, when sphere radius is zero, behaves like a ray")]
        public float radius;

        /// <summary>
        /// <para>Width of sphere, when non zero, makes this sensor behave like a capsule, when zero, behaves like a sphere</para>
        /// </summary>
        [Tooltip("Width of sphere, when non zero, makes this sensor behave like a capsule, when zero, behaves like a sphere")]
        public float width;

        protected override int DoCast(Ray ray, RaycastHit[] hitArray)
        {
            var scale = transform.lossyScale;
            var castDistance = PhysicsSensorUtils.GetCastDistance(maxDistance, scale);

            if (width != 0)
            {
                Vector3 p1;
                Vector3 p2;

                PhysicsSensorUtils.GetCapsulePoints(ray.origin, transform.rotation, width, scale.x, out p1, out p2);

                if (hitArray.Length == 1)
                {

#if UNITY_2019_1_OR_NEWER
                    return PhysicsScene.
#else
                    return Physics.
#endif
                        CapsuleCast(p1, p2, PhysicsSensorUtils.GetScaledCapsuleRadius(radius, scale), 
                            ray.direction, out hitArray[0], castDistance, layerMask, queryTriggerInteraction) ? 1 : 0;
                }

#if UNITY_2019_1_OR_NEWER
                return PhysicsScene.CapsuleCast
#else
                return Physics.CapsuleCastNonAlloc
#endif
                (p1, p2, PhysicsSensorUtils.GetScaledCapsuleRadius(radius, scale), 
                    ray.direction, hitArray, castDistance, layerMask, queryTriggerInteraction);
            }

            if (radius != 0)
            {
                if (hitArray.Length == 1)
                {
#if UNITY_2019_1_OR_NEWER
                    return PhysicsScene.SphereCast
#else
                    return Physics.SphereCast
#endif
                    (ray.origin, PhysicsSensorUtils.GetScaledSphereRadius(radius, scale), 
                        ray.direction, out hitArray[0], castDistance, layerMask, queryTriggerInteraction) ? 1 : 0;
                }
                
#if UNITY_2019_1_OR_NEWER
                return PhysicsScene.SphereCast
#else
                return Physics.SphereCastNonAlloc
#endif
                (ray.origin, PhysicsSensorUtils.GetScaledSphereRadius(radius, scale),
                    ray.direction, hitArray, castDistance, layerMask, queryTriggerInteraction);
            }

            if (hitArray.Length == 1)
            {
#if UNITY_2019_1_OR_NEWER
                return PhysicsScene.
#else
                return Physics.
#endif
                    Raycast(ray.origin, ray.direction, out hitArray[0], 
                        castDistance, layerMask, queryTriggerInteraction) ? 1 : 0;
            }

#if UNITY_2019_1_OR_NEWER
            return PhysicsScene.Raycast
#else
            return Physics.RaycastNonAlloc
#endif
            (ray.origin, ray.direction, hitArray,
                castDistance, layerMask, queryTriggerInteraction);
        }

#if UNITY_EDITOR
        protected override void DrawColliderShape(Vector3 position, Quaternion rotation, Vector3 scale)
        {
            if (width != 0)
            {
                PhysicsSensorUtils.DrawCapsuleGizmo(position, rotation, scale, width, radius);
            }
            else if (radius != 0)
            {
                PhysicsSensorUtils.DrawSphereGizmo(position, rotation, scale, radius);
            }
        }
#endif
    }
}