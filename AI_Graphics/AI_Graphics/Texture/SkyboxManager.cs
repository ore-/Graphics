using AIGraphics.Settings;
using MessagePack;
using System;
using System.Collections;
using System.IO;
using System.Linq;
using UnityEngine;
using UnityEngine.Rendering;
using static AIGraphics.Settings.CameraSettings;

namespace AIGraphics.Textures
{
    [MessagePackObject(true)]
    public struct SkyboxParams
    {
        public float exposure;
        public float rotation;
        public Color tint;
        public string selectedCubeMap;

        public SkyboxParams(float exposure, float rotation, Color tint, string selectedCubeMap)
        {
            this.exposure = exposure;
            this.rotation = rotation;
            this.tint = tint;
            this.selectedCubeMap = selectedCubeMap;
        }
    };

    internal class SkyboxManager : TextureManager
    {
        private static readonly int _Exposure = Shader.PropertyToID("_Exposure");
        private static readonly int _Rotation = Shader.PropertyToID("_Rotation");
        private static readonly int _Tint = Shader.PropertyToID("_Tint");

        public SkyboxSettings dynSkyboxSetting;
        public SkyboxParams skyboxParams = new SkyboxParams(1f, 0f, new Color32(128, 128, 128, 128), "");
        public Material Skyboxbg { get; set; }
        public Material Skybox { get; set; }
        public Material MapSkybox { get; set; }

        internal static string noCubemap = "No skybox";

        private string selectedCubeMap = noCubemap;

        private ReflectionProbe _probe;
        private GameObject _probeGameObject;

        internal ReflectionProbe DefaultProbe => _probe ? DefaultReflectionProbe() : _probe;

        internal AIGraphics Parent { get; set; }
        internal Camera Camera => Parent.CameraSettings.MainCamera;

        public bool Update { get; set; }
        public bool PresetUpdate { get; set; }

        internal BepInEx.Logging.ManualLogSource Logger { get; set; }

        public void ApplySkybox()
        {
            Parent.LightingSettings.SkyboxSetting = Skybox;
            Parent.LightingSettings.AmbientModeSetting = LightingSettings.AIAmbientMode.Skybox;
            Parent.LightingSettings.ReflectionMode = DefaultReflectionMode.Skybox;
            Skybox sky = Camera.GetOrAddComponent<Skybox>();
            sky.enabled = true;
            sky.material = Skyboxbg;
            Parent.CameraSettings.ClearFlag = AICameraClearFlags.Skybox;
        }
        public void ApplySkyboxParams()
        {
            if (Skyboxbg != null)
            {
                if (Skyboxbg.HasProperty(_Exposure)) Skyboxbg.SetFloat(_Exposure, skyboxParams.exposure);
                if (Skyboxbg.HasProperty(_Rotation)) Skyboxbg.SetFloat(_Rotation, skyboxParams.rotation);
                if (Skyboxbg.HasProperty(_Tint)) Skyboxbg.SetColor(_Tint, skyboxParams.tint);
            }
            if (Skybox != null)
            {
                if (Skybox.HasProperty(_Exposure)) Skybox.SetFloat(_Exposure, skyboxParams.exposure);
                if (Skybox.HasProperty(_Tint)) Skybox.SetColor(_Tint, skyboxParams.tint);
                if (Skybox.HasProperty(_Rotation)) Skybox.SetFloat(_Rotation, skyboxParams.rotation);
            }
        }
        public void SaveMapSkyBox()
        {
            //Skybox sky = camera.GetComponent<Skybox>();
            //MapSkybox = null == sky ? RenderSettings.skybox : sky.material;
            //MapSkybox = RenderSettings.skybox;
            MapSkybox = Parent.LightingSettings.SkyboxSetting;
        }
        public void LoadSkyboxParams()
        {
            CurrentTexturePath = skyboxParams.selectedCubeMap;
            Exposure = skyboxParams.exposure;
            Tint = skyboxParams.tint;
            Rotation = skyboxParams.rotation;
        }

