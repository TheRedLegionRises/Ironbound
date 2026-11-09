
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class PlayerHealthUI : MonoBehaviour
{
    [Header("References")]
    public HealthSystem healthSystem;
    public Slider healthSlider;
    public TMP_Text healthText;

    void OnEnable()
    {
        if (healthSystem != null)
        {
            healthSystem.OnHealthChanged += UpdateHealthUI;
        }
    }

    void Start()
    {
        UpdateHealthUI();
    }

    void OnDisable()
    {
        if (healthSystem != null)
        {
            healthSystem.OnHealthChanged -= UpdateHealthUI;
        }
    }

    public void UpdateHealthUI()
    {
        if (healthSystem == null)
            return;

        int currentHealth = healthSystem.currentHealth;
        int maxHealth = healthSystem.MaxHealth;

        // Update slider
        healthSlider.maxValue = maxHealth;
        healthSlider.value = currentHealth;

        // Update text
        healthText.text = currentHealth + " / " + maxHealth;
    }
}
