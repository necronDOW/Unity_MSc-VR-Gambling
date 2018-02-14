using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FCD_ButtonPulser : MonoBehaviour
{
    public FCD_PhysicalButton[] stateDependentTargets;
    public FCD_PhysicalButton[] stateIndependentTargets;
    public float pulseSpeed = 1.0f;
    private int targetStateColorIndex = 0;
    private Color targetColor;
    private float tValue = 0.0f;

    private void Awake()
    {
        targetColor = GetTargetColor();
    }

    private void Update() {
        tValue += (Time.deltaTime * pulseSpeed);
        if (tValue >= 1.0f) {
            targetStateColorIndex = (targetStateColorIndex + 1) % 2;
            targetColor = GetTargetColor();
            tValue = 0.0f;
        }

        for (int i = 0; i < stateDependentTargets.Length; i++) {
            if (stateDependentTargets[i].state == FCD_PhysicalButton.State.Deselected) {
                stateDependentTargets[i].materialInstance.color = Color.Lerp(stateDependentTargets[i].materialInstance.color, targetColor, tValue);
            }
        }

        for (int i = 0; i < stateIndependentTargets.Length; i++) {
            if (stateIndependentTargets[i].state != FCD_PhysicalButton.State.Inactive)
                stateIndependentTargets[i].materialInstance.color = Color.Lerp(stateIndependentTargets[i].materialInstance.color, targetColor, tValue);
        }
    }

    private Color GetTargetColor()
    {
        if (stateDependentTargets.Length > 0)
            return stateDependentTargets[0].GetColor((FCD_PhysicalButton.State)targetStateColorIndex);
        else if (stateIndependentTargets.Length > 0)
            return stateIndependentTargets[0].GetColor((FCD_PhysicalButton.State)targetStateColorIndex);
        else return Color.white;
    }
}
