using RuckusReloaded.Runtime.Vitality;

namespace RuckusReloaded.Runtime.Projectiles
{
    [System.Serializable]
    public class ProjectileSpawnArgs
    {
        public DamageArgs damage;
        public float speed = 100.0f;
        public float lifetime = 2.0f;
        public float gravityScale = 1.0f;
        public int pierce = 0;
    }
}