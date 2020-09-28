using UnityEngine;
using static Graphics.Inspector.Util;

namespace Graphics.Inspector
{
    internal static class SSSInspector
    {
        internal static void Draw()
        {
            if (ReferenceEquals(null, Graphics.Instance.CameraSettings.MainCamera)) return;
            
            SSS sss = SSSManager.SSSInstance;
            if (sss == null) // the fuck
            {
                Label("No subsurface scattering settings found on camera.", "");
                return;
            }
            Toggle("Enabled", sss.Enabled, false, enabled =>
            {
                CullingMaskExtensions.RefreshLayers();
                sss.Enabled = enabled;
                if (!enabled) Graphics.Instance.SSSManager.Destroy();
                else Graphics.Instance.SSSManager.Start();
            });
            if (!sss.Enabled) return;
            GUILayout.BeginVertical(GUIStyles.Skin.box);
            {
                Label("Blur", "", true);
                Toggle("Profile per object", sss.ProfilePerObject, false, perObj => sss.ProfilePerObject = perObj);
                if( !sss.ProfilePerObject)
                    SliderColor("Scattering colour", sss.sssColor, colour => sss.sssColor = colour);
                Slider("Blur size", sss.ScatteringRadius, 0f, 10f, "N1", radius => sss.ScatteringRadius = radius);
                Slider("Postprocess iterations", sss.ScatteringIterations, 0, 10, iterations => sss.ScatteringIterations = iterations);
                Slider("Shader iterations per pass", sss.ShaderIterations, 1, 20, iterations => sss.ShaderIterations = iterations);
                Slider("Downscale factor", sss.Downsampling, 1f, 4f, "N1", sampling => sss.Downsampling = sampling);
                Slider("Max Distance", sss.maxDistance, 0, 10000, distance => sss.maxDistance = distance);
                Toggle("Debug distance", sss.DEBUG_DISTANCE, false, debug => sss.DEBUG_DISTANCE = debug);
                SelectionMask("Layers", sss.SSS_Layer, layer => sss.SSS_Layer = layer);
            }
            GUILayout.EndVertical();
            GUILayout.BeginVertical(GUIStyles.Skin.box);
            {
                Toggle("Dither", sss.Dither, false, dither => sss.Dither = dither);
                if (sss.Dither)
                {
                    Slider("Dither intensity", sss.DitherIntensity, 0f, 5f, "N1", intensity => sss.DitherIntensity = intensity);
                    Slider("Dither scale", sss.DitherScale, 1f, 100f, "N1", scale => sss.DitherScale = scale);
                }
            }
            GUILayout.EndVertical();
            GUILayout.BeginVertical(GUIStyles.Skin.box);
            {
                Label("Debug", "", true);
                Label("Camera Name", sss.transform.parent.name);
                if (sss.LightingTex)
                    Label("Buffer size", sss.LightingTex.width + " x " + sss.LightingTex.height);
                Label("Light pass shader", ReferenceEquals(sss.LightingPassShader, null) ? "NULL" : sss.LightingPassShader.name);
                Selection("View Buffer", sss.toggleTexture, texture => sss.toggleTexture = texture);
                GUILayout.Space(1);
                Label("Edge test", "", true);
                Slider("Depth test", sss.DepthTest, 0f, 1f, "N3", depth => sss.DepthTest = depth);
                Slider("Normal test", sss.NormalTest, 0f, 1f, "N3", normal => sss.NormalTest = normal);
                Toggle("Apply edge test to dither noise", sss.DitherEdgeTest, false, dither => sss.DitherEdgeTest = dither);
                Toggle("Fix pixel leaks", sss.FixPixelLeaks, false, fix => sss.FixPixelLeaks = fix);
                if (sss.FixPixelLeaks)
                {
                    Slider("Normal test", sss.EdgeOffset, 1f, 1.2f, "N3", offset => sss.EdgeOffset = offset);
                }
                Toggle("Profile Test (per obj)", sss.UseProfileTest, false, profileTest => sss.UseProfileTest = profileTest);
                if (sss.ProfilePerObject && sss.UseProfileTest)
                {
                    Slider("Profile Colour Test", sss.ProfileColorTest, 0f, 1f, "N3", test => sss.ProfileColorTest = test);
                    Slider("Profile Radius Test", sss.ProfileRadiusTest, 0f, 1f, "N3", test => sss.ProfileRadiusTest = test);
                }
            }
            GUILayout.EndVertical();
        }
    }
}