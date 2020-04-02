using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MessagePack;
using UnityEngine;
using UnityEngine.Rendering.PostProcessing;

// Haha
// This is funny
// TODO: Find Better Names and change the names with refactoring tools.
// TODO: Is this the best way to do this?
namespace AIGraphics.PostProcessingParameter {
    [MessagePackObject(keyAsPropertyName: true)]
    public class AutoExposure {
        public bool enabled;
        public float[] filtering; // vector2
        public float minLuminance;
        public float maxLuminance;
        public float keyValue;
        public EyeAdaptation eyeAdaptation; // EyeAdaptationParameter
        public float speedUp;
        public float speedDown;
        public void Save(UnityEngine.Rendering.PostProcessing.AutoExposure layer) {
            enabled = layer.enabled.value;
            if (layer != null) {
                filtering = new float[2] { layer.filtering.value[0], layer.filtering.value[1] };
                minLuminance = layer.minLuminance.value;
                maxLuminance = layer.maxLuminance.value;
                keyValue = layer.keyValue.value;
                eyeAdaptation = layer.eyeAdaptation.value;
                speedUp = layer.speedUp.value;
                speedDown = layer.speedDown.value;
            }
        }

        public void Load(UnityEngine.Rendering.PostProcessing.AutoExposure layer) {
            layer.enabled.value = enabled;
            if (layer != null) {
                layer.filtering.value = new Vector2(filtering[0], filtering[1]);
                layer.minLuminance.value = minLuminance;
                layer.maxLuminance.value = maxLuminance;
                layer.keyValue.value = keyValue;
                layer.eyeAdaptation.value = eyeAdaptation;
                layer.speedUp.value = speedUp;
                layer.speedDown.value = speedDown;
            }
        }
    }

    [MessagePackObject(keyAsPropertyName: true)]
    public class AmbientOcclusion {
        public bool enabled;
        public AmbientOcclusionMode mode;
        public float intensity;
        public float[] color;
        public bool ambientOnly;
        public float noiseFilterTolerance;
        public float blurTolerance;
        public float upsampleTolerance;
        public float thicknessModifier;
        public float directLightingStrength;
        public float radius;
        public AmbientOcclusionQuality quality;
        public void Save(UnityEngine.Rendering.PostProcessing.AmbientOcclusion layer) {
            enabled = layer.enabled.value;
            if (layer != null) {
                mode = layer.mode.value;
                intensity = layer.intensity.value;
                color = new float[3] { layer.color.value[0], layer.color.value[1], layer.color.value[2] };
                ambientOnly = layer.ambientOnly.value;
                noiseFilterTolerance = layer.noiseFilterTolerance.value;
                blurTolerance = layer.blurTolerance.value;
                upsampleTolerance = layer.upsampleTolerance.value;
                thicknessModifier = layer.thicknessModifier.value;
                directLightingStrength = layer.directLightingStrength.value;
                radius = layer.radius.value;
                quality = layer.quality.value;
            }
        }

        public void Load(UnityEngine.Rendering.PostProcessing.AmbientOcclusion layer) {
            layer.enabled.value = enabled;
            if (layer != null) {
                layer.mode.value = mode;
                layer.intensity.value = intensity;
                layer.color.value = new Color(color[0], color[1], color[2]);
                layer.ambientOnly.value = ambientOnly;
                layer.noiseFilterTolerance.value = noiseFilterTolerance;
                layer.blurTolerance.value = blurTolerance;
                layer.upsampleTolerance.value = upsampleTolerance;
                layer.thicknessModifier.value = thicknessModifier;
                layer.directLightingStrength.value = directLightingStrength;
                layer.radius.value = radius;
                layer.quality.value = quality;
            }
        }
    }

    [MessagePackObject(keyAsPropertyName: true)]
    public class AmplifyOcclusion {
        public bool enabled;
        // TODO: figure out how to work with it
    }

