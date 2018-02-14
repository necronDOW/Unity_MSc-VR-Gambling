using System.Collections;
using UnityEngine;
using UnityEngine.Events;

public class FCD_PhysicalButton : MonoBehaviour
{
    private const int STATE_COUNT = 3;

    public State state;
    [SerializeField] private State stateOnActivated;
    [SerializeField] private bool retainSelected = true;
    [SerializeField] private Color[] stateColors = { Color.white, new Color(1.0f, 1.0f, 1.0f, 0.5f), new Color(1.0f, 1.0f, 1.0f, 0.0f) };
    [SerializeField] private Texture[] stateGraphics = new Texture[STATE_COUNT];
    public UnityEvent onClicked;
    public Material materialInstance { get; private set; }

    private AudioInterface audioInterface;
    private UIBlockInput blockInput;

    public enum State {
        Selected,
        Deselected,
        Inactive
    }

    private bool active{
        get { return !(state == State.Inactive); }
    }

    private void OnValidate()
    {
        if (stateColors.Length != STATE_COUNT)
            System.Array.Resize(ref stateColors, STATE_COUNT);

        if (stateGraphics.Length != STATE_COUNT)
            System.Array.Resize(ref stateGraphics, STATE_COUNT);
    }

    private void Awake()
    {
        materialInstance = GetComponent<MeshRenderer>().material;
        audioInterface = GetComponent<AudioInterface>();
        blockInput = GameObject.FindGameObjectWithTag("Canvas").GetComponent<UIBlockInput>();

        SetUnassignedStateGraphics();
        SetState(state, 0.0f);
    }

    private void Update()
    {
        if (Input.GetMouseButtonDown(0) && (!blockInput || !blockInput.blockAllGameInput)) {
            RaycastHit hitInfo;
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);

            if (Physics.Raycast(ray, out hitInfo, 100.0f)) {
                if (hitInfo.collider.gameObject == gameObject && hitInfo.collider.isTrigger)
                    Trigger();
            }
        }
    }

    public void SetUnassignedStateGraphics()
    {
        for (int i = 0; i < STATE_COUNT; i++) {
            if (stateGraphics[i] == null)
                stateGraphics[i] = materialInstance.mainTexture;
        }
    }

    public void SetState(State newState, float fadeDuration = 0.2f, bool waitForGraphicUpdate = false)
    {
        state = newState;
        StartCoroutine(MaterialFadeScript.FadeToColor(materialInstance, stateColors[(int)state], fadeDuration));

        if (waitForGraphicUpdate)
            StartCoroutine(SetGraphicAfterSeconds(fadeDuration));
        else materialInstance.mainTexture = stateGraphics[(int)state];
    }

    private IEnumerator SetGraphicAfterSeconds(float seconds)
    {
        yield return new WaitForSeconds(seconds);

        materialInstance.mainTexture = stateGraphics[(int)state];

        yield return null;
    }

    public void SetActive(bool value)
    {
        if (value)
            SetState(stateOnActivated);
        else SetState(State.Inactive, 0.2f, true);
    }

    public void ToggleSelected()
    {
        if (state != State.Inactive) {
            State newState = (state == State.Selected) ? State.Deselected : State.Selected;
            SetState(newState);
        }
    }

    public Color GetColor(State state)
    {
        return stateColors[(int)state];
    }

    public void Trigger()
    {
        if (state != State.Inactive) {
            if (audioInterface)
                audioInterface.PlayClip();

            onClicked.Invoke();
        }
    }
}
