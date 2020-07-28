using KKAPI.Utilities;
using UnityEngine;

namespace Graphics
{
    [RequireComponent(typeof(Camera))]
    internal class SSS : MonoBehaviour
    {
        internal bool Enabled { get; set; }
        internal float Downsampling { get; set; }

        internal float ScatteringRadius { get; set; }

        internal int ScatteringIterations { get; set; }

        internal int ShaderIterations { get; set; }

        internal bool Dither { get; set; }

        internal float DitherIntensity { get; set; }

        internal float DitherScale { get; set; }

        private Shader _lightingPass;
        private Shader _profile;
        private Shader _separableSSS;

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
        public ToggleTexture toggleTexture = ToggleTexture.LightingTex;
        public bool ShowGUI = false;
        public bool DEBUG_DISTANCE = false;
        #region layer

        [HideInInspector]
        public LayerMask SSS_Layer;
        SSS_buffers_viewer sss_buffers_viewer;

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
        public int maxDistance = 10;
        Camera cam;
        Camera LightingCamera;
        Camera ProfileCamera;
        int InitialpixelLights = 0;
        ShadowQuality InitialShadows;
        public Shader ProfileShader, LightingPassShader;
        private Vector2 m_TextureSize;

        [HideInInspector]
        public RenderTexture SSS_ProfileTex, LightingTex, LightingTexBlurred;
        [SerializeField]
        [Range(0, 1)]
        public float DepthTest = 0.3f, NormalTest = 0.3f, ProfileColorTest = .05f, ProfileRadiusTest = .05f;
        [Range(1, 1.2f)]
        public float EdgeOffset = 1.1f;
        public bool FixPixelLeaks = false;
        public bool DitherEdgeTest = false;
        public bool UseProfileTest = false;
        public bool ProfilePerObject = false;
        public Color sssColor = Color.yellow;

        GameObject ProfileCameraGO;
        GameObject LightingCameraGO;
        SSS_convolution sss_convolution;
        [Range(0, 10f)]
        public bool ShowCameras = false;
        public Texture NoiseTexture;

        #region RT formats and camera settings
        private void UpdateCameraModes(Camera src, Camera dest)
        {
            if (dest == null)
                return;

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
                dest.fieldOfView = src.fieldOfView;
        }

        protected RenderTextureReadWrite GetRTReadWrite()
        {
            //return RenderTextureReadWrite.Default;
            return (cam.allowHDR) ? RenderTextureReadWrite.Default : RenderTextureReadWrite.Linear;
        }

        protected RenderTextureFormat GetRTFormat()
        {
            return (cam.allowHDR == true) ? RenderTextureFormat.ARGBFloat : RenderTextureFormat.Default;
        }

