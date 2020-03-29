using KKAPI.Studio.SaveLoad;
using KKAPI.Utilities;
using Studio;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AIGraphics
{
    internal class SceneController : SceneCustomFunctionController
    {
        protected override void OnSceneLoad(SceneOperationKind operation, ReadOnlyDictionary<int, ObjectCtrlInfo> loadedItems)
        {
            Studio.Studio studio = GetStudio();
            AIGraphics parent = GetComponentInParent<AIGraphics>();            
            parent?.SkyboxManager?.SetupDefaultReflectionProbe();
        }

        protected override void OnSceneSave()
        {
            //throw new NotImplementedException();
        }
    }
}
