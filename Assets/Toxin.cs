using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Toxin : MonoBehaviour
{
    public float timer = 4f;
    public float DoT = 0.05f;
    float counter;
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            PlayerController player = collision.GetComponent<PlayerController>();
            StartCoroutine(ApplyToxin(player));
        }
    }
   
    IEnumerator ApplyToxin(PlayerController player)
    {
        while(counter < timer)
        {
            counter += Time.fixedDeltaTime;
            player.GetHurt(DoT, false);
            yield return new WaitForFixedUpdate();
        }
    }
}
