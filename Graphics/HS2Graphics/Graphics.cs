using BepInEx;
using Graphics.Settings;
using KKAPI;
using KKAPI.Studio.SaveLoad;
using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Graphics
{
    public partial class Graphics : BaseUnityPlugin
    {
        private void Awake()
        {
            StudioSaveLoadApi.RegisterExtraBehaviour<SceneController>(GUID);
            SceneManager.sceneLoaded += OnSceneLoaded;
        }

        private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
        {
            if (scene.name == "map_title" && PostProcessingSettings != null)
            {
                PostProcessingSettings.ResetVolume();
            }
            else
            {
                StartCoroutine(InitializeLight(scene));
            }
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
    }
}