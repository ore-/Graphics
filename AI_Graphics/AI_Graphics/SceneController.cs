using ExtensibleSaveFormat;
using KKAPI.Studio.SaveLoad;
using KKAPI.Utilities;
using Studio;
using System.Collections;
using UnityEngine;

namespace AIGraphics
{
    internal class SceneController : SceneCustomFunctionController
    {
        protected override void OnSceneLoad(SceneOperationKind operation, ReadOnlyDictionary<int, ObjectCtrlInfo> loadedItems)
        {
            Studio.Studio studio = GetStudio();
            AIGraphics parent = GetComponentInParent<AIGraphics>();            
            parent?.SkyboxManager?.SetupDefaultReflectionProbe();

            PluginData saveData = GetExtendedData();
            if (saveData != null && saveData.data.TryGetValue("bytes", out var val)) {
                byte[] presetData = (byte[])val;
                if (!presetData.IsNullOrEmpty())
                    AIGraphics.Instance.preset.Load((byte[])presetData);
            }
        }

        protected override void OnSceneSave()
        {
            PluginData saveData = new PluginData();
            AIGraphics.Instance.preset.UpdateParameters();
            byte[] presetData = AIGraphics.Instance.preset.Serialize();
            saveData.data.Add("bytes", presetData);
            saveData.version = 1;
            SetExtendedData(saveData);
        }
    }
}
