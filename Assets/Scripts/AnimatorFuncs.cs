using UnityEngine;

public class AnimatorFuncs : MonoBehaviour
{
    Animator animator;
    public AudioManager audioManager;
    private void Start()
    {
        animator = GetComponent<Animator>();
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
}
