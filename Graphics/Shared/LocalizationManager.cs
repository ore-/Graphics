using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

namespace Graphics
{
    public class LocalizationManager : MonoBehaviour
    {
        public enum Language
        {
            English = SystemLanguage.English,
            Korean = SystemLanguage.Korean,
            Japanese = SystemLanguage.Japanese,
            ChineseSimplified = SystemLanguage.ChineseSimplified
        }

        private static bool Initialized { get; set; }
        private static string _localizationPath;
        private static Language _currentLanguage;
        private static Dictionary<string, Dictionary<string, string>> _lookup;
        private static Dictionary<string, string> _textLookup;
        private static Graphics _parent;

        internal static string LocalizationPath
        {
            get => _localizationPath;
            set
            {
                _parent?.StartCoroutine(Load(value));
                _localizationPath = value;
            }
        }

        internal static Language DefaultLanguage()
        {
            return Enum.IsDefined(typeof(Language), (Language)Application.systemLanguage) ? (Language)Application.systemLanguage : Language.English;
        }

        internal static Language CurrentLanguage
        {
            get => _currentLanguage;
            set
            {
                _currentLanguage = value;
                Graphics.ConfigLanguage.Value = value;
                _parent?.StartCoroutine(LoadLocalization((SystemLanguage)_currentLanguage));
            }

        }
        private static string CurrentLocale()
        {
            return Enum.GetName(typeof(Language), CurrentLanguage);
        }

        internal static string Localized(string text, bool fallBackOnDefault = true)
        {
            if (null != _textLookup && _textLookup.TryGetValue(text, out string localized))
            {
                return localized;
            }

            return fallBackOnDefault ? text : "";
        }

        internal static bool HasLocalization()
        {
            return null != _lookup && _lookup.ContainsKey(CurrentLocale());
        }
        private static IEnumerator LoadLocalization(SystemLanguage language)
        {
            yield return new WaitUntil(() => Initialized);
            if (Enum.IsDefined(typeof(Language), (Language)language))
            {
                _lookup.TryGetValue(CurrentLocale(), out _textLookup);
            }
        }

        private static IEnumerator Load(string path)
        {
            Initialized = false;
            _lookup = new Dictionary<string, Dictionary<string, string>>(StringComparer.OrdinalIgnoreCase);
            List<string> paths = Util.GetFiles(path, "localization.*.txt");
            yield return paths;
            foreach (string localization in paths)
            {
                string[] languageTokens = Path.GetFileNameWithoutExtension(localization).Split('.');
                if (2 != languageTokens.Length)
                {
                    continue;
                }

                IEnumerable<string> lines;
                try
                {
                    lines = File.ReadLines(localization);
                }
                catch
                {
                    Graphics.Instance.Log.Log(BepInEx.Logging.LogLevel.Error, "Failed to read from " + localization);
                    continue;
                }
                yield return lines;
                Dictionary<string, string> languageDictionary = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);
                _lookup.Add(languageTokens[1], languageDictionary);
                foreach (string line in lines)
                {
                    string[] tokens = line.Split(',');
                    if (2 == tokens.Length)
                    {
                        languageDictionary.Add(tokens[0], tokens[1]);
                    }
                }
            }
            Initialized = true;
        }

        internal static Graphics Parent
        {
            get => _parent;
            set
            {
                _parent = value;
                LocalizationPath = Graphics.ConfigLocalizationPath.Value;
            }
        }
    }
}