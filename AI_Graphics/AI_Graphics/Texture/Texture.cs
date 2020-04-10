using System.Collections.Generic;
using UnityEngine;

namespace AI_Graphics.Texture
{
    internal abstract class Texture
    {
        private string assetPath;
        public List<string> TexturePaths { get; set; }
        public List<Texture2D> Textures { get; set; }
        public List<Texture2D> TexturePreviews { get; set; }
    }
}
