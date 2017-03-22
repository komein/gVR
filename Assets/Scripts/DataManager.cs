using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
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
        string savePath = DataObjects.gameManager.SAVE_PATH;

        if (null == savedGame)
            return;

        XmlSerializer serializer = new XmlSerializer(typeof(SavedGame));
        FileStream stream = new FileStream(savePath, FileMode.Create);

        try
        {
            serializer.Serialize(stream, savedGame);
        }
        catch (System.InvalidOperationException e)
        {
            Debug.LogError(e.Message);
        }

        stream.Close();
    }

    public bool LoadWithoutAction()
    {
        string savePath = DataObjects.gameManager.SAVE_PATH;

        if (File.Exists(savePath))
        {
            XmlSerializer serializer = new XmlSerializer(typeof(SavedGame));
            FileStream stream = new FileStream(savePath, FileMode.Open);
            try
            {
                savedGame = serializer.Deserialize(stream) as SavedGame;
            }
            catch (InvalidOperationException e)
            {
                Debug.LogError(e.Message);
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
        string levelsPath = DataObjects.gameManager.LEVELS_PATH;

        if (File.Exists(levelsPath))
        {
            XmlSerializer serializer = new XmlSerializer(typeof(SavedGame));
            FileStream stream = new FileStream(levelsPath, FileMode.Open);
            try
            {
                savedGame = serializer.Deserialize(stream) as SavedGame;
            }
            catch (InvalidOperationException e)
            {
                Debug.LogError(e.Message);
                GenerateDefaultLevels();
            }
            stream.Close();
        }
        else
        {
            Debug.LogWarning("Levels config is not found.");
            GenerateDefaultLevels();
        }

        Save();
        LoadWithoutAction();
    }

    private void GenerateDefaultLevels()
    {
        Debug.LogWarning("Using deprecated level generate values. That is wrong, check Resources/level.xml.");
        List<LevelInfo> levels = new List<LevelInfo>();

        levels.Add(new LevelInfo(1, 50, 0.2f, 0.5f, 1f));
        levels.Add(new LevelInfo(2, 60, 0.3f, 0.8f, 1.5f));
        levels.Add(new LevelInfo(3, 70, 0.5f, 1f, 2f));

        savedGame = new SavedGame(levels);
    }

    public void Load()
    {
        if (LoadWithoutAction())
        {
            if (null != DataObjects.gameController)
            {
                DataObjects.gameController.TriggerOptionalScoreAction();
            }
        }
    }
}
