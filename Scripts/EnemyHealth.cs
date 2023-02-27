using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyHealth : MonoBehaviour
{
    public float maxHealth;
    public float currentHealth;
    Color startingColor;

    public bool isDead = false;

    void Start()
    {
        startingColor = gameObject.GetComponent<Renderer>().material.color;
    }
    // Update is called once per frame
    void Update()
    {
        AdjustCurrentHealth(0);
    }

    public void AdjustCurrentHealth(float addToHealth)
    {
        currentHealth += addToHealth;

        if (currentHealth < 0)
        {
            isDead = true;
            Destroy(gameObject);
        }
        else if (currentHealth > maxHealth)
        {
            currentHealth = maxHealth;
        }

        this.GetComponent<Renderer>().material.color = Color.Lerp(new Color(0.8f, 0.2f, 0.2f), startingColor, currentHealth / maxHealth);
    }

    public void SetMaxHealth(float newMax)
    {
        maxHealth = newMax;
    }

    public void SetCurrentHealth(float newCurrent)
    {
        currentHealth = newCurrent;
    }
}
