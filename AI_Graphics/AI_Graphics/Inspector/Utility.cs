using BepInEx;
using BepInEx.Configuration;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using UnityEngine;

namespace AIGraphics.Inspector
{
    internal class Utility
    {
        public static void Slider(float value, float min, float max, string format, string label, Action<float> onChanged = null, bool enabled = true)
        {
            if (!enabled) GUI.enabled = false;
            GUILayout.BeginHorizontal();
            GUILayout.Label(label, GUILayout.ExpandWidth(false));
            GUILayout.Label("", GUILayout.Width(GUIStyles.labelWidth - GUI.skin.label.CalcSize(new GUIContent(label)).x));
            float newValue = GUILayout.HorizontalSlider(value, min, max);
            string valueString = newValue.ToString(format);
            string newValueString = GUILayout.TextField(valueString, GUILayout.Width(40), GUILayout.ExpandWidth(false));

            if (newValueString != valueString)
            {
                float parseResult;
                if (float.TryParse(newValueString, out parseResult))
                    newValue = Mathf.Clamp(parseResult, min, max);
            }
            GUILayout.EndHorizontal();

            if (onChanged != null && !Mathf.Approximately(value, newValue))
                onChanged(newValue);

            if (!enabled) GUI.enabled = true;
        }

        public static void Slider(int value, int min, int max, string format, string label, Action<int> onChanged = null, bool enabled = true)
        {
            if (!enabled) GUI.enabled = false;
            GUILayout.BeginHorizontal();
            GUILayout.Label(label, GUILayout.ExpandWidth(false));
            GUILayout.Label("", GUILayout.Width(GUIStyles.labelWidth - GUI.skin.label.CalcSize(new GUIContent(label)).x));
            int newValue = (int)GUILayout.HorizontalSlider(value, min, max);
            string valueString = newValue.ToString(format);
            string newValueString = GUILayout.TextField(valueString, GUILayout.Width(40), GUILayout.ExpandWidth(false));

            if (newValueString != valueString)
            {
                int parseResult;
                if (int.TryParse(newValueString, out parseResult))
                    newValue = Mathf.Clamp(parseResult, min, max);
            }
            GUILayout.EndHorizontal();

            if (onChanged != null && !Mathf.Approximately(value, newValue))
                onChanged(newValue);

            if (!enabled) GUI.enabled = true;
        }

        // useColorDisplayColor32 is for setting colour on skybox tint, Color<->Color32 conversion loses precision
        public static void SliderColor(Color value, Action<Color> onChanged = null, bool useColorDisplayColor32 = false, bool enabled = true)
        {
            if (!enabled) GUI.enabled = false;
            GUI.color = new Color(value.r, value.g, value.b, 1f);
            GUILayout.Label(new Texture2D(32, 16, TextureFormat.RGB24, false, true));
            GUI.color = Color.white;
            GUILayout.BeginVertical();
            if (useColorDisplayColor32)
            {
                Color color = value;
                color.r = SliderColor(color.r);
                color.g = SliderColor(color.g);
                color.b = SliderColor(color.b);
                Color newValue = color;
                if (onChanged != null && value != newValue)
                    onChanged(newValue);
            }
            else
            {
                Color32 color = value;
                color.r = SliderColor(color.r);
                color.g = SliderColor(color.g);
                color.b = SliderColor(color.b);
                Color newValue = color;
                if (onChanged != null && value != newValue)
                    onChanged(newValue);
            }
            GUILayout.EndVertical();

            if (!enabled) GUI.enabled = true;
        }

        public static float SliderColor(float value)
        {
            GUILayout.BeginHorizontal();
            float newValue = GUILayout.HorizontalSlider(value, 0, 1);
            GUILayout.TextField((newValue * 255).ToString("N0"), GUIStyles.Skin.label, GUILayout.Width(40), GUILayout.ExpandWidth(false));
            GUILayout.EndHorizontal();
            return newValue;
        }

        public static byte SliderColor(byte value)
        {
            GUILayout.BeginHorizontal();
            byte newValue = (byte)GUILayout.HorizontalSlider(value, 0, 255);
            string valueString = value.ToString();
            string newValueString = GUILayout.TextField(newValue.ToString(), GUILayout.Width(40), GUILayout.ExpandWidth(false));
            GUILayout.EndHorizontal();

            if (newValueString != valueString)
            {
                byte parseResult;
                if (byte.TryParse(newValueString, out parseResult))
                    newValue = parseResult;
            }
            return newValue;
        }

        internal static void Selection<TEnum>(ref TEnum selected, string label, Action<TEnum> onChanged = null, int columns = 1)
        {
            selected = Selection(selected, label, onChanged, columns);
        }

        internal static TEnum Selection<TEnum>(TEnum selected, string label, Action<TEnum> onChanged = null, int columns = -1)
        {
            GUILayout.BeginHorizontal();
            GUILayout.Label(label, GUILayout.ExpandWidth(false));
            GUILayout.Label("", GUILayout.Width(GUIStyles.labelWidth - GUI.skin.label.CalcSize(new GUIContent(label)).x));
            string[] selection = Enum.GetNames(typeof(TEnum));
            int currentIndex = Array.IndexOf(selection, selected.ToString());
            if (-1 == columns) columns = selection.Length;
            int selectedIndex = GUILayout.SelectionGrid(currentIndex, selection, columns, GUIStyles.toolbarbutton);//, GUILayout.ExpandWidth(false));
            GUILayout.EndHorizontal();
            if (selectedIndex == currentIndex)
                return selected;
            string selectedName = selection.GetValue(selectedIndex).ToString();
            selected = (TEnum)Enum.Parse(typeof(TEnum), selectedName);
            onChanged?.Invoke(selected);
            return selected;
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

        internal static bool Toggle(string label, bool toggle)
        {
            GUILayout.BeginHorizontal();
            GUILayout.Label(label, GUILayout.ExpandWidth(false));
            GUILayout.Label("", GUILayout.Width(GUIStyles.labelWidth - GUI.skin.label.CalcSize(new GUIContent(label)).x));
            toggle = GUILayout.Toggle(toggle, "");
            GUILayout.EndHorizontal();
            return toggle;
        }

        internal static void Text(string label, string text)
        {
            GUILayout.BeginHorizontal();
            GUILayout.Label(label, GUILayout.ExpandWidth(false));
            GUILayout.Label("", GUILayout.Width(GUIStyles.labelWidth - GUI.skin.label.CalcSize(new GUIContent(label)).x));
            GUILayout.TextField(text);            
            GUILayout.EndHorizontal();            
        }

        internal static int Text(string label, int Integer)
        {
            GUILayout.BeginHorizontal();
            GUILayout.Label(label, GUILayout.ExpandWidth(false));
            GUILayout.Label("", GUILayout.Width(GUIStyles.labelWidth - GUI.skin.label.CalcSize(new GUIContent(label)).x));
            int.TryParse(GUILayout.TextField(Integer.ToString()), out int count);
            GUILayout.EndHorizontal();
            return count;
        }

        internal static float Text(string label, float Float, string format = "N0")
        {
            GUILayout.BeginHorizontal();
            GUILayout.Label(label, GUILayout.ExpandWidth(false));
            GUILayout.Label("", GUILayout.Width(GUIStyles.labelWidth - GUI.skin.label.CalcSize(new GUIContent(label)).x));
            float.TryParse(GUILayout.TextField(Float.ToString(format)), out float count);
            GUILayout.EndHorizontal();
            return count;
        }
    }
}
