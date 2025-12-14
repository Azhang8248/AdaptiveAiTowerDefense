#if UNITY_EDITOR
using UnityEditor;
using UnityEditor.Timeline.Actions;
#endif
using UnityEngine;

public class AudioManager : MonoBehaviour
{
   public static AudioManager Instance;
   [Header("Music")]
   [SerializeField] private AudioSource backgroundSource;

   private void Awake()
   {
      if(Instance != null)
      {
         Destroy(gameObject);
         return;
      }

      Instance = this;
   }

   public void PlayMusic(AudioClip clip, float volume = .1f)
   {
      if(clip == null || backgroundSource == null)
      {
         return;
      }

      backgroundSource.clip = clip;
      backgroundSource.volume = volume;
      backgroundSource.loop = true;
      backgroundSource.Play();
   }

   public void StopMusic()
   {
      if(backgroundSource != null)
      {
         backgroundSource.Stop();
      }
   }

}