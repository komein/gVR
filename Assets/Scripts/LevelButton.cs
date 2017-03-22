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

    LevelInfo level;

    private void Awake()
    {
        starBar = GetComponentInChildren<StarProgressBar>();

        lockImage = transform.parent.GetComponentsInChildren<Image>().ToList().Find(p => p.name == "LockImage");
        scoreDisplayer = transform.parent.GetComponentInChildren<ScoreDisplayer>();

        if (null != scoreDisplayer)
            scoreDisplayer.levelTitle = scenePath;

        if (null != lockImage)
            lockImage.gameObject.SetActive(false);
    }

    protected override void Start()
    {
        base.Start();

        SavedGame game = DataObjects.savedGame;

        if (null != game)
        {
            level = game.GetLevelByName(scenePath);
            buyButton = FindObjectOfType<BuyButton>();
            Initialize();
        }
    }

    public void Initialize()
    {
        if (null != DataObjects.gameManager && null != DataObjects.dataManager && null != level && null != DataObjects.iapManager)
        {
            if (!DataObjects.dataManager.savedGame.isLevelUnlocked(level.number))
            {
                if (null != container)
                    container.gameObject.SetActive(false);

                SetActiveLevelButton(false);
            }
            else
            {
                if (GameManager.purchaseMode == true)
                {
                    if (GameManager.lastFreeLevelNumber < level.number)
                    {
                        if (!DataObjects.iapManager.LevelsArePurchased())
                        {
                            SetActiveLevelButton(false);
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
