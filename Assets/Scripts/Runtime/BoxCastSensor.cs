﻿using UnityEngine;
 
namespace ThreeDISevenZeroR.SensorKit
{
    /// <summary>
    /// Sensor which casts box<br/>
    /// Behaves like a Physics.BoxCast, Physics.BoxCastNonAlloc, depending on your settings
    /// </summary>
    public class BoxCastSensor : CastSensor
    {
        /// <summary>
        /// Half extents of box
        /// </summary>
        public Vector3 halfExtents;

        protected override int DoCast(Ray ray, RaycastHit[] hitArray)
        {
            var scale = transform.lossyScale;

            if (maxResults == 1)
            {
#if UNITY_2019_1_OR_NEWER
                return PhysicsScene.
#else
                return Physics.
#endif
                BoxCast(ray.origin, PhysicsSensorUtils.GetScaledBoxRadius(halfExtents, scale), ray.direction,
                    out hitArray[0], transform.rotation, PhysicsSensorUtils.GetCastDistance(maxDistance, scale),
                    layerMask, queryTriggerInteraction) ? 1 : 0;
            }
            
#if UNITY_2019_1_OR_NEWER
            return PhysicsScene.BoxCast
#else
            return Physics.BoxCastNonAlloc
#endif
            (ray.origin, PhysicsSensorUtils.GetScaledBoxRadius(halfExtents, scale), 
                ray.direction, hitArray, transform.rotation, PhysicsSensorUtils.GetCastDistance(maxDistance, scale), 
                layerMask, queryTriggerInteraction);
        }

#if UNITY_EDITOR
    protected override void DrawColliderShape(Vector3 position, Quaternion rotation, Vector3 scale)
    {
        PhysicsSensorUtils.DrawBoxGizmo(position, rotation, scale, halfExtents);
    }
#endif
    }
}

