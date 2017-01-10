using System.Collections;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Text))]
public class ScoreDisplayer : MonoBehaviour
{
    private const string DISPLAY_TEXT_FORMAT = "{0}/{1}\n{2}hp";
    private const string DISPLAY_TEXT_FORMAT_MAXED = "{0}(MAXED)\n{1}hp";
    private Text textField;
    public Camera cam;

    DataStorage storage;

    int lvl;

    bool deadFlag;
    bool showingCongratulation = false;

    void Awake()
    {
        textField = GetComponent<Text>();
    }

    void Start()
    {
        deadFlag = false;
        showingCongratulation = false;

        storage = FindObjectOfType<DataStorage>();

        if (cam == null)
        {
            cam = Camera.main;
        }

        if (cam != null)
        {
            transform.SetParent(cam.GetComponent<Transform>(), true);
        }

        if (null != storage)
        {
            storage.Load();
            lvl = storage.GetCurrentLevel();
            storage.SetOptionalAction(UpdateText);
            storage.Load();
        }
    }

    public void UpdateText()
    {
        if (deadFlag)
            return;

        if (showingCongratulation)
            return;

        if (null != storage)
        {
            long score = storage.GetScore();
            int hp = storage.GetHp();

            if (hp <= 0)
            {
                deadFlag = true;
                textField.text = "Kitty needs rest!\nReturning to menu..";
                return;
            }

            int newLvl = storage.GetCurrentLevel();

            if (newLvl > lvl)
            {
                StopAllCoroutines();
                StartCoroutine(ShowCongratulation());
                lvl = newLvl;
                return;
            }

            lvl = newLvl;

            if (lvl > 2)
            {
                textField.text = string.Format(DISPLAY_TEXT_FORMAT_MAXED, score, hp);
                return;
            }

            long toNextLvl = storage.GetLvlUnlock();

            textField.text = string.Format(DISPLAY_TEXT_FORMAT, score, toNextLvl, hp);
        }
    }

    private IEnumerator ShowCongratulation()
    {
        showingCongratulation = true;
        textField.text = "You gained access to a new level!";

        yield return new WaitForSeconds(2f);
       
        showingCongratulation = false;
        UpdateText();

        yield return null;
    }

}
