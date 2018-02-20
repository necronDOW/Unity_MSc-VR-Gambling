using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class EventHandle : MonoBehaviour
{
    public bool disableCanvasInput = true;
    [SerializeField] private UnityEvent onTrigger;

    private void Awake()
    {
        if (disableCanvasInput)
        {
            Button btn = GetComponent<Button>();
            if (btn)
                btn.interactable = false;
        }
    }

    public void Trigger()
    {
        //Debug.Log(gameObject.name + " event triggered.");
        onTrigger.Invoke();
    }
}
