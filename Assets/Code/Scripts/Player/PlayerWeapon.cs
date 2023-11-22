using UnityEngine;

namespace RuckusReloaded.Runtime.Player
{
    public abstract class PlayerWeapon : MonoBehaviour
    {
        public const int ViewportLayer = 3;

        public Camera MainCam { get; private set; }
        public PlayerController Player { get; private set; }
        
        public virtual string DisplayName => name;
        public abstract string AmmoLabel { get; }

        protected virtual void Awake()
        {
            MainCam = Camera.main;
            Player = GetComponentInParent<PlayerController>();
        }
    }
}
