using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TransfigurationBuff : IBuff
{
    public int Duration { get; set; }
    public Transform target;
    private Animator animator;

    private GameObject originRender;
    private Avatar originAvatar;
    private RuntimeAnimatorController originController;
    private Vector3 originArrowPosition;

    private GameObject targetRender;
    private Vector3 targetArrowPosition;

    public TransfigurationBuff(int duration, Transform target)
    {
        this.target = target;
        if (duration <= 0)
        {
            Duration = duration;
        }
        else
        {
            Duration = RoundManager.GetInstance().Players.Count * duration - 1;
        }
    }

    public void Apply(Transform character)
    {
        animator = character.GetComponent<Animator>();

        originRender = character.Find("Render").gameObject;
        originArrowPosition = character.GetComponent<CharacterStatus>().arrowPosition;
        originAvatar = character.GetComponent<Animator>().avatar;
        originController = character.GetComponent<Animator>().runtimeAnimatorController;
        originRender.SetActive(false);

        if (animator.avatar != target.GetComponent<Animator>().avatar)
        {
            var originHash = animator.GetCurrentAnimatorStateInfo(0).fullPathHash;
            var originTime = animator.GetCurrentAnimatorStateInfo(0).normalizedTime;

            targetArrowPosition = target.GetComponent<CharacterStatus>().arrowPosition;
            var go = target.Find("Render").gameObject;
            targetRender = GameObject.Instantiate(go);
            targetRender.transform.SetParent(character);
            targetRender.transform.position = character.position;
            targetRender.transform.rotation = character.rotation;

            animator.avatar = target.GetComponent<Animator>().avatar;
            animator.runtimeAnimatorController = target.GetComponent<Animator>().runtimeAnimatorController;

            //否则会无限循环触发事件，从而无限创建Render。这里如果Particle卡顿，就会导致延迟时间不够用。
            
            animator.Play(originHash, 0, originTime + 0.1f);
            
            character.GetComponent<CharacterStatus>().arrowPosition = targetArrowPosition;
            character.GetComponent<CharacterStatus>().SetTransfiguration();
        }
        else
        {
            //与目标外形一致但属于敌方角色的情况。
            character.GetComponent<CharacterStatus>().SetTransfiguration();
            originRender.SetActive(true);
        }
    }

    public IBuff Clone()
    {
        throw new NotImplementedException();
    }

    public void Undo(Transform character)
    {
        if (!originRender.activeInHierarchy)
        {
            GameObject.Destroy(targetRender);
            originRender.SetActive(true);
            character.GetComponent<CharacterStatus>().arrowPosition = originArrowPosition;

            FXManager.GetInstance().SmokeSpawn(character.position, character.rotation, null);

            var animator = character.GetComponent<Animator>();

            animator.avatar = originAvatar;
            animator.runtimeAnimatorController = originController;
        }
        //延迟等删除Render生效。
        RoundManager.GetInstance().Invoke(() => { character.GetComponent<CharacterStatus>().SetNoumenon(); }, 0.01f);
        character.GetComponent<Unit>().Buffs.Remove(this);
    }
}
