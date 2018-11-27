using UnityEngine;

/// <summary>
/// Sensor which checks for colliders inside its volume, behaves like a Physics.OverlapBox
/// </summary>
public class BoxOverlapSensor : OverlapSensor
{
    public Vector3 halfExtents;
    
    protected override int DoOverlapCheck(Vector3 center, Collider[] colliders)
    {
        return Physics.OverlapBoxNonAlloc(center, 
            PhysicsSensorUtils.GetScaledBoxRadius(halfExtents, transform.lossyScale), 
            colliders, transform.rotation, layerMask, queryTriggerInteraction);
    }

#if UNITY_EDITOR
    protected override void DrawColliderShape(Vector3 position, Quaternion rotation, Vector3 scale)
    {
        PhysicsSensorUtils.DrawBoxGizmo(position, rotation, scale, halfExtents);
    }
#endif
}
