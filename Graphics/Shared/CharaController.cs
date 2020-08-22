using KKAPI;
using KKAPI.Chara;
using System.Collections;
using AIChara;
using UnityEngine;

namespace Graphics
{
    // TODO: Optimize Character Focus Controller... Somehow adding male is killing fps!?
    internal class CharaController : CharaCustomFunctionController
    {
        protected override void OnCardBeingSaved(GameMode currentGameMode)
        {
            //no action
        }

        protected override void OnReload(GameMode currentGameMode)
        {
            StartCoroutine(LoadColliders(currentGameMode));
        }

        private IEnumerator LoadColliders(GameMode currentGameMode)
        {
            if (GameMode.Maker == currentGameMode || GameMode.Studio == currentGameMode)
                ChaControl.LoadHitObject();
            
            yield return new WaitUntil(() => null != ChaControl.objHitBody && null != ChaControl.objHitHead);
            ForceColliderUpdate(false);
        }

        internal void ForceColliderUpdate(bool force)
        {
            GameObject[] hitObjects = { ChaControl.objHitBody, ChaControl.objHitHead };
            foreach (GameObject hitObject in hitObjects)
            {
                if (ReferenceEquals(null, hitObject))
                    return;
            
                SkinnedCollisionHelper[] componentsInChildren = hitObject.GetComponentsInChildren<SkinnedCollisionHelper>(true);
                foreach (SkinnedCollisionHelper skinnedCollisionHelper in componentsInChildren)
                {
                    skinnedCollisionHelper.gameObject.SetActive(true);
                    skinnedCollisionHelper.forceUpdate = force;
                    skinnedCollisionHelper.updateOncePerFrame = !force;
                }
            }
        }
    }
}
