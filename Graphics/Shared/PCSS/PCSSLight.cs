using KKAPI.Utilities;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;

//https://github.com/TheMasonX/UnityPCSS

namespace Graphics
{
    internal class PCSSLight : MonoBehaviour
    {
        internal static int Blocker_SampleCount
        {
            get => Shader.GetGlobalInt("Blocker_Samples");
            set
            {
                Shader.SetGlobalInt("Blocker_Samples", value);
                UpdatePoisson();
            }
        }
        internal static int PCF_SampleCount
        {
            get => Shader.GetGlobalInt("PCF_Samples");
            set
            {
                Shader.SetGlobalInt("PCF_Samples", value);
                UpdatePoisson();
            }
        }
        internal static float Softness
        {
            get => Shader.GetGlobalFloat("Softness") * 64f * Mathf.Sqrt(QualitySettings.shadowDistance);
            set
            {
                Shader.SetGlobalFloat("Softness", value / 64f / Mathf.Sqrt(QualitySettings.shadowDistance));
            }
        }
        internal static float SoftnessFalloff
        {
            get => Mathf.Log(Shader.GetGlobalFloat("SoftnessFalloff"));
            set
            {
                Shader.SetGlobalFloat("SoftnessFalloff", Mathf.Exp(value));
                SetFlag("USE_FALLOFF", value > Mathf.Epsilon);
            }
        }
        internal static float MaxStaticGradientBias
        {
            get => Shader.GetGlobalFloat("RECEIVER_PLANE_MIN_FRACTIONAL_ERROR");
            set
            {
                Shader.SetGlobalFloat("RECEIVER_PLANE_MIN_FRACTIONAL_ERROR", value);
                SetFlag("USE_STATIC_BIAS", value > 0);
            }
        }
        internal static float Blocker_GradientBias
        {
            get => Shader.GetGlobalFloat("Blocker_GradientBias");
            set
            {
                Shader.SetGlobalFloat("Blocker_GradientBias", value);
                SetFlag("USE_BLOCKER_BIAS", value > 0);
            }
        }
        internal static float PCF_GradientBias
        {
            get => Shader.GetGlobalFloat("PCF_GradientBias");
            set
            {
                Shader.SetGlobalFloat("PCF_GradientBias", value);
                SetFlag("USE_PCF_BIAS", value > 0);
            }
        }
        /* Unity's Cascade Blending doesn't work quite right - Might be able to do something about it when I have time to research it more */
        internal static float CascadeBlendDistance
        {
            get => Shader.GetGlobalFloat("CascadeBlendDistance");
            set
            {
                Shader.SetGlobalFloat("CascadeBlendDistance", value);
                SetFlag("USE_CASCADE_BLENDING", value > 0);
            }
        }

        internal FilterMode FilterMode { get; set; }
        internal int MSAA { get; set; }

        //[Range(0f, 1f)]
        //public float NearPlane = .1f;

        //    public bool RotateSamples = true;
        //    public bool UseNoiseTexture = true;
        private static Texture2D _noiseTexture;
        //private bool _supportOrthographicProjection = false;

        private RenderTexture _shadowRenderTexture;
        private RenderTextureFormat _format = RenderTextureFormat.RFloat;
        private LightEvent lightEvent = LightEvent.AfterShadowMap;

        private static string pcssName = "Hidden/PCSS";
        private static string builtinShaderName = "Hidden/Built-In-ScreenSpaceShadows";
        private static Shader _pcss;
        private static Shader _sss;
        private static ShaderVariantCollection _shaderVariants;
        private static int _shadowmapPropID;
        private static List<string> keywords = new List<string>();

        private CommandBuffer copyShadowBuffer;
        private Light _light;
        private int _shadowCustomResolution = 4096;

        #region Initialization

        internal void OnEnable()
        {
            Setup();
        }

        internal void OnDisable()
        {
            ResetShadowMode();
        }

        internal void Setup()
        {
            _light = GetComponent<Light>();

            if (!_light || !_pcss)
                return;

            if (_light.shadowCustomResolution > 0)
                _shadowCustomResolution = _light.shadowCustomResolution;
            else
                _light.shadowCustomResolution = _shadowCustomResolution;

            copyShadowBuffer = new CommandBuffer();
            copyShadowBuffer.name = "PCSS Shadows";

            var buffers = _light.GetCommandBuffers(lightEvent);
            for (int i = 0; i < buffers.Length; i++)
            {
                if (buffers[i].name == "PCSS Shadows")
                {
                    _light.RemoveCommandBuffer(lightEvent, buffers[i]);
                }
            }

            _light.AddCommandBuffer(lightEvent, copyShadowBuffer);

            MSAA = QualitySettings.antiAliasing;
            FilterMode = FilterMode.Trilinear;

            CreateShadowRenderTexture();
            UpdateShaderValues();
            UpdateCommandBuffer();
        }

        internal void CreateShadowRenderTexture()
        {
            _shadowRenderTexture = new RenderTexture(_light.shadowCustomResolution, _light.shadowCustomResolution, 0, _format);
            _shadowRenderTexture.filterMode = FilterMode;
            _shadowRenderTexture.useMipMap = false;
            _shadowRenderTexture.antiAliasing = MSAA;
        }

