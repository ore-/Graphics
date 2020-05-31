using KKAPI.Studio.SaveLoad;
using KKAPI.Utilities;
using Studio;

namespace Graphics
{
    internal class SceneController : SceneCustomFunctionController
    {
        protected override void OnSceneLoad(SceneOperationKind operation, ReadOnlyDictionary<int, ObjectCtrlInfo> loadedItems)
        {
            Studio.Studio studio = GetStudio();
            Graphics parent = Graphics.Instance;
            parent?.SkyboxManager?.SetupDefaultReflectionProbe();
            parent?.PresetManager?.Load(GetExtendedData());
        }

        protected override void OnSceneSave()
        {
            SetExtendedData(Graphics.Instance?.PresetManager.GetExtendedData());
        }
    }
}
