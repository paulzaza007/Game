using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class AIBossManager : AICharacterManager
{
    public AICyclopSFXManager cyclopSFXManager;

    public int bossID = 0;

    [Header("Music")]
    [SerializeField] AudioClip bossIntroClip;
    [SerializeField] AudioClip bossBattleLoopClip;

    [Header("Status")]
    [SerializeField] bool bossFightIsActive = false;
    private bool hasPhaseShifted = false;
    public bool BossFightIsActive
    {
        get => bossFightIsActive;
        set
        {
            if (bossFightIsActive == value) return;

            bool old = bossFightIsActive;
            bossFightIsActive = value;

            OnBossFightIsActive?.Invoke(old, value);
        }
    }
    public event System.Action<bool, bool> OnBossFightIsActive;

    [SerializeField] bool hasBeenDefeated = false;
    public bool HasBeenDefeated
    {
        get => hasBeenDefeated;
        set
        {
            if (hasBeenDefeated == value) return;

            bool old = hasBeenDefeated;
            hasBeenDefeated = value;

            OnDefeat?.Invoke(old, value);
        }
    }

    public event System.Action<bool, bool> OnDefeat;
    
    [SerializeField] bool hasBeenAwaked = false;
    public bool HasBeenAwaked
    {
        get => hasBeenAwaked;
        set
        {
            if (hasBeenAwaked == value) return;

            bool old = hasBeenAwaked;
            hasBeenAwaked = value;

            OnAwaked?.Invoke(old, value);
        }
    }
    public event System.Action<bool, bool> OnAwaked;

    [Header("Phase Shift")]
    public float minimumHealthPercentageToShift = 50;
    [SerializeField] string phaseShiftAnimation = "Phase2";
    [SerializeField] AICombatStanceState phase02CombatStanceState;

    [SerializeField] List<FogWallInteractable> fogWalls;
    [SerializeField] string sleepAnimation;
    [SerializeField] string awakeAnimation;
 
    [Header("DEBUG")]
    [SerializeField] bool defeatBossDebug = false;
    [SerializeField] bool wakeBossUp = false;

    protected override void Awake()
    {
        base.Awake(); 
        cyclopSFXManager = GetComponent<AICyclopSFXManager>();

        OnBossFightIsActive += OnBossFightIsActiveChange;

    }

    protected override void Start()
    {
        base.Start();
        // ถ้า bossID ยังไม่เคยอยู่ในsaveก็ให้เก็บสนาถานะลงไป
        if (!WorldSaveGameManager.instance.currentPlayerData.bossesAwake.ContainsKey(bossID)) 
        {
            WorldSaveGameManager.instance.currentPlayerData.bossesAwake.Add(bossID, false);
            WorldSaveGameManager.instance.currentPlayerData.bossesDefeat.Add(bossID, false);
        }
        else
        // ถ้า bossID อยู่ในsaveก็ให้ดึงสนานะจากsaveมาด้วย bossID นี้
        {
            HasBeenDefeated = WorldSaveGameManager.instance.currentPlayerData.bossesDefeat[bossID];
            HasBeenAwaked = WorldSaveGameManager.instance.currentPlayerData.bossesAwake[bossID];

            //ถ้าบอสตายแล้วให้ลบ
            if (HasBeenDefeated)
            {
                gameObject.SetActive(false);
            }
        }


        StartCoroutine(GetFogWallsFromWorldObjectManager());

        //ถ้าBossยังไม่ถูกกำจัด ให้ Enable fog walls
        if (HasBeenAwaked)
        {
            for (int i = 0; i < fogWalls.Count; i++)
            {
                fogWalls[i].IsActive = true;
            }
        }

        //ถ้าBossถูกกำจัด ให้ Disable fog walls
        if (HasBeenDefeated)
        {
            for (int i = 0; i < fogWalls.Count; i++)
            {
                fogWalls[i].IsActive = false;
            }

            IsActive = false;
        }

        //บอสยังไม่ตื่นก็เล่น animation หลับ
        if (!HasBeenAwaked)
        {
            aIAnimationManager.PlayerTargetActionAnimation(sleepAnimation, true);
            currentState = sleepState;
        }
    }

    protected override void Update() //ForDebug
    {
        base.Update();
        if (defeatBossDebug)
        {
            defeatBossDebug = false;
            HasBeenDefeated = true;

            if (!WorldSaveGameManager.instance.currentPlayerData.bossesAwake.ContainsKey(bossID))
            {
                WorldSaveGameManager.instance.currentPlayerData.bossesAwake.Add(bossID, true);
                WorldSaveGameManager.instance.currentPlayerData.bossesDefeat.Add(bossID, true);
            }
            else
            {
                WorldSaveGameManager.instance.currentPlayerData.bossesAwake.Remove(bossID);
                WorldSaveGameManager.instance.currentPlayerData.bossesDefeat.Remove(bossID);
                WorldSaveGameManager.instance.currentPlayerData.bossesAwake.Add(bossID, true);
                WorldSaveGameManager.instance.currentPlayerData.bossesDefeat.Add(bossID, true);
            }

            WorldSaveGameManager.instance.SaveGame();
        }

        if (wakeBossUp)
        {
            wakeBossUp = false;
            WakeBoss();
        }
    }

    protected override void FixedUpdate()
    {
        base.FixedUpdate();
    }

    private IEnumerator GetFogWallsFromWorldObjectManager() //หา fogwall ที่ตรงกับ bossid
    {
        while(WorldObjectManager.instance.fogWalls.Count == 0)
        {
            yield return new WaitForEndOfFrame();
        }

        fogWalls = new List<FogWallInteractable>(); 

        foreach (var fogWall in WorldObjectManager.instance.fogWalls)
        {
            if (fogWall.fogWallID == bossID) //หา fogwall ที่ตรงกับ id ของบอส
            {
                fogWalls.Add(fogWall);
            }
        }
    }

    protected override void OnCurrentHealthChange(int oldFloat, int newFloat)
    {
        if (aIStatManager != null && aIStatManager.CurrentHealth <= 0)
        {
            StartCoroutine(ProcessDeathEvent());
            aIStatManager.OnCurrentHealth -= OnCurrentHealthChange; //unsubscribe ออกถ้าตาย
        }
        

        if (newFloat <= 0)
        {
            return;
        }
        
        float healthNeedForShift = aIStatManager.maxHealth * (minimumHealthPercentageToShift / 100f);
        if(newFloat <= healthNeedForShift && !hasPhaseShifted) //เช็คว่าเลือดปัจจุบันน้อยกว่าเปอร์เซ็นที่ตั้งไว้มั้ย
        {
            hasPhaseShifted = true;
            PhaseShift(); //เปลี่ยนเฟสบอส
        }
    }

    public override IEnumerator ProcessDeathEvent(bool manuallySelectDeathAnimation = false) //ถ้าบอสตาย
    {
        PlayerUIManager.instance.playerUIPopUpManager.SendBossDefeatPopUp("GREAT FOE FELLED"); //แสดงข้อความบนจอ
        currentState = deathState;
        
        animator.SetLayerWeight(0, 0);//เซ็ตanimation layerหลัก ให้เป็น 0 เพื่อหยุดอนิเมชั่น
        

        aICurrentState.isDead = true;

        characterStatManager.CurrentHealth = 0;
        characterCurrentState.isDead = true;
        BossFightIsActive = false;

        HasBeenDefeated = true;

        foreach (var fogWall in fogWalls) // ปิดfogwall
        {
            fogWall.IsActive = false;
        }

        if (!manuallySelectDeathAnimation) //เลือกว่าจะเซ็ตอนิเมชั่นตายเองมั้ย
        {
            aIAnimationManager.PlayerTargetActionAnimation("Death", true);
        }

        if (!WorldSaveGameManager.instance.currentPlayerData.bossesAwake.ContainsKey(bossID)) 
        {
            //ตั้งสนานะในเซฟว่าตายแล้ว และตื่นแล้ว
            WorldSaveGameManager.instance.currentPlayerData.bossesAwake.Add(bossID, true);
            WorldSaveGameManager.instance.currentPlayerData.bossesDefeat.Add(bossID, true);
        }
        else
        {
            WorldSaveGameManager.instance.currentPlayerData.bossesAwake.Remove(bossID); 
            WorldSaveGameManager.instance.currentPlayerData.bossesDefeat.Remove(bossID);
            WorldSaveGameManager.instance.currentPlayerData.bossesAwake.Add(bossID, true);
            WorldSaveGameManager.instance.currentPlayerData.bossesDefeat.Add(bossID, true);
        }
        //ทำเสร็จละเซฟเกม

        WorldSaveGameManager.instance.SaveGame();

        yield return new WaitForSeconds(5);


    }

    public void WakeBoss() //เรียกบอสตื่น
    {

        if (!HasBeenAwaked) //ยังไม่เคยตื่นก็ให้เรียกaniamtionตื่น
        {
            aIAnimationManager.PlayerTargetActionAnimation(awakeAnimation, true); 
        }

        BossFightIsActive = true; //เรียก OnBossFightIsActiveChange
        HasBeenAwaked = true;
        currentState = idle;
        aICurrentState.isPerformingAction = false;

        //บอกsaveว่าบอสตื่นแล้ว
        if (!WorldSaveGameManager.instance.currentPlayerData.bossesAwake.ContainsKey(bossID))
        {
            WorldSaveGameManager.instance.currentPlayerData.bossesAwake.Add(bossID, true);
        }
        else
        {
            WorldSaveGameManager.instance.currentPlayerData.bossesAwake.Remove(bossID);
            WorldSaveGameManager.instance.currentPlayerData.bossesAwake.Add(bossID, true);
        }

        for (int i = 0; i < fogWalls.Count; i++)
        {
            fogWalls[i].IsActive = true;
        }
    }

    private void OnBossFightIsActiveChange(bool oldBool, bool newBool)
    {
        if (BossFightIsActive)
        {
            WorldSFXManager.instance.PlayBossTrack(bossIntroClip, bossBattleLoopClip); //เล่นเพลง

            //สร้างหลอดเลือดUIให้บอส
            GameObject bossHealthBar =
        Instantiate(PlayerUIManager.instance.playerUIHUDManager.bossHealthBarObject, PlayerUIManager.instance.playerUIHUDManager.bossHealthBarParent);

            //เปิดใช้งานหลอดเลือดบอส
            UI_Boss_HP_Bar bossHPBar = bossHealthBar.GetComponentInChildren<UI_Boss_HP_Bar>();
            bossHPBar.EnableBossHPBar(this);
        }
        else
        {
            //หยุดเพลง
            WorldSFXManager.instance.StopBossMusic();
        }
        
    }

    protected void PhaseShift()
    {
        aIAnimationManager.PlayerTargetActionAnimation(phaseShiftAnimation, true);
        combatStance = Instantiate(phase02CombatStanceState); //เปลี่ยน stateนี้ให้เป็นการใช้phase2
        currentState = combatStance;
    }
}
