using UnityEngine;

namespace ThreeDISevenZeroR.SensorKit
{
    /// <summary>
    /// Sensor which casts ray, sphere or capsule.
    /// Behaves like a Physics.Raycast, Physics.RaycastNonAlloc, Physics.SphereCast, Physics.SphereCastNonAlloc,
    /// Physics.CapsuleCast, Physics.CapsuleCastNonAlloc, depending on your settings
    /// </summary>
    public class SphereCastSensor : CastSensor
    {
        /// <summary>
        /// Radius of sphere, when sphere radius is zero, behaves like a ray
        /// </summary>
        public float radius;

        /// <summary>
        /// With of sphere, when non zero, makes this sensor behave like a capsule, when zero, behaves like a sphere
        /// </summary>
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
                    return Physics.CapsuleCast(p1, p2, PhysicsSensorUtils.GetScaledCapsuleRadius(radius, scale),
                        ray.direction, out hitArray[0], castDistance, layerMask, queryTriggerInteraction)
                        ? 1
                        : 0;
                }

                return Physics.CapsuleCastNonAlloc(p1, p2, PhysicsSensorUtils.GetScaledCapsuleRadius(radius, scale),
                    ray.direction, hitArray, castDistance, layerMask, queryTriggerInteraction);
            }

            if (radius != 0)
            {
                if (hitArray.Length == 1)
                {
                    return Physics.SphereCast(Ray, PhysicsSensorUtils.GetScaledSphereRadius(radius, scale),
                        out hitArray[0], castDistance, layerMask, queryTriggerInteraction)
                        ? 1
                        : 0;
                }

                return Physics.SphereCastNonAlloc(Ray, PhysicsSensorUtils.GetScaledSphereRadius(radius, scale),
                    hitArray, castDistance, layerMask, queryTriggerInteraction);
            }

            if (hitArray.Length == 1)
            {
                return Physics.Raycast(Ray, out hitArray[0], castDistance, layerMask, queryTriggerInteraction) ? 1 : 0;
            }

            return Physics.RaycastNonAlloc(Ray, hitArray,
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