    [MessagePackObject(keyAsPropertyName: true)]
    public class Bloom {
        public bool enabled;
        public float intensity;
        public float threshold;
        public float softKnee;
        public float clamp;
        public float diffusion;
        public float anamorphicRatio;
        public float[] color;
        public bool fastMode;
        public string dirtTexture;
        public float dirtIntensity;
        public void Save(UnityEngine.Rendering.PostProcessing.Bloom layer, string dirtTexturePath = "") {
            enabled = layer.enabled.value;
            intensity = layer.intensity.value;
            threshold = layer.threshold.value;
            softKnee = layer.softKnee.value;
            clamp = layer.clamp.value;
            diffusion = layer.diffusion.value;
            anamorphicRatio = layer.anamorphicRatio.value;
            color = new float[3] { layer.color.value[0], layer.color.value[1], layer.color.value[2] };
            fastMode = layer.fastMode.value;
            dirtIntensity = layer.dirtIntensity.value;

            // Save Path
            dirtTexture = dirtTexturePath;
        }
        public void Load(UnityEngine.Rendering.PostProcessing.Bloom layer) {
            layer.enabled.value = enabled;
            layer.intensity.value = intensity;
            layer.threshold.value = threshold;
            layer.softKnee.value = softKnee;
            layer.clamp.value = clamp;
            layer.diffusion.value = diffusion;
            layer.anamorphicRatio.value = anamorphicRatio;
            layer.color.value = new Color(color[0], color[1], color[2]);
            layer.fastMode.value = fastMode;
            layer.dirtIntensity.value = dirtIntensity;

            // Load from path.
            // layer.dirtTexture.value = dirtTexture
        }
    }

    [MessagePackObject(keyAsPropertyName: true)]
    public class ChromaticAberration {
        public bool enabled;
        public string spectralLut;
        public float intensity;
        public bool fastMode;
        public void Save(UnityEngine.Rendering.PostProcessing.ChromaticAberration layer, string spectralLutPath = "") {
            enabled = layer.enabled.value;
            intensity = layer.intensity.value;
            fastMode = layer.fastMode.value;

            //Save Texture path.
            //spectralLut = spectralLutPath;
        }
        public void Load(UnityEngine.Rendering.PostProcessing.ChromaticAberration layer) {
            layer.enabled.value = enabled;
            layer.intensity.value = intensity;
            layer.fastMode.value = fastMode;

            //Load texture from the path.
            //layer.spectralLutPath.value = spectralLut;
        }
    }

    [MessagePackObject(keyAsPropertyName: true)]
    public class ColorGrading {
        public bool enabled;
        //internal SplineParameter redCurve; // Figure out messagepack parser later.
        //internal SplineParameter greenCurve; // Figure out messagepack parser later.
        //internal SplineParameter blueCurve; // Figure out messagepack parser later.
        //internal SplineParameter hueVsHueCurve; // Figure out messagepack parser later.
        //internal SplineParameter hueVsSatCurve; // Figure out messagepack parser later.
        //internal SplineParameter satVsSatCurve; // Figure out messagepack parser later.
        //internal SplineParameter lumVsSatCurve; // Figure out messagepack parser later.
        //internal SplineParameter masterCurve; // Figure out messagepack parser later.
        public GradingMode gradingMode;
        public float mixerGreenOutGreenIn;
        public float mixerGreenOutBlueIn;
        public float mixerBlueOutRedIn;
        public float mixerBlueOutGreenIn;
        public float mixerBlueOutBlueIn;
        public float[] lift;
        public float[] gamma;
        public float mixerGreenOutRedIn;
        public float[] gain;
        public float mixerRedOutBlueIn;
        public float mixerRedOutGreenIn;
        public string ldrLutPath; // Formerly Texture
        public float toneCurveToeStrength;
        public float toneCurveToeLength;
        public float toneCurveShoulderStrength;
        public float toneCurveShoulderLength;
        public float toneCurveShoulderAngle;
        public float toneCurveGamma;
        public float mixerRedOutRedIn;
        public Tonemapper tonemapper;
        public float ldrLutContribution;
        public float tint;
        public float[] colorFilter;
        public float hueShift;
        public float saturation;
        public float brightness;
        public float postExposure;
        public float contrast;
        public float temperature;
        public string externalLutPath; // Formerly Texture.

