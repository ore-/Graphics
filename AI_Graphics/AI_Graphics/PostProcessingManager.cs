using AIGraphics.Textures;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace AIGraphics
{

    internal class PostProcessingManager : MonoBehaviour
    {
        private AIGraphics _parent;
        private ExternalTextureManager _lensDirtManager;
        private InternalTextureManager _lutManager;
        private readonly string _lutAssetPath = "studio/lut/00.unity3d";
        private readonly List<string> _lutTexturePaths = new List<string>() {
                "Lut_Aliens", "Lut_Art", "Lut_Bright", "Lut_Cold", "Lut_Comic", "Lut_DarkGreyish", "Lut_Deep",
                "Lut_Default", "Lut_Desert", "Lut_Dull", "Lut_Fog", "Lut_Greyish", "Lut_InvertMono", "Lut_Invert",
                "Lut_LimitBlue", "Lut_LimitGreen", "Lut_LimitRed", "Lut_MonoBlue", "Lut_Monochrome", "Lut_MonoGreen",
                "Lut_MonoRed", "Lut_OldPoster1", "Lut_OldPoster2", "Lut_Pale", "Lut_Posterize", "Lut_RobotSalvation",
                "Lut_Sarmo", "Lut_Sepia", "Lut_Soft", "Lut_Strong", "Lut_TimeDay", "Lut_TimeNight", "Lut_TimeSundown",
                "Lut_Vibrance_D", "Lut_Warm"
        };

        internal string LensDirtTexturesPath
        {
            get => _lensDirtManager.AssetPath;
            set => _lensDirtManager.AssetPath = value;
        }

        internal Texture CurrentLensDirtTexture => _lensDirtManager.CurrentTexture;

        internal string CurrentLensDirtTexturePath => _lensDirtManager.CurrentTexturePath;

        internal int CurrentLensDirtTextureIndex => _lensDirtManager.CurrentTextureIndex;

        internal Texture[] LensDirtPreviews => _lensDirtManager.PreviewArray;

        internal void LoadLensDirtTexture(int index, Action<Texture> onChanged = null)
        {
            _lensDirtManager.LoadTexture(index, onChanged);
        }
        internal void LoadLensDirtTexture(string path, Action<Texture> onChanged = null)
        {
            StartCoroutine(_lensDirtManager.LoadTexture(path, onChanged));
        }

        internal Texture CurrentLUTTexture => _lutManager.CurrentTexture;

        internal string CurrentLUTName => _lutManager.CurrentTextureName;

        internal int CurrentLUTIndex => _lutManager.CurrentTextureIndex;

        internal string[] LUTNames => _lutManager.TextureNames;

        internal Texture LoadLUT(int index)
        {
            return _lutManager.GetTexture(index);
        }

        internal Texture LoadLUT(string name)
        {
            return _lutManager.GetTexture(name);
        }

        internal AIGraphics Parent
        {
            get => _parent;
            set
            {
                _parent = value;
                _lensDirtManager = _parent.GetOrAddComponent<ExternalTextureManager>();
                _lensDirtManager.SearchPattern = "*.png";
                _lutManager = _parent.GetOrAddComponent<InternalTextureManager>();
                _lutManager.TexturePaths = _lutTexturePaths;
                _lutManager.SearchPattern = "Lut_";
                _lutManager.AssetPath = _lutAssetPath;
            }
        }
    }
}