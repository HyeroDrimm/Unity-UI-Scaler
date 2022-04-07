using System.Linq;
using UnityEditor;
using UnityEngine;

#if UNITY_EDITOR

public class SelectAllOfTag : EditorWindow
{
    [MenuItem("Tools/UI/Scaler/Apply Scale To Selected RectTransforms and their children")]
    private static void ApplyScaleToSelectedRectTransformssAndTheirChildren()
    {
        for (int i = 0; i < Selection.objects.Length; ++i)
        {
            var obj = Selection.objects[i];
            if (obj is GameObject && (obj as GameObject).TryGetComponent(out RectTransform rectTransform))
                ApplyScaleToAllChildren(rectTransform);
        }
        Debug.Log("Scale applied for selected RectTransforms and their children");
    }

    [MenuItem("Tools/UI/Scaler/Apply Scale To Selected RectTransforms")]
    private static void ApplyScaleToSelectedRects()
    {
        for (int i = 0; i < Selection.objects.Length; ++i)
        {
            var obj = Selection.objects[i];
            if (obj is GameObject && (obj as GameObject).TryGetComponent(out RectTransform rectTransform))
                ApplyScaleToRectTransform(rectTransform);
        }
        Debug.Log("Scale applied for selected RectTransforms");
    }

    private static void ApplyScaleToAllChildren(RectTransform rectTransform)
    {
        var pos = rectTransform.position;
        ApplyScaleToAllChildren(rectTransform, Vector2.one);
        rectTransform.position = pos;
    }

    private static void ApplyScaleToAllChildren(RectTransform rectTransform, Vector2 scale)
    {
        scale *= rectTransform.localScale;

        ApplyScaleToRectTransform(rectTransform, scale);

        foreach (var child in rectTransform.Cast<RectTransform>().ToArray())
        {
            ApplyScaleToAllChildren(child, scale);
        }
    }

    public static void ApplyScaleToRectTransform(RectTransform rectTransform)
    {
        var pos = rectTransform.position;
        ApplyScaleToRectTransform(rectTransform, rectTransform.localScale);
        rectTransform.position = pos;
    }

    public static void ApplyScaleToRectTransform(RectTransform rectTransform, Vector2 scale)
    {
        PrefabUtility.RecordPrefabInstancePropertyModifications(rectTransform);
        Undo.RecordObject(rectTransform, "Apply Scale");


        rectTransform.sizeDelta *= scale;
        rectTransform.anchoredPosition *= scale;
        rectTransform.localScale = Vector3.one;

        if (rectTransform.TryGetComponent(out TMPro.TMP_Text text))
        {
            Undo.RecordObject(text, "Apply Scale");
            text.fontSize *= Mathf.Min(scale.x, scale.y);
        }


        EditorUtility.SetDirty(rectTransform);
    }
}

#endif
