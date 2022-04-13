// /------------------------------------------------------------------------\
// | Copyright (C) 2022 by Wojciech Wroński                                 |
// | This code is licensed under MIT license (see LICENSE.md for details)   |
// \------------------------------------------------------------------------/

using System.Linq;
using TMPro;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;

#if UNITY_EDITOR

public class UIScaler : EditorWindow
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

        #region SpecificComponent
        // There is no precompile tag that tells compiler if there is TMP so you have to manage this manualy
        if (rectTransform.TryGetComponent(out Text text))
        {
            Undo.RecordObject(text, "Apply Scale");
            text.fontSize = (int)(text.fontSize * Mathf.Min(scale.x, scale.y));
        }
        if (rectTransform.TryGetComponent(out ScrollRect scrollRect))
        {
            Undo.RecordObject(scrollRect, "Apply Scale");
            scrollRect.horizontalScrollbarSpacing *= scale.x;
            scrollRect.verticalScrollbarSpacing *= scale.y;
        }
        if (rectTransform.TryGetComponent(out HorizontalOrVerticalLayoutGroup layoutGroup))
        {
            Undo.RecordObject(layoutGroup, "Apply Scale");
            layoutGroup.padding.left = (int)(layoutGroup.padding.left * scale.x);
            layoutGroup.padding.right = (int)(layoutGroup.padding.right * scale.x);
            layoutGroup.padding.top = (int)(layoutGroup.padding.top * scale.y);
            layoutGroup.padding.bottom = (int)(layoutGroup.padding.bottom * scale.y);
            layoutGroup.spacing *= Mathf.Min(scale.x, scale.y);
        }
        if (rectTransform.TryGetComponent(out Shadow shadow))
        {
            Undo.RecordObject(shadow, "Apply Scale");
            shadow.effectDistance *= scale;
        }
        #region TMP
        if (rectTransform.TryGetComponent(out TMP_Text TMPtext))
        {
            Undo.RecordObject(TMPtext, "Apply Scale");
            TMPtext.fontSize *= Mathf.Min(scale.x, scale.y);
            TMPtext.margin.Scale(new Vector4(scale.x, scale.y, scale.x, scale.y));
        }

        if (rectTransform.TryGetComponent(out TMP_InputField TMPInputfield))
        {
            Undo.RecordObject(TMPInputfield, "Apply Scale");
            TMPInputfield.pointSize *= Mathf.Min(scale.x, scale.y);
        }
        #endregion
        #endregion

        EditorUtility.SetDirty(rectTransform);
    }
}

#endif
