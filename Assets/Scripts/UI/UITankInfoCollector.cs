using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UITankInfoCollector : MonoBehaviour
{
    [SerializeField] private Transform m_TankInfoPanel;
    [SerializeField] private UITankInfo m_TankInfoPrefab;

    private UITankInfo[] tanksInfo;
    private List<Vehicle> vehiclesWithoutLocal;

    private void Start()
    {
        NetworkSessionManager.Match.MatchStart += OnMatchStart;
        NetworkSessionManager.Match.MatchEnd += OnMatchEnd;
    }

    private void OnDestroy()
    {
        NetworkSessionManager.Match.MatchStart -= OnMatchStart;
        NetworkSessionManager.Match.MatchEnd -= OnMatchEnd;
    }

    private void OnMatchStart()
    {
        Vehicle[] vehicles = FindObjectsOfType<Vehicle>();

        vehiclesWithoutLocal = new List<Vehicle>(vehicles.Length - 1);

        for(int i = 0; i < vehicles.Length; i++) 
        {
            if (vehicles[i] == Player.Local.ActiveVehicle) continue;

            vehiclesWithoutLocal.Add(vehicles[i]);
        }

        tanksInfo = new UITankInfo[vehiclesWithoutLocal.Count];

        for(int i = 0; i < vehiclesWithoutLocal.Count; i++)
        {
            tanksInfo[i] = Instantiate(m_TankInfoPrefab);
            tanksInfo[i].SetTank(vehiclesWithoutLocal[i]);
            tanksInfo[i].transform.SetParent(m_TankInfoPanel);

        }
    }

    private void OnMatchEnd()
    {
       for(int i = 0; i < m_TankInfoPanel.transform.childCount; i++)
       {
           Destroy(m_TankInfoPanel.transform.GetChild(i).gameObject);
       }

        tanksInfo = null;
    }

    private void Update()
    {
        if (tanksInfo == null) return;

        for(int i = 0; i < tanksInfo.Length; i++)
        {
            if (tanksInfo[i] == null) continue;

            bool isVisible = Player.Local.ActiveVehicle.Viewer.IsVisible(tanksInfo[i].Tank.netIdentity);

            tanksInfo[i].gameObject.SetActive(isVisible);

            if (tanksInfo[i].gameObject.activeSelf == false) continue;

            Vector3 screenPos = Camera.main.WorldToScreenPoint(tanksInfo[i].Tank.transform.position + tanksInfo[i].WorldOffset);

            if(screenPos.z > 0)
            {
                tanksInfo[i].transform.position = screenPos;
            }
        }
    }
}
