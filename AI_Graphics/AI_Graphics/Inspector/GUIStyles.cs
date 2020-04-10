using KKAPI.Utilities;
using System;
using UnityEngine;
using Object = UnityEngine.Object;

namespace AIGraphics.Inspector
{
    internal static class GUIStyles
    {
        private static readonly Texture2D _boxNormalBackground = new Texture2D(1, 1, TextureFormat.ARGB32, false);
        private static readonly Texture2D _winNormalBackground = new Texture2D(1, 1, TextureFormat.ARGB32, false);
        private static readonly Texture2D _winOnNormalBackground = new Texture2D(1, 1, TextureFormat.ARGB32, false);
        private static readonly Texture2D _btnNormalBackground = new Texture2D(1, 1, TextureFormat.ARGB32, false);
        private static readonly Texture2D _btnOnNormalBackground = new Texture2D(1, 1, TextureFormat.ARGB32, false);
        private static readonly Texture2D _btnActiveBackground = new Texture2D(1, 1, TextureFormat.ARGB32, false);
        private static readonly Texture2D _btnOnActiveBackground = new Texture2D(1, 1, TextureFormat.ARGB32, false);
        private static readonly Texture2D _btnFocusedBackground = new Texture2D(1, 1, TextureFormat.ARGB32, false);
        private static readonly Texture2D _btnOnFocusedBackground = new Texture2D(1, 1, TextureFormat.ARGB32, false);
        private static readonly Texture2D _sliderHNormalBackground = new Texture2D(1, 1, TextureFormat.ARGB32, false);
        private static readonly Texture2D _sliderVNormalBackground = new Texture2D(1, 1, TextureFormat.ARGB32, false);
        private static readonly Texture2D _sliderThumbNormalBackground = new Texture2D(1, 1, TextureFormat.ARGB32, false);
        private static readonly Texture2D _sliderThumbActiveBackground = new Texture2D(1, 1, TextureFormat.ARGB32, false);
        private static readonly Texture2D _sliderThumbFocusedBackground = new Texture2D(1, 1, TextureFormat.ARGB32, false);
        private static readonly Texture2D _scrollHNormalBackground = new Texture2D(1, 1, TextureFormat.ARGB32, false);
        private static readonly Texture2D _scrollHLNormalBackground = new Texture2D(1, 1, TextureFormat.ARGB32, false);
        private static readonly Texture2D _scrollHLActiveBackground = new Texture2D(1, 1, TextureFormat.ARGB32, false);
        private static readonly Texture2D _scrollHRNormalBackground = new Texture2D(1, 1, TextureFormat.ARGB32, false);
        private static readonly Texture2D _scrollHRActiveBackground = new Texture2D(1, 1, TextureFormat.ARGB32, false);
        private static readonly Texture2D _scrollVNormalBackground = new Texture2D(1, 1, TextureFormat.ARGB32, false);
        private static readonly Texture2D _scrollVUNormalBackground = new Texture2D(1, 1, TextureFormat.ARGB32, false);
        private static readonly Texture2D _scrollVUActiveBackground = new Texture2D(1, 1, TextureFormat.ARGB32, false);
        private static readonly Texture2D _scrollVDNormalBackground = new Texture2D(1, 1, TextureFormat.ARGB32, false);
        private static readonly Texture2D _scrollVDActiveBackground = new Texture2D(1, 1, TextureFormat.ARGB32, false);
        private static readonly Texture2D _scrollHTNormalBackground = new Texture2D(1, 1, TextureFormat.ARGB32, false);
        private static readonly Texture2D _scrollVTNormalBackground = new Texture2D(1, 1, TextureFormat.ARGB32, false);
        private static readonly Texture2D _toggleNormalBackground = new Texture2D(1, 1, TextureFormat.ARGB32, false);
        private static readonly Texture2D _toggleOnNormalBackground = new Texture2D(1, 1, TextureFormat.ARGB32, false);
        private static readonly Texture2D _toggleActiveBackground = new Texture2D(1, 1, TextureFormat.ARGB32, false);
        private static readonly Texture2D _toggleOnActiveBackground = new Texture2D(1, 1, TextureFormat.ARGB32, false);
        private static readonly Texture2D _toggleFocusedBackground = new Texture2D(1, 1, TextureFormat.ARGB32, false);
        private static readonly Texture2D _toggleOnFocusedBackground = new Texture2D(1, 1, TextureFormat.ARGB32, false);
        private static readonly Texture2D _textNormalBackground = new Texture2D(1, 1, TextureFormat.ARGB32, false);
        private static readonly Texture2D _textFocusedBackground = new Texture2D(1, 1, TextureFormat.ARGB32, false);
        private static readonly Texture2D _toolbarbtnNormalBackground = new Texture2D(1, 1, TextureFormat.ARGB32, false);
        private static readonly Texture2D _toolbarbtnOnNormalBackground = new Texture2D(1, 1, TextureFormat.ARGB32, false);
        private static readonly Texture2D _toolbarbtnActiveBackground = new Texture2D(1, 1, TextureFormat.ARGB32, false);
        private static readonly Texture2D _toolbarbtnOnActiveBackground = new Texture2D(1, 1, TextureFormat.ARGB32, false);

        private static GUISkin _skin;
        private static int fontSize = 12;
        private static readonly string[] fonts = new string[] { "Lucida Grande", "Segoe UI", "Terminal" };

        public static GUIStyle toolbarbutton;
        public static GUIStyle boldlabel;
        public static float labelWidth = fontSize * 15f;

