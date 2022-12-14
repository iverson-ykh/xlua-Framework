using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using UObject = UnityEngine.Object;
using System;

public class ResourceManager : MonoBehaviour
{
    internal class BundleInfo {
        public string AssetName;
        public string BundleName;
        public List<string> Dependences;
    }
    private Dictionary<string, BundleInfo> m_BundleInfos = new Dictionary<string, BundleInfo>();
    private void ParseVersionFile() {
        string url = Path.Combine(PathUtil.BuildOutPath,AppConst.FileListName);
        string[] data = File.ReadAllLines(url);
        for (int i=0;i<data.Length;i++) {
            BundleInfo bundleInfo = new BundleInfo();
            string[] info = data[i].Split('|');
            bundleInfo.AssetName = info[0];
            bundleInfo.BundleName = info[1];
            bundleInfo.Dependences = new List<string>(info.Length-2);
            for (int j=2;j<info.Length;j++) {
                bundleInfo.Dependences.Add(info[j]);
            }
            m_BundleInfos.Add(bundleInfo.AssetName,bundleInfo);
        }
        
        
    }
    IEnumerator LoadBundleAsync(string assetName, Action<UObject> action = null)
    {
        string bundleName = m_BundleInfos[assetName].BundleName;
        string bundlePath = Path.Combine(PathUtil.BuildOutPath, bundleName);
        List<string> dependences = m_BundleInfos[assetName].Dependences;
        if (dependences != null && dependences.Count > 0)
        {
            for (int i = 0; i < dependences.Count; i++)
            {
                yield return LoadBundleAsync(dependences[i]);

            }
        }
        AssetBundleCreateRequest request = AssetBundle.LoadFromFileAsync(bundlePath);
        yield return request;

        AssetBundleRequest bundleRequest = request.assetBundle.LoadAssetAsync(assetName);
        yield return bundleRequest;
        Debug.Log("this is PackageBundle");
        action?.Invoke(bundleRequest?.asset);

    }
    void EditorLoadAsset(string assetName,Action<UObject>action=null) {
        Debug.Log("this is editor");
        UObject obj = UnityEditor.AssetDatabase.LoadAssetAtPath(assetName,typeof(UObject));
        if (obj==null) {
            Debug.LogError("assets name is not exist:"+assetName);
        }
        action? .Invoke(obj);
    }
    private void LoadAsset(string assetName, Action<UObject> action)
    {
        if (AppConst.GameMode == GameMode.EditorMode)
        {
            EditorLoadAsset(assetName, action);
        }
        else {
            StartCoroutine(LoadBundleAsync(assetName, action));
        }
        
    }

    public void LoadUI(string name,Action<UObject>action=null) {
        LoadAsset(PathUtil.GetUIPath(name),action);
    }

    void Start()
    {
        ParseVersionFile();
        //string resource = PathUtil.GetUIPath();
        LoadUI("Login/LoginUI",OnComplete);
    }
    private void OnComplete(UObject obj) {
        GameObject go = Instantiate(obj) as GameObject;
        go.transform.SetParent(this.transform);
        go.SetActive(true);
        go.transform.localPosition = Vector3.zero;
    }
}
