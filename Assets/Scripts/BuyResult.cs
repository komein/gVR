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

	void Start ()
    {
        returnButton.gameObject.SetActive(false);
        
        if (null != DataObjects.IAPManager)
        {
            SetMessage(DataObjects.IAPManager.PurchaseLevel());
        }
	}

    public void SetMessage(bool result)
    {
        if (null != text)
        {
            text.text = result ? successMessage : failMessage;
        }

        if (null != returnButton)
        {
            returnButton.gameObject.SetActive(true);
        }
    }
	
}
