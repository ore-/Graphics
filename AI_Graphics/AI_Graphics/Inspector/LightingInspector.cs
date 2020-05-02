using AIGraphics.Settings;
using AIGraphics.Textures;
using System.Linq;
using UnityEngine;
using static AIGraphics.Inspector.Util;

namespace AIGraphics.Inspector
{
    internal static class LightingInspector
    {
        private const float ExposureMin = 0f;
        private const float ExposureMax = 8f;
        //private const float ExposureDefault = 1f;

        private const float RotationMin = 0f;
        private const float RotationMax = 360f;
        //private const float RotationDefault = 0f;

        private static Vector2 cubeMapScrollView;
        //private static int selectedCubeMapIdx = -1;

        private static Vector2 probeSettingsScrollView;
        private static int selectedProbe = 0;

        internal static void Draw(LightingSettings lightingSettings, SkyboxManager skyboxManager, bool showAdvanced)
        {
            GUILayout.BeginVertical(GUIStyles.Skin.box);
            {
                GUILayout.Label("Environment Skybox", GUIStyles.boldlabel);
                GUILayout.Space(1);
                //inactivate controls if no cubemap
                if (skyboxManager.TexturePaths.IsNullOrEmpty())
                {
                    GUILayout.Label("No custom cubemaps found");
                }
                else
                {
                    cubeMapScrollView = GUILayout.BeginScrollView(cubeMapScrollView);
                    int selectedCubeMapIdx = GUILayout.SelectionGrid(skyboxManager.CurrentTextureIndex, skyboxManager.Previews.ToArray(), Inspector.Width / 150, GUIStyles.Skin.box);
                    if (-1 != selectedCubeMapIdx)
                    {
                        skyboxManager.CurrentTexturePath = skyboxManager.TexturePaths[selectedCubeMapIdx];
                    }

                    GUILayout.EndScrollView();
                }
                DrawDynSkyboxOptions(lightingSettings, skyboxManager, showAdvanced);
                GUILayout.Space(10);
                if (showAdvanced)
                {
                    Label("Skybox Material", lightingSettings?.SkyboxSetting?.name ?? "");
                    Label("Sun Source", lightingSettings?.SunSetting?.name ?? "");
                    GUILayout.Space(10);
                }
                GUILayout.Label("Environment Lighting", GUIStyles.boldlabel);
                GUILayout.Space(1);
                Selection("Source", lightingSettings.AmbientModeSetting, mode =>
                {
                    lightingSettings.AmbientModeSetting = mode;
                    if (mode != LightingSettings.AIAmbientMode.Skybox)
                    {
                        skyboxManager.CurrentTexturePath = SkyboxManager.noCubemap;
                    }
                });
                Slider("Intensity", lightingSettings.AmbientIntensity, LightSettings.IntensityMin, LightSettings.IntensityMax, "N1", intensity => { lightingSettings.AmbientIntensity = intensity; });
                GUI.enabled = null != lightingSettings.SkyboxSetting;
                if (null != skyboxManager.Skybox && null != skyboxManager.Skyboxbg)
                {
                    Slider("Exposure", skyboxManager.Exposure, ExposureMin, ExposureMax, "N1", exp => { skyboxManager.Exposure = exp; skyboxManager.Update = true; });
                    Slider("Rotation", skyboxManager.Rotation, RotationMin, RotationMax, "N1", rot => { skyboxManager.Rotation = rot; skyboxManager.Update = true; });
                    GUILayout.Space(10);
                    SliderColor("Skybox Tint", skyboxManager.Tint, c => { skyboxManager.Tint = c; skyboxManager.Update = true; }, true);
                }
                GUILayout.Space(10);
                GUILayout.Label("Environment Reflections", GUIStyles.boldlabel);
                GUILayout.Space(1);
                Selection("Resolution", lightingSettings.ReflectionResolution, LightingSettings.ReflectionResolutions, resolution => lightingSettings.ReflectionResolution = resolution);
                Slider("Intensity", lightingSettings.ReflectionIntensity, 0f, 1f, "N1", intensity => { lightingSettings.ReflectionIntensity = intensity; });
                Slider("Bounces", lightingSettings.ReflectionBounces, 1, 5, bounces => { lightingSettings.ReflectionBounces = bounces; });
            }
            GUILayout.EndVertical();
            GUILayout.Space(1);
            GUILayout.BeginVertical(GUIStyles.Skin.box);
            {
                GUILayout.Label("Reflection Probes", GUIStyles.boldlabel);
                ReflectionProbe[] rps = skyboxManager.GetReflectinProbes();
                if (0 < rps.Length)
                {
                    string[] probeNames = rps.Select(probe => probe.name).ToArray();
                    selectedProbe = GUILayout.SelectionGrid(selectedProbe, probeNames, 3, GUIStyles.toolbarbutton);
                    ReflectionProbe rp = rps[selectedProbe];
                    GUILayout.Space(1);
                    GUILayout.BeginVertical(GUIStyles.Skin.box);
                    probeSettingsScrollView = GUILayout.BeginScrollView(probeSettingsScrollView);
                    {
                        Label("Type", rp.mode.ToString());
                        GUILayout.Label("Runtime settings");
                        Slider("Importance", rp.importance, 0, 1000, importance => rp.importance = importance);
                        Slider("Intensity", rp.intensity, 0, 10, "N2", intensity => rp.intensity = intensity);
                        rp.boxProjection = Toggle("Box Projection", rp.boxProjection);
                        rp.blendDistance = Text("Blend Distance", rp.blendDistance);
                        Dimension("Box Size", rp.size, size => rp.size = size);
                        Dimension("Box Offset", rp.center, size => rp.center = size);
                        GUILayout.Space(10);
                        GUILayout.Label("Cubemap capture settings");
                        Selection("Resolution", rp.resolution, LightingSettings.ReflectionResolutions, resolution => rp.resolution = resolution);
                        rp.hdr = Toggle("HDR", rp.hdr);
                        rp.shadowDistance = Text("Shadow Distance", rp.shadowDistance);
                        GUILayout.Label("Clear Flags");
                        Selection("Clear Flags", rp.clearFlags, flag => rp.clearFlags = flag);
                        SliderColor("Background", rp.backgroundColor, colour => { rp.backgroundColor = colour; });
                        Label("Culling Mask", rp.cullingMask.ToString());
                        rp.nearClipPlane = Text("Clipping Planes - Near", rp.nearClipPlane, "N1");
                        rp.farClipPlane = Text("Clipping Planes - Far", rp.farClipPlane, "N1");
                        Selection("Time Slicing Mode", rp.timeSlicingMode, mode => rp.timeSlicingMode = mode);
                    }
                    GUILayout.EndScrollView();
                    GUILayout.EndVertical();
                }
            }
            GUILayout.EndVertical();
            GUI.enabled = true;
        }