        public void Save(UnityEngine.Rendering.PostProcessing.ColorGrading layer, string lutPath = "", string extLutPath = "") {
            enabled = layer.enabled.value;
            gradingMode = layer.gradingMode.value;
            mixerGreenOutGreenIn = layer.mixerGreenOutGreenIn.value;
            mixerGreenOutBlueIn = layer.mixerGreenOutBlueIn.value;
            mixerBlueOutRedIn = layer.mixerBlueOutRedIn.value;
            mixerBlueOutGreenIn = layer.mixerBlueOutGreenIn.value;
            mixerBlueOutBlueIn = layer.mixerBlueOutBlueIn.value;
            lift = new float[4] { layer.lift.value[0], layer.lift.value[1], layer.lift.value[2], layer.lift.value[3] };
            gamma = new float[4] { layer.gamma.value[0], layer.gamma.value[1], layer.gamma.value[2], layer.gamma.value[3] };
            mixerGreenOutRedIn = layer.mixerGreenOutRedIn.value;
            gain = new float[4] { layer.gain.value[0], layer.gain.value[1], layer.gain.value[2], layer.gain.value[3] };
            mixerRedOutBlueIn = layer.mixerRedOutBlueIn.value;
            mixerRedOutGreenIn = layer.mixerRedOutGreenIn.value;
            toneCurveToeStrength = layer.toneCurveToeStrength.value;
            toneCurveToeLength = layer.toneCurveToeLength.value;
            toneCurveShoulderStrength = layer.toneCurveShoulderStrength.value;
            toneCurveShoulderLength = layer.toneCurveShoulderLength.value;
            toneCurveShoulderAngle = layer.toneCurveShoulderAngle.value;
            toneCurveGamma = layer.toneCurveGamma.value;
            mixerRedOutRedIn = layer.mixerRedOutRedIn.value;
            tonemapper = layer.tonemapper.value;
            ldrLutContribution = layer.ldrLutContribution.value;
            tint = layer.tint.value;
            colorFilter = new float[4] { layer.colorFilter.value[0], layer.colorFilter.value[1], layer.colorFilter.value[2], layer.colorFilter.value[3] };
            hueShift = layer.hueShift.value;
            saturation = layer.saturation.value;
            brightness = layer.brightness.value;
            postExposure = layer.postExposure.value;
            contrast = layer.contrast.value;
            temperature = layer.temperature.value;

            //ldrLutPath = lutPath; // Formerly Texture
            //externalLutPath = extLutPath; // Formerly Texture.

        }
        public void Load(UnityEngine.Rendering.PostProcessing.ColorGrading layer) {
            layer.enabled.value = enabled;
            layer.gradingMode.value = gradingMode;
            layer.mixerGreenOutGreenIn.value = mixerGreenOutGreenIn;
            layer.mixerGreenOutBlueIn.value = mixerGreenOutBlueIn;
            layer.mixerBlueOutRedIn.value = mixerBlueOutRedIn;
            layer.mixerBlueOutGreenIn.value = mixerBlueOutGreenIn;
            layer.mixerBlueOutBlueIn.value = mixerBlueOutBlueIn;
            layer.lift.value = new Vector4(lift[0], lift[1], lift[2], lift[3]);
            layer.gamma.value = new Vector4(gamma[0], gamma[1], gamma[2], gamma[3]);
            layer.mixerGreenOutRedIn.value = mixerGreenOutRedIn;
            layer.gain.value = new Vector4(gain[0], gain[1], gain[2], gain[3]);
            layer.mixerRedOutBlueIn.value = mixerRedOutBlueIn;
            layer.mixerRedOutGreenIn.value = mixerRedOutGreenIn;
            layer.toneCurveToeStrength.value = toneCurveToeStrength;
            layer.toneCurveToeLength.value = toneCurveToeLength;
            layer.toneCurveShoulderStrength.value = toneCurveShoulderStrength;
            layer.toneCurveShoulderLength.value = toneCurveShoulderLength;
            layer.toneCurveShoulderAngle.value = toneCurveShoulderAngle;
            layer.toneCurveGamma.value = toneCurveGamma;
            layer.mixerRedOutRedIn.value = mixerRedOutRedIn;
            layer.tonemapper.value = tonemapper;
            layer.ldrLutContribution.value = ldrLutContribution;
            layer.tint.value = tint;
            layer.colorFilter.value = new Vector4(colorFilter[0], colorFilter[1], colorFilter[2], colorFilter[3]);
            layer.hueShift.value = hueShift;
            layer.saturation.value = saturation;
            layer.brightness.value = brightness;
            layer.postExposure.value = postExposure;
            layer.contrast.value = contrast;
            layer.temperature.value = temperature;

            // Load from certain directory.
            //layer.ldrLutPath.value = ldrLutPath;
            //layer.externalLutPath.value = externalLutPath;
        }
    }

