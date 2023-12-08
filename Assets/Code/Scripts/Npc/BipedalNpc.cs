using RuckusReloaded.Runtime.Core;
using RuckusReloaded.Runtime.Vitality;
using UnityEngine;
using UnityEngine.AI;

namespace RuckusReloaded.Runtime.Npc
{
    [RequireComponent(typeof(BipedalMovement), typeof(HealthController))]
    [SelectionBase, DisallowMultipleComponent]
    public class BipedalNpc : MonoBehaviour
    {
        private const float PathingThreshold = 1.0f;

        private NavMeshPath navPath;
        private int navPathIndex;

        private bool PathActive => navPathIndex < (navPath?.corners.Length ?? 0);

        public BipedalMovement Movement { get; private set; }
        public HealthController Health { get; private set; }
        public Rigidbody Body => Movement.body;

        public float MoveSpeed { get; set; } = 1.0f;

        private void Awake()
        {
            Movement = GetComponent<BipedalMovement>();
            Health = GetComponent<HealthController>();

            navPath = new NavMeshPath();
        }

        private void FixedUpdate()
        {
            Move();
            UpdatePath();
        }

        private void UpdatePath()
        {
            if (!PathActive) return;

            var corner = navPath.corners[navPathIndex];
            if ((corner - transform.position).magnitude < PathingThreshold)
            {
                navPathIndex++;
            }
        }

        private void Move()
        {
            var targetPosition = PathActive ? navPath.corners[navPathIndex] : transform.position;

            var direction = targetPosition - transform.position;
            direction.y = 0.0f;
            Movement.moveInput = direction * MoveSpeed;
        }

        public void PathTo(Vector3 position)
        {
            if ((position - transform.position).magnitude < PathingThreshold)
            {
                navPath.ClearCorners();
                return;
            }
            
            if (PathActive)
            {
                var end = navPath.corners[^1];
                if ((end - position).magnitude < PathingThreshold) return;
            }

            NavMesh.CalculatePath(transform.position, position, ~0, navPath);
            navPathIndex = 0;
        }

        public void LookIn(Vector3 direction)
        {
            direction.Normalize();
            Movement.viewRotation = new Vector2
            {
                x = Mathf.Atan2(direction.x, direction.z),
                y = Mathf.Asin(direction.y)
            };
        }

        private void OnDrawGizmosSelected()
        {
            if (PathActive)
            {
                Gizmos.color = Color.yellow;
                
                for (var i = navPathIndex; i < navPath.corners.Length - 1; i++)
                {
                    Gizmos.DrawLine(navPath.corners[i], navPath.corners[i + 1]);
                }
                
                for (var i = navPathIndex; i < navPath.corners.Length; i++)
                {
                    Gizmos.DrawSphere(navPath.corners[i], 0.2f);
                }
            }
        }
    }
}