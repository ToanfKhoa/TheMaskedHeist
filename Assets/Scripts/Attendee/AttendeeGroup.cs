using UnityEngine;

/// Gan vao 1 GameObject cha chua nhieu Attendee lam con.
/// Chinh maskColor/useGrayMask roi bam "Apply To Group" (hoac thay doi trong Inspector) de ap dung cho ca nhom.
public class AttendeeGroup : MonoBehaviour
{
    [SerializeField] Color maskColor = Color.white;
    [SerializeField] bool useGrayMask = false;

    private void OnValidate()
    {
        ApplyToGroup();
    }

    [ContextMenu("Apply To Group")]
    public void ApplyToGroup()
    {
        PlayerDetector[] detectors = GetComponentsInChildren<PlayerDetector>(true);
        foreach (PlayerDetector detector in detectors)
        {
            detector.SetMaskColor(maskColor);
            detector.SetRequireGrayscaleMask(useGrayMask);
        }
    }
}
