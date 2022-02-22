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

namespace UnityExtras.Core
{
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

        public static Vector3 ElementWiseMultiply(this Vector3 v1, Vector3 v2)
        {
            return new Vector3(v1.x * v2.x, v1.y * v2.y, v1.z * v2.z);
        }
    
        public static Vector2 Rotate(this Vector2 v, float degrees)
        {
            float sin = Mathf.Sin(degrees * Mathf.Deg2Rad);
            float cos = Mathf.Cos(degrees * Mathf.Deg2Rad);
         
            float tx = v.x;
            float ty = v.y;
            v.x = (cos * tx) - (sin * ty);
            v.y = (sin * tx) + (cos * ty);
            return v;
        }

        public static string ToPreciseString(this Vector3 v)
        {
            return $"{v.x:F4}, {v.y:F4}, {v.z:F4}";
        }

        #endregion

        #region Transform

        public static Pose GetPose(this Transform transform)
        {
            return new Pose(transform.position, transform.rotation);
        }

        public static Pose GetLocalPose(this Transform transform)
        {
            return new Pose(transform.localPosition, transform.localRotation);
        }

        public static void SetPose(this Transform transform, Pose pose)
        {
            transform.position = pose.position;
            transform.rotation = pose.rotation;
        }

        public static void SetLocalPose(this Transform transform, Pose localPose)
        {
            transform.localPosition = localPose.position;
            transform.localRotation = localPose.rotation;
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
                Object.DestroyImmediate( t.GetChild( i ).gameObject );
            }
        }
