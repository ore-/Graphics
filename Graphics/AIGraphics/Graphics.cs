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

        private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
        {
            if (_isLoaded)
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
                // Reset to known preset for everything except Studio -- this loads after scene data loads, don't want to wipe out saved scenes
                if (KKAPI.KoikatuAPI.GetCurrentGameMode() != GameMode.Studio)
                {
                    StartCoroutine("ApplyPresets", scene);
                }
            }
        }

        private IEnumerator ApplyPresets(Scene scene)
        {
            yield return new WaitUntil(() => scene.isLoaded && CameraSettings.MainCamera != null);

            GameMode gameMode = KoikatuAPI.GetCurrentGameMode();
            Log.LogInfo(string.Format("AIS Scene Loaded: {0} Game: {1} CAM FOV: {2}", scene.name, gameMode, CameraSettings.MainCamera.fieldOfView));

            _sssManager?.CheckInstance();

            if (CameraSettings.MainCamera.stereoEnabled) // VR...use VR
            {
                _presetManager?.LoadDefault(PresetDefaultType.VR_GAME);
            }           
            else if (gameMode == GameMode.MainGame)
            {
                // For other main games scenes, we need to preserve the original FOV and not replace from preset
                float _fov = CameraSettings.MainCamera.fieldOfView;
                if (CameraSettings.MainCamera.stereoEnabled)
                {
                    _presetManager?.LoadDefault(PresetDefaultType.VR_GAME);
                }
                else
                {
                    _presetManager?.LoadDefaultForCurrentGameMode();
                }
                CameraSettings.Fov = _fov;  // Not sure why this sometimes doesn't work...
                CameraSettings.MainCamera.fieldOfView = _fov; // But this does...
                Log.LogInfo(string.Format("After Load CAM FOV: {0}", CameraSettings.MainCamera.fieldOfView));
            }
            else if (gameMode != GameMode.Unknown)
            {
                _presetManager?.LoadDefaultForCurrentGameMode();
            }

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