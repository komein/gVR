using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public enum LanguageEnum { English, Chinese };

[Serializable]
public class LocalizationUnit
{
    public LanguageEnum language;
    public string fieldValue;
}

[Serializable]
public class FontUnit
{
    public LanguageEnum language;
    public Font font;
}

[Serializable]
public class LocalizationField
{
    public string id;
    public List<LocalizationUnit> locals;
}

public class LocalizationContainer : MonoBehaviour
{
    public float chineseFontSizeMultiplier;

    public List<FontUnit> fontsStorage;
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

        foreach (var v in FindObjectsOfType<Canvas>())
        {
            v.BroadcastMessage("UpdateFont", SendMessageOptions.DontRequireReceiver);
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

        return "TEST";
    }

    public void SetFont(LanguageEnum language, Text view, int initialSize)
    {
        FontUnit font;
        if ((font = fontsStorage.FirstOrDefault(p => p.language == language)) != null)
        {
            if (null != view)
            {
                view.font = font.font;

                switch (language)
                {
                    case LanguageEnum.English:
                        //view.fontSize = initialSize;
                        break;
                    case LanguageEnum.Chinese:
                        //view.fontSize = (int)(chineseFontSizeMultiplier * initialSize);
                        break;
                }
            }
        }
    }

    public void SetFont(Text view, int initialSize)
    {
        SetFont(CurrentLanguage, view, initialSize);
    }


    public string GetField(string id)
    {
        return GetField(id, CurrentLanguage);
    }
}