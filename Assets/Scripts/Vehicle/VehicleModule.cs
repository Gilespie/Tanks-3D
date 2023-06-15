using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VehicleModule : Destructible
{
    [SerializeField] private string m_Title;
    [SerializeField] private Armor m_Armor;
    [SerializeField] private float m_RecoveredTime;
    private float remainingReciveryTime;

    private void Awake()
    {
        m_Armor.SetDestructible(this);
    }

    private void Start()
    {
        Destroyed += OnModuleDestroyed;
        enabled = false;
    }

    private void OnDestroy()
    {
        Destroyed -= OnModuleDestroyed;
    }

    private void OnModuleDestroyed(Destructible arg0)
    {
        remainingReciveryTime = m_RecoveredTime;
        enabled = true;
    }

    private void Update()
    {
        if(isServer == true)
        {
            remainingReciveryTime -= Time.deltaTime;

            if(remainingReciveryTime <= 0)
            {
                remainingReciveryTime = 0.0f;

                SvRecovery();

                enabled = false;
            }
        }
    }
}