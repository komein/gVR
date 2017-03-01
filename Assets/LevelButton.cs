using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class LevelButton : SceneButton
{

    public LevelButtonContainer container;
    public BuyButton buyButton;

    private StarProgressBar starBar;
    private ScoreDisplayer2 scoreDisplayer;
    GameObject lockImage;

    LevelInfo level;
    DataStorage storage;

    private void Awake()
    {
        starBar = GetComponentInChildren<StarProgressBar>();


        lockImage = transform.parent.GetComponentsInChildren<Canvas>().ToList().Find(p => p.name == "LockImage").gameObject;
        scoreDisplayer = transform.parent.GetComponentsInChildren<Canvas>().ToList().Find(p => p.name == "ScoreDisplayer").GetComponent<ScoreDisplayer2>();

        lockImage.SetActive(false);

    }

    protected override void Start()
    {
        base.Start();

        level = FindObjectOfType<DataStorage>().savedGame.GetLevelByName(scenePath);
        storage = FindObjectOfType<DataStorage>();

        Initialize();
    }

    public void Initialize()
    {
        if (null != storage && null != level)
        {
            if (!storage.savedGame.isLevelUnlocked(level.number))
            {
                if (null != container)
                    container.gameObject.SetActive(false);

                SetActiveLevelButton(false);
            }
            else
            {
                if (DataStorage.purchaseMode == true)
                {
                    if (DataStorage.lastFreeLevelNumber < level.number)
                    {
                        if (!storage.LevelsArePurchased())
                        {
                            SetActiveLevelButton(false);
                            ToggleBuyButton(true);
                            return;
                        }
                    }
                }

                SetActiveLevelButton(true);

                starBar.FillStarsNoAnimation((int)storage.GetStarRecord(level.number));
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
        lockImage.SetActive(!v);
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
        DataStorage scoreStorage = FindObjectOfType<DataStorage>();
        if (null != scoreStorage)
        {
            if (null != level)
            {
                scoreStorage.SetMultiplier(1);
            }
            else
            {
                Debug.LogError("No such level: " + scenePath);
            }
        }

        base.Function();
    }
}
