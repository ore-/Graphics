using BepInEx;
using BepInEx.Configuration;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.SceneManagement;

namespace AILight
{
    [BepInIncompatibility("dhhai4mod")]
    [BepInDependency("RuntimeUnityEditor")]
    [BepInPlugin(GUID, PluginName, Version)]
    public partial class AILight : BaseUnityPlugin
    {
        public const string GUID = "ore.ai.graphics";
        public const string PluginName = "AI Graphics";
        public const string Version = "0.1.0";

        public KeyCode ShowHotkey { get; set; } = KeyCode.F5;
    }
}