#endif

        public static void ResetLocalTransformInSpace(this Transform t, Matrix4x4 matrix4X4)
        {
            t.localPosition = matrix4X4.ExtractPosition();
            t.localRotation = matrix4X4.ExtractRotation();
            t.localScale = matrix4X4.ExtractScale();
        }

        #endregion

        #region GameObject

        /// <summary>
        /// Returns the full hierarchy name of the game object.
        /// </summary>
        /// <param name="gameObject">The game object.</param>
        public static string GetFullName(this GameObject gameObject)
        {
            string name = gameObject.name;
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
            if (gameObject && gameObject.activeSelf != active)
            {
                gameObject.SetActive(active);
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
        public static IEnumerator FadeCanvasAlpha(this CanvasGroup canvasGroup, float target, float fullTransitionDuration)
        {
            yield return Utilities.LerpOverTime(canvasGroup.alpha, target, fullTransitionDuration,
                f => { canvasGroup.alpha = f; });
        }

        /// <summary>
        /// Fades a canvas group's alpha from current to target value.
        /// </summary>
        /// <param name="canvasGroup">CanvasGroup that we're fading</param>
        /// <param name="target">The final alpha value</param>
        /// <param name="fullTransitionDuration">The time the fade would take across a 0->1 or 1->0 fade</param>
        /// <param name="setInteractableOnComplete">Sets the canvas group's interactable and blocks raycast settings to this value once fade finished</param>
        public static IEnumerator FadeCanvasAlpha(this CanvasGroup canvasGroup, float target, float fullTransitionDuration, bool setInteractableOnComplete)
        {
            yield return canvasGroup.FadeCanvasAlpha(target, fullTransitionDuration);
            canvasGroup.interactable = setInteractableOnComplete;
            canvasGroup.blocksRaycasts = setInteractableOnComplete;
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
        
        #region Coroutine

        public static void RestartCoroutine(this MonoBehaviour monoBehaviour, ref Coroutine coroutine, IEnumerator routine)
        {
            if (coroutine != null)
            {
                monoBehaviour.StopCoroutine(coroutine);
            }
            coroutine = monoBehaviour.StartCoroutine(routine);
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
            T[] enumerableArray = enumerable as T[] ?? enumerable.ToArray();
            foreach (T element in enumerableArray)
            {
                func(element);
            }

            return enumerableArray;
        }
        
        /// <summary>
        /// Applies a function to each element of an enumerable
        /// </summary>
        /// <param name="enumerable">The enumerable collection whose elements we'll apply the function</param>
        /// <param name="func">The function to apply with element index as second parameter</param>
        public static IEnumerable<T> ApplyFunctionWithIndex<T>(this IEnumerable<T> enumerable, Action<T, int> func)
        {
            T[] enumerableArray = enumerable as T[] ?? enumerable.ToArray();
            for (int index = 0; index < enumerableArray.Length; index++)
            {
                T element = enumerableArray[index];
                func(element, index);
            }

            return enumerableArray;
        }
    
        /// <summary>
        /// Method of getting a shuffled element from an array without having to shuffle the collection itself.
        /// Really useful when getting random, non-repeating elements of a large array.
        /// Functionally equivalent to reservoir sampling.
        /// </summary>
        /// <param name="enumerableCollection">The collection from which to select a random element</param>
        /// <param name="index">The index of the pseudo-shuffled collection to return</param>
        /// <param name="seed">The random seed. Keep constant during iteration.</param>
        public static T GetNext<T>(this IReadOnlyList<T> enumerableCollection, int index, int seed = 0)
        {
            int elementIndex = GetNextIndex(enumerableCollection, index, seed);
            T element = enumerableCollection[elementIndex];

            return element;
        }

        public static int GetNextIndex<T>(this IEnumerable<T> collection, int index, int seed = 0)
        {
            int[] primes =
            {
                1453, 1459, 1471, 1481, 1483, 1487, 1489, 1493, 1499, 1511,
                1523, 1531, 1543, 1549, 1553, 1559, 1567, 1571, 1579, 1583,
                1597, 1601, 1607, 1609, 1613, 1619, 1621, 1627, 1637, 1657,
            };

            int increment = primes[seed % primes.Length] * primes[(seed + 1) % primes.Length];

            int elementIndex = (index + 1) * increment % collection.Count();

            return elementIndex;
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
            for (int index = 0; index < asArray.Length; index++)
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
            return Mathf.Abs(sourceValue - compareValue) < Mathf.Epsilon;
        }

        #endregion

        #region TextMeshPro

        public static void SetTMPInputFieldValueChangedListener(this TMP_InputField tmpInputField, UnityAction<string> callback)
        {
            TMP_InputField.OnChangeEvent valueChanged = new TMP_InputField.OnChangeEvent();
            valueChanged.AddListener(callback);
            tmpInputField.onValueChanged = valueChanged;
        }

        #endregion

        #region Matrix

        public static Quaternion ExtractRotation(this Matrix4x4 matrix)
        {
            Vector3 forward;
            forward.x = matrix.m02;
            forward.y = matrix.m12;
            forward.z = matrix.m22;
 
            Vector3 upwards;
            upwards.x = matrix.m01;
            upwards.y = matrix.m11;
            upwards.z = matrix.m21;
 
            return Quaternion.LookRotation(forward, upwards);
        }
 
        public static Vector3 ExtractPosition(this Matrix4x4 matrix)
        {
            Vector3 position;
            position.x = matrix.m03;
            position.y = matrix.m13;
            position.z = matrix.m23;
            return position;
        }
 
        public static Vector3 ExtractScale(this Matrix4x4 matrix)
        {
            Vector3 scale;
            scale.x = new Vector4(matrix.m00, matrix.m10, matrix.m20, matrix.m30).magnitude;
            scale.y = new Vector4(matrix.m01, matrix.m11, matrix.m21, matrix.m31).magnitude;
            scale.z = new Vector4(matrix.m02, matrix.m12, matrix.m22, matrix.m32).magnitude;
            return scale;
        }

        #endregion
        
        #region Bits and Flags

        // Extension methods sourced from: https://stackoverflow.com/questions/93744/most-common-c-sharp-bitwise-operations-on-enums
        /// <summary>
        /// Checks whether an enum contains all flags in the argument '<paramref name="value">'
        /// /// Only works on enums that don't extend alternate numerical type - must be an int type only
        /// </summary>
        /// <param name="original">The type we're checking against</param>
        /// <param name="value">The flags that should all be present in the <paramref name="original"> enum</param>
        /// <typeparam name="T">An enum type</typeparam>
        /// <returns></returns>
        public static bool AllFlagsSet<T>(this Enum original, T value)
        {
            try
            {
                return ((int)(object)original & (int)(object)value) == (int)(object)value;
            }
            catch
            {
                return false;
            }
        }

        /// <summary>
        /// Checks whether an enum contains some of the flags in the argument <paramref name="value">.
        /// Only works on enums that don't extend alternate numerical type - must be an int type only
        /// </summary>
        /// <param name="original">The type we're checking against</param>
        /// <param name="value">The flags that could be present in the <paramref name="original"> enum</param>
        /// <typeparam name="T">An enum type</typeparam>
        /// <returns></returns>
        public static bool SomeFlagsSet<T>(this Enum original, T value)
        {
            try
            {
                int bitwiseOr = (int)(object)original & (int)(object)value;
                return bitwiseOr != 0;
            }
            catch
            {
                return false;
            }
        }

        /// <summary>
        /// Sets flags defined by the <paramref name="value"> argument on the enum <paramref name="original">
        /// /// Only works on enums that don't extend alternate numerical type - must be an int type only
        /// </summary>
        /// <param name="original">The enum we're applying the flags to</param>
        /// <param name="value">The flags we're applying</param>
        /// <typeparam name="T">An enum type</typeparam>
        /// <returns></returns>
        public static T SetFlag<T>(this Enum original, T value)
        {
            try
            {
                return (T)(object)((int)(object)original | (int)(object)value);
            }
            catch (Exception ex)
            {
                throw new ArgumentException($"Could not append value from enumerated type '{typeof(T).Name}'.", ex);
            }
        }

        public static bool IsFlagSet<T>(this Enum original, int value)
        {
            try
            {
                return ((int) (object) original & value) != 0;
            }
            catch (Exception ex)
            {
                throw new ArgumentException($"Could not append value from enumerated type '{typeof(T).Name}'.", ex);
            }
        }

        /// <summary>
        /// Removes flags defined by the <paramref name="value"> argument on the enum <paramref name="original">
        /// /// Only works on enums that don't extend alternate numerical type - must be an int type only
        /// </summary>
        /// <param name="original">The enum we're removing the flags from</param>
        /// <param name="value">The flags we're removing</param>
        /// <typeparam name="T">An enum type</typeparam>
        /// <returns></returns>
        public static T RemoveFlag<T>(this Enum original, T value)
        {
            try
            {
                return (T)(object)((int)(object)original & ~(int)(object)value);
            }
            catch (Exception ex)
            {
                throw new ArgumentException($"Could not remove value from enumerated type '{typeof(T).Name}'.", ex);
            }
        }

        /// <summary>
        /// Checks whether there is a single flag set on a flags enum, no matter where the position of that flag is
        /// </summary>
        public static bool IsSingleFlagSet<T>(this T enumValue) where T : Enum
        {
            try
            {
                int intValue = (int)(object)enumValue;
                return intValue != 0 && (intValue & (intValue - 1)) == 0;
            }
            catch (Exception ex)
            {
                throw new ArgumentException($"Could not remove value from enumerated type '{typeof(T).Name}'.", ex);
            }
        }

        /// <summary>
        /// Checks whether there is a single flag set on a flags enum, no matter where the position of that flag is
        /// </summary>
        public static bool IsSingleFlagSetOnByte<T>(this T enumValue) where T : Enum
        {
            try
            {
                byte byteValue = (byte)(object)enumValue;
                return byteValue != 0 && (byteValue & (byteValue - 1)) == 0;
            }
            catch (Exception ex)
            {
                throw new ArgumentException($"Could not remove value from enumerated type '{typeof(T).Name}'.", ex);
            }
        }

        public static T[] GetAllFlagValues<T>(this T _) where T : Enum
        {
            return Enum.GetValues(typeof(T)).Cast<T>().Where(p => p.IsSingleFlagSet()).ToArray();
        }

        #endregion
    }
}