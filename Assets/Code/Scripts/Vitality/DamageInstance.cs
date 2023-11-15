
using UnityEngine;

namespace RuckusReloaded.Runtime.Vitality
{
    public class DamageInstance
    {
        public DamageArgs args;
        public Vector3 point;
        public Vector3 normal;
        public float locationalDamage = 1.0f;

        public DamageInstance(DamageArgs args, Vector3 point, Vector3 normal)
        {
            this.args = args;
            this.point = point;
            this.normal = normal;
        }
        
        
        public int Calculate()
        {
            var damage = args.damage;

            if (!args.ignoreLocationalDamage) damage *= locationalDamage; 
            
            return Mathf.Max(1, Mathf.FloorToInt(damage));
        }
    }
}