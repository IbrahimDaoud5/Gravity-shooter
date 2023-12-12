using System;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class ChangeInput : MonoBehaviour
{
    EventSystem system;
    public Selectable firstInput;



    // Start is called before the first frame update
    void Start()
    {
        system = EventSystem.current;
        firstInput.Select();

    }

    // Update is called once per frame
    void Update()
    {
        try
        {
            if (Input.GetKeyDown(KeyCode.Tab) && Input.GetKey(KeyCode.LeftShift))
            {
                Selectable previous = system.currentSelectedGameObject.GetComponent<Selectable>().FindSelectableOnUp();
                if (previous != null)
                {
                    previous.Select();
                }
            }
            else if (Input.GetKeyDown(KeyCode.Tab))
            {
                Selectable next = system.currentSelectedGameObject.GetComponent<Selectable>().FindSelectableOnDown();
                if (next != null)
                {
                    next.Select();
                }

            }
        }
        catch (Exception e)
        {
            Debug.LogError(e.ToString()); // This will print the entire exception, including the stack trace
        }




    }
}
