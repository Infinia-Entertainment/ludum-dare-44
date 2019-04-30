using UnityEngine;

public class Spike : MonoBehaviour
{
    public float force = 10f;


    private void OnCollisionEnter2D(Collision2D c)
    {
        if (c.gameObject.CompareTag("Player"))
        {
            PlayerController player = c.gameObject.GetComponent<PlayerController>();
            player.GetHurt(20, true, true, true);
        }
    }
    // Update is called once per frame
    void Update()
    {
        
    }
}
