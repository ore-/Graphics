using AIGraphics.Inspector;
using MessagePack;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Rendering.PostProcessing;
using static KKAPI.Studio.StudioAPI;

// TODO: Make Post Process Layer more cleaner?
namespace AIGraphics.Settings {
    [MessagePackObject(keyAsPropertyName: true)]
    public class PostProcessingSettings {
        internal enum Antialiasing {
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

        // Parameters for Messagepack Save
        public PostProcessingParameter.AmbientOcclusion paramAmbientOcclusion = new PostProcessingParameter.AmbientOcclusion();
        public PostProcessingParameter.AutoExposure paramAutoExposure = new PostProcessingParameter.AutoExposure();
        public PostProcessingParameter.Bloom paramBloom = new PostProcessingParameter.Bloom();
        public PostProcessingParameter.ChromaticAberration paramChromaticAberration = new PostProcessingParameter.ChromaticAberration();
        public PostProcessingParameter.ColorGrading paramColorGrading = new PostProcessingParameter.ColorGrading();
        public PostProcessingParameter.DepthOfField paramDepthOfField = new PostProcessingParameter.DepthOfField();
        public PostProcessingParameter.GrainLayer paramGrainLayer = new PostProcessingParameter.GrainLayer();
        public PostProcessingParameter.ScreenSpaceReflection paramScreenSpaceReflection = new PostProcessingParameter.ScreenSpaceReflection();
        public PostProcessingParameter.Vignette paramVignette = new PostProcessingParameter.Vignette();
        //public PostProcessingParameter.AmplifyOcclusion paramAmplifyOcclusion; // not implemented yet

        public PostProcessingSettings() {
            postProcessVolumes = GameObject.FindObjectsOfType<PostProcessVolume>();
            volumeNames = postProcessVolumes.Select(volume => volume.name).ToArray();
        }

        public PostProcessingSettings(Camera camera) {
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

        internal LayerMask VolumeLayerSetting {
            //get => LayerMask.LayerToName(_postProcessLayer.volumeLayer.value);
            get => _postProcessLayer.volumeLayer;
        }

        internal float JitterSpread {
            get => _postProcessLayer.temporalAntialiasing.jitterSpread;
            set => _postProcessLayer.temporalAntialiasing.jitterSpread = value;
        }

        internal float StationaryBlending {
            get => _postProcessLayer.temporalAntialiasing.stationaryBlending;
            set => _postProcessLayer.temporalAntialiasing.stationaryBlending = value;
        }

        internal float MotionBlending {
            get => _postProcessLayer.temporalAntialiasing.motionBlending;
            set => _postProcessLayer.temporalAntialiasing.motionBlending = value;
        }

        internal float Sharpness {
            get => _postProcessLayer.temporalAntialiasing.sharpness;
            set => _postProcessLayer.temporalAntialiasing.sharpness = value;
        }

        internal bool FXAAMode {
            get => _postProcessLayer.fastApproximateAntialiasing.fastMode;
            set => _postProcessLayer.fastApproximateAntialiasing.fastMode = value;
        }

        internal bool FXAAAlpha {
            get => _postProcessLayer.fastApproximateAntialiasing.keepAlpha;
            set => _postProcessLayer.fastApproximateAntialiasing.keepAlpha = value;
        }

        internal SubpixelMorphologicalAntialiasing.Quality SMAAQuality {
            get => _postProcessLayer.subpixelMorphologicalAntialiasing.quality;
            set => _postProcessLayer.subpixelMorphologicalAntialiasing.quality = value;
        }

        internal Antialiasing AntialiasingMode {
            get => (Antialiasing)_postProcessLayer.antialiasingMode;
            set => _postProcessLayer.antialiasingMode = (PostProcessLayer.Antialiasing)value;
        }

        internal float FocalDistance {
            get => depthOfFieldLayers[0].focusDistance.value;
            set => depthOfFieldLayers[0].focusDistance.value = value;
        }

        internal string CurrentLUT {
            get => selectedLUT;
            set {
                if (null != value && value != selectedLUT) {
                    selectedLUT = value;
                }
            }
        }

        internal List<string> GetLUTNames() {
            List<string> luts = new List<string> { };
            return luts;
        }
    }
}
