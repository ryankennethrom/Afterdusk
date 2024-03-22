using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.EventSystems;
using Narrative;
using UnityEngine.Video;

public enum BattleState { START, PLAYERTURN, ENEMYTURN, WON, LOST }
public class BattleSystem : MonoBehaviour
{
    public BattleState state;
    public GameObject redBookMarkContents;
    public GameObject blueBookMarkContents;
    public Canvas choicesInterface;
    public Canvas inventoryInterface;
    public GameObject player;
    public GameObject enemy;
    public BattleHUD playerHUD;
    public BattleHUD enemyHUD;
    // Note: Might want to move Audio System to a different script
    public AudioSource textTransition;
    public AudioSource hitSound;
    public AudioSource gunshots;
    public AudioSource backgroundMusic;
    public AudioSource guncock;
    public Sprite playerDown;
    public Sprite enemyDown;
    public GameObject winCanvas;
    public GameObject skip;

    // Boss Music Transitions
    private AudioSource currentSong;
    public int MCstate = 0;
    public AudioSource MC100;
    public MusicLoop MC100Switch;
    public AudioSource MC50;
    public MusicLoop MC50Switch;
    public AudioSource MC25;
    public MusicLoop MC25Switch;
    public AudioSource MC100SW50;
    public MusicLoop MC100SW50Switch;
    public AudioSource MC50SW50;
    public MusicLoop MC50SW50Switch;
    public AudioSource MC25SW50;
    public MusicLoop MC25SW50Switch;
    public AudioSource MC100SW25;
    public MusicLoop MC100SW25Switch;
    public AudioSource MC50SW25;
    public MusicLoop MC50SW25Switch;
    public AudioSource MC25SW25;
    public MusicLoop MC25SW25Switch;
    public float bpm = 180;
    public float beatTime;

    // For Hat
    public ParticleSystem bullets;
    public AudioSource[] impacts;
    public AudioSource[] shells;
    public AudioSource[] yelps;

    Stats playerStats;
    Stats enemyStats;
    private float enemyAttackModifier = 1.0f;
    public float playerAttackModifier = 1.0f;
    public float attackBuff = 1.0f;
    public float attackBuffAlt = 1.0f;
    private float enemyHpMod = 1.0f;
    public float playerHpMod = 1.0f;
    public int maxEnemyShield = 16;

    public InventoryManager buffLocation;
    public int mirrorCooldown = 3;
    public int clothCooldown = 2;
    public int ballCooldown = 2;
    public int appleCooldown = 4;
    public int venomCooldown = 5;
    public int handsCooldown = 2;
    public int hairCooldown = 4;
    public int godAppleCooldown = 2;
    public int mirrorRestore = 0;
    public int clothRestore = 0;
    public int ballRestore = 0;
    public int appleRestore = 0;
    public int venomRestore = 0;
    public int handsRestore = 0;
    public int hairRestore = 0;
    public int godAppleRestore = 0;
    // Alt is for puppet items
    public int mirrorRestoreAlt = 0;
    public int clothRestoreAlt = 0;
    public int ballRestoreAlt = 0;
    public int appleRestoreAlt = 0;
    public int venomRestoreAlt = 0;
    public int handsRestoreAlt = 0;
    public int hairRestoreAlt = 0;
    public GameObject mirrorObject;
    public GameObject clothObject;
    public GameObject shoesObject;
    public GameObject ballObject;
    public GameObject appleObject;
    public GameObject venomObject;
    public GameObject handsObject;
    public GameObject hairObject;
    public GameObject godAppleObject;
    public bool noCooldowns = false;
    public bool immune = false;

    public EnemyAttackIndicatorController secondAttack;
    public EnemyAttackIndicatorController thirdAttack;
    private List<int> doneAttacks = new List<int>(new int[] { });
    private List<int> doubleDoneAttacks = new List<int>(new int[] { });
    private int attackAmount = 0;

    private int reflectMax = 1;
    private int dmgBoostMax = 1;
    private int vampirismMax = 1;
    private int bindMax = 1;
    private int stunMax = 1;
    private int reflectMaxAlt = 1;
    private int dmgBoostMaxAlt = 1;
    private int vampirismMaxAlt = 1;
    private int bindMaxAlt = 1;
    private int stunMaxAlt = 1;
    private int maxMax = 0;

    // Item First Use Trackers
    private bool CudgelFirstUse;
    private bool MirrorFirstUse;
    private bool ClothFirstUse;
    private bool CrystalBallFirstUse;
    private bool HatFirstUse;
    private bool VenomFirstUse;
    private bool SilvHandsFirstUse;
    private bool HairFirstUse;
    private bool IronShoesFirstUse;
    private bool AppleFirstUse;
    private bool WisdomFirstUse;

    private Animator playerAnim;
    private Animator enemyAnim;
    public AnimationClip angry1;
    public AnimationClip angry2;
    public Animator rootsAnim;

    public GameObject miniNarrator;
    public GameObject pointer;

    public int phase = 1;
    public GameObject x2Text;
    public GameObject x3Text;
    public TextMeshProUGUI playerDamageText;
    public TextMeshProUGUI maxText;
    public TextMeshProUGUI maxTextAlt;
    public TextMeshProUGUI enemyDamageText;
    public TextMeshProUGUI enemyDamageText2;
    private int shieldCounter = 0;
    private int birdCounter = 0;
    private int catCounter = 0;
    private int rootsCounter = 0;
    private int punchCounter = 0;
    private int attackVariants = 4;
    private List<int> introAttacks = new List<int>(new int[] {0, 1, 2, 3});

    public GameObject startParticles;
    public GameObject phase2Particles;
    public GameObject phase3Particles;
    public Animator windEffect;
    public GameObject windParticles;
    public Animator phaseTransition;
    public Animator thacamra;

    public bool badMusicSync = false;
    public float startVolume = 1;
    public MidBattleDialogueSystem phaseChange;
    private bool talking = false;
    private int maxAttack;
    private bool maxShield = false;

    public AudioSource theDie;
    public AudioSource gameOver;
    public VideoPlayer increaseMusicSpeed;
    public bool easy = true;
    public bool puppetFight = false;
    public bool tutorial = false;

    void Start(){
        state = BattleState.START;
        beatTime = (1 / bpm) * 60;
        CudgelFirstUse = true;
        MirrorFirstUse = true;
        ClothFirstUse = true;
        CrystalBallFirstUse = true;
        HatFirstUse = true;
        VenomFirstUse = true;
        SilvHandsFirstUse = true;
        HairFirstUse = true;
        IronShoesFirstUse = true;
        AppleFirstUse = true;
        WisdomFirstUse = true;
        playerAnim = player.GetComponent<Animator>();
        enemyAnim = enemy.GetComponent<Animator>();
        currentSong = MC100;
        startVolume = PlayerPrefs.GetFloat("MusicVolume", 1);
        Debug.Log(startVolume);
        preIntro();
    }

    void preIntro(){
        if (Mark.bookmarkSelected == BookmarkSelected.RED){
            redBookMarkContents.SetActive(true);
        } else {
            blueBookMarkContents.SetActive(true);
        }
        inventoryInterface.GetComponent<Canvas>().enabled = false;
        choicesInterface.GetComponent<Canvas>().enabled = false;
        playerStats = player.GetComponent<Stats>();
        enemyStats = enemy.GetComponent<Stats>();
        if (easy)
        {
            enemyStats.maxHP = 30;
            enemyStats.currentHP = 30;
            maxEnemyShield = 12;
            enemyStats.damage = 4;
        }
        else
        {
            enemyStats.maxHP = 40;
            enemyStats.currentHP = 40;
            maxEnemyShield = 16;
            enemyStats.damage = 6;
        }
        playerHUD.SetHUD(playerStats);
        enemyHUD.SetHUD(enemyStats);
        if (puppetFight)
        {
            introAttacks = new List<int>(new int[] { });
            attackVariants = 17;
            Wait();
        }
    }

    public void Wait()
	{
        state = BattleState.PLAYERTURN;
        StartCoroutine(StartIntro());
    }


    IEnumerator StartIntro(){
        skip.SetActive(false);
        miniNarrator.SetActive(true);
        if (!puppetFight)
            enemy.GetComponent<Animator>().SetTrigger("Fine Again");
        windParticles.SetActive(true);
        var windMain = windParticles.GetComponent<ParticleSystem>().main;
        windMain.simulationSpeed = 1;
        yield return new WaitForSeconds(10.6667f);
        if (buffLocation.passiveBuff == "AttackSet")
        {
            immune = true;
        }
        if (buffLocation.passiveBuff == "DefenseSet")
        {
            attackBuff = 1.5f;
        }
        if (buffLocation.passiveBuff == "BalanceSet")
        {
            playerStats.maxHP = (int)(playerStats.maxHP * 1.5f);
            playerStats.currentHP = playerStats.maxHP;
        }
        if (buffLocation.passiveBuff == "ControlSet")
        {
            noCooldowns = true;
        }
        playerHUD.SetHUD(playerStats);
        miniNarrator.SetActive(false);
        playerHUD.gameObject.SetActive(true);
        enemyHUD.gameObject.SetActive(true);
        player.GetComponent<Animator>().SetTrigger("Fight");
        if (puppetFight)
            enemy.GetComponent<Animator>().SetTrigger("Fight");
        PlayerTurn();
        EnemyAttackIndicatorController.Instance.TurnOnSprites();
        secondAttack.TurnOnSprites();
        thirdAttack.TurnOnSprites();
        int firstAttack = Random.Range(0, 4);
        if (puppetFight)
            firstAttack = Random.Range(7, 15);
        EnemyAttackIndicatorController.Instance.enableIndicator(firstAttack);
        DoTheMario(firstAttack);
        pointer.SetActive(true);
        startParticles.SetActive(true);
        windEffect.enabled = true;
        windMain = windParticles.GetComponent<ParticleSystem>().main;
        windMain.simulationSpeed = 3;
        if (playerStats.currentHP <= playerStats.maxHP * 0.25f)
        {
            MCstate = 2;
            currentSong = MC25;
        }
        else if (playerStats.currentHP <= playerStats.maxHP * 0.5f)
        {
            MCstate = 1;
            currentSong = MC50;
        }
        yield return null;
    }

    IEnumerator WaitForDoneProcess(float timeout)
    {
        while (!(Input.GetKey(KeyCode.Mouse1)))
        {
            yield return null;
            timeout -= Time.deltaTime;
            if (timeout <= 0f) break;
        }
    }

    YieldInstruction WaitForDone(float timeout) { return StartCoroutine(WaitForDoneProcess(timeout)); }

    // Player Attack Method
    IEnumerator PlayerAttack()
    {
        bool isDead = enemyStats.TakeDamage(playerStats.damage);
        transitionMusic();
        if ((enemyStats.currentHP <= enemyStats.maxHP * 0.5f) && phase == 1)
            StartCoroutine(Phase1MusicQuickStop());

        enemyHUD.SetHP(enemyStats);
        hitSound.Play();
        // Shake Effect upon taking damage (Very nice)
        for ( int i = 0; i < 10; i++)
        {
            enemy.transform.position += new Vector3(5f, 0, 0);
            yield return new WaitForSeconds(0.01f);
            enemy.transform.position -= new Vector3(5f, 0, 0);
            yield return new WaitForSeconds(0.01f);
        }
        if(isDead){
            state = BattleState.WON;
            EndBattle();
        } else {
            state = BattleState.ENEMYTURN;
            EnemyTurn();
        }
    }

    IEnumerator BePatient(float talkTime)
	{
        talking = true;
        yield return new WaitForSeconds(talkTime);
        talking = false;
    }

    IEnumerator DelayedPhase3()
	{
        if(tutorial){
            godAppleObject.SetActive(false);
            appleObject.SetActive(true);
        }
        yield return new WaitForSeconds(beatTime * 50);
        phaseTransition.SetTrigger("Phase 3");
        ChangeIdleAnimation(enemyAnim, angry2);
        phaseChange.MakeConversation(2, 10.15873015873016f / 2);
        var windMain2 = windParticles.GetComponent<ParticleSystem>().main;
        windMain2.simulationSpeed = 6;
        windMain2.startSize = new ParticleSystem.MinMaxCurve(0.5f, 2f);
        StartCoroutine(Phase3MusicTransition());
        attackVariants = 6;
        introAttacks.Add(5);
        thacamra.speed = 1.1f;
        phase = 3;

        thirdAttack.enableIndicator(0);
        DoTheMario(0);
        maxShield = true;
    }

    // Enemy Turn Method
    void EnemyTurn(){
        // Updates Health-Based Modifiers
        
        if ((enemyStats.currentHP <= 0.5 * enemyStats.maxHP) && phase == 1)
        {
            ChangeIdleAnimation(enemyAnim, angry1);
            phaseChange.MakeConversation(1, beatTime * 16);
            var windMain = windParticles.GetComponent<ParticleSystem>().main;
            windMain.simulationSpeed = 5;
            StartCoroutine(Phase2MusicTransition());
            attackVariants = 5;
            introAttacks.Add(4);
            phase = 2;
            if ((enemyStats.currentHP <= 0.25 * enemyStats.maxHP))
            {
                StartCoroutine(BePatient((beatTime * 50) + 11.15873015873016f));
                StartCoroutine(DelayedPhase3());
            }
            else
                StartCoroutine(BePatient(beatTime * 48));

            secondAttack.enableIndicator(0);
            DoTheMario(0);
            maxShield = true;

        }
        else if ((enemyStats.currentHP <= 0.25 * enemyStats.maxHP) && phase == 2)
        {
            if (tutorial){
                godAppleObject.SetActive(false);
                appleObject.SetActive(true);
            }
            phaseTransition.SetTrigger("Phase 3");
            ChangeIdleAnimation(enemyAnim, angry2);
            phaseChange.MakeConversation(2, 10.15873015873016f / 2);
            var windMain2 = windParticles.GetComponent<ParticleSystem>().main;
            windMain2.simulationSpeed = 6;
            windMain2.startSize = new ParticleSystem.MinMaxCurve(0.5f, 2f);
            StartCoroutine(Phase3MusicTransition());
            StartCoroutine(BePatient(11.15873015873016f));
            attackVariants = 6;
            introAttacks.Add(5);
            thacamra.speed = 1.1f;
            phase = 3;

            thirdAttack.enableIndicator(0);
            DoTheMario(0);
            maxShield = true;
        }

        ReduceEffects(enemyStats, playerStats);
        ReduceCooldownsAlt();
        enemyHpMod = 1.0f;

        playerAttackModifier = 1;
        // Debuff Logic =============
        if (false)
        {
            switch (enemyStats.debuffState)
            {
                case Debuff.BIND:
                    enemyAttackModifier = 1 - (0.25f * playerHpMod);
                    break;
            }
        }

        enemyAttackModifier = 1;
        if (enemyStats.dmgBoost)
        {
            enemyAttackModifier = enemyAttackModifier + (0.37f * dmgBoostMaxAlt);
        }
        if (enemyStats.bind)
        {
            enemyAttackModifier = enemyAttackModifier - (0.25f * playerHpMod * bindMax);
        }

        doneAttacks.Clear();
        doubleDoneAttacks.Clear();
        attackAmount = 0;

        // ==========================
        if (EnemyAttackIndicatorController.Instance.getEnabledIndicator() == 2) {
            StartCoroutine(BirdAttack());
        } else if (EnemyAttackIndicatorController.Instance.getEnabledIndicator() == 3) {
            StartCoroutine(CatAttack());
        } else if (EnemyAttackIndicatorController.Instance.getEnabledIndicator() == 0)
        {
            StartCoroutine(ShieldDefense());
        }
        else if (EnemyAttackIndicatorController.Instance.getEnabledIndicator() == 1)
        {
            StartCoroutine(ButterflyAttack());
        }
        else if (EnemyAttackIndicatorController.Instance.getEnabledIndicator() == 4)
        {
            StartCoroutine(RootsAttack());
        }
        else if (EnemyAttackIndicatorController.Instance.getEnabledIndicator() == 5)
        {
            StartCoroutine(PunchAttack());
        }
        else if (EnemyAttackIndicatorController.Instance.getEnabledIndicator() == 6)
        {
            StartCoroutine(ShieldAttack());
        }
        else if (EnemyAttackIndicatorController.Instance.getEnabledIndicator() == 7)
        {
            StartCoroutine(CudgelAttack());
        }
        else if (EnemyAttackIndicatorController.Instance.getEnabledIndicator() == 8)
        {
            StartCoroutine(ClothHeal());
        }
        else if (EnemyAttackIndicatorController.Instance.getEnabledIndicator() == 9)
        {
            StartCoroutine(HandsDefense());
        }
        else if (EnemyAttackIndicatorController.Instance.getEnabledIndicator() == 10)
        {
            StartCoroutine(HatAttack());
        }
        else if (EnemyAttackIndicatorController.Instance.getEnabledIndicator() == 11)
        {
            StartCoroutine(MirrorBuff());
        }
        else if (EnemyAttackIndicatorController.Instance.getEnabledIndicator() == 12)
        {
            StartCoroutine(AppleBuff());
        }
        else if (EnemyAttackIndicatorController.Instance.getEnabledIndicator() == 13)
        {
            StartCoroutine(VenomBuff());
        }
        else if (EnemyAttackIndicatorController.Instance.getEnabledIndicator() == 14)
        {
            StartCoroutine(HairAttack());
        }
        else if (EnemyAttackIndicatorController.Instance.getEnabledIndicator() == 15)
        {
            StartCoroutine(BallBuff());
        }
        else if (EnemyAttackIndicatorController.Instance.getEnabledIndicator() == 16)
        {
            StartCoroutine(ShoesAttack());
        }
    }


    // Method to End Battle
    void EndBattle(){
        if (state == BattleState.WON){
            StartCoroutine(WinSequence());
        } else if (state == BattleState.LOST){
            StartCoroutine(LoseSequence());
        }
    }

    IEnumerator WinSequence()
    {
        if (buffLocation.passiveBuff == "AttackSet")
        {
            PlayerPrefs.SetInt("OffenseComplete", 1);
        }
        if (buffLocation.passiveBuff == "DefenseSet")
        {
            PlayerPrefs.SetInt("DefenseComplete", 1);
        }
        if (buffLocation.passiveBuff == "BalanceSet")
        {
            PlayerPrefs.SetInt("BalanceComplete", 1);
        }
        if (buffLocation.passiveBuff == "ControlSet")
        {
            PlayerPrefs.SetInt("ControlComplete", 1);
        }
        phaseTransition.SetTrigger("End");
        enemyAnim.SetTrigger("Die");
        currentSong.Stop();
        windEffect.enabled = false;
        thacamra.enabled = false;
        playerHUD.gameObject.SetActive(false);
        enemyHUD.gameObject.SetActive(false);
        EnemyAttackIndicatorController.Instance.ResetParticles();
        EnemyAttackIndicatorController.Instance.disableAllIndicators();
        secondAttack.disableAllIndicators();
        thirdAttack.disableAllIndicators();
        yield return new WaitForSeconds(3);
        startParticles.SetActive(false);
        phase2Particles.SetActive(false);
        phase3Particles.SetActive(false);
        windParticles.SetActive(false);
        playerHUD.actualShield.SetActive(false);
        enemyHUD.actualShield.SetActive(false);
        yield return new WaitForSeconds(3.6667f);
        phaseChange.MakeConversation(3, 3.5555f);
        yield return new WaitForSeconds(31.3333f);
        SceneChanger.Instance.FadeToNextScene();
    }

    IEnumerator LoseSequence()
	{
        playerAnim.SetTrigger("Die");
        while (currentSong.pitch > 0)
        {
            currentSong.pitch -= 1f * Time.deltaTime;
            yield return null;
        }
        currentSong.Stop();
        gameOver.Play();
        yield return new WaitForSeconds(1);
        GameOverOverlay.Instance.FadeToGameOver();
    }

    void UpdateEffectDisplay(GameObject display, TextMeshProUGUI text, bool buffs, Stats character)
	{
        if (buffs)
		{
            if (!character.reflect && !character.dmgBoost && !character.vampirism && !character.max)
			{
                display.SetActive(false);
                text.text = "";
			}
            else
			{
                display.SetActive(true);
                text.text = "";
                if (character.reflect)
				{
                    text.text = "<color=#2A72FF>REFLECT";
                }
                if (character.dmgBoost)
                {
                    if (text.text == "")
                        text.text = "<color=#FF2A2A>DAMAGE BOOST";
                    else
                        text.text += ", <color=#FF2A2A>DAMAGE BOOST";
                }
                if (character.vampirism)
                {
                    if (text.text == "")
                        text.text = "<color=#696969>VAMPIRISM";
                    else
                        text.text += ", <color=#696969>VAMPIRISM";
                }
                if (character.max)
                {
                    if(maxMax == 0){
                        if (text.text == "")
                            text.text = "<color=#FFFFFF>???";
                        else
                            text.text += ", <color=#FFFFFF>???";
                    } else {
                        if (text.text == "")
                            text.text = "<color=#FFFFFF>MAX ATTACK";
                        else
                            text.text += ", <color=#FFFFFF>MAX ATTACK";
                    }
                }
            }
		}
        else
        {
            if (!character.bind && !character.stun)
            {
                display.SetActive(false);
                text.text = "";
            }
            else
            {
                display.SetActive(true);
                text.text = "";
                if (character.bind)
                {
                    text.text = "<color=#7D7C00>BOUND";
                }
                if (character.stun)
                {
                    if (text.text == "")
                        text.text = "<color=#C75700>STUNNED";
                    else
                        text.text += ", <color=#C75700>STUNNED";
                }
            }
        }
    }

