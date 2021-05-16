using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
public class PanelBehaviour : MonoBehaviour
{
    bool isTagged = false;
    [SerializeField]
    GameObject[] panels;
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
            panels[0].SetActive(false);
            panels[1].SetActive(true);
            //Deactivate();
            taggedEvent.Invoke(gameObject.name);
            GameObject.FindObjectOfType<PlayerMovementRB>().PanelZoneExit();
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        print(other.gameObject.name);
        if (other.gameObject.layer == LayerMask.NameToLayer("Player"))
        {
            GameObject.FindObjectOfType<PlayerMovementRB>().PanelZoneEntered(this);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject.layer == LayerMask.NameToLayer("Player"))
        {
            GameObject.FindObjectOfType<PlayerMovementRB>().PanelZoneExit();
        }
    }

    public void Activate()
    {
        transform.parent.gameObject.SetActive(true);
        isTagged = false;
    }

    public void Deactivate()
    {
        transform.parent.gameObject.SetActive(false);
    }
}
