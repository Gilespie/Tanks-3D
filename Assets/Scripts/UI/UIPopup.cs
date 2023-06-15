using UnityEngine;

public class UIPopup : MonoBehaviour
{
    [SerializeField] private Vector2 m_MovementDirection;
    [SerializeField] private float m_MovementSpeed;
    [SerializeField] private float m_LifeTime;

    void Start()
    {
        Destroy(gameObject, m_LifeTime);
    }

    void Update()
    {
        transform.Translate(m_MovementDirection * m_MovementSpeed * Time.deltaTime);
    }
}
