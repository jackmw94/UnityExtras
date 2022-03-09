using System;
using System.Collections;
using UnityEngine;
using Random = System.Random;

namespace UnityExtras.Core
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
            float start = initialValue;
            float lerpValue = 0f;
            float startTime = Time.time;

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

            float duration = zeroToOneDuration * Mathf.Abs(targetValue - start);

            while (lerpValue < 1f)
            {
                yield return null;
                lerpValue = (Time.time - startTime) / duration;

                float curvedValue = lerpValue;
                if (zeroToOneCurve != null)
                {
                    curvedValue = zeroToOneCurve.Evaluate(lerpValue);
                }
                float value = Mathf.Lerp(start, targetValue, curvedValue);
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
        /// <returns>The shortest distance from the point to the line</returns>
        [Obsolete("Use GetClosestPointOnLine instead. It uses 3D vectors and has a simpler implementation")]
        public static float FindDistanceToLineSegment(Vector2 point, Vector2 lineStart, Vector2 lineEnd, bool limitToLineSegment, out Vector2 closest)
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
            if (t < 0 && limitToLineSegment)
            {
                closest = new Vector2(lineStart.x, lineStart.y);
                dx = point.x - lineStart.x;
                dy = point.y - lineStart.y;
            }
            else if (t > 1 && limitToLineSegment)
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
        
        public static Vector3 GetClosestPointOnLine(Vector3 point, Vector3 lineStart, Vector3 lineEnd, bool keepPointWithLine)
        {
            Vector3 pointProjection = Vector3.Project(point - lineStart, lineEnd - lineStart);

            if (!keepPointWithLine)
            {
                return lineStart + pointProjection;
            }
            
            Vector3 lineVector = lineEnd - lineStart;
            float projectionMagnitude = pointProjection.magnitude;
            
            if (Vector3.Dot(lineVector, pointProjection) < 0)
            {
                // point projection opposite direction to line, constrain to start of line
                return lineStart;
            }

            if (projectionMagnitude > lineVector.magnitude)
            {
                // projection longer than line itself, constrain to end of line
                return lineEnd;
            }

            // projection within line
            return lineStart + pointProjection;
        }
        
        /// <summary>
        /// Two non-parallel lines which may or may not touch each other have a point on each line which are closest
        /// to each other. This function finds those two points. If the lines are not parallel, the function 
        /// outputs true, otherwise false.
        /// </summary>
        public static bool ClosestPointsOnTwoLines(out Vector3 closestPointLine1, out Vector3 closestPointLine2, Vector3 linePoint1, Vector3 lineVec1, Vector3 linePoint2, Vector3 lineVec2)
        {
            closestPointLine1 = Vector3.zero;
            closestPointLine2 = Vector3.zero;

            float a = Vector3.Dot(lineVec1, lineVec1);
            float b = Vector3.Dot(lineVec1, lineVec2);
            float e = Vector3.Dot(lineVec2, lineVec2);

            float d = a * e - b * b;

            if (d.Equals(0.0f))
            {
                return false;
            }
            
            Vector3 r = linePoint1 - linePoint2;
            float c = Vector3.Dot(lineVec1, r);
            float f = Vector3.Dot(lineVec2, r);

            float s = (b * f - c * e) / d;
            float t = (a * f - c * b) / d;

            closestPointLine1 = linePoint1 + lineVec1 * s;
            closestPointLine2 = linePoint2 + lineVec2 * t;

            return true;
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
        public static float Mod(float x, float m) 
        {
            float r = x % m;
            return r<0 ? r+m : r;
        }

        /// <summary>
        /// Determines whether an angle (in degrees) exists within the angle range
        /// specified by the angle range vector, with x being from and y being to
        /// in a clockwise direction. Range is inclusive
        /// </summary>
        /// <param name="angle">Angle in degrees</param>
        /// <param name="angleRange">Degree range</param>
        /// <returns>Whether the angle exists within the range from angleRange.x to angleRange.y</returns>
        public static bool IsAngleWithinAngleRange(float angle, Vector2 angleRange)
        {
            angle = Mod(angle, 360f);
            angleRange.x = Mod(angleRange.x, 360f);
            angleRange.y = Mod(angleRange.y, 360f);

            if (angleRange.y > angleRange.x)
            {
                // no looping to handle
                return angleRange.x <= angle && angleRange.y >= angle;
            }

            // range includes 0, have to handle the looping from 360->0
            return angle >= angleRange.x || angle <= angleRange.y;
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
            return r < 0 ? r + m : r;
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

        public static float UnclampedInverseLerp(double a, double b, float value)
        {
            return Math.Abs(a - b) > float.Epsilon ? (float) ((value - a) / (b - a)) : 0.0f;
        }

        public static long ConstructLongFromInts(int first, int second)
        {
            byte[] firstBytes = BitConverter.GetBytes(first);
            byte[] secondBytes = BitConverter.GetBytes(second);

            byte[] longBytes = new byte[sizeof(long)];
            Buffer.BlockCopy(firstBytes, 0, longBytes, 0, firstBytes.Length);
            Buffer.BlockCopy(secondBytes, 0, longBytes, firstBytes.Length, secondBytes.Length);

            return BitConverter.ToInt64(longBytes, 0);
        }
        
        public static (int, int) DeconstructLongToInts(long longValue)
        {
            byte[] longBytes = BitConverter.GetBytes(longValue);

            byte[] firstBytes = new byte[sizeof(int)];
            byte[] secondBytes = new byte[sizeof(int)];
            Buffer.BlockCopy(longBytes, 0, firstBytes, 0, sizeof(int));
            Buffer.BlockCopy(longBytes, sizeof(int), secondBytes, 0, sizeof(int));

            int first = BitConverter.ToInt32(firstBytes, 0);
            int second = BitConverter.ToInt32(secondBytes, 0);

            return (first, second);
        }
        
        public static float GetSinePulse(float startTime, float frequency)
        {
            float pulseTime = Time.time - startTime;
            float sinValue = Mathf.Sin(pulseTime * frequency - Mathf.PI / 2f);
            float pulseValue = sinValue / 2f + 0.5f;
            return pulseValue;
        }

        public static Pose LerpPose(Pose poseA, Pose poseB, float lerpValue)
        {
            Vector3 lerpedPosition = Vector3.Lerp(poseA.position, poseB.position, lerpValue);
            Quaternion lerpedRotation = Quaternion.Lerp(poseA.rotation, poseB.rotation, lerpValue);
            return new Pose(lerpedPosition, lerpedRotation);
        }
    }
}