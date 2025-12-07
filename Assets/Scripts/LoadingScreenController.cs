using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.SceneManagement;
using System.Collections;

public class LoadingScreenController : MonoBehaviour
{
    [Header("UI")]
    public Slider progressBar;
    public TMP_Text loadingText;
    public TMP_Text hintsText;

    private const float MIN_LOAD_TIME = 1.5f;

    private readonly string[] hints =
    {
        "You can choose your difficulty in the settings (the default is Normal).",
        "Some towers counter certain zombies, and some zombies counter certain towers.",
        "Save some gold to make mid-round decisions!",
        "Some zombies have sheilds. Use the correct tower types to break through.",
        "Don’t forget to place towers near choke points!",
        "Experiment with different tower combinations for synergy.",
    };

    private void Start()
    {
        // Display a random hint
        if (hintsText != null)
            hintsText.text = GetRandomHint();

        StartCoroutine(LoadRoutine("MVP"));
    }

    private string GetRandomHint()
    {
        int index = Random.Range(0, hints.Length);
        return hints[index];
    }

    private IEnumerator LoadRoutine(string sceneName)
    {
        AsyncOperation op = SceneManager.LoadSceneAsync(sceneName);
        op.allowSceneActivation = false;

        float elapsed = 0f;

        while (!op.isDone)
        {
            elapsed += Time.deltaTime;

            float fakeProgress = Mathf.Clamp01(elapsed / MIN_LOAD_TIME);
            float realProgress = Mathf.Clamp01(op.progress / 0.9f);
            float finalProgress = Mathf.Min(fakeProgress, realProgress);

            if (progressBar != null)
                progressBar.value = finalProgress;

            if (loadingText != null)
                loadingText.text = $"Loading... {Mathf.RoundToInt(finalProgress * 100)}%";

            bool fakeDone = fakeProgress >= 1f;
            bool realDone = realProgress >= 1f;

            if (fakeDone && realDone)
            {
                yield return new WaitForSeconds(0.1f);
                op.allowSceneActivation = true;
            }

            yield return null;
        }
    }
}