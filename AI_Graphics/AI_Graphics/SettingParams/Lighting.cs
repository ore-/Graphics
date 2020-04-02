using AIGraphics.Settings;
using MessagePack;
using UnityEngine.Rendering;
using static AIGraphics.Settings.LightingSettings;

namespace AIGraphics.Parameters {
    [MessagePackObject(keyAsPropertyName: true)]
    public class Lighting {
        public AmbientMode ambientModeSetting;
        public float ambientIntensity;
        public DefaultReflectionMode reflectionMode;
        public int reflectionResolution;
        public float reflectionIntensity;
        public int reflectionBounces;

        public void Save(LightingSettings settings) {
            ambientModeSetting = (AmbientMode) settings.AmbientModeSetting;
            ambientIntensity = settings.AmbientIntensity;
            reflectionMode = settings.ReflectionMode;
            reflectionResolution = settings.ReflectionResolution;
            reflectionIntensity = settings.ReflectionIntensity;
            reflectionBounces = settings.ReflectionBounces;
        }

        public void Load(LightingSettings settings) {
            settings.AmbientModeSetting = (AIAmbientMode) ambientModeSetting;
            settings.AmbientIntensity = ambientIntensity;
            settings.ReflectionMode = reflectionMode;
            settings.ReflectionResolution = reflectionResolution;
            settings.ReflectionIntensity = reflectionIntensity;
            settings.ReflectionBounces = reflectionBounces;
        }
    }
}