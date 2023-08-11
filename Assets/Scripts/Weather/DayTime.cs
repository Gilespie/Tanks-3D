using UnityEngine;
using TMPro;
using System;

//[ExecuteInEditMode]
public class DayTime : MonoBehaviour
{
    public static event Action<float> DayTimeChanged;

    [SerializeField] private Transform m_Sun;
    [SerializeField] private TextMeshProUGUI m_DayTimeText;
    //[SerializeField] private float m_GameTimeDuration;
    [SerializeField] private Gradient m_DayGradient;
    [SerializeField] private Gradient m_AmbientColor;

    private float m_currentTime;

    private Vector3 _defaultAngle;
    public Light m_DirLight;
    [SerializeField, Range(0f, 1f)] private float m_TimeProgress = 0;
    [SerializeField, Range(0f, 3600f)] private float m_TimeInSeconds = 60;

    private void Start()
    {
        _defaultAngle = m_DirLight.transform.localEulerAngles;
    }

    private void Update()
    {
        TestTime();

        m_TimeProgress += Time.deltaTime / m_TimeInSeconds;

        if(m_TimeProgress > 1f)
        {
            m_TimeProgress = 0f;
            m_currentTime = 0f;
        }

        m_DirLight.color = m_DayGradient.Evaluate(m_TimeProgress);
        RenderSettings.ambientLight = m_AmbientColor.Evaluate(m_TimeProgress);

        m_DirLight.transform.localEulerAngles = new Vector3(360f * m_TimeProgress - 90f, _defaultAngle.y, _defaultAngle.z);
    }

    private void TestTime()
    {
        m_currentTime = m_TimeProgress * 1440f;
        DayTimeChanged?.Invoke(m_currentTime);
    }

    /*private void SunRotation()
    {
        float angle = m_currentTime/m_GameTimeDuration * 360f;

        m_Sun.transform.rotation = Quaternion.Euler(angle, 0, 0);
    }*/
}
