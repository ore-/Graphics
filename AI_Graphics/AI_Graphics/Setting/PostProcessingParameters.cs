using MessagePack;
using UnityEngine;

// Haha
// This is funny
// TODO: Find Better Names and change the names with refactoring tools.
// TODO: Is this the best way to do this?
namespace AIGraphics.Settings
{
    [MessagePackObject(keyAsPropertyName: true)]
    public struct AutoExposureParams
    {
        public BoolValue enabled;
        public Vector2Value filtering; // vector2
        public FloatValue minLuminance;
        public FloatValue maxLuminance;
        public FloatValue keyValue;
        public EyeAdaptationValue eyeAdaptation; // EyeAdaptationParameter
        public FloatValue speedUp;
        public FloatValue speedDown;
        public void Save(UnityEngine.Rendering.PostProcessing.AutoExposure layer)
        {
            if (layer != null)
            {
                enabled = new BoolValue(layer.enabled);
                filtering = new Vector2Value(layer.filtering);
                minLuminance = new FloatValue(layer.minLuminance);
                maxLuminance = new FloatValue(layer.maxLuminance);
                keyValue = new FloatValue(layer.keyValue);
                speedUp = new FloatValue(layer.speedUp);
                speedDown = new FloatValue(layer.speedDown);
                eyeAdaptation = new EyeAdaptationValue(layer.eyeAdaptation);
            }
        }

        public void Load(UnityEngine.Rendering.PostProcessing.AutoExposure layer)
        {
            if (layer != null)
            {
                enabled.Fill(layer.enabled);
                layer.active = layer.enabled.value;
                filtering.Fill(layer.filtering);
                minLuminance.Fill(layer.minLuminance);
                maxLuminance.Fill(layer.maxLuminance);
                keyValue.Fill(layer.keyValue);
                speedUp.Fill(layer.speedUp);
                speedDown.Fill(layer.speedDown);
                eyeAdaptation.Fill(layer.eyeAdaptation);
            }
        }
    }

    [MessagePackObject(keyAsPropertyName: true)]
    public struct AmbientOcclusionParams
    {
        public BoolValue enabled;
        public AmbientOcclusionModeValue mode;
        public FloatValue intensity;
        public ColorValue color;
        public BoolValue ambientOnly;
        public FloatValue noiseFilterTolerance;
        public FloatValue blurTolerance;
        public FloatValue upsampleTolerance;
        public FloatValue thicknessModifier;
        public FloatValue directLightingStrength;
        public FloatValue radius;
        public AmbientOcclusionQualityValue quality;
        public void Save(UnityEngine.Rendering.PostProcessing.AmbientOcclusion layer)
        {
            if (layer != null)
            {
                enabled = new BoolValue(layer.enabled);
                mode = new AmbientOcclusionModeValue(layer.mode);
                intensity = new FloatValue(layer.intensity);
                color = new ColorValue(layer.color);
                ambientOnly = new BoolValue(layer.ambientOnly);
                noiseFilterTolerance = new FloatValue(layer.noiseFilterTolerance);
                blurTolerance = new FloatValue(layer.blurTolerance);
                upsampleTolerance = new FloatValue(layer.upsampleTolerance);
                thicknessModifier = new FloatValue(layer.thicknessModifier);
                directLightingStrength = new FloatValue(layer.directLightingStrength);
                radius = new FloatValue(layer.radius);
                quality = new AmbientOcclusionQualityValue(layer.quality);
            }
        }

        public void Load(UnityEngine.Rendering.PostProcessing.AmbientOcclusion layer)
        {
            if (layer != null)
            {
                enabled.Fill(layer.enabled);
                layer.active = layer.enabled.value;
                mode.Fill(layer.mode);
                intensity.Fill(layer.intensity);
                color.Fill(layer.color);
                ambientOnly.Fill(layer.ambientOnly);
                noiseFilterTolerance.Fill(layer.noiseFilterTolerance);
                blurTolerance.Fill(layer.blurTolerance);
                upsampleTolerance.Fill(layer.upsampleTolerance);
                thicknessModifier.Fill(layer.thicknessModifier);
                directLightingStrength.Fill(layer.directLightingStrength);
                radius.Fill(layer.radius);
                quality.Fill(layer.quality);
            }
        }
    }