    void ReduceEffects(Stats stats, Stats statsALt)
	{
        // Reduces Duration of Buffs and Debuffs
        if (stats.currentReflectDuration > 0)
        {
            stats.currentReflectDuration -= 1;
            if (stats.currentReflectDuration == 0)
            {
                stats.reflect = false;
                if (stats == playerStats)
                {
                    if (reflectMax >= 2)
                        EnableItem(mirrorObject);
                    reflectMax = 1;
                    mirrorObject.transform.Find("ItemIcon").gameObject.GetComponent<HoverForDescription>().maxTime = false;
                }
                else
                    reflectMaxAlt = 1;
                UpdateEffectDisplay(stats.buffDisplay, stats.buffText, true, stats);
            }
        }
        if (stats.currentDmgBoostDuration > 0)
        {
            stats.currentDmgBoostDuration -= 1;
            if (stats.currentDmgBoostDuration == 0)
            {
                stats.dmgBoost = false;
                if (stats == playerStats)
                {
                    if (dmgBoostMax >= 2)
                        EnableItem(appleObject);
                    dmgBoostMax = 1;
                    appleObject.transform.Find("ItemIcon").gameObject.GetComponent<HoverForDescription>().maxTime = false;
                }
                else
                    dmgBoostMaxAlt = 1;
                UpdateEffectDisplay(stats.buffDisplay, stats.buffText, true, stats);
            }
        }
        if (stats.currentMaxDuration > 0)
        {
            stats.currentMaxDuration -= 1;
            if (stats.currentMaxDuration == 0)
            {
                stats.max = false;
                if (stats == playerStats)
                {
                    if (dmgBoostMax >= 2)
                        EnableItem(godAppleObject);
                    dmgBoostMax = 1;
                    godAppleObject.transform.Find("ItemIcon").gameObject.GetComponent<HoverForDescription>().maxTime = false;
                }
                UpdateEffectDisplay(stats.buffDisplay, stats.buffText, true, stats);
            }
        }
        if (stats.currentVampirismDuration > 0)
        {
            stats.currentVampirismDuration -= 1;
            if (stats.currentVampirismDuration == 0)
            {
                stats.vampirism = false;
                if (stats == playerStats)
                {
                    if (vampirismMax >= 2)
                        EnableItem(venomObject);
                    vampirismMax = 1;
                    venomObject.transform.Find("ItemIcon").gameObject.GetComponent<HoverForDescription>().maxTime = false;
                }
                else
                    vampirismMaxAlt = 1;
                UpdateEffectDisplay(stats.buffDisplay, stats.buffText, true, stats);
            }
        }
        if (stats.currentStunDuration > 0)
        {
            stats.currentStunDuration -= 1;
            if (stats.currentStunDuration == 0)
            {
                stats.stun = false;
                UpdateEffectDisplay(stats.debuffDisplay, stats.debuffText, false, stats);
            }
        }
        if (stats.currentBindDuration > 0)
        {
            stats.currentBindDuration -= 1;
            if (stats.currentBindDuration == 0)
            {
                stats.bind = false;
                if (statsALt == playerStats)
                {
                    if (bindMax >= 2)
                        EnableItem(hairObject);
                    bindMax = 1;
                    hairObject.transform.Find("ItemIcon").gameObject.GetComponent<HoverForDescription>().maxTime = false;
                }
                else
                    bindMaxAlt = 1;
                UpdateEffectDisplay(stats.debuffDisplay, stats.debuffText, false, stats);
            }
        }
        if (statsALt.currentBindDuration > 0)
        {
            statsALt.currentBindDuration -= 1;
            if (statsALt.currentBindDuration == 0)
            {
                statsALt.bind = false;
                if (stats == playerStats)
                {
                    if (bindMax >= 2)
                        EnableItem(hairObject);
                    bindMax = 1;
                    hairObject.transform.Find("ItemIcon").gameObject.GetComponent<HoverForDescription>().maxTime = false;
                }
                else
                    bindMaxAlt = 1;
                UpdateEffectDisplay(statsALt.debuffDisplay, statsALt.debuffText, false, statsALt);
            }
        }
        if (statsALt.currentStunDuration > 0)
        {
            statsALt.currentStunDuration -= 1;
            if (statsALt.currentStunDuration == 0)
            {
                statsALt.stun = false;
                if (stats == playerStats)
                {
                    if (stunMax >= 2)
                        EnableItem(shoesObject);
                    stunMax = 1;
                    shoesObject.transform.Find("ItemIcon").gameObject.GetComponent<HoverForDescription>().maxTime = false;
                }
                else
                    stunMaxAlt = 1;
                UpdateEffectDisplay(statsALt.debuffDisplay, statsALt.debuffText, false, statsALt);
            }
        }
    }

    void ReduceCooldowns()
    {
        // Reduces Duration of Cooldowns
        if (mirrorRestore > 0)
        {
            mirrorRestore -= 1;
            mirrorObject.transform.Find("ItemName").gameObject.GetComponent<TextMeshProUGUI>().text = "Cooldown: " + mirrorRestore;
            if (mirrorRestore == 0)
            {
                EnableItem(mirrorObject);
            }
        }
        if (clothRestore > 0)
        {
            clothRestore -= 1;
            clothObject.transform.Find("ItemName").gameObject.GetComponent<TextMeshProUGUI>().text = "Cooldown: " + clothRestore;
            if (clothRestore == 0)
            {
                EnableItem(clothObject);
            }
        }
        if (ballRestore > 0)
        {
            ballRestore -= 1;
            ballObject.transform.Find("ItemName").gameObject.GetComponent<TextMeshProUGUI>().text = "Cooldown: " + ballRestore;
            if (ballRestore == 0)
            {
                EnableItem(ballObject);
            }
        }
        if (appleRestore > 0)
        {
            appleRestore -= 1;
            appleObject.transform.Find("ItemName").gameObject.GetComponent<TextMeshProUGUI>().text = "Cooldown: " + appleRestore;
            if (appleRestore == 0)
            {
                EnableItem(appleObject);
            }
        }
         if (godAppleRestore > 0)
        {
            godAppleRestore -= 1;
            godAppleObject.transform.Find("ItemName").gameObject.GetComponent<TextMeshProUGUI>().text = "Cooldown: " + godAppleRestore;
            if (godAppleRestore == 0)
            {
                EnableItem(godAppleObject);
            }
        }
        if (venomRestore > 0)
        {
            venomRestore -= 1;
            venomObject.transform.Find("ItemName").gameObject.GetComponent<TextMeshProUGUI>().text = "Cooldown: " + venomRestore;
            if (venomRestore == 0)
            {
                EnableItem(venomObject);
            }
        }
        if (handsRestore > 0)
        {
            handsRestore -= 1;
            handsObject.transform.Find("ItemName").gameObject.GetComponent<TextMeshProUGUI>().text = "Cooldown: " + handsRestore;
            if (handsRestore == 0)
            {
                EnableItem(handsObject);
            }
        }
        if (hairRestore > 0)
        {
            hairRestore -= 1;
            hairObject.transform.Find("ItemName").gameObject.GetComponent<TextMeshProUGUI>().text = "Cooldown: " + hairRestore;
            if (hairRestore == 0)
            {
                EnableItem(hairObject);
            }
        }
    }

    void ReduceCooldownsAlt()
    {
        // Reduces Duration of Cooldowns
        if (mirrorRestoreAlt > 0)
        {
            mirrorRestoreAlt -= 1;
        }
        if (clothRestoreAlt > 0)
        {
            clothRestoreAlt -= 1;
        }
        if (ballRestoreAlt > 0)
        {
            ballRestoreAlt -= 1;
        }
        if (appleRestoreAlt > 0)
        {
            appleRestoreAlt -= 1;
        }
        if (venomRestoreAlt > 0)
        {
            venomRestoreAlt -= 1;
        }
        if (handsRestoreAlt > 0)
        {
            handsRestoreAlt -= 1;
        }
        if (hairRestoreAlt > 0)
        {
            hairRestoreAlt -= 1;
        }
    }

    // Would be nice to not have to update this function every time an item is added but this whole script is spaghetti now basically so siudfhaisudhfauis
    void MaxUpdate()
	{
        // Mirror
        mirrorObject.transform.Find("MAXFLAME").gameObject.SetActive(false);
        if (mirrorRestore <= 0 && playerStats.reflect && reflectMax < 2)
        {
            mirrorObject.transform.Find("MAXFLAME").gameObject.SetActive(true);
            mirrorObject.transform.Find("ItemIcon").gameObject.GetComponent<HoverForDescription>().maxTime = true;
        }
        else if (reflectMax >= 2)
        {
            mirrorObject.GetComponent<Button>().interactable = false;
            mirrorObject.transform.Find("ItemIcon").gameObject.GetComponent<Image>().color = new Color(0.4f, 0.4f, 0.4f, 1.0f);
            mirrorObject.transform.Find("ItemIcon").gameObject.GetComponent<HoverForDescription>().disabled = true;
            mirrorObject.transform.Find("ItemIcon").gameObject.GetComponent<HoverForDescription>().maxTime = false;
            mirrorObject.transform.Find("ItemName").gameObject.GetComponent<TextMeshProUGUI>().text = "Maxed Out!";
        }

        // Apple
        appleObject.transform.Find("MAXFLAME").gameObject.SetActive(false);
        if (appleRestore <= 0 && playerStats.dmgBoost && dmgBoostMax < 2)
        {
            appleObject.transform.Find("MAXFLAME").gameObject.SetActive(true);
            appleObject.transform.Find("ItemIcon").gameObject.GetComponent<HoverForDescription>().maxTime = true;
        }
        else if (dmgBoostMax >= 2)
        {
            appleObject.GetComponent<Button>().interactable = false;
            appleObject.transform.Find("ItemIcon").gameObject.GetComponent<Image>().color = new Color(0.4f, 0.4f, 0.4f, 1.0f);
            appleObject.transform.Find("ItemIcon").gameObject.GetComponent<HoverForDescription>().disabled = true;
            appleObject.transform.Find("ItemIcon").gameObject.GetComponent<HoverForDescription>().maxTime = false;
            appleObject.transform.Find("ItemName").gameObject.GetComponent<TextMeshProUGUI>().text = "Maxed Out!";
        }

        // God Apple
        godAppleObject.transform.Find("MAXFLAME").gameObject.SetActive(false);
        if (godAppleRestore <= 0 && playerStats.max && maxMax < 2)
        {
            godAppleObject.transform.Find("MAXFLAME").gameObject.SetActive(true);
            godAppleObject.transform.Find("ItemIcon").gameObject.GetComponent<HoverForDescription>().maxTime = true;
        }
        else if (maxMax >= 2)
        {
            godAppleObject.GetComponent<Button>().interactable = false;
            godAppleObject.transform.Find("ItemIcon").gameObject.GetComponent<Image>().color = new Color(0.4f, 0.4f, 0.4f, 1.0f);
            godAppleObject.transform.Find("ItemIcon").gameObject.GetComponent<HoverForDescription>().disabled = true;
            godAppleObject.transform.Find("ItemIcon").gameObject.GetComponent<HoverForDescription>().maxTime = false;
            godAppleObject.transform.Find("ItemName").gameObject.GetComponent<TextMeshProUGUI>().text = "Maxed Out!";
        }

        // Shoes
        shoesObject.transform.Find("MAXFLAME").gameObject.SetActive(false);
        if (playerStats.stun && enemyStats.stun && stunMax < 2)
        {
            shoesObject.transform.Find("MAXFLAME").gameObject.SetActive(true);
            shoesObject.transform.Find("ItemIcon").gameObject.GetComponent<HoverForDescription>().maxTime = true;
        }
        else if (stunMax >= 2)
        {
            shoesObject.GetComponent<Button>().interactable = false;
            shoesObject.transform.Find("ItemIcon").gameObject.GetComponent<Image>().color = new Color(0.4f, 0.4f, 0.4f, 1.0f);
            shoesObject.transform.Find("ItemIcon").gameObject.GetComponent<HoverForDescription>().disabled = true;
            shoesObject.transform.Find("ItemIcon").gameObject.GetComponent<HoverForDescription>().maxTime = false;
            shoesObject.transform.Find("ItemName").gameObject.GetComponent<TextMeshProUGUI>().text = "Maxed Out!";
        }

        // Venom
        venomObject.transform.Find("MAXFLAME").gameObject.SetActive(false);
        if (venomRestore <= 0 && playerStats.vampirism && vampirismMax < 2)
        {
            venomObject.transform.Find("MAXFLAME").gameObject.SetActive(true);
            venomObject.transform.Find("ItemIcon").gameObject.GetComponent<HoverForDescription>().maxTime = true;
        }
        else if (vampirismMax >= 2)
        {
            venomObject.GetComponent<Button>().interactable = false;
            venomObject.transform.Find("ItemIcon").gameObject.GetComponent<Image>().color = new Color(0.4f, 0.4f, 0.4f, 1.0f);
            venomObject.transform.Find("ItemIcon").gameObject.GetComponent<HoverForDescription>().disabled = true;
            venomObject.transform.Find("ItemIcon").gameObject.GetComponent<HoverForDescription>().maxTime = false;
            venomObject.transform.Find("ItemName").gameObject.GetComponent<TextMeshProUGUI>().text = "Maxed Out!";
        }

        // Hair
        hairObject.transform.Find("MAXFLAME").gameObject.SetActive(false);
        if (hairRestore <= 0 && enemyStats.bind && bindMax < 2)
        {
            hairObject.transform.Find("MAXFLAME").gameObject.SetActive(true);
            hairObject.transform.Find("ItemIcon").gameObject.GetComponent<HoverForDescription>().maxTime = true;
        }
        else if (bindMax >= 2)
        {
            hairObject.GetComponent<Button>().interactable = false;
            hairObject.transform.Find("ItemIcon").gameObject.GetComponent<Image>().color = new Color(0.4f, 0.4f, 0.4f, 1.0f);
            hairObject.transform.Find("ItemIcon").gameObject.GetComponent<HoverForDescription>().disabled = true;
            hairObject.transform.Find("ItemIcon").gameObject.GetComponent<HoverForDescription>().maxTime = false;
            hairObject.transform.Find("ItemName").gameObject.GetComponent<TextMeshProUGUI>().text = "Maxed Out!";
        }
    }

    // Method to start PlayerTurn
    void PlayerTurn()
    {
        if (playerStats.currentHP <= 0.25 * playerStats.maxHP)
        {
            playerHpMod = 3.0f;
        }
        else if (playerStats.currentHP <= 0.5 * playerStats.maxHP)
        {
            playerHpMod = 2.0f;
        }
        else
        {
            playerHpMod = 1.0f;
        }

        transitionMusic();
        ReduceEffects(playerStats, enemyStats);
        ReduceCooldowns();


        if (!playerStats.stun && !playerStats.bind)
		{
            shoesObject.GetComponent<Button>().interactable = false;
            shoesObject.transform.Find("ItemIcon").gameObject.GetComponent<Image>().color = new Color(0.4f, 0.4f, 0.4f, 1.0f);
        }
        else
		{
            shoesObject.GetComponent<Button>().interactable = true;
            shoesObject.transform.Find("ItemIcon").gameObject.GetComponent<Image>().color = new Color(1.0f, 1.0f, 1.0f, 1.0f);
            shoesObject.transform.Find("ItemIcon").gameObject.GetComponent<HoverForDescription>().disabled = false;
            shoesObject.transform.Find("ItemName").gameObject.GetComponent<TextMeshProUGUI>().text = "";
        }

        if (!playerStats.vampirism && !playerStats.dmgBoost && !playerStats.reflect && !playerStats.max)
		{
            ballObject.GetComponent<Button>().interactable = false;
            ballObject.transform.Find("ItemIcon").gameObject.GetComponent<Image>().color = new Color(0.4f, 0.4f, 0.4f, 1.0f);
        }
        else
		{
            ballObject.GetComponent<Button>().interactable = true;
            ballObject.transform.Find("ItemIcon").gameObject.GetComponent<Image>().color = new Color(1.0f, 1.0f, 1.0f, 1.0f);
            ballObject.transform.Find("ItemIcon").gameObject.GetComponent<HoverForDescription>().disabled = false;
            ballObject.transform.Find("ItemName").gameObject.GetComponent<TextMeshProUGUI>().text = "";
        }
        if (phase == 1)
		{
            godAppleObject.GetComponent<Button>().interactable = false;
            godAppleObject.transform.Find("ItemIcon").gameObject.GetComponent<Image>().color = new Color(0.4f, 0.4f, 0.4f, 1.0f);
        }
        else
		{
            godAppleObject.GetComponent<Button>().interactable = true;
            godAppleObject.transform.Find("ItemIcon").gameObject.GetComponent<Image>().color = new Color(1.0f, 1.0f, 1.0f, 1.0f);
            godAppleObject.transform.Find("ItemIcon").gameObject.GetComponent<HoverForDescription>().disabled = false;
            godAppleObject.transform.Find("ItemName").gameObject.GetComponent<TextMeshProUGUI>().text = "";
        }

        MaxUpdate();

        // Buff Logic =============
        if (false)
        {
            switch (playerStats.buffState)
            {
                case Buff.DMGBOOST:
                    playerAttackModifier = 1.5f;
                    break;
            }
        }

        playerAttackModifier = 1;
        if (playerStats.dmgBoost)
		{
            playerAttackModifier = playerAttackModifier + (0.25f * playerHpMod*dmgBoostMax);
		}
        if (playerStats.bind)
        {
            playerAttackModifier = playerAttackModifier - (0.37f * bindMaxAlt);
        }
        if (playerStats.max)
        {
            playerAttackModifier = playerAttackModifier + (1f * maxMax);
        }
        //=========================


        EventSystem.current.SetSelectedGameObject(null);
        OnInventoryButton();
    }

    // Method to write text in dialogue box (Unused right now)
    IEnumerator Write(string text){
        textTransition.Play();
        yield return new WaitForSeconds(2f);
    }

    // Opens Inventory Menu
    public void OnInventoryButton(){
        inventoryInterface.GetComponent<Canvas>().enabled = true;
    }

    // Method to close all UI
    void closeAllInterface(){
        choicesInterface.GetComponent<Canvas>().enabled = false;
        inventoryInterface.GetComponent<Canvas>().enabled = false;
    }

    // Method for Music Transitions UNFINISHED!
    void transitionMusic()
	{
        if (phase == 1)
        {
            if ((playerStats.currentHP <= (playerStats.maxHP * 0.25f)) && MCstate == 0)
            {
                MCstate = 2;
                MC25Switch.StopAllCoroutines();
                MC100Switch.StopAllCoroutines();
                MC100Switch.VariableTransition(MC25, 0.1666f);
                currentSong = MC25;
            }
            else if ((playerStats.currentHP <= (playerStats.maxHP * 0.5f)) && MCstate == 0)
            {
                MCstate = 1;
                MC50Switch.StopAllCoroutines();
                MC100Switch.StopAllCoroutines();
                MC100Switch.VariableTransition(MC50, 0.1666f);
                currentSong = MC50;
            }
            else if ((playerStats.currentHP > (playerStats.maxHP * 0.5f)) && MCstate == 1)
            {
                MCstate = 0;
                MC100Switch.StopAllCoroutines();
                MC50Switch.StopAllCoroutines();
                MC50Switch.VariableTransition(MC100, 0.1666f);
                currentSong = MC100;
            }
            else if ((playerStats.currentHP <= (playerStats.maxHP * 0.25f)) && MCstate == 1)
            {
                MCstate = 2;
                MC25Switch.StopAllCoroutines();
                MC50Switch.StopAllCoroutines();
                MC50Switch.VariableTransition(MC25, 0.1666f);
                currentSong = MC25;
            }
            else if ((playerStats.currentHP > (playerStats.maxHP * 0.5f)) && MCstate == 2)
            {
                MCstate = 0;
                MC100Switch.StopAllCoroutines();
                MC25Switch.StopAllCoroutines();
                MC25Switch.VariableTransition(MC100, 0.1666f);
                currentSong = MC100;
            }
            else if ((playerStats.currentHP > (playerStats.maxHP * 0.25f)) && MCstate == 2)
            {
                MCstate = 1;
                MC50Switch.StopAllCoroutines();
                MC25Switch.StopAllCoroutines();
                MC25Switch.VariableTransition(MC50, 0.1666f);
                currentSong = MC50;
            }
        }
        else if (phase == 2)
		{
            if ((playerStats.currentHP <= (playerStats.maxHP * 0.25f)) && MCstate == 0)
            {
                MCstate = 2;
                MC25SW50Switch.StopAllCoroutines();
                MC100SW50Switch.StopAllCoroutines();
                MC100SW50Switch.VariableTransition(MC25SW50, 0.1666f);
                currentSong = MC25SW50;
            }
            else if ((playerStats.currentHP <= (playerStats.maxHP * 0.5f)) && MCstate == 0)
            {
                MCstate = 1;
                MC50SW50Switch.StopAllCoroutines();
                MC50SW50Switch.StopAllCoroutines();
                MC100SW50Switch.VariableTransition(MC50SW50, 0.1666f);
                currentSong = MC50SW50;
            }
            else if ((playerStats.currentHP > (playerStats.maxHP * 0.5f)) && MCstate == 1)
            {
                MCstate = 0;
                MC100SW50Switch.StopAllCoroutines();
                MC50SW50Switch.StopAllCoroutines();
                MC50SW50Switch.VariableTransition(MC100SW50, 0.1666f);
                currentSong = MC100SW50;
            }
            else if ((playerStats.currentHP <= (playerStats.maxHP * 0.25f)) && MCstate == 1)
            {
                MCstate = 2;
                MC25SW50Switch.StopAllCoroutines();
                MC50SW50Switch.StopAllCoroutines();
                MC50SW50Switch.VariableTransition(MC25SW50, 0.1666f);
                currentSong = MC25SW50;
            }
            else if ((playerStats.currentHP > (playerStats.maxHP * 0.5f)) && MCstate == 2)
            {
                MCstate = 0;
                MC100SW50Switch.StopAllCoroutines();
                MC25SW50Switch.StopAllCoroutines();
                MC25SW50Switch.VariableTransition(MC100SW50, 0.1666f);
                currentSong = MC100SW50;
            }
            else if ((playerStats.currentHP > (playerStats.maxHP * 0.25f)) && MCstate == 2)
            {
                MCstate = 1;
                MC50SW50Switch.StopAllCoroutines();
                MC25SW50Switch.StopAllCoroutines();
                MC25SW50Switch.VariableTransition(MC50SW50, 0.1666f);
                currentSong = MC50SW50;
            }
        }

        else if (phase == 3)
        {
            if ((playerStats.currentHP <= (playerStats.maxHP * 0.25f)) && MCstate == 0)
            {
                MCstate = 2;
                MC25SW25Switch.StopAllCoroutines();
                MC100SW25Switch.StopAllCoroutines();
                MC100SW25Switch.VariableTransition(MC25SW25, 0.1666f);
                currentSong = MC25SW25;
            }
            else if ((playerStats.currentHP <= (playerStats.maxHP * 0.5f)) && MCstate == 0)
            {
                MCstate = 1;
                MC50SW25Switch.StopAllCoroutines();
                MC50SW25Switch.StopAllCoroutines();
                MC100SW25Switch.VariableTransition(MC50SW25, 0.1666f);
                currentSong = MC50SW25;
            }
            else if ((playerStats.currentHP > (playerStats.maxHP * 0.5f)) && MCstate == 1)
            {
                MCstate = 0;
                MC100SW25Switch.StopAllCoroutines();
                MC50SW25Switch.StopAllCoroutines();
                MC50SW25Switch.VariableTransition(MC100SW25, 0.1666f);
                currentSong = MC100SW25;
            }
            else if ((playerStats.currentHP <= (playerStats.maxHP * 0.25f)) && MCstate == 1)
            {
                MCstate = 2;
                MC25SW25Switch.StopAllCoroutines();
                MC50SW25Switch.StopAllCoroutines();
                MC50SW25Switch.VariableTransition(MC25SW25, 0.1666f);
                currentSong = MC25SW25;
            }
            else if ((playerStats.currentHP > (playerStats.maxHP * 0.5f)) && MCstate == 2)
            {
                MCstate = 0;
                MC100SW25Switch.StopAllCoroutines();
                MC25SW25Switch.StopAllCoroutines();
                MC25SW25Switch.VariableTransition(MC100SW25, 0.1666f);
                currentSong = MC100SW25;
            }
            else if ((playerStats.currentHP > (playerStats.maxHP * 0.25f)) && MCstate == 2)
            {
                MCstate = 1;
                MC50SW25Switch.StopAllCoroutines();
                MC25SW25Switch.StopAllCoroutines();
                MC25SW25Switch.VariableTransition(MC50SW25, 0.1666f);
                currentSong = MC50SW25;
            }
        }

        if (MCstate == 0)
		{
            x2Text.SetActive(false);
            x3Text.SetActive(false);
        }
        else if (MCstate == 1)
        {
            x2Text.SetActive(true);
            x3Text.SetActive(false);
        }
        else if (MCstate == 2)
        {
            x2Text.SetActive(false);
            x3Text.SetActive(true);
        }
    }