        public void SaveSkyboxParams()
        {
            skyboxParams.exposure = Exposure;
            skyboxParams.tint = Tint;
            skyboxParams.rotation = Rotation;
            skyboxParams.selectedCubeMap = CurrentTexturePath;
        }
        public void TurnOffCubeMap(Camera camera)
        {
            //RenderSettings.skybox = MapSkybox;
            Parent.LightingSettings.SkyboxSetting = MapSkybox;
            Skybox sky = camera.GetComponent<Skybox>();
            if (null != sky)
            {
                Destroy(sky);
            }

            if (null == MapSkybox)
            {
                //Parent.LightingSettings.AmbientModeSetting = LightingSettings.AIAmbientMode.Trilight;
                Parent.CameraSettings.ClearFlag = AICameraClearFlags.Colour;
            }
            MapSkybox = null;
        }
        public float Exposure
        {
            get => Skybox.GetFloat(_Exposure);
            set
            {
                Skybox?.SetFloat(_Exposure, value);
                Skyboxbg?.SetFloat(_Exposure, value);
                skyboxParams.exposure = value;
            }
        }
        public Color Tint
        {
            get => Skybox.GetColor(_Tint);
            set
            {
                Skybox?.SetColor(_Tint, value);
                Skyboxbg?.SetColor(_Tint, value);
                skyboxParams.tint = value;
            }
        }
        public float Rotation
        {
            get => Skybox.GetFloat(_Rotation);
            set
            {
                Skyboxbg?.SetFloat(_Rotation, value);
                Skybox?.SetFloat(_Rotation, value);
                skyboxParams.rotation = value;
            }
        }

        internal override IEnumerator LoadTexture(string filePath, Action<Texture> _)
        {
            if (filePath == "" || !File.Exists(filePath))
            {
                yield break;
            }
            yield return new WaitUntil(() => HasAssetsLoaded); // Check if preview is loading the bundle.
            AssetBundleCreateRequest assetBundleCreateRequest = AssetBundle.LoadFromFileAsync(filePath);
            yield return assetBundleCreateRequest;
            AssetBundle cubemapbundle = assetBundleCreateRequest.assetBundle;
            AssetBundleRequest bundleRequest = assetBundleCreateRequest.assetBundle.LoadAssetAsync<Material>("skybox");
            yield return bundleRequest;
            Skybox = bundleRequest.asset as Material;
            AssetBundleRequest bundleRequestBG = assetBundleCreateRequest.assetBundle.LoadAssetAsync<Material>("skybox-bg");
            yield return bundleRequestBG;
            Skyboxbg = bundleRequestBG.asset as Material;
            if (Skyboxbg == null)
            {
                Skyboxbg = Skybox;
            }

            cubemapbundle.Unload(false);
            cubemapbundle = null;
            bundleRequestBG = null;
            bundleRequest = null;
            assetBundleCreateRequest = null;

            ApplySkybox();
            ApplySkyboxParams();

            // dynSkyboxSetting is only being used for setting up parameters from preset after assetbundle loading.
            if (dynSkyboxSetting != null) dynSkyboxSetting.Load();
            dynSkyboxSetting = null;

            Update = true;
            Resources.UnloadUnusedAssets();
        }

        //internal string CurrentCubeMap
        internal override string CurrentTexturePath
        {
            get => selectedCubeMap;
            set
            {
                if (null == value)
                {
                    return;
                }

                if (value.All(char.IsWhiteSpace))
                {
                    value = noCubemap;
                }

                //if cubemap is changed
                if (value != selectedCubeMap || PresetUpdate)
                {
                    //switch off cubemap
                    if (noCubemap == value)
                    {
                        TurnOffCubeMap(Camera);
                        if (KKAPI.GameMode.Maker == KKAPI.KoikatuAPI.GetCurrentGameMode())
                        {
                            ToggleCharaMakerBG(true);
                        }

                        Update = true;
                    }
                    else
                    {
                        //if current skybox isn't set to custom cubemap
                        if (noCubemap == selectedCubeMap)
                        {
                            //TODO - need to save cubemap from Map when Map changes too!
                            if (null != Parent.LightingSettings.SkyboxSetting && "skybox" != Parent.LightingSettings.SkyboxSetting.name)//CubeMapNames.IndexOf(RenderSettings.skybox.name))
                            {
                                //save the skybox in scene/map
                                SaveMapSkyBox();
                            }
                        }
                        if (KKAPI.GameMode.Maker == KKAPI.KoikatuAPI.GetCurrentGameMode())
                        {
                            ToggleCharaMakerBG(false);
                        }
                        StartCoroutine(LoadTexture(value, null));
                    }
                    selectedCubeMap = value;
                    skyboxParams.selectedCubeMap = value;
                    PresetUpdate = false;
                }
            }
        }