    [MessagePackObject(keyAsPropertyName: true)]
    public struct AmplifyOcclusionParams
    {
        public bool enabled;
        // TODO: figure out how to work with it
    }

    [MessagePackObject(keyAsPropertyName: true)]
    public struct BloomParams
    {
        public BoolValue enabled;
        public FloatValue intensity;
        public FloatValue threshold;
        public FloatValue softKnee;
        public FloatValue clamp;
        public FloatValue diffusion;
        public FloatValue anamorphicRatio;
        public ColorValue color;
        public BoolValue fastMode;
        public FloatValue dirtIntensity;
        public string dirtTexture;
        public bool dirtState;

        public void Save(UnityEngine.Rendering.PostProcessing.Bloom layer)
        {
            if (layer != null)
            {
                enabled = new BoolValue(layer.enabled);
                intensity = new FloatValue(layer.intensity);
                threshold = new FloatValue(layer.threshold);
                softKnee = new FloatValue(layer.softKnee);
                clamp = new FloatValue(layer.clamp);
                diffusion = new FloatValue(layer.diffusion);
                anamorphicRatio = new FloatValue(layer.anamorphicRatio);
                color = new ColorValue(layer.color);
                fastMode = new BoolValue(layer.fastMode);
                dirtIntensity = new FloatValue(layer.dirtIntensity);
                dirtTexture = PostProcessingManager.lensDirtTexture.lookupPath;
                dirtState = layer.dirtTexture.overrideState;
                // dirtTexture is getting saved when dirtTexture is being set from PostProcessingSettings.
                // ref: PostProcessingSettings.cs:167
            }
        }
        public void Load(UnityEngine.Rendering.PostProcessing.Bloom layer)
        {
            if (layer != null)
            {
                enabled.Fill(layer.enabled);
                layer.active = layer.enabled.value;
                intensity.Fill(layer.intensity);
                threshold.Fill(layer.threshold);
                softKnee.Fill(layer.softKnee);
                clamp.Fill(layer.clamp);
                diffusion.Fill(layer.diffusion);
                anamorphicRatio.Fill(layer.anamorphicRatio);
                color.Fill(layer.color);
                fastMode.Fill(layer.fastMode);
                dirtIntensity.Fill(layer.dirtIntensity);

                layer.dirtTexture.overrideState = dirtState;

                PostProcessingManager.lensDirtTexture.SetIndexByPath(dirtTexture);
                layer.dirtTexture.value = PostProcessingManager.lensDirtTexture.Texture;
            }
        }
    }

    [MessagePackObject(keyAsPropertyName: true)]
    public struct ChromaticAberrationParams
    {
        public BoolValue enabled;
        public FloatValue intensity;
        public BoolValue fastMode;

        public string spectralLut;
        public void Save(UnityEngine.Rendering.PostProcessing.ChromaticAberration layer, string spectralLutPath = "")
        {
            if (layer != null)
            {
                enabled = new BoolValue(layer.enabled);
                intensity = new FloatValue(layer.intensity);
                fastMode = new BoolValue(layer.fastMode);
                //Save Texture path.
                //spectralLut = spectralLutPath;
            }
        }
        public void Load(UnityEngine.Rendering.PostProcessing.ChromaticAberration layer)
        {
            if (layer != null)
            {
                enabled.Fill(layer.enabled);
                layer.active = layer.enabled.value;
                intensity.Fill(layer.intensity);
                fastMode.Fill(layer.fastMode);

                //Load texture from the path.
                //layer.spectralLutPath.value = spectralLut;
            }
        }
    }

    [MessagePackObject(keyAsPropertyName: true)]
    public struct ColorGradingParams
    {
        //internal SplineParameter redCurve; // Figure out messagepack parser later.
        //internal SplineParameter greenCurve; // Figure out messagepack parser later.
        //internal SplineParameter blueCurve; // Figure out messagepack parser later.
        //internal SplineParameter hueVsHueCurve; // Figure out messagepack parser later.
        //internal SplineParameter hueVsSatCurve; // Figure out messagepack parser later.
        //internal SplineParameter satVsSatCurve; // Figure out messagepack parser later.
        //internal SplineParameter lumVsSatCurve; // Figure out messagepack parser later.
        //internal SplineParameter masterCurve; // Figure out messagepack parser later.

