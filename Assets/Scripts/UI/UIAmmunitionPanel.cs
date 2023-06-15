using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

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
        NetworkSessionManager.Events.PlayerVehicleSpawned += OnPlayerVehicleSpawned;
    }

    private void OnDestroy()
    {
        NetworkSessionManager.Events.PlayerVehicleSpawned -= OnPlayerVehicleSpawned;

        if(allAmmunition != null)
        {
            turret.UpdateSelectedAmmunition += OnTurretUpdateSelectedAmmunition;
        }

        for(int i = 0; i < allAmmunition.Count; i++)
        {
            allAmmunition[i].AmmoCountChanged -= OnAmmoCountChanged;
        }
    }

    private void OnPlayerVehicleSpawned(Vehicle vehicle)
    {
        turret = vehicle.Turret;
        turret.UpdateSelectedAmmunition += OnTurretUpdateSelectedAmmunition;

        for(int i =0; i < turret.Ammunition.Length; i++)
        {
            UIAmmunitionElement ammunitionElement = Instantiate(m_AmmunitionElementPrefab);
            ammunitionElement.transform.SetParent(m_AmmunitionParent);
            ammunitionElement.transform.localScale = Vector3.one;
            ammunitionElement.SetAmmunition(turret.Ammunition[i]);

            turret.Ammunition[i].AmmoCountChanged += OnAmmoCountChanged;

            allAmmunitionElements.Add(ammunitionElement);
            allAmmunition.Add(turret.Ammunition[i]);

            if(i == 0)
            {
                ammunitionElement.Select();
            }
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
