using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Toxin : MonoBehaviour
{
    public float timer = 4f;
    public float DoT = 0.05f;
    public AudioManager audioManager;
    float counter;
    private void Awake()
    {
        audioManager = GameObject.FindObjectOfType<AudioManager>();
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            PlayerController player = collision.GetComponent<PlayerController>();
            StartCoroutine(ApplyToxin(player));
            audioManager.Play("HitWaste");
        }
    }
   
    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            audioManager.Play("HitWaste");
        }
    }
    IEnumerator ApplyToxin(PlayerController player)
    {
        while(counter < timer)
        {
            counter += Time.fixedDeltaTime;
            player.GetHurt(DoT, false, false, false);
            yield return new WaitForFixedUpdate();
        }
    }
}
