using UnityEngine;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using System.IO;

namespace AIGraphics
{
    internal class PostProcessingManager : MonoBehaviour
    {
        private string _lensDirtTexturePath;
        internal List<string> LensDirtPaths { get; private set; }
        internal List<Texture2D> LensDirtPreviews { get; private set; }
        internal List<Texture2D> LensDirts { get; private set; }

        internal string LensDirtTexturesPath
        {
            get => _lensDirtTexturePath;
            set
            {
                _lensDirtTexturePath = value;
                LoadLensDirts();
            }
        }

        private IEnumerator LoadLensDirtPreview(string filePath)
        {
            byte[] textureByte = File.ReadAllBytes(filePath);
            yield return textureByte;
            if (null == textureByte) yield break;
            Texture2D texture = KKAPI.Utilities.TextureUtils.LoadTexture(textureByte);
            LensDirts.Add(texture);
            Texture2D preview = new Texture2D(texture.width, texture.height, texture.format, false);
            Graphics.CopyTexture(texture, preview);
            Util.ResizeTexture(preview, 64, 64);
            LensDirtPreviews.Add(preview);            
            LensDirtPaths.Add(filePath);
            texture = null;
            yield break;
        }

        private IEnumerator LoadLensDirt(string filePath, Texture texture)
        {
            byte[] textureByte = File.ReadAllBytes(filePath);
            yield return textureByte;
            if (null == textureByte) yield break;
            texture = KKAPI.Utilities.TextureUtils.LoadTexture(textureByte);
            yield break;
        }

        internal void LensDirt(string filePath, Texture texture)
        {
            StartCoroutine(LoadLensDirt(filePath, texture));
        }

        private void LoadLensDirts()
        {
            FolderAssist LensDirtFolder = new FolderAssist();
            LensDirtFolder.CreateFolderInfo(_lensDirtTexturePath, "*.png", true, true);
            List<string> paths = LensDirtFolder.lstFile.Select(file => file.FullPath).ToList<string>();
            LensDirtPaths = new List<string>();
            LensDirtPreviews = new List<Texture2D>();
            LensDirts = new List<Texture2D>();
            foreach (string path in paths)
                StartCoroutine(LoadLensDirtPreview(path));
        }
    }
}
