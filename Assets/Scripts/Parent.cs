using UnityEngine;

public class Parent : MonoBehaviour
{
    [SerializeField] private Transform m_Parent;

    private void Awake()
    {
        transform.SetParent(m_Parent);
    }
}