using UnityEngine;

public class CamRotation : MonoBehaviour
{
    void NativeRecenter(Quaternion rot)
    {
        UnityEngine.XR.InputTracking.Recenter();
    }

    private void OnEnable()
    {
        FindObjectOfType<TPoseCalibration>().onSuccess += NativeRecenter;
    }

    void OnDisable()
    {
        FindObjectOfType<TPoseCalibration>().onSuccess -= NativeRecenter;
    }
}
