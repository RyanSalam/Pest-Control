using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HealthUI : MonoBehaviour
{
    [SerializeField] Image healthBarImage;
    public float currentHealthValue;
    public float maxHealthValue;
    [SerializeField] Image bloodyScreen;
    [SerializeField] float bloodScreenDuration;

    Coroutine damagedRoutine;

    private void Start()
    {
        bloodyScreen.color = new Color(bloodyScreen.color.r, bloodyScreen.color.g, bloodyScreen.color.b, 0.01f);
        currentHealthValue = LevelManager.Instance.Player.CurrentHealth;
        LevelManager.Instance.Player.OnHealthChanged += UpdateHealth;
    }

    // Update current health. Caused when taking damage or gaining health
    public void UpdateHealth(float maxHealth, float health)
    {
        // If health is reduced from the last update
        if (currentHealthValue > health)
        {
            // Start bloody screen coroutine here
            // Making sure it doesn't cut out when called before the coroutine is over
            if (damagedRoutine == null)
                damagedRoutine = StartCoroutine(BloodyScreenSequence());
            else
            {
                StopCoroutine(damagedRoutine);
                damagedRoutine = StartCoroutine(BloodyScreenSequence());
            }
        }

        maxHealthValue = maxHealth;
        currentHealthValue = health;
        // Set health to current health as a decimal
        float healthAsDecimal = currentHealthValue / maxHealthValue;
        healthBarImage.fillAmount = healthAsDecimal;
    }
    public void ResetHealth()
    {
        float healthAsDecimal = maxHealthValue / maxHealthValue;
        healthBarImage.fillAmount = healthAsDecimal;
    }

    public IEnumerator BloodyScreenSequence()
    {
        Debug.Log("Coroutine getting called");
        bloodyScreen.CrossFadeAlpha(255f, 0.5f, false);
        yield return new WaitForSeconds(bloodScreenDuration);
        bloodyScreen.CrossFadeAlpha(0.01f, 0.65f, false);
    }
}
