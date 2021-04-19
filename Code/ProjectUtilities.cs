using System;
using System.Collections;
using TMPro;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;
using Object = UnityEngine.Object;

public static class ProjectUtilities
{
    public static double InverseLerpDoubleUnclamped(double from, double to, double value)
    {
        return (value - from) / (to - from);
    }
    
    
    public static IEnumerator LerpOverTime(float initialValue, float targetValue, float fullTransitionDuration, Action<float> applyLerpedValue, AnimationCurve zeroToOneCurve = null)
    {
        var start = initialValue;
        var fraction = 0f;
        var startTime = Time.time;

        if (fullTransitionDuration <= 0f)
        {
            applyLerpedValue(targetValue);
            yield break;
        }

        if (Mathf.Abs(targetValue - start) < Mathf.Epsilon)
        {
            applyLerpedValue(targetValue);
            yield break;
        }

        var duration = fullTransitionDuration * Mathf.Abs(targetValue - start);

        while (fraction < 1f)
        {
            yield return null;
            fraction = (Time.time - startTime) / duration;
            if (zeroToOneCurve != null)
            {
                fraction = zeroToOneCurve.Evaluate(fraction);
            }
            var value = Mathf.Lerp(start, targetValue, fraction);
            applyLerpedValue(value);
        }

        applyLerpedValue(targetValue);
    }
    
    public static (float screenX, float screenY) GetScreenSpaceCoordinates(OnlineMaps map, Vector2 position)
    {
        map.GetCorners(out double topLeftX, out double topLeftY, out double bottomRightX, out double bottomRightY);
        double screenX = InverseLerpDoubleUnclamped(topLeftX, bottomRightX, position.x);
        double screenY = InverseLerpDoubleUnclamped(bottomRightY, topLeftY, position.y);

        return ((float) screenX, (float) screenY);
    }
    
    public static void PositionTransformOnMap(RectTransform rectTransform, OnlineMaps map, Vector2 position)
    {
        (float screenX, float screenY) = GetScreenSpaceCoordinates(map, position);
        rectTransform.anchorMin = new Vector2(screenX, screenY);
        rectTransform.anchorMax = new Vector2(screenX, screenY);
    }
    
    #if UNITY_EDITOR
    [MenuItem("Tools/Apply font to scene")]
    private static void ApplyFontToScene()
    {
        var selectedGuids = Selection.assetGUIDs;
        if (selectedGuids.Length > 1)
        {
            Debug.LogError("Cannot perform this action when multiple assets are selected");
            return;
        }

        if (selectedGuids.Length == 0)
        {
            Debug.LogError("Cannot perform this action when there are no assets selected");
            return;
        }

        var guid = selectedGuids[0];
        var path = AssetDatabase.GUIDToAssetPath(guid);
        var font = AssetDatabase.LoadAssetAtPath<Font>(path);
        var tmpFont = AssetDatabase.LoadAssetAtPath<TMP_FontAsset>(path);

        if (!font && !tmpFont)
        {
            Debug.Log($"Could not load font or tmpro font from path {path}");
            return;
        }
        
        if (font)
        {
            var allText = Object.FindObjectsOfType<Text>(true);
            allText.ApplyFunction(text =>
            {
                text.font = font;
                EditorUtility.SetDirty(text);
            });
            Debug.Log($"Applied font to {allText.Length} text components");
        }

        if (tmpFont)
        {
            var allTMProText = Object.FindObjectsOfType<TextMeshProUGUI>(true);
            allTMProText.ApplyFunction(t =>
            {
                t.font = tmpFont;
                EditorUtility.SetDirty(t);
            });
            Debug.Log($"Applied tmpro font to {allTMProText.Length} text mesh pro components");
        }
    }
    #endif
}