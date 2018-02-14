using UnityEngine;

public class AudioInterface_Advanced : AudioInterface
{
    [SerializeField] private AudioClip[] clipSelection;
    int lastClip = -1;

    private void OnValidate()
    {
        audioClip = null;
    }

    protected override void Awake()
    {
        base.Awake();
    }

    public override void PlayClip()
    {
        if (clipSelection != null && clipSelection.Length > 0) {
            int nextClip = Random.Range(0, clipSelection.Length);

            if (nextClip == lastClip) {
                nextClip = (nextClip + 1) % clipSelection.Length;
            }

            lastClip = nextClip;
            audioClip = clipSelection[nextClip];

            base.PlayClip();
        }
    }
}
