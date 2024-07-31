using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class Console
{

    public static void SpeedLimitedForce(Vector2 direction,float intensity, float maxVelocity, Rigidbody2D body1, ForceMode2D type)
    {
        if (Mathf.Abs(body1.velocity.x) <= Mathf.Abs(maxVelocity))
        {
            body1.AddForce(direction * intensity, type);
        }
    }

    public static IEnumerator Rumble(float intensity, float duration)
    {
        Gamepad.current.SetMotorSpeeds(intensity, intensity);
        yield return new WaitForSeconds(duration);
        Gamepad.current.SetMotorSpeeds(0, 0);
    }

    public static void SetParticles(ParticleSystem particles, bool condition)
    {
        if (particles != null)
        {
            if (condition && !particles.isPlaying)
            {
                particles.Play();
            }
            if (!condition && particles.isPlaying)
            {
                particles.Stop();
            }
        }
    }

    public static float RoundToDP(float value, int decimalPlaces)
    {
        float modifier = Mathf.Pow(10f, decimalPlaces);
        return Mathf.Round(value * modifier) / modifier;
    }

/// <summary>
/// 'x' represents the overall progress through an animation between 0 and 1
/// </summary>
/// <param name="x"></param>
/// <returns></returns>
    public static float easeInOutCubic(float x)
    {
        return x < 0.5? 4*x*x*x+0.05f : 1 - Mathf.Pow(-2 * x + 2, 3) / 2;
    }

    public static float easeOutCubic(float x)
    {
        return 1 - Mathf.Pow(1 - x, 3) + 0.05f;
    }                                                                                                    

    public static float easeInOutQuint(float x)
    {
        return x < 0.5f ? 16 * x * x * x * x * x +0.05f : 1 - Mathf.Pow(-2 * x + 2, 5) / 2;
    }

//     function easeInOutCubic(x: number): number 
//     {
//      return x < 0.5 ? 4 * x * x * x : 1 - Math.pow(-2 * x + 2, 3) / 2;
//     }
}
