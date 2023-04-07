using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class CancelControls : MonoBehaviour, ICancelHandler
{
    [SerializeField] private GameObject options;

    public void CloseControls()
    {
        transform.parent.gameObject.SetActive(false);
        GameObject.FindGameObjectWithTag("Startup").GetComponent<PauseMenu>().SetInControls();
        options.SetActive(true);
    }

    public void OnCancel(BaseEventData eventData)
    {
        CloseControls();
    }
}
