using UnityEngine;
using UnityEngine.Events;

public class OnInstantiated : MonoBehaviour
{
    [HideInInspector] public UnityEvent callback;

    private void Start()
    {
        callback.Invoke();
    }
}
