using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using HybridCLR;
using UnityEngine;
using UnityEngine.Networking;

public class HotUpdate : MonoBehaviour
{
    private static bool gameInit = false;
    private static Dictionary<string, byte[]> s_assetDatas = new Dictionary<string, byte[]>();
    private static Dictionary<string, Assembly> assemblies = new();


    private static List<string> AOTMetaAssemblyFiles { get; } = new List<string>()
    {
		"LitJSON.dll",
		"System.Core.dll",
		"UnityEngine.CoreModule.dll",
		"mscorlib.dll",
    };

    private static List<string> assemblyFiles { get; } = new List<string>()
    {
        "QZSXFW.dll",
        "Config.dll",
        "CustomEvent.dll",
        "Tools.dll",
        "DataManager.dll",
        "Role.dll",
        "Factory.dll",
        "CustomUI.dll",
        "Logic.dll",
    };

    void Start()
    {
        if (!gameInit)
        {
            DontDestroyOnLoad(this.gameObject);
            if (Application.platform == RuntimePlatform.WindowsEditor)
            {
                StartGame();
            }
            else
            {
                StartCoroutine(DownLoadAssets(this.StartGame));

            }
        }
    }

    public void StartGame()
    {
        if (Application.platform != RuntimePlatform.WindowsEditor)
        {
            LoadMetadataForAOTAssemblies();
            // 开始游戏前先加载dll  暂时没理解为AOT补充元数据和泛型相关的问题, 所以暂时忽略对其的处理
            foreach (var item in assemblyFiles)
            {
                var assPath = Path.Combine(AppConst.hotSavePath, item + ".bytes");

                Assembly ass = Assembly.Load(File.ReadAllBytes(assPath));
                assemblies.Add(item, ass);
                Debug.Log($"Load: {assPath}");
            }
        }

        Debug.Log("热更处理完成, 加载预制体启动游戏逻辑");
        gameInit = true;

        // GameObject.Instantiate(Resources.Load("GameManager") as GameObject);
        // GameObject go = new GameObject();
        // Singleton<AssetManager>.Create(go.AddComponent<AssetManager>());
        // go.name = "AssetManager";
        // DontDestroyOnLoad(go);
        // AssetBundle assetBundle = AssetBundle.LoadFromFile(Application.persistentDataPath + "/data/test");
        var go = GameObject.Instantiate(Resources.Load("GameManager") as GameObject);
        

    }

    //  IEnumerator DownLoadAssets(Action onDownloadComplete)
    IEnumerator DownLoadAssets(Action onDownloadComplete)
    {
        var assets = new List<string>
        {

        }.Concat(assemblyFiles).Concat(AOTMetaAssemblyFiles);
        // 遍历获取需要下载的资源
        foreach (var asset in assets)
        {
            string dllPath = GetWebRequestPath(asset + ".bytes");
            Debug.Log($"start download asset:{dllPath}");
            UnityWebRequest www = UnityWebRequest.Get(dllPath);
            yield return www.SendWebRequest();

#if UNITY_2020_1_OR_NEWER
            if (www.result != UnityWebRequest.Result.Success)
            {
                Debug.Log(www.error);
            }
#else
            if (www.isHttpError || www.isNetworkError)
            {
                Debug.Log(www.error);
            }
#endif
            else
            {
                byte[] assetData = www.downloadHandler.data;
                Debug.Log($"dll:{asset}  size:{assetData.Length}");
                s_assetDatas[asset] = assetData;

                var savePath = Path.Combine(AppConst.hotSavePath, asset + ".bytes");

                File.WriteAllBytes(savePath, assetData);
            }
        }

        onDownloadComplete();
    }

    /// <summary>
    /// 为aot assembly加载原始metadata， 这个代码放aot或者热更新都行。
    /// 一旦加载后，如果AOT泛型函数对应native实现不存在，则自动替换为解释模式执行
    /// </summary>
    private static void LoadMetadataForAOTAssemblies()
    {
        /// 注意，补充元数据是给AOT dll补充元数据，而不是给热更新dll补充元数据。
        /// 热更新dll不缺元数据，不需要补充，如果调用LoadMetadataForAOTAssembly会返回错误
        /// 
        HomologousImageMode mode = HomologousImageMode.SuperSet;
        foreach (var aotDllName in AOTMetaAssemblyFiles)
        {
            byte[] dllBytes = ReadBytesFromStreamingAssets(aotDllName);
            // 加载assembly对应的dll，会自动为它hook。一旦aot泛型函数的native函数不存在，用解释器版本代码
            LoadImageErrorCode err = RuntimeApi.LoadMetadataForAOTAssembly(dllBytes, mode);
            Debug.Log($"LoadMetadataForAOTAssembly:{aotDllName}. mode:{mode} ret:{err}");
        }
    }

    public static byte[] ReadBytesFromStreamingAssets(string dllName)
    {
        return s_assetDatas[dllName];
    }

    private string GetWebRequestPath(string asset)
    {
        var path = $"{AppConst.resourcesPath}/{asset}";
        if (!path.Contains("://"))
        {
            path = "file://" + path;
        }
        return path;
    }
}
