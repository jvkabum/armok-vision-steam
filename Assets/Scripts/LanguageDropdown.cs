using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using TMPro;

/// <summary>
/// Preenche o dropdown com idiomas (English / Português (Brasil)), lê/grava GameSettings.Instance.game.language.
/// Suporta UnityEngine.UI.Dropdown e TMPro.TMP_Dropdown.
/// </summary>
public class LanguageDropdown : MonoBehaviour
{
    public const string LangEn = "en";
    public const string LangPtBR = "pt-BR";

    Dropdown _dropdown;
    TMP_Dropdown _tmpDropdown;
    bool _ignoreNextChange;

    void Awake()
    {
        _dropdown = GetComponent<Dropdown>();
        if (_dropdown == null) _dropdown = GetComponentInChildren<Dropdown>(true);
        if (_dropdown == null)
        {
            _tmpDropdown = GetComponent<TMP_Dropdown>();
            if (_tmpDropdown == null) _tmpDropdown = GetComponentInChildren<TMP_Dropdown>(true);
        }
    }

    void Start()
    {
        if (_dropdown == null && _tmpDropdown == null) return;

        PopulateOptions();
        
        if (_dropdown != null)
            _dropdown.onValueChanged.AddListener(OnValueChanged);
        else
            _tmpDropdown.onValueChanged.AddListener(OnValueChanged);
            
        SyncFromSettings();
        LocalizeSettingsPanel();
    }

    void OnEnable()
    {
        SyncFromSettings();
        LocalizeSettingsPanel();
    }

    void PopulateOptions()
    {
        // Usamos nomes fixos se a tradução ainda não estiver pronta, 
        // mas garantimos que a lista seja estável para o clique.
        var opts = new List<string> { "English", "Português (Brasil)" };
        
        if (_dropdown != null)
        {
            _dropdown.ClearOptions();
            _dropdown.AddOptions(opts);
        }
        else if (_tmpDropdown != null)
        {
            _tmpDropdown.ClearOptions();
            _tmpDropdown.AddOptions(opts);
        }
    }

    void SyncFromSettings()
    {
        if ((_dropdown == null && _tmpDropdown == null) || GameSettings.Instance?.game == null) return;
        string lang = GameSettings.Instance.game.language ?? LangEn;
        int index = lang == LangPtBR ? 1 : 0;
        
        if (_dropdown != null)
        {
            if (_dropdown.value != index) { _ignoreNextChange = true; _dropdown.value = index; }
            _dropdown.RefreshShownValue();
        }
        else if (_tmpDropdown != null)
        {
            if (_tmpDropdown.value != index) { _ignoreNextChange = true; _tmpDropdown.value = index; }
            _tmpDropdown.RefreshShownValue();
        }
    }

    void OnValueChanged(int index)
    {
        if (_ignoreNextChange) { _ignoreNextChange = false; return; }
        string lang = index == 1 ? LangPtBR : LangEn;
        if (GameSettings.Instance?.game != null)
        {
            GameSettings.Instance.game.language = lang;
            GameSettings.Persist();
        }
        L.Reload();
        LocalizeSettingsPanel();
        RefreshMenu();
    }

    void LocalizeSettingsPanel()
    {
        Transform panel = transform;
        while (panel.parent != null && panel.name != "Settings Panel")
        {
            panel = panel.parent;
        }

        if (panel == null) panel = transform.parent;
        if (panel == null) return;

        // Traduz textos padrão da UI
        foreach (var text in panel.GetComponentsInChildren<Text>(true))
        {
            LocalizeComponent(text);
        }

        // Traduz textos do TextMesh Pro
        foreach (var text in panel.GetComponentsInChildren<TMP_Text>(true))
        {
            LocalizeComponent(text);
        }
    }

    void LocalizeComponent(Text component)
    {
        if (component == null || string.IsNullOrEmpty(component.text)) return;
        string key = component.text.Trim();
        string translated = L.Get(key);
        if (translated != key) component.text = translated;
    }

    void LocalizeComponent(TMP_Text component)
    {
        if (component == null || string.IsNullOrEmpty(component.text)) return;
        string key = component.text.Trim();
        string translated = L.Get(key);
        if (translated != key) component.text = translated;
    }

    void RefreshMenu()
    {
        var menu = FindObjectOfType<DwarfModeMenu>();
        if (menu != null) menu.RefreshDefaultMenu();
    }
}
