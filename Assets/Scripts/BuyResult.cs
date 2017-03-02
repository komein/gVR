using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BuyResult : MonoBehaviour
{

    public string successMessage;
    public string failMessage;

    public Text text;

    public BuyPageButton returnButton;

    DataStorage storage;

	void Start ()
    {
        returnButton.gameObject.SetActive(false);

        storage = FindObjectOfType<DataStorage>();

        if (null != storage)
        {
            //storage.SetActions(SetSuccessMessage, SetFailMessage);
            if (true == storage.PurchaseLevel())
                SetSuccessMessage();
            else
                SetFailMessage();
        }
	}

    public void SetSuccessMessage()
    {
        text.text = successMessage;
        returnButton.gameObject.SetActive(true);
    }

    public void SetFailMessage()
    {
        text.text = failMessage;
        returnButton.gameObject.SetActive(true);
    }
	
}
