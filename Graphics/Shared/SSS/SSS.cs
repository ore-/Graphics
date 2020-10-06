﻿using KKAPI.Utilities;
using UnityEngine;

namespace Graphics
{
    [RequireComponent(typeof(Camera))]
    public class SSS : MonoBehaviour
    {
        //public bool DebugTab;
        //public bool BlurTab;
        //public bool DitherTab;
        //public bool ResourcesTab;

        public enum ToggleTexture
        {
            LightingTex,
            LightingTexBlurred,
            ProfileTex,
            None
        }

        private Shader _lightingPass;
        private Shader _profile;
        private Shader _separableSSS;
        private Camera cam;
        public bool DEBUG_DISTANCE;
        [SerializeField] [Range(0, 1)] public float DepthTest = 0.3f, NormalTest = 0.3f, ProfileColorTest = .05f, ProfileRadiusTest = .05f;
        public bool DitherEdgeTest;
        [Range(1, 1.2f)] public float EdgeOffset = 1.1f;
        public bool FixPixelLeaks;
        private int InitialpixelLights;
        private ShadowQuality InitialShadows;
        private Camera LightingCamera;
        private GameObject LightingCameraGO;
        private Vector2 m_TextureSize;

        public int maxDistance = 10000;
        public Texture NoiseTexture;
        private Camera ProfileCamera;

        private GameObject ProfileCameraGO;
        public bool ProfilePerObject;
        public Shader ProfileShader, LightingPassShader;
        [Range(0, 10f)] public bool ShowCameras;
        public bool ShowGUI;
        private SSS_convolution sss_convolution;

        [HideInInspector] public RenderTexture SSS_ProfileTex, SSS_ProfileTexR, LightingTex, LightingTexBlurred, LightingTexR, LightingTexBlurredR;
        public Color sssColor = Color.yellow;

        public ToggleTexture toggleTexture = ToggleTexture.LightingTex;
        public bool UseProfileTest;
        internal bool Enabled { get; set; }
        internal float Downsampling { get; set; }

        internal float ScatteringRadius { get; set; }

        internal int ScatteringIterations { get; set; }

        internal int ShaderIterations { get; set; }

        internal bool Dither { get; set; }

        internal float DitherIntensity { get; set; }

        internal float DitherScale { get; set; }

        private void CreateCameras(Camera currentCamera, out Camera ProfileCamera, out Camera LightingCamera)
        {
            ProfileCamera = null;
            if (ProfilePerObject)
            {
                ProfileCameraGO = GameObject.Find("SSS profile Camera");
                if (!ProfileCameraGO)
                {
                    ProfileCameraGO = new GameObject("SSS profile Camera", typeof(Camera));
                    ProfileCameraGO.transform.parent = transform;
                    ProfileCameraGO.transform.localPosition = Vector3.zero;
                    ProfileCameraGO.transform.localEulerAngles = Vector3.zero;

                    ProfileCamera = ProfileCameraGO.GetComponent<Camera>();
                    ProfileCamera.backgroundColor = Color.black;
                    ProfileCamera.enabled = false;
                    ProfileCamera.depth = -254;
                    ProfileCamera.allowMSAA = false;
                }

                ProfileCamera = ProfileCameraGO.GetComponent<Camera>();
            }

            // Camera for lighting
            LightingCamera = null;
            LightingCameraGO = GameObject.Find("SSS Lighting Pass Camera");
            if (!LightingCameraGO)
            {
                LightingCameraGO = new GameObject("SSS Lighting Pass Camera", typeof(Camera));
                LightingCameraGO.transform.parent = transform;
                LightingCameraGO.transform.localPosition = Vector3.zero;
                LightingCameraGO.transform.localEulerAngles = Vector3.zero;

                LightingCamera = LightingCameraGO.GetComponent<Camera>();
                LightingCamera.enabled = false;
                LightingCamera.depth = -846;
                sss_convolution = LightingCameraGO.AddComponent<SSS_convolution>();
                sss_convolution.BlurShader = Shader.Find("Hidden/SeparableSSS");
                if (null == sss_convolution.BlurShader && null != _separableSSS)
                {
                    sss_convolution.BlurShader = _separableSSS;
                }
            }

            LightingCamera = LightingCameraGO.GetComponent<Camera>();
            LightingCamera.allowMSAA = currentCamera.allowMSAA;
            LightingCamera.backgroundColor = currentCamera.backgroundColor;
            LightingCamera.clearFlags = currentCamera.clearFlags;
            LightingCamera.cullingMask = currentCamera.cullingMask;
        }

