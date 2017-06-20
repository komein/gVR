using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public static class DataObjects
{
    public static GameManager GameManager
    {
        get
        {
            return GameManager.instanceRef;
        }
    }
    
    public static GameMusic MusicStorage
    {
        get
        {
            return GameMusic.instanceRef;
        }
    }
    
    public static void SetMusic(string s, AudioSource src)
    {
        if (null == MusicStorage)
        {
            return;
        }

        MusicStorage.SetMusic(s, src);
    }

    public static DataManager DataManager
    {
        get
        {
            if (GameManager == null)
            {
                return null;
            }

            return GameManager.dataManager;
        }
    }

    public static GameController GameController
    {
        get
        {
            if (GameManager == null)
            {
                return null;
            }

            return GameManager.gameController;
        }
    }
    
    public static InAppManager IAPManager
    {
        get
        {
            if (GameManager == null)
            {
                return null;
            }

            return GameManager.iapManager;
        }
    }

    public static SavedGame SavedGame
    {
        get
        {
            if (null == DataManager)
            {
                return null;
            }

            return DataManager.savedGame;
        }
    }

    public static SceneInfo SceneInfo
    {
        get
        {
            if (GameManager == null)
            {
                return null;
            }

            if (null == DataManager)
            {
                return null;
            }

            return DataManager.sceneInfo;
        }
    }

    public static LevelInfo LevelInfo(string s)
    {
        if (null == s)
        {
            return null;
        }

        if (null != SavedGame)
        {
            return SavedGame.GetLevelByName(s);
        }

        return null;
    }

    public static LocalizationContainer Localization
    {
        get
        {
            if (GameManager == null)
            {
                return null;
            }

            return GameManager.Localization;
        }
    }

}
