using NUnit.Framework.Constraints;
using UnityEngine;

public class Cloaker : MonoBehaviour
{

    [Header("Settings")]
    public float stealthDuration = 3f;
    private float timer = 0f;
    private bool isCloaked = false;
    private bool cloak75 = false;
    private bool cloak25 = false;

    private Health health;
    private SpriteRenderer sr;

    void Awake()
    {
        health = GetComponent<Health>();
        sr = GetComponentInChildren<SpriteRenderer>();
    }

    void Update()
    {
        float current = health.GetCurrentHitPoints();
        float max = health.GetMaxHitPoints();
        float hpPercent = current / max;

        if(!cloak75 && hpPercent <= .75f)
      {
         cloak75 = true;
         Cloak();
      }
        if(!cloak25 && hpPercent <= .25f)
      {
         cloak25 = true;
         Cloak();
      }

      if (isCloaked)
      {
         timer += Time.deltaTime;
         if(timer >= stealthDuration)
         {
            Uncloak();
         }
      }
    }

    void Cloak()
   {
        if(isCloaked) return;
        isCloaked = true;
        timer = 0f;
        gameObject.tag = "Untagged";
        sr.color = new Color(.52f, .44f, .62f);
   }
   void Uncloak()
   {
      isCloaked = false;
      timer = 0f;
      gameObject.tag = "Enemy";
      sr.color = new Color(1f, 1f ,1f);
   }
}
