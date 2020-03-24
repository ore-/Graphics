using AIGraphics.Settings;
using UnityEngine;
using static AIGraphics.Inspector.Utility;

namespace AIGraphics.Inspector
{
    public static class SettingsModule 
    {
        private const float FOVMin = 10f;
        private const float FOVMax = 120f;
        private const float FOVDefault = 23.5f;

        public static CameraSettings CameraInspector(CameraSettings cameraSettings, Settings.RenderSettings renderingSettings)
        {
            GUILayout.BeginVertical("box");
            {
                GUILayout.Label("Camera", GUIStyles.boldlabel);
                cameraSettings.ClearFlag = Selection(cameraSettings.ClearFlag, "Clear Flags", flag => cameraSettings.ClearFlag = flag);
                Slider(cameraSettings.NearClipPlane, 0.01f, 15000f, "N2", "Near Clipping Plane", ncp => { cameraSettings.NearClipPlane = ncp; });
                Slider(cameraSettings.FarClipPlane, 0.01f, 15000f, "N2", "Far Clipping Plane", ncp => { cameraSettings.FarClipPlane = ncp; });
                Selection(cameraSettings.RenderingPath, "Rendering Path", path => cameraSettings.RenderingPath = path);
                Slider(cameraSettings.Fov, FOVMin, FOVMax, "N0", "Field of View", fov => { cameraSettings.Fov = fov; });
                cameraSettings.OcculsionCulling = Toggle("Occulsion Culling", cameraSettings.OcculsionCulling);
                cameraSettings.HDR = Toggle("Allow HDR", cameraSettings.HDR);
                cameraSettings.MSAA = Toggle("Allow MSAA (Forward Only)", cameraSettings.MSAA);
                cameraSettings.DynamicResolution = Toggle("Allow DynamicResolution", cameraSettings.DynamicResolution);
            }
            GUILayout.EndVertical();
            GUILayout.Space(1);
            GUILayout.BeginVertical("box");
            {
                GUILayout.Label("Rendering", GUIStyles.boldlabel);
                GUILayout.Space(1);                
                Text("Colour Space", QualitySettings.activeColorSpace.ToString());
                Text("Quality Level", QualitySettings.names[QualitySettings.GetQualityLevel()]);
                renderingSettings.PixelLightCount = Text("Pixel Light Count", renderingSettings.PixelLightCount);
                Selection(renderingSettings.AnisotropicFiltering, "Anisotropic Textures", filtering => renderingSettings.AnisotropicFiltering = filtering);
                Selection(renderingSettings.AntiAliasing, "Anti Aliasing", aliasing => renderingSettings.AntiAliasing = aliasing);
                renderingSettings.RealtimeReflectionProbes = Toggle("Realtime Reflection Probes", renderingSettings.RealtimeReflectionProbes);
                GUILayout.Space(10);
                GUILayout.Label("Shadows", GUIStyles.boldlabel);
                GUILayout.Space(1);
                Selection(renderingSettings.ShadowmaskModeSetting, "Shadowmask Mode", mode => renderingSettings.ShadowmaskModeSetting = mode);
                Selection(renderingSettings.ShadowQualitySetting, "Shadows", setting => renderingSettings.ShadowQualitySetting = setting);
                Selection(renderingSettings.ShadowResolutionSetting, "Shadow Resolution", resolution => renderingSettings.ShadowResolutionSetting = resolution);
                Selection(renderingSettings.ShadowProjectionSetting, "Shadow Projection", projection => renderingSettings.ShadowProjectionSetting = projection);
                renderingSettings.ShadowDistance = Text("Shadow Distance", renderingSettings.ShadowDistance);
                renderingSettings.ShadowNearPlaneOffset = Text("Shadow Near Plane Offset", renderingSettings.ShadowNearPlaneOffset);
                GUILayout.Space(10);
                renderingSettings.LightsUseLinearIntensity = Toggle("Lights Use Linear Intensity", renderingSettings.LightsUseLinearIntensity);
                renderingSettings.LightsUseColorTemperature = Toggle("Lights Use Color Temperature", renderingSettings.LightsUseColorTemperature);
                GUILayout.Space(10);                
                Slider(renderingSettings.FontSize, 12, 17, "N0", "Font Size", size => renderingSettings.FontSize = size);
                GUILayout.Space(10);
                renderingSettings.DebugSettings =  Toggle("Show Debug Settings", renderingSettings.DebugSettings);                
            }
            GUILayout.EndVertical();

            return cameraSettings;
        }
    }
}
