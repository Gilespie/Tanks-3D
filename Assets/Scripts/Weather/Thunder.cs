using UnityEngine;

public class Thunder : MonoBehaviour
{
    [SerializeField] private Transform posSpawn;
    [SerializeField] private GameObject ThunderPrefab;
    private float randomSpawnX;
    private float randomSpawnZ;
    private float timer = 5f;

    void Update()
    { 
        timer -= Time.deltaTime;

        if(timer <= 0)
        {
            GetRandomPoint();
            timer = 5f;
        }
    }

    private Vector3 GetRandomPoint()
    {
        randomSpawnX = Random.Range(-500f, 500f);
        randomSpawnZ = Random.Range(-500f, 500f);

        SpawnThunder();

        return posSpawn.position = new Vector3(randomSpawnX, 200f, randomSpawnZ);
    }

    private void SpawnThunder()
    {
        ThunderPrefab.transform.position = posSpawn.position;
        ThunderPrefab.SetActive(true);
    }
}