        public BoolValue enabled;
        public GradingModeValue gradingMode;
        public FloatValue mixerGreenOutGreenIn;
        public FloatValue mixerGreenOutBlueIn;
        public FloatValue mixerBlueOutRedIn;
        public FloatValue mixerBlueOutGreenIn;
        public FloatValue mixerBlueOutBlueIn;
        public Vector4Value lift;
        public Vector4Value gamma;
        public FloatValue mixerGreenOutRedIn;
        public Vector4Value gain;
        public FloatValue mixerRedOutBlueIn;
        public FloatValue mixerRedOutGreenIn;
        public FloatValue toneCurveToeStrength;
        public FloatValue toneCurveToeLength;
        public FloatValue toneCurveShoulderStrength;
        public FloatValue toneCurveShoulderLength;
        public FloatValue toneCurveShoulderAngle;
        public FloatValue toneCurveGamma;
        public FloatValue mixerRedOutRedIn;
        public TonemapperValue tonemapper;
        public FloatValue ldrLutContribution;
        public FloatValue tint;
        public ColorValue colorFilter;
        public FloatValue hueShift;
        public FloatValue saturation;
        public FloatValue brightness;
        public FloatValue postExposure;
        public FloatValue contrast;
        public FloatValue temperature;
        public IntValue ldrLutIndex;
        public string externalLutPath; // Formerly Texture.