    [MessagePackObject(keyAsPropertyName: true)]
    public class DepthOfField {
        public bool enabled;
        public float focusDistance;
        public float aperture;
        public float focalLength;
        public KernelSize kernelSize;

        public void Save(UnityEngine.Rendering.PostProcessing.DepthOfField layer) {
            enabled = layer.enabled.value;
            focusDistance = layer.focusDistance.value;
            aperture = layer.aperture.value;
            focalLength = layer.focalLength.value;
            kernelSize = layer.kernelSize.value; // KernelSize
        }
        public void Load(UnityEngine.Rendering.PostProcessing.DepthOfField layer) {
            layer.enabled.value = enabled;
            layer.focusDistance.value = focusDistance;
            layer.aperture.value = aperture;
            layer.focalLength.value = focalLength;
            layer.kernelSize.value = kernelSize;
        }
    }

    [MessagePackObject(keyAsPropertyName: true)]
    public class GrainLayer {
        public bool enabled;
        public bool colored;
        public float intensity;
        public float size;
        public float lumContrib;

        public void Save(UnityEngine.Rendering.PostProcessing.Grain layer) {
            enabled = layer.enabled.value;
            colored = layer.colored.value;
            intensity = layer.intensity.value;
            size = layer.size.value;
            lumContrib = layer.lumContrib.value;
        }
        public void Load(UnityEngine.Rendering.PostProcessing.Grain layer) {
            layer.enabled.value = enabled;
            layer.colored.value = colored;
            layer.intensity.value = intensity;
            layer.size.value = size;
            layer.lumContrib.value = lumContrib;
        }
    }

    [MessagePackObject(keyAsPropertyName: true)]
    public class ScreenSpaceReflection {
        public bool enabled;
        public ScreenSpaceReflectionPreset preset;
        public int maximumIterationCount;
        public ScreenSpaceReflectionResolution resolution;
        public float thickness;
        public float maximumMarchDistance;
        public float distanceFade;
        public float vignette;

        public void Save(UnityEngine.Rendering.PostProcessing.ScreenSpaceReflections layer) {
            enabled = layer.enabled.value;
            preset = layer.preset.value;
            maximumIterationCount = layer.maximumIterationCount.value;
            resolution = layer.resolution.value;
            thickness = layer.thickness.value;
            maximumMarchDistance = layer.maximumMarchDistance.value;
            distanceFade = layer.distanceFade.value;
            vignette = layer.vignette.value;
        }
        public void Load(UnityEngine.Rendering.PostProcessing.ScreenSpaceReflections layer) {
            layer.enabled.value = enabled;
            layer.preset.value = preset;
            layer.maximumIterationCount.value = maximumIterationCount;
            layer.resolution.value = resolution;
            layer.thickness.value = thickness;
            layer.maximumMarchDistance.value = maximumMarchDistance;
            layer.distanceFade.value = distanceFade;
            layer.vignette.value = vignette;
        }
    }

    [MessagePackObject(keyAsPropertyName: true)]
    public class Vignette {
        public bool enabled;
        public VignetteMode mode;
        public float[] color; //vector3
        public float[] center; //vector2
        public float intensity;
        public float smoothness;
        public float roundness;
        public bool rounded;
        public string mask; //Mask Texture
        public float opacity;

        public void Save(UnityEngine.Rendering.PostProcessing.Vignette layer, string maskPath = "") {
            enabled = layer.enabled.value;
            mode = layer.mode.value;
            color = new float[3] { layer.color.value[0], layer.color.value[1], layer.color.value[2] };
            center = new float[2] { layer.center.value[0], layer.center.value[1] };
            intensity = layer.intensity.value;
            smoothness = layer.smoothness.value;
            roundness = layer.roundness.value;
            rounded = layer.rounded.value;
            opacity = layer.opacity.value;

            //Save path from the post process object?
            //mask = maskPath;
        }
        public void Load(UnityEngine.Rendering.PostProcessing.Vignette layer) {
            layer.enabled.value = enabled;
            layer.mode.value = mode;
            layer.color.value = new Color(color[0], color[1], color[2]);
            layer.center.value = new Vector2(center[0], center[1]);
            layer.intensity.value = intensity;
            layer.smoothness.value = smoothness;
            layer.roundness.value = roundness;
            layer.rounded.value = rounded;
            layer.opacity.value = opacity;

            // Load Texture from the Path.
            // layer.mask.value = mask;
        }
    }
}
