using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class LoreProxy : MonoBehaviour
{
    [Space(10f)]
    [Header("Text")]
    [SerializeField] private string title;

    [TextArea(5, 20)]
    [SerializeField] private string body;

    [Space(10)]
    [SerializeField] private TextAlignmentOptions textAlignment;

    [Space(10)]
    [SerializeField] private bool equalFontSize;
    [Space(30f)]
    [SerializeField] private TMP_FontAsset font_custom;

    [SerializeField] private bool useColor;

    [SerializeField] private Color textColor;


    private const float minAutoSize = 0.06f;
    private const float maxAutoSize = 0.45f;
    private void Awake()
    {
        if (useColor && textColor.a <= 0) Debug.LogError($"Error: Alpha value is set to 0 in the script attached to the GameObject named '\" {name} \"'. Please ensure the alpha value is greater than 0 to avoid any issues. Script: \"{GetType()}\".");

        Quaternion currentRotation = transform.rotation;
        Vector3 originalScale = transform.localScale;

        transform.rotation = Quaternion.identity;
        transform.localScale = new Vector3(0.3f, 0.3f, 0.3f);
        GameObject canvas = SetupCanvas();

        RectTransform proxyLoreHolder = SetupProxyLoreHolder(canvas);

        TextMeshProUGUI titleText = CreateTextMeshPro("title", proxyLoreHolder.transform);
        TextMeshProUGUI bodyText = CreateTextMeshPro("body", proxyLoreHolder.transform);

        DefaultSettings(titleText, bodyText);

        titleText.transform.SetParent(proxyLoreHolder.transform, false);
        var contentSizeFitter = titleText.gameObject.AddComponent<ContentSizeFitter>();
        contentSizeFitter.verticalFit = ContentSizeFitter.FitMode.PreferredSize;
        bodyText.transform.SetParent(proxyLoreHolder.transform, false);

        SetMinMaxFontSize(titleText, bodyText);
        AdjustTextProperties(titleText, bodyText);
        CheckColor(titleText, bodyText);

        titleText.text = title;
        bodyText.text = body;

        AdjustFontSize(titleText);
        AdjustFontSize(bodyText);

        transform.rotation = currentRotation;
        transform.localScale = originalScale;

        if (equalFontSize) titleText.fontSize = bodyText.fontSize * bodyText.transform.localScale.x;

        LayoutRebuilder.ForceRebuildLayoutImmediate(titleText.rectTransform);
    }

    private GameObject SetupCanvas()
    {
        GameObject canvas = new("Canvas", typeof(Canvas), typeof(CanvasScaler), typeof(GraphicRaycaster));
        canvas.transform.SetParent(transform, false);
        canvas.transform.SetPositionAndRotation(new Vector3(canvas.transform.position.x + 0.050f, canvas.transform.position.y, canvas.transform.position.z - 0.065f), Quaternion.Euler(90, 180, 0));
        return canvas;
    }

    private RectTransform SetupProxyLoreHolder(GameObject canvas)
    {
        GameObject proxyLoreHolderObj = new GameObject("Proxy Lore Holder", typeof(RectTransform));
        RectTransform proxyLoreHolder = proxyLoreHolderObj.GetComponent<RectTransform>();
        proxyLoreHolder.SetParent(canvas.transform, false);
        proxyLoreHolder.sizeDelta = new Vector2(1f, 1f);

        VerticalLayoutGroup verticalLayoutGroup = proxyLoreHolderObj.AddComponent<VerticalLayoutGroup>();
        verticalLayoutGroup.spacing = 0.07f;
        verticalLayoutGroup.childControlHeight = false;
        verticalLayoutGroup.childControlWidth = false;

        return proxyLoreHolder;
    }

    private void SetMinMaxFontSize(TextMeshProUGUI titleText, TextMeshProUGUI bodyText)
    {
        titleText.fontSizeMin = minAutoSize;
        bodyText.fontSizeMin = minAutoSize;
        titleText.fontSizeMax = maxAutoSize;
        bodyText.fontSizeMax = maxAutoSize;
    }

    private TextMeshProUGUI CreateTextMeshPro(string name, Transform transform)
    {
        GameObject textObj = new(name, typeof(TextMeshProUGUI));
        TextMeshProUGUI textMeshPro = textObj.GetComponent<TextMeshProUGUI>();
        textMeshPro.transform.SetParent(transform, false);
        textMeshPro.alignment = textAlignment;
        return textMeshPro;
    }

    private void AdjustTextProperties(TextMeshProUGUI titleText, TextMeshProUGUI bodyText)
    {
        titleText.rectTransform.sizeDelta = new Vector2(1.32f, 0.20f);
        bodyText.rectTransform.sizeDelta = new Vector2(5.25f, 5.20f);
        bodyText.transform.localScale = new Vector3(0.25f, 0.25f, 0.25f);
        bodyText.rectTransform.pivot = new Vector2(0, 1);
    }

    private void AdjustFontSize(TextMeshProUGUI textMeshPro)
    {
        textMeshPro.enableAutoSizing = true;
        textMeshPro.ForceMeshUpdate();
        float optimumPointSize = textMeshPro.fontSize;
        textMeshPro.enableAutoSizing = false;
        textMeshPro.fontSize = optimumPointSize;
    }

    private void CheckColor(TextMeshProUGUI titleText, TextMeshProUGUI bodyText)
    {
        if (!useColor) return;

        titleText.color = textColor;
        bodyText.color = textColor;
    }

    private void DefaultSettings(TextMeshProUGUI titleText, TextMeshProUGUI bodyText)
    {
        titleText.font = font_custom;
        bodyText.font = font_custom;
        if (useColor) return;

        titleText.color = Color.black;
        bodyText.color = Color.black;
    }
}
