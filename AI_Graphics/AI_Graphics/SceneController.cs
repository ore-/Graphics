using ExtensibleSaveFormat;
using KKAPI.Studio.SaveLoad;
using KKAPI.Utilities;
using Studio;
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
                AIGraphics.Instance.preset.Load((byte[]) val);
            }
        }

        protected override void OnSceneSave()
        {
            PluginData saveData = new PluginData();
            saveData.data.Add("bytes", AIGraphics.Instance.preset.ToBytes());
            saveData.version = 1;
            SetExtendedData(saveData);
        }
    }
}
