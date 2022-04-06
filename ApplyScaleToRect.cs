using System.Linq;
using UnityEditor;
using UnityEngine;

#if UNITY_EDITOR

public class SelectAllOfTag : EditorWindow
{
    [MenuItem("UI/Scaler/Apply Scale To Selected RectTransforms")]
    private static void ApplyScaleToSelectedRects()
    {
        for (int i = 0; i < Selection.objects.Length; ++i)
        {
            var obj = Selection.objects[i];
            if (obj is GameObject && (obj as GameObject).TryGetComponent(out RectTransform rectTransform))
            {
                ApplyScaleToRectTransform(rectTransform);
            }
        }
        Debug.Log("Scale applied for selected RectTransforms");
    }

    [MenuItem("UI/Scaler/Apply Scale To Selected RectTransforms and their children")]
    private static void ApplyScaleToSelectedRectTransformssAndTheirChildren()
    {
        for (int i = 0; i < Selection.objects.Length; ++i)
        {
            var obj = Selection.objects[i];
            if (obj is GameObject && (obj as GameObject).TryGetComponent(out RectTransform rectTransform))
            {
                ApplyScaleToAllChildren(rectTransform);
            }
        }
        Debug.Log("Scale applied for selected RectTransforms and their children");
    }

    [MenuItem("UI/Scaler/How Many RectTransforms Are Selected?")]
    private static void HowManyRectTrasformsWithRectsAreSelected()
    {
        var counter = Selection.objects.Where(obj => obj is GameObject && (obj as GameObject).TryGetComponent(out RectTransform rectTransform)).Count();
        Debug.Log("Number of selected RectTransforms is: " + counter);
    }

    private static void ApplyScaleToAllChildren(RectTransform rectTransform, Vector2? additionalScale = null)
    {
        var scale = additionalScale is null ? Vector2.one : additionalScale.Value;

        scale *= rectTransform.localScale;

        ApplyScaleToRectTransform(rectTransform, scale);

        foreach (var child in rectTransform.Cast<RectTransform>().ToArray())
        {
            ApplyScaleToAllChildren(child, scale);
        }
    }

    public static void ApplyScaleToRectTransform(RectTransform rectTransform, Vector2? additionalScale = null)
    {
        PrefabUtility.RecordPrefabInstancePropertyModifications(rectTransform);
        Undo.RecordObject(rectTransform, "Apply Scale");

        var scale = additionalScale is null ? Vector2.one : additionalScale.Value;

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
