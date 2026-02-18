using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Componente para traduzir textos automaticamente via L.Get(key).
/// </summary>
[RequireComponent(typeof(Text))]
public class LocalizedText : MonoBehaviour
{
    public string key;

    void OnEnable()
    {
        UpdateText();
    }

    public void UpdateText()
    {
        if (string.IsNullOrEmpty(key)) return;
        var textComp = GetComponent<Text>();
        if (textComp != null)
        {
            textComp.text = L.Get(key);
        }
    }
}