        public void Save(UnityEngine.Rendering.PostProcessing.ColorGrading layer)
        {
            if (layer != null)
            {
                enabled = new BoolValue(layer.enabled);
                gradingMode = new GradingModeValue(layer.gradingMode);
                mixerGreenOutGreenIn = new FloatValue(layer.mixerGreenOutGreenIn);
                mixerGreenOutBlueIn = new FloatValue(layer.mixerGreenOutBlueIn);
                mixerBlueOutRedIn = new FloatValue(layer.mixerBlueOutRedIn);
                mixerBlueOutGreenIn = new FloatValue(layer.mixerBlueOutGreenIn);
                mixerBlueOutBlueIn = new FloatValue(layer.mixerBlueOutBlueIn);
                lift = new Vector4Value(layer.lift);
                gamma = new Vector4Value(layer.gamma);
                mixerGreenOutRedIn = new FloatValue(layer.mixerGreenOutRedIn);
                gain = new Vector4Value(layer.gain);
                mixerRedOutBlueIn = new FloatValue(layer.mixerRedOutBlueIn);
                mixerRedOutGreenIn = new FloatValue(layer.mixerRedOutGreenIn);
                toneCurveToeStrength = new FloatValue(layer.toneCurveToeStrength);
                toneCurveToeLength = new FloatValue(layer.toneCurveToeLength);
                toneCurveShoulderStrength = new FloatValue(layer.toneCurveShoulderStrength);
                toneCurveShoulderLength = new FloatValue(layer.toneCurveShoulderLength);
                toneCurveShoulderAngle = new FloatValue(layer.toneCurveShoulderAngle);
                toneCurveGamma = new FloatValue(layer.toneCurveGamma);
                mixerRedOutRedIn = new FloatValue(layer.mixerRedOutRedIn);
                tonemapper = new TonemapperValue(layer.tonemapper);
                ldrLutContribution = new FloatValue(layer.ldrLutContribution);
                tint = new FloatValue(layer.tint);
                colorFilter = new ColorValue(layer.colorFilter);
                hueShift = new FloatValue(layer.hueShift);
                saturation = new FloatValue(layer.saturation);
                brightness = new FloatValue(layer.brightness);
                postExposure = new FloatValue(layer.postExposure);
                contrast = new FloatValue(layer.contrast);
                temperature = new FloatValue(layer.temperature);
                ldrLutIndex = new IntValue(PostProcessingManager.lutTexture.Index, layer.ldrLut.overrideState);
                //externalLutPath = extLutPath; // Formerly Texture.
            }
        }
        public void Load(UnityEngine.Rendering.PostProcessing.ColorGrading layer)
        {
            if (layer != null)
            {
                enabled.Fill(layer.enabled);
                layer.active = layer.enabled.value;
                gradingMode.Fill(layer.gradingMode);
                mixerGreenOutGreenIn.Fill(layer.mixerGreenOutGreenIn);
                mixerGreenOutBlueIn.Fill(layer.mixerGreenOutBlueIn);
                mixerBlueOutRedIn.Fill(layer.mixerBlueOutRedIn);
                mixerBlueOutGreenIn.Fill(layer.mixerBlueOutGreenIn);
                mixerBlueOutBlueIn.Fill(layer.mixerBlueOutBlueIn);
                lift.Fill(layer.lift);
                gamma.Fill(layer.gamma);
                mixerGreenOutRedIn.Fill(layer.mixerGreenOutRedIn);
                gain.Fill(layer.gain);
                mixerRedOutBlueIn.Fill(layer.mixerRedOutBlueIn);
                mixerRedOutGreenIn.Fill(layer.mixerRedOutGreenIn);
                toneCurveToeStrength.Fill(layer.toneCurveToeStrength);
                toneCurveToeLength.Fill(layer.toneCurveToeLength);
                toneCurveShoulderStrength.Fill(layer.toneCurveShoulderStrength);
                toneCurveShoulderLength.Fill(layer.toneCurveShoulderLength);
                toneCurveShoulderAngle.Fill(layer.toneCurveShoulderAngle);
                toneCurveGamma.Fill(layer.toneCurveGamma);
                mixerRedOutRedIn.Fill(layer.mixerRedOutRedIn);
                tonemapper.Fill(layer.tonemapper);
                ldrLutContribution.Fill(layer.ldrLutContribution);
                tint.Fill(layer.tint);
                colorFilter.Fill(layer.colorFilter);
                hueShift.Fill(layer.hueShift);
                saturation.Fill(layer.saturation);
                brightness.Fill(layer.brightness);
                postExposure.Fill(layer.postExposure);
                contrast.Fill(layer.contrast);
                temperature.Fill(layer.temperature);

                PostProcessingManager.lutTexture.Index = ldrLutIndex.value;
                if (PostProcessingManager.lutTexture.TryGetTexture(out Texture2D texture))
                {
                    layer.ldrLut.value = texture;
                }
                else
                {
                    layer.ldrLut.value = null;
                }

                layer.ldrLut.overrideState = ldrLutIndex.overrideState;
                //layer.externalLutPath.value = externalLutPath;
            }
        }
    }

    [MessagePackObject(keyAsPropertyName: true)]
    public struct DepthOfFieldParams
    {
        public BoolValue enabled;
        public FloatValue focusDistance;
        public FloatValue aperture;
        public FloatValue focalLength;
        public KernelSizeValue kernelSize;

        public void Save(UnityEngine.Rendering.PostProcessing.DepthOfField layer)
        {
            if (layer != null)
            {
                enabled = new BoolValue(layer.enabled);
                focusDistance = new FloatValue(layer.focusDistance);
                aperture = new FloatValue(layer.aperture);
                focalLength = new FloatValue(layer.focalLength);
                kernelSize = new KernelSizeValue(layer.kernelSize);
            }
        }

        public void Load(UnityEngine.Rendering.PostProcessing.DepthOfField layer)
        {
            if (layer != null)
            {
                enabled.Fill(layer.enabled);
                layer.active = layer.enabled.value;
                focusDistance.Fill(layer.focusDistance);
                aperture.Fill(layer.aperture);
                focalLength.Fill(layer.focalLength);
                kernelSize.Fill(layer.kernelSize);
            }
        }
    }

    [MessagePackObject(keyAsPropertyName: true)]
    public struct GrainLayerParams
    {
        public BoolValue enabled;
        public BoolValue colored;
        public FloatValue intensity;
        public FloatValue size;
        public FloatValue lumContrib;

