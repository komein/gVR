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
        container = GetComponentInParent<LevelButtonContainer>();
        scoreDisplayer = GetComponentInChildren<ScoreDisplayer>();

        if (null != scoreDisplayer)
        {
            scoreDisplayer.levelTitle = scenePath;
            scoreDisplayer.UpdateLevelInfo();

            isInitialized = true;
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

        SavedGame game = DataObjects.savedGame;

        if (null != game)
        {
            level = game.GetLevelByName(scenePath);
            buyButton = FindObjectOfType<BuyButton>();
            RefreshButtonState();
        }
    }

    public void RefreshButtonState()
    {
        if (isInitialized && null != DataObjects.gameManager && null != DataObjects.dataManager && null != level && null != DataObjects.iapManager)
        {
            if (!DataObjects.dataManager.savedGame.isLevelUnlocked(level.number))
            {
                //if (null != container)
                //    container.gameObject.SetActive(false);

                SetActiveLevelButton(false);
                starBar.SetStarsInactive();
            }
            else
            {
                if (DataObjects.gameManager.purchaseMode == true)
                {
                    if (GameManager.lastFreeLevelNumber < level.number)
                    {
                        if (!DataObjects.iapManager.AreLevelsPurchased())
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
                    starBar.FillStarsNoAnimation(DataObjects.gameController.GetStarRecord(level.number));
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
        RefreshButtonState();
    }
}
