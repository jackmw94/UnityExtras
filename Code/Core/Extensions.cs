using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;
using Object = UnityEngine.Object;

public static class Extensions
{
    #region Vector

    /// <summary>
    /// Allows you to change a single element of a Vector3 inline.
    /// E.g. if you want the position of a point at ground level you can write
    /// 'point.ModifyVectorElement(1,0f)' rather than make a temporary variable
    /// </summary>
    /// <param name="vector">The original vector</param>
    /// <param name="element">The index of the element to modify; x=0, y=1, z=2</param>
    /// <param name="modifiedValue">The value to set the specified element to</param>
    /// <returns>The original vector with the element at index change to newValue</returns>
    public static Vector3 ModifyVectorElement(this Vector3 vector, int element, float modifiedValue)
    {
        vector[element] = modifiedValue;
        return vector;
    }

    /// <summary>
    /// Allows you to change a single element of a Vector2 inline
    /// </summary>
    /// <param name="vector">The original vector</param>
    /// <param name="element">The index of the element to modify; x=0, y=1</param>
    /// <param name="modifiedValue">The value to set the specified element to</param>
    /// <returns>The original vector with the element at index change to newValue</returns>
    public static Vector2 ModifyVectorElement(this Vector2 vector, int element, float modifiedValue)
    {
        vector[element] = modifiedValue;
        return vector;
    }

    /// <summary>
    /// Explicitly converts a vector3 to a vector2, keeping only the X and Y elements
    /// </summary>
    /// <param name="vector3">The original Vector3</param>
    /// <returns>The Vector2 with only x and y elements</returns>
    public static Vector2 XY(this Vector3 vector3)
    {
        return new Vector2(vector3.x, vector3.y);
    }
    
    /// <summary>
    /// Converts a vector3 to a vector2, keeping only the X and Z elements. Useful for getting the top-down 2D position of a 3D point
    /// </summary>
    /// <param name="vector3">The original Vector3</param>
    /// <returns>The Vector2 with only X and Z elements</returns>
    public static Vector2 XZ(this Vector3 vector3)
    {
        return new Vector2(vector3.x, vector3.y);
    }

    #endregion

    #region GameObject

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
        if (gameObject.activeSelf != active)
        {
            gameObject.SetActive(active);
        }
    }
    
    /// <summary>
    /// Destroys all child objects of a transform
    /// </summary>
    /// <param name="t">The transform whose children we want to destroy</param>
    public static void DestroyAllChildren( this Transform t )
    {
        for ( int i = t.childCount - 1; i >= 0; i-- )
        {
            Object.Destroy( t.GetChild( i ).gameObject );
        }
    }

#if UNITY_EDITOR
    /// <summary>
    /// Destroys all child objects of a transform in editor. WARNING: this can delete work, use with caution
    /// </summary>
    /// <param name="t">The transform whose children we want to destroy</param>
    public static void DestroyAllChildrenInEditor( this Transform t )
    {
        for ( int i = t.childCount - 1; i >= 0; i-- )
        {
            Object.DestroyImmediate( t.GetChild( i ) );
        }
    }