        private void Awake()
        {
            AssetBundle assetBundle = AssetBundle.LoadFromMemory(ResourceUtils.GetEmbeddedResource("sss"));
            _lightingPass = assetBundle.LoadAsset<Shader>("Assets/SSS/Resources/LightingPass.shader");
            _separableSSS = assetBundle.LoadAsset<Shader>("Assets/SSS/Resources/SeparableSSS.shader");
            _profile = assetBundle.LoadAsset<Shader>("Assets/SSS/Resources/SSS_Profile.shader");
            assetBundle.Unload(false);
            NoiseTexture = ResourceUtils.GetEmbeddedResource("bluenoise.png").LoadTexture();
            ScatteringRadius = 0.2f;
            Downsampling = 1;
            ScatteringIterations = 5;
            ShaderIterations = 10;
            Enabled = false;
            Dither = true;
            DitherIntensity = 1;
            DitherScale = 0.1f;
        }

        private void OnEnable()
        {
            if (SSS_Layer == 0)
            {
                SSS_Layer = 1;
                Graphics.Instance.Log.LogInfo("Setting SSS layer from Nothing to Default");
            }

            //optional
            //Utilities.CreateLayer("SSS pass");
            // SetSSS_Layer(_SSS_LayerName);
            cam = GetComponent<Camera>();

            if (cam.GetComponent<SSS_buffers_viewer>() == null)
            {
                sss_buffers_viewer = cam.gameObject.AddComponent<SSS_buffers_viewer>();
            }

            if (sss_buffers_viewer == null)
            {
                sss_buffers_viewer = cam.gameObject.GetComponent<SSS_buffers_viewer>();
            }

            sss_buffers_viewer.hideFlags = HideFlags.HideAndDontSave;
            //Make things work on load if only scene view is active
            Shader.EnableKeyword("SCENE_VIEW");
            if (ProfilePerObject)
            {
                Shader.EnableKeyword("SSS_PROFILES");
            }
            if (cam.stereoEnabled)
            {
                Graphics.Instance.Log.LogInfo("VR Mode Detected");
                Shader.EnableKeyword("UNITY_STEREO_EYE");
            }
        }

