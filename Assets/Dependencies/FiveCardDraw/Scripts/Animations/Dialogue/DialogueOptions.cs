using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class DialogueOptions
{
    public class ButtonOptions
    {
        public enum StandardAction {
            Null,
            Cancel
        }

        public string text { get; private set; }
        public UnityAction onClickAction { get; private set; }
        public StandardAction onClickStandardAction { get; private set; }
        public bool actionIsCustom { get { return onClickAction != null; } }

        public ButtonOptions(string text, UnityAction onClickAction)
        {
            this.text = text;
            this.onClickAction = onClickAction;
        }

        public ButtonOptions(string text, StandardAction onClickAction)
        {
            this.text = text;
            this.onClickStandardAction = onClickAction;
        }
    }

    public string title { get; private set; }
    public string description { get; private set; }
    public List<ButtonOptions> buttons { get; private set; }
    public int width = 0, height = 0;

    public bool hasTitle { get { return title != ""; } }
    public bool hasDescription { get { return description != ""; } }
    public bool hasButtons { get { return buttons != null; } }

    public DialogueOptions(string title, string description)
    {
        this.title = title;
        this.description = description;
    }

    public DialogueOptions(string title, string description, int width, int height)
    {
        this.title = title;
        this.description = description;
        this.width = width;
        this.height = height;
    }

    public DialogueOptions(string title, string description, List<ButtonOptions> buttons)
    {
        this.title = title;
        this.description = description;
        this.buttons = buttons;
    }

    public DialogueOptions(string title, string description, int width, int height, List<ButtonOptions> buttons)
    {
        this.title = title;
        this.description = description;
        this.buttons = buttons;
        this.width = width;
        this.height = height;
    }

    public void AddButton(string text, UnityAction onClickAction)
    {
        if (buttons == null)
            buttons = new List<ButtonOptions>();

        buttons.Add(new ButtonOptions(text, onClickAction));
    }

    public void AddButton(string text, ButtonOptions.StandardAction onClickAction)
    {
        if (buttons == null)
            buttons = new List<ButtonOptions>();

        buttons.Add(new ButtonOptions(text, onClickAction));
    }
}
