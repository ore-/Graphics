using System;
using UnityEngine;
using static AIGraphics.Inspector.Util;

namespace AIGraphics.Inspector
{
    internal static class PresetInspector
    {
        private const string _nameCue = "(preset name)";
        private static string _nameToSave = _nameCue;

        private static Vector2 presetScrollView;
        internal static void Draw(PresetManager presetManager)
        {
            GUILayout.BeginVertical(GUIStyles.Skin.box);
            GUILayout.Label("Presets");
            GUILayout.Space(1);
            if (presetManager.PresetNames.IsNullOrEmpty())
            {
                GUILayout.Label("No presets found");
            }
            else
            {
                presetScrollView = GUILayout.BeginScrollView(presetScrollView);
                int selectedPresetIdx = Array.IndexOf(presetManager.PresetNames, presetManager.CurrentPreset);
                selectedPresetIdx = GUILayout.SelectionGrid(selectedPresetIdx, presetManager.PresetNames, Inspector.Width / 150);
                if (-1 != selectedPresetIdx) presetManager.CurrentPreset = presetManager.PresetNames[selectedPresetIdx];
                GUILayout.EndScrollView();
            }
            GUILayout.Space(1);
            GUILayout.BeginHorizontal();
            _nameToSave = GUILayout.TextField(_nameToSave);
            bool isValidFileName = (0 != _nameToSave.Length && 256 >= _nameToSave.Length);
            bool isCue = (_nameCue == _nameToSave);
            if (GUILayout.Button("Save", GUILayout.ExpandWidth(false)) && isValidFileName && !isCue )
                presetManager.Save(_nameToSave);
            GUILayout.EndHorizontal();
            GUILayout.Space(1);
            if (!isCue && !isValidFileName )
                GUILayout.Label("Please specify a valid file name.");
            GUILayout.EndVertical();
        }
    }
}
