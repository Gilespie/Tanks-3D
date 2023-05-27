using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UICaptureBase : MonoBehaviour
{
    [SerializeField] private ConditionCaptureBase m_ConditionCaptureBase;

    [SerializeField] private Slider m_LocalTeamSlider;
    [SerializeField] private Slider m_OtherTeamSlider;

    private void Update()
    {
        if(Player.Local == null) return;

        if(Player.Local.TeamId ==  TeamSide.TeamRed)
        {
            UpdateSlider(m_LocalTeamSlider, m_ConditionCaptureBase.RedBaseCaptureLevel);
            UpdateSlider(m_OtherTeamSlider, m_ConditionCaptureBase.GreenBaseCaptureLevel);
        }

        if (Player.Local.TeamId == TeamSide.TeamGreen)
        {
            UpdateSlider(m_LocalTeamSlider, m_ConditionCaptureBase.GreenBaseCaptureLevel);
            UpdateSlider(m_OtherTeamSlider, m_ConditionCaptureBase.RedBaseCaptureLevel);
        }
    }

    private void UpdateSlider(Slider slider, float value)
    {
        if (value == 0)
        {
            slider.gameObject.SetActive(false);
        }
        else
        {
            if (slider.gameObject.activeSelf == false)
            {
                slider.gameObject.SetActive(true);
            }

            slider.value = value;
        }
    }
}
