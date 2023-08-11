using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Block : MonoBehaviour
{
    public static event Action OnDamaged;
    [SerializeField] private GameObject m_StartModel;
    [SerializeField] private List<GameObject> m_childs;
    [SerializeField, Range(0f, 1f)] private float Health;

    void Start()
    {
        OnDamaged += ChangeModel;
        ChildSet();
    }

    //TODO: изменить родительский объект на куб и отключать у него компоненты меш рендера и коллайдера

    void Update()
    {
        if(Input.GetMouseButtonDown(0))
        {
            TakeDamage(0.1f);
        }
    }

    public void TakeDamage(float damage)
    {
        Health -= damage;
        OnDamaged?.Invoke();
    }

    private void ChangeModel()
    {
        if(Health == 1.0f)
        {
            ChildSet();
        }
        if(Health <= 0.8f)
        {
            ChildGet();
        }
        if(Health <= 0.6f)
        {
            ChangeKinematic();
        }
        if(Health <= 0.4f)
        {
            ChangeKinematic();
        }
        if(Health <= 0.2f)
        {
            ChangeKinematic();
        }
        if(Health <= 0f)
        {
            Destroy(gameObject);
        }
    }

    private void ChildSet()
    {
        m_StartModel.SetActive(true);

        for(int i = 0; i < m_childs.Count; i++)
        {
            m_childs[i].GetComponent<Rigidbody>().isKinematic = true;
            m_childs[i].SetActive(false);
        }
    }

    private void ChildGet()
    {
        m_StartModel.SetActive(false);

        for (int i = 0; i < m_childs.Count; i++)
        {
            m_childs[i].gameObject.SetActive(true);
        }
    }

    private void ChangeKinematic()
    {
        for(int i = 0; i <= 1;  i++)
        {
            m_childs[i].GetComponent<Rigidbody>().isKinematic = false;
            m_childs.Remove(m_childs[i]);
        }
    }
}