        internal static void DrawDynSkyboxOptions(LightingSettings lightingSettings, SkyboxManager skyboxManager, bool showAdvanced)
        {
            if (skyboxManager.dynSkyboxSetting != null)
            {
                Material mat = skyboxManager.Skybox;
                // mmmm
                GUILayout.Space(10);
                if (skyboxManager.dynSkyboxSetting.GetType() == typeof(ProceduralSkyboxSettings) &&
                    mat.shader.name == ProceduralSkyboxSettings.shaderName)
                {
                }
                else if (skyboxManager.dynSkyboxSetting.GetType() == typeof(TwoPointColorSkyboxSettings) &&
                    mat.shader.name == TwoPointColorSkyboxSettings.shaderName)
                {
                    SliderColor("Colour A", mat.GetColor("_ColorA"), c =>
                    {
                        mat.SetColor("_ColorA", c);
                        skyboxManager.Update = true;
                    }, true);
                    Slider("Intensity A", mat.GetFloat("_IntensityA"), 0, 2, "N1", intensity =>
                    {
                        mat.SetFloat("_IntensityA", intensity);
                        skyboxManager.Update = true;
                    });
                    Dimension("Box Size", mat.GetVector("_DirA"), direction =>
                    {
                        mat.SetVector("_DirA", direction);
                        skyboxManager.Update = true;
                    });
                    SliderColor("Colour B", mat.GetColor("_ColorB"), c =>
                    {
                        mat.SetColor("_ColorB", c);
                        skyboxManager.Update = true;
                    }, true);
                    Slider("Intensity B", mat.GetFloat("_IntensityB"), 0, 2, "N1", intensity =>
                    {
                        mat.SetFloat("_IntensityB", intensity);
                        skyboxManager.Update = true;
                    });
                    Dimension("Box Size", mat.GetVector("_DirB"), direction =>
                    {
                        mat.SetVector("_DirB", direction);
                        skyboxManager.Update = true;
                    });
                }
            }
        }
    }
}