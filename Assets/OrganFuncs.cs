using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OrganFuncs : MonoBehaviour
{
    public Transform heartPos;
    public Transform lungsPos;

    public GameObject organEffects;

    Animator animator;

    private void Awake()
    {
        animator = GetComponent<Animator>();
    }
    public void HeartPressed()
    {
        animator.SetTrigger("HeartPressed");
        EmitParticles(heartPos);
    }

    public void LungsPressed()
    {
        animator.SetTrigger("LungsPressed");
        EmitParticles(lungsPos);
    }

    void EmitParticles(Transform organTransform)
    {
        Vector3 pos = organTransform.position;
        GameObject instance = Instantiate(organEffects, pos, Quaternion.identity, transform);
        ParticleSystem particles = instance.GetComponentInChildren<ParticleSystem>();
        particles.Play();
    }
}
