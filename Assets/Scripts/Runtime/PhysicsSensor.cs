using UnityEngine;

namespace ThreeDISevenZeroR.SensorKit
{
    /// <summary>
    /// Base class for any physics sensor
    /// Provides shared collider detection logic that can be obtainer from any sensor
    /// </summary>
    public abstract class PhysicsSensor : MonoBehaviour
    {
        private static readonly Collider[] emptyColliders = new Collider[0];

        /// <summary>
        /// If false, all arrays will be allocated on object creation. If true, all arrays will be allocated
        /// on first access to them. Saves memory on sensors that can be unused during object lifetime.
        /// Slightly saves memory, but makes allocation on first access to sensor
        /// </summary>
        public bool lazyAllocation;

        /// <summary>
        /// Maximum amount of detected hits
        /// Every sensor uses no allocation per cast, and it is important to know maximum amount of
        /// objects this sensor is able to detect, to preallocate array at creation
        /// Changing this property at runtime recreates array, so try to not touch it without much need
        /// </summary>
        public int maxResults = 1;

        /// <summary>
        /// Layer mask of cast
        /// </summary>
        public LayerMask layerMask = Physics.DefaultRaycastLayers;

        /// <summary>
        /// Setting that specifies, if sensor should detect triggers too
        /// </summary>
        public QueryTriggerInteraction queryTriggerInteraction = QueryTriggerInteraction.UseGlobal;

        protected int hitCount;
        protected Collider[] hitColliders = emptyColliders;

#if UNITY_2019_1_OR_NEWER

        private bool isCustomPhysicsSceneSet;
        private PhysicsScene customPhysicsScene;

        /// <summary>
        /// Physics scene used for physics checks
        /// Defaults to "Physics.defaultPhysicsScene"
        ///
        /// When set to different scene, it is user responsibility to correctly
        /// handle cases when PhysicsScene is destroyed, sensor will not switch to "Physics.defaultPhysicsScene"
        /// automatically
        /// </summary>
        public PhysicsScene PhysicsScene
        {
            get { return isCustomPhysicsSceneSet ? customPhysicsScene : Physics.defaultPhysicsScene; }
            set
            {
                customPhysicsScene = value;
                isCustomPhysicsSceneSet = true;
            }
        }
#endif

        /// <summary>
        /// Is sensor detected something?
        /// Returns true when HitCount returns more than zero
        /// </summary>
        public bool HasHit
        {
            get { return HitCount > 0; }
        }

        /// <summary>
        /// Count of detected objects
        /// </summary>
        public int HitCount
        {
            get { return hitCount; }
        }

        /// <summary>
        /// Array with colliders of detected objects
        /// This array is cached, and guaranteed to be at least HitCount elements long
        /// </summary>
        public virtual Collider[] HitColliders
        {
            get { return hitColliders; }
        }

        /// <summary>
        /// First collider that was hit, if any
        /// </summary>
        public Collider HitCollider
        {
            get { return HitCount > 0 ? HitColliders[0] : null; }
        }

        /// <summary>
        /// Updates sensor state, fires sensor logic and stores result in its instance
        /// </summary>
        /// <returns>Count of found colliders, same as HitCount</returns>
        public abstract int UpdateSensor();

        /// <summary>
        /// Ensures that specified array contains enough items to store at least maxResults count
        /// Recreates array if not
        /// </summary>
        protected void EnsureArrayCapacity<T>(ref T[] array)
        {
            if (array == null || array.Length != maxResults)
            {
                array = new T[maxResults];
            }
        }
    }
}