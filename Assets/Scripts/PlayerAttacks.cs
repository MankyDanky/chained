using UnityEngine;
using System.Collections;

public class PlayerAttacks : MonoBehaviour
{
    bool hasExploded = false;
    public float grenCD = 5;
    public GameObject grenade;
    public Transform playerLoc;
    Rigidbody grenRB;
    public float grenVel = 50.0f;

    void Start()
    {
        
    }
   
    public void SpawnIn()
    {

        GameObject newGrenade = Instantiate(grenade, playerLoc.position + playerLoc.transform.forward, playerLoc.rotation);
        grenRB = newGrenade.GetComponent<Rigidbody>();
        grenRB.mass = 100;
        grenRB.linearVelocity = playerLoc.transform.forward * grenVel;

        Char1GrenadeScript grenadeScript = newGrenade.GetComponent<Char1GrenadeScript>();
        if (grenadeScript != null)
        {
            StartCoroutine(grenadeScript.ExplosionStart());
        }


    }

    
    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.F) && hasExploded == false)
        {
            StartCoroutine(Grenade());
            
        }
    }
    private IEnumerator Grenade()
    {
        hasExploded = true;
        SpawnIn();
        Debug.Log("grenade on cd");
        yield return new WaitForSeconds(grenCD);
        hasExploded = false;
        Debug.Log("grenade ready");

    }
}
