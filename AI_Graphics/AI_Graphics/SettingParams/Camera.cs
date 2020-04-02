using AIGraphics.Settings;
using MessagePack;
using UnityEngine;
using static AIGraphics.Settings.CameraSettings;

namespace AIGraphics.Parameters {
    [MessagePackObject(keyAsPropertyName: true)]
    public class Camera {
        public CameraClearFlags clearFlag;
        public RenderingPath renderingPath;
        public bool occulsionCulling;
        public bool HDR;
        public bool MSAA;
        public bool dynamicResolution;
        public float fov;
        public float nearClipPlane;
        public float farClipPlane;

        public void Save(CameraSettings settings) {
            clearFlag = (CameraClearFlags) settings.ClearFlag;
            renderingPath = (RenderingPath) settings.RenderingPath;
            occulsionCulling = settings.OcculsionCulling;
            HDR = settings.HDR;
            MSAA = settings.MSAA;
            dynamicResolution = settings.DynamicResolution;
            fov = settings.Fov;
            nearClipPlane = settings.NearClipPlane;
            farClipPlane = settings.FarClipPlane;
        }

        public void Load(CameraSettings settings) {
            settings.ClearFlag = (AICameraClearFlags) clearFlag;
            settings.RenderingPath = (AIRenderingPath) renderingPath;
            settings.OcculsionCulling = occulsionCulling;
            settings.HDR = HDR;
            settings.MSAA = MSAA;
            settings.DynamicResolution = dynamicResolution;
            settings.Fov = fov;
            settings.NearClipPlane = nearClipPlane;
            settings.FarClipPlane = farClipPlane;
        }
    }
}
