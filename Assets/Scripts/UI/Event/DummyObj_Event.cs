using Attributes;
using Data;
using System.Collections;
using System.Collections.Generic;
using UI;
using UnityEngine;

public class DummyObj_Event : MonoBehaviour, IEventDataProvider
{
    [SerializeField] private TextIdentifier defaultEventIdentifier;
    [SerializeField] private TextIdentifier currentEventIdentifier;
    [SerializeField] private EventUI eventUI;
        

    private void Awake()
    {
        currentEventIdentifier.Set(defaultEventIdentifier);
    }

    public string[] GetNextEventIdentifier()
    {
        return Database.EventTree[currentEventIdentifier.Category][currentEventIdentifier.Identifier].nextEventNodeIdentifier;
    }

    public string GetCurrentEventIdentifier()
    {
        return currentEventIdentifier.Identifier;
    }

    public void SetEventIdentifier(string identifier)
    {
        currentEventIdentifier.Set(identifier);
    }

    public void OnClickEvent()
    {
        currentEventIdentifier.Set(defaultEventIdentifier);
        eventUI.InitEventUI(this);
    }
}
