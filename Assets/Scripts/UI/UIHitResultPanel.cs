using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIHitResultPanel : MonoBehaviour
{
    [SerializeField] private Transform m_SpawnPanel;
    [SerializeField] private UIHitResultPopup m_HitResultPopup;

    private void Start()
    {
        NetworkSessionManager.Match.MatchStart += OnMathStart;    
    }

    private void OnDestroy()
    {
        NetworkSessionManager.Match.MatchStart -= OnMathStart;
        Player.Local.ProjectileHit -= OnProjectileHit;
    }

    private void OnMathStart()
    {
        Player.Local.ProjectileHit += OnProjectileHit;
    }

    private void OnProjectileHit(ProjectileHitResult hitResult)
    {
        if (hitResult.Type == ProjectileHitType.Environment || hitResult.Type == ProjectileHitType.ModulePenetration ||
            hitResult.Type == ProjectileHitType.ModuleNon_Penetration) return;

        UIHitResultPopup hitPopup = Instantiate(m_HitResultPopup);
        hitPopup.transform.SetParent(m_SpawnPanel);
        hitPopup.transform.localScale = Vector3.one;
        hitPopup.transform.position = Camera.main.WorldToScreenPoint(hitResult.Point);

        if (hitResult.Type == ProjectileHitType.Penetration)
            hitPopup.SetTypeResult("Пробитие");

        if (hitResult.Type == ProjectileHitType.Ricochet)
            hitPopup.SetTypeResult("Рикошет");

        if (hitResult.Type == ProjectileHitType.Non_Penetration)
            hitPopup.SetTypeResult("Броня не пробита");

        hitPopup.SetDamageResult(hitResult.Damage);
    }
}