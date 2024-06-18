
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

        public static T Find<T>(this Transform transform, string path)
        {
            var find = transform.Find(path);
            return find ? find.GetComponent<T>() : default;
        }

        public static GameObject FindGameObject(this Transform transform, string path)
        {
            var find = transform.Find(path);
            return find ? find.gameObject : null;
        }
    }
}