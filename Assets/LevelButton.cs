using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelButton : SceneButton
{
    public int levelNumber;

    public LevelButtonContainer container;

    protected override void Start()
    {
        base.Start();

        DataStorage store = FindObjectOfType<DataStorage>();
        if (null != store)
        {
            if (!store.savedGame.isLevelUnlocked(levelNumber))
            {
                if (null != container)
                    container.gameObject.SetActive(false);

                isActiveButton = false;
                text.color = text.color / 2f;
                //gameObject.SetActive(false);
                //isActiveButton = false;
                //text.color = text.color / 2f;
            }
            else
            {
                bool isPurchased = true; // TODO check

                isActiveButton = isPurchased;
                if (!isPurchased)
                {
                    text.color = text.color / 2f;
                }
            }
        }

    }

    protected override void Function()
    {
        DataStorage scoreStorage = FindObjectOfType<DataStorage>();
        if (null != scoreStorage)
        {
            scoreStorage.Save();
            scoreStorage.levelInfo.title = scenePath;
            scoreStorage.levelInfo.number = levelNumber;
        }

        base.Function();
    }
}
