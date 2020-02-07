using UnityEngine;
using UnityEngine.Animations;

namespace ThreeDISevenZeroR.SensorKit
{
    /// <summary>
    /// <para>Base class for any physics sensor</para>
    /// <para>Provides shared collider detection logic that can be obtained from any sensor</para>
    /// </summary>
    public abstract class PhysicsSensor : MonoBehaviour
    {
        private static readonly Collider[] emptyColliders = new Collider[0];

        /// <summary>
        /// <para>If false, all arrays will be allocated on object creation.</para>
        /// <para>If true, all arrays will be allocated on first access.</para>
        /// </summary>
#if UNITY_2019_2_OR_NEWER
        [NotKeyable]
#endif
        [SerializeField]
        [Tooltip("If false, all arrays will be allocated on object creation.\n" +
                 "If true, all arrays will be allocated on first access.")]
        protected bool lazyAllocation;

        /// <summary>
        /// <para>Maximum amount of detected hits</para>
        /// <para>Every sensor uses no allocation per cast, and it is important to know maximum amount of
        /// objects this sensor is able to detect, to preallocate array
        /// Changing this property at runtime recreates array, so try to not touch if not necessary</para>
        /// </summary>
        [Tooltip("Maximum amount of detected hits\n" +
                 "Every sensor uses no allocation per cast, and it is important to know maximum amount of objects " +
                 "this sensor is able to detect, to preallocate array. " +
                 "Changing this property at runtime recreates array, so try to not touch if not necessary")]
        public int maxResults = 1;

        /// <summary>
        /// <para>Layer mask which sensor will use<para>
        /// </summary>
        [Tooltip("Layer mask which sensor will use")]
        public LayerMask layerMask = Physics.DefaultRaycastLayers;

        /// <summary>
        /// <para>Should this sensor detect triggers</para>
        /// </summary>
        [Tooltip("Should this sensor detect triggers")]
        public QueryTriggerInteraction queryTriggerInteraction = QueryTriggerInteraction.UseGlobal;

        protected int hitCount;
        protected Collider[] hitColliders = emptyColliders;

#if UNITY_2019_1_OR_NEWER

        private bool isCustomPhysicsSceneSet;
        private PhysicsScene customPhysicsScene;
        
        /// <summary>
        /// <para>Is lazy allocation enabled in inspector</para>
        /// </summary>
        public bool UsesLazyAllocation
        {
            get { return lazyAllocation; }
        }

        /// <summary>
        /// <para>Physics scene used for physics checks
        /// Defaults to "Physics.defaultPhysicsScene"</para>
        ///
        /// <para>When set to different scene, it is user responsibility to correctly
        /// handle cases when PhysicsScene is destroyed, sensor will not switch to "Physics.defaultPhysicsScene"
        /// automatically</para>
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
            get
            {
                EnsureArrayCapacity(ref hitColliders);
                return hitColliders;
            }
        }

        /// <summary>
        /// First collider that was hit, if any
        /// </summary>
        public Collider HitCollider
        {
            get
            {
                return HitCount > 0 ? HitColliders[0] : null;
            }
        }

        /// <summary>
        /// Updates sensor state, fires sensor logic and stores result in its instance
        /// </summary>
        /// <returns>Count of found colliders, same as HitCount</returns>
        public abstract int UpdateSensor();

        /// <summary>
        /// <para>Ensures that specified array contains enough items to store at least maxResults count</para>
        /// <para>If static allocation is not used, </para>
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