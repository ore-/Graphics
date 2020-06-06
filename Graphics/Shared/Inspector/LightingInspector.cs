using Graphics.Settings;
using Graphics.Textures;
using System.Linq;
using UnityEngine;
using static Graphics.Inspector.Util;

namespace Graphics.Inspector
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
        private static Vector2 dynSkyScrollView;
        private static Vector2 segiScrollView;
        private static int selectedProbe = 0;

        private static bool inspectReflectionProbes = true;
        private static bool inspectSEGI = true;
        private static bool enableSEGI = false;
        private static SEGI segi;

        internal static void Draw(LightingSettings lightingSettings, SkyboxManager skyboxManager, bool showAdvanced)
        {
            GUILayout.BeginVertical(GUIStyles.Skin.box);
            {
                Label("Environment Skybox", "", true);
                GUILayout.Space(1);
                //inactivate controls if no cubemap
                if (skyboxManager.TexturePaths.IsNullOrEmpty())
                {
                    Label("No custom cubemaps found", "");
                }
                else
                {
                    cubeMapScrollView = GUILayout.BeginScrollView(cubeMapScrollView, GUILayout.MaxHeight(300));
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
                Label("Environment Lighting", "", true);
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
                if (null != skyboxManager.Skybox && null != skyboxManager.Skyboxbg)
                {
                    Material skybox = skyboxManager.Skybox;
                    if (skybox.HasProperty("_Exposure"))
                        Slider("Exposure", skyboxManager.Exposure, ExposureMin, ExposureMax, "N1", exp => { skyboxManager.Exposure = exp; skyboxManager.Update = true; });
                    if (skybox.HasProperty("_Rotation"))
                        Slider("Rotation", skyboxManager.Rotation, RotationMin, RotationMax, "N1", rot => { skyboxManager.Rotation = rot; skyboxManager.Update = true; });
                    GUILayout.Space(10);
                    if (skybox.HasProperty("_Tint"))
                        SliderColor("Skybox Tint", skyboxManager.Tint, c => { skyboxManager.Tint = c; skyboxManager.Update = true; }, true);
                }
                GUILayout.Space(10);
                Label("Environment Reflections", "", true);
                GUILayout.Space(1);
                Selection("Resolution", lightingSettings.ReflectionResolution, LightingSettings.ReflectionResolutions, resolution => lightingSettings.ReflectionResolution = resolution);
                Slider("Intensity", lightingSettings.ReflectionIntensity, 0f, 1f, "N1", intensity => { lightingSettings.ReflectionIntensity = intensity; });
                Slider("Bounces", lightingSettings.ReflectionBounces, 1, 5, bounces => { lightingSettings.ReflectionBounces = bounces; });
            }
            GUILayout.EndVertical();
            GUILayout.Space(1);
            GUILayout.BeginVertical(GUIStyles.Skin.box);
            {
                Toggle("Reflection Probes", inspectReflectionProbes, true, inspect => inspectReflectionProbes = inspect);
                if (inspectReflectionProbes)
                {
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
                            Label("Runtime settings", "");
                            Slider("Importance", rp.importance, 0, 1000, importance => rp.importance = importance);
                            Slider("Intensity", rp.intensity, 0, 10, "N2", intensity => rp.intensity = intensity);
                            Toggle("Box Projection", rp.boxProjection, false, box => rp.boxProjection = box);
                            Text("Blend Distance", rp.blendDistance, "N2", distance => rp.blendDistance = distance);
                            Dimension("Box Size", rp.size, size => rp.size = size);
                            Dimension("Box Offset", rp.center, size => rp.center = size);
                            GUILayout.Space(10);
                            Label("Cubemap capture settings", "");
                            Selection("Resolution", rp.resolution, LightingSettings.ReflectionResolutions, resolution => rp.resolution = resolution);
                            Toggle("HDR", rp.hdr, false, hdr => rp.hdr = hdr);
                            Text("Shadow Distance", rp.shadowDistance, "N2", distance => rp.shadowDistance = distance);
                            Selection("Clear Flags", rp.clearFlags, flag => rp.clearFlags = flag);
                            if (showAdvanced)
                            {
                                SelectionMask("Culling Mask", rp.cullingMask, mask => rp.cullingMask = mask);
                            }
                            Text("Clipping Planes - Near", rp.nearClipPlane, "N2", plane => rp.nearClipPlane = plane);
                            Text("Clipping Planes - Far", rp.farClipPlane, "N2", plane => rp.farClipPlane = plane);
                            SliderColor("Background", rp.backgroundColor, colour => { rp.backgroundColor = colour; });
                            Selection("Time Slicing Mode", rp.timeSlicingMode, mode => rp.timeSlicingMode = mode);
                        }
                        GUILayout.EndScrollView();
                        GUILayout.EndVertical();
                    }
                }
            }
            GUILayout.EndVertical();
            GUILayout.BeginVertical(GUIStyles.Skin.box);
            {
                Toggle("SEGI", inspectSEGI, true, inspect => inspectSEGI = inspect);
                if (inspectSEGI)
                {
                    Toggle("Enable", enableSEGI, false, enable => enableSEGI = enable);
                    if (enableSEGI && null != Graphics.Instance.CameraSettings.MainCamera)
                    {
                        if (null != segi)
                            segi = Graphics.Instance.CameraSettings.MainCamera.GetOrAddComponent<SEGI>();
                        if (null != segi)
                        {
                            segi.enabled = true;
                            GUILayout.BeginVertical(GUIStyles.Skin.box);
                            segiScrollView = GUILayout.BeginScrollView(segiScrollView);
                            {
                                Label("Main Configuration", "", true);
                                Selection("Voxel Resolution", segi.voxelResolution, resolution => segi.voxelResolution = resolution);
                                Toggle("Volex AA", segi.voxelAA, false, aa => segi.voxelAA = aa);
                                Slider("Inner Occlusion Layers", segi.innerOcclusionLayers, 0, 2, layers => segi.innerOcclusionLayers = layers);
                                Toggle("Gaussian Mip Filter", segi.gaussianMipFilter, false, filter => segi.gaussianMipFilter = filter);
                                Text("Voxel Space Size", segi.voxelSpaceSize, "N2", size => segi.voxelSpaceSize = size);
                                Text("Shadow Space Size", segi.shadowSpaceSize, "N2", size => segi.shadowSpaceSize = size);
                                SelectionMask("GI Culling Mask", segi.giCullingMask, mask => segi.giCullingMask = mask);
                                Toggle("Update GI", segi.updateGI, false, update => segi.updateGI = update);
                                Toggle("Infinite Bounces", segi.infiniteBounces, false, bounce => segi.infiniteBounces = bounce);
                                Label("VRAM usage", segi.vramUsage.ToString("N2") + " MB");
                                GUILayout.Space(10);
                                Label("Environmental Properties", "", true);
                                string sun = segi.sun ? segi.sun.ToString() : "Please set one directional light as sunlight";
                                Label("Sun", sun);
                                Slider("Soft Sunlight", segi.softSunlight, 0f, 16f, "N2", soft => segi.softSunlight = soft);
                                SliderColor("Sky Colour", segi.skyColor, colour => segi.skyColor = colour);
                                Slider("Sky Intensity", segi.skyIntensity, 0f, 8f, "N2", intensity => segi.skyIntensity = intensity);
                                Toggle("Spherical Skylight", segi.sphericalSkylight, false, spherical => segi.sphericalSkylight = spherical);
                                GUILayout.Space(10);
                                Label("Tracing Properties", "", true);
                                Slider("Temporal Blend Weight", segi.temporalBlendWeight, 0f, 1f, "N2", weight => segi.temporalBlendWeight = weight);
                                Toggle("Bilateral Filtering", segi.useBilateralFiltering, false, filter => segi.useBilateralFiltering = filter);
                                Toggle("Half Resolution", segi.halfResolution, false, half => segi.halfResolution = half);
                                Toggle("Stochastic Sampling", segi.stochasticSampling, false, sampling => segi.stochasticSampling = sampling);
                                Slider("Cones", segi.cones, 1, 128, cones => segi.cones = cones);
                                Slider("Cones Trace Steps", segi.coneTraceSteps, 1, 32, cones => segi.coneTraceSteps = cones);
                                Slider("Cones Length", segi.coneLength, 0.1f, 2f, "N2", cones => segi.coneLength = cones);
                                Slider("Cones Width", segi.coneWidth, 0.5f, 6f, "N2", cones => segi.coneWidth = cones);
                                Slider("Cones Trace Bias", segi.coneTraceBias, 0f, 4f, "N2", cones => segi.coneTraceBias = cones);
                                Slider("Occlusion Strength", segi.occlusionStrength, 0f, 4f, "N2", cones => segi.occlusionStrength = cones);
                                Slider("Near Occlusion Strength", segi.nearOcclusionStrength, 0f, 4f, "N2", cones => segi.nearOcclusionStrength = cones);
                                Slider("Far Occlusion Strength", segi.farOcclusionStrength, 0f, 4f, "N2", cones => segi.farOcclusionStrength = cones);
                                Slider("Farthest Occlusion Strength", segi.farthestOcclusionStrength, 0f, 4f, "N2", cones => segi.farthestOcclusionStrength = cones);
                                Slider("Occlusion Power", segi.occlusionPower, 0.001f, 4f, "N2", cones => segi.occlusionPower = cones);
                                Slider("Near Light Gain", segi.nearLightGain, 0f, 4f, "N2", cones => segi.nearLightGain = cones);
                                Slider("GI Gain", segi.giGain, 0f, 4f, "N2", cones => segi.giGain = cones);
                                GUILayout.Space(10);
                                Slider("Secondary Bounce Gain", segi.secondaryBounceGain, 0f, 4f, "N2", cones => segi.secondaryBounceGain = cones);
                                Slider("Secondary Cones", segi.secondaryCones, 3, 16, cones => segi.secondaryCones = cones);
                                Slider("Secondary Occlusion Strength", segi.secondaryOcclusionStrength, 0.1f, 4f, "N2", cones => segi.secondaryOcclusionStrength = cones);
                                GUILayout.Space(10);
                                Label("Reflection Properties", "", true);
                                Toggle("Do Reflections", segi.doReflections, false, reflections => segi.doReflections = reflections);
                                Slider("Reflection Steps", segi.reflectionSteps, 12, 128, cones => segi.reflectionSteps = cones);
                                Slider("Reflection Occlusion Power", segi.reflectionOcclusionPower, 0.001f, 4f, "N2", cones => segi.reflectionOcclusionPower = cones);
                                Slider("Sky Reflection Intensity", segi.skyReflectionIntensity, 0f, 1f, "N2", intensity => segi.skyReflectionIntensity = intensity);
                                GUILayout.Space(10);
                                Label("Debug Tools", "", true);
                                Toggle("Visualize Sun Depth Texture", segi.visualizeSunDepthTexture, false, visual => segi.visualizeSunDepthTexture = visual);
                                Toggle("Visualize GI", segi.visualizeGI, false, visual => segi.visualizeGI = visual);
                                Toggle("Visualize Voxels", segi.visualizeVoxels, false, visual => segi.visualizeVoxels = visual);
                            }
                            GUILayout.EndScrollView();
                            GUILayout.EndVertical();
                        }
                    }
                    else if (!enableSEGI && null != Graphics.Instance.CameraSettings.MainCamera)
                    {
                        if (null != segi)
                            segi = Graphics.Instance.CameraSettings.MainCamera.GetComponent<SEGI>();
                        if (null != segi)
                            segi.enabled = false;
                    }
                }
            }
            GUILayout.EndVertical();
        }
        internal static void DrawDynSkyboxOptions(LightingSettings lightingSettings, SkyboxManager skyboxManager, bool showAdvanced)
        {
            if (skyboxManager != null && skyboxManager.Skybox != null)
            {
                GUILayout.Space(10);

                GUILayout.BeginVertical(GUIStyles.Skin.box);
                dynSkyScrollView = GUILayout.BeginScrollView(dynSkyScrollView);
                {
                    Material mat = skyboxManager.Skybox;

                    if (mat.shader.name == ProceduralSkyboxSettings.shaderName)
                    {
                        Label("Unity Skybox Settings", "", true);

                        Selection("Sun", (ProceduralSkyboxSettings.SunDisk)mat.GetInt("_SunDisk"), quality =>
                        {
                            mat.SetInt("_SunDisk", (int)quality);
                            skyboxManager.Update = true;
                        });
                        Slider("Sun Size", mat.GetFloat("_SunSize"), 0, 1, "N2", value =>
                        {
                            mat.SetFloat("_SunSize", value);
                            skyboxManager.Update = true;
                        });
                        Slider("Sun Size Convergence", mat.GetFloat("_SunSizeConvergence"), 1, 10, "N2", value =>
                        {
                            mat.SetFloat("_SunSizeConvergence", value);
                            skyboxManager.Update = true;
                        });
                        Slider("Atmosphere Thickness", mat.GetFloat("_AtmosphereThickness"), 0, 5, "N2", value =>
                        {
                            mat.SetFloat("_AtmosphereThickness", value);
                            skyboxManager.Update = true;
                        });
                        SliderColor("Sky Tint", mat.GetColor("_SkyTint"), c =>
                        {
                            mat.SetColor("_SkyTint", c);
                            skyboxManager.Update = true;
                        }, true);
                        SliderColor("Ground Color", mat.GetColor("_GroundColor"), c =>
                        {
                            mat.SetColor("_GroundColor", c);
                            skyboxManager.Update = true;
                        }, true);
                        Slider("Exposure", mat.GetFloat("_Exposure"), 0, 8, "N2", value =>
                        {
                            mat.SetFloat("_Exposure", value);
                            skyboxManager.Update = true;
                        });
                    }
                    else if (mat.shader.name == TwoPointColorSkyboxSettings.shaderName)
                    {
                        Label("Two Point Color Skybox Settings", "", true);
                        SliderColor("Colour A", mat.GetColor("_ColorA"), c =>
                        {
                            mat.SetColor("_ColorA", c);
                            skyboxManager.Update = true;
                        }, true);
                        Slider("Intensity A", mat.GetFloat("_IntensityA"), 0, 2, "N2", intensity =>
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
                        Slider("Intensity B", mat.GetFloat("_IntensityB"), 0, 2, "N2", intensity =>
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
                    else if (mat.shader.name == HemisphereGradientSkyboxSetting.shaderName)
                    {
                        Label("Hemisphere Skybox Settings", "", true);
                        SliderColor("North Pole", mat.GetColor("_TopColor"), c =>
                        {
                            mat.SetColor("_TopColor", c);
                            skyboxManager.Update = true;
                        }, true);
                        SliderColor("Equator", mat.GetColor("_MiddleColor"), c =>
                        {
                            mat.SetColor("_MiddleColor", c);
                            skyboxManager.Update = true;
                        }, true);
                        SliderColor("South Pole", mat.GetColor("_BottomColor"), c =>
                        {
                            mat.SetColor("_BottomColor", c);
                            skyboxManager.Update = true;
                        }, true);
                    }
                    else if (mat.shader.name == FourPointGradientSkyboxSetting.shaderName)
                    {
                        Label("Four Point Gradient Skybox Settings", "", true);
                        SliderColor("Base Color", mat.GetColor("_BaseColor"), c =>
                        {
                            mat.SetColor("_BaseColor", c);
                            skyboxManager.Update = true;
                        }, true);
                        for (int i = 1; i <= 4; i++)
                        {
                            // Switch is compile time property.
                            SliderColor("Color " + i, mat.GetColor("_Color" + i), c =>
                            {
                                mat.SetColor("_Color" + i, c);
                                skyboxManager.Update = true;
                            }, true);
                            Dimension("Direction " + i, mat.GetVector("_Direction" + i), direction =>
                            {
                                mat.SetVector("_Direction" + i, direction);
                                skyboxManager.Update = true;
                            });
                            Slider("Exponent " + i, mat.GetFloat("_Exponent" + i), 0, 2, "N1", intensity =>
                            {
                                mat.SetFloat("_Exponent" + i, intensity);
                                skyboxManager.Update = true;
                            });
                        }
                    }
                }
                GUILayout.EndScrollView();
                GUILayout.EndVertical();
            }
        }
    }
}