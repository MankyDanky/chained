using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Health : MonoBehaviour
{

    public float MaxHealth = 100;
    public float currentHealth;
    private string currentSceneName;

    private void Awake()
    {
        currentHealth = MaxHealth; 
    }

    public void takeDamage (int damageNum)
    {
        currentHealth -= damageNum;
        Debug.Log(gameObject.name + " took" + damageNum + " damage");

        if (currentHealth <= 0)
        {
            StartCoroutine(Die());
        }
    }

    //Coroutine for dying
    private IEnumerator Die()
    {
        
        Debug.Log(gameObject.name + " Died!");
        yield return new WaitForSeconds(2f);
        currentSceneName = SceneManager.GetActiveScene().name;
        SceneManager.LoadScene(currentSceneName);
        
    }
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        Debug.Log(gameObject.name + " health is " + currentHealth);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
