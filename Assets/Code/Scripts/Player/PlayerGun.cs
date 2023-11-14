using UnityEngine;

namespace RuckusReloaded.Runtime.Player
{
    public class PlayerGun : PlayerWeapon
    {
        public int damage = 5;
        public bool singleFire = false;
        public float fireRate = 180.0f;
        public int projectilesPerShot = 1;
        public float spread;

        [Space]
        public GameObject hitFX;

        private Animator animator;
        private bool shootFlag;
        private float lastFireTime;

        protected override void Awake()
        {
            base.Awake();
            animator = GetComponentInChildren<Animator>();

            foreach (var t in animator.GetComponentsInChildren<Transform>())
            {
                t.gameObject.layer = ViewportLayer;
            }
        }

        private void Update()
        {
            if (singleFire)
            {
                if (Player.ShootAction.WasPressedThisFrame()) shootFlag = true;
            }
            else shootFlag = Player.ShootAction.IsPressed();

            animator.SetFloat("movement", Player.Movement);
            animator.SetBool("isOnGround", Player.IsOnGround);
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

            for (var i = 0; i < projectilesPerShot; i++)
            {
                var random = Random.insideUnitCircle;
                var direction = MainCam.transform.TransformDirection(random.x * spread, random.y * spread, 10.0f).normalized;
                var point = MainCam.transform.position;

                var ray = new Ray(point, direction);
                var end = ray.GetPoint(500.0f);
                if (Physics.Raycast(ray, out var hit))
                {
                    end = hit.point;
                    ProcessHit(hit);
                }
            }

            animator.Play("Shoot", 0, 0.0f);

            lastFireTime = Time.time;
        }

        private void ProcessHit(RaycastHit hit)
        {
            Instantiate(hitFX, hit.point, Quaternion.LookRotation(hit.normal));
        }

        private void ResetFlags()
        {
            shootFlag = false;
        }
    }
}