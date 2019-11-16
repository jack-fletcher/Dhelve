﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class Item : MonoBehaviour
{
    /// <summary>
    /// A reference to a m_playerHotbar script.
    /// </summary>
    public PlayerHotbar m_playerHotbar;
    /// <summary>
    /// An integer value denoting how many of the item can be stored in a single slot in the hotbar.
    /// </summary>
    [SerializeField] private int m_maxNumberInSlot;
    /// <summary>
    /// A reference to an image variable, which is the image shown on the hotbar UI.
    /// </summary>
    [SerializeField] private Image m_hotBarIcon;
    /// <summary>
    /// A string that stores hover over text, so the player can understand what different items are and do.
    /// </summary>
    [SerializeField] private string m_hoverOverText;
    [SerializeField] public float m_itemCooldown;
    private bool m_isAvailable = true;

    /// <summary>
    /// Occurs when an item interacts with this. Checks that the other GameObject is a player and if so, activeates the OnPickUp script.
    /// </summary>
    /// <param name="other">A reference to the other GameObjects collider.</param>
    public virtual void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Player")
        {
            m_playerHotbar = GameObject.FindGameObjectWithTag("Player").GetComponentInChildren<PlayerHotbar>();
            Debug.Log("Trigger entered.");
            OnPickUp();
        }
    }
    /// <summary>
    /// A method which handles the interaction between player and item - Adds it to their inventory, plays animation/sound or uses it. Meant to be overriden.
    /// </summary>
    public virtual void OnPickUp()
    {

        ///Add to hotbar
        for (int i = 0; i < m_playerHotbar.m_hotBarItems.Length; i++)
        {
            if (m_playerHotbar.m_hotBarItems[i] == null)
            {
                m_playerHotbar.m_hotBarItems[i] = this.gameObject;
                m_playerHotbar.m_hotBarIcons[i].sprite = m_hotBarIcon.sprite;
                this.gameObject.GetComponent<MeshRenderer>().enabled = false;
                this.gameObject.GetComponent<Collider>().enabled = false;
                break;
            }
        }
      

       
    }

    public virtual void ActivateEffect()
    {
        Debug.Log("Item activated.");
    }

    /// <summary>
    /// Activates the item effect.
    /// </summary>
    public virtual void ActivateItem()
    {
        if (m_isAvailable)
        {
            m_isAvailable = false;
            StartCoroutine("ItemCooldown");
            ///Activate potion effect here
            ActivateEffect();
        }
    }

    /// <summary>
    /// Sets m_isAvailable to true after the items cooldown.
    /// </summary>
    /// <returns></returns>
    public virtual IEnumerator ItemCooldown()
    {
        yield return new WaitForSeconds(m_itemCooldown);
        m_isAvailable = true;
    }
}
