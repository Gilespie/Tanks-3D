using System.Collections.Generic;
using UnityEngine;

public class UIAmmunitionPanel : MonoBehaviour
{
    [SerializeField] private Transform m_AmmunitionParent;
    [SerializeField] private UIAmmunitionElement m_AmmunitionElementPrefab;

    private List<UIAmmunitionElement> allAmmunitionElements = new List<UIAmmunitionElement>();
    private List<Ammunition> allAmmunition = new List<Ammunition>();

    private Turret turret;
    private int lastSelectionAmmunitionIndex;

    private void Start()
    {
        NetworkSessionManager.Match.MatchStart += OnMatchStarted;
        NetworkSessionManager.Match.MatchEnd += OnMatchEnded;
    }

    private void OnDestroy()
    {
        NetworkSessionManager.Match.MatchStart -= OnMatchStarted;
        NetworkSessionManager.Match.MatchEnd -= OnMatchEnded;

    }

    private void OnMatchStarted()
    {
        turret = Player.Local.ActiveVehicle.Turret;
        turret.UpdateSelectedAmmunition += OnTurretUpdateSelectedAmmunition;

        for (int i = 0; i < m_AmmunitionParent.childCount; i++)
        {
            Destroy(m_AmmunitionParent.GetChild(i).gameObject);
        }

        allAmmunitionElements.Clear();
        allAmmunition.Clear();

        for (int i = 0; i < turret.Ammunition.Length; i++)
        {
            UIAmmunitionElement ammunitionElement = Instantiate(m_AmmunitionElementPrefab);
            ammunitionElement.transform.SetParent(m_AmmunitionParent);
            ammunitionElement.transform.localScale = Vector3.one;
            ammunitionElement.SetAmmunition(turret.Ammunition[i]);

            turret.Ammunition[i].AmmoCountChanged += OnAmmoCountChanged;

            allAmmunitionElements.Add(ammunitionElement);
            allAmmunition.Add(turret.Ammunition[i]);

            if (i == 0)
            {
                ammunitionElement.Select();
            }
        }
    }

    private void OnMatchEnded()
    {
        if (allAmmunition != null)
        {
            turret.UpdateSelectedAmmunition += OnTurretUpdateSelectedAmmunition;
        }

        for (int i = 0; i < allAmmunition.Count; i++)
        {
            allAmmunition[i].AmmoCountChanged -= OnAmmoCountChanged;
        }
    }

    private void OnAmmoCountChanged(int ammoCount)
    {
        allAmmunitionElements[turret.SelectedAmmunitionIndex].UpdateAmmoCount(ammoCount);
    }

    private void OnTurretUpdateSelectedAmmunition(int index)
    {
        allAmmunitionElements[lastSelectionAmmunitionIndex].UnSelect();
        allAmmunitionElements[index].Select();

        lastSelectionAmmunitionIndex = index;
    }
}
