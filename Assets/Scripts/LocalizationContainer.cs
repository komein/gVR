using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public enum LanguageEnum { English, Chinese };

[Serializable]
public class LocalizationUnit
{
    public LanguageEnum language;
    public string fieldValue;
}

[Serializable]
public class LocalizationField
{
    public string id;
    public List<LocalizationUnit> locals;
}

public class LocalizationContainer : MonoBehaviour
{
    public List<LocalizationField> localizationStorage;

    public LanguageEnum currentLanguage;
    public LanguageEnum CurrentLanguage
    {
        get
        {
            return currentLanguage;
        }
        private set
        {
            currentLanguage = value;
        }
    }
    public Action onLanguageChanged;

    public void SetLanguage(LanguageEnum toSet)
    {
        CurrentLanguage = toSet;

        DataObjects.SavedGame.SetLanguage(currentLanguage);
        DataObjects.DataManager.Save();

        // broadcast here
        foreach(var v in FindObjectsOfType<Canvas>())
        {
            v.BroadcastMessage("UpdateText", SendMessageOptions.DontRequireReceiver);
        }
    }

    public string GetField(string id, LanguageEnum language)
    {
        LocalizationField field;
        if ((field = localizationStorage.FirstOrDefault(p => p.id == id)) != null)
        {
            LocalizationUnit unit;
            if ((unit = field.locals.FirstOrDefault(p => p.language == language)) != null)
            {
                return unit.fieldValue;
            }
        }

        return "test";
    }


    public string GetField(string id)
    {
        LocalizationField field;
        if ((field = localizationStorage.FirstOrDefault(p => p.id == id)) != null)
        {
            LocalizationUnit unit;
            if ((unit = field.locals.FirstOrDefault(p => p.language == currentLanguage)) != null)
            {
                return unit.fieldValue;
            }
        }

        return "test";
    }
}
