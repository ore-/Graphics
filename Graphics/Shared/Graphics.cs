using BepInEx;
using BepInEx.Configuration;
using Graphics.Inspector;
using Graphics.Patch;
using Graphics.Settings;
using Graphics.Textures;
using KKAPI;
using KKAPI.Chara;
using KKAPI.Studio.SaveLoad;
using System;
using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Graphics
{
    [BepInIncompatibility("dhhai4mod")]
    [BepInPlugin(GUID, PluginName, Version)]
    [BepInDependency(KoikatuAPI.GUID, KoikatuAPI.VersionConst)]
    [BepInDependency(ExtensibleSaveFormat.ExtendedSave.GUID)]
    public partial class Graphics : BaseUnityPlugin
    {
        public const string GUID = "ore.graphics";
        public const string PluginName = "Graphics";
        public const string Version = "0.3.3";

        public static ConfigEntry<KeyCode> ConfigShortcut { get; private set; }
        public static ConfigEntry<string> ConfigCubeMapPath { get; private set; }
        public static ConfigEntry<string> ConfigPresetPath { get; private set; }
        public static ConfigEntry<string> ConfigLensDirtPath { get; private set; }
        public static ConfigEntry<string> ConfigLocalizationPath { get; private set; }
        public static ConfigEntry<LocalizationManager.Language> ConfigLanguage { get; private set; }
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
#if AI
        private float _previousTimeScale;
#endif 
        private SkyboxManager _skyboxManager;
        private LightManager _lightManager;
        private PostProcessingManager _postProcessingManager;
        private PresetManager _presetManager;
        private SSSManager _sssManager;
        private Inspector.Inspector _inspector;

        internal GlobalSettings Settings { get; private set; }
        internal CameraSettings CameraSettings { get; private set; }
        internal LightingSettings LightingSettings { get; private set; }
        internal PostProcessingSettings PostProcessingSettings { get; private set; }
        internal SSSSettings SSSSettings { get; private set; }

        internal BepInEx.Logging.ManualLogSource Log => Logger;

        public static Graphics Instance { get; private set; }

        public Graphics()
        {
            if (Instance != null)
            {
                throw new InvalidOperationException("Can only create one instance of Graphics");
            }

            Instance = this;

            ConfigShortcut = Config.Bind("Config", "Keyboard Shortcut", KeyCode.F5, new ConfigDescription("Keyboard Shortcut"));
            ConfigPresetPath = Config.Bind("Config", "Preset Location", Application.dataPath + "/../presets/", new ConfigDescription("Where presets are stored"));
            ConfigCubeMapPath = Config.Bind("Config", "Cubemap path", Application.dataPath + "/../cubemaps/", new ConfigDescription("Where cubemaps are stored"));
            ConfigLensDirtPath = Config.Bind("Config", "Lens dirt texture path", Application.dataPath + "/../lensdirts/", new ConfigDescription("Where lens dirt textures are stored"));
            ConfigLocalizationPath = Config.Bind("Config", "Localization path", Application.dataPath + "/../BepInEx/plugins/Graphics/Resources/", new ConfigDescription("Where localizations are stored"));
            ConfigLanguage = Config.Bind("Config", "Language", LocalizationManager.DefaultLanguage(), new ConfigDescription("Default Language"));
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
            CharacterApi.RegisterExtraBehaviour<CharaController>(GUID);
            StudioSaveLoadApi.RegisterExtraBehaviour<SceneController>(GUID);
            SceneManager.sceneLoaded += OnSceneLoaded;
        }

        private IEnumerator Start()
        {
            if (IsStudio()) StudioHooks.Init();
            yield return new WaitUntil(IsLoaded);
            Settings = new GlobalSettings();
            CameraSettings = new CameraSettings();
            LightingSettings = new LightingSettings();
            PostProcessingSettings = new PostProcessingSettings(CameraSettings.MainCamera);
            SSSSettings = new SSSSettings();
            
            _sssManager = new SSSManager();
            _sssManager.Initialize();
            
            _skyboxManager = Instance.GetOrAddComponent<SkyboxManager>();
            _skyboxManager.Parent = this;
            _skyboxManager.AssetPath = ConfigCubeMapPath.Value;
            _skyboxManager.Logger = Logger;
            DontDestroyOnLoad(_skyboxManager);

            _postProcessingManager = Instance.GetOrAddComponent<PostProcessingManager>();
            _postProcessingManager.Parent = this;
            _postProcessingManager.LensDirtTexturesPath = ConfigLensDirtPath.Value;
            DontDestroyOnLoad(_postProcessingManager);

            LocalizationManager.Parent = this;
            LocalizationManager.CurrentLanguage = ConfigLanguage.Value;

            _lightManager = new LightManager(this);
            _presetManager = new PresetManager(ConfigPresetPath.Value, this);

            yield return new WaitUntil(PCSSLight.LoadAssets);
            yield return new WaitUntil(SEGI.LoadAssets);

            _inspector = new Inspector.Inspector(this);
            _isLoaded = true;
        }

        internal SkyboxManager SkyboxManager => _skyboxManager;
        internal PostProcessingManager PostProcessingManager => _postProcessingManager;
        internal LightManager LightManager => _lightManager;
        internal PresetManager PresetManager => _presetManager;
        internal SSSManager SSSManager => _sssManager;

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

            if (Input.GetKeyDown(ConfigShortcut.Value))
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

        internal bool IsStudio()
        {
            return GameMode.Studio == KoikatuAPI.GetCurrentGameMode();
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
#if AI
                        if (KKAPI.KoikatuAPI.GetCurrentGameMode() == KKAPI.GameMode.MainGame)
                        {
                            _previousTimeScale = Time.timeScale;
                            Time.timeScale = 0;
                        }
#endif
                        _previousCursorLockState = Cursor.lockState;
                        _previousCursorVisible = Cursor.visible;
                    }
                    else
                    {
#if AI
                        if (KKAPI.KoikatuAPI.GetCurrentGameMode() == KKAPI.GameMode.MainGame)
                        {
                            Time.timeScale = _previousTimeScale;
                        }
#endif
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