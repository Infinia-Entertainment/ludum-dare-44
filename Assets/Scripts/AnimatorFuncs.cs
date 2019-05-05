using UnityEngine;

public class AnimatorFuncs : MonoBehaviour
{
    Animator animator;
    public AudioManager audioManager;
    public GameObject playerMoveEffects;
    public GameObject playerJumpEffects;
    public GameObject bloodEffects;
    public Transform baseTranform;
    PlayerController player;

    private void Start()
    {
        animator = GetComponent<Animator>();
        player = GetComponentInChildren<PlayerController>();
    }
    //Animator Functions
    void IsAirbone()
    {
        animator.SetTrigger("Airbone");
    }
    void LandingSound()
    {
        audioManager.Play("Landing");
    }
    void MovingSound()
    {    
        audioManager.Play("Moving");
    }
    void JumpingSound()
    {
        audioManager.Play("Jumping");
    }
    void MoveParticles()
    {
        InstantiateParticles(playerMoveEffects);
    }
    void Blood()
    {
        if (!player.isInvincibile)
        {
            InstantiateParticles(bloodEffects);
        }
    }
    void JumpParticles()
    {
        InstantiateParticles(playerJumpEffects);
    }
    void InstantiateParticles(GameObject effects)
    {
        GameObject instace = Instantiate(effects, baseTranform.position, Quaternion.identity, transform);
        ParticleSystem particles = instace.GetComponentInChildren<ParticleSystem>();
        particles.Play();
    }
}
