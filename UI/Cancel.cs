using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class Cancel : MonoBehaviour, ICancelHandler
{

    private GameObject main;
    private GameObject modes;
    private GameObject characterSelect;
    private GameObject options;
    private GameObject controls;
    private GameObject credits;
    private Sprite redSprite;
    private Sprite orangeSprite;
    private Sprite greenSprite;
    private Sprite purpleSprite;
    private Sprite blueHandleSprite;

    public void SetMenu(GameObject main)
    {
        this.main = main;
    }

    public void SetModes(GameObject modes)
    {
        this.modes = modes;
    }

    public void SetCharSelect(GameObject select)
    {
        this.characterSelect = select;
    }

    public void SetOptions(GameObject options)
    {
        this.options = options;
    }

    public void SetControls(GameObject controls)
    {
        this.controls = controls;
    }

    public void SetRedSprite(Sprite redSprite)
    {
        this.redSprite = redSprite;
    }

    public void SetOrangeSprite(Sprite orangeSprite)
    {
        this.orangeSprite = orangeSprite;
    }

    public void SetGreenSprite(Sprite greenSprite)
    {
        this.greenSprite = greenSprite;
    }

    public void SetPurpleSprite(Sprite purpleSprite)
    {
        this.purpleSprite = purpleSprite;
    }

    public void SetBlueHandleSprite(Sprite blueHandleSprite)
    {
        this.blueHandleSprite = blueHandleSprite;
    }

    public void OnCancel(BaseEventData eventData)
    {
        Return();
    }

    private void Return()
    {
        if (!transform.parent.CompareTag("Main"))
        {
            transform.parent.gameObject.SetActive(false);
            if (transform.parent.CompareTag("Modes"))
            {
                modes.transform.GetChild(6).GetComponent<Image>().sprite = greenSprite;
                modes.transform.GetChild(7).GetComponent<Image>().sprite = orangeSprite;
                modes.transform.GetChild(8).GetComponent<Image>().sprite = redSprite;
                modes.transform.GetChild(9).GetComponent<Image>().sprite = purpleSprite;
                main.SetActive(true);
            }
            else if (transform.parent.CompareTag("CharacterSelect"))
            {
                characterSelect.transform.GetChild(4).GetChild(1).GetComponent<Image>().sprite = redSprite;
                characterSelect.transform.GetChild(5).GetChild(1).GetComponent<Image>().sprite = redSprite;
                modes.SetActive(true);
                modes.transform.GetChild(6).GetComponent<Image>().sprite = greenSprite;
                modes.transform.GetChild(7).GetComponent<Image>().sprite = orangeSprite;
                modes.transform.GetChild(8).GetComponent<Image>().sprite = redSprite;
                modes.transform.GetChild(9).GetComponent<Image>().sprite = purpleSprite;
            }
            else if (transform.parent.CompareTag("Options"))
            {
                options.transform.GetChild(4).GetChild(3).GetChild(0).GetComponent<Image>().sprite = blueHandleSprite;
                options.transform.GetChild(5).GetChild(3).GetChild(0).GetComponent<Image>().sprite = blueHandleSprite;
                options.transform.GetChild(6).GetChild(3).GetChild(0).GetComponent<Image>().sprite = blueHandleSprite;
                options.transform.GetChild(7).GetChild(3).GetChild(0).GetComponent<Image>().sprite = blueHandleSprite;
                main.SetActive(true);
            }
            else if (transform.parent.CompareTag("Controls"))
            {
                options.SetActive(true);
            }
        }
    }
}
