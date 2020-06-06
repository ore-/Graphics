using System;
using System.Linq;
using UnityEngine;

namespace Graphics.Inspector
{
    internal class Util
    {
        private static readonly Texture2D colourIndicator = new Texture2D(32, 16, TextureFormat.RGB24, false, true);
        private static readonly int enableSpacing = 18;

        internal static void Slider(string label, float value, float min, float max, string format, Action<float> onChanged = null, bool enable = true, Action<bool> onChangedEnable = null)
        {
            GUILayout.BeginHorizontal();
            int spacing = 0;
            EnableToggle(label, ref spacing, ref enable, onChangedEnable);
            if (!enable)
            {
                GUI.enabled = false;
            }

            float newValue = GUILayout.HorizontalSlider(value, min, max);
            string valueString = newValue.ToString(format);
            string newValueString = GUILayout.TextField(valueString, GUILayout.Width(40), GUILayout.ExpandWidth(false));

            if (newValueString != valueString)
            {
                if (float.TryParse(newValueString, out float parseResult))
                {
                    newValue = Mathf.Clamp(parseResult, min, max);
                }
            }
            GUILayout.EndHorizontal();

            if (onChanged != null && !Mathf.Approximately(value, newValue))
            {
                onChanged(newValue);
            }

            if (!enable)
            {
                GUI.enabled = true;
            }
        }

        internal static void Slider(string label, int value, int min, int max, Action<int> onChanged = null, bool enable = true, Action<bool> onChangedEnable = null)
        {
            GUILayout.BeginHorizontal();
            int spacing = 0;
            EnableToggle(label, ref spacing, ref enable, onChangedEnable);
            if (!enable)
            {
                GUI.enabled = false;
            }

            int newValue = (int)GUILayout.HorizontalSlider(value, min, max);
            string newValueString = GUILayout.TextField(newValue.ToString(), GUILayout.Width(40), GUILayout.ExpandWidth(false));

            if (newValueString != newValue.ToString())
            {
                if (int.TryParse(newValueString, out int parseResult))
                {
                    newValue = Mathf.Clamp(parseResult, min, max);
                }
            }
            GUILayout.EndHorizontal();

            if (onChanged != null && !Mathf.Approximately(value, newValue))
            {
                onChanged(newValue);
            }

            if (!enable)
            {
                GUI.enabled = true;
            }
        }

        // useColorDisplayColor32 is for setting colour on skybox tint, Color<->Color32 conversion loses precision
        internal static void SliderColor(string label, Color value, Action<Color> onChanged = null,
            bool useColorDisplayColor32 = false, bool enable = true, Action<bool> onChangedEnable = null, string colourGradingName = "", float mincolourGrading = 0, float maxcolourGrading = 1)
        {
            GUILayout.BeginHorizontal();
            int spacing = 0;
            EnableToggle(label, ref spacing, ref enable, onChangedEnable);
            if (!enable)
            {
                GUI.enabled = false;
            }

            GUI.color = new Color(value.r, value.g, value.b, 1f);
            GUILayout.Label(colourIndicator);
            GUI.color = Color.white;
            GUILayout.EndHorizontal();
            GUILayout.BeginVertical();
            if (useColorDisplayColor32)
            {
                Color color = value;
                color.r = SliderColor("Red", color.r, spacing);
                color.g = SliderColor("Green", color.g, spacing);
                color.b = SliderColor("Blue", color.b, spacing);
                Color newValue = color;
                if (!colourGradingName.IsNullOrEmpty())
                {
                    float colourGradingValue = SliderColor(colourGradingName, color.a, spacing, mincolourGrading, maxcolourGrading, false);
                    newValue.a = colourGradingValue;
                }
                if (onChanged != null && value != newValue)
                {
                    onChanged(newValue);
                }
            }
            else
            {
                Color32 color = value;
                color.r = SliderColor("Red", color.r, spacing);
                color.g = SliderColor("Green", color.g, spacing);
                color.b = SliderColor("Blue", color.b, spacing);
                Color newValue = color;
                if (!colourGradingName.IsNullOrEmpty())
                {
                    float colourGradingValue = SliderColor(colourGradingName, value.a, spacing, mincolourGrading, maxcolourGrading, false);
                    newValue.a = colourGradingValue;
                }
                if (onChanged != null && value != newValue)
                {
                    onChanged(newValue);
                }
            }
            GUILayout.EndVertical();
            if (!enable)
            {
                GUI.enabled = true;
            }
        }

