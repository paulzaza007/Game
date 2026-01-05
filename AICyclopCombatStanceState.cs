using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

[CreateAssetMenu(menuName = "A.I/States/Cyclop/Cyclop Combat Stance")]

public class AICyclopCombatStanceState : AICombatStanceState
{
    public List<AICyclopAttackAction> aICyclopAttack; //****
    protected List<AICyclopAttackAction> potentialCyclopAttacks; //****
    protected AICyclopAttackAction chooseCyclopAttack; //****
    protected AICyclopAttackAction previosCyclopAttack; //****

    public override AIState Tick(AICharacterManager aICharacter)
    {
        //Debug.Log("Combat stance ของ Cyclop กำลังทำงาน");
        if (aICharacter.aICurrentState.isPerformingAction) //ถ้าAIเล่นอนิเมชั่นอยู่ให้รัน state นี้ใหม่
        {
            return this;
        }

        if (!aICharacter.navMeshAgent.enabled) //ถ้าnavmeshปิดอยู่ให้เปิด
        {
            aICharacter.navMeshAgent.enabled = true;
        }

        if (!aICharacter.IsMoving) //ไม่ขยับอยู่แหละเป้าหมายอยู่นอกสายตา ให้ขยับตาม
        {
            if (aICharacter.aICharacterCombatManager.viewableAngle < -30 || aICharacter.aICharacterCombatManager.viewableAngle > 30)
            {
                aICharacter.aICharacterCombatManager.PivotTowardTarget(aICharacter); //หันแบบเล่นอนิเมชั่น
            }


        }

        aICharacter.aICharacterCombatManager.RotateTowardsAgent(aICharacter); //หันตามผู้เล่น

        if (aICharacter.aICharacterCombatManager.currentTarget == null) //ไม่เจอผู้เล่น ให้กลับไปidle
        {
            return SwitchState(aICharacter, aICharacter.idle);
        }

        if (!hasAttack) //ยังไม่มีท่าโจมตี เรียกคำสั่งขอท่าโจมตี
        {
            GetNewAttack(aICharacter);
        }
        else //มีท่าโจมตีแล้ว ส่งท่าโจมตีไปให้ stateAttack และเปลี่ยนstate
        {
            aICharacter.attack.currentCyclopAttack = chooseCyclopAttack; //****
            //Debug.Log("สั่งโจมตี");

            return SwitchState(aICharacter, aICharacter.attack);
        }

        if (aICharacter.aICharacterCombatManager.distanceFromTarget > maximumEngagementDistance)  //ถ้าผู้เล่นอยู่ไกลเกินให้เปลี่ยนStateเป็นเดินเข้าหาผู้เล่น
        {
            return SwitchState(aICharacter, aICharacter.purSueTarget);
        }

        //สร้างเส้นทางให้ AI ไปยังผู้เล่น
        NavMeshPath path = new NavMeshPath();
        aICharacter.navMeshAgent.CalculatePath(aICharacter.aICharacterCombatManager.currentTarget.transform.position, path);
        aICharacter.navMeshAgent.SetPath(path);

        return this; //วน
    }

    protected override void GetNewAttack(AICharacterManager aICharacter)
    {
        potentialCyclopAttacks = new List<AICyclopAttackAction>();
        //Debug.Log($"กำลังหาท่าโจมตี... มีท่าในลิสต์ทั้งหมด {aICyclopAttack.Count} ท่า");

        foreach (var potentialAttack in aICyclopAttack) // ลูปหาท่าใน Undead
        {
            if (potentialAttack.minimumDistance > aICharacter.aICharacterCombatManager.distanceFromTarget) //ใกล้ไป
            {
                continue;
            }
            if (potentialAttack.maximumDistance < aICharacter.aICharacterCombatManager.distanceFromTarget) //ไกลไป
            {
                continue;
            }
            if (potentialAttack.minimumAttackAngle > aICharacter.aICharacterCombatManager.viewableAngle) //ซ้ายเกิน
            {
                continue;
            }
            if (potentialAttack.maximumAttackAngle < aICharacter.aICharacterCombatManager.viewableAngle) //ขวาเกิน   
            {
                continue;
            }

            potentialCyclopAttacks.Add(potentialAttack); //เพิ่มท่าที่สามารถโจมตีได้เข้าไปในlist
           // Debug.Log($"มีท่าโจมตีในลิสต์ตอนนี้ทั้งหมด {potentialCyclopAttacks.Count} ท่า");
        }

        if (potentialCyclopAttacks.Count <= 0) //ไม่มีท่าเลยให้ คืนค่าออก
        {
            return;
        }

        var totalWeight = 0; //สร้างน้ำหนักรวมเพื่มมาเก็บค่าน้ำในแต่ละท่าโจมตี

        foreach (var attack in potentialCyclopAttacks)//เก็บค่าน้ำหนักรวมจากทุกท่า
        {
            totalWeight += attack.AttackWeight;
        }

        var randomWeightValue = Random.Range(1, totalWeight + 1); //สุ่มค่าน้ำหนัก
        var processWeight = 0; //ค่าที่จะเอาค่าน้ำหนักแต่ละค่าไปหาท่าโจมตี

        foreach (var attack in potentialCyclopAttacks)
        {
            processWeight += attack.AttackWeight; //รับค่าน้ำหนักจากท่านี้มา

            if (randomWeightValue <= processWeight) //ท่าสุ่มน้ำหนัก น้อยกว่าเท่ากับ น้ำหนักที่เอาเอามาหา ก็จะเลือกท่านั้นเป็นท่าโตมตี
            {
                chooseCyclopAttack = attack; //****
                previosCyclopAttack = chooseCyclopAttack; //****
                hasAttack = true;
                //Debug.Log($"เลือกท่า {attack} ท่า");
                return;
            }
        }
    }


    protected override void ResetStateFlags(AICharacterManager aICharacter)
    {
        base.ResetStateFlags(aICharacter);

        hasAttack = false;
        hasRollForComboChance = false;
    }
}
