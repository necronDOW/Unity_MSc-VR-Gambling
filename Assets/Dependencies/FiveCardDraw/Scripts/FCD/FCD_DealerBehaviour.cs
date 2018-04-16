﻿//#define RUN_DEBUG_SIMULATION

using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FCD_DealerBehaviour : MonoBehaviour
{
    [SerializeField] private FCD_WalletScript walletScript;
    [SerializeField] private FCD_GameOver gameOverScript;
    [SerializeField] private HL_GMBehaviour hl_GameMasterBehaviour;
    [SerializeField] private SceneManager_v2 sceneManager;
    [SerializeField] private FCD_ButtonGroup holdButtonGroup;
    [SerializeField] private UINudge holdReminderNudge;
    public FCD_DelayedWalletUpdate delayedWalletUpdater { get; private set; }

    private FCD_DealerEngine engine;
    private FCD_CardBehaviour[] cards;
    private int phase = 0;
    private int[] baseDealtHand;
    private FX_AnimatorInterface animatorInterface;

    private bool isTiming = false;
    private float playTimer = 0.0f;
    private bool isSwitchingToHiLo = false;
    private bool gameOver = false;

#if RUN_DEBUG_SIMULATION
    private const int maxSimHold = 3;
    private const int maxPlays = -1;
    private int currentPlays = 0;
    public static bool isDebugSimulation = true;
#else
    public static bool isDebugSimulation = false;
#endif

    private void Awake()
    {
        int engineInstance = -1;
        engine = FCD_DealerEngine.GetInstance(ref engineInstance);
        engine.walletScript = walletScript;
        hl_GameMasterBehaviour.currentEngineInstance = engineInstance;

        cards = GetComponentsInChildren<FCD_CardBehaviour>();
        SetBaseDealtHand(new int[5] { -1, -1, -1, -1, -1 });
        delayedWalletUpdater = walletScript.GetComponent<FCD_DelayedWalletUpdate>();
        animatorInterface = GetComponent<FX_AnimatorInterface>();
    }

    private void Update()
    {
        if (isTiming)
            playTimer += Time.deltaTime;

        if (gameOver && !sceneManager.switchingScenes) {
            gameOverScript.TriggerGameOver(walletScript);
            //gameOver = false;
        }
    }

    private void OnApplicationQuit()
    {
        DataLogger.Get().CleanUp();
        TimedDataLogger.Get().CleanUp();
    }

    public void DoPhaseBehaviour()
    {
        if (gameOver || (sceneManager && sceneManager.switchingScenes))
            return;

        switch (phase) {
            case 0:
                if (cards.All(x => x.outcomeRevealed)) {
                    TimedDataLogger.Get().AddToLog("FCD Hand Begin");
                    engine.DrawNewHand();
#if !RUN_DEBUG_SIMULATION
                    UpdateVisuals();
#endif
                    LogHand("New Hand", engine.hand.fullHand);

                    if (holdButtonGroup)
                        holdButtonGroup.SetActive(true);

                    SetBaseDealtHand(engine.hand.fullHand);
                    isTiming = true;
                    cards.All(x => x.outcomeRevealed = false);
                    phase++;


#if RUN_DEBUG_SIMULATION
                DoPhaseBehaviour();
#endif
                }
                break;

            case 1:
                TimedDataLogger.Get().AddToLog("FCD Deal Result");
#if RUN_DEBUG_SIMULATION
                int holdCards = Random.Range(1, maxSimHold+1);
                while (engine.hand.currentHand.Count != holdCards) {
                    int toHoldIndex = Random.Range(0, 5);

                    while (true) {
                        if (engine.hand.heldIndices[toHoldIndex] == true) {
                            toHoldIndex = (toHoldIndex + 1) % 5;
                        }
                        else {
                            engine.hand.ToggleHold(toHoldIndex);
                            break;
                        }
                    }
                }
#endif
                Phase1Behaviour();
                break;
        }
    }

#if RUN_DEBUG_SIMULATION
    private void Phase1Behaviour()
    {
        if (engine.hand.currentHand.Count > 0) {
            LogHand("Held Cards", engine.hand.currentHand.ToArray());

            engine.CompleteRiggedHand();

            LogHand("Final Cards", engine.hand.fullHand);
            LogTime();
            OutputLog();

            string hand = "";
            for (int i = 0; i < engine.hand.fullHand.Length; i++)
                hand += (engine.hand.fullHand[i] % 13) + ",";

            engine.hand.Lock();
            engine.FinishTurn();
            
            SetBaseDealtHand(new int[5] { -1, -1, -1, -1, -1 });

            phase = 0;

            if ((maxPlays == -1 || ++currentPlays < maxPlays) && engine.canDrawNewHand) {
                DoPhaseBehaviour();
            }
        }
}
#else
    private void Phase1Behaviour()
    {
        if (engine.hand.currentHand.Count > 0) {
            LogHand("Held Cards", engine.hand.currentHand.ToArray());

            engine.CompleteRiggedHand();

            UpdateVisuals();

            LogHand("Final Cards", engine.hand.fullHand);
            LogTime();
            OutputLog();

            if (holdButtonGroup)
                holdButtonGroup.SetActive(false);

            string hand = "";
            for (int i = 0; i < engine.hand.fullHand.Length; i++)
                hand += (engine.hand.fullHand[i] % 13) + ",";

            engine.hand.Lock();
            engine.FinishTurn();

            StartCoroutine(TriggerOutcomeAnimations());
            SetBaseDealtHand(new int[5] { -1, -1, -1, -1, -1 });

            phase = 0;

            // Switch to Hi-Lo game
            if (engine.handWasWin) {
                SwitchToHiLow(1.875f);

                FCD_AnimationTracker tracker = GetComponent<FCD_AnimationTracker>();
                if (tracker)
                    tracker.SetTrackingOverride(true);
            }
            else {
                CheckGameOver(2.0f);
            }
        } else {
            if (holdReminderNudge != null) {
                holdReminderNudge.PlayNudge(3.0f);
            }
        }
    }
#endif

    public void ToggleCardHeld(int cardIndex)
    {
#if !RUN_DEBUG_SIMULATION
        if (phase == 1 && cardIndex >= 0 && cardIndex < engine.hand.size) {
            engine.hand.ToggleHold(cardIndex);
            holdReminderNudge.StopNudge();
        }
#endif
    }

    public void ResetCards()
    {
        foreach (FCD_CardBehaviour c in cards) {
            c.TriggerAnimatorReset();
        }
    }

    private void SetBaseDealtHand(int[] values)
    {
        baseDealtHand = new int[values.Length];
        for (int i = 0; i < values.Length; i++)
            baseDealtHand[i] = values[i];
    }
    
    private void UpdateVisuals()
    {
        for (int i = 0; i < cards.Length; i++) {
            if (baseDealtHand[i] != engine.hand.fullHand[i] || !engine.hand.heldIndices[i])
                cards[i].FlipCard(engine.hand.fullHand[i]);
        }
    }

    private IEnumerator TriggerOutcomeAnimations()
    {
        yield return new WaitForSeconds(1.5f);
        
        for (int i = 0; i < cards.Length; i++) {
            Animator animator = cards[i].GetComponent<Animator>();

            if (IsWinningCard(engine.hand.fullHand[i]))
                animator.SetBool("Winning_Card", true);

            animator.SetTrigger("Reveal_Outcome");
        }

        yield return null;
    }

    private bool IsWinningCard(int value)
    {
        if (engine.winningValues != null) {
            foreach (int v in engine.winningValues) {
                if (value == v)
                    return true;
            }
        }

        return false;
    }

    private void LogTime()
    {
        isTiming = false;
        DataLogger.Get().AddToLog("Turn Time", string.Format("{0:f3}s", playTimer));
        playTimer = 0.0f;
    }

    private void LogHand(string label, int[] values)
    {
        string str = "";

        for (int i = 0; i < values.Length; i++) {
            str += FCD_Deck.TranslateValue(values[i]) + ",";
        }

        DataLogger.Get().AddToLog(label, str);
    }

    private void OutputLog()
    {
        DataLogger.Get().Log("Turn " + (engine.turn + 1));
        TimedDataLogger.Get().Log("FCD Turn " + (engine.turn + 1));
    }

    public void SwitchToHiLow(float delay)
    {
        if (sceneManager) {
            StartCoroutine(FX_AnimatorInterface.PlayAnimatorInterfaceAfterSeconds(delay, animatorInterface));
            StartCoroutine(RebindAnimationsAfter(delay + SceneManager_v2.lerpTransitionTime));
            sceneManager.SwitchScene(1, SceneManager_v2.TransitionMode.Lerp, delay, SceneManager_v2.DisableMode.PostTransition);
        }
    }

    public void CheckGameOver(float delay = 0.0f)
    {
        if (!engine.canDrawNewHand) {
            if (delay > 0.0f)
                StartCoroutine(SetGameOver_delayed(delay));
            else gameOver = true;
        }
    }

    private IEnumerator SetGameOver_delayed(float delay)
    {
        yield return new WaitForSeconds(delay);
        gameOver = true;
    }

    private IEnumerator RebindAnimationsAfter(float delay)
    {
        yield return new WaitForSeconds(delay);
        for (int i = 0; i < cards.Length; i++) {
            cards[i].Rebind();
        }
    }
}
