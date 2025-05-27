using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RockSpike : MonoBehaviour
{
    Transform player;
    FirstPersonController playerController;

    [SerializeField] GameObject[] spikePrefabs;
    [SerializeField] List<MeshRenderer> spikeMeshRenderers = new List<MeshRenderer>();
    public float spawnCount;
    [SerializeField] LayerMask groundLayer;

    void Start()
    {
        foreach (Transform child in transform)
        {
            if (child.TryGetComponent<MeshRenderer>(out MeshRenderer meshRenderer))
            {
                spikeMeshRenderers.Add(meshRenderer);
            }
        }
        player = FirstPersonController.Instance;
        playerController = player.GetComponent<FirstPersonController>();
        if ((transform.position - player.position).magnitude < 1.5f)
        {
            playerController.TakeDamage(10f);
            playerController.ApplyImpulse(transform.forward * 10f + Vector3.up * 5f);
        }

        StartCoroutine(SpawnNextSpike());
    }

    IEnumerator SpawnNextSpike()
    {
        yield return new WaitForSeconds(0.15f);
        if (spawnCount < 8 && Physics.Raycast(transform.position + transform.forward * 1.5f + transform.up, Vector3.down, out RaycastHit hit, 2f, groundLayer))
        {
            GameObject spikePrefab = spikePrefabs[Random.Range(0, spikePrefabs.Length)];
            GameObject newSpike = Instantiate(spikePrefab, hit.point, Quaternion.identity);
            RockSpike spikeScript = newSpike.GetComponent<RockSpike>();
            spikeScript.spawnCount = spawnCount + 1;
            newSpike.transform.LookAt(new Vector3(player.position.x, hit.point.y, player.position.z));
            float scaleOffset = Random.Range(0f, 0.2f);
            newSpike.transform.localScale = new Vector3(
                newSpike.transform.localScale.x + scaleOffset,
                newSpike.transform.localScale.y + scaleOffset,
                newSpike.transform.localScale.z + scaleOffset
            );
        }
        yield return new WaitForSeconds(1f);
        float erodeCutoff = -0.01f;
        while (erodeCutoff <= 1f)
        {
            foreach (MeshRenderer meshRenderer in spikeMeshRenderers)
            {
                meshRenderer.material.SetFloat("_Erosion", erodeCutoff);
            }
            erodeCutoff += Time.deltaTime;
            yield return null;
        }
        Destroy(gameObject);
    }
}
