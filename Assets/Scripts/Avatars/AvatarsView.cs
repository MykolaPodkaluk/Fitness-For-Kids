using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IAvatarsView
{
    Animator GetAnimator();
    void Release();
}


public class AvatarsView : MonoBehaviour, IAvatarsView
{
    [SerializeField] private Animator _animator;

    public Animator GetAnimator() 
    {
        return _animator;
    }

    public void Release()
    {
        Destroy(gameObject);
    }
}
