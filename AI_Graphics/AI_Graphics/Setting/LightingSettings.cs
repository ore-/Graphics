using AIGraphics;
using MessagePack;
using System.Linq;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;

namespace AIGraphics.Settings
{
    [MessagePackObject(keyAsPropertyName: true)]
    public class LightingSettings
    {
        private string textAmbientIntensity = "1";
        private string textReflectionIntensity = "1";
        private string textReflectionBounces = "1";

        internal static int[] ReflectionResolutions = { 128, 256, 512, 1024, 2048 };
        public enum AIAmbientMode
        {
            Skybox = AmbientMode.Skybox,
            Trilight = AmbientMode.Trilight,
            Flat = AmbientMode.Flat,
        }

        // TODO: Route this with primitive type to parse with messagepack.
        internal Material SkyboxSetting
        {
            get => RenderSettings.skybox;
            set => RenderSettings.skybox = value;
        }

        // TODO: Route this with primitive type to parse with messagepack.
        internal Light SunSetting
        {
            get => RenderSettings.sun;
        }

        public AIAmbientMode AmbientModeSetting
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

        public float AmbientIntensity
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

        public DefaultReflectionMode ReflectionMode
        {
            get => RenderSettings.defaultReflectionMode;
            set => RenderSettings.defaultReflectionMode = value;
        }

        public int ReflectionResolution
        {
            get => RenderSettings.defaultReflectionResolution;
            set => RenderSettings.defaultReflectionResolution = value;
        }

        public float ReflectionIntensity
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

        public int ReflectionBounces
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

