using System;
using System.Collections;
using Unity.VisualScripting;
using UnityEngine;

public class PlayerCurrentState : CharacterCurrentState
{   

    [SerializeField] float slideSpeed = 2;
    [SerializeField] float slideDuration = 1f;
    [SerializeField] float runToSlideTiming = 2f;
    public bool IsSprinting
    {
        get => isSprinting;
        set
        {
            if (isSprinting == value) return;

            bool old = isSprinting;
            isSprinting = value;

            OnSprinting?.Invoke(old, value);
        }
    }

    public event System.Action<bool, bool> OnSprinting;

    private void Awake()
    {
        //OnSprinting += OnSprintingBoolChanage; ระบบไม่ดี เอาออก
    }

    private Coroutine waitToMoveCoroutine;

    private void OnSprintingBoolChanage(bool oldBool, bool newBool)
    {
        if (!newBool)
        {
            if(waitToMoveCoroutine != null)
            {
                StopCoroutine(waitToMoveCoroutine);
            }
            waitToMoveCoroutine = StartCoroutine(WaitToMove()); 
        }
    }

    private IEnumerator WaitToMove()
    {
        float timeSpentRunning = Player.instance.playerMovement.runningTime; 

        if (Player.instance.characterController.isGrounded && timeSpentRunning > runToSlideTiming)
        {
            Player.instance.animator.CrossFade("RunToStop", 0.3f);
            canMove = false;
            canRotate = false;
            isPerformingAction = true;

            Vector3 slideDirection = Player.instance.transform.forward;
            float timer = 0;

            while (timer < slideDuration)
            {
                float speed = Mathf.Lerp(slideSpeed, 0, timer / slideDuration);

                Vector3 moveVelocity = slideDirection * speed;

                moveVelocity.y = -9.81f;

                Player.instance.characterController.Move(moveVelocity * Time.deltaTime);

                timer += Time.deltaTime;

                yield return null;
            }
        }
        canMove = true;
        canRotate = true;
        isPerformingAction = false;

        Player.instance.playerMovement.runningTime = 0;
    }
}

