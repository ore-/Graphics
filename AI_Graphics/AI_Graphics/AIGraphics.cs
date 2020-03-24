using AIGraphics.Inspector;
using BepInEx;
using BepInEx.Configuration;
using KKAPI;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.SceneManagement;

namespace AIGraphics
{
    [BepInIncompatibility("dhhai4mod")]    
    [BepInPlugin(GUID, PluginName, Version)]
    [BepInDependency(KoikatuAPI.GUID, KoikatuAPI.VersionConst)]
    public class AIGraphics : BaseUnityPlugin
    {
        public const string GUID = "ore.ai.graphics";
        public const string PluginName = "AI Graphics";
        public const string Version = "0.1.0";

        public KeyCode ShowHotkey { get; set; } = KeyCode.F5;

        internal static AIGraphics Instance { get; private set; }

        private Inspector.Inspector Inspector;

        internal AIGraphics()
        {
            if (Instance != null)
                throw new InvalidOperationException("Can only create one instance of the AIGraphics");

            Instance = this;
        }

        private IEnumerator Start()
        {
            yield return new WaitUntil(() =>
            {
                switch (KKAPI.KoikatuAPI.GetCurrentGameMode())
                {
                    case KKAPI.GameMode.Maker:
                        return KKAPI.Maker.MakerAPI.InsideAndLoaded;
                    case KKAPI.GameMode.Studio:
                        return KKAPI.Studio.StudioAPI.StudioLoaded;
                    default:
                        return false;
                }
            });
            
            Inspector = new Inspector.Inspector(ShowHotkey);
        }

        internal void OnGUI()
        {
            Inspector?.OnGUI();
        }

        internal void Update()
        {
            Inspector?.Update();
        }

        internal  void LateUpdate()
        {
            Inspector?.LateUpdate();
        }
    }
}