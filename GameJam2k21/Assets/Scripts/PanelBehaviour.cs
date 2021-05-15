using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
public class PanelBehaviour : MonoBehaviour
{
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
        if (Input.GetKeyDown("k"))
            taggedEvent.Invoke(gameObject.name);
    }

    public void addListener(UnityAction<string> listener)
    {
        taggedEvent.AddListener(listener);
    }
}
