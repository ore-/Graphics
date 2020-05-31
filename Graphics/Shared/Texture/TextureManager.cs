using System;
using System.Collections.Generic;
using UnityEngine;

namespace Graphics.Textures
{
    internal abstract class TextureManager : MonoBehaviour
    {
        private string _assetPath;
        internal int _assetsToLoad = 0;
        internal bool HasAssetsLoaded => 0 == _assetsToLoad;
        internal List<Texture> Previews { get; set; }
        internal Texture[] PreviewArray { get; set; }
        internal abstract string SearchPattern { get; set; }
        internal virtual List<string> TexturePaths { get; set; }
        internal virtual Texture CurrentTexture { get; set; }
        internal virtual string CurrentTexturePath { get; set; }
        internal virtual int CurrentTextureIndex
        {
            get => Array.IndexOf(TexturePaths.ToArray(), CurrentTexturePath);
            set => throw new System.NotImplementedException();
        }

        internal abstract System.Collections.IEnumerator LoadTexture(string path, Action<Texture> onChanged = null);
        internal abstract System.Collections.IEnumerator LoadPreview(string path);

        internal string AssetPath
        {
            get => _assetPath;
            set
            {
                _assetPath = value;
                StartCoroutine(LoadAssets());
            }
        }

        internal virtual System.Collections.IEnumerator LoadAssets()
        {
            CurrentTexturePath = "";
            List<string> paths = Util.GetFiles(AssetPath, SearchPattern);
            //populate TexturePaths in each preview load to ensure index is consistent between TexturePaths and Previews
            TexturePaths = new List<string>();
            Previews = new List<Texture>();
            _assetsToLoad = paths.Count;
            foreach (string path in paths)
            {
                StartCoroutine(LoadPreview(path));
            }
            yield return new WaitUntil(() => HasAssetsLoaded);
            PreviewArray = Previews.ToArray();
        }
    }
}
