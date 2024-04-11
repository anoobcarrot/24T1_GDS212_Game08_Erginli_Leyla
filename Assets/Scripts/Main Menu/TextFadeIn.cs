using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class TextFadeIn : MonoBehaviour
{
    public TextMeshProUGUI textMeshPro;
    public Button buttonToEnable;
    public float letterAppearInterval = 0.05f;
    public float delayBeforeEnable = 1f;

    private void Start()
    {
        // Hide the text initially
        textMeshPro.maxVisibleCharacters = 0;

        // Disable the button initially
        buttonToEnable.gameObject.SetActive(false);

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

        // Wait for a delay before enabling the button
        yield return new WaitForSeconds(delayBeforeEnable);

        // Enable the button
        buttonToEnable.gameObject.SetActive(true);
    }
}



