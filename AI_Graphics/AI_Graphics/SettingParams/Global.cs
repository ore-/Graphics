using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AIGraphics.Settings;
using MessagePack;
using UnityEngine;
using UnityEngine.Rendering.PostProcessing;

namespace AIGraphics.Parameters {
    [MessagePackObject(keyAsPropertyName: true)]
    public class Graphic {
        public int pixelLightCount;
        public AnisotropicFiltering anisotropicFiltering;
        public int antiAliasing;
        public bool realtimeReflectionProbes;
        public ShadowmaskMode shadowmaskModeSetting;
        public ShadowQuality shadowQualitySetting;
        public ShadowResolution shadowResolutionSetting;
        public ShadowProjection shadowProjectionSetting;
        public float shadowDistance;
        public float shadowNearPlaneOffset;
        public bool lightsUseLinearIntensity;
        public bool lightsUseColorTemperature;

        public void Save(GlobalSettings setting) {
            pixelLightCount = setting.PixelLightCount;
            anisotropicFiltering = setting.AnisotropicFiltering;
            antiAliasing = setting.AntiAliasing;
            realtimeReflectionProbes = setting.RealtimeReflectionProbes;
            shadowmaskModeSetting = setting.ShadowmaskModeSetting;
            shadowQualitySetting = setting.ShadowQualitySetting;
            shadowResolutionSetting = setting.ShadowResolutionSetting;
            shadowProjectionSetting = setting.ShadowProjectionSetting;
            shadowDistance = setting.ShadowDistance;
            shadowNearPlaneOffset = setting.ShadowNearPlaneOffset;
            lightsUseLinearIntensity = setting.LightsUseLinearIntensity;
            lightsUseColorTemperature = setting.LightsUseColorTemperature;
        }

        public void Load(GlobalSettings setting) {
            setting.PixelLightCount = pixelLightCount;
            setting.AnisotropicFiltering = anisotropicFiltering;
            setting.AntiAliasing = antiAliasing;
            setting.RealtimeReflectionProbes = realtimeReflectionProbes;
            setting.ShadowmaskModeSetting = shadowmaskModeSetting;
            setting.ShadowQualitySetting = shadowQualitySetting;
            setting.ShadowResolutionSetting = shadowResolutionSetting;
            setting.ShadowProjectionSetting = shadowProjectionSetting;
            setting.ShadowDistance = shadowDistance;
            setting.ShadowNearPlaneOffset = shadowNearPlaneOffset;
            setting.LightsUseLinearIntensity = lightsUseLinearIntensity;
            setting.LightsUseColorTemperature = lightsUseColorTemperature;
        }
    }
}