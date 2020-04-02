using AIGraphics.Settings;
using MessagePack;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace AIGraphics {
    [MessagePackObject(keyAsPropertyName: true)]
    public struct Preset {
        public CameraSettings camera;
        public GlobalSettings global;
        public LightingSettings light;
        public PostProcessingSettings post;
        public SkyboxParams skybox;

        public Preset(GlobalSettings global, CameraSettings camera, LightingSettings light, PostProcessingSettings post, SkyboxParams skybox) {
            this.camera = camera;
            this.global = global;
            this.light = light;
            this.post = post;
            this.skybox = skybox;
        }

        public byte[] ToBytes() {
            skybox = GameObject.Find("SkyboxManager").GetComponent<SkyboxManager>().skyboxParams;
            post.SaveParameters();
            return MessagePackSerializer.Serialize(this);
        }
        public void Save(string name = "default", string path = "", bool overwrite = true) {
            if (path == "")
                path = AIGraphics.ConfigPresetPath.Value; // Runtime Config Preset Path.

            string targetPath = Path.Combine(path, name + ".preset");
            Directory.CreateDirectory(Path.GetDirectoryName(targetPath));
            byte[] bytes = this.ToBytes();
            if (File.Exists(targetPath) && overwrite) {
                File.Delete(targetPath);
                File.WriteAllBytes(targetPath, bytes);
                File.WriteAllText(Path.Combine(path, "debug.json"), MessagePackSerializer.ToJson(this));
            } else
                File.WriteAllBytes(targetPath, bytes);
        }

        public bool Load(string name = "default", string path = "") {
            if (path == "")
                path = AIGraphics.ConfigPresetPath.Value; // Runtime Config Preset Path.

            string targetPath = Path.Combine(path, name + ".preset");
            if (File.Exists(targetPath)) {
                try {
                    byte[] bytes = File.ReadAllBytes(targetPath);
                    Load(bytes);
                    Debug.Log(string.Format("Loaded preset file '{0}'", name + ".preset"));
                    return true;
                } catch {
                    Debug.Log(string.Format("Failed to load preset file '{0}'", name + ".preset"));
                    return false;
                }
            } else {
                Debug.Log(string.Format("Couldn't find preset file '{0}'", name + ".preset"));
                return false;
            }
        }

        public bool Load(byte[] bytes) {
            this = MessagePackSerializer.Deserialize<Preset>(bytes);
            SkyboxManager manager = GameObject.Find("SkyboxManager").GetComponent<SkyboxManager>();
            if (manager) {
                manager.skyboxParams = this.skybox;
                manager.LoadSkyboxParams();
                post.LoadParameters();
                Debug.Log(skybox.selectedCubeMap);
            }
            return true;
            //try {
            //    this = MessagePackSerializer.Deserialize<Preset>(bytes);
            //    SkyboxManager manager = GameObject.Find("SkyboxManager").GetComponent<SkyboxManager>();
            //    if (manager) {
            //        manager.skyboxParams = this.skybox;
            //        manager.LoadSkyboxParams();
            //    }
            //    return true;
            //} catch {
            //    return false;
            //}
        }
    }
}