    void ChangeIdleAnimation(Animator animator, AnimationClip anim)
	{
        AnimatorOverrideController aoc = new AnimatorOverrideController(animator.runtimeAnimatorController);
        var anims = new List<KeyValuePair<AnimationClip, AnimationClip>>();
        foreach (var a in aoc.animationClips)
            if (a.name == "regular idle")
                anims.Add(new KeyValuePair<AnimationClip, AnimationClip>(a, anim));
        aoc.ApplyOverrides(anims);
        animator.runtimeAnimatorController = aoc;
    }


    IEnumerator Phase1MusicQuickStop()
	{
        MC100Switch.StopAllCoroutines();
        MC50Switch.StopAllCoroutines();
        MC25Switch.StopAllCoroutines();
        while (currentSong.volume > 0)
        {
            currentSong.volume -= startVolume * Time.deltaTime;
            yield return null;
        }
        MC100.Stop();
        MC50.Stop();
        MC25.Stop();
    }

    IEnumerator Phase2MusicQuickStop(AudioSource song)
    {
        MC100SW50Switch.StopAllCoroutines();
        MC50SW50Switch.StopAllCoroutines();
        MC25SW50Switch.StopAllCoroutines();
        while (song.pitch > 0)
        {
            song.pitch -= 1.1f * Time.deltaTime;
            yield return null;
        }
        MC100SW50.Stop();
        MC50SW50.Stop();
        MC25SW50.Stop();
    }

    IEnumerator Phase2MusicTransition()
	{
        MC100.Stop();
        MC50.Stop();
        MC25.Stop();
        MC100Switch.StopAllCoroutines();
        MC50Switch.StopAllCoroutines();
        MC25Switch.StopAllCoroutines();
        yield return new WaitForSeconds(beatTime * 16);
        phaseTransition.SetTrigger("Phase 2");
        thacamra.enabled = true;
        AudioSource transitionTo;
        if (playerStats.currentHP <= (playerStats.maxHP * 0.25f))
		{
            transitionTo = MC25SW50;
		}
        else if (playerStats.currentHP <= (playerStats.maxHP * 0.5f))
        {
            transitionTo = MC50SW50;
        }
        else
		{
            transitionTo = MC100SW50;
        }

        transitionTo.volume = 0;
        MC25SW50.time = 130.6667f;
        MC50SW50.time = 130.6667f;
        MC100SW50.time = 130.6667f;
        MC25SW50.Play();
        MC50SW50.Play();
        MC100SW50.Play();

        currentSong = transitionTo;
        while (currentSong.volume < startVolume * 0.05f)
        {
            currentSong.volume += (startVolume * 0.05f) * Time.deltaTime / (beatTime * 8);
            yield return null;
        }
        while (currentSong.volume < startVolume * 0.25f)
		{
            currentSong.volume += (startVolume * 0.20f) * Time.deltaTime / (beatTime * 8);
            yield return null;
        }
        while (currentSong.volume < startVolume)
        {
            currentSong.volume += (startVolume * 0.75f) * Time.deltaTime / (beatTime * 16);
            yield return null;
        }
        currentSong.volume = startVolume;
        phase2Particles.SetActive(true);
    }
    IEnumerator Phase3MusicTransition()
    {
        MC100SW50Switch.StopAllCoroutines();
        MC50SW50Switch.StopAllCoroutines();
        MC25SW50Switch.StopAllCoroutines();
        AudioSource transitionTo;
        if (playerStats.currentHP <= (playerStats.maxHP * 0.25f))
        {
            transitionTo = MC25SW25;
        }
        else if (playerStats.currentHP <= (playerStats.maxHP * 0.5f))
        {
            transitionTo = MC50SW25;
        }
        else
        {
            transitionTo = MC100SW25;
        }

        transitionTo.volume = 0;
        MC25SW25.time = currentSong.time / 1.1f;
        MC50SW25.time = currentSong.time / 1.1f;
        MC100SW25.time = currentSong.time / 1.1f;
        MC25SW25.Play();
        MC50SW25.Play();
        MC100SW25.Play();
        transitionTo.pitch = 0.9f;

        while (currentSong.pitch < 1.1)
        {
            currentSong.pitch += 0.1f * Time.deltaTime / 10.15873015873016f;
            transitionTo.pitch += 0.1f * Time.deltaTime / 10.15873015873016f;
            if (currentSong.pitch >= 1.05f)
            {
                currentSong.volume -= startVolume * Time.deltaTime / 10.15873015873016f/2;
                transitionTo.volume += startVolume * Time.deltaTime / 10.15873015873016f/2;
            }
            yield return null;
        }
        MC25SW25.time = currentSong.time / 1.1f;
        MC50SW25.time = currentSong.time / 1.1f;
        MC100SW25.time = currentSong.time / 1.1f;
        StartCoroutine(Phase2MusicQuickStop(currentSong));

        transitionTo.volume = startVolume;
        currentSong = transitionTo;
        phase3Particles.SetActive(true);
        windEffect.SetTrigger("Windy");
    }

    float GetNextBeat(float multiplier)
	{
        float untilNext = (beatTime * multiplier) - (MC100.time % (beatTime * multiplier));
        Debug.Log(beatTime);
        Debug.Log(multiplier);
        Debug.Log(MC100.time);
        return (beatTime * multiplier) - (MC100.time % (beatTime * multiplier));
	}

    bool CheckMaxAttack()
	{
        if (playerStats.dmgBoost)
        {
            playerAttackModifier = 1 + (0.25f * playerHpMod * dmgBoostMax);
        }
        if (buffLocation.passiveBuff == "AttackSet" || buffLocation.passiveBuff == "ControlSet")
            maxAttack = (int)(6 * playerAttackModifier * playerHpMod * attackBuff);
        else
            maxAttack = (int)(3 * playerAttackModifier * playerHpMod * attackBuff);
        if (phase == 1)
		{
            return maxAttack >= enemyStats.barrier + (enemyStats.currentHP - enemyStats.maxHP * 0.5f);
		}
        else if (phase == 2)
		{
            return maxAttack >= enemyStats.barrier + (enemyStats.currentHP - enemyStats.maxHP * 0.25f);
        }
        else
		{
            return maxAttack >= enemyStats.barrier + enemyStats.currentHP;
        }
    }

    bool EnemyHasDebuffs()
	{
        if (enemyStats.bind || enemyStats.stun)
            return true;
        else
            return false;
	}

    bool EnemyHasBuffs()
    {
        if (enemyStats.dmgBoost || enemyStats.reflect || enemyStats.vampirism)
            return true;
        else
            return false;
    }

    // Methods for Enemy Actions ===================================================================
    IEnumerator BirdAttack(){

        yield return new WaitWhile(() => talking);
        if (!doneAttacks.Contains(2))
            birdCounter += 1;
        attackAmount += 1;
        EnemyAttackIndicatorController.Instance.disableAllIndicators();
        if (phase >= 2)
            secondAttack.disableAllIndicators();
        if (phase == 3)
            thirdAttack.disableAllIndicators();
        EnemyAttackIndicatorController.Instance.ResetParticles();
        int stunChance = Random.Range(1, 5);
        if ((!enemyStats.stun) || (enemyStats.stun && stunChance > 1*playerHpMod*stunMax))
        {

            enemy.GetComponent<Animator>().SetTrigger("Bird");
            yield return new WaitForSeconds(0.3833f);
            playerAnim.SetTrigger("Get Hurt");
            yield return new WaitForSeconds(0.6667f);
            bool isDead = playerStats.TakeDamage((int)(enemyAttackModifier * enemyStats.damage));
            transitionMusic();
            playerHUD.SetHP(playerStats);

            if (playerStats.reflect)
            {
                if (enemyStats.TakeDamage((int)(enemyAttackModifier * enemyStats.damage * (0.25f * playerHpMod) * reflectMax)))
                {
                    enemyStats.currentHP = 1;
                    transitionMusic();
                }
                if ((enemyStats.currentHP <= enemyStats.maxHP * 0.5f) && phase == 1)
                    StartCoroutine(Phase1MusicQuickStop());
                enemyDamageText2.gameObject.SetActive(false);
                enemyDamageText2.text = "-" + (int)(enemyAttackModifier * enemyStats.damage * (0.25f * playerHpMod) * reflectMax);
                enemyDamageText2.gameObject.SetActive(true);
                enemyHUD.SetHP(enemyStats);
            }

            playerDamageText.gameObject.SetActive(false);
            playerDamageText.text = "-" + (int)(enemyStats.damage);
            playerDamageText.gameObject.SetActive(true);
            hitSound.Play();
            // Shaky Effect
            for (int i = 0; i < 10; i++)
            {
                player.transform.position += new Vector3(5f, 0, 0);
                yield return new WaitForSeconds(0.01f);
                player.transform.position -= new Vector3(5f, 0, 0);
                yield return new WaitForSeconds(0.01f);
            }
            // Can use Write Method here-------
            yield return new WaitForSeconds(0.6167f);
            enemyAttackModifier = 1;
            if (isDead)
            {
                state = BattleState.LOST;
                EndBattle();
            }
            else
            {
                state = BattleState.PLAYERTURN;

                // Enemy AI (THIS SHOULD DEFINITELY BE MADE INTO A FUNCTION!!!!!!!!!!!!)
                if (introAttacks.Contains(2))
				{
                    introAttacks.Remove(2);
                }
                if (buffLocation.passiveBuff == "AttackSet" || buffLocation.passiveBuff == "ControlSet")
                    maxAttack = (int)(6 * playerAttackModifier * playerHpMod * attackBuff);
                else
                    maxAttack = (int)(3 * playerAttackModifier * playerHpMod * attackBuff);


                if (attackAmount >= phase)
                {
                    PlayerTurn();
                    ChooseNextAttack(birdCounter, 2, EnemyAttackIndicatorController.Instance);
                    if (phase >= 2)
                    {
                        ChooseNextAttack(birdCounter, 2, secondAttack);
                    }
                    if (phase == 3)
                    {
                        ChooseNextAttack(birdCounter, 2, thirdAttack);
                    }

                    if (birdCounter >= 2)
                        birdCounter = 0;
                }
                else
                {
                    if (attackAmount == 1)
                    {
                        if (secondAttack.getEnabledIndicator() == 2)
                        {
                            StartCoroutine(BirdAttack());
                        }
                        else if (secondAttack.getEnabledIndicator() == 3)
                        {
                            StartCoroutine(CatAttack());
                        }
                        else if (secondAttack.getEnabledIndicator() == 0)
                        {
                            StartCoroutine(ShieldDefense());
                        }
                        else if (secondAttack.getEnabledIndicator() == 1)
                        {
                            StartCoroutine(ButterflyAttack());
                        }
                        else if (secondAttack.getEnabledIndicator() == 4)
                        {
                            StartCoroutine(RootsAttack());
                        }
                        else if (secondAttack.getEnabledIndicator() == 5)
                        {
                            StartCoroutine(PunchAttack());
                        }
                        else if (secondAttack.getEnabledIndicator() == 6)
                        {
                            StartCoroutine(ShieldAttack());
                        }
                    }
                    else if (attackAmount == 2)
                    {
                        if (thirdAttack.getEnabledIndicator() == 2)
                        {
                            StartCoroutine(BirdAttack());
                        }
                        else if (thirdAttack.getEnabledIndicator() == 3)
                        {
                            StartCoroutine(CatAttack());
                        }
                        else if (thirdAttack.getEnabledIndicator() == 0)
                        {
                            StartCoroutine(ShieldDefense());
                        }
                        else if (thirdAttack.getEnabledIndicator() == 1)
                        {
                            StartCoroutine(ButterflyAttack());
                        }
                        else if (thirdAttack.getEnabledIndicator() == 4)
                        {
                            StartCoroutine(RootsAttack());
                        }
                        else if (thirdAttack.getEnabledIndicator() == 5)
                        {
                            StartCoroutine(PunchAttack());
                        }
                        else if (thirdAttack.getEnabledIndicator() == 6)
                        {
                            StartCoroutine(ShieldAttack());
                        }
                    }
                }


                
            }
        }

        else
		{
            enemyDamageText2.gameObject.SetActive(false);
            enemyDamageText2.text = "<color=#C75700>STUN!";
            enemyDamageText2.gameObject.SetActive(true);
            yield return new WaitForSeconds(1);
            state = BattleState.PLAYERTURN;

            // Enemy AI
            if (introAttacks.Contains(2))
            {
                introAttacks.Remove(2);
            }
            if (buffLocation.passiveBuff == "AttackSet" || buffLocation.passiveBuff == "ControlSet")
                maxAttack = (int)(6 * playerAttackModifier * playerHpMod * attackBuff);
            else
                maxAttack = (int)(3 * playerAttackModifier * playerHpMod * attackBuff);

            if (attackAmount >= phase)
			{
                PlayerTurn();
                ChooseNextAttack(birdCounter, 2, EnemyAttackIndicatorController.Instance);
                if (phase >= 2)
                {
                    ChooseNextAttack(birdCounter, 2, secondAttack);
                }
                if (phase == 3)
                {
                    ChooseNextAttack(birdCounter, 2, thirdAttack);
                }

                if (birdCounter >= 2)
                    birdCounter = 0;
                
            }
            else
			{
                if (attackAmount == 1)
				{
                    if (secondAttack.getEnabledIndicator() == 2)
                    {
                        StartCoroutine(BirdAttack());
                    }
                    else if (secondAttack.getEnabledIndicator() == 3)
                    {
                        StartCoroutine(CatAttack());
                    }
                    else if (secondAttack.getEnabledIndicator() == 0)
                    {
                        StartCoroutine(ShieldDefense());
                    }
                    else if (secondAttack.getEnabledIndicator() == 1)
                    {
                        StartCoroutine(ButterflyAttack());
                    }
                    else if (secondAttack.getEnabledIndicator() == 4)
                    {
                        StartCoroutine(RootsAttack());
                    }
                    else if (secondAttack.getEnabledIndicator() == 5)
                    {
                        StartCoroutine(PunchAttack());
                    }
                    else if (secondAttack.getEnabledIndicator() == 6)
                    {
                        StartCoroutine(ShieldAttack());
                    }
                }
                else if (attackAmount == 2)
                {
                    if (thirdAttack.getEnabledIndicator() == 2)
                    {
                        StartCoroutine(BirdAttack());
                    }
                    else if (thirdAttack.getEnabledIndicator() == 3)
                    {
                        StartCoroutine(CatAttack());
                    }
                    else if (thirdAttack.getEnabledIndicator() == 0)
                    {
                        StartCoroutine(ShieldDefense());
                    }
                    else if (thirdAttack.getEnabledIndicator() == 1)
                    {
                        StartCoroutine(ButterflyAttack());
                    }
                    else if (thirdAttack.getEnabledIndicator() == 4)
                    {
                        StartCoroutine(RootsAttack());
                    }
                    else if (thirdAttack.getEnabledIndicator() == 5)
                    {
                        StartCoroutine(PunchAttack());
                    }
                    else if (thirdAttack.getEnabledIndicator() == 6)
                    {
                        StartCoroutine(ShieldAttack());
                    }
                }
            }
        }
    }

    void ChooseNextAttack(int counter, int attackType, EnemyAttackIndicatorController attackDisplay, int itemCooldown = 0)
	{
        if (puppetFight && phase == 1)
        {
            if (attackType >= 11)
                attackVariants = 11;
            else if (EnemyHasBuffs() || EnemyHasDebuffs())
            {
                if (EnemyHasBuffs())
                    attackVariants = 16;
                if (EnemyHasDebuffs())
                    attackVariants = 17;
            }
            else
                attackVariants = 15;
        }
        int startingAttack = 0;
        if (puppetFight && phase == 1)
            startingAttack = 7;
        maxShield = false;
        int enemyAttackType = Random.Range(startingAttack, attackVariants);
        if (CheckMaxAttack() && phase == 2 && attackAmount == 2 && !doneAttacks.Contains(0))
        {
            enemyAttackType = 0;
            maxShield = true;
        }
        else if (CheckMaxAttack() && phase == 3 && introAttacks.Count <= 0 && doneAttacks.Count <= 1)
		{
            enemyAttackType = 5;
		}
        else
        {
            if (CheckMaxAttack() && (!puppetFight || phase > 1))
            {
                enemyAttackType = Random.Range(1, attackVariants);
            }
            if ((counter >= 2) && enemyAttackType == attackType)
            {
                enemyAttackType = Random.Range(startingAttack, attackVariants);
                if (CheckMaxAttack() && (!puppetFight || phase > 1))
                {
                    enemyAttackType = Random.Range(1, attackVariants);
                }
                if (enemyAttackType == attackType)
                {
                    enemyAttackType = attackType - 1;
                    if ((CheckMaxAttack() && attackType - 1 == 0) || enemyAttackType < startingAttack)
                        enemyAttackType = attackVariants - 1;
                }
            }
            if (playerStats.stun && enemyAttackType == 1)
            {
                enemyAttackType = attackType - 1;
                if ((CheckMaxAttack() && attackType - 1 == 0) || enemyAttackType < 0)
                    enemyAttackType = attackVariants - 1;
            }

            if (((!introAttacks.Contains(enemyAttackType)) && introAttacks.Count > 0) && (!puppetFight || phase > 1))
            {
                if (enemyAttackType > introAttacks[0])
                {
                    while (!introAttacks.Contains(enemyAttackType))
                        enemyAttackType -= 1;
                }
                else
                {
                    enemyAttackType = introAttacks[0];
                }
            }

            if (enemyAttackType == 1 && doneAttacks.Contains(1))
            {
                enemyAttackType = Random.Range(0, attackVariants - 1);
                if (enemyAttackType >= 1)
                    enemyAttackType += 1;
                if (enemyAttackType == 5 && doneAttacks.Contains(5))
                {
                    enemyAttackType = Random.Range(0, 4);
                    if (enemyAttackType >= 1)
                        enemyAttackType += 1;
                }
            }
            else if (enemyAttackType == 5 && doneAttacks.Contains(5))
            {
                enemyAttackType = Random.Range(0, 5);
                if (enemyAttackType == 1 && doneAttacks.Contains(1))
                {
                    enemyAttackType = Random.Range(0, 4);
                    if (enemyAttackType >= 1)
                        enemyAttackType += 1;
                }
            }
        }

        Debug.Log("Max Shield is " + maxShield);
        if (introAttacks.Contains(enemyAttackType))
        {
            introAttacks.Remove(enemyAttackType);
        }
        int randomShieldChance = Random.Range(1, 3);
        if (EnemyHasDebuffs() && puppetFight && phase == 1)
		{
            if (Random.Range(0, 2) == 1)
			{
                enemyAttackType = 16;
			}
		}
        else if (attackVariants == 11 && attackType != 15 && ballRestoreAlt <= 0)
		{
            if (Random.Range(0, 3) == 1)
            {
                enemyAttackType = 15;
            }
        }
        if (enemyStats.currentHP == enemyStats.maxHP && enemyAttackType == 8)
            enemyAttackType = 9;
        if (enemyAttackType < 16)
            itemCooldown = PreviousItemCooldown(enemyAttackType + 1);
        else
            itemCooldown = 0;
        while (itemCooldown > 0)
		{
            itemCooldown = PreviousItemCooldown(enemyAttackType);
            enemyAttackType -= 1;
		}
        if (!EnemyHasBuffs() && enemyAttackType == 15)
            enemyAttackType = 16;
        if (enemyAttackType <= 6 && puppetFight && phase == 1)
            enemyAttackType = 10;
        if (enemyAttackType == 16 && puppetFight && phase == 1 && attackType == 16)
            enemyAttackType = 7;
        if (enemyStats.barrier >= maxEnemyShield/randomShieldChance && !maxShield && enemyAttackType == 0)
            attackDisplay.enableIndicator(6);
        else
            attackDisplay.enableIndicator(enemyAttackType);
        if (attackVariants == 11)
            attackVariants = 17;
        DoTheMario(enemyAttackType);
    }

    int PreviousItemCooldown(int currentItem)
	{
        if (currentItem == 9)
            return clothRestoreAlt;
        else if (currentItem == 12)
            return mirrorRestoreAlt;
        else if (currentItem == 13)
            return appleRestoreAlt;
        else if (currentItem == 14)
            return venomRestoreAlt;
        else if (currentItem == 15)
            return hairRestoreAlt;
        else
            return 0;
    }

