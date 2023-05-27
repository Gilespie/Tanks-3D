using UnityEngine;
using UnityEngine.UI;

public class UITankMark : MonoBehaviour
{
    [SerializeField] private Image m_TankMark;

    [SerializeField] private Color m_LocalTeamColor;
    [SerializeField] private Color m_OtherTeamColor;

    public void SetLocalColor()
    {
        m_TankMark.color = m_LocalTeamColor;
    }

    public void SetOtherColor()
    {
        m_TankMark.color = m_OtherTeamColor;
    }
}
