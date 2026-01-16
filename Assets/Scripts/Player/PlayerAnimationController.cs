using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerAnimationController : MonoBehaviour
{
    private Animator animator;

    private void Awake()
    {
        animator = GetComponentInChildren<Animator>();

        if (animator == null)
            Logger.Instance.Log(Color.red, "NO ANIMATOR FOUND", this.gameObject, "Player animator script");
    }
}
