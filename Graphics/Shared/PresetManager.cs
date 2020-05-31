using ExtensibleSaveFormat;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace Graphics
{
    internal class PresetManager
    {
        private readonly string _path;
        private string _currentPresetName;
        private Dictionary<string, string> _presetNameToPath;
        private readonly Graphics _parent;

        internal PresetManager(string presetPath, Graphics parent)
        {
            _path = presetPath;
            _parent = parent;
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
            Preset preset = new Preset(_parent.Settings, _parent.CameraSettings, _parent.LightingSettings, _parent.PostProcessingSettings, _parent.SkyboxManager.skyboxParams);
            preset.Save(presetName);
            LoadPresets();
        }

        internal PluginData GetExtendedData()
        {
            Preset preset = new Preset(_parent.Settings, _parent.CameraSettings, _parent.LightingSettings, _parent.PostProcessingSettings, _parent.SkyboxManager.skyboxParams);
            PluginData saveData = new PluginData();
            preset.UpdateParameters();
            byte[] presetData = preset.Serialize();
            saveData.data.Add("bytes", presetData);
            saveData.version = 1;
            return saveData;
        }

        internal bool Load(string presetName)
        {
            Preset preset = new Preset(_parent.Settings, _parent.CameraSettings, _parent.LightingSettings, _parent.PostProcessingSettings, _parent.SkyboxManager.skyboxParams);
            return preset.Load(presetName);
        }

        internal void Load(PluginData data)
        {
            if (data != null && data.data.TryGetValue("bytes", out object val))
            {
                byte[] presetData = (byte[])val;
                if (!presetData.IsNullOrEmpty())
                {
                    Preset preset = new Preset(_parent.Settings, _parent.CameraSettings, _parent.LightingSettings, _parent.PostProcessingSettings, _parent.SkyboxManager.skyboxParams);
                    preset.Load(presetData);
                }
            }
        }
    }
}
