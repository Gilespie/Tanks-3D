using UnityEngine;

public class ThunderObj : MonoBehaviour
{
    [SerializeField] private float m_Speed;
    [SerializeField] private float m_frequency;
    [SerializeField] private AnimationCurve m_MoveX;
    [SerializeField] private AnimationCurve m_MoveZ;
    [SerializeField] private ParticleSystem m_Prefab;

    private float posYdead = 0f;

    private void Start()
    {
        ParticleSystem prefab = Instantiate(m_Prefab, transform.position, transform.rotation);
        prefab.transform.SetParent(gameObject.transform);
        prefab.Play();
    }

    void Update()
    {
        m_frequency += Time.deltaTime;

        if(m_frequency >= 1f)
            m_frequency = 0f;

        float dirx = m_MoveX.Evaluate(m_frequency);
        float dirz = m_MoveZ.Evaluate(m_frequency);
        Vector3 dir = new Vector3(0, -1f * m_Speed * Time.deltaTime, 0);
        transform.position += dir;

        if(transform.position.y <= posYdead)
        {
            gameObject.SetActive(false);
        }
    }
}