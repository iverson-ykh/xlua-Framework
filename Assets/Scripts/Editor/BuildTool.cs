﻿using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEngine;

public class BuildTool : Editor
{
    [MenuItem("Tools/Build Windows Bundle")]
    static void BundleWindowsBuild() {
        Build(BuildTarget.StandaloneWindows);
    }
    [MenuItem("Tools/Build IOS Bundle")]
    static void BundleIOSBuild()
    {
        Build(BuildTarget.iOS);
    }
    [MenuItem("Tools/Build Andriod Bundle")]
    static void BundleAndriodBuild()
    {
        Build(BuildTarget.Android);
    }
    static void Build(BuildTarget target) {


        List<AssetBundleBuild> assetBundleBuilds = new List<AssetBundleBuild>();
        string[] files = Directory.GetFiles(PathUtil.BuildResourcesPath,"*",SearchOption.AllDirectories);
        for (int i=0;i<files.Length;i++) {
            if (files[i].EndsWith(".meta")) { continue; }
            
            AssetBundleBuild assetBundle = new AssetBundleBuild();

            string fileName = PathUtil.GetStandardPath(files[i]);
            Debug.Log("file:" + fileName);
            string assetName = PathUtil.GetUnityPath(fileName);
            assetBundle.assetNames = new string[] { assetName };
            string bundleName = fileName.Replace(PathUtil.BuildResourcesPath,"").ToLower();
            assetBundle.assetBundleName =bundleName+ ".ab";
            assetBundleBuilds.Add(assetBundle);
        }
        if (Directory.Exists(PathUtil.BuildOutPath)) {
            Directory.Delete(PathUtil.BuildOutPath,true);
        }
        Directory.CreateDirectory(PathUtil.BuildOutPath);

        BuildPipeline.BuildAssetBundles(PathUtil.BuildOutPath,assetBundleBuilds.ToArray(),BuildAssetBundleOptions.None,target);
    }

    
}
