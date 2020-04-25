using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerAnimation : MonoBehaviour
{
    public Animator animator;
    void Start()
    {
        animator = GetComponent<Animator>();
    }

    public void OnMoveAvatarStateChanged(MoveAvatar.AvatarAnimationState state)
    {
        animator.SetBool("Running", state != MoveAvatar.AvatarAnimationState.Idle);
    }
}
