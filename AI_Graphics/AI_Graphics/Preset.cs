using AIGraphics.Settings;
using AIGraphics.Textures;
using MessagePack;
using System;
using System.IO;
using UnityEngine;

namespace AIGraphics
{
    [MessagePackObject(keyAsPropertyName: true)]
    public struct Preset
    {
        public GlobalSettings global;
        public CameraSettings camera;
        public LightingSettings lights;
        public PostProcessingSettings pp;
        public SkyboxParams skybox;
        public SkyboxSettings skyboxSetting;

        public Preset(GlobalSettings global, CameraSettings camera, LightingSettings lights, PostProcessingSettings pp, SkyboxParams skybox)
        {
            this.camera = camera;
            this.global = global;
            this.lights = lights;
            this.pp = pp;
            this.skybox = skybox;

            // Skybox setting is generated when preset is being saved.
            skyboxSetting = null;
        }

        public void UpdateParameters()
        {
            pp.SaveParameters();
            SkyboxManager manager = AIGraphics.Instance.SkyboxManager;

            Material mat = manager.Skybox;
            if (mat)
            {
                SkyboxSettings setting = null;

                // Generate Setting Class
                // TODO: Find better way
                if (mat.shader.name == ProceduralSkyboxSettings.shaderName) setting = new ProceduralSkyboxSettings();
                else if (mat.shader.name == TwoPointColorSkyboxSettings.shaderName) setting = new TwoPointColorSkyboxSettings();
                else if (mat.shader.name == FourPointGradientSkyboxSetting.shaderName) setting = new FourPointGradientSkyboxSetting();
                else if (mat.shader.name == HemisphereGradientSkyboxSetting.shaderName) setting = new HemisphereGradientSkyboxSetting();

                if (setting != null)
                {
                    setting.Save();
                    skyboxSetting = setting;
                }
            }

            skybox = manager.skyboxParams;
        }
        public byte[] Serialize()
        {
            return MessagePackSerializer.Serialize(this);
        }
        public void Save(string name = "default", string path = "", bool overwrite = true)
        {
            if (path == "")
            {
                path = AIGraphics.ConfigPresetPath.Value; // Runtime Config Preset Path.
            }

            string targetPath = Path.Combine(path, name + ".preset");
            Directory.CreateDirectory(Path.GetDirectoryName(targetPath));
            UpdateParameters();
            byte[] bytes = Serialize();
            if (File.Exists(targetPath) && overwrite)
            {
                File.Delete(targetPath);
                File.WriteAllBytes(targetPath, bytes);
                File.WriteAllText(Path.Combine(path, "debug.json"), MessagePackSerializer.ToJson(this));
            }
            else
            {
                File.WriteAllBytes(targetPath, bytes);
            }
        }
        public bool Load(string name = "default")
        {
            string targetPath = AIGraphics.Instance.PresetManager.PresetPath(name);
            if (File.Exists(targetPath))
            {
                try
                {
                    byte[] bytes = File.ReadAllBytes(targetPath);
                    Load(bytes);
                    return true;
                } catch (Exception e)
                {
                    Debug.Log(string.Format("Couldn't open preset file '{0}'", name + ".preset"));
                    Debug.LogError(e.Message);
                    return false;
                }
            }
            else
            {
                Debug.Log(string.Format("Couldn't find preset file '{0}'", name + ".preset"));
                return false;
            }
        }

        public void Load(byte[] bytes)
        {
            Deserialize(bytes);
            ApplyParameters();
        }

        public void Deserialize(byte[] bytes)
        {
            this = MessagePackSerializer.Deserialize<Preset>(bytes);
        }

        public void ApplyParameters()
        {
            pp.LoadParameters();

            SkyboxManager manager = AIGraphics.Instance.SkyboxManager;
            if (manager)
            {
                if (skyboxSetting != null)
                    manager.dynSkyboxSetting = skyboxSetting;
                manager.skyboxParams = skybox;
                manager.PresetUpdate = true;
                manager.LoadSkyboxParams();
            }
        }
    }
}
