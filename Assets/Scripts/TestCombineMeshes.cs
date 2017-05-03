using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using UnityEngine;

#if UNITY_EDITOR
using UnityEditor;
public class TestCombineMeshes : MonoBehaviour
{

    List<MeshFilter> meshFilters;
    MeshFilter meshFilterCombine;
    MeshRenderer meshRendererCombine;

    public void Combine()
    {
        ArrayList materials = new ArrayList();
        ArrayList combineInstanceArrays = new ArrayList();

        GetMeshFilters();

        foreach (MeshFilter meshFilter in meshFilters)
        {

            MeshRenderer meshRenderer = meshFilter.GetComponent<MeshRenderer>();

            if (!meshRenderer ||
                !meshFilter.sharedMesh ||
                meshRenderer.sharedMaterials.Length != meshFilter.sharedMesh.subMeshCount)
            {
                continue;
            }

            for (int s = 0; s < meshFilter.sharedMesh.subMeshCount; s++)
            {
                int materialArrayIndex = ContainsMaterial(materials, meshRenderer.sharedMaterials[s].name);
                if (materialArrayIndex == -1)
                {
                    materials.Add(meshRenderer.sharedMaterials[s]);
                    materialArrayIndex = materials.Count - 1;
                }
                combineInstanceArrays.Add(new ArrayList());

                CombineInstance combineInstance = new CombineInstance();
                combineInstance.transform = meshRenderer.transform.localToWorldMatrix;
                combineInstance.subMeshIndex = s;
                combineInstance.mesh = meshFilter.sharedMesh;
                (combineInstanceArrays[materialArrayIndex] as ArrayList).Add(combineInstance);
            }
        }

        HideMeshes();

        // Get / Create mesh filter & renderer
        meshFilterCombine = gameObject.GetComponent<MeshFilter>();
        if (meshFilterCombine == null)
        {
            meshFilterCombine = gameObject.AddComponent<MeshFilter>();
        }
        meshRendererCombine = gameObject.GetComponent<MeshRenderer>();
        if (meshRendererCombine == null)
        {
            meshRendererCombine = gameObject.AddComponent<MeshRenderer>();
        }

        // Combine by material index into per-material meshes
        // also, Create CombineInstance array for next step
        Mesh[] meshes = new Mesh[materials.Count];
        CombineInstance[] combineInstances = new CombineInstance[materials.Count];

        for (int m = 0; m < materials.Count; m++)
        {
            CombineInstance[] combineInstanceArray = (combineInstanceArrays[m] as ArrayList).ToArray(typeof(CombineInstance)) as CombineInstance[];
            meshes[m] = new Mesh();
            meshes[m].CombineMeshes(combineInstanceArray, true, true);

            combineInstances[m] = new CombineInstance();
            combineInstances[m].mesh = meshes[m];
            combineInstances[m].subMeshIndex = 0;
        }

        // Combine into one
        meshFilterCombine.sharedMesh = new Mesh();
        meshFilterCombine.sharedMesh.CombineMeshes(combineInstances, false, false);


        // Destroy other meshes
        foreach (Mesh oldMesh in meshes)
        {
            oldMesh.Clear();
            DestroyImmediate(oldMesh);
        }

        // Assign materials
        Material[] materialsArray = materials.ToArray(typeof(Material)) as Material[];
        meshRendererCombine.materials = materialsArray;

        MeshUtility.Optimize(meshFilterCombine.sharedMesh);

        Unwrapping.GenerateSecondaryUVSet(meshFilterCombine.sharedMesh);

    }

    private void GetMeshFilters()
    {
        meshFilters = gameObject.GetComponentsInChildren<MeshFilter>().ToList();

        if (null != GetComponent<MeshFilter>())
        {
            meshFilters.Remove(GetComponent<MeshFilter>());
        }
    }

    public void RemoveChildrenMeshesColliders()
    {
        foreach (var v in meshFilters)
        {
            Collider[] colliders = v.GetComponents<Collider>();

            foreach (var c in colliders)
            {
                if (!c.isTrigger)
                {
                    c.enabled = false;
                }
            }
        }
    }


