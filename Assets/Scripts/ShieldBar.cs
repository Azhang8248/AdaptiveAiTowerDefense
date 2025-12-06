using UnityEngine;
using UnityEngine.UI;

public class ShieldBar : MonoBehaviour
{
    [SerializeField] private Slider slider;
    [SerializeField] private Transform target;
    [SerializeField] private Vector3 offset = new Vector3(0, 1f, 0);

    public void Initialize(Transform followTarget, int maxShield, int currentShield)
    {
        target = followTarget;
        slider.maxValue = maxShield;
        slider.value = currentShield;
    }

    public void SetCurrentShield(int currentShield)
    {
        slider.value = Mathf.Clamp(currentShield, 0, slider.maxValue);
    }

    public void UpdateBar(int maxShield, int currentShield)
    {
        slider.maxValue = maxShield;
        slider.value = Mathf.Clamp(currentShield, 0, maxShield);
    }

    private void Update()
    {
        if (target == null) return;

        // Keep UI upright
        transform.rotation = Quaternion.identity;
        transform.position = target.position + offset;
    }
}
