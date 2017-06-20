using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LanguageChangeButton : LookableButton
{
    public LanguageEnum language;

    protected override void Start()
    {
        base.Start();

        text.text = DataObjects.Localization.GetField(caption, language);
    }

    public override void SetGazedAt(bool gazedAt)
    {
        if (DataObjects.Localization.currentLanguage == language)
        {
            img.fillAmount = 1f;
            return;
        }
        base.SetGazedAt(gazedAt);
    }

    protected override void Function()
    {
        if (DataObjects.Localization.currentLanguage == language)
        {
            img.fillAmount = 1f;
            return;
        }

        DataObjects.Localization.SetLanguage(language);

        base.Function();
    }

    public override void UpdateText()
    {
        if (null != text)
        {
            text.text = DataObjects.Localization.GetField(caption, language);
        }
        SetGazedAt(false);
    }
}
