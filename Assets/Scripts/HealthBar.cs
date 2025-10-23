using UnityEngine;
using UnityEngine.UI;

public class HealthBar : MonoBehaviour
{
    [SerializeField] private Slider slider;
    [SerializeField] private Transform target;
    [SerializeField] private Vector3 offset;

    public void SetCurrentHealth(float currentHealth)
    {
        slider.value = currentHealth;
    }

    public void UpdateBar(float maxHealth, float currentHealth)
    {
        slider.maxValue = maxHealth;
        slider.value = currentHealth;
    }

    private void Update()
    {
        if (target == null) return;
        transform.eulerAngles = Vector3.zero;
        transform.position = target.position + offset;
    }
}
