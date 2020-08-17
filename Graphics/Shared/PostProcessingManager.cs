﻿using Graphics.Textures;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace Graphics
{

    internal class PostProcessingManager : MonoBehaviour
    {
        private Graphics _parent;
        private ExternalTextureManager _lensDirtManager;
        private InternalTextureManager _lutManager;
        private readonly string _lutAssetPath = "studio/lut/00.unity3d";
        private readonly List<string> _lutTexturePaths = new List<string>()
        {
                "Lut_Default", "Lut_TimeDay", "Lut_TimeSundown", "Lut_TimeNight", "Lut_Warm", "Lut_Cold",
                "Lut_Dull", "Lut_Pale", "Lut_Soft", "Lut_Strong", "Lut_Deep", "Lut_Bright", "Lut_Sepia", "Lut_Monochrome",
                "Lut_MonoRed", "Lut_MonoBlue", "Lut_MonoGreen", "Lut_LimitRed", "Lut_LimitBlue", "Lut_LimitGreen",
                "Lut_InvertMono", "Lut_Invert", "Lut_Sarmo", "Lut_Posterize",
                "Lut_OldPoster1", "Lut_OldPoster2", "Lut_RobotSalvation", "Lut_Greyish", "Lut_DarkGreyish",
                "Lut_Art", "Lut_Comic", "Lut_Aliens", "Lut_Fog", "Lut_Desert", "Lut_Vibrance_D"
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

        internal string CurrentLUTName => _lutManager.CurrentTextureName.IsNullOrEmpty() ? LUTNames[0] : _lutManager.CurrentTextureName;

        internal int CurrentLUTIndex => _lutManager.CurrentTextureIndex >= 0 ? _lutManager.CurrentTextureIndex : 0;

        internal string[] LUTNames => _lutManager.TextureNames;

        internal Texture LoadLUT(int index)
        {
            return _lutManager.GetTexture(index);
        }

        internal Texture LoadLUT(string name)
        {
            return _lutManager.GetTexture(name);
        }

        internal Graphics Parent
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