        private void OnPreRender()
        {
            if (Enabled && !ReferenceEquals(cam, null))
            {
                Shader.DisableKeyword("SCENE_VIEW");
                if (ReferenceEquals(null, LightingPassShader))
                {
                    LightingPassShader = Shader.Find("Hidden/LightingPass");
                    if (ReferenceEquals(null, LightingPassShader) && !ReferenceEquals(null, _lightingPass))
                        LightingPassShader = _lightingPass;
                }

                if (ReferenceEquals(null, ProfileShader))
                {
                    ProfileShader = Shader.Find("Hidden/SSS_Profile");
                    if (ReferenceEquals(null, ProfileShader) && !ReferenceEquals(null, _profile))
                        ProfileShader = _profile;
                }

                m_TextureSize.x = cam.pixelWidth / Downsampling;
                m_TextureSize.y = cam.pixelHeight / Downsampling;
                CreateCameras(cam, out ProfileCamera, out LightingCamera);

                #region Render Profile
                if (ProfilePerObject && !ReferenceEquals(null, ProfileCamera))
                {
                    UpdateCameraModes(cam, ProfileCamera);
                    //ProfileCamera.allowHDR = false;
                    ////humm, removes a lot of artifacts when far away
                    ///

                    InitialpixelLights = QualitySettings.pixelLightCount;
                    InitialShadows = QualitySettings.shadows;
                    QualitySettings.pixelLightCount = 0;
                    QualitySettings.shadows = ShadowQuality.Disable;
                    Shader.EnableKeyword("SSS_PROFILES");
                    ProfileCamera.cullingMask = SSS_Layer;
                    ProfileCamera.backgroundColor = Color.black;
                    ProfileCamera.clearFlags = CameraClearFlags.SolidColor;

                    if (cam.stereoEnabled)
                    {    //Left eye   
                        if (cam.stereoActiveEye == Camera.MonoOrStereoscopicEye.Left)
                        {
                            ProfileCamera.projectionMatrix = cam.GetStereoProjectionMatrix(Camera.StereoscopicEye.Left);
                            ProfileCamera.worldToCameraMatrix = cam.GetStereoViewMatrix(Camera.StereoscopicEye.Left);

                            GetProfileRT(ref SSS_ProfileTex, (int)m_TextureSize.x, (int)m_TextureSize.y, "SSS_ProfileTex");
                            Util.RenderToTarget(ProfileCamera, SSS_ProfileTex, ProfileShader);
                            Shader.SetGlobalTexture("SSS_ProfileTex", SSS_ProfileTex);

                        }
                        else 
                        {

                            ProfileCamera.projectionMatrix = cam.GetStereoProjectionMatrix(Camera.StereoscopicEye.Right);
                            ProfileCamera.worldToCameraMatrix = cam.GetStereoViewMatrix(Camera.StereoscopicEye.Right);

                            GetProfileRT(ref SSS_ProfileTexR, (int)m_TextureSize.x, (int)m_TextureSize.y, "SSS_ProfileTexR");
                            Util.RenderToTarget(ProfileCamera, SSS_ProfileTexR, ProfileShader);
                            Shader.SetGlobalTexture("SSS_ProfileTexR", SSS_ProfileTexR);

                        }
                    }
                    else
                    {
                        //Mono
                        GetProfileRT(ref SSS_ProfileTex, (int)m_TextureSize.x, (int)m_TextureSize.y, "SSS_ProfileTex");
                        Util.RenderToTarget(ProfileCamera, SSS_ProfileTex, ProfileShader);
                        Shader.SetGlobalTexture("SSS_ProfileTex", SSS_ProfileTex);
                    }

                    QualitySettings.pixelLightCount = InitialpixelLights;
                    QualitySettings.shadows = InitialShadows;

                }
                else
                {
                    Shader.DisableKeyword("SSS_PROFILES");

                    SafeDestroy(SSS_ProfileTex);
                    SafeDestroy(SSS_ProfileTexR);
                }

                #endregion

                #region Render Lighting

                UpdateCameraModes(cam, LightingCamera);
                LightingCamera.allowHDR = cam.allowHDR;
                // if (SurfaceScattering)
                {
                    if (ReferenceEquals(null, sss_convolution)) sss_convolution = LightingCameraGO.GetComponent<SSS_convolution>();

                    if (sss_convolution && sss_convolution._BlurMaterial)
                    {
                        sss_convolution._BlurMaterial.SetFloat("DepthTest", Mathf.Max(.00001f, DepthTest / 20));
                        maxDistance = Mathf.Max(0, maxDistance);
                        sss_convolution._BlurMaterial.SetFloat("maxDistance", maxDistance);
                        sss_convolution._BlurMaterial.SetFloat("NormalTest", Mathf.Max(.001f, NormalTest));
                        sss_convolution._BlurMaterial.SetFloat("EdgeOffset", EdgeOffset);
                        sss_convolution._BlurMaterial.SetInt("_SSS_NUM_SAMPLES", ShaderIterations + 1);
                        sss_convolution._BlurMaterial.SetColor("sssColor", sssColor);

                        if (Dither)
                        {
                            sss_convolution._BlurMaterial.EnableKeyword("RANDOMIZED_ROTATION");
                            sss_convolution._BlurMaterial.SetFloat("DitherScale", DitherScale);
                            //sss_convolution._BlurMaterial.SetFloat("DitherSpeed", DitherSpeed * 10);
                            sss_convolution._BlurMaterial.SetFloat("DitherIntensity", DitherIntensity);

                            if (NoiseTexture)
                            {
                                sss_convolution._BlurMaterial.SetTexture("NoiseTexture", NoiseTexture);
                            }
                            else
                            {
                                Debug.Log("Noise texture not available");
                            }
                        }
                        else sss_convolution._BlurMaterial.DisableKeyword("RANDOMIZED_ROTATION");

                        if (UseProfileTest && ProfilePerObject) sss_convolution._BlurMaterial.EnableKeyword("PROFILE_TEST");
                        else sss_convolution._BlurMaterial.DisableKeyword("PROFILE_TEST");

                        if (DEBUG_DISTANCE) sss_convolution._BlurMaterial.EnableKeyword("DEBUG_DISTANCE");
                        else sss_convolution._BlurMaterial.DisableKeyword("DEBUG_DISTANCE");

                        if (FixPixelLeaks) sss_convolution._BlurMaterial.EnableKeyword("OFFSET_EDGE_TEST");
                        else sss_convolution._BlurMaterial.DisableKeyword("OFFSET_EDGE_TEST");

                        if (DitherEdgeTest) sss_convolution._BlurMaterial.EnableKeyword("DITHER_EDGE_TEST");
                        else sss_convolution._BlurMaterial.DisableKeyword("DITHER_EDGE_TEST");
                    }

                    sss_convolution.iterations = ScatteringIterations;
                    sss_convolution.BlurRadius = ScatteringRadius;

                    LightingCamera.depthTextureMode = DepthTextureMode.DepthNormals;
                   
                    if (cam.stereoEnabled)
                    {
                        if (cam.stereoActiveEye == Camera.MonoOrStereoscopicEye.Left)
                        {
                            LightingCamera.projectionMatrix = cam.GetStereoProjectionMatrix(Camera.StereoscopicEye.Left);
                            LightingCamera.worldToCameraMatrix = cam.GetStereoViewMatrix(Camera.StereoscopicEye.Left);

                            GetRT(ref LightingTex, (int)m_TextureSize.x, (int)m_TextureSize.y, "LightingTexture");
                            GetRT(ref LightingTexBlurred, (int)m_TextureSize.x, (int)m_TextureSize.y, "SSSLightingTextureBlurred");
                            sss_convolution.blurred = LightingTexBlurred;
                            sss_convolution.rtFormat = LightingTex.format;
                            if (!ReferenceEquals(null, LightingPassShader))
                            {
                                Util.RenderToTarget(LightingCamera, LightingTex, LightingPassShader);
                                Shader.SetGlobalTexture("LightingTexBlurred", LightingTexBlurred);
                                Shader.SetGlobalTexture("LightingTex", LightingTex);
                            }
                        }
                        else if (cam.stereoActiveEye == Camera.MonoOrStereoscopicEye.Right)
                        {
                            LightingCamera.projectionMatrix = cam.GetStereoProjectionMatrix(Camera.StereoscopicEye.Right);
                            LightingCamera.worldToCameraMatrix = cam.GetStereoViewMatrix(Camera.StereoscopicEye.Right);

                            GetRT(ref LightingTexR, (int)m_TextureSize.x, (int)m_TextureSize.y, "LightingTextureR");
                            GetRT(ref LightingTexBlurredR, (int)m_TextureSize.x, (int)m_TextureSize.y, "SSSLightingTextureBlurredR");
                            sss_convolution.blurred = LightingTexBlurredR;
                            sss_convolution.rtFormat = LightingTexR.format;
                            if (!ReferenceEquals(null, LightingPassShader))
                            {
                                Util.RenderToTarget(LightingCamera, LightingTexR, LightingPassShader);
                                Shader.SetGlobalTexture("LightingTexBlurredR", LightingTexBlurredR);
                                Shader.SetGlobalTexture("LightingTexR", LightingTexR);
                            }
                        }
                    }
                    else
                    {
                        GetRT(ref LightingTex, (int)m_TextureSize.x, (int)m_TextureSize.y, "LightingTexture");
                        GetRT(ref LightingTexBlurred, (int)m_TextureSize.x, (int)m_TextureSize.y, "SSSLightingTextureBlurred");
                        sss_convolution.blurred = LightingTexBlurred;
                        sss_convolution.rtFormat = LightingTex.format;
                        if (!ReferenceEquals(null, LightingPassShader))
                        {
                            Util.RenderToTarget(LightingCamera, LightingTex, LightingPassShader);
                            Shader.SetGlobalTexture("LightingTexBlurred", LightingTexBlurred);
                            Shader.SetGlobalTexture("LightingTex", LightingTex);
                        }
                    }
                }

                #endregion
            }
            else
            {
                //LightingCamera.depthTextureMode = DepthTextureMode.None;
            }
        }