        internal override string SearchPattern { get => "*.cube"; set => throw new System.NotImplementedException(); }
        internal override Texture CurrentTexture { get => throw new System.NotImplementedException(); set => throw new System.NotImplementedException(); }

        internal override IEnumerator LoadPreview(string filePath)
        {
            AssetBundleCreateRequest assetBundleCreateRequest = AssetBundle.LoadFromFileAsync(filePath);
            yield return assetBundleCreateRequest;
            AssetBundle cubemapbundle = assetBundleCreateRequest?.assetBundle;
            AssetBundleRequest bundleRequest = assetBundleCreateRequest?.assetBundle?.LoadAssetAsync<Cubemap>("skybox");
            yield return bundleRequest;
            if (null == bundleRequest || null == bundleRequest.asset)
            {
                _assetsToLoad--;
                yield break;
            }
            Cubemap cubemap = bundleRequest.asset as Cubemap;
            Texture2D texture = new Texture2D(cubemap.width, cubemap.height);
            Color[] CubeMapColors = cubemap.GetPixels(CubemapFace.PositiveX);
            texture.SetPixels(CubeMapColors);
            Util.ResizeTexture(texture, 128, 128);
            Previews.Add(texture);
            TexturePaths.Add(filePath);
            cubemapbundle.Unload(false);
            cubemapbundle = null;
            bundleRequest = null;
            assetBundleCreateRequest = null;
            CubeMapColors = null;
            texture = null;
            _assetsToLoad--;
        }

        internal ReflectionProbe DefaultReflectionProbe()
        {
            if (null == _probeGameObject || null == _probe)
            {
                _probeGameObject = new GameObject("Default Reflection Probe");
                _probe = _probeGameObject.GetOrAddComponent<ReflectionProbe>();                
                _probe.mode = ReflectionProbeMode.Realtime;
                _probe.boxProjection = false;
                _probe.intensity = 1f;
                _probe.importance = 100;
                _probe.resolution = 512;
                _probe.backgroundColor = Color.white;
                _probe.hdr = true;
                _probe.clearFlags = ReflectionProbeClearFlags.Skybox;
                _probe.cullingMask = Camera.cullingMask;
                _probe.size = new Vector3(100, 100, 100);
                _probe.nearClipPlane = 0.01f;
                _probe.transform.position = new Vector3(0, 0, 0);
                _probe.refreshMode = ReflectionProbeRefreshMode.EveryFrame;
                _probe.timeSlicingMode = ReflectionProbeTimeSlicingMode.AllFacesAtOnce;
                DontDestroyOnLoad(_probeGameObject);
                DontDestroyOnLoad(_probe);
            }
            return _probe;
        }

        internal void SetupDefaultReflectionProbe()
        {
            ReflectionProbe[] rps = GetReflectinProbes();
            //disable default realtime reflection probe if scene has realtime reflection probes.
            DefaultProbe.intensity = (rps.Select(probe => probe.mode == ReflectionProbeMode.Realtime).ToArray().Length > 1) ? 0 : 1;
        }

        internal ReflectionProbe[] GetReflectinProbes()
        {
            return GameObject.FindObjectsOfType<ReflectionProbe>();
        }

        public IEnumerator UpdateEnvironment()//BepInEx.Logging.ManualLogSource logger)
        {
            while (true)
            {
                yield return null;
                if (Update)
                {
                    DynamicGI.UpdateEnvironment();
                    ReflectionProbe[] rps = GetReflectinProbes();
                    for (int i = 0; i < rps.Length; i++)
                    {
                        rps[i].RenderProbe();
                    }
                    Update = false;
                }
            }
        }

        internal static void ToggleCharaMakerBG(bool active)
        {
            CharaCustom.CharaCustom characustom = GameObject.FindObjectOfType<CharaCustom.CharaCustom>();
            if (null == characustom)
            {
                return;
            }

            Transform bgt = characustom.GetComponentsInChildren<Transform>(true).FirstOrDefault(t => t.name == "p_ai_mi_createBG00_00");
            if (null != bgt)
            {
                bgt.gameObject.SetActive(active);
            }
        }

        internal void Start()
        {
            DefaultReflectionProbe();
            StartCoroutine(UpdateEnvironment());
        }
    }
}