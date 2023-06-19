using UnityEngine;
using UnityEngine.UI;

public class UIMinimap : MonoBehaviour
{
    [SerializeField] private Transform m_MainCanvas;
    [SerializeField] private SizeMap m_SizeMap;
    [SerializeField] private UITankMark m_TankMarkPrefab;
    [SerializeField] private Image m_Background;

    private UITankMark[] _tankMarks;
    private Vehicle[] _Vehicles;

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
        _Vehicles = FindObjectsOfType<Vehicle>();

        _tankMarks = new UITankMark[_Vehicles.Length];

        for(int i = 0; i < _tankMarks.Length; i++)
        {
            _tankMarks[i] = Instantiate(m_TankMarkPrefab);

            if (_Vehicles[i].TeamId == Player.Local.TeamId)
                _tankMarks[i].SetLocalColor();
            else
                _tankMarks[i].SetOtherColor();

            _tankMarks[i].transform.SetParent(m_Background.transform);
        }
    }

    private void OnMatchEnd()
    {
        for(int i = 0; i < m_Background.transform.childCount; i++)
        {
            Destroy(m_Background.transform.GetChild(i).gameObject);
        }

        _tankMarks = null;
    }

    private void Update()
    {
        if (_tankMarks == null) return;

        for(int i = 0; i < _tankMarks.Length; i++)
        {
            if (_Vehicles[i] == null) continue;

            if (_Vehicles[i].gameObject != Player.Local.ActiveVehicle)
            {
                bool isVisible = Player.Local.ActiveVehicle.Viewer.IsVisible(_Vehicles[i].netIdentity);

                _tankMarks[i].gameObject.SetActive(isVisible);
            }

            if (_tankMarks[i].gameObject.activeSelf == false) continue; 

            Vector3 normPos = m_SizeMap.GetNormPos(_Vehicles[i].transform.position);

            Vector3 posInMinimap = new Vector3(normPos.x * m_Background.rectTransform.sizeDelta.x * 0.5f, normPos.z * m_Background.rectTransform.sizeDelta.y * 0.5f, 0);
            posInMinimap.x *= m_MainCanvas.localScale.x;
            posInMinimap.y *= m_MainCanvas.localScale.y;

            _tankMarks[i].transform.position = m_Background.transform.position + posInMinimap;
        }
    }
}
