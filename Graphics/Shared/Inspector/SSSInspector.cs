using UnityEngine;
using static AIGraphics.Inspector.Util;

namespace AIGraphics.Inspector
{
    internal static class SSSInspector
    {
        internal static void Draw()
        {
            SSS sss = AIGraphics.Instance.CameraSettings.MainCamera.GetComponent<SSS>();
            if (null == sss)
            {
                Label("No subsurface scattering settings found on camera.", "");
                return;
            }

            GUILayout.BeginVertical(GUIStyles.Skin.box);
            {
                Label("Name", sss.transform.parent.name);
                //float verticalDisplacement = 30;                    
                sss.Enabled = Toggle("Enabled", sss.Enabled);
                if (sss.Enabled)
                {
                    if (sss.LightingTex)
                        Label("Buffer size", sss.LightingTex.width + " x " + sss.LightingTex.height);
                    Slider("Downscale factor", sss.Downsampling, 1f, 4f, "N1", sampling => sss.Downsampling = sampling);
                    Slider("Blur size", sss.ScatteringRadius, 0f, 10f, "N1", radius => sss.ScatteringRadius = radius);
                    Slider("Postprocess iterations", sss.ScatteringIterations, 0, 10, iterations => sss.ScatteringIterations = iterations);
                    Slider("Shader iterations per pass", sss.ShaderIterations, 1, 20, iterations => sss.ShaderIterations = iterations);
                    sss.Dither = Toggle("Dither", sss.Dither);
                    if (sss.Dither)
                    {
                        Slider("Dither scale", sss.DitherScale, 1f, 5f, "N1", scale => sss.DitherScale = scale);
                        Slider("Dither intensity", sss.DitherIntensity, 0f, 0.5f, "N1", intensity => sss.DitherIntensity = intensity);
                    }
                }
            }
            GUILayout.EndVertical();
        }
    }
}