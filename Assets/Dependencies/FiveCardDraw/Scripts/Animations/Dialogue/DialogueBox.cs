using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DialogueBox : MonoBehaviour
{
    public int borderTotal = 50;
    public Object buttonPrefab;
    public UIBlockInput inputBlocker;

    private RectTransform[] elements;
    private GameObject[] addedButtons;

    private void Awake()
    {
        elements = GetComponentsInChildren<RectTransform>();
    }

    public void Show(DialogueOptions options)
    {
        if (inputBlocker)
            inputBlocker.blockAllGameInput = true;
        transform.parent.gameObject.SetActive(true);

        UpdateElementSizes(options.width, options.height);
        HideUnusedElements(options);

        elements[1].GetComponent<Text>().text = options.title;
        elements[2].GetComponent<Text>().text = options.description;
        AddButtons(options);
    }

    public void Hide()
    {
        for (int i = 0; i < addedButtons.Length; i++)
            Destroy(addedButtons[i]);
        addedButtons = null;

        if (inputBlocker)
            inputBlocker.blockAllGameInput = false;
        transform.parent.gameObject.SetActive(false);
    }

    private void UpdateElementSizes(int width, int height)
    {
        width = Mathf.Clamp(width, 0, Screen.width - 10);
        height = Mathf.Clamp(height, 0, Screen.height - 10);

        if (width > borderTotal && height > borderTotal) {
            elements[0].sizeDelta = new Vector2(width, height);
        }
        else if (width > 0 && height > 0) {
            Debug.Log("The specified dialogue dimensions (w=" + width + ", h=" + height + ") do not meet the minimum requirement of " + (borderTotal + 1) + ". Size unchanged.");
            return;
        }

        for (int i = 1; i < elements.Length; i++) {
            elements[i].sizeDelta = new Vector2(
                elements[0].sizeDelta.x - borderTotal,
                elements[i].sizeDelta.y);
        }
    }

    private void HideUnusedElements(DialogueOptions options)
    {
        elements[1].gameObject.SetActive(options.hasTitle);
        elements[2].gameObject.SetActive(options.hasDescription);
        elements[3].gameObject.SetActive(options.hasButtons);

        int totalShownElements = 0;
        for (int i = 1; i < elements.Length; i++) {
            if (elements[i].gameObject.activeSelf)
                totalShownElements++;
        }

        float spacing = elements[0].sizeDelta.y / (totalShownElements + 1);
        for (int i = 1, j = 0; i < elements.Length; i++) {
            if (elements[i].gameObject.activeSelf) {
                float halfHeight = elements[i].sizeDelta.y / 2.0f;
                elements[i].anchoredPosition = new Vector2(elements[i].anchoredPosition.x, -(spacing * ++j) + halfHeight);
            }
        }
    }

    private void AddButtons(DialogueOptions options)
    {
        if (!options.hasButtons || !buttonPrefab)
            return;

        float buttonWidth = elements[3].sizeDelta.x / options.buttons.Count;
        addedButtons = new GameObject[options.buttons.Count];

        for (int i = 0; i < options.buttons.Count; i++) {
            addedButtons[i] = (GameObject)Instantiate(buttonPrefab, elements[3]);

            RectTransform rt = addedButtons[i].GetComponent<RectTransform>();
            rt.anchoredPosition = new Vector2(buttonWidth * i, rt.anchoredPosition.y);
            rt.sizeDelta = new Vector2(buttonWidth, rt.sizeDelta.y);

            Text txt = addedButtons[i].GetComponentInChildren<Text>();
            txt.text = options.buttons[i].text;

            Button btn = addedButtons[i].GetComponent<Button>();
            if (options.buttons[i].onClickAction != null) {
                btn.onClick.AddListener(options.buttons[i].onClickAction);
            }
            else if (options.buttons[i].onClickStandardAction != DialogueOptions.ButtonOptions.StandardAction.Null) {
                switch (options.buttons[i].onClickStandardAction) {
                    case DialogueOptions.ButtonOptions.StandardAction.Cancel:
                        btn.onClick.AddListener(Hide);
                        break;
                }
            }
        }
    }
}
