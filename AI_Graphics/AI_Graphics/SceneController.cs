using KKAPI.Studio.SaveLoad;
using KKAPI.Utilities;
using Studio;

namespace AIGraphics
{
    internal class SceneController : SceneCustomFunctionController
    {
        protected override void OnSceneLoad(SceneOperationKind operation, ReadOnlyDictionary<int, ObjectCtrlInfo> loadedItems)
        {
            Studio.Studio studio = GetStudio();
            AIGraphics parent = AIGraphics.Instance;
            parent?.SkyboxManager?.SetupDefaultReflectionProbe();
            parent?.PresetManager?.Load(GetExtendedData());
        }

        protected override void OnSceneSave()
        {
            SetExtendedData(AIGraphics.Instance?.PresetManager.GetExtendedData());
        }
    }
}
