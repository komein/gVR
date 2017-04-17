using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class ScoreCollectible : Collectible {

    public int value = 1;

    List<MeshRenderer> meshes;

    public Vector3 shift = new Vector3(0.15f, 0f, 0f);

    Coroutine currentCoroutine = null;

    public bool IsVisible
    {
        get;
        private set;
    }

    private void Awake()
    {
        IsVisible = true;
    }

    protected override void Start()
    {
        base.Start();

        meshes = GetComponentsInChildren<MeshRenderer>().ToList();
        ShowOneMesh();

        DataObjects.SetMusic("scorePickup", aus);
    }

    public void Split()
    {
        if (null != currentCoroutine)
        {
            StopCoroutine(currentCoroutine);
        }
        currentCoroutine = StartCoroutine(SplitCoroutine());
    }


    public void MakeWhole()
    {
        if (null != currentCoroutine)
        {
            StopCoroutine(currentCoroutine);
        }
        currentCoroutine = StartCoroutine(MakeWholeCoroutine());
    }

    IEnumerator MakeWholeCoroutine()
    {
        if (meshes.Count > 1)
        {
            for (float t = 0; t < 1; t += Time.deltaTime)
            {
                for (int i = 0; i < meshes.Count; i++)
                {
                    meshes[i].transform.localPosition = Vector3.Lerp(meshes[i].transform.localPosition, Vector3.zero, t);
                }
                yield return new WaitForEndOfFrame();
            }

            ShowOneMesh();
        }

        yield return null;
    }

    IEnumerator SplitCoroutine()
    {
        if (meshes.Count > 1)
        {
            ToggleAllMeshes(true);

            for (float t = 0; t < 1; t += Time.deltaTime)
            {
                for (int i = 0; i < meshes.Count; i++)
                {
                    meshes[i].transform.localPosition = Vector3.Lerp(meshes[i].transform.localPosition, shift * ((i % 2) == 0 ? 1 : -1), t);
                }
                yield return new WaitForEndOfFrame();
            }
        }

        yield return null;
    }

    private void ToggleAllMeshes(bool v)
    {
        if (v && !IsVisible)
        {
            return;
        }

        for (int i = 0; i < meshes.Count; i++)
        {
            meshes[i].enabled = v;
        }
    }

    private void ShowOneMesh()
    {
        for (int i = 0; i < meshes.Count; i++)
        {
            meshes[i].enabled = i == 0;
        }
    }

    public override void Collect()
    {
        if (null != DataObjects.gameController)
        {
            if (null != currentCoroutine)
            {
                StopCoroutine(currentCoroutine);
            }
            base.Collect();
            DataObjects.gameController.AddScore(value);
        }
    }

    public override void SetVisible(bool v)
    {
        IsVisible = v;
        ToggleAllMeshes(v);
    }
}
