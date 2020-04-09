using UnityEngine;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using System.IO;

namespace AIGraphics.Setting {
    class TextureSetting {
        public string lookupPath;  // need to call LoadTextures() after stting look up path.
        public List<string> TexturePaths {
            get; set;
        } 
        public List<Texture2D> Textures {
            get; set;
        }
        public List<Texture2D> TexturePreviews {
            get; set;
        }
        public int index = 0;
        public Texture2D Texture {
            get => index > 0 ? Textures.ToArray()[index] : null;
        }
        public string TexturePath {
            get => index > 0 ? TexturePaths.ToArray()[index] : null;
        }
        public bool TryGetTexture(int index, out Texture2D texture) {
            if (index < 0 || index >= Textures.Count()) {
                texture = null;
                return false;
            }
            texture = Textures.ToArray()[index];
            return true;
        }
        public int FindIndexByPath(string path) => TexturePaths.FindIndex(x => x.Equals(path));
        public void SetIndexByPath(string path) => index = FindIndexByPath(path);
        public IEnumerator LoadPreview(string filePath) {
            byte[] textureByte = File.ReadAllBytes(filePath);
            yield return textureByte;
            if (null == textureByte)
                yield break;
            Texture2D texture = KKAPI.Utilities.TextureUtils.LoadTexture(textureByte);
            Textures.Add(texture);
            Texture2D preview = new Texture2D(texture.width, texture.height, texture.format, false);
            Graphics.CopyTexture(texture, preview);
            Util.ResizeTexture(preview, 64, 64);
            TexturePreviews.Add(preview);
            TexturePaths.Add(filePath);
            texture = null;
            yield break;
        }
        internal IEnumerator LoadTexture(string filePath, Texture texture) {
            byte[] textureByte = File.ReadAllBytes(filePath);
            yield return textureByte;
            if (null == textureByte)
                yield break;
            texture = KKAPI.Utilities.TextureUtils.LoadTexture(textureByte);
            yield break;
        }
        public void LoadTexture(MonoBehaviour component, string filePath, Texture texture) {
            component.StartCoroutine(LoadTexture(filePath, texture));
        }
        public void LoadTextures(MonoBehaviour component) {
            FolderAssist LookupFolder = new FolderAssist();
            LookupFolder.CreateFolderInfo(lookupPath, "*.png", true, true);
            List<string> paths = LookupFolder.lstFile.Select(file => file.FullPath).ToList<string>();
            TexturePaths = new List<string>();
            TexturePreviews = new List<Texture2D>();
            Textures = new List<Texture2D>();
            foreach (string path in paths)
                component.StartCoroutine(LoadPreview(path));
        }
    }
}
