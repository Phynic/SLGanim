using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Threading;

public class Movement {
    public float speed = 0f;                     //角色移动速度
    
    private Transform character = null;
    private List<Vector3> Pos = null;              //存放移动路径拐点的list
    private int i = 0;                      //控制迭代到list的下一个拐点
    private Vector3 startPosition;
    private Quaternion startRotation;
    private MoveState moveState;
    private Animator animator;
    private int runningHash;
    private int runHash;
    private int idleHash;
    private AudioSource audio;
    //private Vector3 curvePosition;
    //private Vector3 curveDestination;
    //private float curveHeight = 0.25f;
    

    public void SetMovement(List<Vector3> path, Transform character)
    {
        this.character = character;
        audio = character.GetComponent<AudioSource>();
        Pos = path;
        moveState = MoveState.startPoint;
        startPosition = character.position;
        startRotation = character.rotation;
        animator = character.GetComponent<Animator>();
        runningHash = Animator.StringToHash("isRunning");
        runHash = Animator.StringToHash("Run");
        idleHash = Animator.StringToHash("Idle");
        animator.applyRootMotion = false;
        animator.SetBool(runningHash, false);
    }
    
    private enum MoveState
    {
        startPoint,
        line,
        point,
        finalPoint
    }

    public bool ExcuteMove()
    {
        
        switch (moveState)
        {
            case MoveState.startPoint:
                if (animator.GetCurrentAnimatorStateInfo(0).shortNameHash == idleHash)
                {
                    if (!animator.applyRootMotion)
                    {
                        animator.applyRootMotion = true;
                        animator.SetBool(runningHash, true);
                        audio.time = 0.5f;
                        audio.Play();
                        FXManager.GetInstance().DustSpawn(character.position, character.rotation, null);
                    }
                    moveState = MoveState.line;
                }
                break;
            case MoveState.line:    //     |  |__->___|  |_______|  |
                if (animator.GetCurrentAnimatorStateInfo(0).shortNameHash == runHash)
                {
                    float distance = (Pos[i] - character.position).magnitude;

                    //character.Translate(((Pos[i] - character.position).normalized) * Time.deltaTime * speed, Space.World);
                    
                    if (!animator.isMatchingTarget)
                    {
                        if (distance > 1)
                        {
                            MatchPoint(Pos[i]);
                        }
                    }
                    if (distance <= 0.35f)
                    {
                        animator.InterruptMatchTarget(false);
                        character.Translate(Pos[i] - character.position, Space.World);
                        animator.applyRootMotion = false;
                        animator.SetBool(runningHash, false);
                        moveState = MoveState.point;
                    }
                }
                break;
            case MoveState.point:   //     |  |_______|->|_______|  |
                if (animator.GetCurrentAnimatorStateInfo(0).shortNameHash == idleHash)
                {
                    if (i >= Pos.Count - 1)
                    {
                        moveState = MoveState.finalPoint;
                    }
                    else
                    {
                        i++;
                        if (Pos[i] != character.position)
                        {
                            Quaternion wantedRot = Quaternion.LookRotation(Pos[i] - character.position);
                            character.rotation = wantedRot;
                            FXManager.GetInstance().DustSpawn(character.position, character.rotation, null);
                        }
                        
                        if (!animator.applyRootMotion)
                        {
                            animator.applyRootMotion = true;
                            animator.SetBool(runningHash, true);
                            audio.Play();   
                            moveState = MoveState.line;
                        }
                    }
                }
                break;
            case MoveState.finalPoint:
                animator.applyRootMotion = false;
                Pos = null;
                return true;
        }
        return false;
    }
    
    private void MatchPoint(Vector3 destination)
    {
        //MatchTargetWeightMask中的positionXYZWeight应该是localPosition，所以forward即可，因为人物会转向。
        animator.MatchTarget(destination, character.rotation, AvatarTarget.Root, new MatchTargetWeightMask(Vector3.forward, 0f), 0.75f, 0.95f);
    }

    public void Reset()
    {
        if (character)
        {
            character.position = startPosition;
            character.rotation = startRotation;
        }
        if (animator)
        {
            animator.applyRootMotion = false;
            animator.SetBool(runningHash, false);
        }
    }
    
}
