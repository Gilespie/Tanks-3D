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
        leftTrackRenderer.material.SetTextureOffset("t34_track", leftTrackRenderer.material.GetTextureOffset("t34_track") + direction * speed);

        speed = tank.RightWheelRmp / 60.0f * modifier * Time.fixedDeltaTime;
        rightTrackRenderer.material.SetTextureOffset("t34_track", rightTrackRenderer.material.GetTextureOffset("t34_track") + direction * speed);
    }
}