    IEnumerator ButterflyAttack()
    {
        
        yield return new WaitWhile(() => talking);
        EnemyAttackIndicatorController.Instance.disableAllIndicators();
        if (phase >= 2)
            secondAttack.disableAllIndicators();
        if (phase == 3)
            thirdAttack.disableAllIndicators();
        EnemyAttackIndicatorController.Instance.ResetParticles();
        attackAmount += 1;
        int stunChance = Random.Range(1, 5);
        if ((!enemyStats.stun) || (enemyStats.stun && stunChance > 1 * playerHpMod*stunMax))
        {

            enemy.GetComponent<Animator>().SetTrigger("Butterfly");
            yield return new WaitForSeconds(2.06f);
            if (playerStats.TakeDamage((int)(enemyAttackModifier * 1)))
			{
                playerStats.currentHP = 1;
            }
            transitionMusic();
            playerHUD.SetHP(playerStats);

            if (playerStats.reflect)
            {
                if (enemyStats.TakeDamage((int)(enemyAttackModifier * 1 * (0.25f * playerHpMod)*reflectMax)))
                {
                    enemyStats.currentHP = 1;
                    transitionMusic();
                }
                if ((enemyStats.currentHP <= enemyStats.maxHP * 0.5f) && phase == 1)
                    StartCoroutine(Phase1MusicQuickStop());
                enemyDamageText2.gameObject.SetActive(false);
                enemyDamageText2.text = "-" + (int)(enemyAttackModifier * 1 * (0.25f * playerHpMod) * reflectMax);
                enemyDamageText2.gameObject.SetActive(true);
                enemyHUD.SetHP(enemyStats);
            }


            if (!immune)
            {
                playerStats.currentStunDuration = playerStats.stunDuration;
                playerStats.stun = true;
                UpdateEffectDisplay(playerStats.debuffDisplay, playerStats.debuffText, false, playerStats);
            }
            playerDamageText.gameObject.SetActive(false);
            playerDamageText.text = "-" + (int)(enemyAttackModifier*1);
            playerDamageText.gameObject.SetActive(true);
            hitSound.Play();
            // Shaky Effect
            // Can use Write Method here-------
            yield return new WaitForSeconds(0.28333333333f);
            enemyAttackModifier = 1;
            state = BattleState.PLAYERTURN;
            if (introAttacks.Contains(1))
            {
                introAttacks.Remove(1);
            }
            if (buffLocation.passiveBuff == "AttackSet" || buffLocation.passiveBuff == "ControlSet")
                maxAttack = (int)(6 * playerAttackModifier * playerHpMod * attackBuff);
            else
                maxAttack = (int)(3 * playerAttackModifier * playerHpMod * attackBuff);


            if (attackAmount >= phase)
            {
                PlayerTurn();
                ChooseNextAttack(0, 1, EnemyAttackIndicatorController.Instance);
                if (phase >= 2)
                {
                    ChooseNextAttack(0, 1, secondAttack);
                }
                if (phase == 3)
                {
                    ChooseNextAttack(0, 1, thirdAttack);
                }

                
            }
            else
            {
                if (attackAmount == 1)
                {
                    if (secondAttack.getEnabledIndicator() == 2)
                    {
                        StartCoroutine(BirdAttack());
                    }
                    else if (secondAttack.getEnabledIndicator() == 3)
                    {
                        StartCoroutine(CatAttack());
                    }
                    else if (secondAttack.getEnabledIndicator() == 0)
                    {
                        StartCoroutine(ShieldDefense());
                    }
                    else if (secondAttack.getEnabledIndicator() == 1)
                    {
                        StartCoroutine(ButterflyAttack());
                    }
                    else if (secondAttack.getEnabledIndicator() == 4)
                    {
                        StartCoroutine(RootsAttack());
                    }
                    else if (secondAttack.getEnabledIndicator() == 5)
                    {
                        StartCoroutine(PunchAttack());
                    }
                    else if (secondAttack.getEnabledIndicator() == 6)
                    {
                        StartCoroutine(ShieldAttack());
                    }
                }
                else if (attackAmount == 2)
                {
                    if (thirdAttack.getEnabledIndicator() == 2)
                    {
                        StartCoroutine(BirdAttack());
                    }
                    else if (thirdAttack.getEnabledIndicator() == 3)
                    {
                        StartCoroutine(CatAttack());
                    }
                    else if (thirdAttack.getEnabledIndicator() == 0)
                    {
                        StartCoroutine(ShieldDefense());
                    }
                    else if (thirdAttack.getEnabledIndicator() == 1)
                    {
                        StartCoroutine(ButterflyAttack());
                    }
                    else if (thirdAttack.getEnabledIndicator() == 4)
                    {
                        StartCoroutine(RootsAttack());
                    }
                    else if (thirdAttack.getEnabledIndicator() == 5)
                    {
                        StartCoroutine(PunchAttack());
                    }
                    else if (thirdAttack.getEnabledIndicator() == 6)
                    {
                        StartCoroutine(ShieldAttack());
                    }
                }
            }
        }
        else
        {
            enemyDamageText2.gameObject.SetActive(false);
            enemyDamageText2.text = "<color=#C75700>STUN!";
            enemyDamageText2.gameObject.SetActive(true);
            yield return new WaitForSeconds(1);
            state = BattleState.PLAYERTURN;
            if (introAttacks.Contains(1))
            {
                introAttacks.Remove(1);
            }
            if (buffLocation.passiveBuff == "AttackSet" || buffLocation.passiveBuff == "ControlSet")
                maxAttack = (int)(6 * playerAttackModifier * playerHpMod * attackBuff);
            else
                maxAttack = (int)(3 * playerAttackModifier * playerHpMod * attackBuff);


            if (attackAmount >= phase)
            {
                PlayerTurn();
                ChooseNextAttack(0, 1, EnemyAttackIndicatorController.Instance);
                if (phase >= 2)
                {
                    ChooseNextAttack(0, 1, secondAttack);
                }
                if (phase == 3)
                {
                    ChooseNextAttack(0, 1, thirdAttack);
                }

                
            }
            else
            {
                if (attackAmount == 1)
                {
                    if (secondAttack.getEnabledIndicator() == 2)
                    {
                        StartCoroutine(BirdAttack());
                    }
                    else if (secondAttack.getEnabledIndicator() == 3)
                    {
                        StartCoroutine(CatAttack());
                    }
                    else if (secondAttack.getEnabledIndicator() == 0)
                    {
                        StartCoroutine(ShieldDefense());
                    }
                    else if (secondAttack.getEnabledIndicator() == 1)
                    {
                        StartCoroutine(ButterflyAttack());
                    }
                    else if (secondAttack.getEnabledIndicator() == 4)
                    {
                        StartCoroutine(RootsAttack());
                    }
                    else if (secondAttack.getEnabledIndicator() == 5)
                    {
                        StartCoroutine(PunchAttack());
                    }
                    else if (secondAttack.getEnabledIndicator() == 6)
                    {
                        StartCoroutine(ShieldAttack());
                    }
                }
                else if (attackAmount == 2)
                {
                    if (thirdAttack.getEnabledIndicator() == 2)
                    {
                        StartCoroutine(BirdAttack());
                    }
                    else if (thirdAttack.getEnabledIndicator() == 3)
                    {
                        StartCoroutine(CatAttack());
                    }
                    else if (thirdAttack.getEnabledIndicator() == 0)
                    {
                        StartCoroutine(ShieldDefense());
                    }
                    else if (thirdAttack.getEnabledIndicator() == 1)
                    {
                        StartCoroutine(ButterflyAttack());
                    }
                    else if (thirdAttack.getEnabledIndicator() == 4)
                    {
                        StartCoroutine(RootsAttack());
                    }
                    else if (thirdAttack.getEnabledIndicator() == 5)
                    {
                        StartCoroutine(PunchAttack());
                    }
                    else if (thirdAttack.getEnabledIndicator() == 6)
                    {
                        StartCoroutine(ShieldAttack());
                    }
                }
            }
        }
    }

    void DoTheMario(int enemyAttackType)
	{
        string amountString = "One";
        if (doneAttacks.Contains(enemyAttackType))
		{
            amountString = "Two";
            doubleDoneAttacks.Add(enemyAttackType);
        }
        else
		{
            doneAttacks.Add(enemyAttackType);
        }
        if (doubleDoneAttacks.Contains(enemyAttackType))
		{
            amountString = "Three";
        }
        if (enemyAttackType == 0 || enemyAttackType == 8 || enemyAttackType == 9)
        {
            EnemyAttackIndicatorController.Instance.DeactivateParticles("Shield", "One");
            EnemyAttackIndicatorController.Instance.DeactivateParticles("Shield", "Two");
            EnemyAttackIndicatorController.Instance.DeactivateParticles("Shield", "Three");
            EnemyAttackIndicatorController.Instance.ActivateParticles("Shield", amountString);
        }
        else if (enemyAttackType == 1 || enemyAttackType >= 11)
        {
            EnemyAttackIndicatorController.Instance.ActivateParticles("Butterfly", amountString);
        }
        else if (enemyAttackType == 2 || enemyAttackType == 7)
        {
            EnemyAttackIndicatorController.Instance.DeactivateParticles("Bird", "One");
            EnemyAttackIndicatorController.Instance.DeactivateParticles("Bird", "Two");
            EnemyAttackIndicatorController.Instance.DeactivateParticles("Bird", "Three");
            EnemyAttackIndicatorController.Instance.ActivateParticles("Bird", amountString);
        }
        else if (enemyAttackType == 3 || enemyAttackType == 10)
        {
            EnemyAttackIndicatorController.Instance.DeactivateParticles("Cat", "One");
            EnemyAttackIndicatorController.Instance.DeactivateParticles("Cat", "Two");
            EnemyAttackIndicatorController.Instance.DeactivateParticles("Cat", "Three");
            EnemyAttackIndicatorController.Instance.ActivateParticles("Cat", amountString);
        }
        else if (enemyAttackType == 4)
        {
            EnemyAttackIndicatorController.Instance.DeactivateParticles("Roots", "One");
            EnemyAttackIndicatorController.Instance.DeactivateParticles("Roots", "Two");
            EnemyAttackIndicatorController.Instance.DeactivateParticles("Roots", "Three");
            EnemyAttackIndicatorController.Instance.ActivateParticles("Roots", amountString);
        }
        else if (enemyAttackType == 5)
        {
            EnemyAttackIndicatorController.Instance.DeactivateParticles("Punch", "One");
            EnemyAttackIndicatorController.Instance.DeactivateParticles("Punch", "Three");
            if (!(amountString == "Two"))
                EnemyAttackIndicatorController.Instance.ActivateParticles("Punch", amountString);
        }
    }

    IEnumerator CatAttack()
    {
        yield return new WaitWhile(() => talking);
        if (!doneAttacks.Contains(3))
            catCounter += 1;
        attackAmount += 1;
        EnemyAttackIndicatorController.Instance.disableAllIndicators();
        if (phase >= 2)
            secondAttack.disableAllIndicators();
        if (phase == 3)
            thirdAttack.disableAllIndicators();
        EnemyAttackIndicatorController.Instance.ResetParticles();
        int enemyDamage= Random.Range(0, (enemyStats.damage*2)+1);
        int stunChance = Random.Range(1, 5);
        if ((!enemyStats.stun) || (enemyStats.stun && stunChance > 1 * playerHpMod*stunMax))
        {
            enemy.GetComponent<Animator>().SetTrigger("Kitty");
            yield return new WaitForSeconds(0.7f);
            playerAnim.SetTrigger("Get Hurt");
            yield return new WaitForSeconds(0.6667f);
            bool isDead = playerStats.TakeDamage((int)(enemyAttackModifier * enemyDamage));
            transitionMusic();
            playerHUD.SetHP(playerStats);

            if (playerStats.reflect)
            {
                if (enemyStats.TakeDamage((int)(enemyAttackModifier * enemyDamage * (0.25f * playerHpMod)*reflectMax)))
                {
                    enemyStats.currentHP = 1;
                    transitionMusic();
                }
                if ((enemyStats.currentHP <= enemyStats.maxHP * 0.5f) && phase == 1)
                    StartCoroutine(Phase1MusicQuickStop());
                enemyDamageText2.gameObject.SetActive(false);
                enemyDamageText2.text = "-" + (int)(enemyAttackModifier * enemyDamage * (0.25f * playerHpMod) * reflectMax);
                enemyDamageText2.gameObject.SetActive(true);
                enemyHUD.SetHP(enemyStats);
            }

            playerDamageText.gameObject.SetActive(false);
            playerDamageText.text = "-" + (int)(enemyAttackModifier * enemyDamage);
            playerDamageText.gameObject.SetActive(true);
            hitSound.Play();
            // Shaky Effect
            for (int i = 0; i < 10; i++)
            {
                player.transform.position += new Vector3(5f, 0, 0);
                yield return new WaitForSeconds(0.01f);
                player.transform.position -= new Vector3(5f, 0, 0);
                yield return new WaitForSeconds(0.01f);
            }
            // Can use Write Method here-------
            yield return new WaitForSeconds(1.15f);
            enemyAttackModifier = 1;
            if (isDead)
            {
                state = BattleState.LOST;
                EndBattle();
            }
            else
            {
                state = BattleState.PLAYERTURN;
                if (introAttacks.Contains(3))
                {
                    introAttacks.Remove(3);
                }
                if (buffLocation.passiveBuff == "AttackSet" || buffLocation.passiveBuff == "ControlSet")
                    maxAttack = (int)(6 * playerAttackModifier * playerHpMod * attackBuff);
                else
                    maxAttack = (int)(3 * playerAttackModifier * playerHpMod * attackBuff);

                if (attackAmount >= phase)
                {
                    PlayerTurn();
                    ChooseNextAttack(catCounter, 3, EnemyAttackIndicatorController.Instance);
                    if (phase >= 2)
                    {
                        ChooseNextAttack(catCounter, 3, secondAttack);
                    }
                    if (phase == 3)
                    {
                        ChooseNextAttack(catCounter, 3, thirdAttack);
                    }

                    if (catCounter >= 2)
                        catCounter = 0;
                    
                }
                else
                {
                    if (attackAmount == 1)
                    {
                        if (secondAttack.getEnabledIndicator() == 2)
                        {
                            StartCoroutine(BirdAttack());
                        }
                        else if (secondAttack.getEnabledIndicator() == 3)
                        {
                            StartCoroutine(CatAttack());
                        }
                        else if (secondAttack.getEnabledIndicator() == 0)
                        {
                            StartCoroutine(ShieldDefense());
                        }
                        else if (secondAttack.getEnabledIndicator() == 1)
                        {
                            StartCoroutine(ButterflyAttack());
                        }
                        else if (secondAttack.getEnabledIndicator() == 4)
                        {
                            StartCoroutine(RootsAttack());
                        }
                        else if (secondAttack.getEnabledIndicator() == 5)
                        {
                            StartCoroutine(PunchAttack());
                        }
                        else if (secondAttack.getEnabledIndicator() == 6)
                        {
                            StartCoroutine(ShieldAttack());
                        }
                    }
                    else if (attackAmount == 2)
                    {
                        if (thirdAttack.getEnabledIndicator() == 2)
                        {
                            StartCoroutine(BirdAttack());
                        }
                        else if (thirdAttack.getEnabledIndicator() == 3)
                        {
                            StartCoroutine(CatAttack());
                        }
                        else if (thirdAttack.getEnabledIndicator() == 0)
                        {
                            StartCoroutine(ShieldDefense());
                        }
                        else if (thirdAttack.getEnabledIndicator() == 1)
                        {
                            StartCoroutine(ButterflyAttack());
                        }
                        else if (thirdAttack.getEnabledIndicator() == 4)
                        {
                            StartCoroutine(RootsAttack());
                        }
                        else if (thirdAttack.getEnabledIndicator() == 5)
                        {
                            StartCoroutine(PunchAttack());
                        }
                        else if (thirdAttack.getEnabledIndicator() == 6)
                        {
                            StartCoroutine(ShieldAttack());
                        }
                    }
                }
            }
        }
        else
        {
            enemyDamageText2.gameObject.SetActive(false);
            enemyDamageText2.text = "<color=#C75700>STUN!";
            enemyDamageText2.gameObject.SetActive(true);
            yield return new WaitForSeconds(1);
            state = BattleState.PLAYERTURN;
            if (introAttacks.Contains(3))
            {
                introAttacks.Remove(3);
            }
            if (buffLocation.passiveBuff == "AttackSet" || buffLocation.passiveBuff == "ControlSet")
                maxAttack = (int)(6 * playerAttackModifier * playerHpMod * attackBuff);
            else
                maxAttack = (int)(3 * playerAttackModifier * playerHpMod * attackBuff);

            if (attackAmount >= phase)
            {
                PlayerTurn();
                ChooseNextAttack(catCounter, 3, EnemyAttackIndicatorController.Instance);
                if (phase >= 2)
                {
                    ChooseNextAttack(catCounter, 3, secondAttack);
                }
                if (phase == 3)
                {
                    ChooseNextAttack(catCounter, 3, thirdAttack);
                }

                if (catCounter >= 2)
                    catCounter = 0;
                
            }
            else
            {
                if (attackAmount == 1)
                {
                    if (secondAttack.getEnabledIndicator() == 2)
                    {
                        StartCoroutine(BirdAttack());
                    }
                    else if (secondAttack.getEnabledIndicator() == 3)
                    {
                        StartCoroutine(CatAttack());
                    }
                    else if (secondAttack.getEnabledIndicator() == 0)
                    {
                        StartCoroutine(ShieldDefense());
                    }
                    else if (secondAttack.getEnabledIndicator() == 1)
                    {
                        StartCoroutine(ButterflyAttack());
                    }
                    else if (secondAttack.getEnabledIndicator() == 4)
                    {
                        StartCoroutine(RootsAttack());
                    }
                    else if (secondAttack.getEnabledIndicator() == 5)
                    {
                        StartCoroutine(PunchAttack());
                    }
                    else if (secondAttack.getEnabledIndicator() == 6)
                    {
                        StartCoroutine(ShieldAttack());
                    }
                }
                else if (attackAmount == 2)
                {
                    if (thirdAttack.getEnabledIndicator() == 2)
                    {
                        StartCoroutine(BirdAttack());
                    }
                    else if (thirdAttack.getEnabledIndicator() == 3)
                    {
                        StartCoroutine(CatAttack());
                    }
                    else if (thirdAttack.getEnabledIndicator() == 0)
                    {
                        StartCoroutine(ShieldDefense());
                    }
                    else if (thirdAttack.getEnabledIndicator() == 1)
                    {
                        StartCoroutine(ButterflyAttack());
                    }
                    else if (thirdAttack.getEnabledIndicator() == 4)
                    {
                        StartCoroutine(RootsAttack());
                    }
                    else if (thirdAttack.getEnabledIndicator() == 5)
                    {
                        StartCoroutine(PunchAttack());
                    }
                    else if (thirdAttack.getEnabledIndicator() == 6)
                    {
                        StartCoroutine(ShieldAttack());
                    }
                }
            }
        }
    }

    // This one isn't used in the Tech Demo so ignore it :D
    IEnumerator ShieldDefense()
    {
        yield return new WaitWhile(() => talking);
        if (!doneAttacks.Contains(0))
            shieldCounter += 1;
        attackAmount += 1;
        EnemyAttackIndicatorController.Instance.disableAllIndicators();
        if (phase >= 2)
            secondAttack.disableAllIndicators();
        if (phase == 3)
            thirdAttack.disableAllIndicators();
        EnemyAttackIndicatorController.Instance.ResetParticles();
        int stunChance = Random.Range(1, 5);
        if (maxShield)
		{
            stunChance = 5;
		}
        if ((!enemyStats.stun) || (enemyStats.stun && stunChance > 1 * playerHpMod*stunMax))
        {
            enemyAnim.SetTrigger("Shield Time");
            yield return new WaitForSeconds(1.5f);
            int barrier = (int)(Random.Range((int)(enemyStats.damage*0.6667f), enemyStats.damage + 1));
            if (maxShield)
                barrier = enemyStats.damage;
            if (tutorial && phase < 3){
                barrier = 10;
            }
            enemyStats.AddBarrier(barrier);
            enemyHUD.SetHP(enemyStats);
            // Can use Write Method here-------
        }
        else
		{
            enemyDamageText2.gameObject.SetActive(false);
            enemyDamageText2.text = "<color=#C75700>STUN!";
            enemyDamageText2.gameObject.SetActive(true);
        }
        yield return new WaitForSeconds(1);
        state = BattleState.PLAYERTURN;
        if (introAttacks.Contains(0))
        {
            introAttacks.Remove(0);
        }
        if (buffLocation.passiveBuff == "AttackSet" || buffLocation.passiveBuff == "ControlSet")
            maxAttack = (int)(6 * playerAttackModifier * playerHpMod * attackBuff);
        else
            maxAttack = (int)(3 * playerAttackModifier * playerHpMod * attackBuff);

        if (attackAmount >= phase)
        {
            PlayerTurn();
            maxShield = false;
            ChooseNextAttack(shieldCounter, 0, EnemyAttackIndicatorController.Instance);
            if (phase >= 2)
            {
                ChooseNextAttack(shieldCounter, 0, secondAttack);
            }
            if (phase == 3)
            {
                ChooseNextAttack(shieldCounter, 0, thirdAttack);
            }

            if (shieldCounter >= 2)
                shieldCounter = 0;
            
        }
        else
        {
            if (attackAmount == 1)
            {
                if (secondAttack.getEnabledIndicator() == 2)
                {
                    StartCoroutine(BirdAttack());
                }
                else if (secondAttack.getEnabledIndicator() == 3)
                {
                    StartCoroutine(CatAttack());
                }
                else if (secondAttack.getEnabledIndicator() == 0)
                {
                    StartCoroutine(ShieldDefense());
                }
                else if (secondAttack.getEnabledIndicator() == 1)
                {
                    StartCoroutine(ButterflyAttack());
                }
                else if (secondAttack.getEnabledIndicator() == 4)
                {
                    StartCoroutine(RootsAttack());
                }
                else if (secondAttack.getEnabledIndicator() == 5)
                {
                    StartCoroutine(PunchAttack());
                }
                else if (secondAttack.getEnabledIndicator() == 6)
                {
                    StartCoroutine(ShieldAttack());
                }
            }
            else if (attackAmount == 2)
            {
                if (thirdAttack.getEnabledIndicator() == 2)
                {
                    StartCoroutine(BirdAttack());
                }
                else if (thirdAttack.getEnabledIndicator() == 3)
                {
                    StartCoroutine(CatAttack());
                }
                else if (thirdAttack.getEnabledIndicator() == 0)
                {
                    StartCoroutine(ShieldDefense());
                }
                else if (thirdAttack.getEnabledIndicator() == 1)
                {
                    StartCoroutine(ButterflyAttack());
                }
                else if (thirdAttack.getEnabledIndicator() == 4)
                {
                    StartCoroutine(RootsAttack());
                }
                else if (thirdAttack.getEnabledIndicator() == 5)
                {
                    StartCoroutine(PunchAttack());
                }
                else if (thirdAttack.getEnabledIndicator() == 6)
                {
                    StartCoroutine(ShieldAttack());
                }
            }
        }
    }

