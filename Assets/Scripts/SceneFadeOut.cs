using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneFadeOut : MonoBehaviour
{
    public TextMeshProUGUI textMeshPro;
    public float letterAppearInterval = 0.05f;
    public float delayBeforeFade = 1f;
    public float fadeDuration = 1f;
    public CanvasGroup fadeCanvasGroup;
    public string sceneToLoad;

    private void Start()
    {
        // Hide the text initially
        textMeshPro.maxVisibleCharacters = 0;

        // Start the typewriter effect
        StartCoroutine(TypeText());
    }

    private IEnumerator TypeText()
    {
        // Gradually reveal each letter
        for (int i = 0; i <= textMeshPro.text.Length; i++)
        {
            textMeshPro.maxVisibleCharacters = i;
            yield return new WaitForSeconds(letterAppearInterval);
        }

        // Wait for a delay before fading the scene
        yield return new WaitForSeconds(delayBeforeFade);

        // Start fading the scene
        StartCoroutine(FadeScene());
    }

    private IEnumerator FadeScene()
    {
        float elapsedTime = 0f;
        fadeCanvasGroup.alpha = 0f;

        // Fade the screen to black
        while (elapsedTime < fadeDuration)
        {
            elapsedTime += Time.deltaTime;
            float alpha = Mathf.Lerp(0f, 1f, elapsedTime / fadeDuration);
            fadeCanvasGroup.alpha = alpha;
            yield return null;
        }

        // Load the next scene
        SceneManager.LoadScene(sceneToLoad);
    }
}


