using System.Linq;
using UnityEditor;
using UnityEngine;

#if UNITY_EDITOR

public class SelectAllOfTag : EditorWindow
{

    [MenuItem("UI/Apply Scale To Selected Objects")]
    private static void ApplyScaleToSelectedObjects()
    {
        for (int i = 0; i < Selection.objects.Length; ++i)
        {
            var obj = Selection.objects[i];
            if (obj.GetType() == typeof(GameObject) && ((GameObject)obj).TryGetComponent(out RectTransform rectTransform))
            {
                Undo.RecordObject(rectTransform, "Apply Scale");
                PrefabUtility.RecordPrefabInstancePropertyModifications(rectTransform);
                rectTransform.ApplyScale();
            }
        }
        Debug.Log("Scale applied for selected objects");
    }

    [MenuItem("UI/Apply Scale To Selected Objects and its children")]
    private static void ApplyScaleToSelectedObjectsAndItsChildren()
    {
        for (int i = 0; i < Selection.objects.Length; ++i)
        {
            var obj = Selection.objects[i];
            if (obj.GetType() == typeof(GameObject) && ((GameObject)obj).TryGetComponent(out RectTransform rectTransform))
            {
                ApplyScaleToAllChildren(rectTransform);
            }
        }
        Debug.Log("Scale applied for selected objects and their children");
    }

    [MenuItem("UI/How Many Objects With Rects Are Selected?")]
    private static void HowManyObjectsWithRectsAreSelected()
    {
        var counter = 0;
        for (int i = 0; i < Selection.objects.Length; ++i)
        {
            var obj = Selection.objects[i];
            if (obj.GetType() == typeof(GameObject) && ((GameObject)obj).TryGetComponent(out RectTransform rectTransform))
            {
                counter++;
            }
        }
        Debug.Log("Number of selected objects with RectTransform is: " + counter);
    }

    private static void ApplyScaleToAllChildren(RectTransform rectTransform)
    {
        var scale = rectTransform.localScale;
        Undo.RecordObject(rectTransform, "Apply Scale");
        rectTransform.ApplyScale();

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

    private static void ApplyScaleToAllChildren(RectTransform rectTransform, Vector2 additionalScale)
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
