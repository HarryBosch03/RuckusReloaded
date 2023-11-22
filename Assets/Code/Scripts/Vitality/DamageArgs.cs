
namespace RuckusReloaded.Runtime.Vitality
{
    [System.Serializable]
    public class DamageArgs
    {
        public float damage = 1;
        public bool ignoreLocationalDamage;

        public static implicit operator DamageArgs(int damage) => new()
        {
            damage = damage,
        };
    }
}