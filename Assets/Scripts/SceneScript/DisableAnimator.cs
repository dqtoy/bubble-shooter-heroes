using UnityEngine;
using System.Collections;

public class DisableAnimator : MonoBehaviour
{

    Animator animator;
    Animation animationA;

    void Start()
    {
        animator = GetComponent<Animator>();
        animationA = gameObject.animation;
    }

    public void disableAnimator()
    {
        if (animator != null)
            animator.enabled = false;
        if (animation != null)
            animationA.enabled = false;
    }

    public void Suicide()
    {
        Destroy(gameObject, 0.1f);
    }
}
