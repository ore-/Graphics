using Graphics.Settings;
using System;
using UnityEngine;
using UnityEngine.Rendering.PostProcessing;
using static Graphics.Inspector.Util;

namespace Graphics.Inspector
{
    internal static class PostProcessingInspector
    {
        private static Vector2 postScrollView;
        private static FocusPuller focusPuller;
        private static bool _autoFocusEnabled;

        internal static bool AutoFocusEnabled
        {
            get => null == focusPuller ? false : focusPuller.enabled;
            set
            {
                _autoFocusEnabled = value;
                if (null != focusPuller)
                    focusPuller.enabled = true;
            }
        }

        internal static void Draw(PostProcessingSettings postProcessingSettings, PostProcessingManager postprocessingManager, bool showAdvanced)
        {
            GUILayout.BeginVertical(GUIStyles.Skin.box);
            {
                Label("Post Process Layer", "", true);
                GUILayout.Space(1);
                if (showAdvanced)
                {
                    Label("Volume blending", "", true);
                    GUILayout.Space(1);
                    string trigger = null != postProcessingSettings && null != postProcessingSettings.VolumeTriggerSetting ? postProcessingSettings.VolumeTriggerSetting.name : "";
                    Label("Trigger", trigger);
                    Label("Layer", LayerMask.LayerToName(Mathf.RoundToInt(Mathf.Log(postProcessingSettings.VolumeLayerSetting.value, 2))));
                    GUILayout.Space(10);
                }
                Label("Anti-aliasing", "", true);
                Selection("Mode", postProcessingSettings.AntialiasingMode, mode => postProcessingSettings.AntialiasingMode = mode);
                if (PostProcessingSettings.Antialiasing.SMAA == postProcessingSettings.AntialiasingMode)
                {
                    Selection("SMAA Quality", postProcessingSettings.SMAAQuality, quality => postProcessingSettings.SMAAQuality = quality);
                }
                else if (PostProcessingSettings.Antialiasing.TAA == postProcessingSettings.AntialiasingMode)
                {
                    Slider("Jitter Spread", postProcessingSettings.JitterSpread, 0.1f, 1f, "N2", spread => { postProcessingSettings.JitterSpread = spread; });
                    Slider("Stationary Blending", postProcessingSettings.StationaryBlending, 0f, 1f, "N2", sblending => { postProcessingSettings.StationaryBlending = sblending; });
                    Slider("Motion Blending", postProcessingSettings.MotionBlending, 0f, 1f, "N2", mblending => { postProcessingSettings.MotionBlending = mblending; });
                    Slider("Sharpness", postProcessingSettings.Sharpness, 0f, 3f, "N2", sharpness => { postProcessingSettings.Sharpness = sharpness; });
                }
                else if (PostProcessingSettings.Antialiasing.FXAA == postProcessingSettings.AntialiasingMode)
                {
                    Toggle("Fast Mode", postProcessingSettings.FXAAMode, false, fxaa => postProcessingSettings.FXAAMode = fxaa);
                    Toggle("Keep Alpha", postProcessingSettings.FXAAAlpha, false, alpha => postProcessingSettings.FXAAAlpha = alpha);
                }
            }

            GUILayout.EndVertical();
            GUILayout.BeginVertical(GUIStyles.Skin.box);
            {
                string volumeLabel = "Post Process Volume";
                if (showAdvanced)
                {
                    volumeLabel = "Post Process Volumes";
                }
                Label(volumeLabel, "", true);
                postScrollView = GUILayout.BeginScrollView(postScrollView);
                PostProcessVolumeSettings(postProcessingSettings, postprocessingManager, showAdvanced);
                GUILayout.EndScrollView();
            }

            GUILayout.EndVertical();
        }

