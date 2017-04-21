using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Xml;
using System.Xml.Serialization;
using UnityEngine;

public class DataManager
{
    public SavedGame savedGame;
    public SceneInfo sceneInfo = new SceneInfo();

    public DataManager()
    {
        savedGame = null;
        sceneInfo = new SceneInfo();
    }

    public void Save()
    {
        string savePath = DataObjects.GameManager.SAVE_PATH;

        if (null == savedGame)
            return;

        XmlSerializer serializer = new XmlSerializer(typeof(SavedGame));
        FileStream stream = new FileStream(savePath, FileMode.Create);

        try
        {
            serializer.Serialize(stream, savedGame);
        }
        catch (Exception e)
        {
            Debug.LogError(e.Message);
        }

        stream.Close();
    }

    public bool LoadWithoutAction()
    {
        string savePath = DataObjects.GameManager.SAVE_PATH;

        if (File.Exists(savePath))
        {
            XmlSerializer serializer = new XmlSerializer(typeof(SavedGame));
            FileStream stream = new FileStream(savePath, FileMode.Open);
            try
            {
                savedGame = serializer.Deserialize(stream) as SavedGame;
            }
            catch (Exception e)
            {
                Debug.LogError(e.Message);
                MakeNewSaveFile();
            }
            stream.Close();
        }
        else
        {
            MakeNewSaveFile();
        }

        return true;
    }

    public void MakeNewSaveFile()
    {
        string levelsPath = DataObjects.GameManager.LEVELS_PATH_SAVED;

        TextAsset t = (TextAsset)Resources.Load("levels");

        if (null != t)
        {
            XmlSerializer serializer = new XmlSerializer(typeof(SavedGame));
            using (var reader = new System.IO.StringReader(t.text))
            {
                try
                {
                    savedGame = serializer.Deserialize(reader) as SavedGame;
                }
                catch (Exception e)
                {
                    Debug.LogError(e.Message);
                    GenerateDefaultLevels();
                }
            }
        }
        else
        {
            Debug.LogWarning("Levels config is not found.");
            GenerateDefaultLevels();
        }
        
        Save();
    }

    private void GenerateDefaultLevels()
    {
        Debug.LogWarning("Using deprecated level generate values. That is wrong, check Resources/level.xml.");
        List<LevelInfo> levels = new List<LevelInfo>();

        levels.Add(new LevelInfo(1, 500, 100, 200, 400));
        levels.Add(new LevelInfo(2, 900, 250, 450, 750));
        levels.Add(new LevelInfo(3, 1200, 450, 800, 1200));

        savedGame = new SavedGame(levels);
    }

    public void Load()
    {
        if (LoadWithoutAction())
        {
            if (null != DataObjects.GameController)
            {
                DataObjects.GameController.TriggerOptionalScoreAction();
            }
        }
    }
}
