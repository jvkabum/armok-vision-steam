using System;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using Newtonsoft.Json;

/// <summary>
/// Localização estática. Usar L.Get("chave") ou L.Get("chave", args).
/// Idioma em GameSettings.Instance.game.language; ficheiros em StreamingAssets/Localization/{language}.json
/// </summary>
public static class L
{
    static Dictionary<string, string> _strings;
    static string _currentLanguage;
    static bool _initialized;

    static string GetLanguage()
    {
        if (GameSettings.Instance != null && GameSettings.Instance.game != null)
            return GameSettings.Instance.game.language ?? "en";
        return "en";
    }

    static void EnsureLoaded()
    {
        string lang = GetLanguage();
        if (_initialized && _currentLanguage == lang && _strings != null)
            return;
        _currentLanguage = lang;
        _initialized = true;
        _strings = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);
        string path = Path.Combine(Application.streamingAssetsPath, "Localization", lang + ".json");
        if (!File.Exists(path) && lang != "en")
            path = Path.Combine(Application.streamingAssetsPath, "Localization", "en.json");
        if (File.Exists(path))
        {
            try
            {
                var dict = JsonConvert.DeserializeObject<Dictionary<string, string>>(File.ReadAllText(path));
                if (dict != null) _strings = dict;
            }
            catch (Exception ex) { Debug.LogWarning("Localization: " + ex.Message); }
        }
    }

    public static string Get(string key)
    {
        if (string.IsNullOrEmpty(key)) return key;
        EnsureLoaded();
        return _strings != null && _strings.TryGetValue(key, out var value) ? value : key;
    }

    public static string Get(string key, params object[] args)
    {
        string template = Get(key);
        try { return args != null && args.Length > 0 ? string.Format(template, args) : template; }
        catch { return template; }
    }

    public static void Reload() { _initialized = false; _currentLanguage = null; }
}