        internal static float SliderColor(string label, float value, int spacing, float min = 0, float max = 1, bool RGB = true)
        {
            GUILayout.BeginHorizontal();
            if (0 != spacing)
            {
                GUILayout.Label("", GUILayout.Width(spacing));
            }
            label = LocalizationManager.HasLocalization() ? LocalizationManager.Localized(label) : label;
            GUILayout.Label(label, GUILayout.ExpandWidth(false));
            GUILayout.Label("", GUILayout.Width(GUIStyles.labelWidth - GUI.skin.label.CalcSize(new GUIContent(label)).x - spacing));
            float newValue = GUILayout.HorizontalSlider(value, min, max);
            string valueString = value.ToString();
            string newValueString = RGB ? (newValue * 255).ToString("N0") : newValue.ToString();
            newValueString = GUILayout.TextField(newValueString, GUILayout.Width(40), GUILayout.ExpandWidth(false));
            if (newValueString != valueString)
            {
                if (float.TryParse(newValueString, out float parseResult))
                {
                    newValue = RGB ? parseResult / 255 : parseResult;
                }
            }
            GUILayout.EndHorizontal();
            return newValue;
        }

        internal static byte SliderColor(string label, byte value, int spacing)
        {
            GUILayout.BeginHorizontal();
            if (0 != spacing)
            {
                GUILayout.Label("", GUILayout.Width(spacing));
            }
            label = LocalizationManager.HasLocalization() ? LocalizationManager.Localized(label) : label;
            GUILayout.Label(label, GUILayout.ExpandWidth(false));
            GUILayout.Label("", GUILayout.Width(GUIStyles.labelWidth - GUI.skin.label.CalcSize(new GUIContent(label)).x - spacing));
            byte newValue = (byte)GUILayout.HorizontalSlider(value, 0, 255);
            string valueString = value.ToString();
            string newValueString = GUILayout.TextField(newValue.ToString(), GUILayout.Width(40), GUILayout.ExpandWidth(false));
            GUILayout.EndHorizontal();
            if (newValueString != valueString)
            {
                if (byte.TryParse(newValueString, out byte parseResult))
                {
                    newValue = parseResult;
                }
            }
            return newValue;
        }

        internal static T Selection<T>(string label, T selected, T[] selection, Action<T> onChanged = null, int columns = -1, bool enable = true, Action<bool> onChangedEnable = null)
        {
            GUILayout.BeginHorizontal();
            int spacing = 0;
            EnableToggle(label, ref spacing, ref enable, onChangedEnable);
            if (!enable)
            {
                GUI.enabled = false;
            }

            string[] selectionString = selection.Select(entry => entry.ToString()).ToArray();
            string[] localizedSelection = LocalizationManager.HasLocalization() ? selectionString.Select(text => LocalizationManager.Localized(text)).ToArray() : selectionString;
            int currentIndex = Array.IndexOf(selection, selected);
            if (-1 == columns)
            {
                columns = selection.Length;
            }

            int selectedIndex = GUILayout.SelectionGrid(currentIndex, localizedSelection, columns);
            if (!enable)
            {
                GUI.enabled = true;
            }

            GUILayout.EndHorizontal();
            if (selectedIndex == currentIndex)
            {
                return selected;
            }

            selected = (T)selection.GetValue(selectedIndex);
            onChanged?.Invoke(selected);
            return selected;
        }

