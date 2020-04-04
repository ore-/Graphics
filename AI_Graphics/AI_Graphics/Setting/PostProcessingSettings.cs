using MessagePack;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Rendering.PostProcessing;
using static KKAPI.Studio.StudioAPI;

// TODO: Messagepack clears out layer lists for a frame. Need to figure out to remove temporary solutions
namespace AIGraphics.Settings {
    [MessagePackObject(keyAsPropertyName: true)]
    public class PostProcessingSettings {
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
        //internal AmplifyOcclusion paramAmplifyOcclusion; // not implemented yet

        public enum Antialiasing {
            None = PostProcessLayer.Antialiasing.None,
            FXAA = PostProcessLayer.Antialiasing.FastApproximateAntialiasing,
            SMAA = PostProcessLayer.Antialiasing.SubpixelMorphologicalAntialiasing,
            TAA = PostProcessLayer.Antialiasing.TemporalAntialiasing
        };

        private string selectedLUT;
        internal List<string> LUTNames {
            get; set;
        }
        internal string[] volumeNames;
        private PostProcessLayer _postProcessLayer;
        internal PostProcessVolume[] postProcessVolumes;
        internal List<AmbientOcclusion> ambientOcclusionLayers = new List<AmbientOcclusion> { };
        internal List<AutoExposure> autoExposureLayers = new List<AutoExposure> { };
        internal List<Bloom> bloomLayers = new List<Bloom> { };
        internal List<ChromaticAberration> chromaticAberrationLayers = new List<ChromaticAberration> { };
        internal List<ColorGrading> colorGradingLayers = new List<ColorGrading> { };
        internal List<DepthOfField> depthOfFieldLayers = new List<DepthOfField> { };
        internal List<Grain> grainLayers = new List<Grain> { };
        internal List<ScreenSpaceReflections> screenSpaceReflectionsLayers = new List<ScreenSpaceReflections> { };
        internal List<Vignette> vignetteLayers = new List<Vignette> { };
        internal Camera initialCamera;

        public PostProcessingSettings() {
            postProcessVolumes = GameObject.FindObjectsOfType<PostProcessVolume>();
            volumeNames = postProcessVolumes.Select(volume => volume.name).ToArray();
        }

        public PostProcessingSettings(Camera camera) {
            initialCamera = camera;
            _postProcessLayer = camera.GetComponent<PostProcessLayer>();
            postProcessVolumes = GameObject.FindObjectsOfType<PostProcessVolume>();
            volumeNames = postProcessVolumes.Select(volume => volume.name).ToArray();
            // Change it later
            for (int i = 0; i < postProcessVolumes.Length; i++) {
                Bloom bloomLayer = null;
                AmbientOcclusion ambientOcclusionLayer = null;
                AutoExposure autoExposureLayer = null;
                ChromaticAberration chromaticAberrationLayer = null;
                ColorGrading colorGradingLayer = null;
                DepthOfField depthOfFieldLayer = null;
                Grain grainLayer = null;
                ScreenSpaceReflections screenSpaceReflectionsLayer = null;
                Vignette vignetteLayer = null;

                postProcessVolumes[i].profile.TryGetSettings(out autoExposureLayer);
                postProcessVolumes[i].profile.TryGetSettings(out ambientOcclusionLayer);
                postProcessVolumes[i].profile.TryGetSettings(out bloomLayer);
                postProcessVolumes[i].profile.TryGetSettings(out chromaticAberrationLayer);
                postProcessVolumes[i].profile.TryGetSettings(out colorGradingLayer);
                postProcessVolumes[i].profile.TryGetSettings(out depthOfFieldLayer);
                postProcessVolumes[i].profile.TryGetSettings(out grainLayer);
                postProcessVolumes[i].profile.TryGetSettings(out screenSpaceReflectionsLayer);
                postProcessVolumes[i].profile.TryGetSettings(out vignetteLayer);

                bloomLayers.Add(bloomLayer);
                ambientOcclusionLayers.Add(ambientOcclusionLayer);
                depthOfFieldLayers.Add(depthOfFieldLayer);
                chromaticAberrationLayers.Add(chromaticAberrationLayer);
                colorGradingLayers.Add(colorGradingLayer);
                vignetteLayers.Add(vignetteLayer);
                screenSpaceReflectionsLayers.Add(screenSpaceReflectionsLayer);
                autoExposureLayers.Add(autoExposureLayer);
                grainLayers.Add(grainLayer);

                LUTNames = GetLUTNames();
            }
        }

