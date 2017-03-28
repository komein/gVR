using System;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public enum VRMode { noVR, Daydream, Cardboard, Oculus };

    public VRMode mode;

    //public bool nonVRMode = true;

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

    public static GameManager instanceRef; // singleton pattern

    public InAppManager iapManager;
    public DataManager dataManager;
    public GameController gameController;

    public ResolutionController rcPrefab;

    public const int lastFreeLevelNumber = 1;
    public bool purchaseMode = true;


    void Awake()
    {
        SAVE_PATH = Application.persistentDataPath + "/3.xml";
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

        ResolutionController rc = FindObjectOfType<ResolutionController>();

        if (null == rc)
        {
            rc = Instantiate(rcPrefab);
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