        internal static TEnum Selection<TEnum>(string label, TEnum selected, Action<TEnum> onChanged = null, int columns = -1, bool enable = true, Action<bool> onChangedEnable = null)
        {
            GUILayout.BeginHorizontal();
            int spacing = 0;
            EnableToggle(label, ref spacing, ref enable, onChangedEnable);
            if (!enable)
            {
                GUI.enabled = false;
            }

            string[] selection = Enum.GetNames(typeof(TEnum));
            string[] localizedSelection = LocalizationManager.HasLocalization() ? selection.Select(text => LocalizationManager.Localized(text)).ToArray() : selection;

            int currentIndex = Array.IndexOf(selection, selected.ToString());
            if (-1 == columns)
            {
                columns = selection.Length;
            }

            int selectedIndex = GUILayout.SelectionGrid(currentIndex, localizedSelection, columns);
            GUILayout.EndHorizontal();
            if (selectedIndex == currentIndex)
            {
                return selected;
            }

            string selectedName = selection.GetValue(selectedIndex).ToString();
            selected = (TEnum)Enum.Parse(typeof(TEnum), selectedName);
            onChanged?.Invoke(selected);
            if (!enable)
            {
                GUI.enabled = true;
            }

            return selected;
        }

        internal static int SelectionTexture(string label, int currentIndex, Texture[] selection, int columns = -1, bool enable = true, Action<bool> onChangedEnable = null, GUIStyle style = null)
        {
            GUILayout.BeginHorizontal();
            int spacing = 0;
            EnableToggle(label, ref spacing, ref enable, onChangedEnable);
            if (!enable)
            {
                GUI.enabled = false;
            }

            if (-1 == columns)
            {
                columns = selection.Length;
            }

            int selectedIndex = null == style ? GUILayout.SelectionGrid(currentIndex, selection, columns) : GUILayout.SelectionGrid(currentIndex, selection, columns, style);
            if (!enable)
            {
                GUI.enabled = true;
            }

            GUILayout.EndHorizontal();
            return selectedIndex;
        }

        internal static void SelectionMask(string label, int cullingMask, Action<int> onChanged = null, int columns = 4)
        {
            label = LocalizationManager.HasLocalization() ? LocalizationManager.Localized(label) : label;
            int newMask = cullingMask;
            GUILayout.BeginHorizontal();
            GUILayout.Label(label, GUILayout.ExpandWidth(false));
            GUILayout.Label("", GUILayout.Width(GUIStyles.labelWidth - GUI.skin.label.CalcSize(new GUIContent(label)).x));
            int included = 0;
            GUILayout.BeginVertical();
            for (int i = 0; i < CullingMaskExtensions.Layers.Count; i++)
            {
                string layer = CullingMaskExtensions.Layers[i];
                bool include = CullingMaskExtensions.LayerCullingIncludes(newMask, layer);
                if (0 == (i % columns))
                    GUILayout.BeginHorizontal();
                include = GUILayout.Toggle(include, layer, GUIStyles.Skin.button);
                included++;
                newMask = CullingMaskExtensions.LayerCullingToggle(newMask, layer, include);
                if (included == columns)
                {
                    GUILayout.EndHorizontal();
                    included = 0;
                }
            }
            if (0 != included)
                GUILayout.EndHorizontal();
            GUILayout.EndVertical();
            GUILayout.EndHorizontal();
            if (newMask != cullingMask)
                onChanged(newMask);
        }

        internal static TEnum Toolbar<TEnum>(TEnum selected)
        {
            GUILayout.BeginHorizontal();
            string[] selection = Enum.GetNames(typeof(TEnum));
            string[] localizedSelection = LocalizationManager.HasLocalization() ? selection.Select(text => LocalizationManager.Localized(text)).ToArray() : selection;
            int currentIndex = Array.IndexOf(selection, selected.ToString());
            int selectedIndex = GUILayout.Toolbar(currentIndex, localizedSelection, GUIStyles.toolbarbutton);
            GUILayout.EndHorizontal();
            if (selectedIndex == currentIndex)
            {
                return selected;
            }

            string selectedName = selection.GetValue(selectedIndex).ToString();
            selected = (TEnum)Enum.Parse(typeof(TEnum), selectedName);
            return selected;
        }

