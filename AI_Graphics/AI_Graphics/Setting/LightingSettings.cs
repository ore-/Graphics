using AIGraphics;
using MessagePack;
using System.Linq;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;

namespace AIGraphics.Settings {
    [MessagePackObject(keyAsPropertyName: true)]
    public class LightingSettings
    {
        public Parameters.Lighting parameters = new Parameters.Lighting();

        private string textAmbientIntensity = "1";
        private string textReflectionIntensity = "1";
        private string textReflectionBounces = "1";

        internal static int[] ReflectionResolutions = { 128, 256, 512, 1024, 2048 };
        internal enum AIAmbientMode
        {
            Skybox = AmbientMode.Skybox,
            Trilight = AmbientMode.Trilight,
            Flat = AmbientMode.Flat,
        }

        internal Material SkyboxSetting
        {
            get => RenderSettings.skybox;
            set => RenderSettings.skybox = value;
        }

        internal Light SunSetting
        {
            get => RenderSettings.sun;
        }

        internal AIAmbientMode AmbientModeSetting
        {
            get => (AIAmbientMode) RenderSettings.ambientMode;
            set
            {
                if (value != AIAmbientMode.Skybox)
                {
                    RenderSettings.ambientLight = Color.white;
                }
                RenderSettings.ambientMode = (AmbientMode) value;                
            }
        }

        internal float AmbientIntensity
        {
            get => RenderSettings.ambientIntensity;
            set
            {
                if (RenderSettings.ambientIntensity != value)
                {
                    textAmbientIntensity = value.ToString("N0");
                    RenderSettings.ambientIntensity = value;
                }
            }
        }

        internal DefaultReflectionMode ReflectionMode
        {
            get => RenderSettings.defaultReflectionMode;
            set => RenderSettings.defaultReflectionMode = value;
        }

        internal int ReflectionResolution
        {
            get => RenderSettings.defaultReflectionResolution;
            set => RenderSettings.defaultReflectionResolution = value;
        }

        internal float ReflectionIntensity
        {
            get => RenderSettings.reflectionIntensity;
            set
            {
                if (RenderSettings.reflectionIntensity != value)
                {
                    textReflectionIntensity = value.ToString("N2");
                    RenderSettings.reflectionIntensity = value;
                }
            }
        }

        internal int ReflectionBounces
        {
            get => RenderSettings.reflectionBounces;
            set
            {
                if (RenderSettings.reflectionBounces != value)
                {
                    textReflectionBounces = value.ToString("N0");
                    RenderSettings.reflectionBounces = value;
                }
            }
        }
    }
}

