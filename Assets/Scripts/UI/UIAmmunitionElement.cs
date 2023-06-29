using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UIAmmunitionElement : MonoBehaviour
{
    [SerializeField] private Image m_ProjectileIcon;
    [SerializeField] private TextMeshProUGUI m_ProjCount;
    [SerializeField] private GameObject m_SelectBorder;

    public void SetAmmunition(Ammunition ammunition)
    {
        m_ProjectileIcon.sprite = ammunition.ProjectileProp.ProjectileIcon;

        UpdateAmmoCount(ammunition.AmmoCount);
    }

    public void UpdateAmmoCount(int count)
    {
        m_ProjCount.text = count.ToString();
    }

    public void Select()
    {
        m_SelectBorder.SetActive(true);
    }

    public void UnSelect()
    {
        m_SelectBorder.SetActive(false);
    }
}
