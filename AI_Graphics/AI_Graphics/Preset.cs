using AIGraphics.Settings;
using MessagePack;
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

        public Preset(GlobalSettings global, CameraSettings camera, LightingSettings lights, PostProcessingSettings pp, SkyboxParams skybox)
        {
            this.camera = camera;
            this.global = global;
            this.lights = lights;
            this.pp = pp;
            this.skybox = skybox;
        }

        public void UpdateParameters()
        {
            pp.SaveParameters();
            skybox = AIGraphics.Instance.SkyboxManager.skyboxParams;
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
        public bool Load(string name = "default", string path = "")
        {
            if (path == "")
            {
                path = AIGraphics.ConfigPresetPath.Value; // Runtime Config Preset Path.
            }

            string targetPath = Path.Combine(path, name + ".preset");
            if (File.Exists(targetPath))
            {
                //try {
                byte[] bytes = File.ReadAllBytes(targetPath);
                Load(bytes);
                return true;
                //} catch {
                //    Debug.Log(string.Format("Failed to load preset file '{0}'", name + ".preset"));
                //    return false;
                //}
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
                manager.skyboxParams = skybox;
                manager.LoadSkyboxParams();
            }
        }
    }
}
