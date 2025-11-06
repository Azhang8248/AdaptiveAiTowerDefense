using System;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class ShieldBar : MonoBehaviour
{
   [SerializeField] private Slider slider;
   [SerializeField] private Transform target;
   [SerializeField] private Vector3 offset;

   public void SetCurrentShield(int currentShield)
   {
      slider.value = currentShield;
   }
   public void UpdateBar(int maxShield, int currentShield)
   {
      slider.maxValue = maxShield;
      slider.value = currentShield;
   }
   
    void Update()
   {
      if (target == null) return;

        transform.eulerAngles = new Vector3(0, 0, 0);
        transform.position = target.position + offset;
   }
}