using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

namespace AIGraphics
{
    internal class LocalizationManager : MonoBehaviour
    {
        internal enum Language
        {
            English = SystemLanguage.English,
            Korean = SystemLanguage.Korean,
            Japanese = SystemLanguage.Japanese
        }

        private static bool Initialized { get; set; }
        private static string _localizationPath;
        private static Language _currentLanguage;
        private static Dictionary<string, Dictionary<string, string>> _lookup;
        private static Dictionary<string, string> _textLookup;
        private static AIGraphics _parent;

        internal static string LocalizationPath
        {
            get => _localizationPath;
            set
            {
                _parent?.StartCoroutine(Load(value));
                _localizationPath = value;
            }
        }

        private static void DefaultLanguage()
        {
            CurrentLanguage = Enum.IsDefined(typeof(Language), (Language)Application.systemLanguage) ? (Language)Application.systemLanguage : Language.English; 
        }

        internal static Language CurrentLanguage
        {
            get => _currentLanguage;
            set 
            {
                _currentLanguage = value;
                _parent?.StartCoroutine(LoadLocalization((SystemLanguage)_currentLanguage));
            }
            
        }
        private static string CurrentLocale()
        {
            return Locale((SystemLanguage)CurrentLanguage);
        }

        private static string Locale(SystemLanguage languge)
        {
            //TODO: use Enum.GetNames
            switch (languge)
            {
                case SystemLanguage.Japanese:
                    return "jpn";
                case SystemLanguage.Korean:
                    return "kor";
                default:
                    return "eng";
            }
        }
        internal static string Localized(string text, bool fallBackOnDefault = true)
        {
            if (_textLookup.TryGetValue(text, out string localized))
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
            if (SystemLanguage.Japanese == language || SystemLanguage.Korean == language)
            {
                _lookup.TryGetValue(CurrentLocale(), out _textLookup);
            }
        }

        private static IEnumerator Load(string path)
        {
            Initialized = false;
            _lookup = new Dictionary<string, Dictionary<string, string>>();
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
                    Debug.Log("Failed to read from " + localization);
                    continue;
                }
                yield return lines;
                Dictionary<string, string> languageDictionary = new Dictionary<string, string>();
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

        internal static AIGraphics Parent
        {
            get => _parent;
            set
            {
                _parent = value;
                DefaultLanguage();
                LocalizationPath = AIGraphics.ConfigLocalizationPath.Value;
            }
        }
    }
}