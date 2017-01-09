using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using UnityEngine;
using UnityEngine.UI;

[System.Serializable]
public class Game
{
    public long score;

    public Game(long s)
    {
        score = s;
    }
}

[RequireComponent(typeof(Text))]
public class ScoreStorage : MonoBehaviour
{
    private const string DISPLAY_TEXT_FORMAT = "{0}/{1}";
    private Text textField;
    public Camera cam;

    private long score = 0;
    long lvl2Unlock = 100;
    long lvl3Unlock = 250;

    long nextUnlock;

    void Awake()
    {
        DontDestroyOnLoad(transform.gameObject);
        textField = GetComponent<Text>();
    }

    void Start()
    {
        if (cam == null)
        {
            cam = Camera.main;
        }

        if (cam != null)
        {
            transform.SetParent(cam.GetComponent<Transform>(), true);
        }

        if (score > lvl2Unlock)
            nextUnlock = lvl3Unlock;
        else
            nextUnlock = lvl2Unlock;

        Load();

        UpdateScore(0);
    }

    public void UpdateScore(int amount)
    {
        score += amount;

        if (score >= nextUnlock)
        {
            textField.color = Color.green;
        }
        else
        {
            textField.color = Color.yellow;
        }
        textField.text = string.Format(DISPLAY_TEXT_FORMAT,
            score, nextUnlock);
    }

    public void Save()
    {
        Game g = new Game(score);
        System.Runtime.Serialization.Formatters.Binary.BinaryFormatter bf = new BinaryFormatter();
        System.IO.FileStream file = File.Create(Application.persistentDataPath + "/savedGames.gd");
        bf.Serialize(file, g);
        file.Close();
    }

    public void Load()
    {
        if (File.Exists(Application.persistentDataPath + "/savedGames.gd"))
        {
            BinaryFormatter bf = new BinaryFormatter();
            FileStream file = File.Open(Application.persistentDataPath + "/savedGames.gd", FileMode.Open);
            Game g = (Game)bf.Deserialize(file);
            file.Close();

            score = g.score;
        }
    }
    void OnApplicationQuit()
    {
        Save();
    }
}
