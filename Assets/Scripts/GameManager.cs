using System;
using System.Collections;
using System.IO;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public enum VRMode { noVR, Daydream, Cardboard, Oculus };

    public VRMode mode;
    public static bool StereoMode
    {
        get
        {
#if UNITY_HAS_GOOGLEVR

#if UNITY_EDITOR
            return false;
#elif UNITY_ANDROID
             return true;
#endif

#else
            return false;
#endif
        }
    }

    public static bool HaveGoogleVR
    {
        get
        {
#if UNITY_HAS_GOOGLEVR
            return true;
#else
            return false;
#endif
        }
    }

    public static bool IsEditorMode
    {
        get
        {
#if UNITY_EDITOR
            return true;
#else
            return false;
#endif
        }
    }

    WWW www;
    string levelsName = "levels.xml";

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

    public string LEVELS_PATH_SAVED
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

#if UNITY_HAS_GOOGLEVR
    public GvrConnectionState controllerState = GvrConnectionState.Error;
#endif

    void Awake()
    {
        SAVE_PATH = Application.persistentDataPath + "/save.xml";

#if UNITY_ANDROID && !UNITY_EDITOR
        LEVELS_PATH = "jar:file://" + Application.dataPath + "!/assets/" + levelsName;
#elif UNITY_EDITOR // For running in Unity
        LEVELS_PATH = "file://" + Application.streamingAssetsPath + "/" + levelsName;
#endif

        //Debug.Log("save path = " + SAVE_PATH);
        //Debug.Log("save path = " + LEVELS_PATH);

        if (instanceRef == null)
        {
            instanceRef = this;
            DontDestroyOnLoad(gameObject);

            iapManager = new InAppManager();
            dataManager = new DataManager();
            gameController = new GameController();

            dataManager.sceneInfo.ResetLevel();
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
#if UNITY_HAS_GOOGLEVR
        controllerState = GvrController.State;
#endif
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
#if UNITY_HAS_GOOGLEVR
            gConf.ReinitGoogleVRController();
#endif
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

#if UNITY_HAS_GOOGLEVR
        if (controllerState != GvrController.State)
        {
            if (controllerState == GvrConnectionState.Connected && GvrController.State != GvrConnectionState.Connected ||
                controllerState != GvrConnectionState.Connected && GvrController.State == GvrConnectionState.Connected)
            {
                //Debug.Log("switching " + controllerState + " to " + GvrController.State);
                controllerState = GvrController.State;
                UpdateController();
            }
        }

        if (GvrController.AppButton)
        {
            PlayerController p = FindObjectOfType<PlayerController>();
            if (null != p)
            {
                // assuming we are in some level
                p.PauseLevel(PauseType.pause, DataObjects.LevelInfo(SceneManager.GetActiveScene().name));
            }
            else
            {
                // assuming we are in main menu
            }
        }
#endif
    }

    private void OnApplicationQuit()
    {
        if (null != dataManager)
        {
            DataObjects.GameController.OnSceneChange();
            dataManager.Save();
        }
    }

    void OnEnable()
    {
        //Tell our 'OnLevelFinishedLoading' function to start listening for a scene change as soon as this script is enabled.
        SceneManager.sceneLoaded += OnLevelFinishedLoading;
    }

    internal void PauseLevel(PauseType reason)
    {
        PlayerController cat = FindObjectOfType<PlayerController>();
        if (null != cat)
        {
            cat.PauseLevel(reason, DataObjects.LevelInfo(SceneManager.GetActiveScene().name));
        }
    }

    void OnDisable()
    {
        //Tell our 'OnLevelFinishedLoading' function to stop listening for a scene change as soon as this script is disabled. Remember to always have an unsubscription for every delegate you subscribe to!
        SceneManager.sceneLoaded -= OnLevelFinishedLoading;
    }

    void OnLevelFinishedLoading(Scene scene, LoadSceneMode mode)
    {
        ReinitGraphics();
    }

    IEnumerator Downloader()
    {
        www = new WWW(LEVELS_PATH);
        yield return www; //will wait until the download finishes
        if (www.isDone == true)
        {
            File.WriteAllBytes(LEVELS_PATH_SAVED, www.bytes);
        }
    }
}
