using AIGraphics.Settings;
using UnityEngine;
using static AIGraphics.Inspector.Util;

namespace AIGraphics.Inspector
{
    internal static class LightInspector
    {
        private static Vector2 lightScrollView;
        private static Vector2 inspectorScrollView;
        private static Light selectedLight;
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
                        if (selectedLight)
                        {
                            if (selectedLight.enabled)
                            {
                                AlloyAreaLight alloyLight = null;
                                if (lightManager.UseAlloyLight)
                                {
                                    alloyLight = selectedLight.GetComponent<AlloyAreaLight>();
                                }

                                Label(selectedLight.name, "", true);
                                GUILayout.Space(10);
                                GUILayout.BeginHorizontal();
                                {
                                    GUILayout.BeginVertical();
                                    {                                        
                                        Label("Colour", "", true);
                                        SliderColor("Colour", selectedLight.color, c => selectedLight.color = c);
                                        if (renderingSettings.LightsUseColorTemperature)
                                        {
                                            Slider("Temperature (K)", selectedLight.colorTemperature, 0f, 30000f, "N0", t => selectedLight.colorTemperature = t);
                                        }

                                        GUILayout.Space(10);
                                        Slider("Intensity", selectedLight.intensity, 0f, 8f, "N2", i => selectedLight.intensity = i);
                                        Slider("Indirect Multiplier", selectedLight.bounceIntensity, 0f, 8f, "N0", bi => selectedLight.bounceIntensity = bi);
                                        GUILayout.Space(10);                                        
                                        Label("Shadows", "", true);
                                        Selection("Shadow Type", selectedLight.shadows, type => selectedLight.shadows = type);
                                        Slider("Strength", selectedLight.shadowStrength, 0f, 1f, "N2", strength => selectedLight.shadowStrength = strength);
                                        Selection("Resolution", selectedLight.shadowResolution, resolution => selectedLight.shadowResolution = resolution, 2);
                                        Slider("Bias", selectedLight.shadowBias, 0f, 2f, "N3", bias => selectedLight.shadowBias = bias);
                                        Slider("Normal Bias", selectedLight.shadowNormalBias, 0f, 3f, "N2", nbias => selectedLight.shadowNormalBias = nbias);
                                        Slider("Near Plane", selectedLight.shadowNearPlane, 0f, 10f, "N2", np => selectedLight.shadowNearPlane = np);
                                        GUILayout.Space(10);
                                        if (showAdvanced)
                                        {
                                            Selection("Render Mode", selectedLight.renderMode, mode => selectedLight.renderMode = mode);
                                            Label("Culling Mask", selectedLight.cullingMask.ToString());
                                        }

                                        if (selectedLight.type == LightType.Directional)
                                        {
                                            Vector3 rot = selectedLight.transform.eulerAngles;
                                            rot.x = Mathf.DeltaAngle(0f, rot.x);
                                            if (rot.x > 180f)
                                            {
                                                rot.x -= 360f;
                                            }
                                            rot.y = Mathf.DeltaAngle(0f, rot.y);
                                            if (rot.y > 180f)
                                            {
                                                rot.y -= 360f;
                                            }
                                            Slider("Vertical Rotation", rot.x, LightSettings.RotationXMin, LightSettings.RotationXMax, "N1", x => { rot.x = x; });
                                            Slider("Horizontal Rotation", rot.y, LightSettings.RotationYMin, LightSettings.RotationYMax, "N1", y => { rot.y = y; });
                                            if (rot != selectedLight.transform.eulerAngles)
                                            {
                                                selectedLight.transform.eulerAngles = rot;
                                            }
                                        }
                                        else
                                        {
                                            Slider("Light Range", selectedLight.range, 0.1f, 100f, "N1", range => { selectedLight.range = range; });
                                            if (selectedLight.type == LightType.Spot)
                                            {
                                                Slider("Spot Angle", selectedLight.spotAngle, 1f, 179f, "N1", angle => { selectedLight.spotAngle = angle; });
                                            }
                                        }

                                        if (lightManager.UseAlloyLight && alloyLight.HasSpecularHighlight && null != alloyLight)
                                        {
                                            Slider("Specular Highlight", alloyLight.Radius, 0f, 1f, "N2", i => alloyLight.Radius = i);

                                            if (selectedLight.type == LightType.Point)
                                            {
                                                Slider("Length", alloyLight.Length, 0f, 1f, "N2", i => alloyLight.Length = i);
                                            }
                                        }
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
                Label(typeName, "", true);
                GUILayout.FlexibleSpace();
                if (KKAPI.GameMode.Studio == KKAPI.KoikatuAPI.GetCurrentGameMode())
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

        private static void LightOverviewModule(LightManager lightManager, Light l)
        {
            if (l == null)
            {
                lightManager.Light();
                return;
            }
            GUILayout.BeginHorizontal();
            GUILayout.Space(5);
            if (GUILayout.Toggle(ReferenceEquals(l, selectedLight), l.name))//, GUIStyles.toolbarbutton))
            {
                selectedLight = l;
            }

            GUILayout.FlexibleSpace();
            l.enabled = GUILayout.Toggle(l.enabled, l.enabled ? " ON" : "OFF");//, GUIStyles.toolbarbutton);
            //l.enabled = !GUILayout.Toggle(!l.enabled, "OFF", GUIStyles.toolbarbutton);
            GUILayout.EndHorizontal();
        }
    }
}