    IEnumerator ShieldAttack()
    {
        yield return new WaitWhile(() => talking);
        if (!doneAttacks.Contains(3))
            shieldCounter += 1;
        attackAmount += 1;
        EnemyAttackIndicatorController.Instance.disableAllIndicators();
        if (phase >= 2)
            secondAttack.disableAllIndicators();
        if (phase == 3)
            thirdAttack.disableAllIndicators();
        EnemyAttackIndicatorController.Instance.ResetParticles();
        int enemyDamage = Random.Range((int)(enemyStats.damage * 0.6667f), enemyStats.damage + 1);
        int stunChance = Random.Range(1, 5);
        if ((!enemyStats.stun) || (enemyStats.stun && stunChance > 1 * playerHpMod * stunMax))
        {
            enemy.GetComponent<Animator>().SetTrigger("Throw Shield");
            yield return new WaitForSeconds(0.1666f);
            playerAnim.SetTrigger("Get Hurt");
            yield return new WaitForSeconds(0.6667f);
            bool isDead = false;
            if (playerStats.barrier > 0)
			{
                if ((int)(enemyAttackModifier * enemyDamage / 2) > playerStats.barrier)
				{
                    playerDamageText.gameObject.SetActive(false);
                    playerDamageText.text = "-" + playerStats.barrier;
                    playerDamageText.gameObject.SetActive(true);
                    playerStats.barrier = 0;
				}
                else
				{
                    isDead = playerStats.TakeDamage((int)(enemyAttackModifier * enemyDamage / 2));
                    playerDamageText.gameObject.SetActive(false);
                    playerDamageText.text = "-" + (int)(enemyAttackModifier * enemyDamage / 2);
                    playerDamageText.gameObject.SetActive(true);
                }
			}
            else
			{
                isDead = playerStats.TakeDamage((int)(enemyAttackModifier * enemyDamage));
                playerDamageText.gameObject.SetActive(false);
                playerDamageText.text = "-" + (int)(enemyAttackModifier * enemyDamage);
                playerDamageText.gameObject.SetActive(true);
            }

            if ((int)(enemyAttackModifier * enemyDamage / 2) > enemyStats.barrier)
            {
                enemyDamageText2.gameObject.SetActive(false);
                enemyDamageText2.text = "-" + enemyStats.barrier;
                enemyDamageText2.gameObject.SetActive(true);
                enemyStats.barrier = 0;
            }
            else
            {
                enemyStats.TakeDamage((int)(enemyAttackModifier * enemyDamage / 2));
                enemyDamageText2.gameObject.SetActive(false);
                enemyDamageText2.text = "-" + (int)(enemyAttackModifier * enemyDamage / 2);
                enemyDamageText2.gameObject.SetActive(true);
            }

            transitionMusic();
            playerHUD.SetHP(playerStats);
            enemyHUD.SetHP(enemyStats);

            if (playerStats.reflect)
            {
                if (enemyStats.TakeDamage((int)(enemyAttackModifier * enemyDamage * (0.25f * playerHpMod) * reflectMax)))
                {
                    enemyStats.currentHP = 1;
                    transitionMusic();
                }
                if ((enemyStats.currentHP <= enemyStats.maxHP * 0.5f) && phase == 1)
                    StartCoroutine(Phase1MusicQuickStop());
                enemyDamageText2.gameObject.SetActive(false);
                enemyDamageText2.text = "-" + (int)(enemyAttackModifier * enemyDamage * (0.25f * playerHpMod) * reflectMax);
                enemyDamageText2.gameObject.SetActive(true);
                enemyHUD.SetHP(enemyStats);
            }

            playerDamageText.gameObject.SetActive(false);
            playerDamageText.text = "-" + (int)(enemyAttackModifier * enemyDamage);
            playerDamageText.gameObject.SetActive(true);
            hitSound.Play();
            // Can use Write Method here-------
            yield return new WaitForSeconds(1.5f);
            enemyAttackModifier = 1;
            if (isDead)
            {
                state = BattleState.LOST;
                EndBattle();
            }
            else
            {
                state = BattleState.PLAYERTURN;
                if (introAttacks.Contains(3))
                {
                    introAttacks.Remove(3);
                }
                if (buffLocation.passiveBuff == "AttackSet" || buffLocation.passiveBuff == "ControlSet")
                    maxAttack = (int)(6 * playerAttackModifier * playerHpMod * attackBuff);
                else
                    maxAttack = (int)(3 * playerAttackModifier * playerHpMod * attackBuff);

                if (attackAmount >= phase)
                {
                    PlayerTurn();
                    ChooseNextAttack(shieldCounter, 0, EnemyAttackIndicatorController.Instance);
                    if (phase >= 2)
                    {
                        ChooseNextAttack(shieldCounter, 0, secondAttack);
                    }
                    if (phase == 3)
                    {
                        ChooseNextAttack(shieldCounter, 0, thirdAttack);
                    }

                    if (shieldCounter >= 2)
                        shieldCounter = 0;

                }
                else
                {
                    if (attackAmount == 1)
                    {
                        if (secondAttack.getEnabledIndicator() == 2)
                        {
                            StartCoroutine(BirdAttack());
                        }
                        else if (secondAttack.getEnabledIndicator() == 3)
                        {
                            StartCoroutine(CatAttack());
                        }
                        else if (secondAttack.getEnabledIndicator() == 0)
                        {
                            StartCoroutine(ShieldDefense());
                        }
                        else if (secondAttack.getEnabledIndicator() == 1)
                        {
                            StartCoroutine(ButterflyAttack());
                        }
                        else if (secondAttack.getEnabledIndicator() == 4)
                        {
                            StartCoroutine(RootsAttack());
                        }
                        else if (secondAttack.getEnabledIndicator() == 5)
                        {
                            StartCoroutine(PunchAttack());
                        }
                        else if (secondAttack.getEnabledIndicator() == 6)
                        {
                            StartCoroutine(ShieldAttack());
                        }
                    }
                    else if (attackAmount == 2)
                    {
                        if (thirdAttack.getEnabledIndicator() == 2)
                        {
                            StartCoroutine(BirdAttack());
                        }
                        else if (thirdAttack.getEnabledIndicator() == 3)
                        {
                            StartCoroutine(CatAttack());
                        }
                        else if (thirdAttack.getEnabledIndicator() == 0)
                        {
                            StartCoroutine(ShieldDefense());
                        }
                        else if (thirdAttack.getEnabledIndicator() == 1)
                        {
                            StartCoroutine(ButterflyAttack());
                        }
                        else if (thirdAttack.getEnabledIndicator() == 4)
                        {
                            StartCoroutine(RootsAttack());
                        }
                        else if (thirdAttack.getEnabledIndicator() == 5)
                        {
                            StartCoroutine(PunchAttack());
                        }
                        else if (thirdAttack.getEnabledIndicator() == 6)
                        {
                            StartCoroutine(ShieldAttack());
                        }
                    }
                }
            }
        }
        else
        {
            enemyDamageText2.gameObject.SetActive(false);
            enemyDamageText2.text = "<color=#C75700>STUN!";
            enemyDamageText2.gameObject.SetActive(true);
            yield return new WaitForSeconds(1);
            state = BattleState.PLAYERTURN;
            if (introAttacks.Contains(3))
            {
                introAttacks.Remove(3);
            }
            if (buffLocation.passiveBuff == "AttackSet" || buffLocation.passiveBuff == "ControlSet")
                maxAttack = (int)(6 * playerAttackModifier * playerHpMod * attackBuff);
            else
                maxAttack = (int)(3 * playerAttackModifier * playerHpMod * attackBuff);

            if (attackAmount >= phase)
            {
                PlayerTurn();
                ChooseNextAttack(shieldCounter, 0, EnemyAttackIndicatorController.Instance);
                if (phase >= 2)
                {
                    ChooseNextAttack(shieldCounter, 0, secondAttack);
                }
                if (phase == 3)
                {
                    ChooseNextAttack(shieldCounter, 0, thirdAttack);
                }

                if (shieldCounter >= 2)
                    shieldCounter = 0;

            }
            else
            {
                if (attackAmount == 1)
                {
                    if (secondAttack.getEnabledIndicator() == 2)
                    {
                        StartCoroutine(BirdAttack());
                    }
                    else if (secondAttack.getEnabledIndicator() == 3)
                    {
                        StartCoroutine(CatAttack());
                    }
                    else if (secondAttack.getEnabledIndicator() == 0)
                    {
                        StartCoroutine(ShieldDefense());
                    }
                    else if (secondAttack.getEnabledIndicator() == 1)
                    {
                        StartCoroutine(ButterflyAttack());
                    }
                    else if (secondAttack.getEnabledIndicator() == 4)
                    {
                        StartCoroutine(RootsAttack());
                    }
                    else if (secondAttack.getEnabledIndicator() == 5)
                    {
                        StartCoroutine(PunchAttack());
                    }
                    else if (secondAttack.getEnabledIndicator() == 6)
                    {
                        StartCoroutine(ShieldAttack());
                    }
                }
                else if (attackAmount == 2)
                {
                    if (thirdAttack.getEnabledIndicator() == 2)
                    {
                        StartCoroutine(BirdAttack());
                    }
                    else if (thirdAttack.getEnabledIndicator() == 3)
                    {
                        StartCoroutine(CatAttack());
                    }
                    else if (thirdAttack.getEnabledIndicator() == 0)
                    {
                        StartCoroutine(ShieldDefense());
                    }
                    else if (thirdAttack.getEnabledIndicator() == 1)
                    {
                        StartCoroutine(ButterflyAttack());
                    }
                    else if (thirdAttack.getEnabledIndicator() == 4)
                    {
                        StartCoroutine(RootsAttack());
                    }
                    else if (thirdAttack.getEnabledIndicator() == 5)
                    {
                        StartCoroutine(PunchAttack());
                    }
                    else if (thirdAttack.getEnabledIndicator() == 6)
                    {
                        StartCoroutine(ShieldAttack());
                    }
                }
            }
        }
    }

    IEnumerator RootsAttack()
    {

        yield return new WaitWhile(() => talking);
        if (!doneAttacks.Contains(4))
            rootsCounter += 1;
        attackAmount += 1;
        EnemyAttackIndicatorController.Instance.disableAllIndicators();
        if (phase >= 2)
            secondAttack.disableAllIndicators();
        if (phase == 3)
            thirdAttack.disableAllIndicators();
        EnemyAttackIndicatorController.Instance.ResetParticles();
        int stunChance = Random.Range(1, 5);
        if ((!enemyStats.stun) || (enemyStats.stun && stunChance > 1 * playerHpMod * stunMax))
        {

            enemy.GetComponent<Animator>().SetTrigger("RootsMagic");
            yield return new WaitForSeconds(0.25f);
            playerAnim.SetTrigger("Get Hurt");
            yield return new WaitForSeconds(0.6667f);
            bool isDead = false;
            playerStats.currentHP -= (int)((int)(enemyStats.damage * 0.6667f)* enemyAttackModifier);
            if (playerStats.currentHP <= 0)
			{
                playerStats.currentHP = 0;
                isDead = true;
			}
            transitionMusic();
            playerHUD.SetHP(playerStats);

            if (playerStats.reflect)
            {
                if (enemyStats.TakeDamage((int)(enemyAttackModifier * (int)(enemyStats.damage * 0.6667f) * (0.25f * playerHpMod) * reflectMax)))
                {
                    enemyStats.currentHP = 1;
                    transitionMusic();
                }
                if ((enemyStats.currentHP <= enemyStats.maxHP * 0.5f) && phase == 1)
                    StartCoroutine(Phase1MusicQuickStop());
                enemyDamageText2.gameObject.SetActive(false);
                enemyDamageText2.text = "-" + (int)(enemyAttackModifier * (int)(enemyStats.damage * 0.6667f) * (0.25f * playerHpMod) * reflectMax);
                enemyDamageText2.gameObject.SetActive(true);
                enemyHUD.SetHP(enemyStats);
            }

            playerDamageText.gameObject.SetActive(false);
            playerDamageText.text = "-" + (int)((int)(enemyStats.damage * 0.6667f) * enemyAttackModifier);
            playerDamageText.gameObject.SetActive(true);
            hitSound.Play();
            // Shaky Effect
            for (int i = 0; i < 10; i++)
            {
                player.transform.position += new Vector3(5f, 0, 0);
                yield return new WaitForSeconds(0.01f);
                player.transform.position -= new Vector3(5f, 0, 0);
                yield return new WaitForSeconds(0.01f);
            }
            // Can use Write Method here-------
            yield return new WaitForSeconds(0.6167f);
            enemyAttackModifier = 1;
            if (isDead)
            {
                state = BattleState.LOST;
                EndBattle();
            }
            else
            {
                state = BattleState.PLAYERTURN;

                // Enemy AI
                if (introAttacks.Contains(4))
                {
                    introAttacks.Remove(4);
                }

                if (buffLocation.passiveBuff == "AttackSet" || buffLocation.passiveBuff == "ControlSet")
                    maxAttack = (int)(6 * playerAttackModifier * playerHpMod * attackBuff);
                else
                    maxAttack = (int)(3 * playerAttackModifier * playerHpMod * attackBuff);

                if (attackAmount >= phase)
                {
                    PlayerTurn();
                    ChooseNextAttack(rootsCounter, 4, EnemyAttackIndicatorController.Instance);
                    if (phase >= 2)
                    {
                        ChooseNextAttack(rootsCounter, 4, secondAttack);
                    }
                    if (phase == 3)
                    {
                        ChooseNextAttack(rootsCounter, 4, thirdAttack);
                    }

                    if (rootsCounter >= 2)
                        rootsCounter = 0;
                    
                }
                else
                {
                    if (attackAmount == 1)
                    {
                        if (secondAttack.getEnabledIndicator() == 2)
                        {
                            StartCoroutine(BirdAttack());
                        }
                        else if (secondAttack.getEnabledIndicator() == 3)
                        {
                            StartCoroutine(CatAttack());
                        }
                        else if (secondAttack.getEnabledIndicator() == 0)
                        {
                            StartCoroutine(ShieldDefense());
                        }
                        else if (secondAttack.getEnabledIndicator() == 1)
                        {
                            StartCoroutine(ButterflyAttack());
                        }
                        else if (secondAttack.getEnabledIndicator() == 4)
                        {
                            StartCoroutine(RootsAttack());
                        }
                        else if (secondAttack.getEnabledIndicator() == 5)
                        {
                            StartCoroutine(PunchAttack());
                        }
                        else if (secondAttack.getEnabledIndicator() == 6)
                        {
                            StartCoroutine(ShieldAttack());
                        }
                    }
                    else if (attackAmount == 2)
                    {
                        if (thirdAttack.getEnabledIndicator() == 2)
                        {
                            StartCoroutine(BirdAttack());
                        }
                        else if (thirdAttack.getEnabledIndicator() == 3)
                        {
                            StartCoroutine(CatAttack());
                        }
                        else if (thirdAttack.getEnabledIndicator() == 0)
                        {
                            StartCoroutine(ShieldDefense());
                        }
                        else if (thirdAttack.getEnabledIndicator() == 1)
                        {
                            StartCoroutine(ButterflyAttack());
                        }
                        else if (thirdAttack.getEnabledIndicator() == 4)
                        {
                            StartCoroutine(RootsAttack());
                        }
                        else if (thirdAttack.getEnabledIndicator() == 5)
                        {
                            StartCoroutine(PunchAttack());
                        }
                        else if (thirdAttack.getEnabledIndicator() == 6)
                        {
                            StartCoroutine(ShieldAttack());
                        }
                    }
                }
            }
        }

        else
        {
            enemyDamageText2.gameObject.SetActive(false);
            enemyDamageText2.text = "<color=#C75700>STUN!";
            enemyDamageText2.gameObject.SetActive(true);
            yield return new WaitForSeconds(1);
            state = BattleState.PLAYERTURN;

            // Enemy AI
            if (introAttacks.Contains(4))
            {
                introAttacks.Remove(4);
            }
            if (buffLocation.passiveBuff == "AttackSet" || buffLocation.passiveBuff == "ControlSet")
                maxAttack = (int)(6 * playerAttackModifier * playerHpMod * attackBuff);
            else
                maxAttack = (int)(3 * playerAttackModifier * playerHpMod * attackBuff);

            if (attackAmount >= phase)
            {
                PlayerTurn();
                ChooseNextAttack(rootsCounter, 4, EnemyAttackIndicatorController.Instance);
                if (phase >= 2)
                {
                    ChooseNextAttack(rootsCounter, 4, secondAttack);
                }
                if (phase == 3)
                {
                    ChooseNextAttack(rootsCounter, 4, thirdAttack);
                }

                if (rootsCounter >= 2)
                    rootsCounter = 0;
                
            }
            else
            {
                if (attackAmount == 1)
                {
                    if (secondAttack.getEnabledIndicator() == 2)
                    {
                        StartCoroutine(BirdAttack());
                    }
                    else if (secondAttack.getEnabledIndicator() == 3)
                    {
                        StartCoroutine(CatAttack());
                    }
                    else if (secondAttack.getEnabledIndicator() == 0)
                    {
                        StartCoroutine(ShieldDefense());
                    }
                    else if (secondAttack.getEnabledIndicator() == 1)
                    {
                        StartCoroutine(ButterflyAttack());
                    }
                    else if (secondAttack.getEnabledIndicator() == 4)
                    {
                        StartCoroutine(RootsAttack());
                    }
                    else if (secondAttack.getEnabledIndicator() == 5)
                    {
                        StartCoroutine(PunchAttack());
                    }
                    else if (secondAttack.getEnabledIndicator() == 6)
                    {
                        StartCoroutine(ShieldAttack());
                    }
                }
                else if (attackAmount == 2)
                {
                    if (thirdAttack.getEnabledIndicator() == 2)
                    {
                        StartCoroutine(BirdAttack());
                    }
                    else if (thirdAttack.getEnabledIndicator() == 3)
                    {
                        StartCoroutine(CatAttack());
                    }
                    else if (thirdAttack.getEnabledIndicator() == 0)
                    {
                        StartCoroutine(ShieldDefense());
                    }
                    else if (thirdAttack.getEnabledIndicator() == 1)
                    {
                        StartCoroutine(ButterflyAttack());
                    }
                    else if (thirdAttack.getEnabledIndicator() == 4)
                    {
                        StartCoroutine(RootsAttack());
                    }
                    else if (thirdAttack.getEnabledIndicator() == 5)
                    {
                        StartCoroutine(PunchAttack());
                    }
                    else if (thirdAttack.getEnabledIndicator() == 6)
                    {
                        StartCoroutine(ShieldAttack());
                    }
                }
            }
        }
    }

    IEnumerator PunchAttack()
    {
        yield return new WaitWhile(() => talking);
        if (!doneAttacks.Contains(5))
            punchCounter += 1;
        attackAmount += 1;
        EnemyAttackIndicatorController.Instance.disableAllIndicators();
        if (phase >= 2)
            secondAttack.disableAllIndicators();
        if (phase == 3)
            thirdAttack.disableAllIndicators();
        EnemyAttackIndicatorController.Instance.ResetParticles();
        int missChance = Random.Range(0, 3);
        int stunChance = Random.Range(1, 5);
        if ((!enemyStats.stun) || (enemyStats.stun && stunChance > 1 * playerHpMod * stunMax))
        {
            playerAnim.SetTrigger("Get Hurt");
            yield return new WaitForSeconds(0.25f);
            enemy.GetComponent<Animator>().SetTrigger("omg punch");
            yield return new WaitForSeconds(0.4167f);
            bool isDead = false;
            if (missChance >= 1)
                isDead = playerStats.TakeDamage((int)(enemyAttackModifier * (enemyStats.damage * 2)));
            transitionMusic();
            playerHUD.SetHP(playerStats);

            if (playerStats.reflect && missChance == 0)
            {
                if (enemyStats.TakeDamage((int)(enemyAttackModifier * (enemyStats.damage * 2) * (0.25f * playerHpMod) * reflectMax)))
                {
                    enemyStats.currentHP = 1;
                    transitionMusic();
                }
                if ((enemyStats.currentHP <= enemyStats.maxHP * 0.5f) && phase == 1)
                    StartCoroutine(Phase1MusicQuickStop());
                enemyDamageText2.gameObject.SetActive(false);
                enemyDamageText2.text = "-" + (int)(enemyAttackModifier * (enemyStats.damage * 2) * (0.25f * playerHpMod) * reflectMax);
                enemyDamageText2.gameObject.SetActive(true);
                enemyHUD.SetHP(enemyStats);
            }

            if (missChance >= 1)
            {
                playerDamageText.gameObject.SetActive(false);
                playerDamageText.text = "-" + (int)(enemyAttackModifier * (enemyStats.damage * 2));
                playerDamageText.gameObject.SetActive(true);
                hitSound.Play();
                // Shaky Effect
                for (int i = 0; i < 10; i++)
                {
                    player.transform.position += new Vector3(5f, 0, 0);
                    yield return new WaitForSeconds(0.01f);
                    player.transform.position -= new Vector3(5f, 0, 0);
                    yield return new WaitForSeconds(0.01f);
                }
                // Can use Write Method here-------
            }
            yield return new WaitForSeconds(1.15f);
            enemyAttackModifier = 1;
            if (isDead)
            {
                state = BattleState.LOST;
                EndBattle();
            }
            else
            {
                state = BattleState.PLAYERTURN;
                if (introAttacks.Contains(5))
                {
                    introAttacks.Remove(5);
                }
                if (buffLocation.passiveBuff == "AttackSet" || buffLocation.passiveBuff == "ControlSet")
                    maxAttack = (int)(6 * playerAttackModifier * playerHpMod * attackBuff);
                else
                    maxAttack = (int)(3 * playerAttackModifier * playerHpMod * attackBuff);

                if (attackAmount >= phase)
                {
                    PlayerTurn();
                    ChooseNextAttack(punchCounter, 5, EnemyAttackIndicatorController.Instance);
                    if (phase >= 2)
                    {
                        ChooseNextAttack(punchCounter, 5, secondAttack);
                    }
                    if (phase == 3)
                    {
                        ChooseNextAttack(punchCounter, 5, thirdAttack);
                    }

                    if (punchCounter >= 2)
                        punchCounter = 0;
                }
                else
                {
                    if (attackAmount == 1)
                    {
                        if (secondAttack.getEnabledIndicator() == 2)
                        {
                            StartCoroutine(BirdAttack());
                        }
                        else if (secondAttack.getEnabledIndicator() == 3)
                        {
                            StartCoroutine(CatAttack());
                        }
                        else if (secondAttack.getEnabledIndicator() == 0)
                        {
                            StartCoroutine(ShieldDefense());
                        }
                        else if (secondAttack.getEnabledIndicator() == 1)
                        {
                            StartCoroutine(ButterflyAttack());
                        }
                        else if (secondAttack.getEnabledIndicator() == 4)
                        {
                            StartCoroutine(RootsAttack());
                        }
                        else if (secondAttack.getEnabledIndicator() == 5)
                        {
                            StartCoroutine(PunchAttack());
                        }
                        else if (secondAttack.getEnabledIndicator() == 6)
                        {
                            StartCoroutine(ShieldAttack());
                        }
                    }
                    else if (attackAmount == 2)
                    {
                        if (thirdAttack.getEnabledIndicator() == 2)
                        {
                            StartCoroutine(BirdAttack());
                        }
                        else if (thirdAttack.getEnabledIndicator() == 3)
                        {
                            StartCoroutine(CatAttack());
                        }
                        else if (thirdAttack.getEnabledIndicator() == 0)
                        {
                            StartCoroutine(ShieldDefense());
                        }
                        else if (thirdAttack.getEnabledIndicator() == 1)
                        {
                            StartCoroutine(ButterflyAttack());
                        }
                        else if (thirdAttack.getEnabledIndicator() == 4)
                        {
                            StartCoroutine(RootsAttack());
                        }
                        else if (thirdAttack.getEnabledIndicator() == 5)
                        {
                            StartCoroutine(PunchAttack());
                        }
                        else if (thirdAttack.getEnabledIndicator() == 6)
                        {
                            StartCoroutine(ShieldAttack());
                        }
                    }
                }
            }
        }
        else
        {
            enemyDamageText2.gameObject.SetActive(false);
            enemyDamageText2.text = "<color=#C75700>STUN!";
            enemyDamageText2.gameObject.SetActive(true);
            yield return new WaitForSeconds(1);
            state = BattleState.PLAYERTURN;
            if (introAttacks.Contains(5))
            {
                introAttacks.Remove(5);
            }
            if (buffLocation.passiveBuff == "AttackSet" || buffLocation.passiveBuff == "ControlSet")
                maxAttack = (int)(6 * playerAttackModifier * playerHpMod * attackBuff);
            else
                maxAttack = (int)(3 * playerAttackModifier * playerHpMod * attackBuff);

            if (attackAmount >= phase)
            {
                PlayerTurn();
                ChooseNextAttack(punchCounter, 5, EnemyAttackIndicatorController.Instance);
                if (phase >= 2)
                {
                    ChooseNextAttack(punchCounter, 5, secondAttack);
                }
                if (phase == 3)
                {
                    ChooseNextAttack(punchCounter, 5, thirdAttack);
                }

                if (punchCounter >= 2)
                    punchCounter = 0;
            }
            else
            {
                if (attackAmount == 1)
                {
                    if (secondAttack.getEnabledIndicator() == 2)
                    {
                        StartCoroutine(BirdAttack());
                    }
                    else if (secondAttack.getEnabledIndicator() == 3)
                    {
                        StartCoroutine(CatAttack());
                    }
                    else if (secondAttack.getEnabledIndicator() == 0)
                    {
                        StartCoroutine(ShieldDefense());
                    }
                    else if (secondAttack.getEnabledIndicator() == 1)
                    {
                        StartCoroutine(ButterflyAttack());
                    }
                    else if (secondAttack.getEnabledIndicator() == 4)
                    {
                        StartCoroutine(RootsAttack());
                    }
                    else if (secondAttack.getEnabledIndicator() == 5)
                    {
                        StartCoroutine(PunchAttack());
                    }
                    else if (secondAttack.getEnabledIndicator() == 6)
                    {
                        StartCoroutine(ShieldAttack());
                    }
                }
                else if (attackAmount == 2)
                {
                    if (thirdAttack.getEnabledIndicator() == 2)
                    {
                        StartCoroutine(BirdAttack());
                    }
                    else if (thirdAttack.getEnabledIndicator() == 3)
                    {
                        StartCoroutine(CatAttack());
                    }
                    else if (thirdAttack.getEnabledIndicator() == 0)
                    {
                        StartCoroutine(ShieldDefense());
                    }
                    else if (thirdAttack.getEnabledIndicator() == 1)
                    {
                        StartCoroutine(ButterflyAttack());
                    }
                    else if (thirdAttack.getEnabledIndicator() == 4)
                    {
                        StartCoroutine(RootsAttack());
                    }
                    else if (thirdAttack.getEnabledIndicator() == 5)
                    {
                        StartCoroutine(PunchAttack());
                    }
                    else if (thirdAttack.getEnabledIndicator() == 6)
                    {
                        StartCoroutine(ShieldAttack());
                    }
                }
            }
        }
    }

