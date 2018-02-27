using System.Collections;
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

    private void Awake()
    {
        cardSpawner = GetComponent<HL_CardSpawner>();
        animatorInterface = GetComponent<FX_AnimatorInterface>();
    }

    private void Update()
    {
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
        if (sceneManager && sceneManager.switchingScenes)
            return;

        if (CanDeal()) {
            TimedDataLogger.Get().AddToLog("HL Deal (choice=" + (userChoiceHigher ? "higher)" : "lower)"), "hl_deal");

            SetButtonsEnabled(false);

            float potentialWalletOutcome = walletScript.wallet + (walletScript.potScript.potTotal * 2); /* This line accounts for the result of a hi-low win (i.e. doubling 
                                                                                                           a pot of 10 to 20, with a target of 15, would be a loss). */
            //bool requiresWin = (testBalance < FCD_RiggingTools.Globals.balanceCurve[engineInPlay.turn]);
            bool requiresWin = (potentialWalletOutcome < FCD_RiggingTools.Globals.balanceCurve[engineInPlay.turn-1]);
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
        if (!evaluating) {
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
                dealerBehaviour.ResetCards();
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
        return cardSpawner && engineInPlay != null && walletScript && !evaluating && spawningEnabled;
    }
}
