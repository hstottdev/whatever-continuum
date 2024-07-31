using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class HealthManager : MonoBehaviour
{
    public float maxHealth = 10;
    [SerializeField] Color damageColour;
    Color normalColor;
    [SerializeField] float flashDuration;
    public UnityEvent onDeath;
    [HideInInspector] public float health;

    void Awake()
    {
        health = maxHealth;
        if(TryGetComponent(out SpriteRenderer rend))
        {
            normalColor = rend.color;
        }
    }

    public void Transaction(float healthChange, float delay = 0)
    {
        StartCoroutine(TransactionIEnum(healthChange, delay));
    }

    public void Kill()
    {
        Transaction(-maxHealth);
    }

    IEnumerator TransactionIEnum(float healthChange, float delay)
    {
        yield return new WaitForSeconds(delay);

        if(TryGetComponent(out SpriteRenderer rend))
        {
            for(int x = 0; x < 4; x++)
            {
                rend.color = damageColour;
                yield return new WaitForSeconds(flashDuration);
                rend.color = normalColor;
                yield return new WaitForSeconds(flashDuration);
            }
       }
        health += healthChange;

        if(health > maxHealth)
        {
            health = maxHealth;
        }

        if(health <= 0)
        {
            health = 0;
            onDeath.Invoke();
        }
    }
}