    void ExtraAttacks(EnemyAttackIndicatorController attack)
	{
        if (attack.getEnabledIndicator() == 2)
        {
            StartCoroutine(BirdAttack());
        }
        else if (attack.getEnabledIndicator() == 3)
        {
            StartCoroutine(CatAttack());
        }
        else if (attack.getEnabledIndicator() == 0)
        {
            StartCoroutine(ShieldDefense());
        }
        else if (attack.getEnabledIndicator() == 1)
        {
            StartCoroutine(ButterflyAttack());
        }
        else if (attack.getEnabledIndicator() == 4)
        {
            StartCoroutine(RootsAttack());
        }
        else if (attack.getEnabledIndicator() == 5)
        {
            StartCoroutine(PunchAttack());
        }
        else if (attack.getEnabledIndicator() == 6)
        {
            StartCoroutine(ShieldAttack());
        }
    }

    // Puppet Item Variants --------------------------------------------------------
    IEnumerator CudgelAttack()
    {
        yield return new WaitWhile(() => talking);
        if (!doneAttacks.Contains(7))
            birdCounter += 1;
        birdCounter += 1;
        attackAmount += 1;
        EnemyAttackIndicatorController.Instance.disableAllIndicators();
        if (phase >= 2)
            secondAttack.disableAllIndicators();
        if (phase == 3)
            thirdAttack.disableAllIndicators();
        EnemyAttackIndicatorController.Instance.ResetParticles();
        int stunChance = Random.Range(1, 5);
        if ((!enemyStats.stun) || (enemyStats.stun && stunChance > 1 * playerHpMod * stunMax))
        {

            enemy.GetComponent<Animator>().SetTrigger("PCudgel");
            yield return new WaitForSeconds(beatTime * 5.3f);
            playerAnim.SetTrigger("Get Hurt");
            yield return new WaitForSeconds(beatTime * 2.2f);
            bool isDead = false;
            if (enemyStats.vampirism)
            {
                if ((int)((1 * enemyAttackModifier * attackBuffAlt)) > playerStats.barrier)
                {
                    enemyStats.Heal(3);
                    enemyDamageText2.gameObject.SetActive(false);
                    enemyDamageText2.text = "<color=#FF3A3A>+" + (3);
                    enemyDamageText2.gameObject.SetActive(true);
                }
            }
            if (playerStats.TakeDamage((int)((4 * enemyAttackModifier * attackBuffAlt))))
            {
                isDead = true;
            }
            transitionMusic();
            playerHUD.SetHP(playerStats);

            if (playerStats.reflect)
            {
                if (enemyStats.TakeDamage((int)(enemyAttackModifier * 4 * (0.25f * playerHpMod) * reflectMax)))
                {
                    enemyStats.currentHP = 1;
                    transitionMusic();
                }
                if ((enemyStats.currentHP <= enemyStats.maxHP * 0.5f) && phase == 1)
                    StartCoroutine(Phase1MusicQuickStop());
                enemyDamageText2.gameObject.SetActive(false);
                enemyDamageText2.text = "-" + (int)(enemyAttackModifier * 4 * (0.25f * playerHpMod) * reflectMax);
                enemyDamageText2.gameObject.SetActive(true);
                enemyHUD.SetHP(enemyStats);
            }

            playerDamageText.gameObject.SetActive(false);
            playerDamageText.text = "-" + (int)(4);
            playerDamageText.gameObject.SetActive(true);
            hitSound.Play();
            // Can use Write Method here-------
            yield return new WaitForSeconds(2.2f);
            enemyAttackModifier = 1;
            if (isDead)
            {
                state = BattleState.LOST;
                EndBattle();
            }
            else
            {
                state = BattleState.PLAYERTURN;

                if (buffLocation.passiveBuff == "AttackSet" || buffLocation.passiveBuff == "ControlSet")
                    maxAttack = (int)(6 * playerAttackModifier * playerHpMod * attackBuff);
                else
                    maxAttack = (int)(3 * playerAttackModifier * playerHpMod * attackBuff);

                if (attackAmount >= phase)
                {

                    PlayerTurn();
                    ChooseNextAttack(birdCounter, 7, EnemyAttackIndicatorController.Instance);
                    if (phase >= 2)
                    {
                        ChooseNextAttack(birdCounter, 7, secondAttack);
                    }
                    if (phase == 3)
                    {
                        ChooseNextAttack(birdCounter, 7, thirdAttack);
                    }

                    if (birdCounter >= 2)
                        birdCounter = 0;
                }
                else
                {
                    if (attackAmount == 1)
                    {
                        ExtraAttacks(secondAttack);
                    }
                    else if (attackAmount == 2)
                    {
                        ExtraAttacks(thirdAttack);
                    }
                }



            }
        }

        else
        {
            enemyDamageText2.gameObject.SetActive(false);
            enemyDamageText2.text = "<color=#C75700>STUN!";
            enemyDamageText2.gameObject.SetActive(true);
            yield return new WaitForSeconds(1);
            state = BattleState.PLAYERTURN;

            if (buffLocation.passiveBuff == "AttackSet" || buffLocation.passiveBuff == "ControlSet")
                maxAttack = (int)(6 * playerAttackModifier * playerHpMod * attackBuff);
            else
                maxAttack = (int)(3 * playerAttackModifier * playerHpMod * attackBuff);

            if (attackAmount >= phase)
            {
                PlayerTurn();
                ChooseNextAttack(birdCounter, 7, EnemyAttackIndicatorController.Instance);
                if (phase >= 2)
                {
                    ChooseNextAttack(birdCounter, 7, secondAttack);
                }
                if (phase == 3)
                {
                    ChooseNextAttack(birdCounter, 7, thirdAttack);
                }

                if (birdCounter >= 2)
                    birdCounter = 0;

            }
            else
            {
                if (attackAmount == 1)
                {
                    ExtraAttacks(secondAttack);
                }
                else if (attackAmount == 2)
                {
                    ExtraAttacks(thirdAttack);
                }
            }
        }
    }

    IEnumerator ClothHeal()
    {
        yield return new WaitWhile(() => talking);
        if (!doneAttacks.Contains(8))
            shieldCounter += 1;
        attackAmount += 1;
        EnemyAttackIndicatorController.Instance.disableAllIndicators();
        if (phase >= 2)
            secondAttack.disableAllIndicators();
        if (phase == 3)
            thirdAttack.disableAllIndicators();
        EnemyAttackIndicatorController.Instance.ResetParticles();
        int stunChance = Random.Range(1, 5);
        if ((!enemyStats.stun) || (enemyStats.stun && stunChance > 1 * playerHpMod * stunMax))
        {
            enemyAnim.SetTrigger("Cloth");
            yield return new WaitForSeconds(beatTime * 5);
            if (enemyStats.currentHP == enemyStats.maxHP)
            {
            }
            else
            {
                enemyDamageText2.gameObject.SetActive(false);
                if ((enemyStats.currentHP + 6) <= enemyStats.maxHP)
                    enemyDamageText2.text = "<color=#FF3A3A>+" + (int)(6);
                else
                    enemyDamageText2.text = "<color=#FF3A3A>+" + (enemyStats.maxHP - enemyStats.currentHP);
                enemyDamageText2.gameObject.SetActive(true);
                enemyStats.Heal(6);
                transitionMusic();
                enemyHUD.SetHP(enemyStats);
            }
            yield return new WaitForSeconds(beatTime * 4.5f);
            clothRestoreAlt = clothCooldown;
            // Can use Write Method here-------
        }
        else
        {
            enemyDamageText2.gameObject.SetActive(false);
            enemyDamageText2.text = "<color=#C75700>STUN!";
            enemyDamageText2.gameObject.SetActive(true);
        }
        yield return new WaitForSeconds(1);
        state = BattleState.PLAYERTURN;
        if (buffLocation.passiveBuff == "AttackSet" || buffLocation.passiveBuff == "ControlSet")
            maxAttack = (int)(6 * playerAttackModifier * playerHpMod * attackBuff);
        else
            maxAttack = (int)(3 * playerAttackModifier * playerHpMod * attackBuff);

        if (attackAmount >= phase)
        {
            PlayerTurn();
            maxShield = false;
            ChooseNextAttack(shieldCounter, 8, EnemyAttackIndicatorController.Instance, itemCooldown: clothRestoreAlt);
            if (phase >= 2)
            {
                ChooseNextAttack(shieldCounter, 8, secondAttack, itemCooldown: clothRestoreAlt);
            }
            if (phase == 3)
            {
                ChooseNextAttack(shieldCounter, 8, thirdAttack, itemCooldown: clothRestoreAlt);
            }

            if (shieldCounter >= 2)
                shieldCounter = 0;

        }
        else
        {
            if (attackAmount == 1)
            {
                ExtraAttacks(secondAttack);
            }
            else if (attackAmount == 2)
            {
                ExtraAttacks(thirdAttack);
            }
        }
    }

    IEnumerator HandsDefense()
    {
        yield return new WaitWhile(() => talking);
        if (!doneAttacks.Contains(9))
            shieldCounter += 1;
        attackAmount += 1;
        EnemyAttackIndicatorController.Instance.disableAllIndicators();
        if (phase >= 2)
            secondAttack.disableAllIndicators();
        if (phase == 3)
            thirdAttack.disableAllIndicators();
        EnemyAttackIndicatorController.Instance.ResetParticles();
        int stunChance = Random.Range(1, 5);
        if ((!enemyStats.stun) || (enemyStats.stun && stunChance > 1 * playerHpMod * stunMax))
        {
            enemyAnim.SetTrigger("Hands");
            yield return new WaitForSeconds(beatTime * 6f);
            enemyDamageText2.gameObject.SetActive(false);
            enemyDamageText2.text = "<color=#3E7EFF>+" + 4;
            enemyDamageText2.gameObject.SetActive(true);
            enemyStats.AddBarrier(4);

            enemyHUD.SetHP(enemyStats);
            yield return new WaitForSeconds(beatTime * 3.5f);
            handsRestoreAlt = handsCooldown;
            // Can use Write Method here-------
        }
        else
        {
            enemyDamageText2.gameObject.SetActive(false);
            enemyDamageText2.text = "<color=#C75700>STUN!";
            enemyDamageText2.gameObject.SetActive(true);
        }
        yield return new WaitForSeconds(1);
        state = BattleState.PLAYERTURN;
        if (buffLocation.passiveBuff == "AttackSet" || buffLocation.passiveBuff == "ControlSet")
            maxAttack = (int)(6 * playerAttackModifier * playerHpMod * attackBuff);
        else
            maxAttack = (int)(3 * playerAttackModifier * playerHpMod * attackBuff);

        if (attackAmount >= phase)
        {
            PlayerTurn();
            maxShield = false;
            ChooseNextAttack(shieldCounter, 9, EnemyAttackIndicatorController.Instance, itemCooldown: handsRestoreAlt);
            if (phase >= 2)
            {
                ChooseNextAttack(shieldCounter, 9, secondAttack, itemCooldown: handsRestoreAlt);
            }
            if (phase == 3)
            {
                ChooseNextAttack(shieldCounter, 9, thirdAttack, itemCooldown: handsRestoreAlt);
            }

            if (shieldCounter >= 2)
                shieldCounter = 0;

        }
        else
        {
            if (attackAmount == 1)
            {
                ExtraAttacks(secondAttack);
            }
            else if (attackAmount == 2)
            {
                ExtraAttacks(thirdAttack);
            }
        }
    }

    IEnumerator HatAttack()
    {
        yield return new WaitWhile(() => talking);
        if (!doneAttacks.Contains(10))
            catCounter += 1;
        attackAmount += 1;
        EnemyAttackIndicatorController.Instance.disableAllIndicators();
        if (phase >= 2)
            secondAttack.disableAllIndicators();
        if (phase == 3)
            thirdAttack.disableAllIndicators();
        EnemyAttackIndicatorController.Instance.ResetParticles();
        int randInt = Random.Range(1, 7);
        int stunChance = Random.Range(1, 5);
        if ((!enemyStats.stun) || (enemyStats.stun && stunChance > 1 * playerHpMod * stunMax))
        {
            enemyAnim.SetTrigger("Hat " + randInt);
            yield return new WaitForSeconds(beatTime * 3.75f);
            enemyDamageText.text = "-" + (int)((1 * playerAttackModifier * playerHpMod) * attackBuff);
            bool isDead = false;
            playerAnim.SetTrigger("Get Hurt");
            for (int i = 0; i < randInt; i++)
            {
                if (enemyStats.vampirism)
                {
                    if ((int)((1 * enemyAttackModifier) * attackBuffAlt) > playerStats.barrier)
                    {
                        enemyStats.Heal(3);
                        enemyDamageText2.gameObject.SetActive(false);
                        enemyDamageText2.text = "<color=#FF3A3A>+" + (3);
                        enemyDamageText2.gameObject.SetActive(true);
                    }
                }
                if (playerStats.TakeDamage((int)((2 * enemyAttackModifier) * attackBuffAlt)))
                {
                    isDead = true;
                }
                if (playerStats.reflect)
                {
                    if (enemyStats.TakeDamage((int)(enemyAttackModifier * 2 * (0.25f * playerHpMod) * reflectMax)))
                    {
                        enemyStats.currentHP = 1;
                        transitionMusic();
                    }
                    if ((enemyStats.currentHP <= enemyStats.maxHP * 0.5f) && phase == 1)
                        StartCoroutine(Phase1MusicQuickStop());
                    enemyDamageText2.gameObject.SetActive(false);
                    enemyDamageText2.text = "-" + (int)(enemyAttackModifier * 2 * (0.25f * playerHpMod) * reflectMax);
                    enemyDamageText2.gameObject.SetActive(true);
                    enemyHUD.SetHP(enemyStats);
                }
                if (isDead)
                {
                    state = BattleState.LOST;
                    EndBattle();
                }
                playerHUD.SetHP(playerStats);
                enemyHUD.SetHP(enemyStats);
                if (i == randInt - 1)
                {
                    hitSound.Play();
                }
                yield return new WaitForSeconds(beatTime / 2);
            }
            transitionMusic();
            playerHUD.SetHP(playerStats);

            // Can use Write Method here-------
            enemyAttackModifier = 1;

            if (!isDead)
            {
                yield return new WaitForSeconds(beatTime + ((beatTime / 2) * (6 - randInt)));
                yield return new WaitForSeconds(1);
                state = BattleState.PLAYERTURN;
                if (buffLocation.passiveBuff == "AttackSet" || buffLocation.passiveBuff == "ControlSet")
                    maxAttack = (int)(6 * playerAttackModifier * playerHpMod * attackBuff);
                else
                    maxAttack = (int)(3 * playerAttackModifier * playerHpMod * attackBuff);

                if (attackAmount >= phase)
                {
                    PlayerTurn();
                    ChooseNextAttack(catCounter, 10, EnemyAttackIndicatorController.Instance);
                    if (phase >= 2)
                    {
                        ChooseNextAttack(catCounter, 10, secondAttack);
                    }
                    if (phase == 3)
                    {
                        ChooseNextAttack(catCounter, 10, thirdAttack);
                    }

                    if (catCounter >= 2)
                        catCounter = 0;

                }
                else
                {
                    if (attackAmount == 1)
                    {
                        ExtraAttacks(secondAttack);
                    }
                    else if (attackAmount == 2)
                    {
                        ExtraAttacks(thirdAttack);
                    }
                }
            }
        }
        else
        {
            enemyDamageText2.gameObject.SetActive(false);
            enemyDamageText2.text = "<color=#C75700>STUN!";
            enemyDamageText2.gameObject.SetActive(true);
            yield return new WaitForSeconds(1);
            state = BattleState.PLAYERTURN;
            if (buffLocation.passiveBuff == "AttackSet" || buffLocation.passiveBuff == "ControlSet")
                maxAttack = (int)(6 * playerAttackModifier * playerHpMod * attackBuff);
            else
                maxAttack = (int)(3 * playerAttackModifier * playerHpMod * attackBuff);

            if (attackAmount >= phase)
            {
                PlayerTurn();
                ChooseNextAttack(catCounter, 10, EnemyAttackIndicatorController.Instance);
                if (phase >= 2)
                {
                    ChooseNextAttack(catCounter, 10, secondAttack);
                }
                if (phase == 3)
                {
                    ChooseNextAttack(catCounter, 10, thirdAttack);
                }

                if (catCounter >= 2)
                    catCounter = 0;

            }
            else
            {
                if (attackAmount == 1)
                {
                    ExtraAttacks(secondAttack);
                }
                else if (attackAmount == 2)
                {
                    ExtraAttacks(thirdAttack);
                }
            }
        }
    }

    IEnumerator MirrorBuff()
    {

        yield return new WaitWhile(() => talking);
        EnemyAttackIndicatorController.Instance.disableAllIndicators();
        if (phase >= 2)
            secondAttack.disableAllIndicators();
        if (phase == 3)
            thirdAttack.disableAllIndicators();
        EnemyAttackIndicatorController.Instance.ResetParticles();
        attackAmount += 1;
        int stunChance = Random.Range(1, 5);
        if (enemyStats.reflect)
        {
            reflectMaxAlt = 2;
            maxTextAlt.gameObject.SetActive(false);
            maxTextAlt.gameObject.SetActive(true);
        }
        if ((!enemyStats.stun) || (enemyStats.stun && stunChance > 1 * playerHpMod * stunMax))
        {

            enemy.GetComponent<Animator>().SetTrigger("Mirror");
            yield return new WaitForSeconds(beatTime * 9.5f);
            enemyStats.currentReflectDuration = enemyStats.reflectDuration;
            enemyStats.reflect = true;
            transitionMusic();
            playerHUD.SetHP(playerStats);

            UpdateEffectDisplay(enemyStats.buffDisplay, enemyStats.buffText, true, enemyStats);
            // Shaky Effect
            // Can use Write Method here-------
            yield return new WaitForSeconds(0.28333333333f);
            mirrorRestoreAlt = mirrorCooldown;
            enemyAttackModifier = 1;
            state = BattleState.PLAYERTURN;
            if (buffLocation.passiveBuff == "AttackSet" || buffLocation.passiveBuff == "ControlSet")
                maxAttack = (int)(6 * playerAttackModifier * playerHpMod * attackBuff);
            else
                maxAttack = (int)(3 * playerAttackModifier * playerHpMod * attackBuff);


            if (attackAmount >= phase)
            {
                PlayerTurn();
                ChooseNextAttack(0, 11, EnemyAttackIndicatorController.Instance, itemCooldown: mirrorRestoreAlt);
                if (phase >= 2)
                {
                    ChooseNextAttack(0, 11, secondAttack, itemCooldown: mirrorRestoreAlt);
                }
                if (phase == 3)
                {
                    ChooseNextAttack(0, 11, thirdAttack, itemCooldown: mirrorRestoreAlt);
                }


            }
            else
            {
                if (attackAmount == 1)
                {
                    ExtraAttacks(secondAttack);
                }
                else if (attackAmount == 2)
                {
                    ExtraAttacks(thirdAttack);
                }
            }
        }
        else
        {
            enemyDamageText2.gameObject.SetActive(false);
            enemyDamageText2.text = "<color=#C75700>STUN!";
            enemyDamageText2.gameObject.SetActive(true);
            yield return new WaitForSeconds(1);
            state = BattleState.PLAYERTURN;
            if (buffLocation.passiveBuff == "AttackSet" || buffLocation.passiveBuff == "ControlSet")
                maxAttack = (int)(6 * playerAttackModifier * playerHpMod * attackBuff);
            else
                maxAttack = (int)(3 * playerAttackModifier * playerHpMod * attackBuff);


            if (attackAmount >= phase)
            {
                PlayerTurn();
                ChooseNextAttack(0, 11, EnemyAttackIndicatorController.Instance, itemCooldown: mirrorRestoreAlt);
                if (phase >= 2)
                {
                    ChooseNextAttack(0, 11, secondAttack, itemCooldown: mirrorRestoreAlt);
                }
                if (phase == 3)
                {
                    ChooseNextAttack(0, 11, thirdAttack, itemCooldown: mirrorRestoreAlt);
                }


            }
            else
            {
                if (attackAmount == 1)
                {
                    ExtraAttacks(secondAttack);
                }
                else if (attackAmount == 2)
                {
                    ExtraAttacks(thirdAttack);
                }
            }
        }
    }

