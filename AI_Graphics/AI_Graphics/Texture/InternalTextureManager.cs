using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace AIGraphics.Textures
{
    internal class InternalTextureManager : TextureManager
    {
        private List<string> _texturePaths;
        internal string[] TextureNames { get; set; }
        internal List<Texture> Textures { get; set; }
        internal override string SearchPattern { get; set; }

        internal override IEnumerator LoadPreview(string path)
        {
            throw new NotImplementedException();
        }

        internal override IEnumerator LoadTexture(string path, Action<Texture> onChanged = null)
        {
            throw new NotImplementedException();
        }

        internal override System.Collections.IEnumerator LoadAssets()
        {
            CurrentTexturePath = "";
            Textures = new List<Texture>();
            foreach (string asset in TexturePaths)
            {
                Texture2D texture = CommonLib.LoadAsset<Texture2D>(AssetPath, asset, false, string.Empty);
                yield return texture;
                if (texture != null)
                {
                    Textures.Add(texture);
                }
                _assetsToLoad--;
            }
            yield return new WaitUntil(() => HasAssetsLoaded);
            TextureNames = TexturePaths.Select(path => getName(path)).ToArray();
        }

        internal string CurrentTextureName => getName(CurrentTexturePath);

        private string getName(string path)
        {
            string name = path.StartsWith(SearchPattern) ? path.Remove(0, SearchPattern.Length) : path;
            return name;
        }

        internal Texture GetTexture(string textureName)
        {
            int index = TexturePaths.IndexOf(SearchPattern + textureName);
            return GetTexture(index);
        }

        internal Texture GetTexture(int index)
        {
            if (0 <= index)
            {
                CurrentTexture = Textures[index];
                CurrentTexturePath = TexturePaths[index];
                return CurrentTexture;
            }
            return null;
        }

        internal override List<string> TexturePaths
        {
            get => _texturePaths;
            set
            {
                _texturePaths = value;
                _assetsToLoad = _texturePaths.Count;
            }
        }
    }
}
