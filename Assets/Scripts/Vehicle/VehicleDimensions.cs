using UnityEngine;

[RequireComponent(typeof(Vehicle))]
public class VehicleDimensions : MonoBehaviour
{
    [SerializeField] private Transform[] m_Points;
    [SerializeField] private Transform[] m_PriorityFirePoints;

    private Vehicle vehicle;
    public Vehicle Vehicle => vehicle;

    RaycastHit[] hits = new RaycastHit[10];

    private void Awake()
    {
        vehicle = GetComponent<Vehicle>();
    }

    public bool IsVisibleFromPoint(Transform source, Vector3 point, Color color)
    {
        bool visible = true;

        for(int i = 0; i < m_Points.Length; i++)
        {
            //Debug.DrawLine(point, m_Points[i].position, color);

            int l = Physics.RaycastNonAlloc(point, (m_Points[i].position - point).normalized, hits, Vector3.Distance(point, m_Points[i].position));

            visible = true;

            for(int j = 0; j < l; j++)
            {
                if (hits[j].collider.transform.root == source) continue; //TODO: Fix it
                if (hits[i].collider.transform.root == transform.root) continue;

                visible = false;
            }

            if (visible == true) return visible;

        }
        return false;
    }

    public Transform GetPriorityFirePoint()
    {
        return m_PriorityFirePoints[0]; //TODO: —делать случайное выпадение приоритетных точек.
    }

#if UNITY_EDITOR
    private void OnDrawGizmos()
    {
        if (m_Points == null) return;

        Gizmos.color = Color.blue;

        for(int i = 0; i < m_Points.Length; i++)
        {
            Gizmos.DrawSphere(m_Points[i].position, 0.2f);
        }
    }
#endif
}