#endif

    #endregion

    #region CanvasGroup and Image

    /// <summary>
    /// Fades a canvas group's alpha from current to target value.
    /// </summary>
    /// <param name="canvasGroup">CanvasGroup that we're fading</param>
    /// <param name="target">The final alpha value</param>
    /// <param name="fullTransitionDuration">The time the fade would take across a 0->1 or 1->0 fade</param>
    public static IEnumerator FadeCanvasAlpha(this CanvasGroup canvasGroup, float target, float fullTransitionDuration)
    {
        yield return Utilities.LerpOverTime(canvasGroup.alpha, target, fullTransitionDuration,
            f => { canvasGroup.alpha = f; });
    }

    /// <summary>
    /// Fades an image's alpha from current to target value.
    /// </summary>
    /// <param name="image">CanvasGroup that we're fading</param>
    /// <param name="target">The final alpha value</param>
    /// <param name="fullTransitionDuration">The time the fade would take across a 0->1 or 1->0 fade</param>
    public static IEnumerator FadeImageAlpha(this Image image, float target, float fullTransitionDuration)
    {
        yield return Utilities.LerpOverTime(image.color.a, target, fullTransitionDuration, image.SetImageAlpha);
    }

    /// <summary>
    /// Sets an images alpha without changing the rgb values of the colour
    /// </summary>
    /// <param name="image">The image in quesiton</param>
    /// <param name="alpha">The desired alpha value</param>
    public static void SetImageAlpha(this Image image, float alpha)
    {
        Color changeColourAlpha = image.color;
        changeColourAlpha.a = alpha;
        image.color = changeColourAlpha;
    }

    #endregion

    #region Colour

    /// <summary>
    /// Add colour tags to a string. This means it will display in the specified colour in UI elements that support colour tags, such as TextMeshPro
    /// </summary>
    /// <param name="str">The original string we want to be coloured</param>
    /// <param name="colour">The colour in which we want the string to be shown</param>
    /// <returns></returns>
    public static string AddColourTagsToString(this string str, Color colour)
    {
        string colourHex = ColorUtility.ToHtmlStringRGBA(colour);
        return $"<color=#{colourHex}>{str}</color>";
    }

    #endregion

    #region AnimationCurve

    /// <summary>
    /// Gets the 'duration' of a curve. Note this checks the point at which the last key occurs, it assumes that the curve will start at zero even if the first key is not there.
    /// </summary>
    /// <param name="curve">The curve whose duration we want to obtain</param>
    /// <returns>The duration of the curve</returns>
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

    /// <summary>
    /// Applies a function to each element of an enumerable
    /// </summary>
    /// <param name="enumerable">The enumerable collection whose elements we'll apply the function</param>
    /// <param name="func">The function to apply</param>
    public static IEnumerable<T> ApplyFunction<T>(this IEnumerable<T> enumerable, Action<T> func)
    {
        foreach (T element in enumerable)
        {
            func(element);
        }

        return enumerable;
    }
    
    /// <summary>
    /// Method of getting a shuffled element from an array without having to shuffle the collection itself.
    /// Really useful when getting random, non-repeating elements of a large array.
    /// Functionally equivalent to reservoir sampling.
    /// </summary>
    /// <param name="collection">The collection from which to select a random element</param>
    /// <param name="index">The index of the pseudo-shuffled collection to return</param>
    /// <param name="seed">The random seed. Keep constant during iteration.</param>
    public static T GetNext<T>(this T[] collection, int index, int seed = 0)
    {
        int[] primes =
        {
            1453, 1459, 1471, 1481, 1483, 1487, 1489, 1493, 1499, 1511,
            1523, 1531, 1543, 1549, 1553, 1559, 1567, 1571, 1579, 1583,
            1597, 1601, 1607, 1609, 1613, 1619, 1621, 1627, 1637, 1657,
        };

        var increment = primes[seed % primes.Length] * primes[(seed + 1) % primes.Length];

        var elementIndex = ((index + 1) * increment) % collection.Length;

        T element = collection[elementIndex];

        return element;
    }

    /// <summary>
    /// Creates a string from the elements of a collection, separated by the specified separator
    /// </summary>
    /// <param name="enumerable">The collection of elements from which to create a string</param>
    /// <param name="elementSeparator">The string that separates the elements in the resultant string</param>
    /// <param name="includeSeparatorAtEnd">Whether or not to add the separator string after the final element</param>
    public static string JoinToString<T>(this IEnumerable<T> enumerable, string elementSeparator, bool includeSeparatorAtEnd = false)
    {
        StringBuilder stringBuilder = new StringBuilder();
        var asArray = enumerable.ToArray();
        for (var index = 0; index < asArray.Length; index++)
        {
            T elem = asArray[index];
            stringBuilder.Append(elem);
            if (index != asArray.Length - 1 || includeSeparatorAtEnd)
            {
                stringBuilder.Append(elementSeparator);
            }
        }
        return stringBuilder.ToString();
    }

    #endregion

    #region String

    /// <summary>
    /// Check if two strings have equivalent contents regardless of their case
    /// </summary>
    /// <param name="source">The source string</param>
    /// <param name="toCheck">The string to compare to the source string</param>
    /// <returns>Whether or not the two strings are equal regardless of case</returns>
    public static bool EqualsIgnoreCase(this string source, string toCheck)
    {
        return source.Equals(toCheck, StringComparison.OrdinalIgnoreCase);
    }

    /// <summary>
    /// Check if the source string contains another string, disregarding their case
    /// </summary>
    /// <param name="source">The source string</param>
    /// <param name="toCheck">The string that we're testing whether is present in the source string</param>
    /// <returns>Whether or not the toCheck string exists in the source string, regardless of case</returns>
    public static bool ContainsIgnoreCase(this string source, string toCheck)
    {
        return source.IndexOf(toCheck, StringComparison.OrdinalIgnoreCase) >= 0;
    }

    #endregion

    #region Numbers

    public static bool Approximately(this float sourceValue, float compareValue)
    {
        return Math.Abs(sourceValue - compareValue) < Mathf.Epsilon;
    }

    #endregion

    #region TextMeshPro

    public static void OnTMPInputFieldValueChanged(this TMP_InputField tmpInputField, UnityAction<string> callback)
    {
        TMP_InputField.OnChangeEvent valueChanged = new TMP_InputField.OnChangeEvent();
        valueChanged.AddListener(callback);
        tmpInputField.onValueChanged = valueChanged;
    }

    #endregion
}