    IEnumerator AppleBuff()
    {

        yield return new WaitWhile(() => talking);
        EnemyAttackIndicatorController.Instance.disableAllIndicators();
        if (phase >= 2)
            secondAttack.disableAllIndicators();
        if (phase == 3)
            thirdAttack.disableAllIndicators();
        EnemyAttackIndicatorController.Instance.ResetParticles();
        attackAmount += 1;
        int stunChance = Random.Range(1, 5);
        if (enemyStats.dmgBoost)
        {
            dmgBoostMaxAlt = 2;
            maxTextAlt.gameObject.SetActive(false);
            maxTextAlt.gameObject.SetActive(true);
        }
        if ((!enemyStats.stun) || (enemyStats.stun && stunChance > 1 * playerHpMod * stunMax))
        {

            enemy.GetComponent<Animator>().SetTrigger("Apple");
            yield return new WaitForSeconds(beatTime * 5);
            enemyStats.currentDmgBoostDuration = enemyStats.dmgBoostDuration;
            if (enemyStats.dmgBoost)
            {
                dmgBoostMax = 2;
                maxText.gameObject.SetActive(false);
                maxText.gameObject.SetActive(true);
            }
            enemyStats.dmgBoost = true;
            transitionMusic();
            enemyHUD.SetHP(enemyStats);

            UpdateEffectDisplay(enemyStats.buffDisplay, enemyStats.buffText, true, enemyStats);
            // Shaky Effect
            // Can use Write Method here-------
            yield return new WaitForSeconds(0.28333333333f);
            appleRestoreAlt = appleCooldown;
            enemyAttackModifier = 1;
            state = BattleState.PLAYERTURN;
            if (buffLocation.passiveBuff == "AttackSet" || buffLocation.passiveBuff == "ControlSet")
                maxAttack = (int)(6 * playerAttackModifier * playerHpMod * attackBuff);
            else
                maxAttack = (int)(3 * playerAttackModifier * playerHpMod * attackBuff);


            if (attackAmount >= phase)
            {
                PlayerTurn();
                ChooseNextAttack(0, 12, EnemyAttackIndicatorController.Instance, itemCooldown: appleRestoreAlt);
                if (phase >= 2)
                {
                    ChooseNextAttack(0, 12, secondAttack, itemCooldown: appleRestoreAlt);
                }
                if (phase == 3)
                {
                    ChooseNextAttack(0, 12, thirdAttack, itemCooldown: appleRestoreAlt);
                }


            }
            else
            {
                if (attackAmount == 1)
                {
                    ExtraAttacks(secondAttack);
                }
                else if (attackAmount == 2)
                {
                    ExtraAttacks(thirdAttack);
                }
            }
        }
        else
        {
            enemyDamageText2.gameObject.SetActive(false);
            enemyDamageText2.text = "<color=#C75700>STUN!";
            enemyDamageText2.gameObject.SetActive(true);
            yield return new WaitForSeconds(1);
            state = BattleState.PLAYERTURN;
            if (buffLocation.passiveBuff == "AttackSet" || buffLocation.passiveBuff == "ControlSet")
                maxAttack = (int)(6 * playerAttackModifier * playerHpMod * attackBuff);
            else
                maxAttack = (int)(3 * playerAttackModifier * playerHpMod * attackBuff);


            if (attackAmount >= phase)
            {
                PlayerTurn();
                ChooseNextAttack(0, 12, EnemyAttackIndicatorController.Instance, itemCooldown: appleRestoreAlt);
                if (phase >= 2)
                {
                    ChooseNextAttack(0, 12, secondAttack, itemCooldown: appleRestoreAlt);
                }
                if (phase == 3)
                {
                    ChooseNextAttack(0, 12, thirdAttack, itemCooldown: appleRestoreAlt);
                }


            }
            else
            {
                if (attackAmount == 1)
                {
                    ExtraAttacks(secondAttack);
                }
                else if (attackAmount == 2)
                {
                    ExtraAttacks(thirdAttack);
                }
            }
        }
    }

    IEnumerator VenomBuff()
    {

        yield return new WaitWhile(() => talking);
        EnemyAttackIndicatorController.Instance.disableAllIndicators();
        if (phase >= 2)
            secondAttack.disableAllIndicators();
        if (phase == 3)
            thirdAttack.disableAllIndicators();
        EnemyAttackIndicatorController.Instance.ResetParticles();
        attackAmount += 1;
        int stunChance = Random.Range(1, 5);
        if (enemyStats.vampirism)
        {
            vampirismMaxAlt = 2;
            maxTextAlt.gameObject.SetActive(false);
            maxTextAlt.gameObject.SetActive(true);
        }
        if ((!enemyStats.stun) || (enemyStats.stun && stunChance > 1 * playerHpMod * stunMax))
        {

            enemyAnim.SetTrigger("Venom");
            yield return new WaitForSeconds(beatTime * 6);
            if (enemyStats.TakeDamage(3))
                enemyStats.currentHP = 1;
            enemyStats.currentVampirismDuration = enemyStats.vampirismDuration;
            if (enemyStats.vampirism)
            {
                vampirismMaxAlt = 2;
                maxText.gameObject.SetActive(false);
                maxText.gameObject.SetActive(true);
            }
            enemyStats.vampirism = true;
            transitionMusic();
            enemyHUD.SetHP(enemyStats);

            UpdateEffectDisplay(enemyStats.buffDisplay, enemyStats.buffText, true, enemyStats);
            // Shaky Effect
            // Can use Write Method here-------
            yield return new WaitForSeconds(0.28333333333f);
            venomRestoreAlt = venomCooldown;
            enemyAttackModifier = 1;
            state = BattleState.PLAYERTURN;
            if (buffLocation.passiveBuff == "AttackSet" || buffLocation.passiveBuff == "ControlSet")
                maxAttack = (int)(6 * playerAttackModifier * playerHpMod * attackBuff);
            else
                maxAttack = (int)(3 * playerAttackModifier * playerHpMod * attackBuff);


            if (attackAmount >= phase)
            {
                PlayerTurn();
                ChooseNextAttack(0, 13, EnemyAttackIndicatorController.Instance, itemCooldown: venomRestoreAlt);
                if (phase >= 2)
                {
                    ChooseNextAttack(0, 13, secondAttack, itemCooldown: venomRestoreAlt);
                }
                if (phase == 3)
                {
                    ChooseNextAttack(0, 13, thirdAttack, itemCooldown: venomRestoreAlt);
                }


            }
            else
            {
                if (attackAmount == 1)
                {
                    ExtraAttacks(secondAttack);
                }
                else if (attackAmount == 2)
                {
                    ExtraAttacks(thirdAttack);
                }
            }
        }
        else
        {
            enemyDamageText2.gameObject.SetActive(false);
            enemyDamageText2.text = "<color=#C75700>STUN!";
            enemyDamageText2.gameObject.SetActive(true);
            yield return new WaitForSeconds(1);
            state = BattleState.PLAYERTURN;
            if (buffLocation.passiveBuff == "AttackSet" || buffLocation.passiveBuff == "ControlSet")
                maxAttack = (int)(6 * playerAttackModifier * playerHpMod * attackBuff);
            else
                maxAttack = (int)(3 * playerAttackModifier * playerHpMod * attackBuff);


            if (attackAmount >= phase)
            {
                PlayerTurn();
                ChooseNextAttack(0, 13, EnemyAttackIndicatorController.Instance, itemCooldown: venomRestoreAlt);
                if (phase >= 2)
                {
                    ChooseNextAttack(0, 13, secondAttack, itemCooldown: venomRestoreAlt);
                }
                if (phase == 3)
                {
                    ChooseNextAttack(0, 13, thirdAttack, itemCooldown: venomRestoreAlt);
                }


            }
            else
            {
                if (attackAmount == 1)
                {
                    ExtraAttacks(secondAttack);
                }
                else if (attackAmount == 2)
                {
                    ExtraAttacks(thirdAttack);
                }
            }
        }
    }

    IEnumerator HairAttack()
    {

        yield return new WaitWhile(() => talking);
        EnemyAttackIndicatorController.Instance.disableAllIndicators();
        if (phase >= 2)
            secondAttack.disableAllIndicators();
        if (phase == 3)
            thirdAttack.disableAllIndicators();
        EnemyAttackIndicatorController.Instance.ResetParticles();
        attackAmount += 1;
        int stunChance = Random.Range(1, 5);
        if (playerStats.bind)
        {
            bindMaxAlt = 2;
            maxTextAlt.gameObject.SetActive(false);
            maxTextAlt.gameObject.SetActive(true);
        }
        if ((!enemyStats.stun) || (enemyStats.stun && stunChance > 1 * playerHpMod * stunMax))
        {

            enemyAnim.SetTrigger("PHair");
            yield return new WaitForSeconds(beatTime * 6);
            playerStats.currentBindDuration = playerStats.bindDuration;
            if (playerStats.bind)
            {
                bindMaxAlt = 2;
                maxText.gameObject.SetActive(false);
                maxText.gameObject.SetActive(true);
            }
            playerStats.bind = true;
            transitionMusic();
            enemyHUD.SetHP(enemyStats);

            UpdateEffectDisplay(playerStats.debuffDisplay, playerStats.debuffText, false, playerStats);
            // Shaky Effect
            // Can use Write Method here-------
            yield return new WaitForSeconds(0.28333333333f);
            hairRestoreAlt = hairCooldown;
            enemyAttackModifier = 1;
            state = BattleState.PLAYERTURN;
            if (buffLocation.passiveBuff == "AttackSet" || buffLocation.passiveBuff == "ControlSet")
                maxAttack = (int)(6 * playerAttackModifier * playerHpMod * attackBuff);
            else
                maxAttack = (int)(3 * playerAttackModifier * playerHpMod * attackBuff);


            if (attackAmount >= phase)
            {
                PlayerTurn();
                ChooseNextAttack(0, 14, EnemyAttackIndicatorController.Instance, itemCooldown: hairRestoreAlt);
                if (phase >= 2)
                {
                    ChooseNextAttack(0, 14, secondAttack, itemCooldown: hairRestoreAlt);
                }
                if (phase == 3)
                {
                    ChooseNextAttack(0, 14, thirdAttack, itemCooldown: hairRestoreAlt);
                }


            }
            else
            {
                if (attackAmount == 1)
                {
                    ExtraAttacks(secondAttack);
                }
                else if (attackAmount == 2)
                {
                    ExtraAttacks(thirdAttack);
                }
            }
        }
        else
        {
            enemyDamageText2.gameObject.SetActive(false);
            enemyDamageText2.text = "<color=#C75700>STUN!";
            enemyDamageText2.gameObject.SetActive(true);
            yield return new WaitForSeconds(1);
            state = BattleState.PLAYERTURN;
            if (buffLocation.passiveBuff == "AttackSet" || buffLocation.passiveBuff == "ControlSet")
                maxAttack = (int)(6 * playerAttackModifier * playerHpMod * attackBuff);
            else
                maxAttack = (int)(3 * playerAttackModifier * playerHpMod * attackBuff);


            if (attackAmount >= phase)
            {
                PlayerTurn();
                ChooseNextAttack(0, 14, EnemyAttackIndicatorController.Instance, itemCooldown: hairRestoreAlt);
                if (phase >= 2)
                {
                    ChooseNextAttack(0, 14, secondAttack, itemCooldown: hairRestoreAlt);
                }
                if (phase == 3)
                {
                    ChooseNextAttack(0, 14, thirdAttack, itemCooldown: hairRestoreAlt);
                }


            }
            else
            {
                if (attackAmount == 1)
                {
                    ExtraAttacks(secondAttack);
                }
                else if (attackAmount == 2)
                {
                    ExtraAttacks(thirdAttack);
                }
            }
        }
    }

    IEnumerator BallBuff()
    {

        yield return new WaitWhile(() => talking);
        EnemyAttackIndicatorController.Instance.disableAllIndicators();
        if (phase >= 2)
            secondAttack.disableAllIndicators();
        if (phase == 3)
            thirdAttack.disableAllIndicators();
        EnemyAttackIndicatorController.Instance.ResetParticles();
        attackAmount += 1;
        int stunChance = Random.Range(1, 5);
        if ((!enemyStats.stun) || (enemyStats.stun && stunChance > 1 * playerHpMod * stunMax))
        {

            enemyAnim.SetTrigger("Ball");
            yield return new WaitForSeconds(beatTime * 6);

            if (enemyStats.currentReflectDuration > 0)
            {
                enemyStats.currentReflectDuration += 1;
            }
            if (enemyStats.currentDmgBoostDuration > 0)
            {
                enemyStats.currentDmgBoostDuration += 1;
            }
            if (enemyStats.currentVampirismDuration > 0)
            {
                enemyStats.currentVampirismDuration += 1;
            }

            transitionMusic();
            enemyHUD.SetHP(enemyStats);

            UpdateEffectDisplay(enemyStats.buffDisplay, enemyStats.buffText, true, enemyStats);
            // Shaky Effect
            // Can use Write Method here-------
            yield return new WaitForSeconds(0.28333333333f);
            enemyAttackModifier = 1;
            state = BattleState.PLAYERTURN;
            if (buffLocation.passiveBuff == "AttackSet" || buffLocation.passiveBuff == "ControlSet")
                maxAttack = (int)(6 * playerAttackModifier * playerHpMod * attackBuff);
            else
                maxAttack = (int)(3 * playerAttackModifier * playerHpMod * attackBuff);


            if (attackAmount >= phase)
            {
                PlayerTurn();
                ChooseNextAttack(0, 15, EnemyAttackIndicatorController.Instance);
                if (phase >= 2)
                {
                    ChooseNextAttack(0, 15, secondAttack);
                }
                if (phase == 3)
                {
                    ChooseNextAttack(0, 15, thirdAttack);
                }


            }
            else
            {
                if (attackAmount == 1)
                {
                    ExtraAttacks(secondAttack);
                }
                else if (attackAmount == 2)
                {
                    ExtraAttacks(thirdAttack);
                }
            }
        }
        else
        {
            enemyDamageText2.gameObject.SetActive(false);
            enemyDamageText2.text = "<color=#C75700>STUN!";
            enemyDamageText2.gameObject.SetActive(true);
            yield return new WaitForSeconds(1);
            state = BattleState.PLAYERTURN;
            if (buffLocation.passiveBuff == "AttackSet" || buffLocation.passiveBuff == "ControlSet")
                maxAttack = (int)(6 * playerAttackModifier * playerHpMod * attackBuff);
            else
                maxAttack = (int)(3 * playerAttackModifier * playerHpMod * attackBuff);


            if (attackAmount >= phase)
            {
                PlayerTurn();
                ChooseNextAttack(0, 15, EnemyAttackIndicatorController.Instance);
                if (phase >= 2)
                {
                    ChooseNextAttack(0, 15, secondAttack);
                }
                if (phase == 3)
                {
                    ChooseNextAttack(0, 15, thirdAttack);
                }


            }
            else
            {
                if (attackAmount == 1)
                {
                    ExtraAttacks(secondAttack);
                }
                else if (attackAmount == 2)
                {
                    ExtraAttacks(thirdAttack);
                }
            }
        }
    }

    IEnumerator ShoesAttack()
    {

        yield return new WaitWhile(() => talking);
        EnemyAttackIndicatorController.Instance.disableAllIndicators();
        if (phase >= 2)
            secondAttack.disableAllIndicators();
        if (phase == 3)
            thirdAttack.disableAllIndicators();
        EnemyAttackIndicatorController.Instance.ResetParticles();
        attackAmount += 1;
        int stunChance = Random.Range(1, 5);
        if (playerStats.stun && enemyStats.stun)
        {
            stunMaxAlt = 2;
            maxTextAlt.gameObject.SetActive(false);
            maxTextAlt.gameObject.SetActive(true);
        }
        if (playerStats.bind && enemyStats.bind)
        {
            bindMaxAlt = 2;
            maxTextAlt.gameObject.SetActive(false);
            maxTextAlt.gameObject.SetActive(true);
        }
        if ((!enemyStats.stun) || (enemyStats.stun && stunChance > 1 * playerHpMod * stunMax))
        {


            enemyAnim.SetTrigger("Shoes");
            yield return new WaitForSeconds(beatTime * 9.5f);
            if (enemyStats.stun)
            {
                playerStats.currentStunDuration = playerStats.stunDuration;
                playerStats.stun = true;
                UpdateEffectDisplay(playerStats.debuffDisplay, playerStats.debuffText, false, playerStats);
                // Need to get code for activating debuff intigators
                enemyStats.currentStunDuration = 0;
                enemyStats.stun = false;
                UpdateEffectDisplay(enemyStats.debuffDisplay, enemyStats.debuffText, false, enemyStats);
            }

            if (enemyStats.bind)
            {
                playerStats.currentBindDuration = playerStats.bindDuration;
                playerStats.bind = true;
                UpdateEffectDisplay(playerStats.debuffDisplay, playerStats.debuffText, false, playerStats);
                // Need to get code for activating debuff intigators
                enemyStats.currentBindDuration = 0;
                enemyStats.bind = false;
                UpdateEffectDisplay(enemyStats.debuffDisplay, enemyStats.debuffText, false, enemyStats);
            }
            transitionMusic();
            playerHUD.SetHP(playerStats);

            UpdateEffectDisplay(playerStats.debuffDisplay, playerStats.debuffText, false, playerStats);
            // Shaky Effect
            // Can use Write Method here-------
            yield return new WaitForSeconds(0.28333333333f);
            enemyAttackModifier = 1;
            state = BattleState.PLAYERTURN;
            if (buffLocation.passiveBuff == "AttackSet" || buffLocation.passiveBuff == "ControlSet")
                maxAttack = (int)(6 * playerAttackModifier * playerHpMod * attackBuff);
            else
                maxAttack = (int)(3 * playerAttackModifier * playerHpMod * attackBuff);


            if (attackAmount >= phase)
            {
                PlayerTurn();
                ChooseNextAttack(0, 16, EnemyAttackIndicatorController.Instance);
                if (phase >= 2)
                {
                    ChooseNextAttack(0, 16, secondAttack);
                }
                if (phase == 3)
                {
                    ChooseNextAttack(0, 16, thirdAttack);
                }


            }
            else
            {
                if (attackAmount == 1)
                {
                    ExtraAttacks(secondAttack);
                }
                else if (attackAmount == 2)
                {
                    ExtraAttacks(thirdAttack);
                }
            }
        }
        else
        {
            enemyDamageText2.gameObject.SetActive(false);
            enemyDamageText2.text = "<color=#C75700>STUN!";
            enemyDamageText2.gameObject.SetActive(true);
            yield return new WaitForSeconds(1);
            state = BattleState.PLAYERTURN;
            if (buffLocation.passiveBuff == "AttackSet" || buffLocation.passiveBuff == "ControlSet")
                maxAttack = (int)(6 * playerAttackModifier * playerHpMod * attackBuff);
            else
                maxAttack = (int)(3 * playerAttackModifier * playerHpMod * attackBuff);


            if (attackAmount >= phase)
            {
                PlayerTurn();
                ChooseNextAttack(0, 16, EnemyAttackIndicatorController.Instance);
                if (phase >= 2)
                {
                    ChooseNextAttack(0, 16, secondAttack);
                }
                if (phase == 3)
                {
                    ChooseNextAttack(0, 16, thirdAttack);
                }


            }
            else
            {
                if (attackAmount == 1)
                {
                    ExtraAttacks(secondAttack);
                }
                else if (attackAmount == 2)
                {
                    ExtraAttacks(thirdAttack);
                }
            }
        }
    }

    // Method for Items -----------------------------------------------------------------------------

    // Method for Glock Item
    IEnumerator UseGlock(){
        backgroundMusic.Pause();
        guncock.Play();
        yield return new WaitForSeconds(2f);
        gunshots.Play();
        yield return new WaitForSeconds(22f);
        backgroundMusic.Play();

        bool isDead = enemyStats.TakeDamage(9999999);
        transitionMusic();

        enemyHUD.SetHP(enemyStats);
        hitSound.Play();
        for ( int i = 0; i < 10; i++)
        {
            enemy.transform.position += new Vector3(5f, 0, 0);
            yield return new WaitForSeconds(0.01f);
            enemy.transform.position -= new Vector3(5f, 0, 0);
            yield return new WaitForSeconds(0.01f);
        }
        if(isDead){
            state = BattleState.WON;
            EndBattle();
        } else {
            state = BattleState.ENEMYTURN;
            EnemyTurn();
        }

    }

    IEnumerator VolumeItem(float timeStop, AudioSource music, bool skip = false)
	{
        if (false)
        {
            if (!skip)
                yield return new WaitForSeconds(beatTime * 3.5f);
            while (music.volume > startVolume * 0.8f)
            {
                music.volume -= startVolume * Time.deltaTime * 4 / timeStop / 1.25f;
                yield return null;
            }
            yield return new WaitForSeconds(timeStop * .5f);
            while (music.volume < startVolume)
            {
                music.volume += startVolume * Time.deltaTime * 4 / timeStop / 1.25f;
                yield return null;
            }
            music.volume = startVolume;
        }
    }

    int DisableItem(GameObject itemObject, int maxWait)
	{
        itemObject.GetComponent<Button>().interactable = false;
        Debug.Log(itemObject.GetComponent<Button>().interactable);
        itemObject.transform.Find("ItemIcon").gameObject.GetComponent<Image>().color = new Color(0.4f, 0.4f, 0.4f, 1.0f);
        itemObject.transform.Find("ItemIcon").gameObject.GetComponent<HoverForDescription>().disabled = true;
        itemObject.transform.Find("ItemName").gameObject.GetComponent<TextMeshProUGUI>().text = "Cooldown: "+maxWait;
        return maxWait;
    }

    void EnableItem(GameObject itemObject)
    {
        itemObject.GetComponent<Button>().interactable = true;
        itemObject.transform.Find("ItemIcon").gameObject.GetComponent<Image>().color = new Color(1.0f, 1.0f, 1.0f, 1.0f);
        itemObject.transform.Find("ItemIcon").gameObject.GetComponent<HoverForDescription>().disabled = false;
        itemObject.transform.Find("ItemName").gameObject.GetComponent<TextMeshProUGUI>().text = "";
    }

    // Method for Mirror Item - Needs Audio (my bad)
    IEnumerator UseMirror()
    {
        
        if (badMusicSync)
        {
            float waitTime = GetNextBeat(4);
            Debug.Log(waitTime);
            if (waitTime > (beatTime * 3.5f))
            {
                yield return new WaitForSeconds(waitTime - (beatTime * 3.5f));
            }
            else
            {
                yield return new WaitForSeconds(waitTime + (beatTime * 0.5f));
            }
        }
        int stunChance = Random.Range(1, 3);
        if ((!playerStats.stun) || (playerStats.stun && stunChance == 1))
        {
            playerAnim.SetTrigger("Mirror");
            yield return new WaitForSeconds(beatTime * 9.5f);
            playerStats.currentReflectDuration = playerStats.reflectDuration;
            if (playerStats.reflect)
			{
                reflectMax = 2;
                maxText.gameObject.SetActive(false);
                maxText.gameObject.SetActive(true);
            }
            playerStats.reflect = true;
            UpdateEffectDisplay(playerStats.buffDisplay, playerStats.buffText, true, playerStats);

            playerHUD.SetHP(playerStats);

            if (!noCooldowns)
                mirrorRestore = DisableItem(mirrorObject, mirrorCooldown);
        }
        else
		{
            playerDamageText.gameObject.SetActive(false);
            playerDamageText.text = "<color=#C75700>STUN!";
            playerDamageText.gameObject.SetActive(true);
            yield return new WaitForSeconds(1);
        }

        yield return new WaitForSeconds(1);
        state = BattleState.ENEMYTURN;
        EnemyTurn();

    }


