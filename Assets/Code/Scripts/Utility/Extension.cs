
using UnityEngine;

namespace RuckusReloaded.Runtime.Utility
{
    public static class Extension
    {
        public static T GetOrAddComponent<T>(this GameObject gameObject) where T : Component
        {
            var c = gameObject.GetComponent<T>();
            return c ? c : gameObject.AddComponent<T>();
        }
    }
}