        protected void GetRT(ref RenderTexture rt, int x, int y, string name)
        {
            ReleaseRT(rt);
            if (cam.allowMSAA && QualitySettings.antiAliasing > 0)
            {
                sss_convolution.AllowMSAA = cam.allowMSAA;
                rt = RenderTexture.GetTemporary(x, y, 24, GetRTFormat(), GetRTReadWrite(), QualitySettings.antiAliasing);
            }
            else
                rt = RenderTexture.GetTemporary(x, y, 24, GetRTFormat(), GetRTReadWrite());
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

        void ReleaseRT(RenderTexture rt)
        {
            if (rt != null)
            {
                RenderTexture.ReleaseTemporary(rt);
                rt = null;
            }
        }
        #endregion

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

        void Awake()
        {
            AssetBundle assetBundle = AssetBundle.LoadFromMemory(ResourceUtils.GetEmbeddedResource("sss"));
            _lightingPass = assetBundle.LoadAsset<Shader>("Assets/SSS/Resources/LightingPass.shader");
            _separableSSS = assetBundle.LoadAsset<Shader>("Assets/SSS/Resources/SeparableSSS.shader");
            _profile = assetBundle.LoadAsset<Shader>("Assets/SSS/Resources/SSS_Profile.shader");
            assetBundle.Unload(false);
            NoiseTexture = KKAPI.Utilities.TextureUtils.LoadTexture(ResourceUtils.GetEmbeddedResource("bluenoise.tga"));
            ScatteringRadius = 0.2f;
            Downsampling = 1;
            ScatteringIterations = 5;
            ShaderIterations = 10;
            Enabled = false;
            Dither = true;
            DitherIntensity = 1;
            DitherScale = 0.1f;
        }

        void OnEnable()
        {
            if (SSS_Layer == 0)
            {
                SSS_Layer = 1;
                print("Setting SSS layer from Nothing to Default");
            }

            //optional
            //Utilities.CreateLayer("SSS pass");
            // SetSSS_Layer(_SSS_LayerName);
            cam = GetComponent<Camera>();

            if (cam.GetComponent<SSS_buffers_viewer>() == null)
                sss_buffers_viewer = cam.gameObject.AddComponent<SSS_buffers_viewer>();

            if (sss_buffers_viewer == null)
                sss_buffers_viewer = cam.gameObject.GetComponent<SSS_buffers_viewer>();

            sss_buffers_viewer.hideFlags = HideFlags.HideAndDontSave;
            //Make things work on load if only scene view is active
            Shader.EnableKeyword("SCENE_VIEW");
            if (ProfilePerObject)
                Shader.EnableKeyword("SSS_PROFILES");
        }

        private void OnPreRender()
        {
            if (Enabled && null != cam)
            {
                Shader.DisableKeyword("SCENE_VIEW");
                if (LightingPassShader == null)
                {
                    LightingPassShader = Shader.Find("Hidden/LightingPass");
                    if (null == LightingPassShader && null != _lightingPass)
                        LightingPassShader = _lightingPass;
                }
                if (ProfileShader == null)
                {
                    ProfileShader = Shader.Find("Hidden/SSS_Profile");
                    if (null == ProfileShader && null != _profile)
                        ProfileShader = _profile;
                }

                m_TextureSize.x = cam.pixelWidth / Downsampling;
                m_TextureSize.y = cam.pixelHeight / Downsampling;
                CreateCameras(cam, out ProfileCamera, out LightingCamera);

                #region Render Profile
                if (ProfilePerObject)
                {
                    UpdateCameraModes(cam, ProfileCamera);
                    //ProfileCamera.allowHDR = false;//humm, removes a lot of artifacts when far away

                    InitialpixelLights = QualitySettings.pixelLightCount;
                    InitialShadows = QualitySettings.shadows;
                    QualitySettings.pixelLightCount = 0;
                    QualitySettings.shadows = ShadowQuality.Disable;
                    Shader.EnableKeyword("SSS_PROFILES");
                    ProfileCamera.cullingMask = SSS_Layer;
                    ProfileCamera.backgroundColor = Color.black;
                    ProfileCamera.clearFlags = CameraClearFlags.SolidColor;
                    /*
                    if (cam.stereoEnabled)
                    {    //Left eye   
                        if (cam.stereoTargetEye == StereoTargetEyeMask.Both || cam.stereoTargetEye == StereoTargetEyeMask.Left)
                        {
                            ProfileCamera.stereoTargetEye = StereoTargetEyeMask.Left;

                            //ProfileCamera.transform.localPosition = InputTracking.GetLocalPosition(XRNode.LeftEye);
                            //ProfileCamera.transform.localRotation = InputTracking.GetLocalRotation(XRNode.LeftEye);
                            ProfileCamera.projectionMatrix = cam.GetStereoProjectionMatrix(Camera.StereoscopicEye.Left);
                            ProfileCamera.worldToCameraMatrix = cam.GetStereoViewMatrix(Camera.StereoscopicEye.Left);
                            GetProfileRT(ref SSS_ProfileTex, (int)m_TextureSize.x, (int)m_TextureSize.y, "SSS_ProfileTex");
                            Util.RenderToTarget(ProfileCamera, SSS_ProfileTex, ProfileShader);
                            Shader.SetGlobalTexture("SSS_ProfileTex", SSS_ProfileTex);
                        }
                        //Right eye   
                        if (cam.stereoTargetEye == StereoTargetEyeMask.Both || cam.stereoTargetEye == StereoTargetEyeMask.Left)
                        {
                            ProfileCamera.stereoTargetEye = StereoTargetEyeMask.Right;
                            //ProfileCamera.transform.localPosition = InputTracking.GetLocalPosition(XRNode.RightEye);
                            //ProfileCamera.transform.localRotation = InputTracking.GetLocalRotation(XRNode.RightEye);
                            ProfileCamera.projectionMatrix = cam.GetStereoProjectionMatrix(Camera.StereoscopicEye.Right);
                            ProfileCamera.projectionMatrix = cam.GetStereoProjectionMatrix(Camera.StereoscopicEye.Right);
                            ProfileCamera.worldToCameraMatrix = cam.GetStereoViewMatrix(Camera.StereoscopicEye.Right);
                            GetProfileRT(ref SSS_ProfileTexR, (int)m_TextureSize.x, (int)m_TextureSize.y, "SSS_ProfileTexR");
                            Rendering.RenderToTarget(ProfileCamera, SSS_ProfileTexR, ProfileShader);
                            Shader.SetGlobalTexture("SSS_ProfileTexR", SSS_ProfileTexR);
                        }

                    }
                    else
                    {
                    */
                    //Mono
                    //ProfileCamera.projectionMatrix = cam.projectionMatrix;//avoid frustum jitter from taa
                    GetProfileRT(ref SSS_ProfileTex, (int)m_TextureSize.x, (int)m_TextureSize.y, "SSS_ProfileTex");
                    Util.RenderToTarget(ProfileCamera, SSS_ProfileTex, ProfileShader);
                    Shader.SetGlobalTexture("SSS_ProfileTex", SSS_ProfileTex);
                    //}

                    QualitySettings.pixelLightCount = InitialpixelLights;
                    QualitySettings.shadows = InitialShadows;

                }
                else
                {
                    Shader.DisableKeyword("SSS_PROFILES");

                    SafeDestroy(SSS_ProfileTex);
                    //SafeDestroy(SSS_ProfileTexR);
                }
                #endregion

                #region Render Lighting
                UpdateCameraModes(cam, LightingCamera);
                LightingCamera.allowHDR = cam.allowHDR;
                // if (SurfaceScattering)
                {
                    if (sss_convolution == null)
                        sss_convolution = LightingCameraGO.GetComponent<SSS_convolution>();

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
                                sss_convolution._BlurMaterial.SetTexture("NoiseTexture", NoiseTexture);
                            else
                                Debug.Log("Noise texture not available");
                        }
                        else
                            sss_convolution._BlurMaterial.DisableKeyword("RANDOMIZED_ROTATION");

                        if (UseProfileTest && ProfilePerObject)
                            sss_convolution._BlurMaterial.EnableKeyword("PROFILE_TEST");
                        else
                            sss_convolution._BlurMaterial.DisableKeyword("PROFILE_TEST");

                        if (DEBUG_DISTANCE)
                            sss_convolution._BlurMaterial.EnableKeyword("DEBUG_DISTANCE");
                        else
                            sss_convolution._BlurMaterial.DisableKeyword("DEBUG_DISTANCE");

                        if (FixPixelLeaks)
                            sss_convolution._BlurMaterial.EnableKeyword("OFFSET_EDGE_TEST");
                        else
                            sss_convolution._BlurMaterial.DisableKeyword("OFFSET_EDGE_TEST");

                        if (DitherEdgeTest)
                            sss_convolution._BlurMaterial.EnableKeyword("DITHER_EDGE_TEST");
                        else
                            sss_convolution._BlurMaterial.DisableKeyword("DITHER_EDGE_TEST");
                    }
                    sss_convolution.iterations = ScatteringIterations;
                    sss_convolution.BlurRadius = ScatteringRadius;

                    LightingCamera.depthTextureMode = DepthTextureMode.DepthNormals;
                    //LightingCamera.transform.position = cam.transform.position;
                    //LightingCamera.transform.rotation = cam.transform.rotation;
                    //LightingCamera.projectionMatrix = cam.projectionMatrix;//avoid frustum jitter from taa
                    GetRT(ref LightingTex, (int)m_TextureSize.x, (int)m_TextureSize.y, "LightingTexture");
                    GetRT(ref LightingTexBlurred, (int)m_TextureSize.x, (int)m_TextureSize.y, "SSSLightingTextureBlurred");
                    sss_convolution.blurred = LightingTexBlurred;
                    sss_convolution.rtFormat = LightingTex.format;
                    if (null != LightingPassShader)
                    {
                        Util.RenderToTarget(LightingCamera, LightingTex, LightingPassShader);
                        Shader.SetGlobalTexture("LightingTexBlurred", LightingTexBlurred);
                        Shader.SetGlobalTexture("LightingTex", LightingTex);
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
        void SafeDestroy(Object obj)
        {
            if (obj != null)
                DestroyImmediate(obj);
            obj = null;
        }
        void Cleanup()
        {
            SafeDestroy(LightingCameraGO);
            SafeDestroy(LightingTex);
            SafeDestroy(LightingTexBlurred);
        }

        // Cleanup all the objects we possibly have created
        void OnDisable()
        {
            Shader.EnableKeyword("SCENE_VIEW");
            Cleanup();
        }
    }
}