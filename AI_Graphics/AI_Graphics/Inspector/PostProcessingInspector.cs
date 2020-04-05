using AIGraphics.Settings;
using System;
using System.Linq;
using UnityEngine;
using UnityEngine.Rendering.PostProcessing;
using static AIGraphics.Inspector.Util;

namespace AIGraphics.Inspector
{
    internal static class PostProcessingInspector
    {
        private static Vector2 postScrollView;
        private static int selectedVolumeIndex = 0;
        private static PostProcessVolume selectedVolume;

        internal static void Draw(PostProcessingSettings postProcessingSettings, PostProcessingManager postprocessingManager, FocusPuller focusPuller, bool showAdvanced)
        {
            GUILayout.BeginVertical(GUIStyles.Skin.box);
            {
                GUILayout.Label("Post Process Layer", GUIStyles.boldlabel);
                GUILayout.Space(1);
                if (showAdvanced)
                {
                    GUILayout.Label("Volume blending", GUIStyles.boldlabel);
                    GUILayout.Space(1);
                    Label("Trigger", postProcessingSettings.VolumeTriggerSetting?.name);
                    Label("Layer", LayerMask.LayerToName(Mathf.RoundToInt(Mathf.Log(postProcessingSettings.VolumeLayerSetting.value, 2))));
                    GUILayout.Space(10);
                }
                GUILayout.Label("Anti-aliasing", GUIStyles.boldlabel);
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
                    postProcessingSettings.FXAAMode = Toggle("Fast Mode", postProcessingSettings.FXAAMode);
                    postProcessingSettings.FXAAAlpha = Toggle("Keep Alpha", postProcessingSettings.FXAAAlpha);
                }
            }
            GUILayout.EndVertical();
            GUILayout.BeginVertical(GUIStyles.Skin.box);
            {
                string volumeLabel = "Post Process Volume";
                if (showAdvanced) volumeLabel = "Post Process Volumes";
                GUILayout.Label(volumeLabel, GUIStyles.boldlabel);
                if (showAdvanced)
                {
                    selectedVolumeIndex = GUILayout.Toolbar(selectedVolumeIndex, postProcessingSettings.volumeNames, GUIStyles.toolbarbutton);
                    selectedVolume = postProcessingSettings.postProcessVolumes[selectedVolumeIndex];
                }
                else
                {
                    PostProcessVolume volumeToDisable;

                    switch (KKAPI.KoikatuAPI.GetCurrentGameMode())
                    {
                        case KKAPI.GameMode.Maker:
                            selectedVolume = postProcessingSettings.postProcessVolumes.First(thisVolume => "PostProcessVolume3D" == thisVolume.name);
                            selectedVolumeIndex = Array.IndexOf(postProcessingSettings.postProcessVolumes, selectedVolume);
                            volumeToDisable = postProcessingSettings.postProcessVolumes.First(thisVolume => "PostProcessVolume2D" == thisVolume.name);
                            volumeToDisable.enabled = false;
                            break;
                        case KKAPI.GameMode.Studio:
                            selectedVolume = postProcessingSettings.postProcessVolumes.First(thisVolume => "PostProcessVolume" == thisVolume.name);
                            selectedVolumeIndex = Array.IndexOf(postProcessingSettings.postProcessVolumes, selectedVolume);
                            volumeToDisable = postProcessingSettings.postProcessVolumes.First(thisVolume => "PostProcessVolumeColor" == thisVolume.name);
                            volumeToDisable.enabled = false;
                            break;
                        default:
                            selectedVolumeIndex = 0;
                            selectedVolume = postProcessingSettings.postProcessVolumes[0];
                            break;
                    }
                }
                postScrollView = GUILayout.BeginScrollView(postScrollView);
                PostProcessVolumeSettings(postProcessingSettings, selectedVolumeIndex, selectedVolume, postprocessingManager, focusPuller, showAdvanced);
                GUILayout.EndScrollView();
            }
            GUILayout.EndVertical();
        }

