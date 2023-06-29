using UnityEngine;
using TMPro;

public class UIDayTime : MonoBehaviour
{
    private TextMeshProUGUI timeDayText;

    void Start()
    {
        timeDayText = GetComponent<TextMeshProUGUI>();
        DayTime.DayTimeChanged += OnDayTimeChanged;
    }

    private void OnDestroy()
    {
        DayTime.DayTimeChanged -= OnDayTimeChanged;
    }

    private void OnDayTimeChanged(float time)
    {
        float hours = Mathf.FloorToInt(time / 60);
        float minutes = Mathf.FloorToInt(time % 60f);
        timeDayText.SetText(string.Format("{0:00}:{1:00}", hours, minutes));
    }
}
