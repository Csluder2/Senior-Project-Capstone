using UnityEngine;
using TMPro;

public class FlashingText : MonoBehaviour
{
    public TextMeshProUGUI text;
    public float flashSpeed = 1.0f; // Speed of the flashing effect.

    private bool isVisible = true;

    void Start()
    {
        if (text == null)
            text = GetComponent<TextMeshProUGUI>();
    }

    void Update()
    {
        // Toggle visibility based on time.
        if (Time.time % (1 / flashSpeed) < (0.5f / flashSpeed))
        {
            if (!isVisible)
            {
                text.alpha = 1f;
                isVisible = true;
            }
        }
        else
        {
            if (isVisible)
            {
                text.alpha = 0f;
                isVisible = false;
            }
        }
    }
}

