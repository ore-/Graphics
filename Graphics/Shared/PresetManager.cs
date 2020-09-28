using ExtensibleSaveFormat;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEngine;

namespace Graphics
{
    internal class PresetManager
    {
        private readonly string _defaultsPath;
        private readonly string _path;
        private string _currentPresetName;
        private Dictionary<string, string> _presetNameToPath;
        private readonly Graphics _parent;

        internal PresetManager(string presetPath, Graphics parent)
        {
            _path = presetPath;
            _parent = parent;            
            _defaultsPath = Path.Combine(Path.GetDirectoryName(Graphics.Instance.Info.Location), "Resources", "defaults");

            LoadPresets();
        }

        private void LoadPresets()
        {
            _presetNameToPath = Util.GetRelativeFileNameToFullPathMap(_path, "*.preset");
        }

        internal string PresetPath(string presetName)
        {
            return _presetNameToPath.TryGetValue(presetName, out string presetPath) ? presetPath : "";
        }
        internal string[] PresetNames => _presetNameToPath.Keys.ToArray();

        internal string CurrentPreset
        {
            get => _currentPresetName;
            set
            {
                Load(value);
                _currentPresetName = value;
            }
        }

        internal void Save(string presetName)
        {
            Preset preset = new Preset(_parent.Settings, _parent.CameraSettings, _parent.LightingSettings, _parent.PostProcessingSettings, _parent.SkyboxManager.skyboxParams, _parent.SSSSettings);
            string path = Graphics.ConfigPresetPath.Value; // Runtime Config Preset Path.
            string targetPath = Path.Combine(path, presetName + ".preset");
            preset.Save(targetPath);
            LoadPresets();
        }

        internal void SaveDefault(PresetDefaultType defaultType)
        {
            Preset preset = new Preset(_parent.Settings, _parent.CameraSettings, _parent.LightingSettings, _parent.PostProcessingSettings, _parent.SkyboxManager.skyboxParams, _parent.SSSSettings);
            string targetPath = Path.Combine(_defaultsPath, defaultType.ToString() + ".default");
            preset.Save(targetPath);
            Graphics.Instance.Log.LogInfo(string.Format("Saved New Default: {0}", defaultType));
        }

        internal PluginData GetExtendedData()
        {
            Preset preset = new Preset(_parent.Settings, _parent.CameraSettings, _parent.LightingSettings, _parent.PostProcessingSettings, _parent.SkyboxManager.skyboxParams, _parent.SSSSettings);
            PluginData saveData = new PluginData();
            preset.UpdateParameters();
            byte[] presetData = preset.Serialize();
            saveData.data.Add("bytes", presetData);
            saveData.version = 1;
            return saveData;
        }

        internal bool Load(string presetName)
        {
            Preset preset = new Preset(_parent.Settings, _parent.CameraSettings, _parent.LightingSettings, _parent.PostProcessingSettings, _parent.SkyboxManager.skyboxParams, _parent.SSSSettings);
            presetName = (presetName == null ? "default" : presetName);
            string targetPath = PresetPath(presetName);
            return preset.Load(targetPath, presetName);
        }

        internal bool LoadDefault(PresetDefaultType defaultType)
        {
            Preset preset = new Preset(_parent.Settings, _parent.CameraSettings, _parent.LightingSettings, _parent.PostProcessingSettings, _parent.SkyboxManager.skyboxParams, _parent.SSSSettings);
            string presetName = defaultType.ToString();
            string targetPath = Path.Combine(_defaultsPath, presetName + ".default");
            if (!preset.Load(targetPath, presetName))
            {
                // Failed to load the default...try falling back to defaults
                return RestoreDefault(defaultType);
            } 
            else
            {
                Graphics.Instance.Log.LogInfo(string.Format("Loaded Default: {0}", defaultType));
                return true;
            }
        }

        internal bool LoadDefaultForCurrentGameMode()
        {
            return LoadDefault(PresetDefaultTypeUtils.ForGameMode(KKAPI.KoikatuAPI.GetCurrentGameMode()));
        }

        internal bool RestoreDefault(PresetDefaultType defaultType)
        {
            Preset preset = new Preset(_parent.Settings, _parent.CameraSettings, _parent.LightingSettings, _parent.PostProcessingSettings, _parent.SkyboxManager.skyboxParams, _parent.SSSSettings);
            string presetName = defaultType.ToString();
            string targetPath = Path.Combine(_defaultsPath, presetName + ".factory");
            if (preset.Load(targetPath, presetName))
            {
                SaveDefault(defaultType);
                Graphics.Instance.Log.LogInfo(string.Format("Restored {0} to factory shiny.", defaultType.ToString()));
                return true;
            }
            else
            {
                Graphics.Instance.Log.LogError(string.Format("Unable to restore default type {0}", defaultType.ToString()));
                return false;
            }
        }

        internal void Load(PluginData data)
        {
            if (data != null && data.data.TryGetValue("bytes", out object val))
            {
                byte[] presetData = (byte[])val;
                if (!presetData.IsNullOrEmpty())
                {
                    Preset preset = new Preset(_parent.Settings, _parent.CameraSettings, _parent.LightingSettings, _parent.PostProcessingSettings, _parent.SkyboxManager.skyboxParams, _parent.SSSSettings);
                    preset.Load(presetData);
                }
            }
        }
    }
}
