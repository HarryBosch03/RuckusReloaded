using System.Collections;
using System.Collections.Generic;
using RuckusReloaded.Runtime.Core;
using RuckusReloaded.Runtime.Npc.StateMachines;
using RuckusReloaded.Runtime.Projectiles;
using RuckusReloaded.Runtime.Vitality;
using UnityEngine;

namespace RuckusReloaded.Runtime.Npc.Enemies.States
{
    [System.Serializable]
    public class ShootAtAgro : State<BipedalNpc>
    {
        public float windUp;
        public int projectiles;
        public float delay;
        public float windDown;
        
        [Space]
        public ProjectileSpawnArgs projectileSpawnArgs;
        public Projectile projectile;
        public Transform spawnPoint;

        public Transition next;
        private GameObject agro;
        private IEnumerator routine;

        public override void Enter()
        {
            agro = Blackboard.Get<GameObject>("agro");
            if (!agro)
            {
                sm.ChangeState(next());
                return;
            }

            routine = Routine();
        }

        private IEnumerator Routine()
        {
            yield return wait(windUp);

            for (var i = 0; i < projectiles; i++)
            {
                SpawnProjectile();
                yield return wait(delay);
            }
            
            yield return wait(windDown);

            IEnumerator wait(float secconds)
            {
                var t = 0.0f;
                while (t < secconds)
                {
                    t += Time.deltaTime;
                    yield return null;
                }
            }
        }

        private void SpawnProjectile()
        {
            var direction = (IPersonality.LookTargetOf(agro) - spawnPoint.position).normalized;
            projectile.SpawnFromPrefab(Target.gameObject, projectileSpawnArgs, spawnPoint.position, direction);
        }

        public override void FixedUpdate()
        {
            if (routine.MoveNext())
            {
                sm.ChangeState(next());
            }
            
            Target.LookAt(agro);
        }
    }
}