        internal static void Toggle(string label, bool toggle, bool bold = false, Action<bool> onChanged = null)
        {
            label = LocalizationManager.HasLocalization() ? LocalizationManager.Localized(label) : label;
            GUILayout.BeginHorizontal();
            if (bold)
            {
                GUILayout.Label(label, GUIStyles.boldlabel, GUILayout.ExpandWidth(false));
                GUILayout.Label("", GUILayout.Width(GUIStyles.labelWidth - GUIStyles.boldlabel.CalcSize(new GUIContent(label)).x));
            }
            else
            {
                GUILayout.Label(label, GUILayout.ExpandWidth(false));
                GUILayout.Label("", GUILayout.Width(GUIStyles.labelWidth - GUI.skin.label.CalcSize(new GUIContent(label)).x));
            }
            bool newToggle = GUILayout.Toggle(toggle, "");
            GUILayout.EndHorizontal();
            if (onChanged != null && toggle != newToggle)
            {
                onChanged(newToggle);
            }
        }

        internal static bool ToggleButton(string label, bool toggle, bool UseButton = false)
        {
            label = LocalizationManager.HasLocalization() ? LocalizationManager.Localized(label) : label;
            return UseButton ? GUILayout.Toggle(toggle, label, GUIStyles.Skin.button) : GUILayout.Toggle(toggle, label);
        }

        internal static bool Button(string label, bool ExpandWidth = false)
        {
            label = LocalizationManager.HasLocalization() ? LocalizationManager.Localized(label) : label;
            return GUILayout.Button(label, GUILayout.ExpandWidth(ExpandWidth));
        }

        internal static void Label(string label, string text, bool bold = false)
        {
            label = LocalizationManager.HasLocalization() ? LocalizationManager.Localized(label) : label;
            if (0 < text.Length) text = LocalizationManager.HasLocalization() ? LocalizationManager.Localized(text) : text;
            GUILayout.BeginHorizontal();
            if (bold)
            {
                GUILayout.Label(label, GUIStyles.boldlabel, GUILayout.ExpandWidth(false));
            }
            else
            {
                GUILayout.Label(label, GUILayout.ExpandWidth(false));
            }
            GUILayout.Label("", GUILayout.Width(GUIStyles.labelWidth - GUI.skin.label.CalcSize(new GUIContent(label)).x));
            GUILayout.Label(text);
            GUILayout.EndHorizontal();
        }

        internal static void Text(string label, int Integer, Action<int> onChanged = null, bool enable = true, Action<bool> onChangedEnable = null)
        {
            GUILayout.BeginHorizontal();
            int spacing = 0;
            EnableToggle(label, ref spacing, ref enable, onChangedEnable);
            if (!enable)
            {
                GUI.enabled = false;
            }

            int.TryParse(GUILayout.TextField(Integer.ToString()), out int count);
            if (!enable)
            {
                GUI.enabled = true;
            }

            GUILayout.EndHorizontal();
            if (onChanged != null && Integer != count)
            {
                onChanged(count);
            }
        }

        internal static void Text(string label, float Float, string format = "N0", Action<float> onChanged = null, bool enable = true, Action<bool> onChangedEnable = null)
        {
            GUILayout.BeginHorizontal();
            int spacing = 0;
            EnableToggle(label, ref spacing, ref enable, onChangedEnable);
            if (!enable)
            {
                GUI.enabled = false;
            }

            float.TryParse(GUILayout.TextField(Float.ToString(format)), out float count);
            if (!enable)
            {
                GUI.enabled = true;
            }

            GUILayout.EndHorizontal();
            if (onChanged != null && Float != count)
            {
                onChanged(count);
            }
        }