        public static GUISkin Skin
        {
            get
            {
                if (_skin == null)
                {
                    try
                    {
                        _skin = CreateSkin();
                    }
                    catch (Exception ex)
                    {
                        Debug.Log("Could not load custom GUISkin - " + ex.Message);
                        _skin = GUI.skin;
                    }
                }

                return _skin;
            }
        }

        public static int FontSize
        {
            get => fontSize;
            set
            {
                // Need to limit fontSize to prevent permantly breaking GUI from loading preset values.
                fontSize = Math.Max(Math.Min(17, value), 12);
                labelWidth = fontSize * 15f;
                Font font = Font.CreateDynamicFontFromOSFont(fonts, fontSize);
                if (_skin != null)
                {
                    _skin.font = font;
                }
            }
        }

        private static void LoadImage(Texture2D texture, byte[] tex)
        {
            ImageConversion.LoadImage(texture, tex);
            texture.anisoLevel = 1;
            texture.filterMode = FilterMode.Point;
            texture.wrapMode = TextureWrapMode.Clamp;
        }

        private static GUISkin CreateSkin()
        {
            GUISkin newSkin = Object.Instantiate(GUI.skin);
            Object.DontDestroyOnLoad(newSkin);
            Font font = Font.CreateDynamicFontFromOSFont(fonts, fontSize);
            newSkin.font = font;

            // Load the custom skin from resources
            byte[] texData = ResourceUtils.GetEmbeddedResource("box.png");
            LoadImage(_boxNormalBackground, texData);
            Object.DontDestroyOnLoad(_boxNormalBackground);
            newSkin.box.normal.background = _boxNormalBackground;
            newSkin.box.normal.textColor = Color.black;
            newSkin.box.border = new RectOffset(3, 3, 2, 2);
            newSkin.box.margin = new RectOffset(4, 4, 4, 4);
            newSkin.box.padding = new RectOffset(3, 3, 3, 3);
            newSkin.box.overflow = new RectOffset(0, 0, 0, 0);
            newSkin.box.font = null;
            newSkin.box.fontSize = 0;
            newSkin.box.fontStyle = FontStyle.Normal;
            newSkin.box.alignment = TextAnchor.UpperCenter;
            newSkin.box.wordWrap = true;
            newSkin.box.richText = false;
            newSkin.box.clipping = TextClipping.Clip;
            newSkin.box.imagePosition = ImagePosition.ImageLeft;
            newSkin.box.contentOffset = new Vector2(0, 0);
            newSkin.box.fixedWidth = 0;
            newSkin.box.fixedHeight = 0;
            newSkin.box.stretchWidth = false;
            newSkin.box.stretchHeight = false;

            texData = ResourceUtils.GetEmbeddedResource("PopupWindowOff.png");
            LoadImage(_winNormalBackground, texData);
            Object.DontDestroyOnLoad(_winNormalBackground);
            texData = ResourceUtils.GetEmbeddedResource("PopupWindowOn.png");
            LoadImage(_winOnNormalBackground, texData);
            Object.DontDestroyOnLoad(_winOnNormalBackground);
            newSkin.window.normal.background = _winNormalBackground;
            newSkin.window.normal.textColor = new Color32(180, 180, 180, 255);
            newSkin.window.onNormal.background = _winOnNormalBackground;
            newSkin.window.onNormal.textColor = new Color32(180, 180, 180, 255);
            newSkin.window.border = new RectOffset(11, 11, 22, 15);
            newSkin.window.margin = new RectOffset(0, 0, 0, 0);
            newSkin.window.padding = new RectOffset(5, 5, 20, 5);
            newSkin.window.overflow = new RectOffset(8, 8, 5, 12);
            newSkin.window.font = font;
            newSkin.window.fontSize = 0;
            newSkin.window.fontStyle = FontStyle.Normal;
            newSkin.window.alignment = TextAnchor.UpperCenter;
            newSkin.window.wordWrap = false;
            newSkin.window.richText = false;
            newSkin.window.clipping = TextClipping.Clip;
            newSkin.window.imagePosition = ImagePosition.ImageLeft;
            newSkin.window.contentOffset = new Vector2(0, -18);
            newSkin.window.fixedWidth = 0;
            newSkin.window.fixedHeight = 0;
            newSkin.window.stretchWidth = true;
            newSkin.window.stretchHeight = true;

            texData = ResourceUtils.GetEmbeddedResource("toggle.png");
            LoadImage(_toggleNormalBackground, texData);
            Object.DontDestroyOnLoad(_toggleNormalBackground);
            texData = ResourceUtils.GetEmbeddedResource("toggle on.png");
            LoadImage(_toggleOnNormalBackground, texData);
            Object.DontDestroyOnLoad(_toggleOnNormalBackground);
            texData = ResourceUtils.GetEmbeddedResource("toggle act.png");
            LoadImage(_toggleActiveBackground, texData);
            Object.DontDestroyOnLoad(_toggleActiveBackground);
            texData = ResourceUtils.GetEmbeddedResource("toggle on act.png");
            LoadImage(_toggleOnActiveBackground, texData);
            Object.DontDestroyOnLoad(_toggleOnActiveBackground);
            texData = ResourceUtils.GetEmbeddedResource("toggle focus.png");
            LoadImage(_toggleFocusedBackground, texData);
            Object.DontDestroyOnLoad(_toggleFocusedBackground);
            texData = ResourceUtils.GetEmbeddedResource("toggle on focus.png");
            LoadImage(_toggleOnFocusedBackground, texData);
            Object.DontDestroyOnLoad(_toggleOnFocusedBackground);
            newSkin.toggle.normal.background = _toggleNormalBackground;
            newSkin.toggle.normal.textColor = new Color32(180, 180, 180, 255);
            newSkin.toggle.onNormal.background = _toggleOnNormalBackground;
            newSkin.toggle.onNormal.textColor = new Color32(180, 180, 180, 255);
            newSkin.toggle.hover.background = null;
            newSkin.toggle.hover.textColor = new Color32(180, 180, 180, 255);
            newSkin.toggle.onHover.background = null;
            newSkin.toggle.onHover.textColor = new Color32(180, 180, 180, 255);
            newSkin.toggle.active.background = _toggleActiveBackground;
            newSkin.toggle.active.textColor = new Color32(180, 180, 180, 255);
            newSkin.toggle.onActive.background = _toggleOnActiveBackground;
            newSkin.toggle.onActive.textColor = new Color32(180, 180, 180, 255);
            newSkin.toggle.focused.background = _toggleFocusedBackground;
            newSkin.toggle.focused.textColor = new Color32(180, 180, 180, 255);
            newSkin.toggle.onFocused.background = _toggleOnFocusedBackground;
            newSkin.toggle.onFocused.textColor = new Color32(180, 180, 180, 255);
            newSkin.toggle.border = new RectOffset(16, 0, 15, 0);
            newSkin.toggle.margin = new RectOffset(4, 4, 2, 2);//new RectOffset(4, 4, 3, 2);
            newSkin.toggle.padding = new RectOffset(15, 3, 1, 2);
            newSkin.toggle.overflow = new RectOffset(0, 0, -3, 1);
            newSkin.toggle.alignment = TextAnchor.UpperLeft;
            newSkin.toggle.wordWrap = false;
            newSkin.toggle.richText = false;
            newSkin.toggle.clipping = TextClipping.Clip;
            newSkin.toggle.imagePosition = ImagePosition.ImageLeft;
            newSkin.toggle.contentOffset = new Vector2(0, 0);
            newSkin.toggle.fixedWidth = 0;
            newSkin.toggle.fixedHeight = 0;
            newSkin.toggle.stretchWidth = true;
            newSkin.toggle.stretchHeight = false;

            texData = ResourceUtils.GetEmbeddedResource("btn.png");
            LoadImage(_btnNormalBackground, texData);
            Object.DontDestroyOnLoad(_btnNormalBackground);
            texData = ResourceUtils.GetEmbeddedResource("btn on.png");
            LoadImage(_btnOnNormalBackground, texData);
            Object.DontDestroyOnLoad(_btnOnNormalBackground);
            texData = ResourceUtils.GetEmbeddedResource("btn act.png");
            LoadImage(_btnActiveBackground, texData);
            Object.DontDestroyOnLoad(_btnActiveBackground);
            texData = ResourceUtils.GetEmbeddedResource("btn onact.png");
            LoadImage(_btnOnActiveBackground, texData);
            Object.DontDestroyOnLoad(_btnOnActiveBackground);
            texData = ResourceUtils.GetEmbeddedResource("btn focus.png");
            LoadImage(_btnFocusedBackground, texData);
            Object.DontDestroyOnLoad(_btnFocusedBackground);
            texData = ResourceUtils.GetEmbeddedResource("btn on focus.png");
            LoadImage(_btnOnFocusedBackground, texData);
            Object.DontDestroyOnLoad(_btnOnFocusedBackground);
            newSkin.button.normal.background = _btnNormalBackground;
            newSkin.button.normal.textColor = new Color32(180, 180, 180, 255);
            newSkin.button.hover.background = null;
            newSkin.button.hover.textColor = Color.white;
            newSkin.button.onHover.background = null;
            newSkin.button.onHover.textColor = Color.white;
            newSkin.button.onNormal.background = _btnOnNormalBackground;
            newSkin.button.onNormal.textColor = new Color32(240, 240, 240, 255);
            newSkin.button.active.background = _btnActiveBackground;
            newSkin.button.active.textColor = new Color32(180, 180, 180, 255);
            newSkin.button.onActive.background = _btnOnActiveBackground;
            newSkin.button.onActive.textColor = new Color32(180, 180, 180, 255);
            newSkin.button.focused.background = _btnFocusedBackground;
            newSkin.button.focused.textColor = new Color32(180, 180, 180, 255);
            newSkin.button.onFocused.background = _btnFocusedBackground;
            newSkin.button.onFocused.textColor = new Color32(180, 180, 180, 255);
            newSkin.button.border = new RectOffset(6, 6, 4, 4);
            newSkin.button.margin = new RectOffset(4, 4, 3, 3);
            newSkin.button.padding = new RectOffset(6, 6, 2, 3);
            newSkin.button.overflow = new RectOffset(0, 0, -1, 2);
            newSkin.button.imagePosition = ImagePosition.ImageLeft;
            newSkin.button.alignment = TextAnchor.MiddleCenter;
            newSkin.button.contentOffset = new Vector2(0, 0);
            newSkin.button.stretchWidth = true;
            newSkin.button.stretchHeight = false;

            newSkin.label.normal.textColor = new Color32(180, 180, 180, 255);
            newSkin.label.hover.textColor = Color.black;
            newSkin.label.active.textColor = Color.black;
            newSkin.label.focused.textColor = Color.black;
            newSkin.label.border = new RectOffset(0, 0, 0, 0);
            newSkin.label.margin = new RectOffset(4, 4, 2, 2); //new RectOffset(4, 4, 2, 2);
            newSkin.label.padding = new RectOffset(2, 2, 1, 2); //new RectOffset(2, 2, 1, 2);
            newSkin.label.overflow = new RectOffset(0, 0, 0, 0); //new RectOffset(2, 2, 1, 2);
            newSkin.label.fontStyle = FontStyle.Normal;
            newSkin.label.alignment = TextAnchor.UpperLeft;
            newSkin.label.wordWrap = false;
            newSkin.label.richText = false;
            newSkin.label.clipping = TextClipping.Clip;
            newSkin.label.imagePosition = ImagePosition.ImageLeft;
            newSkin.label.contentOffset = new Vector2(0, 0);
            newSkin.label.fixedWidth = 0;
            newSkin.label.fixedHeight = 0;
            newSkin.label.stretchWidth = true;
            newSkin.label.stretchHeight = true;

            texData = ResourceUtils.GetEmbeddedResource("TextField.png");
            LoadImage(_textNormalBackground, texData);
            Object.DontDestroyOnLoad(_textNormalBackground);
            texData = ResourceUtils.GetEmbeddedResource("TextField focused.png");
            LoadImage(_textFocusedBackground, texData);
            Object.DontDestroyOnLoad(_textFocusedBackground);
            newSkin.textField.normal.background = _textNormalBackground;
            newSkin.textField.normal.textColor = new Color32(180, 180, 180, 255);
            newSkin.textField.onNormal.textColor = new Color32(180, 180, 180, 255);
            newSkin.textField.focused.background = _textFocusedBackground;
            newSkin.textField.focused.textColor = new Color32(180, 180, 180, 255);
            newSkin.textField.onFocused.textColor = new Color32(180, 180, 180, 255);
            newSkin.textField.hover.textColor = new Color32(180, 180, 180, 255);
            newSkin.textField.onHover.textColor = new Color32(180, 180, 180, 255);
            newSkin.textField.active.textColor = new Color32(180, 180, 180, 255);
            newSkin.textField.onActive.textColor = new Color32(180, 180, 180, 255);
            newSkin.textField.border = new RectOffset(3, 3, 3, 3);
            newSkin.textField.margin = new RectOffset(4, 4, 2, 2);
            newSkin.textField.padding = new RectOffset(3, 3, 1, 2);
            newSkin.textField.overflow = new RectOffset(0, 0, 0, 0);
            newSkin.textField.font = null;
            newSkin.textField.fontSize = 0;
            newSkin.textField.fontStyle = FontStyle.Normal;
            newSkin.textField.alignment = TextAnchor.UpperLeft;
            newSkin.textField.wordWrap = false;
            newSkin.textField.richText = false;
            newSkin.textField.clipping = TextClipping.Clip;
            newSkin.textField.imagePosition = ImagePosition.TextOnly;
            newSkin.textField.contentOffset = new Vector2(0, 0);
            newSkin.textField.fixedWidth = 0;
            newSkin.textField.fixedHeight = 0;
            newSkin.textField.stretchWidth = true;
            newSkin.textField.stretchHeight = false;

            texData = ResourceUtils.GetEmbeddedResource("slider horiz.png");
            LoadImage(_sliderHNormalBackground, texData);
            Object.DontDestroyOnLoad(_sliderHNormalBackground);
            newSkin.horizontalSlider.normal.background = _sliderHNormalBackground;
            newSkin.horizontalSlider.normal.textColor = Color.black;
            newSkin.horizontalSlider.border = new RectOffset(3, 3, 0, 0);
            newSkin.horizontalSlider.margin = new RectOffset(4, 4, 2, 0);
            newSkin.horizontalSlider.padding = new RectOffset(-1, -1, 0, 0);
            newSkin.horizontalSlider.overflow = new RectOffset(0, 0, -7, -6);
            newSkin.horizontalSlider.font = null;
            newSkin.horizontalSlider.fontSize = 0;
            newSkin.horizontalSlider.fontStyle = FontStyle.Normal;
            newSkin.horizontalSlider.alignment = TextAnchor.UpperLeft;
            newSkin.horizontalSlider.wordWrap = false;
            newSkin.horizontalSlider.richText = false;
            newSkin.horizontalSlider.imagePosition = ImagePosition.ImageOnly;
            newSkin.horizontalSlider.clipping = TextClipping.Clip;
            newSkin.horizontalSlider.contentOffset = new Vector2(0, 0);
            newSkin.horizontalSlider.fixedWidth = 0f;
            newSkin.horizontalSlider.fixedHeight = 18f;
            newSkin.horizontalSlider.stretchWidth = true;
            newSkin.horizontalSlider.stretchHeight = false;

            texData = ResourceUtils.GetEmbeddedResource("slider thumb.png");
            LoadImage(_sliderThumbNormalBackground, texData);
            Object.DontDestroyOnLoad(_sliderThumbNormalBackground);
            texData = ResourceUtils.GetEmbeddedResource("slider thumb act.png");
            LoadImage(_sliderThumbActiveBackground, texData);
            Object.DontDestroyOnLoad(_sliderThumbActiveBackground);
            texData = ResourceUtils.GetEmbeddedResource("slider thumb focus.png");
            LoadImage(_sliderThumbFocusedBackground, texData);
            Object.DontDestroyOnLoad(_sliderThumbFocusedBackground);
            newSkin.horizontalSliderThumb.normal.background = _sliderThumbNormalBackground;
            newSkin.horizontalSliderThumb.normal.textColor = Color.black;
            newSkin.horizontalSliderThumb.onNormal.textColor = Color.black;
            newSkin.horizontalSliderThumb.hover.background = null;
            newSkin.horizontalSliderThumb.hover.textColor = Color.black;
            newSkin.horizontalSliderThumb.onHover.background = null;
            newSkin.horizontalSliderThumb.onHover.textColor = Color.black;
            newSkin.horizontalSliderThumb.active.background = _sliderThumbActiveBackground;
            newSkin.horizontalSliderThumb.active.textColor = Color.black;
            newSkin.horizontalSliderThumb.onActive.textColor = Color.black;
            newSkin.horizontalSliderThumb.focused.background = _sliderThumbFocusedBackground;
            newSkin.horizontalSliderThumb.focused.textColor = Color.black;
            newSkin.horizontalSliderThumb.onFocused.textColor = Color.black;
            newSkin.horizontalSliderThumb.border = new RectOffset(0, 0, 0, 0);
            newSkin.horizontalSliderThumb.margin = new RectOffset(0, 0, 0, 0);
            newSkin.horizontalSliderThumb.padding = new RectOffset(0, 0, 0, 0);
            newSkin.horizontalSliderThumb.overflow = new RectOffset(1, 1, -4, 4);
            newSkin.horizontalSliderThumb.font = null;
            newSkin.horizontalSliderThumb.fontSize = 0;
            newSkin.horizontalSliderThumb.fontStyle = FontStyle.Normal;
            newSkin.horizontalSliderThumb.alignment = TextAnchor.UpperLeft;
            newSkin.horizontalSliderThumb.wordWrap = false;
            newSkin.horizontalSliderThumb.richText = false;
            newSkin.horizontalSliderThumb.imagePosition = ImagePosition.ImageOnly;
            newSkin.horizontalSliderThumb.clipping = TextClipping.Clip;
            newSkin.horizontalSliderThumb.contentOffset = new Vector2(0, 0);
            newSkin.horizontalSliderThumb.fixedWidth = 10f;
            newSkin.horizontalSliderThumb.fixedHeight = 12f;
            newSkin.horizontalSliderThumb.stretchWidth = true;
            newSkin.horizontalSliderThumb.stretchHeight = false;

            texData = ResourceUtils.GetEmbeddedResource("scroll horiz.png");
            LoadImage(_scrollHNormalBackground, texData);
            Object.DontDestroyOnLoad(_scrollHNormalBackground);
            newSkin.horizontalScrollbar.normal.background = _scrollHNormalBackground;
            newSkin.horizontalScrollbar.normal.textColor = Color.black;
            newSkin.horizontalScrollbar.border = new RectOffset(25, 25, 0, 0);
            newSkin.horizontalScrollbar.margin = new RectOffset(0, 0, 0, 0);
            newSkin.horizontalScrollbar.padding = new RectOffset(-1, -1, 0, 0);
            newSkin.horizontalScrollbar.overflow = new RectOffset(1, 1, 0, 0);
            newSkin.horizontalScrollbar.font = null;
            newSkin.horizontalScrollbar.fontSize = 0;
            newSkin.horizontalScrollbar.fontStyle = FontStyle.Normal;
            newSkin.horizontalScrollbar.alignment = TextAnchor.UpperLeft;
            newSkin.horizontalScrollbar.wordWrap = false;
            newSkin.horizontalScrollbar.richText = false;
            newSkin.horizontalScrollbar.imagePosition = ImagePosition.ImageOnly;
            newSkin.horizontalScrollbar.clipping = TextClipping.Clip;
            newSkin.horizontalScrollbar.contentOffset = new Vector2(0, 0);
            newSkin.horizontalScrollbar.fixedWidth = 0f;
            newSkin.horizontalScrollbar.fixedHeight = 15f;
            newSkin.horizontalScrollbar.stretchWidth = true;
            newSkin.horizontalScrollbar.stretchHeight = false;

            texData = ResourceUtils.GetEmbeddedResource("scroll vert.png");
            LoadImage(_scrollVNormalBackground, texData);
            Object.DontDestroyOnLoad(_scrollVNormalBackground);
            newSkin.verticalScrollbar.normal.background = _scrollVNormalBackground;
            newSkin.verticalScrollbar.normal.textColor = Color.black;
            newSkin.verticalScrollbar.border = new RectOffset(0, 0, 9, 9);
            newSkin.verticalScrollbar.margin = new RectOffset(0, 0, 0, 0);
            newSkin.verticalScrollbar.padding = new RectOffset(0, 0, -1, -1);
            newSkin.verticalScrollbar.overflow = new RectOffset(0, 0, 1, 1);
            newSkin.verticalScrollbar.font = null;
            newSkin.verticalScrollbar.fontSize = 0;
            newSkin.verticalScrollbar.fontStyle = FontStyle.Normal;
            newSkin.verticalScrollbar.alignment = TextAnchor.UpperLeft;
            newSkin.verticalScrollbar.wordWrap = false;
            newSkin.verticalScrollbar.richText = false;
            newSkin.verticalScrollbar.imagePosition = ImagePosition.ImageLeft;
            newSkin.verticalScrollbar.clipping = TextClipping.Clip;
            newSkin.verticalScrollbar.contentOffset = new Vector2(0, 0);
            newSkin.verticalScrollbar.fixedWidth = 15f;
            newSkin.verticalScrollbar.fixedHeight = 0f;
            newSkin.verticalScrollbar.stretchWidth = true;
            newSkin.verticalScrollbar.stretchHeight = false;

            texData = ResourceUtils.GetEmbeddedResource("scroll horiz thumb.png");
            LoadImage(_scrollHTNormalBackground, texData);
            Object.DontDestroyOnLoad(_scrollHTNormalBackground);
            newSkin.horizontalScrollbarThumb.normal.background = _scrollHTNormalBackground;
            newSkin.horizontalScrollbarThumb.normal.textColor = Color.black;
            newSkin.horizontalScrollbarThumb.border = new RectOffset(8, 8, 0, 0);
            newSkin.horizontalScrollbarThumb.margin = new RectOffset(0, 0, 0, 0);
            newSkin.horizontalScrollbarThumb.padding = new RectOffset(8, 8, 0, 0);
            newSkin.horizontalScrollbarThumb.overflow = new RectOffset(0, 0, 0, 0);
            newSkin.horizontalScrollbarThumb.font = null;
            newSkin.horizontalScrollbarThumb.fontSize = 0;
            newSkin.horizontalScrollbarThumb.fontStyle = FontStyle.Normal;
            newSkin.horizontalScrollbarThumb.alignment = TextAnchor.UpperLeft;
            newSkin.horizontalScrollbarThumb.wordWrap = false;
            newSkin.horizontalScrollbarThumb.richText = false;
            newSkin.horizontalScrollbarThumb.imagePosition = ImagePosition.ImageLeft;
            newSkin.horizontalScrollbarThumb.clipping = TextClipping.Clip;
            newSkin.horizontalScrollbarThumb.contentOffset = new Vector2(0, 0);
            newSkin.horizontalScrollbarThumb.fixedWidth = 0f;
            newSkin.horizontalScrollbarThumb.fixedHeight = 15f;
            newSkin.horizontalScrollbarThumb.stretchWidth = true;
            newSkin.horizontalScrollbarThumb.stretchHeight = false;

            texData = ResourceUtils.GetEmbeddedResource("scroll vert thumb.png");
            LoadImage(_scrollVTNormalBackground, texData);
            Object.DontDestroyOnLoad(_scrollVTNormalBackground);
            newSkin.verticalScrollbarThumb.normal.background = _scrollVTNormalBackground;
            newSkin.verticalScrollbarThumb.normal.textColor = Color.black;
            newSkin.verticalScrollbarThumb.border = new RectOffset(0, 0, 8, 8);
            newSkin.verticalScrollbarThumb.margin = new RectOffset(0, 0, 0, 0);
            newSkin.verticalScrollbarThumb.padding = new RectOffset(0, 0, 10, 10);
            newSkin.verticalScrollbarThumb.overflow = new RectOffset(0, 0, 0, 0);
            newSkin.verticalScrollbarThumb.font = null;
            newSkin.verticalScrollbarThumb.fontSize = 0;
            newSkin.verticalScrollbarThumb.fontStyle = FontStyle.Normal;
            newSkin.verticalScrollbarThumb.alignment = TextAnchor.UpperLeft;
            newSkin.verticalScrollbarThumb.wordWrap = false;
            newSkin.verticalScrollbarThumb.richText = false;
            newSkin.verticalScrollbarThumb.imagePosition = ImagePosition.ImageOnly;
            newSkin.verticalScrollbarThumb.clipping = TextClipping.Clip;
            newSkin.verticalScrollbarThumb.contentOffset = new Vector2(0, 0);
            newSkin.verticalScrollbarThumb.fixedWidth = 15f;
            newSkin.verticalScrollbarThumb.fixedHeight = 0f;
            newSkin.verticalScrollbarThumb.stretchWidth = false;
            newSkin.verticalScrollbarThumb.stretchHeight = true;

            texData = ResourceUtils.GetEmbeddedResource("scroll horiz left.png");
            LoadImage(_scrollHLNormalBackground, texData);
            Object.DontDestroyOnLoad(_scrollHLNormalBackground);
            texData = ResourceUtils.GetEmbeddedResource("scroll horiz left act.png");
            LoadImage(_scrollHLActiveBackground, texData);
            Object.DontDestroyOnLoad(_scrollHLActiveBackground);
            newSkin.horizontalScrollbarLeftButton.normal.background = _scrollHLNormalBackground;
            newSkin.horizontalScrollbarLeftButton.normal.textColor = Color.black;
            newSkin.horizontalScrollbarLeftButton.active.background = _scrollHLActiveBackground;
            newSkin.horizontalScrollbarLeftButton.border = new RectOffset(0, 0, 0, 0);
            newSkin.horizontalScrollbarLeftButton.margin = new RectOffset(0, 0, 0, 0);
            newSkin.horizontalScrollbarLeftButton.padding = new RectOffset(0, 0, 0, 0);
            newSkin.horizontalScrollbarLeftButton.overflow = new RectOffset(0, 8, 0, 0);
            newSkin.horizontalScrollbarLeftButton.font = null;
            newSkin.horizontalScrollbarLeftButton.fontSize = 0;
            newSkin.horizontalScrollbarLeftButton.fontStyle = FontStyle.Normal;
            newSkin.horizontalScrollbarLeftButton.alignment = TextAnchor.UpperLeft;
            newSkin.horizontalScrollbarLeftButton.wordWrap = false;
            newSkin.horizontalScrollbarLeftButton.richText = false;
            newSkin.horizontalScrollbarLeftButton.imagePosition = ImagePosition.ImageLeft;
            newSkin.horizontalScrollbarLeftButton.clipping = TextClipping.Clip;
            newSkin.horizontalScrollbarLeftButton.contentOffset = new Vector2(0, 0);
            newSkin.horizontalScrollbarLeftButton.fixedWidth = 17f;
            newSkin.horizontalScrollbarLeftButton.fixedHeight = 15f;
            newSkin.horizontalScrollbarLeftButton.stretchWidth = true;
            newSkin.horizontalScrollbarLeftButton.stretchHeight = false;

            texData = ResourceUtils.GetEmbeddedResource("scroll horiz right.png");
            LoadImage(_scrollHRNormalBackground, texData);
            Object.DontDestroyOnLoad(_scrollHRNormalBackground);
            texData = ResourceUtils.GetEmbeddedResource("scroll horiz right act.png");
            LoadImage(_scrollHRActiveBackground, texData);
            Object.DontDestroyOnLoad(_scrollHRActiveBackground);
            newSkin.horizontalScrollbarRightButton.normal.background = _scrollHRNormalBackground;
            newSkin.horizontalScrollbarRightButton.normal.textColor = Color.black;
            newSkin.horizontalScrollbarRightButton.active.background = _scrollHRActiveBackground;
            newSkin.horizontalScrollbarRightButton.border = new RectOffset(0, 0, 0, 0);
            newSkin.horizontalScrollbarRightButton.margin = new RectOffset(0, 0, 0, 0);
            newSkin.horizontalScrollbarRightButton.padding = new RectOffset(0, 0, 0, 0);
            newSkin.horizontalScrollbarRightButton.overflow = new RectOffset(8, 0, 0, 0);
            newSkin.horizontalScrollbarRightButton.font = null;
            newSkin.horizontalScrollbarRightButton.fontSize = 0;
            newSkin.horizontalScrollbarRightButton.fontStyle = FontStyle.Normal;
            newSkin.horizontalScrollbarRightButton.alignment = TextAnchor.UpperLeft;
            newSkin.horizontalScrollbarRightButton.wordWrap = false;
            newSkin.horizontalScrollbarRightButton.richText = false;
            newSkin.horizontalScrollbarRightButton.imagePosition = ImagePosition.ImageLeft;
            newSkin.horizontalScrollbarRightButton.clipping = TextClipping.Clip;
            newSkin.horizontalScrollbarRightButton.contentOffset = new Vector2(0, 0);
            newSkin.horizontalScrollbarRightButton.fixedWidth = 17.24739f;
            newSkin.horizontalScrollbarRightButton.fixedHeight = 15f;
            newSkin.horizontalScrollbarRightButton.stretchWidth = true;
            newSkin.horizontalScrollbarRightButton.stretchHeight = false;

            texData = ResourceUtils.GetEmbeddedResource("scroll vert up.png");
            LoadImage(_scrollVUNormalBackground, texData);
            Object.DontDestroyOnLoad(_scrollVUNormalBackground);
            texData = ResourceUtils.GetEmbeddedResource("scroll vert up act.png");
            LoadImage(_scrollVUActiveBackground, texData);
            Object.DontDestroyOnLoad(_scrollVUActiveBackground);
            newSkin.verticalScrollbarUpButton.normal.background = _scrollVUNormalBackground;
            newSkin.verticalScrollbarUpButton.normal.textColor = Color.black;
            newSkin.verticalScrollbarUpButton.active.background = _scrollVUActiveBackground;
            newSkin.verticalScrollbarUpButton.border = new RectOffset(0, 0, 0, 0);
            newSkin.verticalScrollbarUpButton.margin = new RectOffset(0, 0, 0, 0);
            newSkin.verticalScrollbarUpButton.padding = new RectOffset(0, 0, 0, 0);
            newSkin.verticalScrollbarUpButton.overflow = new RectOffset(0, 0, 0, 8);
            newSkin.verticalScrollbarUpButton.font = null;
            newSkin.verticalScrollbarUpButton.fontSize = 0;
            newSkin.verticalScrollbarUpButton.fontStyle = FontStyle.Normal;
            newSkin.verticalScrollbarUpButton.alignment = TextAnchor.UpperLeft;
            newSkin.verticalScrollbarUpButton.wordWrap = false;
            newSkin.verticalScrollbarUpButton.richText = false;
            newSkin.verticalScrollbarUpButton.imagePosition = ImagePosition.ImageLeft;
            newSkin.verticalScrollbarUpButton.clipping = TextClipping.Clip;
            newSkin.verticalScrollbarUpButton.contentOffset = new Vector2(0, 0);
            newSkin.verticalScrollbarUpButton.fixedWidth = 15f;
            newSkin.verticalScrollbarUpButton.fixedHeight = 17f;
            newSkin.verticalScrollbarUpButton.stretchWidth = true;
            newSkin.verticalScrollbarUpButton.stretchHeight = false;

            texData = ResourceUtils.GetEmbeddedResource("scroll vert down.png");
            LoadImage(_scrollVDNormalBackground, texData);
            Object.DontDestroyOnLoad(_scrollVDNormalBackground);
            texData = ResourceUtils.GetEmbeddedResource("scroll vert down act.png");
            LoadImage(_scrollVDActiveBackground, texData);
            Object.DontDestroyOnLoad(_scrollVDActiveBackground);
            newSkin.verticalScrollbarDownButton.normal.background = _scrollVDNormalBackground;
            newSkin.verticalScrollbarDownButton.normal.textColor = Color.black;
            newSkin.verticalScrollbarDownButton.active.background = _scrollVDActiveBackground;
            newSkin.verticalScrollbarDownButton.border = new RectOffset(0, 0, 0, 0);
            newSkin.verticalScrollbarDownButton.margin = new RectOffset(0, 0, 0, 0);
            newSkin.verticalScrollbarDownButton.padding = new RectOffset(0, 0, 0, 0);
            newSkin.verticalScrollbarDownButton.overflow = new RectOffset(0, 0, 8, 0);
            newSkin.verticalScrollbarDownButton.font = null;
            newSkin.verticalScrollbarDownButton.fontSize = 0;
            newSkin.verticalScrollbarDownButton.fontStyle = FontStyle.Normal;
            newSkin.verticalScrollbarDownButton.alignment = TextAnchor.UpperLeft;
            newSkin.verticalScrollbarDownButton.wordWrap = false;
            newSkin.verticalScrollbarDownButton.richText = false;
            newSkin.verticalScrollbarDownButton.imagePosition = ImagePosition.ImageLeft;
            newSkin.verticalScrollbarDownButton.clipping = TextClipping.Clip;
            newSkin.verticalScrollbarDownButton.contentOffset = new Vector2(0, 0);
            newSkin.verticalScrollbarDownButton.fixedWidth = 15f;
            newSkin.verticalScrollbarDownButton.fixedHeight = 17f;
            newSkin.verticalScrollbarDownButton.stretchWidth = true;
            newSkin.verticalScrollbarDownButton.stretchHeight = false;

            boldlabel = new GUIStyle
            {
                name = "boldlabel"
            };
            boldlabel.normal.textColor = newSkin.label.normal.textColor;
            boldlabel.border = newSkin.label.border;
            boldlabel.margin = new RectOffset(4, 4, 6, 4); //newSkin.label.margin;
            boldlabel.padding = newSkin.label.padding;
            boldlabel.overflow = newSkin.label.overflow;
            boldlabel.stretchWidth = newSkin.label.stretchWidth;
            boldlabel.stretchHeight = newSkin.label.stretchHeight;
            boldlabel.fontStyle = FontStyle.Bold;
            boldlabel.alignment = TextAnchor.UpperLeft;
            boldlabel.wordWrap = false;
            boldlabel.richText = false;
            boldlabel.clipping = TextClipping.Clip;
            boldlabel.imagePosition = ImagePosition.ImageLeft;
            boldlabel.contentOffset = new Vector2(0, 0);
            boldlabel.fixedWidth = 0;
            boldlabel.fixedHeight = 0;
            boldlabel.stretchWidth = true;
            boldlabel.stretchHeight = false;

            toolbarbutton = new GUIStyle
            {
                name = "toolbarbutton"
            };
            texData = ResourceUtils.GetEmbeddedResource("toolbar button.png");
            LoadImage(_toolbarbtnNormalBackground, texData);
            Object.DontDestroyOnLoad(_toolbarbtnNormalBackground);
            texData = ResourceUtils.GetEmbeddedResource("toolbar button on.png");
            LoadImage(_toolbarbtnOnNormalBackground, texData);
            Object.DontDestroyOnLoad(_toolbarbtnOnNormalBackground);
            texData = ResourceUtils.GetEmbeddedResource("toolbar button act.png");
            LoadImage(_toolbarbtnActiveBackground, texData);
            Object.DontDestroyOnLoad(_toolbarbtnActiveBackground);
            texData = ResourceUtils.GetEmbeddedResource("toolbar button act on.png");
            LoadImage(_toolbarbtnOnActiveBackground, texData);
            Object.DontDestroyOnLoad(_toolbarbtnOnActiveBackground);
            toolbarbutton.normal.background = _toolbarbtnNormalBackground;
            toolbarbutton.normal.textColor = new Color32(180, 180, 180, 255);
            toolbarbutton.hover.background = null;
            toolbarbutton.hover.textColor = Color.black;
            toolbarbutton.onHover.background = null;
            toolbarbutton.onHover.textColor = Color.black;
            toolbarbutton.onNormal.background = _toolbarbtnOnNormalBackground;
            toolbarbutton.onNormal.textColor = new Color32(180, 180, 180, 255);
            toolbarbutton.active.background = _toolbarbtnActiveBackground;
            toolbarbutton.active.textColor = new Color32(180, 180, 180, 255);
            toolbarbutton.onActive.background = _toolbarbtnOnActiveBackground;
            toolbarbutton.onActive.textColor = new Color32(180, 180, 180, 255);
            toolbarbutton.focused.background = null;
            toolbarbutton.focused.textColor = Color.black;
            toolbarbutton.onFocused.background = null;
            toolbarbutton.onFocused.textColor = new Color32(180, 180, 180, 255);
            toolbarbutton.border = new RectOffset(6, 6, 0, 0);
            toolbarbutton.margin = new RectOffset(0, 0, 0, 0);
            toolbarbutton.padding = new RectOffset(5, 5, 0, 0);
            toolbarbutton.overflow = new RectOffset(0, 1, 0, 0);
            toolbarbutton.fontStyle = FontStyle.Normal;
            toolbarbutton.alignment = TextAnchor.MiddleCenter;
            toolbarbutton.wordWrap = false;
            toolbarbutton.richText = false;
            toolbarbutton.clipping = TextClipping.Clip;
            toolbarbutton.imagePosition = ImagePosition.ImageLeft;
            toolbarbutton.contentOffset = new Vector2(0, -3);
            toolbarbutton.fixedWidth = 0f;
            toolbarbutton.fixedHeight = 18f;
            toolbarbutton.stretchWidth = true;
            toolbarbutton.stretchHeight = false;

            newSkin.customStyles = new GUIStyle[] { toolbarbutton, boldlabel };

            newSkin.settings.doubleClickSelectsWord = true;
            newSkin.settings.tripleClickSelectsLine = true;
            newSkin.settings.cursorColor = new Color32(180, 180, 180, 255);
            newSkin.settings.cursorFlashSpeed = 0;
            newSkin.settings.selectionColor = new Color(61, 128, 223, 166);

            return newSkin;
        }
    }
}
