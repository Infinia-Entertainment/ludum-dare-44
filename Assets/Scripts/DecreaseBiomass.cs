using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DecreaseBiomass : MonoBehaviour
{
    AudioManager audioManager;

    private void Awake()
    {
        audioManager = GameObject.FindObjectOfType<AudioManager>();
    }
    public float damage = 1f;
    private void OnTriggerStay2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            collision.GetComponent<PlayerController>().GetHurt(damage, true);
        }
    }

}
