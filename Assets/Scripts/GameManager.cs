using System;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public string SAVE_PATH
    {
        get;
        private set;
    }

    public string LEVELS_PATH
    {
        get;
        private set;
    }

    private static GameManager instanceRef; // singleton pattern

    public InAppManager iapManager;
    public DataManager dataManager;
    public GameController gameController;

    public const int lastFreeLevelNumber = 1;
    public const bool purchaseMode = true;


    void Awake()
    {
        SAVE_PATH = Application.persistentDataPath + "/save.xml";
        LEVELS_PATH = Application.dataPath + "/Resources/levels.xml";

        if (instanceRef == null)
        {
            instanceRef = this;
            DontDestroyOnLoad(gameObject);

            iapManager = new InAppManager();
            dataManager = new DataManager();
            gameController = new GameController();

            dataManager.sceneInfo.Reset();
            dataManager.LoadWithoutAction();
        }
        else
        {
            DestroyImmediate(gameObject);
        }
    }
    
    void LateUpdate()
    {
        if (null != GvrViewer.Instance)
        {
            GvrViewer.Instance.UpdateState();
            if (GvrViewer.Instance.BackButtonPressed)
            {
                Application.Quit();
            }
        }
    }

    private void OnApplicationQuit()
    {
        if (null != dataManager)
            dataManager.Save();
    }
}
