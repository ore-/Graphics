using UnityEngine;

namespace AIGraphics
{
    internal class Util
    {
        internal static void ResizeTexture(Texture2D tex, int width, int height, FilterMode mode = FilterMode.Trilinear)
        {
            Scale(tex, width, height);
            FlipTextureVertically(tex);
        }

        //https://pastebin.com/qkkhWs2J
        internal static void Scale(Texture2D tex, int width, int height, FilterMode mode = FilterMode.Trilinear)
        {
            Rect texR = new Rect(0, 0, width, height);
            _gpu_scale(tex, width, height, mode);

            tex.Resize(width, height);
            tex.ReadPixels(texR, 0, 0, true);
            tex.Apply(true);
        }

        // Internal unility that renders the source texture into the RTT - the scaling method itself.
        //static void _gpu_scale(Texture2D src, int width, int height, FilterMode fmode)
        internal static void _gpu_scale(Texture2D src, int width, int height, FilterMode fmode)
        {
            //We need the source texture in VRAM because we render with it
            src.filterMode = fmode;
            src.Apply(true);

            //Using RTT for best quality and performance. Thanks, Unity 5
            RenderTexture rtt = new RenderTexture(width, height, 32);

            //Set the RTT in order to render to it
            Graphics.SetRenderTarget(rtt);

            //Setup 2D matrix in range 0..1, so nobody needs to care about sized
            GL.LoadPixelMatrix(0, 1, 1, 0);

            //Then clear & draw the texture to fill the entire RTT.
            GL.Clear(true, true, new Color(0, 0, 0, 0));
            Graphics.DrawTexture(new Rect(0, 0, 1, 1), src);
            rtt = null;
        }

        internal static void FlipTextureVertically(Texture2D original)
        {
            var originalPixels = original.GetPixels();

            Color[] newPixels = new Color[originalPixels.Length];

            int width = original.width;
            int rows = original.height;

            for (int x = 0; x < width; x++)
            {
                for (int y = 0; y < rows; y++)
                {
                    newPixels[x + y * width] = originalPixels[x + (rows - y - 1) * width];
                }
            }

            original.SetPixels(newPixels);
            original.Apply();
        }
    }
}