        private void OnPostRender()
        {
            Shader.EnableKeyword("SCENE_VIEW");            
        }

        private void SafeDestroy(Object obj)
        {
            if (obj != null)
            {
                DestroyImmediate(obj);
            }

            obj = null;
        }

        private void Cleanup()
        {
            SafeDestroy(LightingCameraGO);
            SafeDestroy(LightingTex);
            SafeDestroy(LightingTexBlurred);
            SafeDestroy(LightingTexR);
            SafeDestroy(LightingTexBlurredR);
        }

        // Cleanup all the objects we possibly have created
        private void OnDisable()
        {
    //        Shader.EnableKeyword("UNITY_STEREO_EYE");
            Shader.EnableKeyword("SCENE_VIEW");
            Cleanup();
        }

        #region layer

        [HideInInspector] public LayerMask SSS_Layer;
        private SSS_buffers_viewer sss_buffers_viewer;

        //[SerializeField]
        //[HideInInspector]
        //string _SSS_LayerName = "SSS pass";
        //public string SSS_LayerName
        //{
        //    get { return _SSS_LayerName; }
        //    set
        //    {
        //        if (_SSS_LayerName != value)
        //            SetSSS_Layer(value);
        //    }
        //}

        //void SetSSS_Layer(string NewSSS_LayerName)
        //{
        //    _SSS_LayerName = NewSSS_LayerName;
        //    SSS_Layer = 1 << LayerMask.NameToLayer(_SSS_LayerName);
        //}

