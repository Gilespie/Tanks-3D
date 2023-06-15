using UnityEngine;
using TMPro;

public class UIHitResultPopup : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI m_TypeText;
    [SerializeField] private TextMeshProUGUI m_DamageText;

    public void SetTypeResult(string textResult)
    {
        m_TypeText.text = textResult;
    }

    public void SetDamageResult(float damageResult)
    {
        if (damageResult <= 0) return;

        m_DamageText.text = "-" + damageResult.ToString("F0");
    }
}