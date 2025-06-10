using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class ButtonFix : MonoBehaviour
{
    void Start()
    {
        Button[] buttons = FindObjectsOfType<Button>();
        foreach (var btn in buttons)
        {
            EventTrigger trigger = btn.gameObject.GetComponent<EventTrigger>();
            if (trigger == null)
                trigger = btn.gameObject.AddComponent<EventTrigger>();

            var entry = new EventTrigger.Entry();
            entry.eventID = EventTriggerType.PointerEnter;
            entry.callback.AddListener((data) => {
                EventSystem.current.SetSelectedGameObject(btn.gameObject);
            });

            trigger.triggers.Add(entry);
        }
    }
}
