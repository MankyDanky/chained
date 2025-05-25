using UnityEngine;

public class Billboard : MonoBehaviour
{

    Transform player;
    
    void Start()
    {
        player = FirstPersonController.Instance;
    }

    // Update is called once per frame
    void Update()
    {
        transform.LookAt(player.position);
    }
}
