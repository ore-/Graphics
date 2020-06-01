using BepInEx;
using KKAPI;
using KKAPI.Studio.SaveLoad;
using System.Collections;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.SceneManagement;

namespace Graphics
{
    public partial class Graphics : BaseUnityPlugin
    {
        private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
        {   
            StartCoroutine(InitializeLight(scene));
            CullingMaskExtensions.RefreshLayers();
        }

        private IEnumerator InitializeLight(Scene scene)
        {
            yield return new WaitUntil(() => _lightManager != null);

            if (GameMode.Maker == KoikatuAPI.GetCurrentGameMode() && scene.name == "CharaCustom")
            {
                GameObject lights = GameObject.Find("CharaCustom/CustomControl/CharaCamera/Main Camera/Lights Custom");
                if (lights != null)
                {
                    Transform backLight = lights.transform.Find("Directional Light Back");
                    if (backLight != null)
                    {
                        Light light = backLight.GetComponent<Light>();
                        if (light != null)
                        {
                            light.enabled = false;
                        }
                    }
                }
            }

            LightManager.Light();
        }

        private bool IsLoaded()
        {
            switch (KoikatuAPI.GetCurrentGameMode())
            {
                case GameMode.Maker:
                    return KKAPI.Maker.MakerAPI.InsideAndLoaded;
                case GameMode.Studio:
                    return KKAPI.Studio.StudioAPI.StudioLoaded;
                case GameMode.MainGame:
                    return null != GameObject.Find("MapScene") && SceneManager.GetActiveScene().isLoaded && null != Camera.main; //KKAPI doesn't provide an api for in game check 
                default:
                    return false;
            }
        }
    }
}