        private static void PostProcessVolumeSettings(PostProcessingSettings settings, int volumeIndex, PostProcessVolume volume, PostProcessingManager postprocessingManager, FocusPuller focusPuller, bool showAdvanced)
        {
            GUILayout.Space(10);
            Slider("Weight", volume.weight, 0f, 1f, "N1", weight => volume.weight = weight);
            if (showAdvanced)
            {
                Label("Priority", volume.priority.ToString());
                GUILayout.Space(1);
                Label("Profile", volume.profile.name);
            }
            GUILayout.Space(10);
            GUILayout.BeginVertical(GUIStyles.Skin.box);
            settings.ambientOcclusionLayers[volumeIndex].active =
            settings.ambientOcclusionLayers[volumeIndex].enabled.value = Toggle("Ambient Occlusion", settings.ambientOcclusionLayers[volumeIndex].enabled.value, true);
            if (settings.ambientOcclusionLayers[volumeIndex].enabled.value)
            {
                Selection("Mode", settings.ambientOcclusionLayers[volumeIndex].mode.value, mode => settings.ambientOcclusionLayers[volumeIndex].mode.value = mode);
                Slider("Intensity", settings.ambientOcclusionLayers[volumeIndex].intensity.value, 0f, 4f, "N2",
                    intensity => settings.ambientOcclusionLayers[volumeIndex].intensity.value = intensity, settings.ambientOcclusionLayers[volumeIndex].intensity.overrideState,
                    overrideState => settings.ambientOcclusionLayers[volumeIndex].intensity.overrideState = overrideState);

                if (AmbientOcclusionMode.MultiScaleVolumetricObscurance == settings.ambientOcclusionLayers[volumeIndex].mode.value)
                {
                    Slider("Thickness Modifier", settings.ambientOcclusionLayers[volumeIndex].thicknessModifier.value, 1f, 10f, "N2",
                        thickness => settings.ambientOcclusionLayers[volumeIndex].thicknessModifier.value = thickness, settings.ambientOcclusionLayers[volumeIndex].thicknessModifier.overrideState,
                        overrideState => settings.ambientOcclusionLayers[volumeIndex].thicknessModifier.overrideState = overrideState);
                }
                else if (AmbientOcclusionMode.ScalableAmbientObscurance == settings.ambientOcclusionLayers[volumeIndex].mode.value)
                {
                    Slider("Radius", settings.ambientOcclusionLayers[volumeIndex].radius.value, 1f, 10f, "N2",
                        radius => settings.ambientOcclusionLayers[volumeIndex].radius.value = radius, settings.ambientOcclusionLayers[volumeIndex].radius.overrideState,
                        overrideState => settings.ambientOcclusionLayers[volumeIndex].radius.overrideState = overrideState);
                }

                SliderColor("Colour", settings.ambientOcclusionLayers[volumeIndex].color.value,
                    colour => settings.ambientOcclusionLayers[volumeIndex].color.value = colour, false, settings.ambientOcclusionLayers[volumeIndex].color.overrideState,
                    overrideState => settings.ambientOcclusionLayers[volumeIndex].color.overrideState = overrideState);
                settings.ambientOcclusionLayers[volumeIndex].ambientOnly.value = Toggle("Ambient Only", settings.ambientOcclusionLayers[volumeIndex].ambientOnly.value);
            }
            GUILayout.EndVertical();
            GUILayout.Space(1);
            GUILayout.BeginVertical(GUIStyles.Skin.box);
            settings.autoExposureLayers[volumeIndex].active =
            settings.autoExposureLayers[volumeIndex].enabled.value = Toggle("Auto Exposure", settings.autoExposureLayers[volumeIndex].enabled.value, true);
            if (settings.autoExposureLayers[volumeIndex].enabled.value)
            {
                settings.autoExposureLayers[volumeIndex].filtering.overrideState = Toggle("Histogram Filtering (%)", settings.autoExposureLayers[volumeIndex].filtering.overrideState);
                Vector2 filteringRange = settings.autoExposureLayers[volumeIndex].filtering.value;
                Slider("Lower Bound", filteringRange.x, 1f, Math.Min(filteringRange.y, 99f), "N0", filtering => filteringRange.x = filtering, settings.autoExposureLayers[volumeIndex].filtering.overrideState);
                Slider("Upper Bound", filteringRange.y, Math.Max(filteringRange.x, 1f), 99f, "N0", filtering => filteringRange.y = filtering, settings.autoExposureLayers[volumeIndex].filtering.overrideState);
                settings.autoExposureLayers[volumeIndex].filtering.value = filteringRange;
                Slider("Min Luminance (EV)", settings.autoExposureLayers[volumeIndex].minLuminance.value, -9f, 9f, "N0",
                    luminance => settings.autoExposureLayers[volumeIndex].minLuminance.value = luminance, settings.autoExposureLayers[volumeIndex].minLuminance.overrideState,
                    overrideState => settings.autoExposureLayers[volumeIndex].minLuminance.overrideState = overrideState);
                Slider("Max Luminance (EV)", settings.autoExposureLayers[volumeIndex].maxLuminance.value, -9f, 9f, "N0",
                    luminance => settings.autoExposureLayers[volumeIndex].maxLuminance.value = luminance, settings.autoExposureLayers[volumeIndex].maxLuminance.overrideState,
                    overrideState => settings.autoExposureLayers[volumeIndex].maxLuminance.overrideState = overrideState);
                GUILayout.Space(5);
                settings.autoExposureLayers[volumeIndex].eyeAdaptation.overrideState = Toggle("Eye Adaptation", settings.autoExposureLayers[volumeIndex].eyeAdaptation.overrideState);
                Selection("Type", settings.autoExposureLayers[volumeIndex].eyeAdaptation.value, type => settings.autoExposureLayers[volumeIndex].eyeAdaptation.value = type, -1,
                    settings.autoExposureLayers[volumeIndex].eyeAdaptation.overrideState);
                Slider("Speed from light to dark", settings.autoExposureLayers[volumeIndex].speedUp.value, 0f, 10f, "N1",
                    luminance => settings.autoExposureLayers[volumeIndex].speedUp.value = luminance, settings.autoExposureLayers[volumeIndex].speedUp.overrideState,
                    overrideState => settings.autoExposureLayers[volumeIndex].speedUp.overrideState = overrideState);
                Slider("Speed from dark to light", settings.autoExposureLayers[volumeIndex].speedDown.value, 0f, 10f, "N1",
                    luminance => settings.autoExposureLayers[volumeIndex].speedDown.value = luminance, settings.autoExposureLayers[volumeIndex].speedDown.overrideState,
                    overrideState => settings.autoExposureLayers[volumeIndex].speedDown.overrideState = overrideState);
            }
            GUILayout.EndVertical();
            GUILayout.Space(1);
            GUILayout.BeginVertical(GUIStyles.Skin.box);
            settings.bloomLayers[volumeIndex].active =
            settings.bloomLayers[volumeIndex].enabled.value = Toggle("Bloom", settings.bloomLayers[volumeIndex].enabled.value, true);
            if (settings.bloomLayers[volumeIndex].enabled.value)
            {
                Slider("Intensity", settings.bloomLayers[volumeIndex].intensity.value, 0f, 10f, "N1", intensity => settings.bloomLayers[volumeIndex].intensity.value = intensity,
                    settings.bloomLayers[volumeIndex].intensity.overrideState, overrideState => settings.bloomLayers[volumeIndex].intensity.overrideState = overrideState);
                Slider("Threshold", settings.bloomLayers[volumeIndex].threshold.value, 0f, 10f, "N1", threshold => settings.bloomLayers[volumeIndex].threshold.value = threshold,
                    settings.bloomLayers[volumeIndex].threshold.overrideState, overrideState => settings.bloomLayers[volumeIndex].threshold.overrideState = overrideState);
                Slider("SoftKnee", settings.bloomLayers[volumeIndex].softKnee.value, 0f, 1f, "N1", softKnee => settings.bloomLayers[volumeIndex].softKnee.value = softKnee,
                    settings.bloomLayers[volumeIndex].softKnee.overrideState, overrideState => settings.bloomLayers[volumeIndex].softKnee.overrideState = overrideState);
                Slider("Clamp", settings.bloomLayers[volumeIndex].clamp.value, 0, 65472, "N0", clamp => settings.bloomLayers[volumeIndex].clamp.value = clamp,
                    settings.bloomLayers[volumeIndex].clamp.overrideState, overrideState => settings.bloomLayers[volumeIndex].clamp.overrideState = overrideState);
                Slider("Diffusion", (int)settings.bloomLayers[volumeIndex].diffusion.value, 1, 10, "N0", diffusion => settings.bloomLayers[volumeIndex].diffusion.value = diffusion,
                    settings.bloomLayers[volumeIndex].diffusion.overrideState, overrideState => settings.bloomLayers[volumeIndex].diffusion.overrideState = overrideState);
                Slider("AnamorphicRatio", settings.bloomLayers[volumeIndex].anamorphicRatio.value, -1, 1, "N1", anamorphicRatio => settings.bloomLayers[volumeIndex].anamorphicRatio.value = anamorphicRatio,
                    settings.bloomLayers[volumeIndex].anamorphicRatio.overrideState, overrideState => settings.bloomLayers[volumeIndex].anamorphicRatio.overrideState = overrideState);
                SliderColor("Colour", settings.bloomLayers[volumeIndex].color.value, colour => { settings.bloomLayers[volumeIndex].color.value = colour; }, settings.bloomLayers[volumeIndex].color.overrideState,
                    settings.bloomLayers[volumeIndex].color.overrideState, overrideState => settings.bloomLayers[volumeIndex].color.overrideState = overrideState);
                settings.bloomLayers[volumeIndex].fastMode.value = Toggle("Fast Mode", settings.bloomLayers[volumeIndex].fastMode.value);

                PostProcessingManager.currentDirtIndex = SelectionTexture("Lens Dirt", PostProcessingManager.currentDirtIndex, PostProcessingManager.LensDirtPreviews.ToArray(), Inspector.Width / 100,
                    settings.bloomLayers[volumeIndex].dirtTexture.overrideState, overrideState => settings.bloomLayers[volumeIndex].dirtTexture.overrideState = overrideState, GUIStyles.Skin.box);
                if (-1 != PostProcessingManager.currentDirtIndex) {
                    settings.bloomLayers[volumeIndex].dirtTexture.value = PostProcessingManager.DirtTexture;
                }

                settings.bloomLayers[volumeIndex].dirtIntensity.value = Text("Dirt Intensity", settings.bloomLayers[volumeIndex].dirtIntensity.value, "N2",
                    settings.bloomLayers[volumeIndex].dirtIntensity.overrideState, overrideState => settings.bloomLayers[volumeIndex].dirtIntensity.overrideState = overrideState);
            }
            /*
            GUILayout.Space(10);
            settings.chromaticAberrationLayers[volumeIndex].active =
            settings.chromaticAberrationLayers[volumeIndex].enabled.value = Toggle("Chromatic Aberration", settings.chromaticAberrationLayers[volumeIndex].enabled.value, true);
            if (settings.chromaticAberrationLayers[volumeIndex].enabled.value)
            {
                Slider("Intensity", settings.chromaticAberrationLayers[volumeIndex].intensity.value, 0.1f, 20f, "N2", intensity => settings.chromaticAberrationLayers[volumeIndex].intensity.value = intensity, 
                    settings.chromaticAberrationLayers[volumeIndex].intensity.overrideState, overrideState => settings.chromaticAberrationLayers[volumeIndex].intensity.overrideState = overrideState);
            }
            */
            GUILayout.EndVertical();
            GUILayout.Space(1);
            GUILayout.BeginVertical(GUIStyles.Skin.box);
            settings.colorGradingLayers[volumeIndex].active =
            settings.colorGradingLayers[volumeIndex].enabled.value = Toggle("Colour Grading", settings.colorGradingLayers[volumeIndex].enabled.value, true);
            if (settings.colorGradingLayers[volumeIndex].enabled.value)
            {
                Selection("Mode", settings.colorGradingLayers[volumeIndex].gradingMode.value, mode => settings.colorGradingLayers[volumeIndex].gradingMode.value = mode);
                if (GradingMode.External != settings.colorGradingLayers[volumeIndex].gradingMode.value)
                {
                    Selection("Tonemapping", settings.colorGradingLayers[volumeIndex].tonemapper.value, mode => settings.colorGradingLayers[volumeIndex].tonemapper.value = mode);
                    GUILayout.Space(1);
                    GUILayout.Label("White Balance");
                    Slider("Temperature", settings.colorGradingLayers[volumeIndex].temperature.value, -100, 100, "N1", temperature => settings.colorGradingLayers[volumeIndex].temperature.value = temperature,
                        settings.colorGradingLayers[volumeIndex].temperature.overrideState, overrideState => settings.colorGradingLayers[volumeIndex].temperature.overrideState = overrideState);
                    Slider("Tint", settings.colorGradingLayers[volumeIndex].tint.value, -100, 100, "N1", tint => settings.colorGradingLayers[volumeIndex].tint.value = tint,
                        settings.colorGradingLayers[volumeIndex].tint.overrideState, overrideState => settings.colorGradingLayers[volumeIndex].tint.overrideState = overrideState);
                    GUILayout.Space(1);
                    GUILayout.Label("Tone");
                    if (GradingMode.HighDefinitionRange == settings.colorGradingLayers[volumeIndex].gradingMode.value)
                        settings.colorGradingLayers[volumeIndex].postExposure.value = Text("Post-exposure (EV)", settings.colorGradingLayers[volumeIndex].postExposure.value, "N2",
                            settings.colorGradingLayers[volumeIndex].postExposure.overrideState, overrideState => settings.colorGradingLayers[volumeIndex].postExposure.overrideState = overrideState);
                    Slider("Hue Shift", settings.colorGradingLayers[volumeIndex].hueShift.value, -180, 180, "N1", hueShift => settings.colorGradingLayers[volumeIndex].hueShift.value = hueShift,
                        settings.colorGradingLayers[volumeIndex].hueShift.overrideState, overrideState => settings.colorGradingLayers[volumeIndex].hueShift.overrideState = overrideState);
                    Slider("Saturation", settings.colorGradingLayers[volumeIndex].saturation.value, -100, 100, "N1", saturation => settings.colorGradingLayers[volumeIndex].saturation.value = saturation,
                        settings.colorGradingLayers[volumeIndex].saturation.overrideState, overrideState => settings.colorGradingLayers[volumeIndex].saturation.overrideState = overrideState);
                    Slider("Contrast", settings.colorGradingLayers[volumeIndex].contrast.value, -100, 100, "N1", contrast => settings.colorGradingLayers[volumeIndex].contrast.value = contrast,
                        settings.colorGradingLayers[volumeIndex].contrast.overrideState, overrideState => settings.colorGradingLayers[volumeIndex].contrast.overrideState = overrideState);
                    SliderColor("Lift", settings.colorGradingLayers[volumeIndex].lift.value, colour => settings.colorGradingLayers[volumeIndex].lift.value = colour, false,
                        settings.colorGradingLayers[volumeIndex].lift.overrideState, overrideState => settings.colorGradingLayers[volumeIndex].lift.overrideState = overrideState);
                    SliderColor("Gamma", settings.colorGradingLayers[volumeIndex].gamma.value, colour => settings.colorGradingLayers[volumeIndex].gamma.value = colour, false,
                        settings.colorGradingLayers[volumeIndex].gamma.overrideState, overrideSate => settings.colorGradingLayers[volumeIndex].gamma.overrideState = overrideSate);
                    SliderColor("Gain", settings.colorGradingLayers[volumeIndex].gain.value, colour => settings.colorGradingLayers[volumeIndex].gain.value = colour, false,
                        settings.colorGradingLayers[volumeIndex].gain.overrideState, overrideSate => settings.colorGradingLayers[volumeIndex].gain.overrideState = overrideSate);
                }
                else
                {
                    GUILayout.Label("Not supported at present");
                }
            }
            GUILayout.EndVertical();
            GUILayout.Space(1);
            GUILayout.BeginVertical(GUIStyles.Skin.box);
            settings.depthOfFieldLayers[volumeIndex].active =
            settings.depthOfFieldLayers[volumeIndex].enabled.value = Toggle("Depth Of Field", settings.depthOfFieldLayers[volumeIndex].enabled.value, true);
            if (settings.depthOfFieldLayers[volumeIndex].enabled.value)
            {
                focusPuller.enabled = Toggle("Auto Focus", focusPuller.enabled);
                Slider("Focal Distance", settings.depthOfFieldLayers[volumeIndex].focusDistance.value, 0.1f, 20f, "N2", focusDistance => settings.depthOfFieldLayers[volumeIndex].focusDistance.value = focusDistance,
                    settings.depthOfFieldLayers[volumeIndex].focusDistance.overrideState && !focusPuller.enabled, overrideState => settings.depthOfFieldLayers[volumeIndex].focusDistance.overrideState = overrideState);
                Slider("Aperture", settings.depthOfFieldLayers[volumeIndex].aperture.value, 1f, 22f, "N1", aperture => settings.depthOfFieldLayers[volumeIndex].aperture.value = aperture,
                    settings.depthOfFieldLayers[volumeIndex].aperture.overrideState, overrideState => settings.depthOfFieldLayers[volumeIndex].aperture.overrideState = overrideState);
                Slider("Focal Length", settings.depthOfFieldLayers[volumeIndex].focalLength.value, 10f, 600f, "N0", focalLength => settings.depthOfFieldLayers[volumeIndex].focalLength.value = focalLength,
                    settings.depthOfFieldLayers[volumeIndex].focalLength.overrideState, overrideState => settings.depthOfFieldLayers[volumeIndex].focalLength.overrideState = overrideState);
                Selection("Max Blur Size", settings.depthOfFieldLayers[volumeIndex].kernelSize.value, kernelSize => settings.depthOfFieldLayers[volumeIndex].kernelSize.value = kernelSize, -1,
                    settings.depthOfFieldLayers[volumeIndex].kernelSize.overrideState, overrideState => settings.depthOfFieldLayers[volumeIndex].kernelSize.overrideState = overrideState);
            }
            /*
            GUILayout.Space(10);
            settings.grainLayers[volumeIndex].active =
            settings.grainLayers[volumeIndex].enabled.value = Toggle("Grain", settings.grainLayers[volumeIndex].enabled.value, true);
            if (settings.grainLayers[volumeIndex].enabled.value)
            {
                settings.grainLayers[volumeIndex].colored.overrideState = Toggle("Colored", settings.grainLayers[volumeIndex].colored.overrideState);
                Slider("Intensity", settings.grainLayers[volumeIndex].intensity.value, 0f, 20f, "N2", intensity => settings.grainLayers[volumeIndex].intensity.value = intensity, 
                    settings.grainLayers[volumeIndex].intensity.overrideState, overrideState => settings.grainLayers[volumeIndex].intensity.overrideState = overrideState);
                Slider("Size", settings.grainLayers[volumeIndex].size.value, 0f, 10f, "N0", focalLength => settings.grainLayers[volumeIndex].size.value = focalLength, 
                    settings.grainLayers[volumeIndex].size.overrideState, overrideState => settings.grainLayers[volumeIndex].size.overrideState = overrideState);
                Slider("Luminance Contribution", settings.grainLayers[volumeIndex].lumContrib.value, 0f, 22f, "N1", lumContrib => settings.grainLayers[volumeIndex].lumContrib.value = lumContrib, 
                    settings.grainLayers[volumeIndex].lumContrib.overrideState, overrideState => settings.grainLayers[volumeIndex].lumContrib.overrideState = overrideState);
            }
            */
            GUILayout.EndVertical();
            GUILayout.Space(1);
            GUILayout.BeginVertical(GUIStyles.Skin.box);
            settings.screenSpaceReflectionsLayers[volumeIndex].active =
            settings.screenSpaceReflectionsLayers[volumeIndex].enabled.value = Toggle("Screen Space Reflection", settings.screenSpaceReflectionsLayers[volumeIndex].enabled.value, true);
            if (settings.screenSpaceReflectionsLayers[volumeIndex].enabled.value)
            {
                Selection("Preset", settings.screenSpaceReflectionsLayers[volumeIndex].preset.value, preset => settings.screenSpaceReflectionsLayers[volumeIndex].preset.value = preset, -1,
                    settings.screenSpaceReflectionsLayers[volumeIndex].preset.overrideState, overrideState => settings.screenSpaceReflectionsLayers[volumeIndex].preset.overrideState = overrideState);
                Text("Maximum March Distance", settings.screenSpaceReflectionsLayers[volumeIndex].maximumMarchDistance.value, "N2",
                    settings.screenSpaceReflectionsLayers[volumeIndex].maximumMarchDistance.overrideState, overrideState => settings.screenSpaceReflectionsLayers[volumeIndex].maximumMarchDistance.overrideState = overrideState);
                Slider("Distance Fade", settings.screenSpaceReflectionsLayers[volumeIndex].distanceFade, 0f, 1f, "N3", fade => settings.screenSpaceReflectionsLayers[volumeIndex].distanceFade.value = fade,
                    settings.screenSpaceReflectionsLayers[volumeIndex].distanceFade.overrideState, overrideState => settings.screenSpaceReflectionsLayers[volumeIndex].distanceFade.overrideState = overrideState);
                Slider("Vignette", settings.screenSpaceReflectionsLayers[volumeIndex].vignette.value, 0f, 1f, "N3", vignette => settings.screenSpaceReflectionsLayers[volumeIndex].vignette.value = vignette,
                    settings.screenSpaceReflectionsLayers[volumeIndex].vignette.overrideState, overrideState => settings.screenSpaceReflectionsLayers[volumeIndex].vignette.overrideState = overrideState);
            }
            GUILayout.EndVertical();
            GUILayout.Space(1);
            GUILayout.BeginVertical(GUIStyles.Skin.box);
            settings.vignetteLayers[volumeIndex].active =
            settings.vignetteLayers[volumeIndex].enabled.value = Toggle("Vignette", settings.vignetteLayers[volumeIndex].enabled.value, true);
            if (settings.vignetteLayers[volumeIndex].enabled.value)
            {
                Selection("Mode", settings.vignetteLayers[volumeIndex].mode.value, mode => settings.vignetteLayers[volumeIndex].mode.value = mode, -1,
                    settings.vignetteLayers[volumeIndex].mode.overrideState, overrideState => settings.vignetteLayers[volumeIndex].mode.overrideState = overrideState);
                SliderColor("Color", settings.vignetteLayers[volumeIndex].color.value, colour => settings.vignetteLayers[volumeIndex].color.value = colour, false,
                    settings.vignetteLayers[volumeIndex].color.overrideState, overrideState => settings.vignetteLayers[volumeIndex].color.overrideState = overrideState);
                Slider("Intensity", settings.vignetteLayers[volumeIndex].intensity, 0f, 1f, "N3", fade => settings.vignetteLayers[volumeIndex].intensity.value = fade,
                    settings.vignetteLayers[volumeIndex].intensity.overrideState, overrideState => settings.vignetteLayers[volumeIndex].intensity.overrideState = overrideState);
                Slider("Smoothness", settings.vignetteLayers[volumeIndex].smoothness.value, 0.01f, 1f, "N3", vignette => settings.vignetteLayers[volumeIndex].smoothness.value = vignette,
                    settings.vignetteLayers[volumeIndex].smoothness.overrideState, overrideState => settings.vignetteLayers[volumeIndex].smoothness.overrideState = overrideState);
                Slider("Roundness", settings.vignetteLayers[volumeIndex].roundness.value, 0f, 1f, "N3", vignette => settings.vignetteLayers[volumeIndex].roundness.value = vignette,
                    settings.vignetteLayers[volumeIndex].roundness.overrideState, overrideState => settings.vignetteLayers[volumeIndex].roundness.overrideState = overrideState);
                settings.vignetteLayers[volumeIndex].rounded.value = Toggle("Rounded", settings.vignetteLayers[volumeIndex].rounded, settings.vignetteLayers[volumeIndex].rounded.overrideState);
            }
            GUILayout.EndVertical();
        }
    }
}
