using System;
using RuckusReloaded.Runtime.Utility;
using RuckusReloaded.Runtime.Vitality;
using UnityEngine;
using Object = UnityEngine.Object;
using Random = UnityEngine.Random;

namespace RuckusReloaded.Runtime.Player
{
    public class PlayerGun : PlayerWeapon
    {
        public DamageArgs damage;
        public bool singleFire = false;
        public float fireRate = 180.0f;
        public int projectilesPerShot = 1;
        public float spread;

        [Space]
        public GameObject hitFX;
        public ParticleSystem trailFX;
        public float trailDensity = 16.0f;
        public Vector3 muzzleOffset;

        private Animator animator;
        private bool shootFlag;
        private float lastFireTime;

        private ParticleSystem flash;

        protected override void Awake()
        {
            base.Awake();
            var viewport = transform.Find("Viewport");
            animator = viewport.GetComponentInChildren<Animator>();

            foreach (var t in viewport.GetComponentsInChildren<Transform>())
            {
                t.gameObject.layer = ViewportLayer;
            }

            flash = viewport.Find<ParticleSystem>("Flash");
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

            for (var i = 0; i < projectilesPerShot; i++)
            {
                var random = Random.insideUnitCircle;
                var direction = MainCam.transform.TransformDirection(random.x * spread, random.y * spread, 10.0f).normalized;
                var point = MainCam.transform.position;

                var ray = new Ray(point, direction);
                var end = ray.GetPoint(100.0f);
                if (Physics.Raycast(ray, out var hit))
                {
                    end = hit.point;
                    ProcessHit(ray, hit);
                }
                
                if (trailFX)
                {
                    var start = transform.TransformPoint(muzzleOffset);
                    var vector = end - start;
                    var dist = vector.magnitude;
                    var step = 1.0f / trailDensity;
                    var dir = vector / dist;
                    
                    for (var d = 0.0f; d < dist; d += step)
                    {
                        trailFX.Emit(new ParticleSystem.EmitParams()
                        {
                            position = start + dir * d,
                        }, 1);
                    }
                }
            }

            animator.Play("Shoot", 0, 0.0f);
            if (flash) flash.Play();

            lastFireTime = Time.time;
        }

        private void ProcessHit(Ray ray, RaycastHit hit)
        {
            var damageable = hit.collider.GetComponentInParent<IDamageable>();
            if ((Object)damageable)
            {
                damageable.Damage(new DamageInstance(damage, hit.point, ray.direction));
            }

            if (hitFX) Instantiate(hitFX, hit.point, Quaternion.LookRotation(hit.normal));
        }

        private void OnDrawGizmosSelected()
        {
            Gizmos.matrix = transform.localToWorldMatrix;
            Gizmos.color = Color.yellow;
            Gizmos.DrawSphere(muzzleOffset, 0.04f);
        }

        private void ResetFlags()
        {
            shootFlag = false;
        }
    }
}