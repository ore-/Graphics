using MessagePack;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering.PostProcessing;

// TODO: Turn on Post Processing in main menu.
// TODO: Messagepack clears out layer lists for a frame. Need to figure out to remove temporary solutions
namespace AIGraphics.Settings
{
    [MessagePackObject(keyAsPropertyName: true)]
    public class PostProcessingSettings
    {
        // Parameters for Messagepack Save
        internal AmbientOcclusionParams paramAmbientOcclusion = new AmbientOcclusionParams();
        internal AutoExposureParams paramAutoExposure = new AutoExposureParams();
        internal BloomParams paramBloom = new BloomParams();
        internal ChromaticAberrationParams paramChromaticAberration = new ChromaticAberrationParams();
        internal ColorGradingParams paramColorGrading = new ColorGradingParams();
        internal DepthOfFieldParams paramDepthOfField = new DepthOfFieldParams();
        internal GrainLayerParams paramGrainLayer = new GrainLayerParams();
        internal ScreenSpaceReflectionParams paramScreenSpaceReflection = new ScreenSpaceReflectionParams();
        internal VignetteParams paramVignette = new VignetteParams();
        internal AmplifyOcclusionParams paramAmplifyOcclusion = new AmplifyOcclusionParams();
        //internal AmplifyOcclusion paramAmplifyOcclusion; // not implemented yet

        public enum Antialiasing
        {
            None = PostProcessLayer.Antialiasing.None,
            FXAA = PostProcessLayer.Antialiasing.FastApproximateAntialiasing,
            SMAA = PostProcessLayer.Antialiasing.SubpixelMorphologicalAntialiasing,
            TAA = PostProcessLayer.Antialiasing.TemporalAntialiasing
        };

        private readonly PostProcessLayer _postProcessLayer;
        internal AmbientOcclusion ambientOcclusionLayer;
        internal AutoExposure autoExposureLayer;
        internal Bloom bloomLayer;
        internal ChromaticAberration chromaticAberrationLayer;
        internal ColorGrading colorGradingLayer;
        internal DepthOfField depthOfFieldLayer;
        internal Grain grainLayer;
        internal ScreenSpaceReflections screenSpaceReflectionsLayer;
        internal Vignette vignetteLayer;
        internal Camera initialCamera;
        internal AmplifyOcclusionEffect amplifyOcclusionComponent;
        [IgnoreMember]
        public AmplifyOcclusionEffect AmplifyOcclusionComponent
        {
            get
            {
                if (initialCamera == null && Camera.main != null)
                    amplifyOcclusionComponent = Camera.main.GetComponent<AmplifyOcclusionEffect>();
                return amplifyOcclusionComponent;
            }
        }

        public PostProcessingSettings()
        {
            SetupVolume();
        }

        public PostProcessingSettings(Camera camera)
        {
            initialCamera = camera;
            _postProcessLayer = camera.GetComponent<PostProcessLayer>();
            SetupVolume();
        }

        internal PostProcessLayer PostProcessLayer => (_postProcessLayer == null) ? (initialCamera == null ? Camera.main : initialCamera).GetComponent<PostProcessLayer>() : _postProcessLayer;

        internal PostProcessVolume _volume;
        internal PostProcessVolume Volume => AIGraphics.Instance.GetOrAddComponent<PostProcessVolume>();

