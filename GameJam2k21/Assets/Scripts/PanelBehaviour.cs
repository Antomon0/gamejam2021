using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
public class PanelBehaviour : MonoBehaviour
{
    bool isTagged = false;
    public class PanelEvent : UnityEvent<string>
    {

    }
    PanelEvent taggedEvent = new PanelEvent();
    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
    }

    public void addListener(UnityAction<string> listener)
    {
        taggedEvent.AddListener(listener);
    }

    public void Tag()
    {
        if (!isTagged)
        {
            isTagged = true;
            taggedEvent.Invoke(gameObject.name);
        }
    }
}
