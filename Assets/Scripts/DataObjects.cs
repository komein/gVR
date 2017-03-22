﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class DataObjects
{
    public static GameManager gameManager
    {
        get
        {
            return GameObject.FindObjectOfType<GameManager>();
        }
    }

    public static DataManager dataManager
    {
        get
        {
            if (gameManager == null)
            {
                return null;
            }

            return gameManager.dataManager;
        }
    }

    public static GameController gameController
    {
        get
        {
            if (gameManager == null)
            {
                return null;
            }

            return gameManager.gameController;
        }
    }

    public static InAppManager iapManager
    {
        get
        {
            if (gameManager == null)
            {
                return null;
            }

            return gameManager.iapManager;
        }
    }

    public static SavedGame savedGame
    {
        get
        {
            if (gameManager == null)
            {
                return null;
            }

            if (null == dataManager)
            {
                return null;
            }

            return dataManager.savedGame;
        }
    }

    public static SceneInfo sceneInfo
    {
        get
        {
            if (gameManager == null)
            {
                return null;
            }

            if (null == dataManager)
            {
                return null;
            }

            return dataManager.sceneInfo;
        }
    }

}