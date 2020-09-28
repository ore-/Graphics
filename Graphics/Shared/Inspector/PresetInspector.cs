using System;
using UnityEngine;
using static Graphics.Inspector.Util;

namespace Graphics.Inspector
{
    internal static class PresetInspector
    {
        private const string _nameCue = "(preset name)";
        private static string _nameToSave = _nameCue;
        private static int _presetIndexOld = -1;
        private static int _presetIndexCurrent = -1;

        private static bool ShouldUpdate => _presetIndexCurrent != -1 && _presetIndexCurrent != _presetIndexOld;

        private static Vector2 presetScrollView;
        internal static void Draw(PresetManager presetManager)
        {
            GUILayout.BeginVertical(GUIStyles.Skin.box);
            Label("Presets","");
            GUILayout.Space(1);
            if (presetManager.PresetNames.IsNullOrEmpty())
            {
                Label("No presets found","");
            }
            else
            {
                presetScrollView = GUILayout.BeginScrollView(presetScrollView);
                _presetIndexCurrent = Array.IndexOf(presetManager.PresetNames, presetManager.CurrentPreset);
                _presetIndexCurrent = GUILayout.SelectionGrid(_presetIndexCurrent, presetManager.PresetNames, Inspector.Width / 200);
                if (ShouldUpdate)
                {
                    presetManager.CurrentPreset = presetManager.PresetNames[_presetIndexCurrent];
                    _presetIndexOld = _presetIndexCurrent; // to prevent continous update;
                }

                GUILayout.EndScrollView();
            }
            GUILayout.Space(1);
            GUILayout.BeginHorizontal();
            _nameToSave = GUILayout.TextField(_nameToSave);
            bool isValidFileName = (0 != _nameToSave.Length && 256 >= _nameToSave.Length);
            bool isCue = (_nameCue == _nameToSave);
            if (Button("Save") && isValidFileName && !isCue)
            {
                presetManager.Save(_nameToSave);
                presetManager.CurrentPreset = _nameToSave;
                _presetIndexOld = Array.IndexOf(presetManager.PresetNames, presetManager.CurrentPreset);
            }
            GUILayout.EndHorizontal();
            GUILayout.Space(1);
            if (!isCue && !isValidFileName)
            {
                GUILayout.Label("Please specify a valid file name.");
            }
            GUILayout.Space(10);
            GUILayout.BeginHorizontal();
            if (Button("Load Main Game DEFAULT", true))
            {
                presetManager.LoadDefault(PresetDefaultType.MAIN_GAME);
            }
            GUILayout.Space(10);
            if (Button("Save Current as Main Game DEFAULT", true))
            {
                presetManager.SaveDefault(PresetDefaultType.MAIN_GAME);
            }
            GUILayout.Space(10);
            if (Button("Reset Main Game DEFAULT", true))
            {
                presetManager.RestoreDefault(PresetDefaultType.MAIN_GAME);
            }
            GUILayout.EndHorizontal();
            GUILayout.BeginHorizontal();
            if (Button("Load Maker DEFAULT", true))
            {
                presetManager.LoadDefault(PresetDefaultType.MAKER);
            }
            GUILayout.Space(10);
            if (Button("Save Current as Maker DEFAULT", true))
            {
                presetManager.SaveDefault(PresetDefaultType.MAKER);
            }
            GUILayout.Space(10);
            if (Button("Reset Maker DEFAULT", true))
            {
                presetManager.RestoreDefault(PresetDefaultType.MAKER);
            }
            GUILayout.EndHorizontal();
            GUILayout.BeginHorizontal();
            if (Button("Load VR DEFAULT", true))
            {
                presetManager.LoadDefault(PresetDefaultType.VR_GAME);
            }
            GUILayout.Space(10);
            if (Button("Save Current as VR DEFAULT", true))
            {
                presetManager.SaveDefault(PresetDefaultType.VR_GAME);
            }
            GUILayout.Space(10);
            if (Button("Reset VR DEFAULT", true))
            {
                presetManager.RestoreDefault(PresetDefaultType.VR_GAME);
            }
            GUILayout.EndHorizontal();
            GUILayout.BeginHorizontal();
            if (Button("Load Studio DEFAULT", true))
            {
                presetManager.LoadDefault(PresetDefaultType.STUDIO);
            }
            GUILayout.Space(10);
            if (Button("Save Current as Studio DEFAULT", true))
            {
                presetManager.SaveDefault(PresetDefaultType.STUDIO);
            }
            GUILayout.Space(10);
            if (Button("Reset Studio DEFAULT", true))
            {
                presetManager.RestoreDefault(PresetDefaultType.STUDIO);
            }
            GUILayout.EndHorizontal();


            GUILayout.EndVertical();
        }
    }
}
