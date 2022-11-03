using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class ParameterCurve
{
    [SerializeField] private AnimationCurve m_Curve;
    [SerializeField] private float m_Duration = 1;
    private float expiredTime;

    public float MoveTowards(float deltaTime)
    {
        expiredTime += deltaTime;

        return m_Curve.Evaluate(expiredTime / m_Duration);
    }

    public float Reset()
    {
        expiredTime = 0;

        return m_Curve.Evaluate(0);
    }

    public float GetValueBetween(float startValue, float endValue, float currentValue)
    {
        if (m_Curve.length == 0 || startValue == endValue) return 0;

        float startTime = m_Curve.keys[0].time;
        float endTime = m_Curve.keys[m_Curve.length - 1].time;

        float currentTime = Mathf.Lerp(startTime, endTime, (currentValue - startValue) / (endValue - startValue));

        return m_Curve.Evaluate(currentTime);
    }
}