        internal PostProcessLayer PostProcessLayer {
            get {
                if (_postProcessLayer == null)
                    return (initialCamera == null ? Camera.main : initialCamera).GetComponent<PostProcessLayer>();

                return _postProcessLayer;
            }
        }

        internal bool FindTargetVolume(out PostProcessVolume targetVolume) {
            // Find active volume to save.
            targetVolume = null;

            foreach (var volume in postProcessVolumes) 
                if (volume.enabled) {
                    targetVolume = volume;
                    return true;
                }

            return false;
        }

        public void SaveParameters() {
            if (FindTargetVolume(out PostProcessVolume targetVolume)) {
                if (targetVolume.profile.TryGetSettings(out AutoExposure autoExposureLayer))
                    paramAutoExposure.Save(autoExposureLayer);
                if (targetVolume.profile.TryGetSettings(out AmbientOcclusion ambientOcclusionLayer))
                    paramAmbientOcclusion.Save(ambientOcclusionLayer);
                if (targetVolume.profile.TryGetSettings(out Bloom bloomLayer))
                    paramBloom.Save(bloomLayer);
                if (targetVolume.profile.TryGetSettings(out DepthOfField depthOfFieldLayer))
                    paramDepthOfField.Save(depthOfFieldLayer);
                if (targetVolume.profile.TryGetSettings(out ChromaticAberration chromaticAberrationLayer))
                    paramChromaticAberration.Save(chromaticAberrationLayer);
                if (targetVolume.profile.TryGetSettings(out ColorGrading colorGradingLayer))
                    paramColorGrading.Save(colorGradingLayer);
                if (targetVolume.profile.TryGetSettings(out Grain grainLayer))
                    paramGrainLayer.Save(grainLayer);
                if (targetVolume.profile.TryGetSettings(out ScreenSpaceReflections screenSpaceReflectionsLayer))
                    paramScreenSpaceReflection.Save(screenSpaceReflectionsLayer);
                if (targetVolume.profile.TryGetSettings(out Vignette vignetteLayer))
                    paramVignette.Save(vignetteLayer);
            }
        }

        public void LoadParameters() {
            if (FindTargetVolume(out PostProcessVolume targetVolume)) {
                if (targetVolume.profile.TryGetSettings(out AutoExposure autoExposureLayer))
                    paramAutoExposure.Load(autoExposureLayer);
                if (targetVolume.profile.TryGetSettings(out AmbientOcclusion ambientOcclusionLayer))
                    paramAmbientOcclusion.Load(ambientOcclusionLayer);
                if (targetVolume.profile.TryGetSettings(out Bloom bloomLayer))
                    paramBloom.Load(bloomLayer);
                if (targetVolume.profile.TryGetSettings(out DepthOfField depthOfFieldLayer))
                    paramDepthOfField.Load(depthOfFieldLayer);
                if (targetVolume.profile.TryGetSettings(out ChromaticAberration chromaticAberrationLayer))
                    paramChromaticAberration.Load(chromaticAberrationLayer);
                if (targetVolume.profile.TryGetSettings(out ColorGrading colorGradingLayer))
                    paramColorGrading.Load(colorGradingLayer);
                if (targetVolume.profile.TryGetSettings(out Grain grainLayer))
                    paramGrainLayer.Load(grainLayer);
                if (targetVolume.profile.TryGetSettings(out ScreenSpaceReflections screenSpaceReflectionsLayer))
                    paramScreenSpaceReflection.Load(screenSpaceReflectionsLayer);
                if (targetVolume.profile.TryGetSettings(out Vignette vignetteLayer))
                    paramVignette.Load(vignetteLayer);
            }
        }

