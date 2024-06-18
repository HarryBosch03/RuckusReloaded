using System.Collections;
using UnityEngine;
using UnityEngine.Rendering.Universal;

namespace RuckusReloaded.Runtime.VFX
{
    [SelectionBase, DisallowMultipleComponent]
    public sealed class GunHitDecal : MonoBehaviour
    {
        public float preDelay = 1.0f;
        public float fadeDuration = 2.0f;
        public float postDelay = 0.0f;

        private DecalProjector decal;

        private void Awake() { decal = GetComponentInChildren<DecalProjector>(); }

        private void Start()
        {
            StartCoroutine(routine());
            
            IEnumerator routine()
            {
                yield return new WaitForSeconds(preDelay);

                var a = 1.0f;
                while (a > 0.0f)
                {
                    decal.fadeFactor = a;

                    a -= Time.deltaTime / fadeDuration;
                    yield return null;
                }

                yield return new WaitForSeconds(postDelay);
                Destroy(gameObject);
            }
        }
    }
}