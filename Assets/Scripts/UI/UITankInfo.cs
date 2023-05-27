using UnityEngine;
using UnityEngine.UI;

public class UITankInfo : MonoBehaviour
{
    [SerializeField] private UIHealthSlider m_HealthSlider;

    [SerializeField] private Vector3 m_WorldOffset;
    public Vector3 WorldOffset => m_WorldOffset;

    private Vehicle tank;
    public Vehicle Tank => tank;

    public void SetTank(Vehicle tank)
    {
        this.tank = tank;

        m_HealthSlider.Init(tank, tank.Owner.GetComponent<Player>().TeamId, Player.Local.TeamId);
    }
}
