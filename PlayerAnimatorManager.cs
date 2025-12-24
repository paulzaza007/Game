using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;


public class PlayerAnimatorManger : MonoBehaviour
{
    [Header("Damage Animation")]
    public string lastDamageAnimationPlayed;

    [SerializeField] string hit_Forward_Medium_01 = "hit_Forward_Medium_01";
    [SerializeField] string hit_Forward_Medium_02 = "hit_Forward_Medium_02";

    public string hit_Backward_Medium_01 = "hit_Backward_Medium_01";

    [SerializeField] string hit_Left_Medium_01 = "hit_Left_Medium_01";
    [SerializeField] string hit_Left_Medium_02 = "hit_Left_Medium_02";

    [SerializeField] string hit_Right_Medium_01 = "hit_Right_Medium_01";
    [SerializeField] string hit_Right_Medium_02 = "hit_Right_Medium_02";

    public List<string> forward_Midium_Damage = new List<string>();
    public List<string> right_Midium_Damage = new List<string>();
    public List<string> left_Midium_Damage = new List<string>();


    protected virtual void Awake()
    {
        
    }

    private void Start()
    {
        forward_Midium_Damage.Add(hit_Forward_Medium_01);
        forward_Midium_Damage.Add(hit_Forward_Medium_02);

        right_Midium_Damage.Add(hit_Left_Medium_01);
        right_Midium_Damage.Add(hit_Left_Medium_02);
        
        left_Midium_Damage.Add(hit_Right_Medium_01);
        left_Midium_Damage.Add(hit_Right_Medium_02);
    }

    public string GetRandomAnimationFromList(List<string> animationList)
    {
        List<string> finalList = new List<string>();

        foreach (var item in animationList)
        {
            finalList.Add(item);
        }

        finalList.Remove(lastDamageAnimationPlayed);

        for (int i = finalList.Count - 1; i < -1; i--)
        {
            if(finalList[i] == null)
            {
                finalList.RemoveAt(i);
            }
        }

        int randomValue = Random.Range(0, finalList.Count);
        Debug.Log(finalList + "NUMBER" + randomValue + "IS PLAYING");

        return finalList[randomValue];
    }

    public void UpdateAnimatorMovement(float horizontalValue, float verticalValue)
    {
        Player.instance.animator.SetFloat("Horizontal", horizontalValue, 0.1f, Time.deltaTime);
        Player.instance.animator.SetFloat("Vertical", verticalValue, 0.1f, Time.deltaTime);
    }

    public void PlayerTargetActionAnimation(
        string targetAnimation,
        bool performingActionFlag,
        bool applyRootMotion = true,
        bool canRotate = false,
        bool canMove = false)
    {
        Player.instance.playerCurrentState.isPerformingAction = performingActionFlag;
        Player.instance.playerCurrentState.applyRootMotion = applyRootMotion;
        Player.instance.playerCurrentState.canRotate = canRotate;
        Player.instance.playerCurrentState.canMove = canMove;

        //เปิด Root Motion ใน Animator
        Player.instance.animator.applyRootMotion = applyRootMotion;

        Player.instance.animator.CrossFade(targetAnimation, 0.2f);
    }

    public void PlayerTargetAttackActionAnimation(
        Player player,
        AttackType attackType,
        string targetAnimation,
        bool performingActionFlag,
        bool applyRootMotion = true,
        bool canRotate = false,
        bool canMove = false)
    {
        player.playerCombatManager.currentAttackType = attackType;
        player.playerCombatManager.lastAttackAnimationPerform = targetAnimation;
        player.playerCurrentState.isPerformingAction = performingActionFlag;
        player.playerCurrentState.applyRootMotion = applyRootMotion;
        player.playerCurrentState.canRotate = canRotate;
        player.playerCurrentState.canMove = canMove;

        //เปิด Root Motion ใน Animator
        player.animator.applyRootMotion = applyRootMotion;

        player.animator.CrossFade(targetAnimation, 0.2f);
    }

    private Coroutine closeComboCoroutine;

    public void EnableCanDoCombo()
    {
        if (Player.instance.isUsingRightHand)
        {
            Player.instance.playerCombatManager.canComboWithMainHandWeapon = true;
            if (closeComboCoroutine != null)
            {
                StopCoroutine(closeComboCoroutine);
            }

            closeComboCoroutine = StartCoroutine(CloseCombo());
        }
        else
        {

        }
    }

    private IEnumerator CloseCombo()
    {
        yield return new WaitForSeconds(1.5f);
        DisableCanDoCombo();
        closeComboCoroutine = null;
    }

    public void DisableCanDoCombo()
    {
        Player.instance.playerCombatManager.canComboWithMainHandWeapon = false;
    }
}
