using System.Collections;
using System.Collections.Generic;
using Microlight.MicroBar;
using TMPro;
using UnityEngine;

public class PlayerManager : Entity
{
    public Transform playerTarget;
    public ScreenFlash screenFlash;
    public MicroBar healthBar;
    public TMP_Text healthText;

    public void Start()
    {
        screenFlash = FindObjectOfType<ScreenFlash>();
        healthBar = FindObjectOfType<MicroBar>();
        healthBar.Initialize(maxHealth);

        healthText = GameObject.Find("HealthText").GetComponent<TMP_Text>();
        healthText.text = maxHealth.ToString();
    }

    public override void GetHit(float damage)
    {
        float previous = currentHealth;
        base.GetHit(damage);
        StartCoroutine(UpdateHealthText(previous, currentHealth, 0.5f));
        screenFlash.Flash();
        healthBar.UpdateBar(currentHealth);
        CinemachineShake.Instance.Shake(0.5f, 0.2f);

        AudioManager.instance.PlayOneShot(FMODEvents.instance.playerHit, this.transform.position);
    }

    protected override void Die()
    {
        GameManager.Instance.LoseLevel();
    }

    public Transform GetPlayerTarget()
    {
        return playerTarget;
    }

    private IEnumerator UpdateHealthText(float current, float target, float duration)
    {
        float elapsed = 0f;
        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            healthText.text = Mathf.Lerp(current, target, elapsed / duration).ToString("0");
            yield return null;
        }
        healthText.text = target.ToString();
    }
}
