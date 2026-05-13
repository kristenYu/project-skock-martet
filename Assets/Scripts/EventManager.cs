using System.Collections.Generic;
using UnityEngine;


[System.Serializable]
public class ArrayWrapper
{
    public Event[] innerArray;
}

public class EventManager : MonoBehaviour
{
    public ArrayWrapper[] allEventChains;
    public List<Event> eventQueue;

    void Start()
    {
        eventQueue = new List<Event>();
    }
    public void addChaintoEventQueue(Event[] eventChain)
    {
        for(int i = 0; i < eventChain.Length; i++)
        {
            int eventIndex;
            for(eventIndex = 0; eventIndex < eventQueue.Count; eventIndex++)
            {
                if(eventQueue[eventIndex] == null)
                {
                    eventQueue[eventIndex] = eventChain[i];
                    break;
                }
            }
            if(eventIndex == eventQueue.Count - 1)
            {
                eventQueue.Add(eventChain[i]);
            }
        }     
    }
}
