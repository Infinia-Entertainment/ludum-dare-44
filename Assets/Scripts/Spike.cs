using UnityEngine;

public class Spike : MonoBehaviour
{
    float damage = 15;
    private void OnTriggerEnter2D(Collider2D c)
    {
        if (c.gameObject.CompareTag("Player"))
        {
            PlayerController player = c.gameObject.GetComponent<PlayerController>();
            player.GetHurt(damage, true, true, true, true);
        }
    
    }

}
