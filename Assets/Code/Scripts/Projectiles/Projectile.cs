using System.Linq;
using RuckusReloaded.Runtime.Vitality;
using UnityEngine;

namespace RuckusReloaded.Runtime.Projectiles
{
    public class Projectile : MonoBehaviour
    {
        private GameObject hitFX;

        private ProjectileSpawnArgs args;
        private Vector3 velocity;
        private Vector3 force;

        private float age;
        private int pierce;

        private GameObject owner;

        public static event System.Action<Projectile, RaycastHit> HitEvent;
        public static event System.Action<Projectile, RaycastHit, IDamageable, DamageArgs> DamageEvent;
        public static event System.Action<Projectile> DespawnEvent;
        
        private void Awake()
        {
            hitFX = transform.Find("HitFX").gameObject;
            hitFX.SetActive(false);
        }

        public Projectile SpawnFromPrefab(GameObject owner, ProjectileSpawnArgs args, Vector3 position, Vector3 direction)
        {
            var instance = Instantiate(this, position, Quaternion.LookRotation(direction));

            instance.owner = owner;
            instance.SetupWithArgs(args);
            return instance;
        }

        private void SetupWithArgs(ProjectileSpawnArgs args)
        {
            if (args == null) Destroy(gameObject);
            this.args = args;

            velocity = transform.forward * args.speed;
        }

        private void FixedUpdate()
        {
            Collide();
            Iterate();

            UpdateAge();
        }

        private void UpdateAge()
        {
            if (age > args.lifetime) Despawn(null);

            age += Time.deltaTime;
        }

        private void Collide()
        {
            var ray = new Ray(transform.position, velocity);
            var step = velocity.magnitude * Time.deltaTime;
            var hits = Physics.RaycastAll(ray, step * 1.02f);

            foreach (var hit in hits.OrderBy(e => e.distance))
            {
                HitEvent?.Invoke(this, hit);
                
                var damageable = hit.collider.GetComponentInParent<IDamageable>();
                if (damageable != null)
                {
                    DamageEvent?.Invoke(this, hit, damageable, args.damage);
                    damageable.Damage(new DamageInstance(args.damage, hit.point, -velocity.normalized));
                }

                if (pierce == 0)
                {
                    Despawn(hit);
                    break;
                }

                pierce--;

                SpawnFX(hitFX, hit);
            }
        }

        private void Despawn(RaycastHit? hit)
        {
            if (hit != null) SpawnFX(hitFX, hit.Value);
            DespawnEvent?.Invoke(this);
            Destroy(gameObject);
        }

        private void SpawnFX(GameObject inlinePrefab, RaycastHit hit)
        {
            if (!inlinePrefab) return;
            
            var direction = Vector3.Reflect(velocity.normalized, hit.normal);
            var instance = Instantiate(inlinePrefab, hit.point, Quaternion.LookRotation(direction));
            instance.SetActive(true);
        }

        private void Iterate()
        {
            transform.position += velocity * Time.deltaTime;
            velocity += force * Time.deltaTime;
            force = Physics.gravity * args.gravityScale;
        }
    }
}