        private static void PostProcessVolumeSettings(PostProcessingSettings settings, PostProcessingManager postprocessingManager, bool showAdvanced)
        {
            PostProcessVolume volume = settings.Volume;
            GUILayout.Space(10);
            Slider("Weight", volume.weight, 0f, 1f, "N1", weight => volume.weight = weight);
            GUILayout.Space(10);
            if (settings.ambientOcclusionLayer != null)
            {
                GUILayout.BeginVertical(GUIStyles.Skin.box);

                Toggle("Ambient Occlusion", settings.ambientOcclusionLayer.enabled.value, true, enabled => settings.ambientOcclusionLayer.active = settings.ambientOcclusionLayer.enabled.value = enabled);
                if (settings.ambientOcclusionLayer.enabled.value)
                {
#if AI
                    if (!showAdvanced && null != settings.AmplifyOcclusionComponent) settings.AmplifyOcclusionComponent.enabled = false;
#endif
                    Selection("Mode", settings.ambientOcclusionLayer.mode.value, mode => settings.ambientOcclusionLayer.mode.value = mode);
                    Slider("Intensity", settings.ambientOcclusionLayer.intensity.value, 0f, 4f, "N2",
                        intensity => settings.ambientOcclusionLayer.intensity.value = intensity, settings.ambientOcclusionLayer.intensity.overrideState,
                        overrideState => settings.ambientOcclusionLayer.intensity.overrideState = overrideState);

                    if (AmbientOcclusionMode.MultiScaleVolumetricObscurance == settings.ambientOcclusionLayer.mode.value)
                    {
                        Slider("Thickness Modifier", settings.ambientOcclusionLayer.thicknessModifier.value, 1f, 10f, "N2",
                            thickness => settings.ambientOcclusionLayer.thicknessModifier.value = thickness, settings.ambientOcclusionLayer.thicknessModifier.overrideState,
                            overrideState => settings.ambientOcclusionLayer.thicknessModifier.overrideState = overrideState);
                    }
                    else if (AmbientOcclusionMode.ScalableAmbientObscurance == settings.ambientOcclusionLayer.mode.value)
                    {
                        Slider("Radius", settings.ambientOcclusionLayer.radius.value, 1f, 10f, "N2",
                            radius => settings.ambientOcclusionLayer.radius.value = radius, settings.ambientOcclusionLayer.radius.overrideState,
                            overrideState => settings.ambientOcclusionLayer.radius.overrideState = overrideState);
                    }

                    SliderColor("Colour", settings.ambientOcclusionLayer.color.value,
                        colour => settings.ambientOcclusionLayer.color.value = colour, false, settings.ambientOcclusionLayer.color.overrideState,
                        overrideState => settings.ambientOcclusionLayer.color.overrideState = overrideState);
                    Toggle("Ambient Only", settings.ambientOcclusionLayer.ambientOnly.value, false, ambient => settings.ambientOcclusionLayer.ambientOnly.value = ambient);
                }
                GUILayout.EndVertical();
            }
#if AI
            if (settings.AmplifyOcclusionComponent != null)
            {
                GUILayout.Space(1);
                GUILayout.BeginVertical(GUIStyles.Skin.box);
                Toggle("Amplify Ambient Occlusion", settings.AmplifyOcclusionComponent.enabled, true, enabled => settings.AmplifyOcclusionComponent.enabled = enabled);
                if (settings.AmplifyOcclusionComponent.enabled)
                {
                    if (!showAdvanced && null != settings.ambientOcclusionLayer) settings.ambientOcclusionLayer.enabled.value = false;
                    Toggle("Cache Aware", settings.AmplifyOcclusionComponent.CacheAware, false, aware => settings.AmplifyOcclusionComponent.CacheAware = aware);
                    Toggle("Downsample", settings.AmplifyOcclusionComponent.Downsample, false, sample => settings.AmplifyOcclusionComponent.Downsample = sample);

                    Selection("Apply Method", settings.AmplifyOcclusionComponent.ApplyMethod, mode => settings.AmplifyOcclusionComponent.ApplyMethod = mode);
                    Selection("PerPixel Normals", settings.AmplifyOcclusionComponent.PerPixelNormals, mode => settings.AmplifyOcclusionComponent.PerPixelNormals = mode);
                    Selection("Sample Count", settings.AmplifyOcclusionComponent.SampleCount, mode => settings.AmplifyOcclusionComponent.SampleCount = mode);

                    Slider("Bias", settings.AmplifyOcclusionComponent.Bias, 0f, 0.99f, "N2", bias => settings.AmplifyOcclusionComponent.Bias = bias);
                    Slider("Intensity", settings.AmplifyOcclusionComponent.Intensity, 0f, 4f, "N2", intensity => settings.AmplifyOcclusionComponent.Intensity = intensity);
                    Slider("Power Exponent", settings.AmplifyOcclusionComponent.PowerExponent, 0f, 16f, "N2", powerExponent => settings.AmplifyOcclusionComponent.PowerExponent = powerExponent);
                    Slider("Radius", settings.AmplifyOcclusionComponent.Radius, 0f, 32f, "N2", radius => settings.AmplifyOcclusionComponent.Radius = radius);
                    Slider("Thickness", settings.AmplifyOcclusionComponent.Thickness, 0f, 1f, "N2", thickness => settings.AmplifyOcclusionComponent.Thickness = thickness);
                    SliderColor("Tint", settings.AmplifyOcclusionComponent.Tint, colour => settings.AmplifyOcclusionComponent.Tint = colour);

                    Toggle("Blur Enabled", settings.AmplifyOcclusionComponent.BlurEnabled, false, enabled => settings.AmplifyOcclusionComponent.BlurEnabled = enabled);
                    if (settings.AmplifyOcclusionComponent.BlurEnabled)
                    {
                        Slider("Blur Sharpness", settings.AmplifyOcclusionComponent.BlurSharpness, 0f, 20f, "N2", blurSharpness => settings.AmplifyOcclusionComponent.BlurSharpness = blurSharpness);
                        Slider("Blur Passes", settings.AmplifyOcclusionComponent.BlurPasses, 1, 4, blurPasses => settings.AmplifyOcclusionComponent.BlurPasses = blurPasses);
                        Slider("Blur Radius", settings.AmplifyOcclusionComponent.BlurRadius, 1, 4, blurRadius => settings.AmplifyOcclusionComponent.BlurRadius = blurRadius);
                    }

                    Toggle("Filter Enabled", settings.AmplifyOcclusionComponent.FilterEnabled, false, enabled => settings.AmplifyOcclusionComponent.FilterEnabled = enabled);
                    if (settings.amplifyOcclusionComponent.FilterEnabled)
                    {
                        Slider("Filter Blending", settings.AmplifyOcclusionComponent.FilterBlending, 0f, 1f, "N2", filterBlending => settings.AmplifyOcclusionComponent.FilterBlending = filterBlending);
                        Slider("Filter Response", settings.AmplifyOcclusionComponent.FilterResponse, 0f, 1f, "N2", filterResponse => settings.AmplifyOcclusionComponent.FilterResponse = filterResponse);
                    }

                    Toggle("Fade Enabled", settings.AmplifyOcclusionComponent.FadeEnabled, false, enabled => settings.AmplifyOcclusionComponent.FadeEnabled = enabled);
                    if (settings.AmplifyOcclusionComponent.FadeEnabled)
                    {
                        Slider("Fade Length", settings.AmplifyOcclusionComponent.FadeLength, 0f, 100f, "N2", fadeLength => settings.AmplifyOcclusionComponent.FadeLength = fadeLength);
                        Slider("Fade Start", settings.AmplifyOcclusionComponent.FadeStart, 0f, 100f, "N2", fadeStart => settings.AmplifyOcclusionComponent.FadeStart = fadeStart);
                        Slider("Fade To Intensity", settings.AmplifyOcclusionComponent.FadeToIntensity, 0f, 1f, "N2", fadeToIntensity => settings.AmplifyOcclusionComponent.FadeToIntensity = fadeToIntensity);
                        Slider("Fade To Power Exponent", settings.AmplifyOcclusionComponent.FadeToPowerExponent, 0f, 16f, "N2", fadeToPowerExponent => settings.AmplifyOcclusionComponent.FadeToPowerExponent = fadeToPowerExponent);
                        Slider("Fade To Radius", settings.AmplifyOcclusionComponent.FadeToRadius, 0f, 32f, "N2", fadeToRadius => settings.AmplifyOcclusionComponent.FadeToRadius = fadeToRadius);
                        Slider("Fade To Thickness", settings.AmplifyOcclusionComponent.FadeToThickness, 0f, 1f, "N2", fadeToThickness => settings.AmplifyOcclusionComponent.FadeToThickness = fadeToThickness);
                        SliderColor("Fade To Tint", settings.AmplifyOcclusionComponent.FadeToTint, colour => settings.AmplifyOcclusionComponent.FadeToTint = colour);
                    }
                }

                GUILayout.EndVertical();
            }
#endif
            if (settings.autoExposureLayer != null)
            {
                GUILayout.Space(1);
                GUILayout.BeginVertical(GUIStyles.Skin.box);
                Toggle("Auto Exposure", settings.autoExposureLayer.enabled.value, true, enabled => settings.autoExposureLayer.active = settings.autoExposureLayer.enabled.value = enabled);
                if (settings.autoExposureLayer.enabled.value)
                {
                    Toggle("Histogram Filtering (%)", settings.autoExposureLayer.filtering.overrideState, false, overrideState => settings.autoExposureLayer.filtering.overrideState = overrideState);
                    Vector2 filteringRange = settings.autoExposureLayer.filtering.value;
                    Slider("Lower Bound", filteringRange.x, 1f, Math.Min(filteringRange.y, 99f), "N0", filtering => filteringRange.x = filtering, settings.autoExposureLayer.filtering.overrideState);
                    Slider("Upper Bound", filteringRange.y, Math.Max(filteringRange.x, 1f), 99f, "N0", filtering => filteringRange.y = filtering, settings.autoExposureLayer.filtering.overrideState);
                    settings.autoExposureLayer.filtering.value = filteringRange;
                    Slider("Min Luminance (EV)", settings.autoExposureLayer.minLuminance.value, -9f, 9f, "N0",
                        luminance => settings.autoExposureLayer.minLuminance.value = luminance, settings.autoExposureLayer.minLuminance.overrideState,
                        overrideState => settings.autoExposureLayer.minLuminance.overrideState = overrideState);
                    Slider("Max Luminance (EV)", settings.autoExposureLayer.maxLuminance.value, -9f, 9f, "N0",
                        luminance => settings.autoExposureLayer.maxLuminance.value = luminance, settings.autoExposureLayer.maxLuminance.overrideState,
                        overrideState => settings.autoExposureLayer.maxLuminance.overrideState = overrideState);
                    GUILayout.Space(5);
                    Toggle("Eye Adaptation", settings.autoExposureLayer.eyeAdaptation.overrideState, false, overrideState => settings.autoExposureLayer.eyeAdaptation.overrideState = overrideState);
                    Selection("Type", settings.autoExposureLayer.eyeAdaptation.value, type => settings.autoExposureLayer.eyeAdaptation.value = type, -1,
                        settings.autoExposureLayer.eyeAdaptation.overrideState);
                    Slider("Speed from light to dark", settings.autoExposureLayer.speedUp.value, 0f, 10f, "N1",
                        luminance => settings.autoExposureLayer.speedUp.value = luminance, settings.autoExposureLayer.speedUp.overrideState,
                        overrideState => settings.autoExposureLayer.speedUp.overrideState = overrideState);
                    Slider("Speed from dark to light", settings.autoExposureLayer.speedDown.value, 0f, 10f, "N1",
                        luminance => settings.autoExposureLayer.speedDown.value = luminance, settings.autoExposureLayer.speedDown.overrideState,
                        overrideState => settings.autoExposureLayer.speedDown.overrideState = overrideState);
                }
                GUILayout.EndVertical();
            }

            if (settings.bloomLayer != null)
            {
                GUILayout.Space(1);
                GUILayout.BeginVertical(GUIStyles.Skin.box);
                Toggle("Bloom", settings.bloomLayer.enabled.value, true, enabled => settings.bloomLayer.active = settings.bloomLayer.enabled.value = enabled);
                if (settings.bloomLayer.enabled.value)
                {
                    Slider("Intensity", settings.bloomLayer.intensity.value, 0f, 10f, "N1", intensity => settings.bloomLayer.intensity.value = intensity,
                        settings.bloomLayer.intensity.overrideState, overrideState => settings.bloomLayer.intensity.overrideState = overrideState);
                    Slider("Threshold", settings.bloomLayer.threshold.value, 0f, 10f, "N1", threshold => settings.bloomLayer.threshold.value = threshold,
                        settings.bloomLayer.threshold.overrideState, overrideState => settings.bloomLayer.threshold.overrideState = overrideState);
                    Slider("SoftKnee", settings.bloomLayer.softKnee.value, 0f, 1f, "N1", softKnee => settings.bloomLayer.softKnee.value = softKnee,
                        settings.bloomLayer.softKnee.overrideState, overrideState => settings.bloomLayer.softKnee.overrideState = overrideState);
                    Slider("Clamp", settings.bloomLayer.clamp.value, 0, 65472, "N0", clamp => settings.bloomLayer.clamp.value = clamp,
                        settings.bloomLayer.clamp.overrideState, overrideState => settings.bloomLayer.clamp.overrideState = overrideState);
                    Slider("Diffusion", (int)settings.bloomLayer.diffusion.value, 1, 10, "N0", diffusion => settings.bloomLayer.diffusion.value = diffusion,
                        settings.bloomLayer.diffusion.overrideState, overrideState => settings.bloomLayer.diffusion.overrideState = overrideState);
                    Slider("AnamorphicRatio", settings.bloomLayer.anamorphicRatio.value, -1, 1, "N1", anamorphicRatio => settings.bloomLayer.anamorphicRatio.value = anamorphicRatio,
                        settings.bloomLayer.anamorphicRatio.overrideState, overrideState => settings.bloomLayer.anamorphicRatio.overrideState = overrideState);
                    SliderColor("Colour", settings.bloomLayer.color.value, colour => { settings.bloomLayer.color.value = colour; }, settings.bloomLayer.color.overrideState,
                        settings.bloomLayer.color.overrideState, overrideState => settings.bloomLayer.color.overrideState = overrideState);
                    Toggle("Fast Mode", settings.bloomLayer.fastMode.value, false, fastMode => settings.bloomLayer.fastMode.value = fastMode);
                    int lensDirtIndex = SelectionTexture("Lens Dirt", postprocessingManager.CurrentLensDirtTextureIndex, postprocessingManager.LensDirtPreviews, Inspector.Width / 100,
                        settings.bloomLayer.dirtTexture.overrideState, overrideState => settings.bloomLayer.dirtTexture.overrideState = overrideState, GUIStyles.Skin.box);
                    if (-1 != lensDirtIndex && lensDirtIndex != postprocessingManager.CurrentLensDirtTextureIndex)
                    {
                        postprocessingManager.LoadLensDirtTexture(lensDirtIndex, dirtTexture => settings.bloomLayer.dirtTexture.value = dirtTexture);
                    }
                    Text("Dirt Intensity", settings.bloomLayer.dirtIntensity.value, "N2", value => settings.bloomLayer.dirtIntensity.value = value,
                        settings.bloomLayer.dirtIntensity.overrideState, overrideState => settings.bloomLayer.dirtIntensity.overrideState = overrideState);
                }
                GUILayout.EndVertical();
            }

            if (settings.chromaticAberrationLayer)
            {
                GUILayout.Space(1);
                GUILayout.BeginVertical(GUIStyles.Skin.box);
                Toggle("Chromatic Aberration", settings.chromaticAberrationLayer.enabled.value, true, enabled => settings.chromaticAberrationLayer.active = settings.chromaticAberrationLayer.enabled.value = enabled);
                if (settings.chromaticAberrationLayer.enabled.value)
                {
                    Slider("Intensity", settings.chromaticAberrationLayer.intensity.value, 0f, 5f, "N3", intensity => settings.chromaticAberrationLayer.intensity.value = intensity,
                        settings.chromaticAberrationLayer.intensity.overrideState, overrideState => settings.chromaticAberrationLayer.intensity.overrideState = overrideState);
                    Toggle("Fast Mode", settings.chromaticAberrationLayer.fastMode.value, false, fastMode => settings.chromaticAberrationLayer.fastMode.value = fastMode);
                }
                GUILayout.EndVertical();
            }

            if (settings.colorGradingLayer)
            {
                GUILayout.Space(1);
                GUILayout.BeginVertical(GUIStyles.Skin.box);
                Toggle("Colour Grading", settings.colorGradingLayer.enabled.value, true, enabled => settings.colorGradingLayer.active = settings.colorGradingLayer.enabled.value = enabled);
                if (settings.colorGradingLayer.enabled.value)
                {
                    Selection("Mode", (PostProcessingSettings.GradingMode)settings.colorGradingLayer.gradingMode.value, mode => settings.colorGradingLayer.gradingMode.value = (UnityEngine.Rendering.PostProcessing.GradingMode)mode);
                    if (GradingMode.External != settings.colorGradingLayer.gradingMode.value)
                    {
                        if (GradingMode.LowDefinitionRange == settings.colorGradingLayer.gradingMode.value)
                        {
                            Selection("LUT", postprocessingManager.CurrentLUTName, postprocessingManager.LUTNames,
                                lut => { if (lut != postprocessingManager.CurrentLUTName) { settings.colorGradingLayer.ldrLut.value = postprocessingManager.LoadLUT(lut); } }, Inspector.Width / 150,
                                settings.colorGradingLayer.ldrLut.overrideState, overrideState => settings.colorGradingLayer.ldrLut.overrideState = overrideState);
                            Slider("LUT Blend", settings.colorGradingLayer.ldrLutContribution.value, 0, 1, "N3", ldrLutContribution => settings.colorGradingLayer.ldrLutContribution.value = ldrLutContribution,
                                settings.colorGradingLayer.ldrLutContribution.overrideState, overrideState => settings.colorGradingLayer.ldrLutContribution.overrideState = overrideState);
                        }
                        else
                        {
                            Selection("Tonemapping", settings.colorGradingLayer.tonemapper.value, mode => settings.colorGradingLayer.tonemapper.value = mode);
                        }
                        GUILayout.Space(1);
                        Label("White Balance", "");
                        Slider("Temperature", settings.colorGradingLayer.temperature.value, -100, 100, "N1", temperature => settings.colorGradingLayer.temperature.value = temperature,
                            settings.colorGradingLayer.temperature.overrideState, overrideState => settings.colorGradingLayer.temperature.overrideState = overrideState);
                        Slider("Tint", settings.colorGradingLayer.tint.value, -100, 100, "N1", tint => settings.colorGradingLayer.tint.value = tint,
                            settings.colorGradingLayer.tint.overrideState, overrideState => settings.colorGradingLayer.tint.overrideState = overrideState);
                        GUILayout.Space(1);
                        Label("Tone", "");
                        if (GradingMode.HighDefinitionRange == settings.colorGradingLayer.gradingMode.value)
                        {
                            Text("Post-exposure (EV)", settings.colorGradingLayer.postExposure.value, "N2", value => settings.colorGradingLayer.postExposure.value = value,
                                settings.colorGradingLayer.postExposure.overrideState, overrideState => settings.colorGradingLayer.postExposure.overrideState = overrideState);
                        }
                        Slider("Hue Shift", settings.colorGradingLayer.hueShift.value, -180, 180, "N1", hueShift => settings.colorGradingLayer.hueShift.value = hueShift,
                            settings.colorGradingLayer.hueShift.overrideState, overrideState => settings.colorGradingLayer.hueShift.overrideState = overrideState);
                        Slider("Saturation", settings.colorGradingLayer.saturation.value, -100, 100, "N1", saturation => settings.colorGradingLayer.saturation.value = saturation,
                            settings.colorGradingLayer.saturation.overrideState, overrideState => settings.colorGradingLayer.saturation.overrideState = overrideState);
                        if (GradingMode.LowDefinitionRange == settings.colorGradingLayer.gradingMode.value)
                        {
                            Slider("Brightness", settings.colorGradingLayer.brightness.value, -100, 100, "N1", brightness => settings.colorGradingLayer.brightness.value = brightness,
                                settings.colorGradingLayer.brightness.overrideState, overrideState => settings.colorGradingLayer.brightness.overrideState = overrideState);
                        }
                        Slider("Contrast", settings.colorGradingLayer.contrast.value, -100, 100, "N1", contrast => settings.colorGradingLayer.contrast.value = contrast,
                            settings.colorGradingLayer.contrast.overrideState, overrideState => settings.colorGradingLayer.contrast.overrideState = overrideState);
                        SliderColor("Lift", settings.colorGradingLayer.lift.value, colour => settings.colorGradingLayer.lift.value = colour, false,
                            settings.colorGradingLayer.lift.overrideState, overrideState => settings.colorGradingLayer.lift.overrideState = overrideState, "Lift range", -3f, 12f);
                        SliderColor("Gamma", settings.colorGradingLayer.gamma.value, colour => settings.colorGradingLayer.gamma.value = colour, false,
                            settings.colorGradingLayer.gamma.overrideState, overrideSate => settings.colorGradingLayer.gamma.overrideState = overrideSate, "Gamma range", -1.5f, 100f);
                        SliderColor("Gain", settings.colorGradingLayer.gain.value, colour => settings.colorGradingLayer.gain.value = colour, false,
                            settings.colorGradingLayer.gain.overrideState, overrideSate => settings.colorGradingLayer.gain.overrideState = overrideSate, "Gain range", -1.5f, 100f);
                    }
                    else
                    {
                        Label("Not supported at present", "");
                    }
                }
                GUILayout.EndVertical();
            }

            if (settings.depthOfFieldLayer)
            {
                GUILayout.Space(1);
                GUILayout.BeginVertical(GUIStyles.Skin.box);
                Toggle("Depth Of Field", settings.depthOfFieldLayer.enabled.value, true, enabled => settings.depthOfFieldLayer.active = settings.depthOfFieldLayer.enabled.value = enabled);
                if (settings.depthOfFieldLayer.enabled.value)
                {
                    if (null != Graphics.Instance.CameraSettings.MainCamera && null == focusPuller)
                    {
                        focusPuller = Graphics.Instance.CameraSettings.MainCamera.gameObject.GetOrAddComponent<FocusPuller>();
                        if (null != focusPuller)
                            focusPuller.enabled = _autoFocusEnabled;
                    }

                    if (null != focusPuller)
                    {
                        Toggle("Auto Focus", focusPuller.enabled, false, enabled => focusPuller.enabled = enabled);
                        Slider("Auto Focus Speed", focusPuller.Speed, FocusPuller.MinSpeed, FocusPuller.MaxSpeed, "N2", speed => focusPuller.Speed = speed, focusPuller.enabled);
                    }
                    Slider("Focal Distance", settings.depthOfFieldLayer.focusDistance.value, 0.1f, 1000f, "N2", focusDistance => settings.depthOfFieldLayer.focusDistance.value = focusDistance,
                        settings.depthOfFieldLayer.focusDistance.overrideState && !focusPuller.enabled, overrideState => settings.depthOfFieldLayer.focusDistance.overrideState = overrideState);
                    Slider("Aperture", settings.depthOfFieldLayer.aperture.value, 1f, 22f, "N1", aperture => settings.depthOfFieldLayer.aperture.value = aperture,
                        settings.depthOfFieldLayer.aperture.overrideState, overrideState => settings.depthOfFieldLayer.aperture.overrideState = overrideState);
                    Slider("Focal Length", settings.depthOfFieldLayer.focalLength.value, 10f, 600f, "N0", focalLength => settings.depthOfFieldLayer.focalLength.value = focalLength,
                        settings.depthOfFieldLayer.focalLength.overrideState, overrideState => settings.depthOfFieldLayer.focalLength.overrideState = overrideState);
                    Selection("Max Blur Size", settings.depthOfFieldLayer.kernelSize.value, kernelSize => settings.depthOfFieldLayer.kernelSize.value = kernelSize, -1,
                        settings.depthOfFieldLayer.kernelSize.overrideState, overrideState => settings.depthOfFieldLayer.kernelSize.overrideState = overrideState);

                    if (showAdvanced)
                    {
                        SelectionMask("Focus culling mask", focusPuller.HitLayer, mask => focusPuller.HitLayer = mask);
                        GUI.enabled = false;
                        Label("Max Distance", focusPuller.MaxDistance.ToString());
                        Dimension("Target Focus Point", focusPuller.TargetFocusPoint);
                        Dimension("Target Position", focusPuller.TargetPosition);
                        Dimension("Transform Position", focusPuller.TransformPosition);
                        Dimension("Hit Point", focusPuller.HitPoint);
                        Dimension("Ray Origin", focusPuller.RayOrigin);
                        GUI.enabled = true;
                    }
                }
                GUILayout.EndVertical();
            }

            if (settings.grainLayer != null)
            {
                GUILayout.Space(1);
                GUILayout.BeginVertical(GUIStyles.Skin.box);
                Toggle("Grain", settings.grainLayer.enabled.value, true, enabled => settings.grainLayer.active = settings.grainLayer.enabled.value = enabled);
                if (settings.grainLayer.enabled.value)
                {
                    Toggle("Colored", settings.grainLayer.colored.overrideState, false, overrideState => settings.grainLayer.colored.overrideState = overrideState);
                    Slider("Intensity", settings.grainLayer.intensity.value, 0f, 20f, "N2", intensity => settings.grainLayer.intensity.value = intensity,
                        settings.grainLayer.intensity.overrideState, overrideState => settings.grainLayer.intensity.overrideState = overrideState);
                    Slider("Size", settings.grainLayer.size.value, 0f, 10f, "N0", focalLength => settings.grainLayer.size.value = focalLength,
                        settings.grainLayer.size.overrideState, overrideState => settings.grainLayer.size.overrideState = overrideState);
                    Slider("Luminance Contribution", settings.grainLayer.lumContrib.value, 0f, 22f, "N1", lumContrib => settings.grainLayer.lumContrib.value = lumContrib,
                        settings.grainLayer.lumContrib.overrideState, overrideState => settings.grainLayer.lumContrib.overrideState = overrideState);
                }
                GUILayout.EndVertical();
            }

            if (settings.screenSpaceReflectionsLayer != null)
            {
                GUILayout.Space(1);
                GUILayout.BeginVertical(GUIStyles.Skin.box);
                Toggle("Screen Space Reflection", settings.screenSpaceReflectionsLayer.enabled.value, true, enabled => settings.screenSpaceReflectionsLayer.active = settings.screenSpaceReflectionsLayer.enabled.value = enabled);
                if (settings.screenSpaceReflectionsLayer.enabled.value)
                {
                    Selection("Preset", settings.screenSpaceReflectionsLayer.preset.value, preset => settings.screenSpaceReflectionsLayer.preset.value = preset, -1,
                        settings.screenSpaceReflectionsLayer.preset.overrideState, overrideState => settings.screenSpaceReflectionsLayer.preset.overrideState = overrideState);

                    if (ScreenSpaceReflectionPreset.Custom == settings.screenSpaceReflectionsLayer.preset.value)
                    {
                        Slider("Maximum Iteration Count", settings.screenSpaceReflectionsLayer.maximumIterationCount.value, 0, 256, iteration => settings.screenSpaceReflectionsLayer.maximumIterationCount.value = iteration,
                            settings.screenSpaceReflectionsLayer.maximumIterationCount.overrideState, overrideState => settings.screenSpaceReflectionsLayer.maximumIterationCount.overrideState = overrideState);

                        Slider("Thickness", settings.screenSpaceReflectionsLayer.thickness.value, 1f, 64f, "N1", thickness => settings.screenSpaceReflectionsLayer.thickness.value = thickness,
                            settings.screenSpaceReflectionsLayer.thickness.overrideState, overrideState => settings.screenSpaceReflectionsLayer.thickness.overrideState = overrideState);

                        Selection("Resolution", settings.screenSpaceReflectionsLayer.resolution.value, resolution => settings.screenSpaceReflectionsLayer.resolution.value = resolution, -1,
                            settings.screenSpaceReflectionsLayer.resolution.overrideState, overrideState => settings.screenSpaceReflectionsLayer.resolution.overrideState = overrideState);
                    }

                    Text("Maximum March Distance", settings.screenSpaceReflectionsLayer.maximumMarchDistance.value, "N2", value => settings.screenSpaceReflectionsLayer.maximumMarchDistance.value = value,
                        settings.screenSpaceReflectionsLayer.maximumMarchDistance.overrideState, overrideState => settings.screenSpaceReflectionsLayer.maximumMarchDistance.overrideState = overrideState);
                    Slider("Distance Fade", settings.screenSpaceReflectionsLayer.distanceFade, 0f, 1f, "N3", fade => settings.screenSpaceReflectionsLayer.distanceFade.value = fade,
                        settings.screenSpaceReflectionsLayer.distanceFade.overrideState, overrideState => settings.screenSpaceReflectionsLayer.distanceFade.overrideState = overrideState);
                    Slider("Vignette", settings.screenSpaceReflectionsLayer.vignette.value, 0f, 1f, "N3", vignette => settings.screenSpaceReflectionsLayer.vignette.value = vignette,
                        settings.screenSpaceReflectionsLayer.vignette.overrideState, overrideState => settings.screenSpaceReflectionsLayer.vignette.overrideState = overrideState);
                }
                GUILayout.EndVertical();
            }

            if (settings.vignetteLayer != null)
            {
                GUILayout.Space(1);
                GUILayout.BeginVertical(GUIStyles.Skin.box);
                Toggle("Vignette", settings.vignetteLayer.enabled.value, true, enabled => settings.vignetteLayer.active = settings.vignetteLayer.enabled.value = enabled);
                if (settings.vignetteLayer.enabled.value)
                {
                    Selection("Mode", settings.vignetteLayer.mode.value, mode => settings.vignetteLayer.mode.value = mode, -1,
                        settings.vignetteLayer.mode.overrideState, overrideState => settings.vignetteLayer.mode.overrideState = overrideState);
                    SliderColor("Colour", settings.vignetteLayer.color.value, colour => settings.vignetteLayer.color.value = colour, false,
                        settings.vignetteLayer.color.overrideState, overrideState => settings.vignetteLayer.color.overrideState = overrideState);
                    Slider("Intensity", settings.vignetteLayer.intensity, 0f, 1f, "N3", fade => settings.vignetteLayer.intensity.value = fade,
                        settings.vignetteLayer.intensity.overrideState, overrideState => settings.vignetteLayer.intensity.overrideState = overrideState);
                    Slider("Smoothness", settings.vignetteLayer.smoothness.value, 0.01f, 1f, "N3", vignette => settings.vignetteLayer.smoothness.value = vignette,
                        settings.vignetteLayer.smoothness.overrideState, overrideState => settings.vignetteLayer.smoothness.overrideState = overrideState);
                    Slider("Roundness", settings.vignetteLayer.roundness.value, 0f, 1f, "N3", vignette => settings.vignetteLayer.roundness.value = vignette,
                        settings.vignetteLayer.roundness.overrideState, overrideState => settings.vignetteLayer.roundness.overrideState = overrideState);
                    Toggle("Rounded", settings.vignetteLayer.rounded, settings.vignetteLayer.rounded.overrideState, rounded => settings.vignetteLayer.rounded.value = rounded);
                }
                GUILayout.EndVertical();
            }

            if (settings.motionBlurLayer != null)
            {
                GUILayout.Space(1);
                GUILayout.BeginVertical(GUIStyles.Skin.box);
                Toggle("Motion Blur", settings.motionBlurLayer.enabled.value, true, enabled => settings.motionBlurLayer.active = settings.motionBlurLayer.enabled.value = enabled);
                if (settings.motionBlurLayer.enabled.value)
                {
                    Slider("Shutter Angle", settings.motionBlurLayer.shutterAngle.value, 0f, 360f, "N2", intensity => settings.motionBlurLayer.shutterAngle.value = intensity,
                        settings.motionBlurLayer.shutterAngle.overrideState, overrideState => settings.motionBlurLayer.shutterAngle.overrideState = overrideState);
                    Slider("Sample Count", settings.motionBlurLayer.sampleCount.value, 4, 32, intensity => settings.motionBlurLayer.sampleCount.value = intensity,
                        settings.motionBlurLayer.sampleCount.overrideState, overrideState => settings.motionBlurLayer.sampleCount.overrideState = overrideState);
                }
                GUILayout.EndVertical();
            }
        }
    }
}
