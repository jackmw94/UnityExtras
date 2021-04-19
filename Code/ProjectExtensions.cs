using System;
using System.Collections;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using UnityEngine;
using UnityEngine.UI;
using Object = UnityEngine.Object;

#if UNITY_EDITOR
using UnityEditor;
#endif

public static class ProjectExtensions
{
    #region Transform

    /// <summary>
    /// Recursive method that traverses the transform hierachy to find a child to <paramref name="transform"> with the given <paramref name="name"/>.
    /// This method can return null and will not throw
    /// </summary>
    public static GameObject FindChildInHierarchy(this Transform transform, string name)
    {
        GameObject result = null;
        for (var i = 0; i < transform.childCount; i++)
        {
            Transform child = transform.GetChild(i);
            // Return if we find a gameobject by the name we are looking for
            if (child.name == name)
            {
                return child.gameObject;
            }

            if (child.childCount > 0)
            {
                // If this branch returns a non-null result, take that as the new result
                GameObject branchResult = FindChildInHierarchy(child, name);

                // Cannot use null coalescing operators with unity objects
                if (branchResult != null)
                {
                    result = branchResult;
                }
            }
        }

        return result;
    }

    public static void SetAnchors(this RectTransform rectTransform, Anchors anchors)
    {
        rectTransform.anchorMin = anchors.MinAnchors;
        rectTransform.anchorMax = anchors.MaxAnchors;
    }

    public static Anchors GetAnchors(this RectTransform rectTransform)
    {
        return new Anchors
        {
            MinAnchors = rectTransform.anchorMin,
            MaxAnchors = rectTransform.anchorMax
        };
    }

    #endregion

    #region Vector3

    public static Vector3 ModifyVectorElement(this Vector3 vector, int element, float newValue)
    {
        vector[element] = newValue;
        return vector;
    }

    public static Vector2 ModifyVectorElement(this Vector2 vector, int element, float newValue)
    {
        vector[element] = newValue;
        return vector;
    }

    #endregion

    #region GameObject

    public static T AddOrReplaceComponent<T>(this GameObject gameObject) // TODO: Delete. Doesn't work for rigidbodies
        where T : Component
    {
        if (gameObject.TryGetComponent(out T component))
        {
            Object.Destroy(component);
        }

        return gameObject.AddComponent<T>();
    }

    public static T GetOrAddComponent<T>(this GameObject gameObject) where T : Component
    {
        T component = gameObject.GetComponent<T>();

        if (!component)
        {
            component = gameObject.AddComponent<T>();
        }

        return component;
    }

    /// <summary>
    /// Setting a gameobject as active/inactive is expensive and there is no internal optimisation if you're setting it to the state it's already in.
    /// This function just checks that you're actually making a change to a gameobject's active state before calling the SetActive function
    /// </summary>
    /// <param name="gameObject">The gameobject in question</param>
    /// <param name="active">Whether or not it should be active</param>
    public static void SetActiveSafe(this GameObject gameObject, bool active)
    {
        if (gameObject.activeSelf ^ active)
        {
            gameObject.SetActive(active);
        }
    }

    public static void SetLayerRecursively(this GameObject obj, string newLayer)
    {
        int layerMask = LayerMask.NameToLayer(newLayer);
        obj.SetLayerRecursively(layerMask);
    }

    /// <summary>
    /// Setting gameObject.layer only changes the current object, not including its children
    /// This function sets the layer of the obj argument and all child objects below it
    /// </summary>
    /// <param name="obj">Root gameobject from which to start setting the new layer</param>
    /// <param name="newLayer">The new layer value that obj and its children will be set to</param>
    public static void SetLayerRecursively(this GameObject obj, int newLayer)
    {
        if (null == obj)
        {
            return;
        }

        obj.layer = newLayer;

        foreach (Transform child in obj.transform)
        {
            if (null == child)
            {
                continue;
            }

            child.gameObject.SetLayerRecursively(newLayer);
        }
    }

    #endregion

    #region CanvasGroup and Image

    /// <summary>
    /// Fades a canvas group's alpha from current to target value.
    /// </summary>
    /// <param name="canvasGroup">CanvasGroup that we're fading</param>
    /// <param name="target">The final alpha value</param>
    /// <param name="fullTransitionDuration">The time the fade would take across a 0->1 or 1->0 fade</param>
    /// <returns></returns>
    public static IEnumerator FadeCanvasAlpha(this CanvasGroup canvasGroup, float target, float fullTransitionDuration)
    {
        yield return ProjectUtilities.LerpOverTime(canvasGroup.alpha, target, fullTransitionDuration,
            f => { canvasGroup.alpha = f; });
    }