        internal Transform VolumeTriggerSetting {
            get => _postProcessLayer.volumeTrigger;
        }
        
        internal string BloomTexturePath {
            set => this.paramBloom.dirtTexture = value;
        }

        public LayerMask VolumeLayerSetting {
            //get => LayerMask.LayerToName(_postProcessLayer.volumeLayer.value);
            get => PostProcessLayer.volumeLayer;
        }

        public float JitterSpread {
            get => PostProcessLayer.temporalAntialiasing.jitterSpread;
            set => PostProcessLayer.temporalAntialiasing.jitterSpread = value;
        }

        public float StationaryBlending {
            get => PostProcessLayer.temporalAntialiasing.stationaryBlending;
            set => PostProcessLayer.temporalAntialiasing.stationaryBlending = value;
        }

        public float MotionBlending {
            get => PostProcessLayer.temporalAntialiasing.motionBlending;
            set => PostProcessLayer.temporalAntialiasing.motionBlending = value;
        }

        public float Sharpness {
            get => PostProcessLayer.temporalAntialiasing.sharpness;
            set => PostProcessLayer.temporalAntialiasing.sharpness = value;
        }

        public bool FXAAMode {
            get => PostProcessLayer.fastApproximateAntialiasing.fastMode;
            set => PostProcessLayer.fastApproximateAntialiasing.fastMode = value;
        }

        public bool FXAAAlpha {
            get => PostProcessLayer.fastApproximateAntialiasing.keepAlpha;
            set => PostProcessLayer.fastApproximateAntialiasing.keepAlpha = value;
        }

        public SubpixelMorphologicalAntialiasing.Quality SMAAQuality {
            get => PostProcessLayer.subpixelMorphologicalAntialiasing.quality;
            set => PostProcessLayer.subpixelMorphologicalAntialiasing.quality = value;
        }

        public Antialiasing AntialiasingMode {
            get => (Antialiasing)PostProcessLayer.antialiasingMode;
            set => PostProcessLayer.antialiasingMode = (PostProcessLayer.Antialiasing)value;
        }

        public float FocalDistance {
            get => (depthOfFieldLayers.Count > 0) ? depthOfFieldLayers[0].focusDistance.value : 0f;
            set {
                if (depthOfFieldLayers.Count > 0) 
                    depthOfFieldLayers[0].focusDistance.value = value;
            }
        }

        public string CurrentLUT {
            get => selectedLUT;
            set {
                if (null != value && value != selectedLUT) {
                    selectedLUT = value;
                }
            }
        }
        public AmbientOcclusionParams AmbientOcclusion {
            get => paramAmbientOcclusion;
            set {
                paramAmbientOcclusion = value;
            }
        }
        public AutoExposureParams AutoExposure {
            get => paramAutoExposure;
            set {
                paramAutoExposure = value;
            }
        }
        public BloomParams Bloom {
            get => paramBloom;
            set => paramBloom = value;
        }

        public ChromaticAberrationParams ChromaticAberration {
            get => paramChromaticAberration;
            set {
                paramChromaticAberration = value;
            }
        }
        public ColorGradingParams ColorGrading {
            get => paramColorGrading;
            set {
                paramColorGrading = value;
            }
        }
        public DepthOfFieldParams DepthOfField {
            get => paramDepthOfField;
            set {
                paramDepthOfField = value;
            }
        }
        public GrainLayerParams GrainLayer {
            get => paramGrainLayer;
            set {
                paramGrainLayer = value;
            }
        }
        public ScreenSpaceReflectionParams ScreenSpaceReflection {
            get => paramScreenSpaceReflection;
            set {
                paramScreenSpaceReflection = value;
            }
        }
        public VignetteParams Vignette {
            get => paramVignette;
            set {
                paramVignette = value;
            }
        }

        internal List<string> GetLUTNames() {
            List<string> luts = new List<string> { };
            return luts;
        }
    }
}
