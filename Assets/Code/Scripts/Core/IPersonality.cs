using UnityEngine;

namespace RuckusReloaded.Runtime.Core
{
    public interface IPersonality
    {
        Vector3 LookTarget { get; }

        public static Vector3 LookTargetOf(GameObject gameObject)
        {
            var get = gameObject.GetComponentInParent<IPersonality>();
            return get?.LookTarget ?? gameObject.transform.position;
        }
    }
}