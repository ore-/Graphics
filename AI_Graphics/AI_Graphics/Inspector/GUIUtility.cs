using System;
using System.Linq;
using UnityEngine;

namespace AIGraphics.Inspector
{
    internal class Util
    {
        private static Texture2D colourIndicator = new Texture2D(32, 16, TextureFormat.RGB24, false, true);
        private static int enableSpacing = 18;

        internal static void Slider(string label, float value, float min, float max, string format, Action<float> onChanged = null, bool enable = true, Action<bool> onChangedEnable = null)
        {
            GUILayout.BeginHorizontal();
            int spacing = 0;
            EnableToggle(label, ref spacing, ref enable, onChangedEnable);
            if (!enable) GUI.enabled = false;
            float newValue = GUILayout.HorizontalSlider(value, min, max);
            string valueString = newValue.ToString(format);
            string newValueString = GUILayout.TextField(valueString, GUILayout.Width(40), GUILayout.ExpandWidth(false));

            if (newValueString != valueString)
            {
                if (float.TryParse(newValueString, out float parseResult))
                    newValue = Mathf.Clamp(parseResult, min, max);
            }
            GUILayout.EndHorizontal();

            if (onChanged != null && !Mathf.Approximately(value, newValue))
                onChanged(newValue);

            if (!enable) GUI.enabled = true;
        }

        internal static void Slider(string label, int value, int min, int max, Action<int> onChanged = null, bool enable = true, Action<bool> onChangedEnable = null)
        {
            GUILayout.BeginHorizontal();
            int spacing = 0;
            EnableToggle(label, ref spacing, ref enable, onChangedEnable);
            if (!enable) GUI.enabled = false;            
            int newValue = (int)GUILayout.HorizontalSlider(value, min, max);
            string newValueString = GUILayout.TextField(newValue.ToString(), GUILayout.Width(40), GUILayout.ExpandWidth(false));

            if (newValueString != newValue.ToString())
            {
                if (int.TryParse(newValueString, out int parseResult))
                    newValue = Mathf.Clamp(parseResult, min, max);
            }
            GUILayout.EndHorizontal();

            if (onChanged != null && !Mathf.Approximately(value, newValue))
                onChanged(newValue);

            if (!enable) GUI.enabled = true;
        }

        // useColorDisplayColor32 is for setting colour on skybox tint, Color<->Color32 conversion loses precision
        internal static void SliderColor(string label, Color value, Action<Color> onChanged = null, bool useColorDisplayColor32 = false, bool enable = true, Action<bool> onChangedEnable = null)
        {
            GUILayout.BeginHorizontal();
            int spacing = 0;
            EnableToggle(label, ref spacing, ref enable, onChangedEnable);
            if (!enable) GUI.enabled = false;
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
                if (onChanged != null && value != newValue)
                    onChanged(newValue);
            }
            else
            {
                Color32 color = value;
                color.r = SliderColor("Red", color.r, spacing);
                color.g = SliderColor("Green", color.g, spacing);
                color.b = SliderColor("Blue", color.b, spacing);
                Color newValue = color;
                if (onChanged != null && value != newValue)
                    onChanged(newValue);
            }
            GUILayout.EndVertical();
            if (!enable) GUI.enabled = true;
        }

        internal static float SliderColor(string label, float value, int spacing)
        {
            GUILayout.BeginHorizontal();
            if( 0 != spacing) GUILayout.Label("", GUILayout.Width(spacing));
            GUILayout.Label(label, GUILayout.ExpandWidth(false));
            GUILayout.Label("", GUILayout.Width(GUIStyles.labelWidth - GUI.skin.label.CalcSize(new GUIContent(label)).x - spacing));
            float newValue = GUILayout.HorizontalSlider(value, 0, 1);
            string valueString = value.ToString();            
            string newValueString = GUILayout.TextField((newValue * 255).ToString("N0"), GUILayout.Width(40), GUILayout.ExpandWidth(false));
            if (newValueString != valueString)
            {
                if (float.TryParse(newValueString, out float parseResult))
                    newValue = parseResult / 255;
            }
            GUILayout.EndHorizontal();
            return newValue;
        }

        internal static byte SliderColor(string label, byte value, int spacing)
        {
            GUILayout.BeginHorizontal();
            if (0 != spacing) GUILayout.Label("", GUILayout.Width(spacing));
            GUILayout.Label(label, GUILayout.ExpandWidth(false));
            GUILayout.Label("", GUILayout.Width(GUIStyles.labelWidth - GUI.skin.label.CalcSize(new GUIContent(label)).x - spacing));
            byte newValue = (byte)GUILayout.HorizontalSlider(value, 0, 255);
            string valueString = value.ToString();
            string newValueString = GUILayout.TextField(newValue.ToString(), GUILayout.Width(40), GUILayout.ExpandWidth(false));
            GUILayout.EndHorizontal();
            if (newValueString != valueString)
            {
                if (byte.TryParse(newValueString, out byte parseResult))
                    newValue = parseResult;
            }
            return newValue;
        }

