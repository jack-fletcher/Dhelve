﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class PlayerHotbar : MonoBehaviour
{

    /// <summary>
    /// A reference to the hotbar items. 
    /// </summary>
    public GameObject[] m_hotBarItems = new GameObject[4];
    /// <summary>
    /// A reference to the hotbar icons. 
    /// </summary>
    public Image[] m_hotBarIcons = new Image[4];

    // Update is called once per frame
    void Update()
    {
        CheckForInput();
    }

    void CheckForInput()
    {
        if (Input.GetButtonDown("Hotbar Slot 1"))
        {
            if (m_hotBarItems[0] != null)
            {
                ActivateEffect(m_hotBarItems[0]);
                m_hotBarItems[0] = null;
                m_hotBarIcons[0].sprite = null;
            }
        }
        else if (Input.GetButtonDown("Hotbar Slot 2"))
        {
            if (m_hotBarItems[1] != null)
            {
                ActivateEffect(m_hotBarItems[1]);
                m_hotBarItems[1] = null;
                m_hotBarIcons[1].sprite = null;
            }
        }
        else if (Input.GetButtonDown("Hotbar Slot 3"))
        {
            if (m_hotBarItems[2] != null)
            {
                ActivateEffect(m_hotBarItems[2]);
                m_hotBarItems[2] = null;
                m_hotBarIcons[2].sprite = null;
            }
        }
        else if (Input.GetButtonDown("Hotbar Slot 4"))
        {
            if (m_hotBarItems[3] != null)
            {
                ActivateEffect(m_hotBarItems[3]);
                m_hotBarItems[3] = null;
                m_hotBarIcons[3].sprite = null;
            }
        }
    }

    void ActivateEffect(GameObject item)
    {
        item.GetComponent<Item>().ActivateItem();
    }
}
