using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelButton : SceneButton
{
    public int levelNumber;

    public LevelButtonContainer container;
    public BuyButton buyButton;

    protected override void Start()
    {
        base.Start();

        StartCoroutine(InitCoroutine());
    }

    IEnumerator InitCoroutine()
    {
        int i = 0;

        InAppManager manager = FindObjectOfType<InAppManager>();
        while (null == manager && i++ < 1000)
        {
            yield return new WaitForSeconds(0.1f);
        }

        i = 0;

        if (i < 1000)
            while (!manager.readyFlag && i++ < 1000)
            {
                yield return new WaitForSeconds(0.1f);
            }
        else
            Debug.Log("WTF");

        if (i < 1000)
            Initialize();

        yield return null;
    }

    public void Initialize()
    {
        DataStorage store = FindObjectOfType<DataStorage>();
        if (null != store)
        {
            if (!store.savedGame.isLevelUnlocked(levelNumber))
            {
                if (null != container)
                    container.gameObject.SetActive(false);

                SetActiveLevelButton(false);
            }
            else
            {
                if (DataStorage.purchaseMode == true)
                {
                    if (DataStorage.lastFreeLevelNumber < levelNumber)
                    {
                        if (!store.LevelsArePurchased())
                        {
                            SetActiveLevelButton(false);
                            ToggleBuyButton(true);
                            return;
                        }
                    }
                }

                SetActiveLevelButton(true);
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
        if (!v)
            if (null != text)
                text.color = text.color / 2f;
    }

    protected override void Function()
    {
        DataStorage scoreStorage = FindObjectOfType<DataStorage>();
        if (null != scoreStorage)
        {
            scoreStorage.SetMultiplier(1);
            scoreStorage.Save();
            scoreStorage.levelInfo.title = scenePath;
            scoreStorage.levelInfo.number = levelNumber;
        }

        base.Function();
    }
}