    /// <summary>
    /// Fades an image's alpha from current to target value.
    /// </summary>
    /// <param name="image">CanvasGroup that we're fading</param>
    /// <param name="target">The final alpha value</param>
    /// <param name="fullTransitionDuration">The time the fade would take across a 0->1 or 1->0 fade</param>
    /// <returns></returns>
    public static IEnumerator FadeImageAlpha(this Image image, float target, float fullTransitionDuration)
    {
        yield return ProjectUtilities.LerpOverTime(image.color.a, target, fullTransitionDuration, image.SetImageAlpha);
    }

    public static void SetImageAlpha(this Image image, float alpha)
    {
        Color changeColourAlpha = image.color;
        changeColourAlpha.a = alpha;
        image.color = changeColourAlpha;
    }

    public static void SetImageColourNotAlpha(this Image image, Color colour)
    {
        colour.a = image.color.a;
        image.color = colour;
    }

    /// <summary>
    /// Returns the full hierarchy name of the game object.
    /// </summary>
    /// <param name="gameObject">The game object.</param>
    public static string GetFullName(this GameObject gameObject)
    {
        var name = gameObject.name;
        while (gameObject.transform.parent != null)
        {
            gameObject = gameObject.transform.parent.gameObject;
            name = gameObject.name + "/" + name;
        }

        return name;
    }

    #endregion

    #region Colour

    public static string AddColourTags(this string str, Color colour)
    {
        string colourHex = ColorUtility.ToHtmlStringRGBA(colour);
        return $"<color=#{colourHex}>{str}</color>";
    }

    #endregion

    #region AnimationCurve

    public static float GetCurveDuration(this AnimationCurve curve)
    {
        if (curve.keys.Length == 0)
        {
            return 0f;
        }

        int lastKey = curve.keys.Length - 1;
        return curve.keys[lastKey].time;
    }

    #endregion

    #region Collections

    public static IEnumerable<T> ApplyFunction<T>(this IEnumerable<T> enumerable, Action<T> func)
    {
        foreach (T element in enumerable)
        {
            func(element);
        }

        return enumerable;
    }

    #endregion

    #region String

    public static string ReplaceNthOccurence(this string obj, string find, string replace, int nthOccurance, bool removeCharactersAfterMatch = false)
    {
        if (nthOccurance > 0)
        {
            MatchCollection matchCollection = Regex.Matches(obj, Regex.Escape(find));
            if (matchCollection.Count >= nthOccurance)
            {
                Match match = matchCollection[nthOccurance - 1];
                if (removeCharactersAfterMatch)
                {
                    return obj.Remove(match.Index, obj.Length - match.Index).Insert(match.Index, replace);
                }

                return obj.Remove(match.Index, match.Length).Insert(match.Index, replace);
            }
        }

        return obj;
    }

    #endregion

    #region Properties

    #if UNITY_EDITOR
    public static object GetPropertyValue(this SerializedProperty property)
    {
        if (property is null)
        {
            throw new ArgumentNullException(nameof(property));
        }

        switch (property.propertyType)
        {
            case SerializedPropertyType.Integer:
                return property.intValue;
            case SerializedPropertyType.Boolean:
                return property.boolValue;
            case SerializedPropertyType.Float:
                return property.floatValue;
            case SerializedPropertyType.String:
                return property.stringValue;
            case SerializedPropertyType.Color:
                return property.colorValue;
            case SerializedPropertyType.ObjectReference:
                return property.objectReferenceValue;
            case SerializedPropertyType.LayerMask:
                return (LayerMask) property.intValue;
            case SerializedPropertyType.Enum:
                return property.enumValueIndex;
            case SerializedPropertyType.Vector2:
                return property.vector2Value;
            case SerializedPropertyType.Vector3:
                return property.vector3Value;
            case SerializedPropertyType.Vector4:
                return property.vector4Value;
            case SerializedPropertyType.Rect:
                return property.rectValue;
            case SerializedPropertyType.ArraySize:
                return property.arraySize;
            case SerializedPropertyType.Character:
                return (char) property.intValue;
            case SerializedPropertyType.AnimationCurve:
                return property.animationCurveValue;
            case SerializedPropertyType.Bounds:
                return property.boundsValue;
            case SerializedPropertyType.Gradient:
                throw new InvalidOperationException("Can not handle Gradient types.");
            default:
                throw new InvalidOperationException($"Can not handle property type: {property.propertyType}");
        }
    }
    #endif

    #endregion
}