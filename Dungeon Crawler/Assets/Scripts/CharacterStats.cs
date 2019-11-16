﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterStats : MonoBehaviour
{
    [SerializeField] protected bool m_isLinked = true;
    /// <summary>
    /// Variable to store the level of the entity.
    /// </summary>
    [SerializeField] protected int m_level = 1;
    /// <summary>
    /// Variable to store the max health of an entity.
    /// </summary>
    [SerializeField] public int m_maxHealth = 100;
    /// <summary>
    /// Variable to store the current health of an entity.
    /// </summary>
    [SerializeField] public int m_currentHealth;
    /// <summary>
    /// A variable to store the maximum insanity level.
    /// </summary>
    public Stat m_maxInsanity;
    /// <summary>
    /// A variable to store the current insanity level.
    /// </summary>
    public float m_currentInsanity = 0;
    /// <summary>
    /// A variable to store the damage an entity does.
    /// </summary>
    [SerializeField] protected Stat m_damage;
    /// <summary>
    /// A variable to store the armour an entity has.
    /// </summary>
    [SerializeField] protected Stat m_armour;
    /// <summary>
    /// A variable to store the strength an entity has.
    /// </summary>
    [SerializeField] protected Stat m_strength;
    /// <summary>
    /// A variable to store the vitality an entity has.
    /// </summary>
    [SerializeField] protected Stat m_vitality;
    /// <summary>
    /// A variable to store the movement speed an entity has.
    /// </summary>
    [SerializeField] protected Stat m_movementSpeed;
    /// <summary>
    /// A variable to store the gold a player has; or the gold an enemy gives on death.
    /// </summary>
    public Stat m_gold;
    /// <summary>
    /// A variable to store the experience a player has; or the experience an enemy gives on death.
    /// </summary>
    public Stat m_experience;

    [SerializeField] protected Stat m_attackSpeed;
    /// <summary>
    /// A method which is called when the script is loaded. It calculates the entities stats.
    /// </summary>
    private void Awake()
    {
        
    }

    /// <summary>
    /// A method which is called every frame.
    /// </summary>
    private void Update()
    {

    }

    /// <summary>
    /// A method that is called on the first frame.
    /// </summary>
    private void Start()
    {
        m_currentHealth = m_maxHealth;
    }
    /// <summary>
    /// A method which is called when an entity takes damage. Removes health and calls the Die method when the entity dies.
    /// </summary>
    /// <param name="damage"></param>
    public virtual void TakeDamage(int damage)
    {
        damage -= m_armour.GetValue();
        damage = Mathf.Clamp(damage, 0, int.MaxValue);

        m_currentHealth -= damage;
        Debug.Log(transform.name + " takes " + damage + "damage." + "Current health is: " + m_currentHealth );

        if (m_currentHealth <= 0)
        {
            Die();
        }
    }

    /// <summary>
    /// Performs an action when an entity dies.
    /// </summary>
    public virtual void Die()
    {
        //Die in some way
        //Meant to be overriden
        Debug.Log(transform.name + " died.");
        Debug.Log("Aight imma head out.");
        Destroy(gameObject);
    }

    /// <summary>
    /// Calculates the entities stats. Meant to be overriden.
    /// </summary>
    public virtual void CalculateStats()
    {
       
    }
    /// <summary>
    /// Gets the entities movement speed.
    /// </summary>
    /// <returns></returns>
    public float GetMovementSpeed()
    {
        return m_movementSpeed.GetValue();
    }
    public float GetAttack()
    {
        return m_damage.GetValue();
    }
    public float GetArmour()
    {
        return m_armour.GetValue();
    }
    public float GetAttackSpeed()
    {
        return m_attackSpeed.GetValue();
    }
    public float SetAttackSpeed(int input)
    {
        return m_attackSpeed.SetValue(input);
    }
    public int GetStrength()
    {
        return m_strength.GetValue();
    }
    public int SetStrength(int input)
    {
        return m_strength.SetValue(input);
    }
    public int GetDamage()
    {
        return m_damage.GetValue();
    }
    public int SetDamage(int input)
    {
        return m_damage.SetValue(input);
    }
    public int SetArmour(int input)
    {
        return m_armour.SetValue(input);
    }
}