        internal PostProcessVolume SetupVolume()
        {
            // Turn off everything, We're not going to use 
            foreach (PostProcessVolume postProcessVolume in GameObject.FindObjectsOfType<PostProcessVolume>())
            {
                if (SettingValues.profile == null && (postProcessVolume.name == "PostProcessVolume3D" || postProcessVolume.name == "PostProcessVolume"))
                {
                    SettingValues.profile = GameObject.Instantiate(postProcessVolume.profile);
                    SettingValues.profile.name = "AIGraphics Post Processing Profile";
                    InitializeProfiles();
                }

                postProcessVolume.weight = 0;
                postProcessVolume.enabled = false;
            }
            if (SettingValues.profile == null)
            {
                // Just in case
                SettingValues.profile = (PostProcessProfile)ScriptableObject.CreateInstance("PostProcessProfile");
                InitializeProfiles();
            }

            _volume = AIGraphics.Instance.GetOrAddComponent<PostProcessVolume>();
            _volume.enabled = true;
            _volume.isGlobal = true;
            _volume.blendDistance = 0;
            _volume.weight = 1;
            _volume.priority = 1;
            _volume.useGUILayout = true;
            _volume.sharedProfile = SettingValues.profile;
            _volume.profile = SettingValues.profile;
            _volume.gameObject.layer = LayerMask.NameToLayer("PostProcessing");

            if (initialCamera != null)
                amplifyOcclusionComponent = initialCamera.GetComponent<AmplifyOcclusionEffect>();

            return _volume;
        }
        internal void InitializeProfiles()
        {
            if (!SettingValues.profile.TryGetSettings(out chromaticAberrationLayer))
            {
                chromaticAberrationLayer = SettingValues.profile.AddSettings<ChromaticAberration>();
            }

            if (!SettingValues.profile.TryGetSettings(out grainLayer))
            {
                grainLayer = SettingValues.profile.AddSettings<Grain>();
            }

            if (!SettingValues.profile.TryGetSettings(out ambientOcclusionLayer))
            {
                ambientOcclusionLayer = SettingValues.profile.AddSettings<AmbientOcclusion>();
            }

            if (!SettingValues.profile.TryGetSettings(out autoExposureLayer))
            {
                autoExposureLayer = SettingValues.profile.AddSettings<AutoExposure>();
            }

            if (!SettingValues.profile.TryGetSettings(out bloomLayer))
            {
                bloomLayer = SettingValues.profile.AddSettings<Bloom>();
            }

            if (!SettingValues.profile.TryGetSettings(out colorGradingLayer))
            {
                colorGradingLayer = SettingValues.profile.AddSettings<ColorGrading>();
            }

            if (!SettingValues.profile.TryGetSettings(out depthOfFieldLayer))
            {
                depthOfFieldLayer = SettingValues.profile.AddSettings<DepthOfField>();
            }

            if (!SettingValues.profile.TryGetSettings(out screenSpaceReflectionsLayer))
            {
                screenSpaceReflectionsLayer = SettingValues.profile.AddSettings<ScreenSpaceReflections>();
            }

            if (!SettingValues.profile.TryGetSettings(out vignetteLayer))
            {
                vignetteLayer = SettingValues.profile.AddSettings<Vignette>();
            }

            depthOfFieldLayer.enabled.value = false; // Make people use Depth of Field Manually
        }

        internal void ResetVolume()
        {
            if (SettingValues.defaultProfile != null)
            {
                Volume.sharedProfile = SettingValues.defaultProfile;
                Volume.profile = SettingValues.defaultProfile;
            }
        }

        public void SaveParameters()
        {
            if (Volume.profile.TryGetSettings(out AutoExposure autoExposureLayer))
            {
                paramAutoExposure.Save(autoExposureLayer);
            }

            if (Volume.profile.TryGetSettings(out AmbientOcclusion ambientOcclusionLayer))
            {
                paramAmbientOcclusion.Save(ambientOcclusionLayer);
            }

            if (Volume.profile.TryGetSettings(out Bloom bloomLayer))
            {
                paramBloom.Save(bloomLayer);
            }

            if (Volume.profile.TryGetSettings(out DepthOfField depthOfFieldLayer))
            {
                paramDepthOfField.Save(depthOfFieldLayer);
            }

            if (Volume.profile.TryGetSettings(out ChromaticAberration chromaticAberrationLayer))
            {
                paramChromaticAberration.Save(chromaticAberrationLayer);
            }

            if (Volume.profile.TryGetSettings(out ColorGrading colorGradingLayer))
            {
                paramColorGrading.Save(colorGradingLayer);
            }

            if (Volume.profile.TryGetSettings(out Grain grainLayer))
            {
                paramGrainLayer.Save(grainLayer);
            }

            if (Volume.profile.TryGetSettings(out ScreenSpaceReflections screenSpaceReflectionsLayer))
            {
                paramScreenSpaceReflection.Save(screenSpaceReflectionsLayer);
            }

            if (Volume.profile.TryGetSettings(out Vignette vignetteLayer))
            {
                paramVignette.Save(vignetteLayer);
            }

            if (AmplifyOcclusionComponent != null)
            {
                paramAmplifyOcclusion.Save(AmplifyOcclusionComponent);
            }
        }

