using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class FX_Animator : MonoBehaviour
{
    [SerializeField]
    private Sprite[] animationSprites;
    private int lastSpriteIndex = -1;
    private Image uiImage;
    private Animator uiAnimator;
    
    private bool initSuccess {
        get { return lastSpriteIndex != -1; }
    }

    public void Awake()
    {
        if (animationSprites.Length > 0) {
            uiImage = GetComponent<Image>();
            uiAnimator = GetComponent<Animator>();

            if (uiImage && uiAnimator) {
                uiImage.preserveAspect = true;
                uiImage.transform.localScale = Vector3.zero;
                ChangeSprite(0);

                lastSpriteIndex = 0;
            }
        }
    }

    public void Trigger(int index)
    {
        if (initSuccess && ChangeSprite(index)) {
            uiAnimator.SetTrigger("Animate");
        }
        else {
            Debug.LogWarning("The index provided to " + gameObject.name + "->FX_Animator (" + index + ") is invalid. Animation will not play!");
        }
    }

    private bool ChangeSprite(int index)
    {
        if (index >= 0 && index < animationSprites.Length) {
            uiImage.sprite = animationSprites[index];
            return true;
        }
        else return false;
    }
}
