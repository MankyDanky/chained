using System.Collections;
using UnityEngine;

public class RockSpike : MonoBehaviour
{
    Transform player;
    [SerializeField] GameObject[] spikePrefabs;

    void Start()
    {

        player = FirstPersonController.Instance;
        StartCoroutine(SpawnNextSpike());
    }

    IEnumerator SpawnNextSpike()
    {
        yield return new WaitForSeconds(0.25f);
        if (Physics.Raycast(transform.position + transform.forward + transform.up, Vector3.down, out RaycastHit hit, 2f))
        {
            GameObject spikePrefab = spikePrefabs[Random.Range(0, spikePrefabs.Length)];
            GameObject newSpike = Instantiate(spikePrefab, hit.point, Quaternion.identity);
            newSpike.transform.LookAt(new Vector3(player.position.x, hit.point.y, player.position.z));
            float scaleOffset = Random.Range(0f, 0.2f);
            newSpike.transform.localScale = new Vector3(
                newSpike.transform.localScale.x + scaleOffset,
                newSpike.transform.localScale.y + scaleOffset,
                newSpike.transform.localScale.z + scaleOffset
            );
        }
    }
}