        internal static T Selection<T>(string label, T selected, T[] selection, Action<T> onChanged = null, int columns = -1, bool enable = true, Action<bool> onChangedEnable = null)
        {
            GUILayout.BeginHorizontal();
            int spacing = 0;
            EnableToggle(label, ref spacing, ref enable, onChangedEnable);
            if (!enable) GUI.enabled = false;
            string[] selectionString = selection.Select(entry => entry.ToString()).ToArray();
            int currentIndex = Array.IndexOf(selection, selected);
            if (-1 == columns) columns = selection.Length;
            int selectedIndex = GUILayout.SelectionGrid(currentIndex, selectionString, columns);
            if (!enable) GUI.enabled = true;
            GUILayout.EndHorizontal();
            if (selectedIndex == currentIndex)
                return selected;
            selected = (T)selection.GetValue(selectedIndex);            
            onChanged?.Invoke(selected);
            return selected;
        }

        internal static TEnum Selection<TEnum>(string label, TEnum selected, Action<TEnum> onChanged = null, int columns = -1, bool enable = true, Action<bool> onChangedEnable = null)
        {
            GUILayout.BeginHorizontal();
            int spacing = 0;
            EnableToggle(label, ref spacing, ref enable, onChangedEnable);
            if (!enable) GUI.enabled = false;
            string[] selection = Enum.GetNames(typeof(TEnum));
            int currentIndex = Array.IndexOf(selection, selected.ToString());
            if (-1 == columns) columns = selection.Length;
            int selectedIndex = GUILayout.SelectionGrid(currentIndex, selection, columns);
            GUILayout.EndHorizontal();
            if (selectedIndex == currentIndex)
                return selected;
            string selectedName = selection.GetValue(selectedIndex).ToString();
            selected = (TEnum)Enum.Parse(typeof(TEnum), selectedName);
            onChanged?.Invoke(selected);
            if (!enable) GUI.enabled = true;
            return selected;
        }

        internal static int SelectionTexture(string label, int currentIndex, Texture[] selection, int columns = -1, bool enable = true, Action<bool> onChangedEnable = null, GUIStyle style = null)
        {
            GUILayout.BeginHorizontal();
            int spacing = 0;
            EnableToggle(label, ref spacing, ref enable, onChangedEnable);
            if (!enable) GUI.enabled = false;
            if (-1 == columns) columns = selection.Length;
            int selectedIndex = null == style ? GUILayout.SelectionGrid(currentIndex, selection, columns) : GUILayout.SelectionGrid(currentIndex, selection, columns, style);
            if (!enable) GUI.enabled = true;
            GUILayout.EndHorizontal();
            return selectedIndex;
        }

        internal static TEnum Toolbar<TEnum>(TEnum selected)
        {
            GUILayout.BeginHorizontal();
            string[] selection = Enum.GetNames(typeof(TEnum));
            int currentIndex = Array.IndexOf(selection, selected.ToString());            
            int selectedIndex = GUILayout.Toolbar(currentIndex, selection, GUIStyles.toolbarbutton);
            GUILayout.EndHorizontal();
            if (selectedIndex == currentIndex)
                return selected;
            string selectedName = selection.GetValue(selectedIndex).ToString();
            selected = (TEnum)Enum.Parse(typeof(TEnum), selectedName);            
            return selected;
        }

        internal static bool Toggle(string label, bool toggle, bool bold = false)
        {
            GUILayout.BeginHorizontal();
            if (bold)
                GUILayout.Label(label, GUIStyles.boldlabel, GUILayout.ExpandWidth(false));
            else
                GUILayout.Label(label, GUILayout.ExpandWidth(false));
            GUILayout.Label("", GUILayout.Width(GUIStyles.labelWidth - GUI.skin.label.CalcSize(new GUIContent(label)).x));
            toggle = GUILayout.Toggle(toggle, "");
            GUILayout.EndHorizontal();
            return toggle;
        }

        internal static void Label(string label, string text)
        {
            GUILayout.BeginHorizontal();
            GUILayout.Label(label, GUILayout.ExpandWidth(false));
            GUILayout.Label("", GUILayout.Width(GUIStyles.labelWidth - GUI.skin.label.CalcSize(new GUIContent(label)).x));
            GUILayout.Label(text);
            GUILayout.EndHorizontal();
        }

        internal static int Text(string label, int Integer, bool enable = true, Action<bool> onChangedEnable = null)
        {
            GUILayout.BeginHorizontal();
            int spacing = 0;
            EnableToggle(label, ref spacing, ref enable, onChangedEnable);
            if (!enable) GUI.enabled = false;
            int.TryParse(GUILayout.TextField(Integer.ToString()), out int count);
            if (!enable) GUI.enabled = true;
            GUILayout.EndHorizontal();            
            return count;
        }

        internal static float Text(string label, float Float, string format = "N0", bool enable = true, Action<bool> onChangedEnable = null)
        {
            GUILayout.BeginHorizontal();
            int spacing = 0;
            EnableToggle(label, ref spacing, ref enable, onChangedEnable);
            if (!enable) GUI.enabled = false;
            float.TryParse(GUILayout.TextField(Float.ToString(format)), out float count);
            if (!enable) GUI.enabled = true;
            GUILayout.EndHorizontal();            
            return count;
        }

        internal static Vector3 Dimension(string label, Vector3 size, Action<Vector3> onChanged = null)
        {
            GUILayout.BeginHorizontal();
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

        private static void EnableToggle(string label, ref int spacing, ref bool enable, Action<bool> onChangedEnable = null)
        {
            bool newEnable = enable;
            if (onChangedEnable != null)
            {
                spacing = enableSpacing;
                newEnable = GUILayout.Toggle(enable, "", GUILayout.ExpandWidth(false));
            }
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
