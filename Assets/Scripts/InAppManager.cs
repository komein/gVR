﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Purchasing;
using System;

public class InAppManager : IStoreListener
{
	private static IStoreController m_StoreController;
	private static IExtensionProvider m_StoreExtensionProvider;

	public const string pLevels = "plvl";
	public const string pLevelsAppStore = "app_plvl";
	public const string pLevelsGooglePlay = "gp_plvl";

    public bool readyFlag = false;

    Action successAction = null;
    Action failAction = null;

    public InAppManager()
    {
        if (m_StoreController == null)
        {
            InitializePurchasing();
        }
    }

	public void InitializePurchasing()
	{
		if (IsInitialized())
		{
			return;
		}
#if IAP
        var builder = ConfigurationBuilder.Instance(StandardPurchasingModule.Instance());
		builder.AddProduct(pLevels, ProductType.NonConsumable, new IDs() { { pLevelsAppStore, AppleAppStore.Name }, { pLevelsGooglePlay, GooglePlay.Name } });
		UnityPurchasing.Initialize(this, builder);
#endif
    }

    private bool IsInitialized()
	{
		return m_StoreController != null && m_StoreExtensionProvider != null;
	}

	public bool BuyProductID(string productId)
	{
		try
		{
			if (IsInitialized())
			{
				Product product = m_StoreController.products.WithID(productId);

				if (product != null && product.availableToPurchase)
				{
					Debug.Log(string.Format("Purchasing product asychronously: '{0}'", product.definition.id));
					m_StoreController.InitiatePurchase(product);
                    return true;
				}
				else
				{
					Debug.Log("BuyProductID: FAIL. Not purchasing product, either is not found or is not available for purchase");
                    return false;
				}
			}
			else
			{
				Debug.Log("BuyProductID FAIL. Not initialized.");
                return false;
			}
		}
		catch (Exception e)
		{
			Debug.Log("BuyProductID: FAIL. Exception during purchase. " + e);
            return false;
		}
	}

	public void RestorePurchases()
	{
		if (!IsInitialized())
		{
			Debug.Log("RestorePurchases FAIL. Not initialized.");
			return;
		}
        /*
		if (Application.platform == RuntimePlatform.IPhonePlayer || Application.platform == RuntimePlatform.OSXPlayer)
		{
			Debug.Log("RestorePurchases started ...");
            
			var apple = m_StoreExtensionProvider.GetExtension<IAppleExtensions>();
			apple.RestoreTransactions((result) =>
				{
					Debug.Log("RestorePurchases continuing: " + result + ". If no further messages, no purchases available to restore.");
				});
		}
		else*/
		{
			Debug.Log("RestorePurchases FAIL. Not supported on this platform. Current = " + Application.platform);
		}
	}

	public void OnInitialized(IStoreController controller, IExtensionProvider extensions)
	{
		Debug.Log("OnInitialized: Completed!");

		m_StoreController = controller;
		m_StoreExtensionProvider = extensions;

        readyFlag = true;
    }

	public void OnInitializeFailed(InitializationFailureReason error)
	{
		Debug.Log("OnInitializeFailed InitializationFailureReason:" + error);
	}

	public PurchaseProcessingResult ProcessPurchase(PurchaseEventArgs args)
	{
		Debug.Log(string.Format("ProcessPurchase: PASS. Product: '{0}'", args.purchasedProduct.definition.id));
		if (String.Equals(args.purchasedProduct.definition.id, pLevels, StringComparison.Ordinal))
		{
            if (null != successAction)
            {
                successAction();
            }
		}
		return PurchaseProcessingResult.Complete;
	}

	public void OnPurchaseFailed(Product product, PurchaseFailureReason failureReason)
	{
		Debug.Log(string.Format("OnPurchaseFailed: FAIL. Product: '{0}', PurchaseFailureReason: {1}", product.definition.storeSpecificId, failureReason));
        if (null != failAction)
        {
            failAction();
        }
	}

    public bool IsProductBought(string id)
    {
        foreach(var v in m_StoreController.products.all)
        {
            if (v.definition.id == id)
            {
                if (v.receipt != null)
                    return true;
                return false;
            }
        }
        return false;
    }

    public void SetActions(Action s, Action f)
    {
        successAction = s;
        failAction = f;
    }

    internal bool AreLevelsPurchased()
    {
        return IsProductBought(pLevels);
    }

    internal bool PurchaseLevel()
    {
        return BuyProductID(pLevels);
    }
    
}