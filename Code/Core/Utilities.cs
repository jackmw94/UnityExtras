using System;
using System.Collections;
using UnityEngine;
using Random = System.Random;

namespace UnityExtras.Code.Core
{
    public static class Utilities
    {
        /// <summary>
        /// Lerps from an initial value to a target value over a set duration
        /// </summary>
        /// <param name="initialValue">The starting value</param>
        /// <param name="targetValue">The target value</param>
        /// <param name="zeroToOneDuration">The normalised duration, how long the lerp would take if the initial and target values had an absolute difference of 1f</param>
        /// <param name="applyValueFunction">The callback, passing the in-progress lerped value</param>
        /// <param name="zeroToOneCurve">An optional curve to alter the lerp value. Note that if this doesn't map from 0->0 and 1->1 then there might be some snapping at the start or end</param>
        public static IEnumerator LerpOverTime(float initialValue, float targetValue, float zeroToOneDuration, Action<float> applyValueFunction, AnimationCurve zeroToOneCurve = null)
        {
            var start = initialValue;
            var lerpValue = 0f;
            var startTime = Time.time;

            if (zeroToOneDuration <= 0f)
            {
                applyValueFunction(targetValue);
                yield break;
            }

            if (Mathf.Abs(targetValue - start) < Mathf.Epsilon)
            {
                applyValueFunction(targetValue);
                yield break;
            }

            var duration = zeroToOneDuration * Mathf.Abs(targetValue - start);

            while (lerpValue < 1f)
            {
                yield return null;
                lerpValue = (Time.time - startTime) / duration;

                float curvedValue = lerpValue;
                if (zeroToOneCurve != null)
                {
                    curvedValue = zeroToOneCurve.Evaluate(lerpValue);
                }
                var value = Mathf.Lerp(start, targetValue, curvedValue);
                applyValueFunction(value);
            }

            float curvedTarget = targetValue;
            if (zeroToOneCurve != null)
            {
                curvedTarget = zeroToOneCurve.Evaluate(targetValue);
            }
            applyValueFunction(curvedTarget);
        }

        /// <summary>
        /// A function to get perlin noise colour - colour that has a continuously noisy hue rotating around the colour wheel
        /// An application of this function could be to set the tint of an HDR material on a TV screen to get the
        /// impression of a bright fluctuating screen without having to show any actual footage
        /// </summary>
        /// <param name="noiseFrequency">The rate at which the colour changes</param>
        /// <param name="constantSaturation">The saturation for the HSV colour</param>
        /// <param name="constantValue">The value for the HSV colour</param>
        /// <returns>The colour</returns>
        public static Color PerlinNoiseColour(float noiseFrequency, float constantSaturation, float constantValue)
        {
            float hue = Mathf.PerlinNoise(Time.time * noiseFrequency, 0f);
            Color colour = Color.HSVToRGB(hue, constantSaturation, constantValue);
            return colour;
        }
    
    
        /// <summary>
        /// Finds the shortest distance from point to the line running from lineStart to lineEnd
        /// </summary>
        /// <param name="closest">The position on the line that has the shortest distance to point</param>
        /// <returns>The shortest distance from the point to the line</returns>
        public static float FindDistanceToLineSegment(Vector2 point, Vector2 lineStart, Vector2 lineEnd, out Vector2 closest)
        {
            float dx = lineEnd.x - lineStart.x;
            float dy = lineEnd.y - lineStart.y;
            if ((dx == 0) && (dy == 0))
            {
                // It's a point not a line segment.
                closest = lineStart;
                dx = point.x - lineStart.x;
                dy = point.y - lineStart.y;
                return Mathf.Sqrt(dx * dx + dy * dy);
            }

            // Calculate the t that minimizes the distance.
            float t = ((point.x - lineStart.x) * dx + (point.y - lineStart.y) * dy) /
                      (dx * dx + dy * dy);

            // See if this represents one of the segment's
            // end points or a point in the middle.
            if (t < 0)
            {
                closest = new Vector2(lineStart.x, lineStart.y);
                dx = point.x - lineStart.x;
                dy = point.y - lineStart.y;
            }
            else if (t > 1)
            {
                closest = new Vector2(lineEnd.x, lineEnd.y);
                dx = point.x - lineEnd.x;
                dy = point.y - lineEnd.y;
            }
            else
            {
                closest = new Vector2(lineStart.x + t * dx, lineStart.y + t * dy);
                dx = point.x - closest.x;
                dy = point.y - closest.y;
            }

            return Mathf.Sqrt(dx * dx + dy * dy);
        }
	
        /// <summary>
        /// A wrapper around a hard-to-remember call to spherically lerp to a target. Speed reduces as the target gets closer to give a less mechanical and more natural rotation
        /// </summary>
        public static Quaternion SlerpToTarget(Quaternion currentRotation, Quaternion targetRotation, float speed)
        {
            return Quaternion.Slerp(currentRotation, targetRotation, 1f - Mathf.Exp(-speed * Time.deltaTime));
        }
        
        /// <summary>
        /// A negative-safe modulo function
        /// In C#: -1 % 3 == -1
        /// This function accounts for negatives, ensuring that it returns a value between 0(inc) and m(exc)
        /// Mod(-1, 3) == 2
        /// </summary>
        public static int Mod(int x, int m) 
        {
            int r = x % m;
            return r<0 ? r+m : r;
        }

        /// <summary>
        /// Generates a random long value. Good for unique ids.
        /// </summary>
        public static long RandomLong(int seed)
        {
            byte[] buf = new byte[8];
            Random rand = new Random(seed);
            rand.NextBytes(buf);
            long longRand = BitConverter.ToInt64(buf, 0);
            return longRand;
        }
    }
}