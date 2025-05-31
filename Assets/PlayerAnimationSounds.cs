using UnityEngine;

public class PlayerAnimationSounds : MonoBehaviour
{
    [SerializeField] private GameObject[] footstepSound;
    [SerializeField] FirstPersonController player;

    private void Start()
    {
        player = FirstPersonController.Instance.GetComponent<FirstPersonController>();
    }
    
    public void PlayFootstepSound()
    {
        if (player.isGrounded)
        {
            Instantiate(footstepSound[Random.Range(0, footstepSound.Length)], transform.position, Quaternion.identity);
        }
    }
}