        public void Save(UnityEngine.Rendering.PostProcessing.Grain layer)
        {
            if (layer != null)
            {
                enabled = new BoolValue(layer.enabled);
                colored = new BoolValue(layer.colored);
                intensity = new FloatValue(layer.intensity);
                size = new FloatValue(layer.size);
                lumContrib = new FloatValue(layer.lumContrib);
            }
        }
        public void Load(UnityEngine.Rendering.PostProcessing.Grain layer)
        {
            if (layer != null)
            {
                enabled.Fill(layer.enabled);
                layer.active = layer.enabled.value;
                colored.Fill(layer.colored);
                intensity.Fill(layer.intensity);
                size.Fill(layer.size);
                lumContrib.Fill(layer.lumContrib);
            }
        }
    }

    [MessagePackObject(keyAsPropertyName: true)]
    public struct ScreenSpaceReflectionParams
    {
        public BoolValue enabled;
        public ScreenSpaceReflectionPresetValue preset;
        public IntValue maximumIterationCount;
        public ScreenSpaceReflectionResolutionValue resolution;
        public FloatValue thickness;
        public FloatValue maximumMarchDistance;
        public FloatValue distanceFade;
        public FloatValue vignette;

        public void Save(UnityEngine.Rendering.PostProcessing.ScreenSpaceReflections layer)
        {
            if (layer != null)
            {
                enabled = new BoolValue(layer.enabled);
                preset = new ScreenSpaceReflectionPresetValue(layer.preset);
                maximumIterationCount = new IntValue(layer.maximumIterationCount);
                resolution = new ScreenSpaceReflectionResolutionValue(layer.resolution);
                thickness = new FloatValue(layer.thickness);
                maximumMarchDistance = new FloatValue(layer.maximumMarchDistance);
                distanceFade = new FloatValue(layer.distanceFade);
                vignette = new FloatValue(layer.vignette);
            }
        }
        public void Load(UnityEngine.Rendering.PostProcessing.ScreenSpaceReflections layer)
        {
            if (layer != null)
            {
                enabled.Fill(layer.enabled);
                layer.active = layer.enabled.value;
                preset.Fill(layer.preset);
                maximumIterationCount.Fill(layer.maximumIterationCount);
                resolution.Fill(layer.resolution);
                thickness.Fill(layer.thickness);
                maximumMarchDistance.Fill(layer.maximumMarchDistance);
                distanceFade.Fill(layer.distanceFade);
                vignette.Fill(layer.vignette);
            }
        }
    }

    [MessagePackObject(keyAsPropertyName: true)]
    public struct VignetteParams
    {
        public BoolValue enabled;
        public VignetteModeValue mode;
        public ColorValue color; //vector3
        public Vector2Value center; //vector2
        public FloatValue intensity;
        public FloatValue smoothness;
        public FloatValue roundness;
        public BoolValue rounded;
        public FloatValue opacity;
        public string mask; //Mask Texture

        public void Save(UnityEngine.Rendering.PostProcessing.Vignette layer, string maskPath = "")
        {
            if (layer != null)
            {
                enabled = new BoolValue(layer.enabled);
                mode = new VignetteModeValue(layer.mode);
                color = new ColorValue(layer.color);
                center = new Vector2Value(layer.center);
                intensity = new FloatValue(layer.intensity);
                smoothness = new FloatValue(layer.smoothness);
                roundness = new FloatValue(layer.roundness);
                rounded = new BoolValue(layer.rounded);
                opacity = new FloatValue(layer.opacity);

                //Save path from the post process object?
                //mask = maskPath;
            }
        }
        public void Load(UnityEngine.Rendering.PostProcessing.Vignette layer)
        {
            if (layer != null)
            {
                enabled.Fill(layer.enabled);
                layer.active = layer.enabled.value;
                mode.Fill(layer.mode);
                color.Fill(layer.color);
                center.Fill(layer.center);
                intensity.Fill(layer.intensity);
                smoothness.Fill(layer.smoothness);
                roundness.Fill(layer.roundness);
                rounded.Fill(layer.rounded);
                opacity.Fill(layer.opacity);

                // Load Texture from the Path.
                // layer.mask.value = mask;
            }
        }
    }
}
