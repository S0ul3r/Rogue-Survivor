using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[DisallowMultipleComponent]
public class Health : MonoBehaviour
{
    private int startingHelath;
    private int currentHealth;

    /// <summary>
    /// Set starting health
    /// </summary>
    public void SetStartingHealth(int health)
    {
        this.startingHelath = health;
        currentHealth = health;
    }

    /// <summary>
    /// Get starting health
    /// </summary>
    public int GetStartingHealth()
    {
        return startingHelath;
    }
}