    // Method for Cudgel Item - Needs Audio (my bad)
    IEnumerator UseCudgel()
    {
        if (badMusicSync)
        {
            float waitTime = GetNextBeat(4);
            Debug.Log(waitTime);
            if (waitTime > (beatTime * 3.5f))
            {
                yield return new WaitForSeconds(waitTime - (beatTime * 3.5f));
            }
            else
            {
                yield return new WaitForSeconds(waitTime + (beatTime * 0.5f));
            }
        }
        int stunChance = Random.Range(1, 3);
        if ((!playerStats.stun) || (playerStats.stun && stunChance == 1))
        {
            if (puppetFight)
                playerAnim.SetTrigger("PCudgel");
            else
                playerAnim.SetTrigger("Cudgel");
            bool isDead = false;
            if (enemyStats.TakeDamage((int)((3 * playerAttackModifier * playerHpMod) * attackBuff)))
            {
                if (phase == 3)
                    isDead = true;
                else
                {
                    enemyStats.currentHP = 1;
                }
            }
            if (isDead)
            {
                yield return new WaitForSeconds(beatTime * 3.3f);
                theDie.Play();
                yield return new WaitForSeconds(beatTime * 4);
                enemyHUD.SetHP(enemyStats);
                enemyAnim.SetTrigger("Die");
                state = BattleState.WON;
                EndBattle();
            }
            else
            {

                if (puppetFight)
                {
                    yield return new WaitForSeconds(beatTime * 5.3f);
                    enemyAnim.SetTrigger("Get Hurt");
                    yield return new WaitForSeconds(beatTime * 2.2f);
                }
                else
                {
                    yield return new WaitForSeconds(beatTime * 7.3f);
                    enemyAnim.SetTrigger("Owie");
                    yield return new WaitForSeconds(beatTime * 0.2f);
                }

                if (playerStats.vampirism)
                {
                    if ((int)((5 * playerAttackModifier * playerHpMod) * attackBuff) > enemyStats.barrier)
                    {
                        playerStats.Heal((int)(2 * playerHpMod * vampirismMax));
                        playerDamageText.gameObject.SetActive(false);
                        playerDamageText.text = "<color=#FF3A3A>+" + ((int)(2 * playerHpMod * vampirismMax));
                        playerDamageText.gameObject.SetActive(true);
                    }
                }

                if (enemyStats.reflect)
                {
                    if (playerStats.TakeDamage((int)(playerAttackModifier * playerHpMod * 3 * (0.37f) * reflectMaxAlt)))
                    {
                        playerStats.currentHP = 1;
                        transitionMusic();
                    }
                    playerDamageText.gameObject.SetActive(false);
                    playerDamageText.text = "-" + (int)(playerAttackModifier * playerHpMod * 3 * (0.37f) * reflectMaxAlt);
                    playerDamageText.gameObject.SetActive(true);
                    playerHUD.SetHP(playerStats);
                }

                enemyDamageText.text = "-" + (int)((3 * playerAttackModifier * playerHpMod) * attackBuff);
                transitionMusic();
                if ((enemyStats.currentHP <= enemyStats.maxHP * 0.5f) && phase == 1)
                    StartCoroutine(Phase1MusicQuickStop());

                playerHUD.SetHP(playerStats);
                enemyHUD.SetHP(enemyStats);
                hitSound.Play();
                // Shaky Effect!
                // for ( int i = 0; i < 10; i++)
                //{
                //enemy.transform.position += new Vector3(5f, 0, 0);
                //yield return new WaitForSeconds(0.01f);
                //enemy.transform.position -= new Vector3(5f, 0, 0);
                //yield return new WaitForSeconds(0.01f);
                //}
                enemy.GetComponent<Animator>().enabled = true;
                yield return new WaitForSeconds(2.2f);
                state = BattleState.ENEMYTURN;
                EnemyTurn();
            }
        }
        else
        {
            playerDamageText.gameObject.SetActive(false);
            playerDamageText.text = "<color=#C75700>STUN!";
            playerDamageText.gameObject.SetActive(true);
            yield return new WaitForSeconds(1);
            state = BattleState.ENEMYTURN;
            EnemyTurn();
        }
    }


    // Method for Magic Cloth Item - Needs Audio (my bad)
    IEnumerator UseMagicCloth()
    {
        if (badMusicSync)
        {
            float waitTime = GetNextBeat(4);
            Debug.Log(waitTime);
            if (waitTime > (beatTime))
            {
                yield return new WaitForSeconds(waitTime - (beatTime));
            }
            else
            {
                yield return new WaitForSeconds(waitTime + (beatTime * 3));
            }
        }
        int stunChance = Random.Range(1, 3);
        if ((!playerStats.stun) || (playerStats.stun && stunChance == 1))
        {
            playerAnim.SetTrigger("Cloth");
            yield return new WaitForSeconds(beatTime * 5);
            if (playerStats.currentHP == playerStats.maxHP)
            {
            }
            else
            {
                playerDamageText.gameObject.SetActive(false);
                if ((playerStats.currentHP + (int)(4 * playerHpMod)) <= playerStats.maxHP)
                    playerDamageText.text = "<color=#FF3A3A>+" + (int)(4 * playerHpMod);
                else
                    playerDamageText.text = "<color=#FF3A3A>+" + (playerStats.maxHP - playerStats.currentHP);
                playerDamageText.gameObject.SetActive(true);
                playerStats.Heal((int)(4 * playerHpMod));
                transitionMusic();
                playerHUD.SetHP(playerStats);
            }
            yield return new WaitForSeconds(beatTime * 4.5f);

            if (!noCooldowns)
                clothRestore = DisableItem(clothObject, clothCooldown);
        }
        else
        {
            playerDamageText.gameObject.SetActive(false);
            playerDamageText.text = "<color=#C75700>STUN!";
            playerDamageText.gameObject.SetActive(true);
            yield return new WaitForSeconds(1);
        }


        yield return new WaitForSeconds(1);
        state = BattleState.ENEMYTURN;
        EnemyTurn();
    }


    // Method for Silver Hands Item - Needs Audio (my bad)
    IEnumerator UseSilverHands()
    {
        if (badMusicSync)
        {
            float waitTime = GetNextBeat(4);
            Debug.Log(waitTime);
            if (waitTime > (beatTime * 1.5f))
            {
                yield return new WaitForSeconds(waitTime - (beatTime * 1.5f));
            }
            else
            {
                yield return new WaitForSeconds(waitTime + (beatTime * 2.5f));
            }
        }
        int stunChance = Random.Range(1, 3);
        if ((!playerStats.stun) || (playerStats.stun && stunChance == 1))
        {
            playerAnim.SetTrigger("Hands");
            yield return new WaitForSeconds(beatTime * 6f);
            playerDamageText.gameObject.SetActive(false);
            playerDamageText.text = "<color=#3E7EFF>+" + (int)(3 * playerHpMod);
            playerDamageText.gameObject.SetActive(true);
            playerStats.AddBarrier((int)(3 * playerHpMod));

            playerHUD.SetHP(playerStats);
            yield return new WaitForSeconds(beatTime * 3.5f);

            if (!noCooldowns)
                handsRestore = DisableItem(handsObject, handsCooldown);
        }
        else
        {
            playerDamageText.gameObject.SetActive(false);
            playerDamageText.text = "<color=#C75700>STUN!";
            playerDamageText.gameObject.SetActive(true);
            yield return new WaitForSeconds(1);
        }

        yield return new WaitForSeconds(1);
        state = BattleState.ENEMYTURN;
        EnemyTurn();
        yield return null;
    }

    // Method for Rapunzels Hair Item - Needs Audio (my bad)
    IEnumerator UseRapunzelHair()
    {
        if (badMusicSync)
        {
            float waitTime = GetNextBeat(4);
            Debug.Log(waitTime);
            if (waitTime > (beatTime * 2.5f))
            {
                yield return new WaitForSeconds(waitTime - (beatTime * 2.5f));
            }
            else
            {
                yield return new WaitForSeconds(waitTime + (beatTime * 1.5f));
            }
        }
        int stunChance = Random.Range(1, 3);
        if ((!playerStats.stun) || (playerStats.stun && stunChance == 1))
        {

            if (puppetFight)
                playerAnim.SetTrigger("PHair");
            else
                playerAnim.SetTrigger("Hair");
            yield return new WaitForSeconds(beatTime * 6);
            enemyStats.currentBindDuration = enemyStats.bindDuration;
            if (enemyStats.bind)
			{
                bindMax = 2;
                maxText.gameObject.SetActive(false);
                maxText.gameObject.SetActive(true);
            }
            enemyStats.bind = true;
            UpdateEffectDisplay(enemyStats.debuffDisplay, enemyStats.debuffText, false, enemyStats);

            enemyHUD.SetHP(enemyStats);

            yield return new WaitForSeconds(beatTime * 3.5f);

            if (!noCooldowns)
                hairRestore = DisableItem(hairObject, hairCooldown);
        }
        else
        {
            playerDamageText.gameObject.SetActive(false);
            playerDamageText.text = "<color=#C75700>STUN!";
            playerDamageText.gameObject.SetActive(true);
            yield return new WaitForSeconds(1);
        }

        yield return new WaitForSeconds(1);
        state = BattleState.ENEMYTURN;
        EnemyTurn();
    }

    // New Items =================================================================
    // Method for Iron Shoes Item
    IEnumerator UseIronShoes()
    {
        if (badMusicSync)
        {
            float waitTime = GetNextBeat(4);
            Debug.Log(waitTime);
            if (waitTime > (beatTime * 1.5f))
            {
                yield return new WaitForSeconds(waitTime - (beatTime * 1.5f));
            }
            else
            {
                yield return new WaitForSeconds(waitTime + (beatTime * 2.5f));
            }
        }
        playerAnim.SetTrigger("Shoes");
        yield return new WaitForSeconds(beatTime * 9.5f);
        if (playerStats.stun)
        {
            enemyStats.currentStunDuration = enemyStats.stunDuration;
            if (enemyStats.stun)
            {
                stunMax = 2;
                maxText.gameObject.SetActive(false);
                maxText.gameObject.SetActive(true);
            }
            enemyStats.stun = true;
            UpdateEffectDisplay(enemyStats.debuffDisplay, enemyStats.debuffText, false, enemyStats);
            // Need to get code for activating debuff intigators
            playerStats.currentStunDuration = 0;
            playerStats.stun = false;
            UpdateEffectDisplay(playerStats.debuffDisplay, playerStats.debuffText, false, playerStats);
        }

        if (playerStats.bind)
        {
            enemyStats.currentBindDuration = enemyStats.bindDuration;
            if (enemyStats.bind)
            {
                bindMax = 2;
                maxText.gameObject.SetActive(false);
                maxText.gameObject.SetActive(true);
            }
            enemyStats.bind = true;
            UpdateEffectDisplay(enemyStats.debuffDisplay, enemyStats.debuffText, false, enemyStats);
            // Need to get code for activating debuff intigators
            playerStats.currentBindDuration = 0;
            playerStats.bind = false;
            UpdateEffectDisplay(playerStats.debuffDisplay, playerStats.debuffText, false, playerStats);
        }
        else
        {


        }

        yield return new WaitForSeconds(1);
        state = BattleState.ENEMYTURN;
        EnemyTurn();
    }

    // Method for Hat that fires bullets Item
    IEnumerator UseHat()
    {

        int randInt = Random.Range(1, 7);
        if (badMusicSync)
        {
            float waitTime = GetNextBeat(4);
            Debug.Log(waitTime);
            if (waitTime > (beatTime * 1.5f))
            {
                yield return new WaitForSeconds(waitTime - (beatTime * 1.5f));
            }
            else
            {
                yield return new WaitForSeconds(waitTime + (beatTime * 2.5f));
            }
        }
        int stunChance = Random.Range(1, 3);
        if ((!playerStats.stun) || (playerStats.stun && stunChance == 1))
        {
            playerAnim.SetTrigger("Hat " + randInt);
            if (puppetFight)
                yield return new WaitForSeconds(beatTime * 3.75f);
            else
                yield return new WaitForSeconds(beatTime * 5.75f);
            enemyDamageText.text = "-" + (int)((1 * playerAttackModifier * playerHpMod) * attackBuff);
            bool isDead = false;
            if (puppetFight)
                enemyAnim.SetTrigger("Get Hurt");
            else
                enemyAnim.SetTrigger("Shot");
            for (int i = 0; i < randInt; i++)
            {
                if (playerStats.vampirism)
                {
                    if ((int)((1 * playerAttackModifier * playerHpMod) * attackBuff) > enemyStats.barrier)
                    {
                        playerStats.Heal(2 * (int)playerHpMod);
                        playerDamageText.gameObject.SetActive(false);
                        playerDamageText.text = "<color=#FF3A3A>+" + ((int)(2 * playerHpMod));
                        playerDamageText.gameObject.SetActive(true);
                    }
                }
                if (enemyStats.TakeDamage((int)((1 * playerAttackModifier * playerHpMod) * attackBuff)))
                {
                    if (phase == 3)
                        isDead = true;
                    else
                    {
                        enemyStats.currentHP = 1;
                    }
                }
                if (enemyStats.reflect)
                {
                    if (playerStats.TakeDamage((int)(playerAttackModifier * playerHpMod * 1 * (0.37f) * reflectMaxAlt)))
                    {
                        playerStats.currentHP = 1;
                        transitionMusic();
                    }
                    playerDamageText.gameObject.SetActive(false);
                    playerDamageText.text = "-" + (int)(playerAttackModifier * playerHpMod * 1 * (0.37f) * reflectMaxAlt);
                    playerDamageText.gameObject.SetActive(true);
                    playerHUD.SetHP(playerStats);
                }
                if (isDead)
                {
                    theDie.Play();
                    yield return new WaitForSeconds(beatTime * 4);
                    enemyHUD.SetHP(enemyStats);
                    enemyAnim.SetTrigger("Die");
                    state = BattleState.WON;
                    EndBattle();
                    break;
                }
                transitionMusic();
                if ((enemyStats.currentHP <= enemyStats.maxHP * 0.5f) && phase == 1)
                    StartCoroutine(Phase1MusicQuickStop());
                playerHUD.SetHP(playerStats);
                enemyHUD.SetHP(enemyStats);
                if (i == randInt - 1)
                {
                    hitSound.Play();
                }
                yield return new WaitForSeconds(beatTime / 2);
            }
            if (!isDead)
            {
                if (!puppetFight)
                    enemyAnim.SetTrigger("Unshot");
                yield return new WaitForSeconds(beatTime + ((beatTime / 2) * (6 - randInt)));
                yield return new WaitForSeconds(1);
                state = BattleState.ENEMYTURN;
                EnemyTurn();
            }
        }
        else
        {
            playerDamageText.gameObject.SetActive(false);
            playerDamageText.text = "<color=#C75700>STUN!";
            playerDamageText.gameObject.SetActive(true);
            yield return new WaitForSeconds(1);
            state = BattleState.ENEMYTURN;
            EnemyTurn();
        }
    }

    // Method for Crystal Ball Item
    IEnumerator UseCrystalBall()
    {
        if (badMusicSync)
        {
            float waitTime = GetNextBeat(4);
            Debug.Log(waitTime);
            if (waitTime > (beatTime * 3.5f))
            {
                yield return new WaitForSeconds(waitTime - (beatTime * 3.5f));
            }
            else
            {
                yield return new WaitForSeconds(waitTime + (beatTime * 0.5f));
            }
        }
        int stunChance = Random.Range(1, 3);
        if ((!playerStats.stun) || (playerStats.stun && stunChance == 1))
        {
            playerAnim.SetTrigger("Ball");
            yield return new WaitForSeconds(beatTime * 6);
            if (playerStats.currentReflectDuration > 0)
            {
                playerStats.currentReflectDuration += 1 * (int)playerHpMod;
            }
            if (playerStats.currentDmgBoostDuration > 0)
            {
                playerStats.currentDmgBoostDuration += 1 * (int)playerHpMod;
            }
            if (playerStats.currentVampirismDuration > 0)
            {
                playerStats.currentVampirismDuration += 1 * (int)playerHpMod;
            }

            if (!noCooldowns)
                ballRestore = DisableItem(ballObject, ballCooldown);
        }
        else
        {
            playerDamageText.gameObject.SetActive(false);
            playerDamageText.text = "<color=#C75700>STUN!";
            playerDamageText.gameObject.SetActive(true);
            yield return new WaitForSeconds(1);
        }

        yield return new WaitForSeconds(1);
        state = BattleState.ENEMYTURN;
        EnemyTurn();
    }

    // Method for Apple from Tree of Life Item
    IEnumerator UseApple()
    {
        if (badMusicSync)
        {
            float waitTime = GetNextBeat(4);
            Debug.Log(waitTime);
            if (waitTime > (beatTime * 1))
            {
                yield return new WaitForSeconds(waitTime - (beatTime * 1));
            }
            else
            {
                yield return new WaitForSeconds(waitTime + (beatTime * 3));
            }
        }
        int stunChance = Random.Range(1, 3);
        if ((!playerStats.stun) || (playerStats.stun && stunChance == 1))
        {
            playerAnim.SetTrigger("Apple");
            yield return new WaitForSeconds(beatTime * 5);
            playerStats.currentDmgBoostDuration = playerStats.dmgBoostDuration;
            if (playerStats.dmgBoost)
			{
                dmgBoostMax = 2;
                maxText.gameObject.SetActive(false);
                maxText.gameObject.SetActive(true);
            }
            playerStats.dmgBoost = true;
            UpdateEffectDisplay(playerStats.buffDisplay, playerStats.buffText, true, playerStats);
            yield return new WaitForSeconds(beatTime * 2);

            if (!noCooldowns)
                appleRestore = DisableItem(appleObject, appleCooldown);
        }
        else
        {
            playerDamageText.gameObject.SetActive(false);
            playerDamageText.text = "<color=#C75700>STUN!";
            playerDamageText.gameObject.SetActive(true);
            yield return new WaitForSeconds(1);
        }

        yield return new WaitForSeconds(1);
        state = BattleState.ENEMYTURN;
        EnemyTurn();
        // Buff Indicator handling needed here!
    }

    // Method for God Apple
    IEnumerator UseGodApple()
    {
        if (badMusicSync)
        {
            float waitTime = GetNextBeat(4);
            Debug.Log(waitTime);
            if (waitTime > (beatTime * 1))
            {
                yield return new WaitForSeconds(waitTime - (beatTime * 1));
            }
            else
            {
                yield return new WaitForSeconds(waitTime + (beatTime * 3));
            }
        }
        int stunChance = Random.Range(1, 3);
        if ((!playerStats.stun) || (playerStats.stun && stunChance == 1))
        {
            playerAnim.SetTrigger("Apple");
            yield return new WaitForSeconds(beatTime * 5);
            playerStats.currentMaxDuration = playerStats.maxDuration;
            if (playerStats.max)
			{
                maxMax = 10;
                maxText.gameObject.SetActive(false);
                maxText.gameObject.SetActive(true);
            }
            playerStats.max = true;
            UpdateEffectDisplay(playerStats.buffDisplay, playerStats.buffText, true, playerStats);
            yield return new WaitForSeconds(beatTime * 2);

            if (!noCooldowns)
                godAppleRestore = DisableItem(godAppleObject, godAppleCooldown);
        }
        else
        {
            playerDamageText.gameObject.SetActive(false);
            playerDamageText.text = "<color=#C75700>STUN!";
            playerDamageText.gameObject.SetActive(true);
            yield return new WaitForSeconds(1);
        }

        yield return new WaitForSeconds(1);
        state = BattleState.ENEMYTURN;
        EnemyTurn();
        // Buff Indicator handling needed here!
    }

    // Method for White Snake Venom Item
    IEnumerator UseVenom()
    {
        if (badMusicSync)
        {
            float waitTime = GetNextBeat(4);
            Debug.Log(waitTime);
            if (waitTime > (beatTime * 2))
            {
                yield return new WaitForSeconds(waitTime - (beatTime * 2));
            }
            else
            {
                yield return new WaitForSeconds(waitTime + (beatTime * 2));
            }
        }
        int stunChance = Random.Range(1, 3);
        if ((!playerStats.stun) || (playerStats.stun && stunChance == 1))
        {
            playerAnim.SetTrigger("Venom");
            yield return new WaitForSeconds(beatTime * 6);
            if (playerStats.TakeDamage(3))
                playerStats.currentHP = 1;
            transitionMusic();
            playerHUD.SetHP(playerStats);
            // Shaky Effect
            if (false)
            {
                for (int i = 0; i < 10; i++)
                {
                    player.transform.position += new Vector3(5f, 0, 0);
                    yield return new WaitForSeconds(0.01f);
                    player.transform.position -= new Vector3(5f, 0, 0);
                    yield return new WaitForSeconds(0.01f);
                }
            }
            yield return new WaitForSeconds(beatTime * 3.5f);
            playerStats.currentVampirismDuration = playerStats.vampirismDuration;
            if (playerStats.vampirism)
			{
                vampirismMax = 2;
                maxText.gameObject.SetActive(false);
                maxText.gameObject.SetActive(true);
            }
            playerStats.vampirism = true;
            UpdateEffectDisplay(playerStats.buffDisplay, playerStats.buffText, true, playerStats);

            if (!noCooldowns)
                venomRestore = DisableItem(venomObject, venomCooldown);

            yield return new WaitForSeconds(1);
            state = BattleState.ENEMYTURN;
            EnemyTurn();
            // Needs Indicator handling
        }
        else
        {
            playerDamageText.gameObject.SetActive(false);
            playerDamageText.text = "<color=#C75700>STUN!";
            playerDamageText.gameObject.SetActive(true);
            yield return new WaitForSeconds(1);
            state = BattleState.ENEMYTURN;
            EnemyTurn();
        }

    }

    // Method for Sack of Knowledge Item
    IEnumerator UseSackOfKnowledge(){
        state = BattleState.ENEMYTURN;
        EnemyTurn();
        yield return null;
    }
    // New Items =================================================================

    //UI Element Methods -----------------------------------------------------------------------------
    public void OnGlockUsed(){
        closeAllInterface();
        if(state != BattleState.PLAYERTURN){
            return;
        }

        StartCoroutine(UseGlock());

    }

    public void OnMirrorUsed(){
        closeAllInterface();
        if(state != BattleState.PLAYERTURN){
            return;
        }

        StartCoroutine(UseMirror());

    }

    public void OnCudgelUsed(){
        closeAllInterface();
        if(state != BattleState.PLAYERTURN){
            return;
        }

        StartCoroutine(UseCudgel());

    }

    public void OnMagicClothUsed(){
        closeAllInterface();
        if(state != BattleState.PLAYERTURN){
            return;
        }

        StartCoroutine(UseMagicCloth());

    }

    public void OnSilverHandsUsed(){
        closeAllInterface();
        if(state != BattleState.PLAYERTURN){
            return;
        }

        StartCoroutine(UseSilverHands());

    }

    public void OnRapunzelHairUsed(){
        closeAllInterface();
        if(state != BattleState.PLAYERTURN){
            return;
        }

        StartCoroutine(UseRapunzelHair());

    }

    // New Items=================================================
    public void OnIronShoesUsed(){
        closeAllInterface();
        if(state != BattleState.PLAYERTURN){
            return;
        }

        StartCoroutine(UseIronShoes());

    }

    public void OnHatUsed(){
        closeAllInterface();
        if(state != BattleState.PLAYERTURN){
            return;
        }

        StartCoroutine(UseHat());

    }

    public void OnCrystalBallUsed(){
        closeAllInterface();
        if(state != BattleState.PLAYERTURN){
            return;
        }

        StartCoroutine(UseCrystalBall());

    }

    public void OnAppleUsed(){
        closeAllInterface();
        if(state != BattleState.PLAYERTURN){
            return;
        }

        StartCoroutine(UseApple());

    }

    public void OnGodAppleUsed(){
        closeAllInterface();
        if(state != BattleState.PLAYERTURN){
            return;
        }

        StartCoroutine(UseGodApple());

    }

    public void OnVenomUsed(){
        closeAllInterface();
        if(state != BattleState.PLAYERTURN){
            return;
        }

        StartCoroutine(UseVenom());

    }

    public void OnSackOfKnowledgeUsed(){
        closeAllInterface();
        if(state != BattleState.PLAYERTURN){
            return;
        }

        StartCoroutine(UseSackOfKnowledge());

    }
    // New Items=================================================

    public void OnAttackButton()
    {
        closeAllInterface();
        if(state != BattleState.PLAYERTURN){
            return;
        }

        StartCoroutine(PlayerAttack());
    }

}
