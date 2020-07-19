using UnityEngine;
using static AIGraphics.Inspector.Util;

namespace AIGraphics.Inspector
{
    internal static class SSSInspector
    {
        private static RenderTexture LightingTex;
        // SSS_ProfileTex, , LightingTexBlurred;
        internal static void Draw(SSSManager sssManager)
        {
            GUILayout.BeginVertical(GUIStyles.Skin.box);
            {
                //float verticalDisplacement = 30;
                GUI.color = Color.white;
                sssManager.Enabled = Toggle("Enabled", sssManager.Enabled);
                if (LightingTex)
                    Label("Buffer size", LightingTex.width + " x " + LightingTex.height);
                Label("Screen size", Screen.width + " x " + Screen.height);
                Slider("Downscale factor", sssManager.Downsampling, 1f, 4f, "N1", sampling => sssManager.Downsampling = sampling);
                Slider("Blur size", sssManager.ScatteringRadius, 0f, 10f, "N1", radius => sssManager.ScatteringRadius = radius);
                Slider("Postprocess iterations", sssManager.ScatteringIterations, 0, 10, iterations => sssManager.ScatteringIterations = iterations);
                Slider("Blur size", sssManager.ScatteringRadius, 0f, 10f, "N1", radius => sssManager.ScatteringRadius = radius);
                Slider("Shader iterations per pass", sssManager.ShaderIterations, 1, 20, iterations => sssManager.ShaderIterations = iterations);                
                sssManager.Dithered = Toggle("Dither", sssManager.Dithered);
                if (sssManager.Dithered)
                {
                    Slider("Dither scale", sssManager.DitherScale, 1f, 5f, "N1", scale => sssManager.DitherScale = scale);
                    Slider("Dither intensity", sssManager.DitherIntensity, 0f, 0.5f, "N1", intensity => sssManager.DitherIntensity = intensity);
                }
            }
            GUILayout.EndVertical();
        }
    }
}