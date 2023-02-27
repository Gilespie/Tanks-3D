using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(TrackTank))]
public class TrackTankTextureMovement : MonoBehaviour
{
    private TrackTank tank;

    [SerializeField] private Renderer leftTrackRenderer;
    [SerializeField] private Renderer rightTrackRenderer;

    [SerializeField] private Vector2 direction;
    [SerializeField] private float modifier;

    private void Start()
    {
        tank = GetComponent<TrackTank>();
    }

    private void FixedUpdate()
    {
        float speed = tank.LeftWheelRmp / 60.0f * modifier * Time.fixedDeltaTime;
        leftTrackRenderer.material.SetTextureOffset("_MainTex", leftTrackRenderer.material.GetTextureOffset("_MainTex") + direction * speed);

        speed = tank.RightWheelRmp / 60.0f * modifier * Time.fixedDeltaTime;
        rightTrackRenderer.material.SetTextureOffset("_MainTex", rightTrackRenderer.material.GetTextureOffset("_MainTex") + direction * speed);
    }
}
