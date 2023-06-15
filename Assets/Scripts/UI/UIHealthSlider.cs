using UnityEngine;
using UnityEngine.UI;

public class UIHealthSlider : MonoBehaviour
{
    [SerializeField] private Slider m_Slider;
    [SerializeField] private Image m_SliderImages;

    [SerializeField] private Color m_LocalTeamColor;
    [SerializeField] private Color m_OtherTeamColor;

    private Destructible destructible;

    public void Init(Destructible destructible, int destructibleTeamId, int localPlayerId)
    {
        this.destructible = destructible;

        destructible.HitPointChanged += OnHitPointChanged;

        m_Slider.maxValue = destructible.HitPointMax;
        m_Slider.value = m_Slider.maxValue;

        if(localPlayerId == destructibleTeamId)
        {
            SetlocalColor();
        }
        else
        {
            SetOtherColor();
        }
    }

    private void OnDestroy()
    {
        if (destructible == null) return;
        
        destructible.HitPointChanged -= OnHitPointChanged;
    }

    private void OnHitPointChanged(int hitPoint)
    {
        m_Slider.value = hitPoint;
    }

    private void SetlocalColor()
    {
        m_SliderImages.color = m_LocalTeamColor;
    }

    private void SetOtherColor()
    {
        m_SliderImages.color = m_OtherTeamColor;
    }
}
