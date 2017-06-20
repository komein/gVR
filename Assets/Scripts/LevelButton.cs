using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class LevelButton : SceneButton, IUICanReinitialize
{
    public LevelButtonContainer container;

    private BuyButton buyButton;
    private StarProgressBar starBar;
    private ScoreDisplayer scoreDisplayer;

    Image lockImage;
    Image activeBg;
    Image inactiveBg;

    LevelInfo level;

    bool isInitialized = false;

    public void Initialize()
    {
        if (!isInitialized)
        {
            container = GetComponentInParent<LevelButtonContainer>();
            scoreDisplayer = GetComponentInChildren<ScoreDisplayer>();

            level = DataObjects.LevelInfo(scenePath);

            if (null != level)
            {
                if (null != scoreDisplayer)
                {
                    scoreDisplayer.levelTitle = scenePath;
                    isInitialized = true;
                }
            }
        }

        if (isInitialized)
        {
            RefreshButtonState();
        }
    }

    private void Awake()
    {
        scoreDisplayer = GetComponentInChildren<ScoreDisplayer>();
        starBar = GetComponentInChildren<StarProgressBar>();

        lockImage = GetComponentsInChildren<Image>().ToList().Find(p => p.name == "lock");
        if (null != lockImage)
            lockImage.gameObject.SetActive(false);

        activeBg = GetComponentsInChildren<Image>().ToList().Find(p => p.name == "back_active");
        inactiveBg = GetComponentsInChildren<Image>().ToList().Find(p => p.name == "back_inactive");

    }

    protected override void Start()
    {
        base.Start();

        buyButton = FindObjectOfType<BuyButton>();

        Initialize();
    }

    public void RefreshButtonState()
    {
        if (null != scoreDisplayer)
        {
            scoreDisplayer.UpdateLevelInfo();
            scoreDisplayer.UpdateText();
        }

        if (isInitialized && null != DataObjects.GameManager && null != DataObjects.DataManager && null != level && null != DataObjects.IAPManager)
        {
            if (!DataObjects.DataManager.savedGame.IsLevelUnlocked(level.number))
            {
                SetActiveLevelButton(false);
                if (null != starBar)
                {
                    starBar.SetStarsInactive();
                }
            }
            else
            {
                if (DataObjects.GameManager.purchaseMode == true)
                {
                    if (GameManager.lastFreeLevelNumber < level.number)
                    {
                        if (!DataObjects.IAPManager.AreLevelsPurchased())
                        {
                            SetActiveLevelButton(false);
                            starBar.SetStarsInactive();
                            ToggleBuyButton(true);
                            return;
                        }
                    }
                }

                SetActiveLevelButton(true);

                if (null != starBar)
                {
                    //Debug.Log(level.StoredStarRecord);
                    //Debug.Log(level.bestScoreRecord);
                    starBar.FillStarsNoAnimation(level.StoredStarRecord);
                }
            }
        }
    }

    private void ToggleBuyButton(bool v)
    {
        if (null != buyButton)
        {
            buyButton.Toggle(v);
        }
    }

    private void SetActiveLevelButton(bool v)
    {
        isActiveButton = v;

        if (null != lockImage)
            lockImage.gameObject.SetActive(!v);

        if (null != scoreDisplayer)
            scoreDisplayer.gameObject.SetActive(v);

        if (null != activeBg)
            activeBg.enabled = v;

        if (null != inactiveBg)
            inactiveBg.enabled = !v;

        if (!v)
        {
            if (null != text)
            {
                text.color = text.color / 2f;
            }
        }
    }

    protected override void Function()
    {
        if (null == level)
        {
            Debug.LogError("No such level: " + scenePath);
            return;
        }

        base.Function();
    }

    public void Reinitialize()
    {
        Initialize();
    }
}
