using AIGraphics.Settings;
using UnityEngine;
using static AIGraphics.Inspector.Util;

namespace AIGraphics.Inspector
{
    internal static class SettingsInspector 
    {
        private const float FOVMin = 10f;
        private const float FOVMax = 120f;
        private const float FOVDefault = 23.5f;

        internal static void Draw(CameraSettings cameraSettings, GlobalSettings renderingSettings)
        {
            GUILayout.BeginVertical(GUIStyles.Skin.box);
            {
                GUILayout.Label("Camera", GUIStyles.boldlabel);
                cameraSettings.ClearFlag = Selection("Clear Flags", cameraSettings.ClearFlag, flag => cameraSettings.ClearFlag = flag);
                Slider("Near Clipping Plane", cameraSettings.NearClipPlane, 0.01f, 15000f, "N2", ncp => { cameraSettings.NearClipPlane = ncp; });
                Slider("Far Clipping Plane", cameraSettings.FarClipPlane, 0.01f, 15000f, "N2", ncp => { cameraSettings.FarClipPlane = ncp; });
                Selection("Rendering Path", cameraSettings.RenderingPath, path => cameraSettings.RenderingPath = path);
                Slider("Field of View", cameraSettings.Fov, FOVMin, FOVMax, "N0", fov => { cameraSettings.Fov = fov; });
                cameraSettings.OcculsionCulling = Toggle("Occulsion Culling", cameraSettings.OcculsionCulling);
                cameraSettings.HDR = Toggle("Allow HDR", cameraSettings.HDR);
                cameraSettings.MSAA = Toggle("Allow MSAA (Forward Only)", cameraSettings.MSAA);
                cameraSettings.DynamicResolution = Toggle("Allow DynamicResolution", cameraSettings.DynamicResolution);
                GUILayout.Space(10);
                GUILayout.Label("Rendering", GUIStyles.boldlabel);
                GUILayout.Space(1);                
                Label("Colour Space", QualitySettings.activeColorSpace.ToString());
                Label("Quality Level", QualitySettings.names[QualitySettings.GetQualityLevel()]);
                renderingSettings.PixelLightCount = Text("Pixel Light Count", renderingSettings.PixelLightCount);
                Selection("Anisotropic Textures", renderingSettings.AnisotropicFiltering, filtering => renderingSettings.AnisotropicFiltering = filtering);
                Slider("MSAA Multiplier", renderingSettings.AntiAliasing, 0, 8, aa => renderingSettings.AntiAliasing = aa);
                renderingSettings.RealtimeReflectionProbes = Toggle("Realtime Reflection Probes", renderingSettings.RealtimeReflectionProbes);
                GUILayout.Space(10);
                GUILayout.Label("Shadows", GUIStyles.boldlabel);
                GUILayout.Space(1);
                Selection("Shadowmask Mode", renderingSettings.ShadowmaskModeSetting, mode => renderingSettings.ShadowmaskModeSetting = mode);
                Selection("Shadows", renderingSettings.ShadowQualitySetting, setting => renderingSettings.ShadowQualitySetting = setting);
                Selection("Shadow Resolution", renderingSettings.ShadowResolutionSetting, resolution => renderingSettings.ShadowResolutionSetting = resolution);
                Selection("Shadow Projection", renderingSettings.ShadowProjectionSetting, projection => renderingSettings.ShadowProjectionSetting = projection);
                renderingSettings.ShadowDistance = Text("Shadow Distance", renderingSettings.ShadowDistance);
                renderingSettings.ShadowNearPlaneOffset = Text("Shadow Near Plane Offset", renderingSettings.ShadowNearPlaneOffset);
                GUILayout.Space(10);
                Slider("Font Size", renderingSettings.FontSize, 12, 17, size => renderingSettings.FontSize = size);
                Slider("Window Width", Inspector.Width, 750, Screen.width / 2, size => Inspector.Width = size);
                Slider("Window Height", Inspector.Height, 1024, Screen.height, size => Inspector.Height = size);
                GUILayout.Space(10);
                renderingSettings.ShowAdvancedSettings = Toggle("Show Advanced Settings", renderingSettings.ShowAdvancedSettings);                
            }
            GUILayout.EndVertical();
        }
    }
}