        internal void ResetShadowMode()
        {
            DisablePCSS();
            DestroyImmediate(_shadowRenderTexture);
            if (!_light) return;
            _light.shadowCustomResolution = 0;
            _light.RemoveCommandBuffer(LightEvent.AfterShadowMap, copyShadowBuffer);
        }
        #endregion

        #region UpdateSettings
        internal static void UpdatePoisson()
        {
            int maxSamples = Mathf.Max(Blocker_SampleCount, PCF_SampleCount);
            SetFlag("POISSON_32", maxSamples < 33);
            SetFlag("POISSON_64", maxSamples > 33);
        }
        internal void UpdateShaderValues()
        {
            keywords.Clear();
            if (_shadowRenderTexture)
            {
                if (_shadowRenderTexture.format != _format || _shadowCustomResolution != _light.shadowCustomResolution)
                    CreateShadowRenderTexture();

                _shadowRenderTexture.antiAliasing = MSAA;
                _shadowRenderTexture.filterMode = FilterMode;
            }
            //Shader.SetGlobalFloat("NearPlane", NearPlane);
            //SetFlag("ROTATE_SAMPLES", RotateSamples);
            //SetFlag("USE_NOISE_TEX", UseNoiseTexture);
            //SetFlag("ORTHOGRAPHIC_SUPPORTED", _supportOrthographicProjection);
            UpdatePoisson();
            if (_shaderVariants)
                _shaderVariants.Add(new ShaderVariantCollection.ShaderVariant(_pcss, PassType.Normal, keywords.ToArray()));
        }

        internal void UpdateCommandBuffer()
        {
            if (!_light)
                return;

            copyShadowBuffer.Clear();
            copyShadowBuffer.SetShadowSamplingMode(BuiltinRenderTextureType.CurrentActive, ShadowSamplingMode.RawDepth);
            copyShadowBuffer.Blit(BuiltinRenderTextureType.CurrentActive, _shadowRenderTexture);
            copyShadowBuffer.SetGlobalTexture(_shadowmapPropID, _shadowRenderTexture);
        }
        #endregion

        internal static void SetFlag(string shaderKeyword, bool value)
        {
            if (value)
            {
                Shader.EnableKeyword(shaderKeyword);
                keywords.Add(shaderKeyword);
            }
            else
                Shader.DisableKeyword(shaderKeyword);
        }

        internal static void LoadAssets()
        {
            AssetBundle assetBundle = AssetBundle.LoadFromMemory(ResourceUtils.GetEmbeddedResource("pcss"));
            _pcss = assetBundle.LoadAsset<Shader>("Assets/PCSS/Shaders/PCSS.shader");
            _sss = assetBundle.LoadAsset<Shader>("Assets/PCSS/Shaders/Built-In-ScreenSpaceShadows.shader");
            _shaderVariants = assetBundle.LoadAsset<ShaderVariantCollection>("Assets/PCSS/Shaders/PCSS-ShaderVariants.shadervariants");
            assetBundle.Unload(false);
            _noiseTexture = KKAPI.Utilities.TextureUtils.LoadTexture(ResourceUtils.GetEmbeddedResource("blue noise.png"));
            _shadowmapPropID = Shader.PropertyToID("_ShadowMap");
            //sanity check
            Shader pcss = Shader.Find(pcssName);
            Shader sss = Shader.Find(builtinShaderName);
            if (!pcss || !sss)
            {
                Debug.Log("failed to load " + pcssName + " " + builtinShaderName);
            }
        }

        internal static void EnablePCSS()
        {
            if (null != _pcss)
            {
                GraphicsSettings.SetCustomShader(BuiltinShaderType.ScreenSpaceShadows, _pcss);
                GraphicsSettings.SetShaderMode(BuiltinShaderType.ScreenSpaceShadows, BuiltinShaderMode.UseCustom);
                Blocker_SampleCount = 64;
                PCF_SampleCount = 64;
                Softness = 1.25f;
                SoftnessFalloff = 1f;
                MaxStaticGradientBias = 0f;
                Blocker_GradientBias = 0.01f;
                PCF_GradientBias = 1f;
                CascadeBlendDistance = 0f;
                if (_noiseTexture)
                {
                    Shader.SetGlobalVector("NoiseCoords", new Vector4(1f / _noiseTexture.width, 1f / _noiseTexture.height, 0f, 0f));
                    Shader.SetGlobalTexture("_NoiseTexture", _noiseTexture);
                }
            }
        }

        internal static void DisablePCSS()
        {
            _sss = Shader.Find(builtinShaderName);
            //		if (!Application.isEditor)
            //		{
            //			if (builtinShader)
            //				Debug.LogErrorFormat("Built-In Shadow Shader Found: {0} | Supported {1}", builtinShader.name, builtinShader.isSupported);
            //			else
            //				Debug.LogError("Shadow Shader Not Found!!!");
            //		}

            GraphicsSettings.SetCustomShader(BuiltinShaderType.ScreenSpaceShadows, _sss);
            GraphicsSettings.SetShaderMode(BuiltinShaderType.ScreenSpaceShadows, BuiltinShaderMode.Disabled);
            GraphicsSettings.SetShaderMode(BuiltinShaderType.ScreenSpaceShadows, BuiltinShaderMode.UseBuiltin);
        }
    }
}