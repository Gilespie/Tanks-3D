using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class UIPlayerLabel : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI m_FragText;
    [SerializeField] private TextMeshProUGUI m_NicknameText;
    [SerializeField] private Image m_BackgroundImage;
    [SerializeField] private Color m_SelfColor;

    private int netId;
    public int NetId => netId;

    public void Init(int netId, string nickname)
    {
        this.netId = netId;
        m_NicknameText.text = nickname;

        if(netId  == Player.Local.netId)
        {
            m_BackgroundImage.color = m_SelfColor;
        }
    }

    public void UpdateFrags(int frags)
    {
       m_FragText.text = frags.ToString();
    }
}
