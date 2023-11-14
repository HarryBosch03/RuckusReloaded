using UnityEngine;

namespace RuckusReloaded.Runtime.Player
{
    public class PlayerWeapon : MonoBehaviour
    {
        public const int ViewportLayer = 3;

        public Camera MainCam { get; private set; }
        public PlayerController Player { get; private set; }

        protected virtual void Awake()
        {
            MainCam = Camera.main;
            Player = GetComponentInParent<PlayerController>();
        }
    }
}
