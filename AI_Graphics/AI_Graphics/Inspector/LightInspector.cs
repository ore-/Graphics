using AIGraphics.Settings;
using Studio;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using static AIGraphics.Inspector.Util;
using static AIGraphics.LightManager;

namespace AIGraphics.Inspector
{
    internal static class LightInspector
    {
        private static Vector2 lightScrollView;
        private static Vector2 inspectorScrollView;
        private static int customLightIndex = 0;
        internal static void Draw(GlobalSettings renderingSettings, LightManager lightManager, bool showAdvanced)
        {
            GUILayout.BeginVertical(GUIStyles.Skin.box);
            {
                if (showAdvanced)
                {
                    lightManager.UseAlloyLight = Toggle("Use Alloy Light", lightManager.UseAlloyLight);
                    renderingSettings.LightsUseLinearIntensity = Toggle("Lights Use Linear Intensity", renderingSettings.LightsUseLinearIntensity);
                    renderingSettings.LightsUseColorTemperature = Toggle("Lights Use Color Temperature", renderingSettings.LightsUseColorTemperature);
                }
                GUILayout.BeginHorizontal(GUIStyles.Skin.box);
                {
                    lightScrollView = GUILayout.BeginScrollView(lightScrollView);
                    GUILayout.BeginVertical(GUIStyles.Skin.box, GUILayout.Width(200), GUILayout.MaxWidth(250));
                    {
                        LightGroup(lightManager, "Directional Lights", LightSettings.LightType.Directional);
                        GUILayout.Space(5);
                        lightManager.DirectionalLights.ForEach(l => LightOverviewModule(lightManager, l));
                        GUILayout.Space(10);
                        LightGroup(lightManager, "Point Lights", LightSettings.LightType.Point);
                        GUILayout.Space(5);
                        lightManager.PointLights.ForEach(l => LightOverviewModule(lightManager, l));
                        GUILayout.Space(10);
                        LightGroup(lightManager, "Spot Lights", LightSettings.LightType.Spot);
                        GUILayout.Space(5);
                        lightManager.SpotLights.ForEach(l => LightOverviewModule(lightManager, l));
                    }
                    GUILayout.EndVertical();
                    GUILayout.EndScrollView();
                    GUILayout.Space(1);
                    inspectorScrollView = GUILayout.BeginScrollView(inspectorScrollView);
                    GUILayout.BeginVertical(GUIStyles.Skin.box);
                    {
                        if (null != lightManager.SelectedLight)
                        {
                            if (lightManager.SelectedLight.enabled)
                            {
                                AlloyAreaLight alloyLight = null;
                                if (lightManager.UseAlloyLight)
                                {
                                    alloyLight = lightManager.SelectedLight.light.GetComponent<AlloyAreaLight>();
                                }
                                
                                GUILayout.Label(lightManager.SelectedLight.light.name, GUIStyles.boldlabel);
                                GUILayout.Space(10);
                                GUILayout.BeginHorizontal();
                                {
                                    GUILayout.BeginVertical();
                                    {
                                        GUILayout.Label("Colour", GUIStyles.boldlabel);                                        
                                        SliderColor("Colour", lightManager.SelectedLight.color, c => lightManager.SelectedLight.color = c);
                                        if (renderingSettings.LightsUseColorTemperature)
                                        {
                                            Slider("Temperature (K)", lightManager.SelectedLight.light.colorTemperature, 0f, 30000f, "N0", t => lightManager.SelectedLight.light.colorTemperature = t);
                                        }

                                        GUILayout.Space(10);
                                        Slider("Intensity", lightManager.SelectedLight.intensity, LightSettings.IntensityMin, LightSettings.IntensityMax, "N2", i => lightManager.SelectedLight.intensity = i);
                                        Slider("Indirect Multiplier", lightManager.SelectedLight.light.bounceIntensity, LightSettings.IntensityMin, LightSettings.IntensityMax, "N0", bi => lightManager.SelectedLight.light.bounceIntensity = bi);
                                        GUILayout.Space(10);
                                        GUILayout.Label("Shadows", GUIStyles.boldlabel);
                                        Selection("Shadow Type", lightManager.SelectedLight.shadows, type => lightManager.SelectedLight.shadows = type);
                                        Slider("Strength", lightManager.SelectedLight.light.shadowStrength, 0f, 1f, "N2", strength => lightManager.SelectedLight.light.shadowStrength = strength);
                                        Selection("Resolution", lightManager.SelectedLight.light.shadowResolution, resolution => lightManager.SelectedLight.light.shadowResolution = resolution, 2);
                                        Slider("Bias", lightManager.SelectedLight.light.shadowBias, 0f, 2f, "N3", bias => lightManager.SelectedLight.light.shadowBias = bias);
                                        Slider("Normal Bias", lightManager.SelectedLight.light.shadowNormalBias, 0f, 3f, "N2", nbias => lightManager.SelectedLight.light.shadowNormalBias = nbias);
                                        Slider("Near Plane", lightManager.SelectedLight.light.shadowNearPlane, 0f, 10f, "N2", np => lightManager.SelectedLight.light.shadowNearPlane = np);
                                        GUILayout.Space(10);
                                        if (showAdvanced)
                                        {
                                            Selection("Render Mode", lightManager.SelectedLight.light.renderMode, mode => lightManager.SelectedLight.light.renderMode = mode);
                                            Label("Culling Mask", lightManager.SelectedLight.light.cullingMask.ToString());
                                        }

                                        if (lightManager.SelectedLight.type == LightType.Directional)
                                        {
                                            Vector3 rot = lightManager.SelectedLight.rotation;
                                            Slider("Vertical Rotation", rot.x, LightSettings.RotationXMin, LightSettings.RotationXMax, "N1", x => { rot.x = x; });
                                            Slider("Horizontal Rotation", rot.y, LightSettings.RotationYMin, LightSettings.RotationYMax, "N1", y => { rot.y = y; });

                                            if (rot != lightManager.SelectedLight.rotation)
                                            {
                                                lightManager.SelectedLight.rotation = rot;
                                            }
                                        }
                                        else
                                        {
                                            Slider("Light Range", lightManager.SelectedLight.range, 0.1f, 100f, "N1", range => { lightManager.SelectedLight.range = range; });
                                            if (lightManager.SelectedLight.type == LightType.Spot)                                            
                                            {
                                                Slider("Spot Angle", lightManager.SelectedLight.spotAngle, 1f, 179f, "N1", angle => { lightManager.SelectedLight.spotAngle = angle; });
                                            }
                                        }
                                        GUILayout.Space(10);
                                        if (lightManager.UseAlloyLight && alloyLight.HasSpecularHighlight && null != alloyLight)
                                        {
                                            Slider("Specular Highlight", alloyLight.Radius, 0f, 1f, "N2", i => alloyLight.Radius = i);

                                            if (lightManager.SelectedLight.type == LightType.Point)                                            
                                            {
                                                Slider("Length", alloyLight.Length, 0f, 1f, "N2", i => alloyLight.Length = i);                                                
                                            }
                                        }
                                        GUILayout.Space(10);

                                        /* PCSS
                                            public Slider softnessSlider;
                                            public Text softnessText;

                                            [Space(10f)]
                                            public Slider softnessFalloffSlider;
                                            public Text softnessFalloffText;

                                            [Space(10f)]
                                            public Slider blockerSlider;
                                            public Text blockerText;

                                            [Space(10f)]
                                            public Slider pcfSlider;
                                            public Text pcfText;

                                            public void SetBlockerSamples (float samplesFloat)
                                            {
                                                int samples = Mathf.RoundToInt(samplesFloat);
                                                pcssScript.Blocker_SampleCount = samples;
                                                blockerText.text = string.Format("Blocker Samples: {0}", samples);
                                                pcssScript.UpdateShaderValues();
                                            }

                                            public void SetPCFSamples (float samplesFloat)
                                            {
                                                int samples = Mathf.RoundToInt(samplesFloat);
                                                pcssScript.PCF_SampleCount = samples;
                                                pcfText.text = string.Format("PCF Samples: {0}", samples);
                                                pcssScript.UpdateShaderValues();
                                            }

                                            public void SetSoftness (float softness)
                                            {
                                                pcssScript.Softness = softness;
                                                softnessText.text = string.Format("Softness: {0}", softness.ToString("N2"));
                                                pcssScript.UpdateShaderValues();
                                            }

                                            public void SetSoftnessFalloff (float softnessFalloff)
                                            {
                                                pcssScript.SoftnessFalloff = softnessFalloff;
                                                softnessFalloffText.text = string.Format("Softness Falloff: {0}", softnessFalloff.ToString("N2"));
                                                pcssScript.UpdateShaderValues();
                                            }

                                            PCSSLight script = target as PCSSLight;
                                            script.UpdateShaderValues();

                                            if (script.Blocker_GradientBias < Mathf.Epsilon && QualitySettings.shadowCascades == 1)
                                            {
                                                EditorGUILayout.HelpBox("A 'Blocker Gradient Bias' of 0 seems to cause issues when not using shadow cascades. Any non-zero value should fix this.", MessageType.Error);
                                            }
                                         */
                                    }
                                    GUILayout.EndVertical();
                                }
                                GUILayout.EndHorizontal();
                            }
                            else
                            {
                                GUILayout.Label("Selected light is disabled.");
                            }
                        }
                        else
                        {
                            GUILayout.Label("Select a light source on the left panel.");
                        }
                    }
                    GUILayout.EndVertical();
                    GUILayout.EndScrollView();
                }
                GUILayout.EndHorizontal();
            }
            GUILayout.EndVertical();
        }

