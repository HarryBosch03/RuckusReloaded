using System;
using UnityEngine;

namespace RuckusReloaded.Runtime.Rendering
{
    [SelectionBase, DisallowMultipleComponent]
    public sealed class ThickSprite : MonoBehaviour
    {
        public const float ppu = 16.0f;
        public const float spinSpeed = 1.0f;
        
        public Texture2D texture;

        private Material material;
        private Mesh mesh;

        private void Awake()
        {
            mesh
        }
    }
}
