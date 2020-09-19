using UnityEngine;

namespace Graphics
{
    public class SSSManager
    {
        internal static SSS SSSInstance;

        // Initialize Components
        internal void Initialize()
        {
            SSSInstance = Graphics.Instance.CameraSettings.MainCamera.GetOrAddComponent<SSS>();
        }

        // When user enabled the option
        internal void Start()
        {
            // Apparently after release, it's going to be generated again automatically.
            // https://docs.unity3d.com/ScriptReference/RenderTexture.Release.html
        }

        // When user disabled the option
        internal void Destroy()
        {
            // cleanup render texture. 
            if (ReferenceEquals(SSSInstance, null)) return;
            
            if (!ReferenceEquals(null, SSSInstance.LightingTex))
            {
                Clear(ref SSSInstance.LightingTex);
                SSSInstance.LightingTex.Release();
            }

            if (!ReferenceEquals(null, SSSInstance.LightingTexBlurred))
            {
                Clear(ref SSSInstance.LightingTexBlurred);
                SSSInstance.LightingTexBlurred.Release();
            }
            
            if (!ReferenceEquals(null, SSSInstance.SSS_ProfileTex))
            {
                Clear(ref SSSInstance.SSS_ProfileTex);
                SSSInstance.SSS_ProfileTex.Release();
            }
        }

        private static void Clear(ref RenderTexture texture)
        {
            RenderTexture rt = RenderTexture.active;
            RenderTexture.active = texture;
            GL.Clear(true, true, Color.clear);
            RenderTexture.active = rt;
        }
    }
}