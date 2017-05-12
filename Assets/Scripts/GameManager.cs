using System;
using System.Collections;
using System.IO;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public enum VRMode { none, GoogleVR_Daydream, GoogleVR_Cardboard, Oculus };

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

    private GraphicsConfigurator gConf = null;

    public const int lastFreeLevelNumber = 1;
    public bool purchaseMode = true;

    void Awake()
    {
        SAVE_PATH = Application.persistentDataPath + "/save.xml";

#if UNITY_ANDROID && !UNITY_EDITOR
        LEVELS_PATH = "jar:file://" + Application.dataPath + "!/assets/" + levelsName;
#elif UNITY_EDITOR // For running in Unity
        LEVELS_PATH = "file://" + Application.streamingAssetsPath + "/" + levelsName;
#endif

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
    }

    private void ReinitGraphics()
    {
        if (null == gConf)
        {
            gConf = FindObjectOfType<GraphicsConfigurator>();
            if (null == gConf)
            {
                gConf = new GameObject().AddComponent<GraphicsConfigurator>();
                gConf.name = "VRConfigurator";
            }
        }
        else
        {
            gConf.Initialize();
        }
    }
    
    void LateUpdate()
    {

#if UNITY_HAS_GOOGLEVR

        if (GvrController.AppButton)
        {
            PauseTheLevel();
        }


        if (GvrController.HomeButtonDown)
        {
            PauseTheLevel();
        }
#endif
    }

    private static void PauseTheLevel()
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

    private void OnApplicationQuit()
    {
        if (null != dataManager)
        {
            dataManager.Save();
            dataManager = null;
        }
        if (null != iapManager)
        {
            iapManager = null;
        }
        if (null != gameController)
        {
            gameController = null;
        }

        Destroy(gameObject);
    }

    private void OnApplicationPause(bool pause)
    {
        if (pause)
        {
            PauseTheLevel();
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
