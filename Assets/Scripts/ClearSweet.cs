using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ClearSweet : MonoBehaviour {

    public AnimationClip clearAnimation;
    private bool isClearing;
    public AudioClip destoryAudio;
    public bool IsClearing
    {
        get
        {
            return isClearing;
        }


    }
    protected GameSweet sweet;
    
    private void Awake()
    {
        sweet = GetComponent<GameSweet>();
    }

    public virtual void Clear()
    {
        StartCoroutine(ClearCoroutine());
        isClearing = true;
        print("清除");

    }

    private IEnumerator ClearCoroutine()
    {
        Animator animator = GetComponent<Animator>();
        if (animator != null)
        {
            print("销毁");
            animator.Play(clearAnimation.name);
            GamaManager.Instance.playscore++;
            AudioSource.PlayClipAtPoint(destoryAudio, transform.position);
            yield return new WaitForSeconds(clearAnimation.length);
            Destroy(gameObject);
        }
    }
}
