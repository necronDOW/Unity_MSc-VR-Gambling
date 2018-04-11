﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HL_GMBehaviour : MonoBehaviour
{
    [SerializeField] private FCD_WalletScript walletScript;
    [SerializeField] private FCD_DealerBehaviour dealerBehaviour;
    [SerializeField] private SceneManager_v2 sceneManager;
    [SerializeField] private FCD_PhysicalButton[] gameButtons;

    private FX_AnimatorInterface animatorInterface;
    
    public float testBalance = 50.0f;

    private FCD_DealerEngine engineInPlay;
    public int currentEngineInstance
    {
        set {
            engineInPlay = FCD_DealerEngine.GetInstance(ref value);
        }
    }
    private HL_CardSpawner cardSpawner;
    private bool dealResult;
    private bool spawningEnabled = false;
    private bool evaluating = false;
    private float evaluateTimer = 0.0f;
    private int turn = 0;
    private const int maxTurns = 1;

    private bool exit = false;
    private float exitTime = 0.0f;

    private void Awake()
    {
        cardSpawner = GetComponent<HL_CardSpawner>();
        animatorInterface = GetComponent<FX_AnimatorInterface>();
    }

    private void Update()
    {
        if (exit) {
            exitTime -= Time.deltaTime;
            if (exitTime <= 0.0f) {
                ExitGame(dealResult);
            }
            return;
        }

        if (evaluating) {
            evaluateTimer += Time.deltaTime;
            if (evaluateTimer >= 0.5f) {
                SetButtonsEnabled(true);

                evaluateTimer = 0.0f;
                evaluating = false;

                if (dealResult)
                {
                    walletScript.DoublePot();
                    walletScript.UpdateUI();
                }
                else {
                    ExitGame(false);
                }

                if (turn >= maxTurns) {
                    exit = true;
                    exitTime = 0.1f;
                }
            }
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        HL_CardBehaviour cardBehaviour = other.GetComponent<HL_CardBehaviour>();
        if (cardBehaviour != null) {
            spawningEnabled = true;

            TimedDataLogger.Get().AddToLog("HL Card Shown (value=" + FCD_Deck.TranslateValue(cardBehaviour.CardValue) + ")", "hl_cardvalue");

            if (cardSpawner.spawnedCount > 1)
                evaluating = true;
        }
    }

    // This should be called before the engines turn has been incremented.
    public void DealCard(bool userChoiceHigher)
    {
        if ((sceneManager && sceneManager.switchingScenes) || exit)
            return;

        if (CanDeal()) {
            turn++;

            TimedDataLogger.Get().AddToLog("HL Deal (choice=" + (userChoiceHigher ? "higher)" : "lower)"), "hl_deal");

            SetButtonsEnabled(false);

            float curveTarget = FCD_RiggingTools.Globals.balanceCurve[engineInPlay.turn - 1];
            float offsetFromCurve = Mathf.Abs(walletScript.wallet - curveTarget);
            float potentialOffsetFromCurve = Mathf.Abs((walletScript.wallet + walletScript.potScript.potTotal * 2) - curveTarget); // This line accounts for the result of a hi-low win.

            // If a doubled pot would bring me closer to the target, regardless of going over, allow for a win.
            // Or... If the difference is equal, give a 50/50 chance of a win.
            bool requiresWin = potentialOffsetFromCurve < offsetFromCurve || (potentialOffsetFromCurve == offsetFromCurve && Random.Range(0, 2) == 1);
            bool adjustedChoice = userChoiceHigher;
            int lastCardValue = cardSpawner.lastCardValue;

            // If a loss is needed, reverse the user choice.
            if (!requiresWin)
                adjustedChoice = !adjustedChoice;

            cardSpawner.SpawnCard(adjustedChoice);
            spawningEnabled = false;

            dealResult = (userChoiceHigher && cardSpawner.lastCardValue > lastCardValue)
                || (!userChoiceHigher && cardSpawner.lastCardValue < lastCardValue);
        }
    }

    public void ExitGame(bool collectPot = true)
    {
        if (!evaluating && spawningEnabled) {
            spawningEnabled = false;

            if (collectPot) {
                TimedDataLogger.Get().AddToLog("Collected " + string.Format("£{0:f2}", walletScript.potScript.potTotal), "hl_collect");
            }
            
            TimedDataLogger.Get().Log("HL Done");

            walletScript.EmptyPot(collectPot);

            if (animatorInterface) {
                if (collectPot) {
                    animatorInterface.PlayAnimation(1);
                }
                else {
                    animatorInterface.PlayAnimation(2);
                }
            }

            if (sceneManager) {
                turn = 0;
                exit = false;
                exitTime = 0.0f;
                
                dealerBehaviour.CheckGameOver();
                sceneManager.SwitchScene(0, SceneManager_v2.TransitionMode.Lerp, 0.0f, SceneManager_v2.DisableMode.PostTransition);
            }
        }
    }

    private void SetButtonsEnabled(bool value)
    {
        for (int i = 0; i < gameButtons.Length; i++) {
            gameButtons[i].SetActive(value);
        }
    }

    private bool CanDeal()
    {
        return turn < maxTurns && cardSpawner && engineInPlay != null && walletScript && !evaluating && spawningEnabled;
    }
}
