using System.Linq;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class FCD_GameOver : MonoBehaviour
{
    [SerializeField] private Text finalBalanceText;
    private MaskableGraphic[] gfx;
    private Color[] gfxColors, gfxColorsTemp;

    private void Awake()
    {
        gfx = GetComponentsInChildren<MaskableGraphic>();
        gfxColorsTemp = gfxColors = new Color[gfx.Length];

        for (int i = 0; i < gfx.Length; i++) {
            gfxColorsTemp[i] = gfxColors[i] = gfx[i].color;

            gfxColorsTemp[i].a = 0.0f;
            gfx[i].color = gfxColorsTemp[i];
        }
    }

    public void TriggerGameOver(FCD_WalletScript walletScript)
    {
        if (finalBalanceText != null) {
            finalBalanceText.text = string.Format("Balance £{0:f2}", walletScript.wallet);
            StartCoroutine(FadeToVisible(1.0f));
        }
        else {
            Debug.LogWarning("No final balance text target set, game over screen will not be shown.");
        }
    }

    private IEnumerator FadeToVisible(float duration)
    {
        float t = 0.0f;
        while (t <= 1.0f) {
            for (int i = 0; i < gfx.Length; i++) {
                gfxColorsTemp[i].a = t;
                gfx[i].color = gfxColorsTemp[i];
            }
            
            t += Time.deltaTime / duration;
            yield return null;
        }
    }
}
