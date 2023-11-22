using RuckusReloaded.Runtime.Projectiles;
using RuckusReloaded.Runtime.Utility;
using UnityEngine;
using Random = UnityEngine.Random;

namespace RuckusReloaded.Runtime.Player
{
    public class PlayerGun : PlayerWeapon
    {
        public Projectile projectile;
        public ProjectileSpawnArgs args;
        public bool singleFire = false;
        public float fireRate = 180.0f;
        public int projectilesPerShot = 1;
        [Range(0.0f, 10.0f)]
        public float spread;

        [Space]
        public int ammo;
        public int maxAmmo = -1;
        
        [Space]
        public Vector3 muzzleOffset;

        private PlayerController player;
        private Animator animator;
        private bool shootFlag;
        private float lastFireTime;

        private ParticleSystem flash;
        private ParticleSystem smoke;

        public override string AmmoLabel => ammo >= 0 ? $"{ammo}/{maxAmmo}" : "--/--";
        public Vector3 MuzzlePosition => (MainCam ? MainCam.transform : transform).TransformPoint(muzzleOffset);

        protected override void Awake()
        {
            player = GetComponentInParent<PlayerController>();
            
            base.Awake();
            var viewport = transform.Find("Viewport");
            animator = viewport.GetComponentInChildren<Animator>();

            foreach (var t in viewport.GetComponentsInChildren<Transform>())
            {
                t.gameObject.layer = ViewportLayer;
            }

            flash = viewport.Find<ParticleSystem>("Flash");
            smoke = viewport.Find<ParticleSystem>("Smoke");

            ammo = maxAmmo;
        }

        private void Update()
        {
            if (singleFire)
            {
                if (Player.ShootAction.WasPressedThisFrame()) shootFlag = true;
            }
            else shootFlag = Player.ShootAction.IsPressed();

            animator.SetFloat("movement", Player.Biped.Movement);
            animator.SetBool("isOnGround", Player.Biped.IsOnGround);
        }

        private void FixedUpdate()
        {
            if (shootFlag)
            {
                Shoot();
            }

            ResetFlags();
        }

        private void Shoot()
        {
            if (Time.time < lastFireTime + 60.0f / fireRate) return;
            if (ammo == 0) return;

            for (var i = 0; i < projectilesPerShot; i++)
            {
                var random = Random.insideUnitCircle;
                var direction = MainCam.transform.TransformDirection(random.x * spread, random.y * spread, 10.0f).normalized;

                projectile.SpawnFromPrefab(player.gameObject, args, MuzzlePosition, direction);
            }

            animator.Play("Shoot", 0, 0.0f);
            if (flash) flash.Play();
            if (smoke && !smoke.isPlaying) smoke.Play();

            lastFireTime = Time.time;
            ammo--;
        }
        
        private void OnDrawGizmosSelected()
        {
            Gizmos.matrix = transform.localToWorldMatrix;
            Gizmos.color = Color.yellow;
            Gizmos.DrawSphere(muzzleOffset, 0.04f);

            Gizmos.DrawRay(MuzzlePosition, new Vector3(spread, 0.0f, 10.0f).normalized);
            Gizmos.DrawRay(MuzzlePosition, new Vector3(-spread, 0.0f, 10.0f).normalized);
            Gizmos.DrawRay(MuzzlePosition, new Vector3(0.0f, spread, 10.0f).normalized);
            Gizmos.DrawRay(MuzzlePosition, new Vector3(0.0f, -spread, 10.0f).normalized);
        }

        private void ResetFlags()
        {
            shootFlag = false;
        }
    }
}