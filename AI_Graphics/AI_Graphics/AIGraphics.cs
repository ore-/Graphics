using AIGraphics.Inspector;
using AIGraphics.Settings;
using BepInEx;
using BepInEx.Configuration;
using KKAPI;
using KKAPI.Studio.SaveLoad;
using System;
using System.Collections;
using UnityEngine;

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
        public static ConfigEntry<string> ConfigCubeMapPath { get; private set; }
        public static ConfigEntry<string> ConfigLensDirtPath { get; private set; }
        public static ConfigEntry<int> ConfigFontSize { get; internal set; }
        public static ConfigEntry<int> ConfigWindowWidth { get; internal set; }
        public static ConfigEntry<int> ConfigWindowHeight { get; internal set; }
        public static ConfigEntry<int> ConfigWindowOffsetX { get; internal set; }
        public static ConfigEntry<int> ConfigWindowOffsetY { get; internal set; }

        private bool _showGUI;
        private CursorLockMode _previousCursorLockState;
        private bool _previousCursorVisible;

        private GameObject _skyboxGO, _postGO;
        private SkyboxManager _skyboxManager;
        private FocusPuller _focusPuller;
        private LightManager _lightManager;
        private PostProcessingManager _postProcessingManager;
        private Inspector.Inspector _inspector;

        internal GlobalSettings Settings { get; private set; }
        internal CameraSettings CameraSettings { get; private set; }
        internal LightingSettings LightingSettings { get; private set; }
        internal PostProcessingSettings PostProcessingSettings { get; private set; }
        
        internal static AIGraphics Instance { get; private set; }

        public AIGraphics()
        {
            if (Instance != null)
                throw new InvalidOperationException("Can only create one instance of AIGraphics");
            Instance = this;

            ConfigCubeMapPath = Config.Bind("Config", "Cubemap path", Application.dataPath + "/../cubemaps/", new ConfigDescription("Where cubemaps are stored"));
            ConfigLensDirtPath = Config.Bind("Config", "Lens dirt texture path", Application.dataPath + "/../lensdirts/", new ConfigDescription("Where lens dirt textures are stored"));
            ConfigFontSize = Config.Bind("Config", "Font Size", 12, new ConfigDescription("Font Size"));
            GUIStyles.FontSize = ConfigFontSize.Value;
            ConfigWindowWidth = Config.Bind("Config", "Window Width", 750, new ConfigDescription("Window Width"));
            Inspector.Inspector.Width = ConfigWindowWidth.Value;
            ConfigWindowHeight = Config.Bind("Config", "Window Height", 1024, new ConfigDescription("Window Height"));
            Inspector.Inspector.Height = ConfigWindowHeight.Value;
            ConfigWindowOffsetX = Config.Bind("Config", "Window Position Offset X", (Screen.width - ConfigWindowWidth.Value) / 2, new ConfigDescription("Window Position Offset X"));
            Inspector.Inspector.StartOffsetX = ConfigWindowOffsetX.Value;
            ConfigWindowOffsetY = Config.Bind("Config", "Window Position Offset Y", (Screen.height - ConfigWindowHeight.Value) / 2, new ConfigDescription("Window Position Offset Y"));
            Inspector.Inspector.StartOffsetY = ConfigWindowOffsetY.Value;
        }

        private void Awake()
        {
            StudioSaveLoadApi.RegisterExtraBehaviour<SceneController>(GUID);
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

            Settings = new GlobalSettings();
            CameraSettings = new CameraSettings();
            LightingSettings = new LightingSettings();
            PostProcessingSettings = new PostProcessingSettings(CameraSettings.MainCamera);            

            _skyboxGO = new GameObject("SkyboxManager");
            _skyboxManager = _skyboxGO.AddComponent<SkyboxManager>();
            _skyboxManager.Parent = this;
            _skyboxManager.Camera = CameraSettings.MainCamera;
            _skyboxManager.CubemapPath = ConfigCubeMapPath.Value;
            _skyboxManager.Logger = Logger;

            _postGO = new GameObject("PostProcessingManager");
            _postProcessingManager = _postGO.AddComponent<PostProcessingManager>();
            _postProcessingManager.LensDirtTexturesPath = ConfigLensDirtPath.Value;

            _focusPuller = CameraSettings.MainCamera.gameObject.AddComponent<FocusPuller>();
            _focusPuller.init(this, CameraSettings.MainCamera);

            _lightManager = new LightManager(this);
            
            _inspector = new Inspector.Inspector(this);            
        }

        internal SkyboxManager SkyboxManager { get => _skyboxManager; }
        internal PostProcessingManager PostProcessingManager { get => _postProcessingManager; }
        internal LightManager LightManager { get => _lightManager; }
        internal FocusPuller FocusPuller { get => _focusPuller; }

        internal void OnGUI()
        {
            if (Show)
            {
                Cursor.lockState = CursorLockMode.None;
                Cursor.visible = true;

                var originalSkin = GUI.skin;
                GUI.skin = GUIStyles.Skin;
                _inspector.DrawWindow();
                GUI.skin = originalSkin;
            }
        }

        internal void Update()
        {
            if (KKAPI.KoikatuAPI.GetCurrentGameMode() != KKAPI.GameMode.Maker && KKAPI.KoikatuAPI.GetCurrentGameMode() != KKAPI.GameMode.Studio )
                return;

            if( Input.GetKeyDown(ShowHotkey))
                Show = !Show;

            if (Show)
            {
                Cursor.lockState = CursorLockMode.None;
                Cursor.visible = true;
            }
        }

        internal void LateUpdate()
        {
            if (Show)
            {
                Cursor.lockState = CursorLockMode.None;
                Cursor.visible = true;
            }
        }

        private bool Show
        {
            get => _showGUI;
            set
            {
                if (_showGUI != value)
                {
                    if (value)
                    {
                        _previousCursorLockState = Cursor.lockState;
                        _previousCursorVisible = Cursor.visible;
                    }
                    else
                    {
                        if (!_previousCursorVisible || _previousCursorLockState != CursorLockMode.None)
                        {
                            Cursor.lockState = _previousCursorLockState;
                            Cursor.visible = _previousCursorVisible;
                        }
                    }
                    _showGUI = value;
                }
            }
        }
    }
}