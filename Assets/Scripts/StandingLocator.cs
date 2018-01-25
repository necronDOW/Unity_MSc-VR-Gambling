using UnityEngine;

public class StandingLocator : MonoBehaviour
{
    [SerializeField] private Transform vrCameraRig;
    [SerializeField] private bool ignoreY = true;

    private void Start()
    {
        Camera vrCamera = vrCameraRig.GetComponentInChildren<Camera>();
        Vector3 targetPosition = transform.position - vrCamera.transform.localPosition;

        vrCameraRig.position = new Vector3(targetPosition.x, ignoreY ? vrCameraRig.position.y : targetPosition.y, targetPosition.z);
        vrCameraRig.RotateAround(vrCamera.transform.position, Vector3.up, 180 + vrCamera.transform.localEulerAngles.y);
    }
}
