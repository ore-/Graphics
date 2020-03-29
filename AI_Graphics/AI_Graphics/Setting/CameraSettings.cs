using Cinemachine;
using MessagePack;
using UnityEngine;
using static KKAPI.Studio.StudioAPI;

namespace AIGraphics.Settings
{
    [MessagePackObject(keyAsPropertyName: true)]
    internal class CameraSettings 
    {
        private Camera _camera;
        private CameraClearFlags _clearFlags;
        private RenderingPath _renderingPath;
        private bool _occulsionCulling;
        private bool _hdr;
        private bool _msaa;
        private bool _dynamicResolution;
        private float _fov;
        private float _nearClipPlane;
        private float _farClipPlane;

        internal CameraSettings()
        {
            _clearFlags = MainCamera.clearFlags;
            _renderingPath = MainCamera.renderingPath;
            _occulsionCulling = MainCamera.useOcclusionCulling;
            _hdr = MainCamera.allowHDR;
            _msaa = MainCamera.allowMSAA;
            _dynamicResolution = MainCamera.allowDynamicResolution;
        }

        internal enum AICameraClearFlags
        {
            Skybox = CameraClearFlags.Skybox,
            Colour = CameraClearFlags.SolidColor,
            Depth = CameraClearFlags.Depth,
            Nothing = CameraClearFlags.Nothing,
        }

        internal enum AIRenderingPath
        {
            VertexLit = UnityEngine.RenderingPath.VertexLit,
            Forward = UnityEngine.RenderingPath.Forward,
            Deferred = UnityEngine.RenderingPath.DeferredShading,
        }

        internal AICameraClearFlags ClearFlag
        {
            get => (AICameraClearFlags) MainCamera.clearFlags;
            set => _clearFlags = MainCamera.clearFlags = (CameraClearFlags) value;
        }

        internal AIRenderingPath RenderingPath
        {
            get => (AIRenderingPath) MainCamera.renderingPath;
            set
            {
                if (AIRenderingPath.Forward != value) MSAA = false;
                _renderingPath = MainCamera.renderingPath = (RenderingPath) value;
            }
        }

        internal bool OcculsionCulling
        {
            get => MainCamera.useOcclusionCulling;
            set => _occulsionCulling = MainCamera.useOcclusionCulling = value;
        }

        internal bool HDR
        {
            get => MainCamera.allowHDR;
            set => _hdr = MainCamera.allowHDR = value;
        }

        internal bool MSAA
        {
            get => MainCamera.allowMSAA;
            set
            {
                //MSAA is Forward only
                if (value && RenderingPath != AIRenderingPath.Forward)
                    return;
                _msaa = MainCamera.allowMSAA = value;                
            }
        }

        internal bool DynamicResolution
        {
            get => MainCamera.allowDynamicResolution;
            set => _dynamicResolution = MainCamera.allowDynamicResolution = value;
        }

        internal float Fov
        {
            get
            {
//                if (default(float) == _fov)
                {
                    if (InsideStudio)
                    {
                        Studio.CameraControl control = (Studio.CameraControl)MainCamera.GetComponent<CinemachineBrain>().ActiveVirtualCamera;
                        return control.fieldOfView;
                    }
                    else
                    {
                        return MainCamera.fieldOfView;
                    }
                }
//                return _fov;
            }
            set
            {
                if (value != _fov)
                {
                    if (InsideStudio)
                    {

                        Studio.CameraControl control = (Studio.CameraControl)MainCamera.GetComponent<CinemachineBrain>().ActiveVirtualCamera;
                        control.fieldOfView = value;
                    }
                    else
                    {
                        MainCamera.fieldOfView = value;
                    }
                    _fov = value;
                }
            }
        }

        internal float NearClipPlane
        {
            get 
            {
  //              if (default(float) == _nearClipPlane)
                {
                    if (InsideStudio)
                    {
                        Studio.CameraControl control = (Studio.CameraControl)MainCamera.GetComponent<CinemachineBrain>().ActiveVirtualCamera;
                        return control.State.Lens.NearClipPlane;
                    }
                    else
                    {
                        return MainCamera.nearClipPlane;
                    }
                }
//                return _nearClipPlane;
            }
            set
            {
                if (InsideStudio)
                {
                    Studio.CameraControl control = (Studio.CameraControl)MainCamera.GetComponent<CinemachineBrain>().ActiveVirtualCamera;

                    LensSettings lensSettings = control.GetFieldValue<LensSettings>("lensSettings");
                    lensSettings.NearClipPlane = value;
                    control.SetFieldValue<LensSettings>("lensSettings", lensSettings);

                    CameraState cameraState = control.GetFieldValue<CameraState>("cameraState");
                    cameraState.Lens = lensSettings;
                    control.SetFieldValue<CameraState>("cameraState", cameraState);                    
                }
                else
                {
                    MainCamera.nearClipPlane = value;
                }
                _nearClipPlane = value;
            }
        }

        internal float FarClipPlane
        {
            get 
            {
//                if (default(float) == _farClipPlane)
                {
                    if (InsideStudio)
                    {
                        Studio.CameraControl control = (Studio.CameraControl)MainCamera.GetComponent<CinemachineBrain>().ActiveVirtualCamera;
                        return control.State.Lens.FarClipPlane;
                    }
                    else
                    {
                        return MainCamera.farClipPlane;
                    }
                }
//                return _farClipPlane;
            }
            set
            {
                if (InsideStudio)
                {
                    Studio.CameraControl control = (Studio.CameraControl)MainCamera.GetComponent<CinemachineBrain>().ActiveVirtualCamera;

                    LensSettings lensSettings = control.GetFieldValue<LensSettings>("lensSettings");
                    lensSettings.FarClipPlane = value;
                    control.SetFieldValue<LensSettings>("lensSettings", lensSettings);

                    CameraState cameraState = control.GetFieldValue<CameraState>("cameraState");
                    cameraState.Lens = lensSettings;
                    control.SetFieldValue<CameraState>("cameraState", cameraState);
                }
                else
                {
                    MainCamera.farClipPlane = value;
                }
                _farClipPlane = value;
            }
        }

        internal Camera MainCamera
        {
            get
            {
                if (null == _camera && 0 < Camera.allCameras.Length)
                {   
                    //foreach (var cam in Camera.allCameras)
                    _camera = Camera.main;
                }
                return _camera;
            }
        }
    }
}
