using AIGraphics.Inspector;
using MessagePack;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Rendering.PostProcessing;
using static KKAPI.Studio.StudioAPI;

namespace AIGraphics.Settings
{
    [MessagePackObject(keyAsPropertyName: true)]
    internal class PostProcessingSettings
    {
        internal enum Antialiasing
        {
            None = PostProcessLayer.Antialiasing.None,
            FXAA = PostProcessLayer.Antialiasing.FastApproximateAntialiasing,
            SMAA = PostProcessLayer.Antialiasing.SubpixelMorphologicalAntialiasing,
            TAA = PostProcessLayer.Antialiasing.TemporalAntialiasing
        };

        private string selectedLUT;
        internal List<string> LUTNames { get; set; }
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

        internal PostProcessingSettings(Camera camera)
        {
            _postProcessLayer = camera.GetComponent<PostProcessLayer>();
            postProcessVolumes = GameObject.FindObjectsOfType<PostProcessVolume>();
            volumeNames = postProcessVolumes.Select(volume => volume.name).ToArray();
            for (int i = 0; i < postProcessVolumes.Length; i++)
            {
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

        internal Transform VolumeTriggerSetting
        {
            get => _postProcessLayer.volumeTrigger;
        }

        internal LayerMask VolumeLayerSetting
        {
            //get => LayerMask.LayerToName(_postProcessLayer.volumeLayer.value);
            get => _postProcessLayer.volumeLayer;
        }

        internal float JitterSpread
        {
            get => _postProcessLayer.temporalAntialiasing.jitterSpread;
            set => _postProcessLayer.temporalAntialiasing.jitterSpread = value;
        }

        internal float StationaryBlending
        {
            get => _postProcessLayer.temporalAntialiasing.stationaryBlending;
            set => _postProcessLayer.temporalAntialiasing.stationaryBlending = value;
        }

        internal float MotionBlending
        {
            get => _postProcessLayer.temporalAntialiasing.motionBlending;
            set => _postProcessLayer.temporalAntialiasing.motionBlending = value;
        }

        internal float Sharpness
        {
            get => _postProcessLayer.temporalAntialiasing.sharpness;
            set => _postProcessLayer.temporalAntialiasing.sharpness = value;
        }

        internal bool FXAAMode
        {
            get => _postProcessLayer.fastApproximateAntialiasing.fastMode;
            set => _postProcessLayer.fastApproximateAntialiasing.fastMode = value;
        }

        internal bool FXAAAlpha
        {
            get => _postProcessLayer.fastApproximateAntialiasing.keepAlpha;
            set => _postProcessLayer.fastApproximateAntialiasing.keepAlpha = value;
        }

        internal SubpixelMorphologicalAntialiasing.Quality SMAAQuality
        {
            get => _postProcessLayer.subpixelMorphologicalAntialiasing.quality;
            set => _postProcessLayer.subpixelMorphologicalAntialiasing.quality = value;
        }

        internal Antialiasing AntialiasingMode
        {
            get => (Antialiasing)_postProcessLayer.antialiasingMode;
            set => _postProcessLayer.antialiasingMode = (PostProcessLayer.Antialiasing)value;
        }

        internal float FocalDistance
        {
            get => depthOfFieldLayers[0].focusDistance.value;
            set => depthOfFieldLayers[0].focusDistance.value = value;
        }

        internal string CurrentLUT
        {
            get => selectedLUT;
            set
            {
                if (null != value && value != selectedLUT)
                {
                    selectedLUT = value;
                }
            }
        }

        internal List<string> GetLUTNames()
        {
            List<string> luts = new List<string> { };
            return luts;
        }
    }
}
