using ExtensibleSaveFormat;
using System.Collections.Generic;
using System.Linq;

namespace AIGraphics
{
    internal class PresetManager
    {
        private readonly string _path;
        private string _currentPresetName;
        private FolderAssist _presetFolder;
        private Dictionary<string, string> _presetNameToPath;
        private readonly AIGraphics _parent;

        internal PresetManager(string presetPath, AIGraphics parent)
        {
            _path = presetPath;
            _parent = parent;
            LoadPresets();
        }

        private void LoadPresets()
        {
            _presetFolder = new FolderAssist();
            _presetFolder.CreateFolderInfo(_path, "*.preset", true, true);
            _presetNameToPath = _presetFolder.lstFile.ToDictionary(entry => entry.FileName, entry => entry.FullPath);
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
