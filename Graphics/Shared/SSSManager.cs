using System.Collections;
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

            if (SSSInstance.LightingTex != null)  // I believe textures have an overridden ReferenceEquals that doesn't do what we expect here...
            {
                Clear(ref SSSInstance.LightingTex);
                SSSInstance.LightingTex?.Release();
            }
            if (SSSInstance.LightingTexR != null)
            {
                Clear(ref SSSInstance.LightingTexR);
                SSSInstance.LightingTexR?.Release();
            }
            if (SSSInstance.LightingTexBlurred != null)
            {
                Clear(ref SSSInstance.LightingTexBlurred);
                SSSInstance.LightingTexBlurred?.Release();
            }
            if (SSSInstance.LightingTexBlurredR != null)
            {
                Clear(ref SSSInstance.LightingTexBlurredR);
                SSSInstance.LightingTexBlurredR?.Release();
            }
            if (SSSInstance.SSS_ProfileTex != null)
            {
                Clear(ref SSSInstance.SSS_ProfileTex);
                SSSInstance.SSS_ProfileTex?.Release();
            }
            if (SSSInstance.SSS_ProfileTexR != null)
            {
                Clear(ref SSSInstance.SSS_ProfileTexR);
                SSSInstance.SSS_ProfileTexR?.Release();
            }
        }

        private static void Clear(ref RenderTexture texture)
        {
            RenderTexture rt = RenderTexture.active;
            RenderTexture.active = texture;
            GL.Clear(true, true, Color.clear);
            RenderTexture.active = rt;
        }

        IEnumerator WaitForCamera()
        {
            Camera camera = Graphics.Instance.CameraSettings.MainCamera;
            yield return new WaitUntil(() => camera != null);
            CheckInstance();
        }
        public void CheckInstance()
        {
            if (SSSInstance == null)
            {
                Camera camera = Graphics.Instance.CameraSettings.MainCamera;
                SSSInstance = camera.GetOrAddComponent<SSS>();
            }
        }
    }
}