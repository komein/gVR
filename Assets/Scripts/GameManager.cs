using System;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public enum VRMode { noVR, Daydream, Cardboard, Oculus };

    public VRMode mode;
    public bool noStereoMode
    {
        get
        {

            #if !(UNITY_HAS_GOOGLEVR && (UNITY_ANDROID))
                        return false;
            #endif

            #if !(UNITY_HAS_GOOGLEVR && (UNITY_EDITOR))
                        return true;
            #endif

            return true;
        }
    }

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

    public GraphicsConfigurator rcPrefab;

    public const int lastFreeLevelNumber = 1;
    public bool purchaseMode = true;

    private GraphicsConfigurator gConf;

    public GvrConnectionState controllerState = GvrConnectionState.Error;

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
            Destroy(gameObject);
            return;
        }

        //UnityEditor.PlayerSettings.statusBarHidden = true;
    }

    private void Start()
    {
        controllerState = GvrController.State;
    }

    private void ReinitGraphics()
    {
        if (null == gConf)
        {
            gConf = Instantiate(rcPrefab);
        }
        else
        {
            gConf.Initialize();
        }
    }

    private void UpdateController()
    {
        if (null != FindObjectOfType<GraphicsOverrider>())
        {
            return;
        }

        if (null == gConf)
        {
            gConf = Instantiate(rcPrefab);
        }
        else
        {
            gConf.ReinitGoogleVRController();
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

        if (controllerState != GvrController.State)
        {
            //Debug.Log("switching " + controllerState + " to " + GvrController.State);
            controllerState = GvrController.State;
            UpdateController();
        }
    }

    private void OnApplicationQuit()
    {
        if (null != dataManager)
        {
            dataManager.Save();
        }
    }

    void OnEnable()
    {
        //Tell our 'OnLevelFinishedLoading' function to start listening for a scene change as soon as this script is enabled.
        SceneManager.sceneLoaded += OnLevelFinishedLoading;
    }

    void OnDisable()
    {
        //Tell our 'OnLevelFinishedLoading' function to stop listening for a scene change as soon as this script is disabled. Remember to always have an unsubscription for every delegate you subscribe to!
        SceneManager.sceneLoaded -= OnLevelFinishedLoading;
    }

    void OnLevelFinishedLoading(Scene scene, LoadSceneMode mode)
    {
        Debug.Log("Level Loaded");
        Debug.Log(scene.name);
        Debug.Log(mode);

        ReinitGraphics();
    }
}
