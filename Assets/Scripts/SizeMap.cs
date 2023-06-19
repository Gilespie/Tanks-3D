using UnityEngine;

public class SizeMap : MonoBehaviour
{
    [SerializeField] private Vector2 m_Size;
    public Vector2 Size { get { return m_Size; } }

    public Vector3 GetNormPos(Vector3 pos)
    {
        return new Vector3(pos.x / (m_Size.x * 0.5f), 0, pos.z / (m_Size.y * 0.5f));
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireCube(transform.position, new Vector3(m_Size.x, 0, m_Size.y));
    }
}
