using UnityEngine;
using TMPro;

public class UIMatchTimer : MonoBehaviour
{
    [SerializeField] private MatchTimer m_Timer;
    [SerializeField] private TextMeshProUGUI m_TimerText;
    private float timerLeft = 0.5f;

    private void Update()
    {
        timerLeft -= Time.deltaTime;

        if(timerLeft <= 0)
        {
            timerLeft = 0.5f;
            ShowTimer();
        }
    }

    private void ShowTimer()
    {
        int minutes = Mathf.FloorToInt(m_Timer.TimeLeft / 60);
        int seconds = Mathf.FloorToInt(m_Timer.TimeLeft % 60);
        m_TimerText.SetText(string.Format("{0:00}:{1:00}", minutes, seconds));
    }
}
