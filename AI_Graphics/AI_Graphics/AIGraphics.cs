using AIGraphics.Inspector;
using AIGraphics.Settings;
using AIGraphics.Textures;
using BepInEx;
using BepInEx.Configuration;
using KKAPI;
using KKAPI.Studio.SaveLoad;
using System;
using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace AIGraphics
{
    [BepInIncompatibility("dhhai4mod")]
    [BepInPlugin(GUID, PluginName, Version)]
    [BepInDependency(KoikatuAPI.GUID, KoikatuAPI.VersionConst)]
    [BepInDependency(ExtensibleSaveFormat.ExtendedSave.GUID)]
    public class AIGraphics : BaseUnityPlugin
    {
        public const string GUID = "ore.ai.graphics";
        public const string PluginName = "AI Graphics";
        public const string Version = "0.2.0";

        public KeyCode ShowHotkey { get; set; } = KeyCode.F5;
        public static ConfigEntry<string> ConfigCubeMapPath { get; private set; }
        public static ConfigEntry<string> ConfigPresetPath { get; private set; }
        public static ConfigEntry<string> ConfigLensDirtPath { get; private set; }
        public static ConfigEntry<int> ConfigFontSize { get; internal set; }
        public static ConfigEntry<int> ConfigWindowWidth { get; internal set; }
        public static ConfigEntry<int> ConfigWindowHeight { get; internal set; }
        public static ConfigEntry<int> ConfigWindowOffsetX { get; internal set; }
        public static ConfigEntry<int> ConfigWindowOffsetY { get; internal set; }

        public Preset preset;

        private bool _showGUI;
        private bool _isLoaded = false;
        private CursorLockMode _previousCursorLockState;
        private bool _previousCursorVisible;
        private float _previousTimeScale;

        private SkyboxManager _skyboxManager;
        private FocusPuller _focusPuller;
        private LightManager _lightManager;
        private PostProcessingManager _postProcessingManager;
        private PresetManager _presetManager;
        private Inspector.Inspector _inspector;

        internal GlobalSettings Settings { get; private set; }
        internal CameraSettings CameraSettings { get; private set; }
        internal LightingSettings LightingSettings { get; private set; }
        internal PostProcessingSettings PostProcessingSettings { get; private set; }

        public static AIGraphics Instance { get; private set; }

        public AIGraphics()
        {
            if (Instance != null)
            {
                throw new InvalidOperationException("Can only create one instance of AIGraphics");
            }

            Instance = this;

            ConfigPresetPath = Config.Bind("Config", "Preset Location", Application.dataPath + "/../presets/", new ConfigDescription("Where presets are stored"));
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
            SceneManager.sceneLoaded += OnSceneLoaded;
        }

        private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
        {
            if (scene.name == "map_title" && PostProcessingSettings != null)
            {
                PostProcessingSettings.ResetVolume();
            }
            else
            {
                StartCoroutine(InitializeLight(scene));
            }
            CullingMaskExtensions.RefreshLayers();
        }

        private IEnumerator InitializeLight(Scene scene)
        {
            yield return new WaitUntil(() => _lightManager != null);

            if (KKAPI.GameMode.Maker == KKAPI.KoikatuAPI.GetCurrentGameMode() && scene.name == "CharaCustom")
            {
                GameObject lights = GameObject.Find("CharaCustom/CustomControl/CharaCamera/Main Camera/Lights Custom");
                if (lights != null)
                {
                    Transform backLight = lights.transform.Find("Directional Light Back");
                    if (backLight != null)
                    {
                        Light light = backLight.GetComponent<Light>();
                        if (light != null)
                        {
                            light.enabled = false;
                        }
                    }
                }
            }

            LightManager.Light();
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
                    case KKAPI.GameMode.MainGame:
                        return null != GameObject.Find("MapScene") && SceneManager.GetActiveScene().isLoaded && null != Camera.main; //KKAPI doesn't provide an api for in game check 
                    default:
                        return false;
                }
            });

            Settings = new GlobalSettings();
            CameraSettings = new CameraSettings();
            LightingSettings = new LightingSettings();
            PostProcessingSettings = new PostProcessingSettings(CameraSettings.MainCamera);

            _skyboxManager = Instance.GetOrAddComponent<SkyboxManager>();
            _skyboxManager.Parent = this;
            _skyboxManager.AssetPath = ConfigCubeMapPath.Value;
            _skyboxManager.Logger = Logger;
            DontDestroyOnLoad(_skyboxManager);

            _postProcessingManager = Instance.GetOrAddComponent<PostProcessingManager>();
            _postProcessingManager.Parent = this;
            _postProcessingManager.LensDirtTexturesPath = ConfigLensDirtPath.Value;
            DontDestroyOnLoad(_postProcessingManager);

            _lightManager = new LightManager(this);

            _focusPuller = Instance.GetOrAddComponent<FocusPuller>();
            _focusPuller.init(this);
            DontDestroyOnLoad(_focusPuller);
            _presetManager = new PresetManager(ConfigPresetPath.Value, this);

            _inspector = new Inspector.Inspector(this);

            // It takes some time to fully loaded in studio to save/load stuffs.
            yield return new WaitUntil(() =>
            {
                return (KoikatuAPI.GetCurrentGameMode() == GameMode.Studio) ? KKAPI.Studio.StudioAPI.InsideStudio && _skyboxManager != null : true;
            });

            _isLoaded = true;
        }

        internal SkyboxManager SkyboxManager => _skyboxManager;
        internal PostProcessingManager PostProcessingManager => _postProcessingManager;
        internal LightManager LightManager => _lightManager;
        internal FocusPuller FocusPuller => _focusPuller;
        internal PresetManager PresetManager => _presetManager;

        internal void OnGUI()
        {
            if (Show)
            {
                Cursor.lockState = CursorLockMode.None;
                Cursor.visible = true;

                GUISkin originalSkin = GUI.skin;
                GUI.skin = GUIStyles.Skin;
                _inspector.DrawWindow();
                GUI.skin = originalSkin;
            }
        }

        internal void Update()
        {
            if (!_isLoaded)
            {
                return;
            }

            if (Input.GetKeyDown(ShowHotkey))
            {
                Show = !Show;
            }

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
                        if (KKAPI.KoikatuAPI.GetCurrentGameMode() == KKAPI.GameMode.MainGame)
                        {
                            _previousTimeScale = Time.timeScale;
                            Time.timeScale = 0;
                        }
                        _previousCursorLockState = Cursor.lockState;
                        _previousCursorVisible = Cursor.visible;
                    }
                    else
                    {
                        if (KKAPI.KoikatuAPI.GetCurrentGameMode() == KKAPI.GameMode.MainGame)
                        {
                            Time.timeScale = _previousTimeScale;
                        }

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