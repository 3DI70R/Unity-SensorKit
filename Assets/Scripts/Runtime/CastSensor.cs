using UnityEngine;

namespace ThreeDISevenZeroR.SensorKit
{
    /// <summary>
    /// Abstract class for any cast sensor<br/>
    /// Cast sensor can detect objects at specified distance
    /// </summary>
    public abstract class CastSensor : PhysicsSensor
    {
        private static readonly RaycastHit[] emptyRays = new RaycastHit[0];

        /// <summary>
        /// Transform which overrides ray cast direction
        /// Makes possible to cast rotated shapes
        ///
        /// If null, uses sensor object for direction
        /// </summary>
        public Transform rayDirectionOverride;
        
        /// <summary>
        /// Maximum ray cast distance
        /// </summary>
        public float maxDistance = Mathf.Infinity;

        private static RaycastHit[] rayHits = emptyRays;
        private bool outdatedColliders;

        /// <summary>
        /// Actual ray that will be fired on update
        /// </summary>
        public Ray Ray
        {
            get
            {
                return new Ray(transform.position, CastDirection *
                                                   new Vector3(0, 0, transform.lossyScale.z > 0 ? 1 : -1));
            }
        }

        /// <summary>
        /// Array with all hits that have been detected during sensor update<br/>
        /// This array is cached, and guaranteed to be at least HitCount long
        /// </summary>
        public RaycastHit[] RayHits
        {
            get { return rayHits; }
        }

        /// <summary>
        /// Returns first RayHit<br/>
        /// Convenience method, when maxCount is 1
        /// </summary>
        public RaycastHit RayHit
        {
            get { return HitCount > 0 ? RayHits[0] : default(RaycastHit); }
        }

        /// <summary>
        /// Returns closest RayHit<br/>
        /// Since NonAlloc methods returns array with no order, it 
        /// </summary>
        public RaycastHit ClosestRayHit
        {
            get
            {
                if (hitCount == 1)
                {
                    return rayHits[0];
                }

                if (hitCount == 0)
                {
                    return default(RaycastHit);
                }

                var closestIndex = 0;
                var closestDistance = float.MaxValue;

                for (var i = 0; i < hitCount; i++)
                {
                    var distance = rayHits[i].distance;

                    if (distance < closestDistance)
                    {
                        closestIndex = i;
                        closestDistance = distance;
                    }
                }

                return rayHits[closestIndex];
            }
        }

        /// <summary>
        /// Direction in which ray will be casted
        /// Either rotation used by this object, or rotation from rayDirectionOverride
        /// </summary>
        public Quaternion CastDirection
        {
            get { return rayDirectionOverride ? rayDirectionOverride.rotation : transform.rotation; }
        }
        
        /// <summary>
        /// Actual distance of cast, with respect of object scale
        /// </summary>
        public float CastDistance
        {
            get { return PhysicsSensorUtils.GetCastDistance(maxDistance, transform.lossyScale); }
        }
        
        public override Collider[] HitColliders
        {
            get
            {
                if (outdatedColliders)
                {
                    UpdateCollidersArray();
                    outdatedColliders = false;
                }

                return hitColliders;
            }
        }

        private void Start()
        {
            if (!lazyAllocation)
            {
                EnsureArrayCapacity(ref hitColliders);
                EnsureArrayCapacity(ref rayHits);
            }
        }

        public override int UpdateSensor()
        {
            EnsureArrayCapacity(ref hitColliders);
            EnsureArrayCapacity(ref rayHits);
            hitCount = DoCast(Ray, rayHits);
            outdatedColliders = true;
            return hitCount;
        }

        private void UpdateCollidersArray()
        {
            for (var i = 0; i < hitCount; i++)
            {
                hitColliders[i] = rayHits[i].collider;
            }

            for (var i = hitCount; i < hitColliders.Length; i++)
            {
                hitColliders[i] = null;
            }
        }

        protected abstract int DoCast(Ray ray, RaycastHit[] hit);

#if UNITY_EDITOR

        private RaycastHit[] gizmoRayHits = emptyRays;

        private void OnDrawGizmosSelected()
        {
            var castDistance = CastDistance;

            if (float.IsPositiveInfinity(castDistance))
            {
                castDistance = 1000000f;
            }

            var castRay = Ray;
            EnsureArrayCapacity(ref gizmoRayHits);
            var gizmoHitCount = DoCast(castRay, gizmoRayHits);
            var rayEnd = castRay.GetPoint(castDistance);

            if (gizmoHitCount > 0)
            {
                for (var i = 0; i < gizmoHitCount; i++)
                {
                    var gizmoHit = gizmoRayHits[i];
                    var collisionPoint = castRay.GetPoint(gizmoHit.distance);

                    Gizmos.color = PhysicsSensorUtils.hasHitColor;
                    Gizmos.DrawLine(castRay.origin, collisionPoint);
                    DrawColliderShape(collisionPoint, transform.rotation, transform.lossyScale);
                    Gizmos.color = PhysicsSensorUtils.rayEndColor;
                    Gizmos.DrawLine(collisionPoint, rayEnd);

                    PhysicsSensorUtils.DrawNormal(gizmoHit);
                    PhysicsSensorUtils.DrawCollisionPoints(collisionPoint, gizmoHit);
                    PhysicsSensorUtils.HighlightMeshVertices(gizmoHit);
                    PhysicsSensorUtils.DrawHitInfo(gizmoHit);
                }
            }
            else
            {
                Gizmos.color = PhysicsSensorUtils.noHitColor;
                Gizmos.DrawLine(castRay.origin, rayEnd);
                DrawColliderShape(rayEnd, transform.rotation, transform.lossyScale);
            }
        }

        protected virtual void DrawColliderShape(Vector3 position, Quaternion rotation, Vector3 scale)
        {
            // noop
        }
#endif
    }
}