        internal static void Text(string label, string text, Action<string> onChanged = null, bool enable = true, Action<bool> onChangedEnable = null)
        {
            GUILayout.BeginHorizontal();
            int spacing = 0;
            EnableToggle(label, ref spacing, ref enable, onChangedEnable);
            if (!enable)
            {
                GUI.enabled = false;
            }
            string newText = GUILayout.TextField(text, 32);
            if (!enable)
            {
                GUI.enabled = true;
            }

            GUILayout.EndHorizontal();
            if (onChanged != null && text != newText)
            {
                onChanged(newText);
            }
        }

        internal static Vector3 Dimension(string label, Vector3 size, Action<Vector3> onChanged = null)
        {
            GUILayout.BeginHorizontal();
            label = LocalizationManager.HasLocalization() ? LocalizationManager.Localized(label) : label;
            GUILayout.Label(label, GUILayout.ExpandWidth(false));
            GUILayout.Label("", GUILayout.Width(GUIStyles.labelWidth - GUI.skin.label.CalcSize(new GUIContent(label)).x));
            GUILayout.Label("X", GUILayout.ExpandWidth(false));
            float.TryParse(GUILayout.TextField(size.x.ToString()), out float x);
            GUILayout.Label("Y", GUILayout.ExpandWidth(false));
            float.TryParse(GUILayout.TextField(size.y.ToString()), out float y);
            GUILayout.Label("Z", GUILayout.ExpandWidth(false));
            float.TryParse(GUILayout.TextField(size.z.ToString()), out float z);
            Vector3 newSize = size;
            if (x != size.x || y != size.y || z != size.z)
            {
                newSize = new Vector3(x, y, z);
                onChanged?.Invoke(newSize);
            }
            GUILayout.EndHorizontal();
            return newSize;
        }

        internal static Vector4 Dimension(string label, Vector4 size, Action<Vector4> onChanged = null)
        {
            GUILayout.BeginHorizontal();
            label = LocalizationManager.HasLocalization() ? LocalizationManager.Localized(label) : label;
            GUILayout.Label(label, GUILayout.ExpandWidth(false));
            GUILayout.Label("", GUILayout.Width(GUIStyles.labelWidth - GUI.skin.label.CalcSize(new GUIContent(label)).x));
            GUILayout.Label("X", GUILayout.ExpandWidth(false));
            float.TryParse(GUILayout.TextField(size.x.ToString()), out float x);
            GUILayout.Label("Y", GUILayout.ExpandWidth(false));
            float.TryParse(GUILayout.TextField(size.y.ToString()), out float y);
            GUILayout.Label("Z", GUILayout.ExpandWidth(false));
            float.TryParse(GUILayout.TextField(size.z.ToString()), out float z);
            GUILayout.Label("W", GUILayout.ExpandWidth(false));
            float.TryParse(GUILayout.TextField(size.w.ToString()), out float w);
            Vector4 newSize = size;
            if (x != size.x || y != size.y || z != size.z || w != size.w)
            {
                newSize = new Vector4(x, y, z, w);
                onChanged?.Invoke(newSize);
            }
            GUILayout.EndHorizontal();
            return newSize;
        }

        private static void EnableToggle(string label, ref int spacing, ref bool enable, Action<bool> onChangedEnable = null)
        {
            bool newEnable = enable;
            if (onChangedEnable != null)
            {
                spacing = enableSpacing;
                newEnable = GUILayout.Toggle(enable, "", GUILayout.ExpandWidth(false));
            }
            label = LocalizationManager.HasLocalization() ? LocalizationManager.Localized(label) : label;
            GUILayout.Label(label, GUILayout.ExpandWidth(false));
            GUILayout.Label("", GUILayout.Width(GUIStyles.labelWidth - GUI.skin.label.CalcSize(new GUIContent(label)).x - spacing));
            if (onChangedEnable != null && newEnable != enable)
            {
                onChangedEnable(newEnable);
                enable = newEnable;
            }
        }
    }
}