        private static void LightGroup(LightManager lightManager, string typeName, LightSettings.LightType type)
        {
            GUILayout.BeginHorizontal();
            {
                GUILayout.Label(typeName, GUIStyles.boldlabel);
                GUILayout.FlexibleSpace();
                if (AIGraphics.Instance.IsStudio())
                {
                    if (GUILayout.Button("+"))
                    {
                        Singleton<Studio.Studio>.Instance.AddLight((int)type);
                        lightManager.Light();
                    }
                }
                else if (KKAPI.GameMode.Maker == KKAPI.KoikatuAPI.GetCurrentGameMode() & typeName == "Directional Lights") //add custom directional lights in maker
                {
                    if (GUILayout.Button("+"))
                    {
                        customLightIndex += 1;
                        GameObject lightGameObject = new GameObject("Directional Light " + customLightIndex);
                        Light lightComp = lightGameObject.AddComponent<Light>();
                        lightGameObject.GetComponent<Light>().type = LightType.Directional;
                        lightManager.Light();
                    }
                }
            }
            GUILayout.EndHorizontal();
        }

        private static void LightOverviewModule(LightManager lightManager, LightObject l)
        {
            if (null == l || null == l.light )
            {
                lightManager.Light();
                return;
            }
            GUILayout.BeginHorizontal();
            GUILayout.Space(5);
            
            if (GUILayout.Toggle(ReferenceEquals(l, lightManager.SelectedLight), l.light.name, GUIStyles.Skin.button))
            {
                lightManager.SelectedLight = l;
            }
            GUILayout.FlexibleSpace();
            l.enabled = GUILayout.Toggle(l.enabled, l.enabled ? " ON" : "OFF", GUIStyles.Skin.button);
            GUILayout.EndHorizontal();
        }
    }
}