using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DealerScript : MonoBehaviour
{
    public GameObject cardPrefab;
    public Transform[] dealTargets;
    public Configs config;
    public LoggerWrapper logger;

    [HideInInspector]
    public CardScript[] cardsInPlay { get; protected set; }

    [HideInInspector]
    public DeckScript deck { get; protected set; }

    private WalletScript wallet;
    private bool evaluated = false;
    private const float betValue = 0.5f;
    private FXManager fxManager;
    private MainAudioManager audioManager;
    private int turn = 0;
    private bool dealEnabled = true;
    private int logFileIndex = -1;
    private CameraFade cameraFade;

    protected virtual void Start()
    {
        wallet = GameObject.FindGameObjectWithTag("UI").GetComponent<WalletScript>();
        audioManager = GameObject.FindGameObjectWithTag("Scene").GetComponent<MainAudioManager>();
        fxManager = transform.Find("FX").GetComponent<FXManager>();

        if (logger)
            logFileIndex = logger.CreateLogFile("gamblingtask", "resp(ms)", "click(x)", "click(y)", "win");

        cameraFade = Camera.main.GetComponent<CameraFade>();
    }
    
    private void Update()
    {
        if (AnimationsFinished())
        {
            if (!evaluated)
            {
                JokerSwap(cardsInPlay[0], cardsInPlay[1]);
                bool win = cardsInPlay[0].value == cardsInPlay[1].value;

                if (win)
                {
                    float cardValue = cardsInPlay[0].value - deck.suitValue;
                    float winValue = betValue;

                    if (cardValue == 1)
                        winValue *= 11;
                    else winValue *= Mathf.Clamp(cardValue, 1, 10);

                    wallet.Add(winValue);

                    fxManager.PlayPositiveFX();
                    audioManager.PlayWinSound((int)(winValue / betValue));
                }
                else
                {
                    fxManager.PlayNegativeFX();
                    audioManager.PlayLoseSound();
                }

                if (logger)
                    logger.LogBool(logFileIndex, win, true);

                evaluated = true;
                
                if (turn == config.cardSceneInterventionTurn)
                {
                    GameObject.FindGameObjectWithTag("Scene Manager").GetComponent<SceneMngr>().SwitchScene(config.interventionSceneIndex, config.interventionTransition, 0.5f);
                    dealEnabled = false;
                }
            }
        }
    }

    private void OnEnable()
    {
        dealEnabled = true;

        if (logger)
            logger.RecordResponseTime(true);
    }

    private void OnDisable()
    {
        if (logger)
            logger.RecordResponseTime(false);
    }

    public void Deal()
    {
        if (dealEnabled)
        {
            if ((AnimationsFinished() && evaluated) || (!cardsInPlay[0] && !cardsInPlay[1]))
            {
                if (logger)
                {
                    logger.LogResponseTime(logFileIndex);
                    logger.LogClickPoint(logFileIndex);
                }

                audioManager.PlayPaySound();

                int valueA = deck.GetNextCard();
                int valueB = deck.GetNextCard();

                if (valueA != -1 && valueB != -1 && wallet.CanDeduct(betValue))
                {
                    InstantiateCard(0, valueA);
                    InstantiateCard(1, valueB);
                    wallet.Deduct(betValue);

                    evaluated = false;
                    turn++;
                }
                else
                {
                    wallet.playerData.cashedOut = false;
                    ShowGameOverScene();
                }
            }
        }
    }

    public void CashOut()
    {
        if (turn > config.cardSceneInterventionTurn)
        {
            wallet.playerData.cashedOut = true;
            ShowGameOverScene();
        }
    }

    private void JokerSwap(CardScript a, CardScript b)
    {
        if (a.value + b.value == deck.jokerValue * 2)
            a.value *= -1;
        else if (a.value == deck.jokerValue)
            a.value = b.value;
        else if (b.value == deck.jokerValue)
            b.value = a.value;
    }

    private void InstantiateCard(ushort index, int value)
    {
        if (cardsInPlay[index])
            Destroy(cardsInPlay[index].gameObject);

        GameObject newCard = Instantiate(cardPrefab, transform.position + new Vector3(0.0f, 0.01f * index, 0.0f), cardPrefab.transform.rotation);
        newCard.transform.SetParent(transform);

        cardsInPlay[index] = newCard.GetComponent<CardScript>();
        cardsInPlay[index].SetParameters(value, dealTargets[index], 1 - index, config.cardSceneDealSpeed);
    }

    private bool AnimationsFinished()
    {
        return (cardsInPlay[0] && cardsInPlay[1] && cardsInPlay[0].animationDone && cardsInPlay[1].animationDone);
    }

    private void ShowGameOverScene()
    {
        if (cameraFade)
            cameraFade.FadeOut(0.25f, 3);
        else UnityEngine.SceneManagement.SceneManager.LoadScene(3);
    }
}