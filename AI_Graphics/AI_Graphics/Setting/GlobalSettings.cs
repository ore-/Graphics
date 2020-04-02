using AIGraphics.Inspector;
using MessagePack;
using UnityEngine;
using UnityEngine.Rendering;
using static KKAPI.Studio.StudioAPI;

namespace AIGraphics.Settings {
    [MessagePackObject(keyAsPropertyName: true)]
    public class GlobalSettings {
        public Parameters.Graphic parameters = new Parameters.Graphic();

        private int _pixelLightCount;

        internal int PixelLightCount
        {
            get => QualitySettings.pixelLightCount;
            set => QualitySettings.pixelLightCount = _pixelLightCount = value;
        }

        internal AnisotropicFiltering AnisotropicFiltering
        {
            get => QualitySettings.anisotropicFiltering;
            set => QualitySettings.anisotropicFiltering = value;
        }

        internal int AntiAliasing
        {
            get => QualitySettings.antiAliasing;
            set => QualitySettings.antiAliasing = (int)value;
        }

        internal bool RealtimeReflectionProbes
        {
            get => QualitySettings.realtimeReflectionProbes;
            set => QualitySettings.realtimeReflectionProbes = value;
        }

        internal ShadowmaskMode ShadowmaskModeSetting
        {
            get => QualitySettings.shadowmaskMode;
            set => QualitySettings.shadowmaskMode = value;
        }

        internal ShadowQuality ShadowQualitySetting
        {
            get => QualitySettings.shadows;
            set => QualitySettings.shadows = value;
        }

        internal ShadowResolution ShadowResolutionSetting
        {
            get => QualitySettings.shadowResolution;
            set => QualitySettings.shadowResolution = value;
        }

        internal ShadowProjection ShadowProjectionSetting
        {
            get => QualitySettings.shadowProjection;
            set => QualitySettings.shadowProjection = value;
        }

        internal float ShadowDistance
        {
            get => QualitySettings.shadowDistance;
            set => QualitySettings.shadowDistance = value;
        }

        internal float ShadowNearPlaneOffset
        {
            get => QualitySettings.shadowNearPlaneOffset;
            set => QualitySettings.shadowNearPlaneOffset = value;
        }

        internal bool LightsUseLinearIntensity
        {
            get => GraphicsSettings.lightsUseLinearIntensity;
            set => GraphicsSettings.lightsUseLinearIntensity = value;
        }

        internal bool LightsUseColorTemperature
        {
            get => GraphicsSettings.lightsUseColorTemperature;
            set
            {
                if (value) LightsUseLinearIntensity = value;
                GraphicsSettings.lightsUseColorTemperature = value;
            }
        }

        internal int FontSize
        {
            get => GUIStyles.FontSize;
            set => GUIStyles.FontSize = AIGraphics.ConfigFontSize.Value = value;
        }

        internal bool ShowAdvancedSettings { get; set; }
    }
}
