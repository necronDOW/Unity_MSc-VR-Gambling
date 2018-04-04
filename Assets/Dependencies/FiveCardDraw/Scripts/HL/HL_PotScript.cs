using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class HL_PotScript : MonoBehaviour
{
    public Transform targetOnMerge;
    public int targetFontSizeOnMerge = -1;
    public Object smokePrefab;
    public AudioInterface positiveAudioInterface;
    public AudioInterface negativeAudioInterface;

    public float potTotal { get; private set; }
    private Text textComponent;
    private RectTransform rTransform;
    private Vector3 resetPosition;
    private Vector3 resetScale;
    private Color resetColor;

    private void Awake()
    {
        textComponent = GetComponent<Text>();
        rTransform = GetComponent<RectTransform>();
        resetPosition = rTransform.localPosition;
        resetScale = rTransform.localScale;
        resetColor = textComponent.color;

        SetVisible(false);
    }

    public void AddToPot(float amount)
    {
        if (amount > 0.0f)
            potTotal += amount;
        UpdateUI();
    }

    public void EmptyPot(FCD_WalletScript targetWallet)
    {
        if (targetWallet)
            StartCoroutine(EmptyPot_Coroutine_Positive(1.125f, targetWallet));
        else {
            if (smokePrefab) {
                GameObject.Instantiate(smokePrefab, transform.position, transform.rotation, transform.parent);
                PlayOutcomeAudio(false);
            }

            StartCoroutine(EmptyPot_Coroutine_Negative(0.15f));
        }
    }

    public void SetVisible(bool value)
    {
        Text[] textComponents = GetComponentsInChildren<Text>();
        foreach (Text t in textComponents)
            t.enabled = value;

        if (value) {
            ResetPot();
            UpdateUI();
        }
    }

    private void UpdateUI()
    {
        textComponent.text = string.Format("£{0:f2}", potTotal);
    }

    private void ResetPot()
    {
        rTransform.localPosition = resetPosition;
        rTransform.localScale = resetScale;
        textComponent.color = resetColor;
    }

    private IEnumerator EmptyPot_Coroutine_Negative(float duration, float delay = 0.0f)
    {
        if (delay > 0.0f)
            yield return new WaitForSeconds(delay);

        for (float t = 0.0f; t < 1.0f; t += Time.deltaTime) {
            t += (Time.deltaTime / duration);
            rTransform.localScale = Vector3.Lerp(resetScale, Vector3.zero, t);
            yield return null;
        }

        potTotal = 0.0f;
        yield return null;
    }

    private IEnumerator EmptyPot_Coroutine_Positive(float duration, FCD_WalletScript targetWallet, float delay = 0.0f)
    {
        if (delay > 0.0f)
            yield return new WaitForSeconds(delay);

        if (targetOnMerge) {
            float scalar = (targetFontSizeOnMerge != -1) ? ((float)targetFontSizeOnMerge / (float)textComponent.fontSize) : 0.0f;
            Vector3 targetScale = resetScale * scalar;

            Color targetColor = resetColor;
            targetColor.a = 0.0f;

            for (float t = 0.0f; t < 1.0f; t += Time.deltaTime) {
                t += (Time.deltaTime / duration);
                rTransform.position = Vector3.Lerp(resetPosition, targetOnMerge.position, t);
                rTransform.localScale = Vector3.Lerp(resetScale, targetScale, t);
                textComponent.color = Color.Lerp(resetColor, targetColor, t);

                yield return null;
            }
        }
        else yield return new WaitForSeconds(duration);

        if (targetWallet) {
            targetWallet.AddDirect(potTotal);
            PlayOutcomeAudio(true);
        }

        SetVisible(false);

        potTotal = 0.0f;
        yield return null;
    }

    private void PlayOutcomeAudio(bool positive)
    {
        if (positive && positiveAudioInterface) {
            positiveAudioInterface.PlayClip();
        }
        else if (!positive && negativeAudioInterface)
            negativeAudioInterface.PlayClip();
    }
}
