using System;
using UnityEngine;

/// <summary>
/// Functions that convert linear 0->1 transitions to eased 0->1 transitions
/// The functions' order from most gradual to most sudden as: Sine, Cubic, Expo
///
/// Credit to: https://easings.net/ for the math
/// </summary>
public static class EasingFunctions
{
    private const float ElasticConst = 2f * (float)Math.PI / 3f;
    
    public enum EasingType
    {
        Linear,
        Sine,
        Cubic,
        Expo,
        Bounce,
        Elastic
    }

    public enum EasingDirection
    {
        In,
        Out,
        InAndOut
    }

    // Sine:
    public static float EaseInOutSine(float x)
    {
        return -(Mathf.Cos(Mathf.PI * x) - 1f) / 2f;
    }
    
    public static float EaseInSine(float x)
    {
        return 1f - Mathf.Cos((x * Mathf.PI) / 2);
    }
    
    public static float EaseOutSine(float x)
    {
        return Mathf.Sin((x * Mathf.PI) / 2);
    }
    
    
    // Cubic:
    public static float EaseInOutCubic(float x)
    {
        return x < 0.5f ? 4f * Mathf.Pow(x,3) : 1f - Mathf.Pow(-2f * x + 2f, 3f) / 2f;
    }
    
    public static float EaseInCubic(float x)
    {
        return Mathf.Pow(x, 3);
    }
    
    public static float EaseOutCubic(float x)
    {
        return 1 - Mathf.Pow(1 - x, 3);
    }

    
    // Expo:
    public static float EaseInOutExpo(float x)
    {
        return x.Approximately(0f)
            ? 0f
            : x.Approximately(1f)
                ? 1f
                : x < 0.5f ? Mathf.Pow(2f, 20f * x - 10f) / 2f
                    : (2f - Mathf.Pow(2f, -20f * x + 10f)) / 2f;
    }
    
    public static float EaseInExpo(float x)
    {
        return x.Approximately(0f) ? 0f : Mathf.Pow(2f, 10f * x - 10f);
    }
    
    public static float EaseOutExpo(float x)
    {
        return x.Approximately(1f) ? 1f : 1 - Mathf.Pow(2f, -10f * x);
    }
    
    
    // Miscellaneous:
    public static float EaseElastic(float x)
    {
        return x.Approximately(0f)
            ? 0f
            : x.Approximately(1f)
                ? 1f
                : Mathf.Pow(2f, -10f * x) * Mathf.Sin((x * 10f - 0.75f) * ElasticConst) + 1;
    }

    public static float EaseBounce(float x)
    {
        float  n1 = 7.5625f;
        float  d1 = 2.75f;
        
        if (x < 1 / d1) return n1 * x * x;
        
        if (x < 2 / d1) return n1 * (x -= 1.5f / d1) * x + 0.75f;

        if (x < 2.5 / d1) return n1 * (x -= 2.25f / d1) * x + 0.9375f;

        return n1 * (x -= 2.625f / d1) * x + 0.984375f;
    }
    
    public static float ConvertLinearToEased(EasingType easingType, EasingDirection easingDirection, float linear)
    {
        switch (easingType)
        {
            case EasingType.Linear: return linear;
            case EasingType.Sine when easingDirection == EasingDirection.InAndOut: return EaseInOutSine(linear);
            case EasingType.Sine when easingDirection == EasingDirection.In: return EaseInSine(linear);
            case EasingType.Sine when easingDirection == EasingDirection.Out: return EaseOutSine(linear);
            case EasingType.Cubic when easingDirection == EasingDirection.InAndOut: return EaseInOutCubic(linear);
            case EasingType.Cubic when easingDirection == EasingDirection.In: return EaseInCubic(linear);
            case EasingType.Cubic when easingDirection == EasingDirection.Out: return EaseOutCubic(linear);
            case EasingType.Expo when easingDirection == EasingDirection.InAndOut: return EaseInOutExpo(linear);
            case EasingType.Expo when easingDirection == EasingDirection.In: return EaseInExpo(linear);
            case EasingType.Expo when easingDirection == EasingDirection.Out: return EaseOutExpo(linear);
            case EasingType.Bounce: return EaseBounce(linear);
            case EasingType.Elastic: return EaseElastic(linear);
            default: throw new ArgumentException($"No case for easing type {easingType} and easing direction {easingDirection}");
        }
    }
}