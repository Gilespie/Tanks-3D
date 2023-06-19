using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum ArmorType
{
    Vehicle,
    Module
}

public class Armor : MonoBehaviour
{
    [SerializeField] private ArmorType m_ArmorType;
    [SerializeField] private Destructible m_Destructible;
    [SerializeField] private int m_Thickness;

    public ArmorType Type => m_ArmorType;
    public Destructible Destructible => m_Destructible;
    public int Thickness => m_Thickness;

    public void SetDestructible(Destructible destructible)
    {
        this.m_Destructible = destructible;
    }
}
