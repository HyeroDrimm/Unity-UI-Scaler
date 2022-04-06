using System.Linq;
using UnityEditor;
using UnityEngine;

#if UNITY_EDITOR

public class SelectAllOfTag : EditorWindow
{
    [MenuItem("UI/Scaler/Apply Scale To Selected TransformRects")]
    private static void ApplyScaleToSelectedRects()
    {
        for (int i = 0; i < Selection.objects.Length; ++i)
        {
            var obj = Selection.objects[i];
            if (obj is GameObject && ((GameObject)obj).TryGetComponent(out RectTransform rectTransform))
            {
                Undo.RecordObject(rectTransform, "Apply Scale");
                PrefabUtility.RecordPrefabInstancePropertyModifications(rectTransform);
                rectTransform.ApplyScale();
            }
        }
        Debug.Log("Scale applied for selected objects");
    }

    [MenuItem("UI/Scaler/Apply Scale To Selected TransformRects and their children")]
    private static void ApplyScaleToSelectedTransformRectssAndTheirChildren()
    {
        for (int i = 0; i < Selection.objects.Length; ++i)
        {
            var obj = Selection.objects[i];
            if (obj is GameObject && ((GameObject)obj).TryGetComponent(out RectTransform rectTransform))
            {
                ApplyScaleToAllChildren(rectTransform);
            }
        }
        Debug.Log("Scale applied for selected objects and their children");
    }

    [MenuItem("UI/Scaler/How Many TransformRects Are Selected?")]
    private static void HowManyRectTrasformsWithRectsAreSelected()
    {
        var counter = 0;
        for (int i = 0; i < Selection.objects.Length; ++i)
        {
            var obj = Selection.objects[i];
            if (obj is GameObject && ((GameObject)obj).TryGetComponent(out RectTransform rectTransform))
                counter++;
        }
        Debug.Log("Number of selected RectTransforms is: " + counter);
    }

    private static void ApplyScaleToAllChildren(RectTransform rectTransform, Vector2 additionalScale = Vector2.one)
    {
        var scale = rectTransform.localScale * additionalScale;
        Undo.RecordObject(rectTransform, "Apply Scale");
        rectTransform.ApplyScale(scale);
        rectTransform.anchoredPosition *= scale;

        if (rectTransform.TryGetComponent(out TMPro.TMP_Text text))
        {
            Undo.RecordObject(text, "Apply Scale");
            text.fontSize *= Mathf.Min(scale.x, scale.y);
        }

        RectTransform[] childs = rectTransform.Cast<RectTransform>().ToArray();
        for (int i = 0; i < childs.Length; ++i)
        {
            ApplyScaleToAllChildren(childs[i], scale);
        }
    }
}

#endif
