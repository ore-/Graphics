using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEngine;

namespace Graphics
{
    internal class Util
    {
        internal static void ResizeTexture(Texture2D tex, int width, int height, bool doVFlip, FilterMode mode = FilterMode.Trilinear)
        {
            Scale(tex, width, height);
            if (doVFlip) FlipTextureVertically(tex);
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
            UnityEngine.Graphics.SetRenderTarget(rtt);

            //Setup 2D matrix in range 0..1, so nobody needs to care about sized
            GL.LoadPixelMatrix(0, 1, 1, 0);

            //Then clear & draw the texture to fill the entire RTT.
            GL.Clear(true, true, new Color(0, 0, 0, 0));
            UnityEngine.Graphics.DrawTexture(new Rect(0, 0, 1, 1), src);
            rtt = null;
        }

        internal static void FlipTextureVertically(Texture2D original)
        {
            Color[] originalPixels = original.GetPixels();

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

        internal static List<string> GetFiles(string path, string fileSearchPattern = "*", string dirSearchPattern = "*", SearchOption searchOption = SearchOption.AllDirectories)
        {
            List<string> files = GetFiles(path, fileSearchPattern);

            if (searchOption == SearchOption.TopDirectoryOnly)
            {
                return files;
            }

            List<string> directories = new List<string>(GetDirectories(path, dirSearchPattern));

            for (int i = 0; i < directories.Count; i++)
            {
                directories.AddRange(GetDirectories(directories[i], dirSearchPattern));
            }

            for (int i = 0; i < directories.Count; i++)
            {
                files.AddRange(GetFiles(directories[i], fileSearchPattern));
            }

            return files;
        }

        internal static Dictionary<string, string> GetRelativeFileNameToFullPathMap(string path, string fileSearchPattern)
        {
            return GetFiles(path, fileSearchPattern, "*", SearchOption.AllDirectories).ToDictionary(entry
                => GetRelativePathWithFileExtension(entry,path), entry => entry);
        }

        private static string GetRelativePathWithFileExtension(string fileFullPath, string basePath)
        {
            string fileDir = Path.GetDirectoryName(fileFullPath);
            string baseDir = Path.GetDirectoryName(basePath);
            if (string.Equals(Path.GetFullPath(fileDir), Path.GetFullPath(baseDir), StringComparison.OrdinalIgnoreCase)) 
                return Path.GetFileNameWithoutExtension(fileFullPath);
            
            // Uri requires trailing backslash for relative path to work as expected
            if (!baseDir.EndsWith("\\"))
                baseDir += "\\";

            if (!fileDir.EndsWith("\\"))
                fileDir += "\\";

            Uri baseUri = new Uri(baseDir);
            Uri fullUri = new Uri(fileDir);
            Uri relativeUri = baseUri.MakeRelativeUri(fullUri);

            // Uri's use forward slashes so convert back to backward slashes
            return Path.Combine(relativeUri.ToString().Replace("../", "").Replace("/", "\\"), Path.GetFileNameWithoutExtension(fileFullPath));
        }

        private static List<string> GetDirectories(string path, string searchPattern)
        {
            try
            {
                return Directory.GetDirectories(path, searchPattern).ToList();
            }
            catch (Exception)
            {
                return new List<string>();
            }
        }

        private static List<string> GetFiles(string path, string searchPattern)
        {
            try
            {
                return Directory.GetFiles(path, searchPattern).ToList();
            }
            catch (Exception)
            {
                return new List<string>();
            }
        }
    }
}
