using UnityEngine;
using UnityEngine.UI;

public class UIMinimap : MonoBehaviour
{
    [SerializeField] private SizeMap m_SizeMap;
    [SerializeField] private UITankMark m_TankMarkPrefab;
    [SerializeField] private Image m_Background;

    private UITankMark[] _tankMarks;
    private Player[] _players;

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
        _players = FindObjectsOfType<Player>();

        _tankMarks = new UITankMark[_players.Length];

        for(int i = 0; i < _tankMarks.Length; i++)
        {
            _tankMarks[i] = Instantiate(m_TankMarkPrefab);

            if (_players[i].TeamId == Player.Local.TeamId)
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
            if (_players[i] == null) continue;

            Vector3 normPos = m_SizeMap.GetNormPos(_players[i].ActiveVehicle.transform.position);

            Vector3 posInMinimap = new Vector3(normPos.x * m_Background.rectTransform.sizeDelta.x * 0.5f, normPos.z * m_Background.rectTransform.sizeDelta.y * 0.5f, 0);

            _tankMarks[i].transform.position = m_Background.transform.position + posInMinimap;
        }
    }
}
