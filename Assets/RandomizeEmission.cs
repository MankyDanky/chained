using System.Collections;
using UnityEngine;

public class RandomizeEmission : MonoBehaviour
{

    [SerializeField] float lowerBound = 3f;
    [SerializeField] float upperBound = 6f;
    ParticleSystem particleSystem;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        particleSystem = GetComponent<ParticleSystem>();
        StartCoroutine(RandomizeEmissionCoroutine());
    }
    
    IEnumerator RandomizeEmissionCoroutine()
    {
        while (true)
        {
            float randomTime = Random.Range(lowerBound, upperBound);
            yield return new WaitForSeconds(randomTime);
            particleSystem.Play();
        }
    }
}
