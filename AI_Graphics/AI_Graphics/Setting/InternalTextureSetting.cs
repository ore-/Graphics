using System;
using UnityEngine;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using System.IO;

namespace AIGraphics.Setting {
    class InternalTextureSetting {
        private string bundlePath = "";
        private int index = 0;
        public List<string> TexturePaths {
            get; set;
        }
        private List<Texture2D> Textures {
            get; set;
        }
        public string BundlePath {
            get => bundlePath;
            set => bundlePath = value;
        }
        public int Index {
            get => index;
            set => index = value;
        }
        public bool TryGetTexture(out Texture2D texture) {
            if (index < 0 || index >= Textures.Count()) {
                texture = null;
                return false;
            }
            texture = Textures[Index];
            return true;
        }

        public InternalTextureSetting(string bundlePath, List<string> paths) {
            BundlePath = bundlePath;

            List<Texture2D> textures = new List<Texture2D>();
            paths.ForEach(asset => {
                Texture2D texture = CommonLib.LoadAsset<Texture2D>(bundlePath, asset, false, string.Empty);
                if (texture != null)
                    textures.Add(texture);
            });

            Textures = textures;
            TexturePaths = paths;
        }
    }
}
