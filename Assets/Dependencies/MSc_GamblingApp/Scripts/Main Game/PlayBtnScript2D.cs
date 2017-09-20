using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Collider2D))]
public class PlayBtnScript2D : MonoBehaviour
{
    public Sprite onClickSprite;

    private DealerScript2D dealer;
    private SpriteRenderer spriteRenderer;
    private Sprite originalSprite;

    private void Start()
    {
        dealer = GameObject.FindGameObjectWithTag("Dealer").GetComponent<DealerScript2D>();

        spriteRenderer = GetComponent<SpriteRenderer>();
        
        if (spriteRenderer)
            originalSprite = spriteRenderer.sprite;
    }

    private void OnMouseUp()
    {
        if (spriteRenderer && originalSprite)
            spriteRenderer.sprite = originalSprite;
    }
    private void OnMouseDown()
    {
        if (spriteRenderer && onClickSprite)
            spriteRenderer.sprite = onClickSprite;

        dealer.Deal();
    }
}