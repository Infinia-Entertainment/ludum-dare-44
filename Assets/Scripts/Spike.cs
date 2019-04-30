using UnityEngine;

public class Spike : MonoBehaviour
{
    private void OnTriggerEnter2D(Collider2D c)
    {
        if (c.gameObject.CompareTag("Player"))
        {
            PlayerController player = c.gameObject.GetComponent<PlayerController>();
            player.GetHurt(10, true, true);
        }
    
    }

}