        public void LoadParameters()
        {
            if (Volume.profile.TryGetSettings(out AutoExposure autoExposureLayer))
            {
                paramAutoExposure.Load(autoExposureLayer);
            }

            if (Volume.profile.TryGetSettings(out AmbientOcclusion ambientOcclusionLayer))
            {
                paramAmbientOcclusion.Load(ambientOcclusionLayer);
            }

            if (Volume.profile.TryGetSettings(out Bloom bloomLayer))
            {
                paramBloom.Load(bloomLayer);
            }

            if (Volume.profile.TryGetSettings(out DepthOfField depthOfFieldLayer))
            {
                paramDepthOfField.Load(depthOfFieldLayer);
            }

            if (Volume.profile.TryGetSettings(out ChromaticAberration chromaticAberrationLayer))
            {
                paramChromaticAberration.Load(chromaticAberrationLayer);
            }

            if (Volume.profile.TryGetSettings(out ColorGrading colorGradingLayer))
            {
                paramColorGrading.Load(colorGradingLayer);
            }

            if (Volume.profile.TryGetSettings(out Grain grainLayer))
            {
                paramGrainLayer.Load(grainLayer);
            }

            if (Volume.profile.TryGetSettings(out ScreenSpaceReflections screenSpaceReflectionsLayer))
            {
                paramScreenSpaceReflection.Load(screenSpaceReflectionsLayer);
            }

            if (Volume.profile.TryGetSettings(out Vignette vignetteLayer))
            {
                paramVignette.Load(vignetteLayer);
            }

            if (AmplifyOcclusionComponent != null)
            {
                paramAmplifyOcclusion.Load(AmplifyOcclusionComponent);
            }
        }

        internal Transform VolumeTriggerSetting => _postProcessLayer.volumeTrigger;

        public LayerMask VolumeLayerSetting => PostProcessLayer.volumeLayer;

        public float JitterSpread
        {
            get => PostProcessLayer.temporalAntialiasing.jitterSpread;
            set => PostProcessLayer.temporalAntialiasing.jitterSpread = value;
        }

        public float StationaryBlending
        {
            get => PostProcessLayer.temporalAntialiasing.stationaryBlending;
            set => PostProcessLayer.temporalAntialiasing.stationaryBlending = value;
        }

        public float MotionBlending
        {
            get => PostProcessLayer.temporalAntialiasing.motionBlending;
            set => PostProcessLayer.temporalAntialiasing.motionBlending = value;
        }

        public float Sharpness
        {
            get => PostProcessLayer.temporalAntialiasing.sharpness;
            set => PostProcessLayer.temporalAntialiasing.sharpness = value;
        }

        public bool FXAAMode
        {
            get => PostProcessLayer.fastApproximateAntialiasing.fastMode;
            set => PostProcessLayer.fastApproximateAntialiasing.fastMode = value;
        }

        public bool FXAAAlpha
        {
            get => PostProcessLayer.fastApproximateAntialiasing.keepAlpha;
            set => PostProcessLayer.fastApproximateAntialiasing.keepAlpha = value;
        }

        public SubpixelMorphologicalAntialiasing.Quality SMAAQuality
        {
            get => PostProcessLayer.subpixelMorphologicalAntialiasing.quality;
            set => PostProcessLayer.subpixelMorphologicalAntialiasing.quality = value;
        }

        public Antialiasing AntialiasingMode
        {
            get => (Antialiasing)PostProcessLayer.antialiasingMode;
            set => PostProcessLayer.antialiasingMode = (PostProcessLayer.Antialiasing)value;
        }

        public float FocalDistance
        {
            get => depthOfFieldLayer != null ? depthOfFieldLayer.focusDistance.value : 0f;
            set
            {
                if (depthOfFieldLayer != null)
                {
                    depthOfFieldLayer.focusDistance.value = value;
                }
            }
        }

        public AmbientOcclusionParams AmbientOcclusion {
            get => paramAmbientOcclusion;
            set => paramAmbientOcclusion = value;
        }
        public AutoExposureParams AutoExposure {
            get => paramAutoExposure;
            set => paramAutoExposure = value;
        }
        public BloomParams Bloom
        {
            get => paramBloom;
            set => paramBloom = value;
        }
        public ChromaticAberrationParams ChromaticAberration {
            get => paramChromaticAberration;
            set => paramChromaticAberration = value;
        }
        public ColorGradingParams ColorGrading {
            get => paramColorGrading;
            set => paramColorGrading = value;
        }
        public DepthOfFieldParams DepthOfField {
            get => paramDepthOfField;
            set => paramDepthOfField = value;
        }
        public GrainLayerParams GrainLayer {
            get => paramGrainLayer;
            set => paramGrainLayer = value;
        }
        public ScreenSpaceReflectionParams ScreenSpaceReflection {
            get => paramScreenSpaceReflection;
            set => paramScreenSpaceReflection = value;
        }
        public VignetteParams Vignette {
            get => paramVignette;
            set => paramVignette = value;
        }
        public AmplifyOcclusionParams AmplifyOcclusion
        {
            get => paramAmplifyOcclusion;
            set => paramAmplifyOcclusion = value;
        }
    }
}
