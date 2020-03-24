using AIGraphics.Settings;
using BepInEx;
using BepInEx.Configuration;
using Studio;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.SceneManagement;
using UnityEngine.Serialization;
using static AIGraphics.Inspector.Utility;

namespace AIGraphics.Inspector
{
    internal class Inspector
    {
        private Rect _windowRect;
        private int _windowID = 0;
        private enum Tab { Lighting, Lights, PostProcessing, Settings };
        private Tab SelectedTab { get; set; }
        private const float _width = 650f;
        private const float _height = 1024f;
        private float _startOffsetX;
        private float _startOffsetY;
        private KeyCode _hotKey;
        private bool _showGUI;

        private CursorLockMode _previousCursorLockState;
        private bool _previousCursorVisible;

        private CameraSettings _cameraSettings;
        private Settings.RenderSettings _renderSettings;

        internal Inspector(KeyCode keyCode)
        {
            _hotKey = keyCode;
            _startOffsetX = (Screen.width - _width) / 2;
            _startOffsetY = (Screen.height - _height) / 2;
            _windowRect = new Rect(_startOffsetX, _startOffsetY, _width, _height);
            _cameraSettings = new CameraSettings();
            _renderSettings = new Settings.RenderSettings();
        }

        internal void OnGUI()
        {
            if (Show)
            {
                Cursor.lockState = CursorLockMode.None;
                Cursor.visible = true;

                var originalSkin = GUI.skin;
                GUI.skin = GUIStyles.Skin;
                _windowRect = GUILayout.Window(_windowID, _windowRect, WindowFunction, "");
                EatInputInRect(_windowRect);
                GUI.skin = originalSkin;
            }
        }

        internal void Update()
        {
            if (Input.GetKeyDown(_hotKey))
                Show = !Show;

            if (Show)
            {
               Cursor.lockState = CursorLockMode.None;
               Cursor.visible = true;
            }
        }

        internal void LateUpdate()
        {
            if (Show)
            {
                Cursor.lockState = CursorLockMode.None;
                Cursor.visible = true;
            }
        }

        private void WindowFunction(int thisWindowID)
        {
            GUILayout.BeginVertical("box");
            SelectedTab = Toolbar(SelectedTab);
            DrawTabs(SelectedTab);
            GUILayout.FlexibleSpace();
            GUILayout.EndVertical();
            GUI.DragWindow();
        }

        private void DrawTabs(Tab tabSelected)
        {
            GUILayout.Space(10);
            switch (tabSelected)
            {
                case Tab.Lighting:
                    //LightingModule();
                    break;
                case Tab.Lights:
                    //LightModule();
                    break;
                case Tab.PostProcessing:
                    //PostProcessModule();
                    break;
                case Tab.Settings:
                    SettingsModule.CameraInspector(_cameraSettings, _renderSettings);
                    break;
            }
        }

        private static void EatInputInRect(Rect eatRect)
        {
            if (eatRect.Contains(new Vector2(Input.mousePosition.x, Screen.height - Input.mousePosition.y)))
                Input.ResetInputAxes();
        }

        private bool Show
        {
            get => _showGUI;
            set
            {
                if (_showGUI != value)
                {
                    if (value)
                    {
                        _previousCursorLockState = Cursor.lockState;
                        _previousCursorVisible = Cursor.visible;
                    }
                    else
                    {
                        if (!_previousCursorVisible || _previousCursorLockState != CursorLockMode.None)
                        {
                            Cursor.lockState = _previousCursorLockState;
                            Cursor.visible = _previousCursorVisible;
                        }
                    }
                    _showGUI = value;
                }
            }
        }
    }
}
