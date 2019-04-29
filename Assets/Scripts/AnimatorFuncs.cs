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
    void JumpingSound()
    {
        audioManager.Play("Jumping");
    }
    void HitWasteSound()
    {
        audioManager.Play("WasteHit");
    }
}
