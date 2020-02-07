using UnityEngine;

namespace ThreeDISevenZeroR.SensorKit
{
    /// <summary>
    /// <para>Sensor which checks for colliders inside its volume, behaves like a Physics.OverlapBox</para>
    /// </summary>
    public class BoxOverlapSensor : OverlapSensor
    {
        /// <summary>
        /// <para>Half extents of box</para>
        /// </summary>
        [Tooltip("Half extents of box")]
        public Vector3 halfExtents;

        protected override int DoOverlapCheck(Vector3 center, Collider[] colliders)
        {
#if UNITY_2019_1_OR_NEWER
            return PhysicsScene.OverlapBox
#else
            return Physics.OverlapBoxNonAlloc
#endif
            (center, PhysicsSensorUtils.GetScaledBoxRadius(halfExtents, transform.lossyScale),
                colliders, transform.rotation, layerMask, queryTriggerInteraction);
        }

#if UNITY_EDITOR
        protected override void DrawColliderShape(Vector3 position, Quaternion rotation, Vector3 scale)
        {
            PhysicsSensorUtils.DrawBoxGizmo(position, rotation, scale, halfExtents);
        }
#endif
    }
}