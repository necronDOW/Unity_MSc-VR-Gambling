using System.Collections;
using UnityEngine;

public class BettingMachineBehaviour : MonoBehaviour
{
    [HideInInspector] public MinigameMngr minigameMngr;
    [HideInInspector] public int machineIndex = -1;
    [HideInInspector] public CardBehaviour insertedCard;
    public VirtualAudioListener machineAudio;

    [SerializeField] private Material screenBaseMaterial;
    [SerializeField] private BM_Screen[] screens;
    [SerializeField] private string sceneName = "";
    private bool isOn = false;

    private void Start()
    {
        for (int i = 0; i < screens.Length; i++) {
            screens[i].Setup(this, i, 1024, 1024, screenBaseMaterial);
        }
        
        ToggleOn();
    }

    private void OnCollisionEnter(Collision collision)
    {
        BM_Screen screen = collision.contacts[0].thisCollider.GetComponent<BM_Screen>();

        if (screen && !screen.InteractionIsBlocked(collision)) {
            screen.ProjectToMinigame(collision.contacts[0].point);
            screen.AddToBlockedInteractions(collision);

            SteamVR_TrackedObject controller = collision.gameObject.GetComponent<SteamVR_TrackedObject>();
            if (controller != null) {
                StartCoroutine(ControllerRumble((int)controller.index, 0.1f));
            }
        }
    }

    public void ToggleOn()
    {
        isOn = !isOn;

        if (isOn) {
            minigameMngr.LoadMinigame(machineIndex, sceneName);
        }
        else {
            minigameMngr.UnloadMinigame(machineIndex);
        }
    }

    public void SetScreenFeed(Camera feed, int index)
    {
        screens[index].SetFeed(feed);

        if (machineAudio) {
            machineAudio.PopulateInputsRelativeToCamera(feed);
        }
    }

    private IEnumerator ControllerRumble(int controllerIndex, float duration, ushort strength = 3999)
    {
        strength = (ushort)Mathf.Clamp(strength, 1, 3999);

        while (duration > 0) {
            duration -= Time.deltaTime;
            SteamVR_Controller.Input(controllerIndex).TriggerHapticPulse(strength);
            yield return null;
        }

        yield return null;
    }
}