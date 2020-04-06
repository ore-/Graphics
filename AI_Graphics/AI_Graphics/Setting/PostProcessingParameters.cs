using MessagePack;
using UnityEngine;
using UnityEngine.Rendering.PostProcessing;

// Haha
// This is funny
// TODO: Find Better Names and change the names with refactoring tools.
// TODO: Is this the best way to do this?
namespace AIGraphics.Settings {
    [MessagePackObject(keyAsPropertyName: true)]
    public struct AutoExposureParams {
        public BoolValue enabled;
        public Vector2Value filtering; // vector2
        public FloatValue minLuminance;
        public FloatValue maxLuminance;
        public FloatValue keyValue;
        public EyeAdaptationValue eyeAdaptation; // EyeAdaptationParameter
        public FloatValue speedUp;
        public FloatValue speedDown;
        public void Save(UnityEngine.Rendering.PostProcessing.AutoExposure layer) {
            if (layer != null) {
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

        public void Load(UnityEngine.Rendering.PostProcessing.AutoExposure layer) {
            if (layer != null) {
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
    public struct AmbientOcclusionParams {
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
        public void Save(UnityEngine.Rendering.PostProcessing.AmbientOcclusion layer) {
            if (layer != null) {
                this.enabled = new BoolValue(layer.enabled);
                this.mode = new AmbientOcclusionModeValue(layer.mode);
                this.intensity = new FloatValue(layer.intensity);
                this.color = new ColorValue(layer.color);
                this.ambientOnly = new BoolValue(layer.ambientOnly);
                this.noiseFilterTolerance = new FloatValue(layer.noiseFilterTolerance);
                this.blurTolerance = new FloatValue(layer.blurTolerance);
                this.upsampleTolerance = new FloatValue(layer.upsampleTolerance);
                this.thicknessModifier = new FloatValue(layer.thicknessModifier);
                this.directLightingStrength = new FloatValue(layer.directLightingStrength);
                this.radius = new FloatValue(layer.radius);
                this.quality = new AmbientOcclusionQualityValue(layer.quality);
            }
        }

        public void Load(UnityEngine.Rendering.PostProcessing.AmbientOcclusion layer) {
            if (layer != null) {
                this.enabled.Fill(layer.enabled);
                layer.active = layer.enabled.value;
                this.mode.Fill(layer.mode);
                this.intensity.Fill(layer.intensity);
                this.color.Fill(layer.color);
                this.ambientOnly.Fill(layer.ambientOnly);
                this.noiseFilterTolerance.Fill(layer.noiseFilterTolerance);
                this.blurTolerance.Fill(layer.blurTolerance);
                this.upsampleTolerance.Fill(layer.upsampleTolerance);
                this.thicknessModifier.Fill(layer.thicknessModifier);
                this.directLightingStrength.Fill(layer.directLightingStrength);
                this.radius.Fill(layer.radius);
                this.quality.Fill(layer.quality);
            }
        }
    }

    [MessagePackObject(keyAsPropertyName: true)]
    public struct AmplifyOcclusionParams {
        public bool enabled;
        // TODO: figure out how to work with it
    }

    [MessagePackObject(keyAsPropertyName: true)]
    public struct BloomParams {
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

        public void Save(UnityEngine.Rendering.PostProcessing.Bloom layer) {
            if (layer != null) {
                this.enabled = new BoolValue(layer.enabled);
                this.intensity = new FloatValue(layer.intensity);
                this.threshold = new FloatValue(layer.threshold);
                this.softKnee = new FloatValue(layer.softKnee);
                this.clamp = new FloatValue(layer.clamp);
                this.diffusion = new FloatValue(layer.diffusion);
                this.anamorphicRatio = new FloatValue(layer.anamorphicRatio);
                this.color = new ColorValue(layer.color);
                this.fastMode = new BoolValue(layer.fastMode);
                this.dirtIntensity = new FloatValue(layer.dirtIntensity);
                dirtTexture = PostProcessingManager.DirtTexturePath;
                dirtState = layer.dirtTexture.overrideState;
                // dirtTexture is getting saved when dirtTexture is being set from PostProcessingSettings.
                // ref: PostProcessingSettings.cs:167
            }
        }
        public void Load(UnityEngine.Rendering.PostProcessing.Bloom layer) {
            if (layer != null) {
                this.enabled.Fill(layer.enabled);
                layer.active = layer.enabled.value;
                this.intensity.Fill(layer.intensity);
                this.threshold.Fill(layer.threshold);
                this.softKnee.Fill(layer.softKnee);
                this.clamp.Fill(layer.clamp);
                this.diffusion.Fill(layer.diffusion);
                this.anamorphicRatio.Fill(layer.anamorphicRatio);
                this.color.Fill(layer.color);
                this.fastMode.Fill(layer.fastMode);
                this.dirtIntensity.Fill(layer.dirtIntensity);

                layer.dirtTexture.overrideState = dirtState;
                int textureIndex = PostProcessingManager.FindIndexByPath(dirtTexture);
                layer.dirtTexture.value = (textureIndex > 0) ? PostProcessingManager.LensDirts[textureIndex] : null;
            }
        }
    }

    [MessagePackObject(keyAsPropertyName: true)]
    public struct ChromaticAberrationParams {
        public BoolValue enabled;
        public FloatValue intensity;
        public BoolValue fastMode;

        public string spectralLut;
        public void Save(UnityEngine.Rendering.PostProcessing.ChromaticAberration layer, string spectralLutPath = "") {
            if (layer != null) {
                enabled = new BoolValue(layer.enabled);
                intensity = new FloatValue(layer.intensity);
                fastMode = new BoolValue(layer.fastMode);
                //Save Texture path.
                //spectralLut = spectralLutPath;
            }
        }
        public void Load(UnityEngine.Rendering.PostProcessing.ChromaticAberration layer) {
            if (layer != null) {
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
    public struct ColorGradingParams {
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

        public string ldrLutPath; // Formerly Texture
        public string externalLutPath; // Formerly Texture.

        public void Save(UnityEngine.Rendering.PostProcessing.ColorGrading layer) {
            if (layer != null) {
                this.enabled = new BoolValue(layer.enabled);
                this.gradingMode = new GradingModeValue(layer.gradingMode);
                this.mixerGreenOutGreenIn = new FloatValue(layer.mixerGreenOutGreenIn);
                this.mixerGreenOutBlueIn = new FloatValue(layer.mixerGreenOutBlueIn);
                this.mixerBlueOutRedIn = new FloatValue(layer.mixerBlueOutRedIn);
                this.mixerBlueOutGreenIn = new FloatValue(layer.mixerBlueOutGreenIn);
                this.mixerBlueOutBlueIn = new FloatValue(layer.mixerBlueOutBlueIn);
                this.lift = new Vector4Value(layer.lift);
                this.gamma = new Vector4Value(layer.gamma);
                this.mixerGreenOutRedIn = new FloatValue(layer.mixerGreenOutRedIn);
                this.gain = new Vector4Value(layer.gain);
                this.mixerRedOutBlueIn = new FloatValue(layer.mixerRedOutBlueIn);
                this.mixerRedOutGreenIn = new FloatValue(layer.mixerRedOutGreenIn);
                this.toneCurveToeStrength = new FloatValue(layer.toneCurveToeStrength);
                this.toneCurveToeLength = new FloatValue(layer.toneCurveToeLength);
                this.toneCurveShoulderStrength = new FloatValue(layer.toneCurveShoulderStrength);
                this.toneCurveShoulderLength = new FloatValue(layer.toneCurveShoulderLength);
                this.toneCurveShoulderAngle = new FloatValue(layer.toneCurveShoulderAngle);
                this.toneCurveGamma = new FloatValue(layer.toneCurveGamma);
                this.mixerRedOutRedIn = new FloatValue(layer.mixerRedOutRedIn);
                this.tonemapper = new TonemapperValue(layer.tonemapper);
                this.ldrLutContribution = new FloatValue(layer.ldrLutContribution);
                this.tint = new FloatValue(layer.tint);
                this.colorFilter = new ColorValue(layer.colorFilter);
                this.hueShift = new FloatValue(layer.hueShift);
                this.saturation = new FloatValue(layer.saturation);
                this.brightness = new FloatValue(layer.brightness);
                this.postExposure = new FloatValue(layer.postExposure);
                this.contrast = new FloatValue(layer.contrast);
                this.temperature = new FloatValue(layer.temperature);

                //ldrLutPath = lutPath; // Formerly Texture
                //externalLutPath = extLutPath; // Formerly Texture.
            }
        }
        public void Load(UnityEngine.Rendering.PostProcessing.ColorGrading layer) {
            if (layer != null) {
                this.enabled.Fill(layer.enabled);
                layer.active = layer.enabled.value;
                this.gradingMode.Fill(layer.gradingMode);
                this.mixerGreenOutGreenIn.Fill(layer.mixerGreenOutGreenIn);
                this.mixerGreenOutBlueIn.Fill(layer.mixerGreenOutBlueIn);
                this.mixerBlueOutRedIn.Fill(layer.mixerBlueOutRedIn);
                this.mixerBlueOutGreenIn.Fill(layer.mixerBlueOutGreenIn);
                this.mixerBlueOutBlueIn.Fill(layer.mixerBlueOutBlueIn);
                this.lift.Fill(layer.lift);
                this.gamma.Fill(layer.gamma);
                this.mixerGreenOutRedIn.Fill(layer.mixerGreenOutRedIn);
                this.gain.Fill(layer.gain);
                this.mixerRedOutBlueIn.Fill(layer.mixerRedOutBlueIn);
                this.mixerRedOutGreenIn.Fill(layer.mixerRedOutGreenIn);
                this.toneCurveToeStrength.Fill(layer.toneCurveToeStrength);
                this.toneCurveToeLength.Fill(layer.toneCurveToeLength);
                this.toneCurveShoulderStrength.Fill(layer.toneCurveShoulderStrength);
                this.toneCurveShoulderLength.Fill(layer.toneCurveShoulderLength);
                this.toneCurveShoulderAngle.Fill(layer.toneCurveShoulderAngle);
                this.toneCurveGamma.Fill(layer.toneCurveGamma);
                this.mixerRedOutRedIn.Fill(layer.mixerRedOutRedIn);
                this.tonemapper.Fill(layer.tonemapper);
                this.ldrLutContribution.Fill(layer.ldrLutContribution);
                this.tint.Fill(layer.tint);
                this.colorFilter.Fill(layer.colorFilter);
                this.hueShift.Fill(layer.hueShift);
                this.saturation.Fill(layer.saturation);
                this.brightness.Fill(layer.brightness);
                this.postExposure.Fill(layer.postExposure);
                this.contrast.Fill(layer.contrast);
                this.temperature.Fill(layer.temperature);

                // Load from certain directory.
                //layer.ldrLutPath.value = ldrLutPath;
                //layer.externalLutPath.value = externalLutPath;
            }
        }
    }

    [MessagePackObject(keyAsPropertyName: true)]
    public struct DepthOfFieldParams {
        public BoolValue enabled;
        public FloatValue focusDistance;
        public FloatValue aperture;
        public FloatValue focalLength;
        public KernelSizeValue kernelSize;

        public void Save(UnityEngine.Rendering.PostProcessing.DepthOfField layer) {
            if (layer != null) {
                enabled = new BoolValue(layer.enabled);
                focusDistance = new FloatValue(layer.focusDistance);
                aperture = new FloatValue(layer.aperture);
                focalLength = new FloatValue(layer.focalLength);
                kernelSize = new KernelSizeValue(layer.kernelSize);
            }
        }
        public void Load(UnityEngine.Rendering.PostProcessing.DepthOfField layer) {
            if (layer != null) {
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
    public struct GrainLayerParams {
        public BoolValue enabled;
        public BoolValue colored;
        public FloatValue intensity;
        public FloatValue size;
        public FloatValue lumContrib;

        public void Save(UnityEngine.Rendering.PostProcessing.Grain layer) {
            if (layer != null) {
                enabled = new BoolValue(layer.enabled);
                colored = new BoolValue(layer.colored);
                intensity = new FloatValue(layer.intensity);
                size = new FloatValue(layer.size);
                lumContrib = new FloatValue(layer.lumContrib);
            }
        }
        public void Load(UnityEngine.Rendering.PostProcessing.Grain layer) {
            if (layer != null) {
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
    public struct ScreenSpaceReflectionParams {
        public BoolValue enabled;
        public ScreenSpaceReflectionPresetValue preset;
        public IntValue maximumIterationCount;
        public ScreenSpaceReflectionResolutionValue resolution;
        public FloatValue thickness;
        public FloatValue maximumMarchDistance;
        public FloatValue distanceFade;
        public FloatValue vignette;

        public void Save(UnityEngine.Rendering.PostProcessing.ScreenSpaceReflections layer) {
            if (layer != null) {
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
        public void Load(UnityEngine.Rendering.PostProcessing.ScreenSpaceReflections layer) {
            if (layer != null) {
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
    public struct VignetteParams {
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

        public void Save(UnityEngine.Rendering.PostProcessing.Vignette layer, string maskPath = "") {
            if (layer != null) {
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
        public void Load(UnityEngine.Rendering.PostProcessing.Vignette layer) {
            if (layer != null) {
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
