using Graphics;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[ExecuteInEditMode]
public class SSS_convolution : MonoBehaviour
{
    //public bool Enabled = true;
    float FOV_compensation = 0;
    float initFOV;
    [HideInInspector] public bool AllowMSAA;
    [HideInInspector]
    [Range(0, 1f)]
    public float BlurRadius = 1;
    [HideInInspector]
    public Shader BlurShader = null;
    Camera _ThisCamera;
    [HideInInspector]
    public RenderTextureFormat rtFormat;
    [HideInInspector]
    public Material _BlurMaterial = null;
    Material BlurMaterial
    {
        get
        {
            if (_BlurMaterial == null && BlurShader)
            {
                _BlurMaterial = new Material(BlurShader);
                _BlurMaterial.hideFlags = HideFlags.HideAndDontSave;
            }
            return _BlurMaterial;
        }
    }
    [HideInInspector]
    [Range(0, 10)]
    public int iterations = 3;
    Camera ParentCamera;

    void OnEnable()
    {
     
        _ThisCamera = gameObject.GetComponent<Camera>();
        try
        {
            ParentCamera = transform.parent.GetComponent<Camera>();
        }
        catch
        {
            ParentCamera = FindObjectOfType<SSS>().GetComponent<Camera>();
        }

        initFOV = ParentCamera.fieldOfView;
    }
    //private void Update()
    //{
    //    if (Input.GetKeyDown(KeyCode.P))
    //        Enabled = !Enabled;
    //}
    // Called by the camera to apply the image effect
    //[SerializeField]
    RenderTexture buffer;
    [HideInInspector]
    public RenderTexture blurred;
    int AA = 1;
    float Pitagoras(float x, float y)
    {
        return Mathf.Sqrt(x * x + y * y);
    }

    [ImageEffectOpaque]
    void OnRenderImage(RenderTexture source, RenderTexture destination)
    {

        // if (Enabled)
        {
            FOV_compensation = initFOV / _ThisCamera.fieldOfView;
            //BlurMaterial.SetFloat("BlurRadius", BlurRadius * FOV_compensation);

            int rtW = source.width;
            int rtH = source.height;
            float ScreenSizeCorrection = Pitagoras(rtH, rtW) / Pitagoras(1920, 1080);
         
            BlurRadius *= FOV_compensation;
            BlurRadius *= ScreenSizeCorrection;//so everything will be fine with high-res screenshots
            //float ratio = rtW / rtH;
            //print(ratio);
            //https://github.com/Heep042/Unity-Graphics-Demo/blob/master/Assets/Standard%20Assets/Effects/ImageEffects/Scripts/Bloom.cs
            if (_ThisCamera.allowMSAA && QualitySettings.antiAliasing > 0 && AllowMSAA)
                AA = QualitySettings.antiAliasing;
            else
                AA = 1;

            buffer = RenderTexture.GetTemporary(rtW, rtH, 0, rtFormat, RenderTextureReadWrite.Linear, AA);

            UnityEngine.Graphics.Blit(source, buffer);

            for (int i = 0; i < iterations; i++)
            {
                // Blur vertical
                RenderTexture buffer2 = RenderTexture.GetTemporary(rtW, rtH, 0, rtFormat, RenderTextureReadWrite.Linear, AA);
                BlurMaterial.SetVector("_TexelOffsetScale", new Vector4(0, BlurRadius, 0, 0));
                UnityEngine.Graphics.Blit(buffer, buffer2, BlurMaterial);
                RenderTexture.ReleaseTemporary(buffer);
                buffer = buffer2;

                // Blur horizontal
                buffer2 = RenderTexture.GetTemporary(rtW, rtH, 0, rtFormat, RenderTextureReadWrite.Linear, AA);
                BlurMaterial.SetVector("_TexelOffsetScale", new Vector4(BlurRadius, 0, 0, 0));
                UnityEngine.Graphics.Blit(buffer, buffer2, BlurMaterial);
                RenderTexture.ReleaseTemporary(buffer);
                buffer = buffer2;


                //Circular blur
                /* RenderTexture buffer2 = RenderTexture.GetTemporary(rtW, rtH, 0, rtFormat, RenderTextureReadWrite.Linear, AA);
                 BlurMaterial.SetFloat("_TexelOffsetScale", BlurRadius * FOV_compensation);
                 Graphics.Blit(buffer, buffer2, BlurMaterial);
                 RenderTexture.ReleaseTemporary(buffer);
                 buffer = buffer2;*/
            }

            Debug.Assert(blurred);
            UnityEngine.Graphics.Blit(buffer, blurred);

            UnityEngine.Graphics.Blit(source, destination);
            RenderTexture.ReleaseTemporary(buffer);
        }
        //else
        //{
        //    Graphics.Blit(source, destination);
        //    Graphics.Blit(source, blurred);
        //}
    }
}