        #endregion

        #region RT formats and camera settings

        private void UpdateCameraModes(Camera src, Camera dest)
        {
            if (ReferenceEquals(null, dest))
            {
                return;
            }

            dest.farClipPlane = src.farClipPlane;
            dest.nearClipPlane = src.nearClipPlane;
            dest.stereoTargetEye = src.stereoTargetEye;
            dest.orthographic = src.orthographic;
            dest.aspect = src.aspect;
            dest.renderingPath = RenderingPath.Forward;
            dest.orthographicSize = src.orthographicSize;
            if (src.stereoEnabled == false)
            {
                if (src.usePhysicalProperties == false)
                {
                    dest.fieldOfView = src.fieldOfView;
                }
                else
                {
                    dest.usePhysicalProperties = src.usePhysicalProperties;
                    dest.projectionMatrix = src.projectionMatrix;
                }
            }


            if (src.stereoEnabled && dest.fieldOfView != src.fieldOfView)
            {
                dest.fieldOfView = src.fieldOfView;
            }
        }

        protected RenderTextureReadWrite GetRTReadWrite()
        {
            //return RenderTextureReadWrite.Default;
            return cam.allowHDR ? RenderTextureReadWrite.Default : RenderTextureReadWrite.Linear;
        }

        protected RenderTextureFormat GetRTFormat()
        {
            return cam.allowHDR ? RenderTextureFormat.ARGBFloat : RenderTextureFormat.Default;
        }

        protected void GetRT(ref RenderTexture rt, int x, int y, string name)
        {
            if (x <= 0 || y <= 0)
            {
                return; // Below-equal zero request will crash the game.
            }

            ReleaseRT(rt);
            if (cam.allowMSAA && QualitySettings.antiAliasing > 0)
            {
                sss_convolution.AllowMSAA = cam.allowMSAA;
                rt = RenderTexture.GetTemporary(x, y, 24, GetRTFormat(), GetRTReadWrite(), QualitySettings.antiAliasing);
            }
            else
            {
                rt = RenderTexture.GetTemporary(x, y, 24, GetRTFormat(), GetRTReadWrite());
            }

            rt.filterMode = FilterMode.Bilinear;
            //rt.autoGenerateMips = false;
            rt.name = name;
            rt.wrapMode = TextureWrapMode.Clamp;
        }

        protected void GetProfileRT(ref RenderTexture rt, int x, int y, string name)
        {
            ReleaseRT(rt);
            //if (cam.allowMSAA && QualitySettings.antiAliasing > 0 && AllowMSAA)
            //    rt = RenderTexture.GetTemporary(x, y, 16, RenderTextureFormat.ARGB32, RenderTextureReadWrite.Linear, QualitySettings.antiAliasing);
            //else
            rt = RenderTexture.GetTemporary(x, y, 16, RenderTextureFormat.ARGB32, RenderTextureReadWrite.Linear);
            rt.filterMode = FilterMode.Point;
            rt.autoGenerateMips = false;
            rt.name = name;
            rt.wrapMode = TextureWrapMode.Clamp;
        }

        private void ReleaseRT(RenderTexture rt)
        {
            if (!ReferenceEquals(null, rt))
            {
                RenderTexture.ReleaseTemporary(rt);
                rt = null;
            }
        }

        #endregion
    }
}