    public void TurnOnChildrenMeshesColliders()
    {
        foreach (var v in meshFilters)
        {
            Collider[] colliders = v.GetComponents<Collider>();

            foreach (var c in colliders)
            {
                if (!c.isTrigger)
                {
                    c.enabled = true;
                }
            }
        }
    }

    public void HideMeshFilters()
    {
        foreach (MeshFilter meshFilter in meshFilters)
        {
            meshFilter.gameObject.SetActive(false);
            //DestroyImmediate(meshFilter);
        }
    }

    public void ShowMeshFilters()
    {
        foreach (MeshFilter meshFilter in meshFilters)
        {
            meshFilter.gameObject.SetActive(true);
            //DestroyImmediate(meshFilter);
        }
    }

    public void HideMeshes()
    {
        foreach (MeshFilter meshFilter in meshFilters)
        {
            MeshRenderer r = meshFilter.GetComponent<MeshRenderer>();
            if (null != r)
            {
                if (r != GetComponent<MeshRenderer>())
                    r.enabled = false;
            }
            //DestroyImmediate(meshFilter);
        }
    }


    public void DestroyMeshes()
    {
        foreach (MeshFilter meshFilter in meshFilters)
        {
            MeshRenderer r = meshFilter.GetComponent<MeshRenderer>();
            DestroyImmediate(meshFilter.gameObject);
        }
    }

    public void ShowMeshes()
    {
        foreach (MeshFilter meshFilter in meshFilters)
        {
            MeshRenderer r = meshFilter.GetComponent<MeshRenderer>();
            if (null != r)
            {
                if (r != GetComponent<MeshRenderer>())
                    r.enabled = true;
            }
            //DestroyImmediate(meshFilter);
        }
    }

    public void SaveAsset()
    {
        string path = EditorUtility.SaveFilePanel("Save Separate Mesh Asset", "Assets/", name, "asset");
        if (string.IsNullOrEmpty(path)) return;

        path = FileUtil.GetProjectRelativePath(path);

        meshFilterCombine = GetComponent<MeshFilter>();
        if (null == meshFilterCombine)
            return;

        Mesh meshToSave = meshFilterCombine.sharedMesh;

        AssetDatabase.CreateAsset(meshToSave, path);
        AssetDatabase.SaveAssets();
    }

    public string MeshToString(MeshFilter mf, MeshRenderer mr)
    {

        if (null == mf || null == mr)
            return null;

        Mesh m = mf.mesh;
        Material[] mats = mr.sharedMaterials;

        StringBuilder sb = new StringBuilder();

        sb.Append("g ").Append(mf.name).Append("\n");
        foreach (Vector3 v in m.vertices)
        {
            sb.Append(string.Format("v {0} {1} {2}\n", v.x, v.y, v.z));
        }
        sb.Append("\n");
        foreach (Vector3 v in m.normals)
        {
            sb.Append(string.Format("vn {0} {1} {2}\n", v.x, v.y, v.z));
        }
        sb.Append("\n");
        foreach (Vector3 v in m.uv)
        {
            sb.Append(string.Format("vt {0} {1}\n", v.x, v.y));
        }
        for (int material = 0; material < m.subMeshCount; material++)
        {
            sb.Append("\n");
            sb.Append("usemtl ").Append(mats[material].name).Append("\n");
            sb.Append("usemap ").Append(mats[material].name).Append("\n");

            int[] triangles = m.GetTriangles(material);
            for (int i = 0; i < triangles.Length; i += 3)
            {
                sb.Append(string.Format("f {0}/{0}/{0} {1}/{1}/{1} {2}/{2}/{2}\n",
                    triangles[i] + 1, triangles[i + 1] + 1, triangles[i + 2] + 1));
            }
        }
        return sb.ToString();
    }

    public void MeshToFile(MeshFilter mf, MeshRenderer mr, string filename)
    {
        using (StreamWriter sw = new StreamWriter(filename))
        {
            sw.Write(MeshToString(mf, mr));
        }
    }

    private int ContainsMaterial(ArrayList searchList, string searchName)
    {
        for (int i = 0; i < searchList.Count; i++)
        {
            if (((Material)searchList[i]).name == searchName)
            {
                return i;
            }
        }
        return -1;
    }
}

#endif