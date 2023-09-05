using System;
using UnityEngine;
using UnityEngine.SceneManagement;
using Object = UnityEngine.Object;

namespace Match3Game.Asset.AssetLoader
{
    public class BundleAssetLoader :IAssetLoader
    {
        public T LoadAsset<T>(string assetName) where T : Object
        {
            //1.查找资源所在bundle assetName-bundle  manifest
            string bundleName = "";
            AssetBundle bundle = AssetBundle.LoadFromFile(bundleName);
            //这里可以不用判断，在打包assetbundle时做好分类就可以
            if (bundle.isStreamedSceneAssetBundle)
            {
                throw new Exception($"资源加载错误！ 该资源属于场景资源{assetName}");
            }

            return bundle.LoadAsset<T>(assetName);
        }

        public void LoadScene(string sceneName)
        {
            //1.查找场景 sceneName-bundle

            string bundleName = "";
            AssetBundle bundle = AssetBundle.LoadFromFile(bundleName);
            //这里可以不用判断，在打包assetbundle时做好分类就可以
            if (!bundle.isStreamedSceneAssetBundle)
            {
                
            }
            SceneManager.LoadScene(sceneName);
        }

        public T LoadSubAsset<T>(string assetName) where T : Object
        {
            //1.查找父级资源
            string parentAsset = "";
            string bundleName = "";
            AssetBundle bundle = AssetBundle.LoadFromFile(bundleName);

            T[] assets = bundle.LoadAssetWithSubAssets<T>(parentAsset);
            return Array.Find(assets, _ => _.name == assetName);
        }

        public T[] LoadAssetWithSubAssets<T>(string assetName) where T : Object
        {
            string bundleName = "";
            AssetBundle bundle = AssetBundle.LoadFromFile(bundleName);

            return  bundle.LoadAssetWithSubAssets<T>(assetName);
        }

        public void LoadAssetAsync<T>(string assetName, Action<T> completed) where T : Object
        {
            string bundleName = "";
            AssetBundleCreateRequest request = AssetBundle.LoadFromFileAsync(bundleName);
            //request.
        }

        public void LoadSubAssetAsync<T>(string assetName, Action<T> completed) where T : Object
        {
            throw new NotImplementedException();
        }

        public void LoadAssetWithSubAssetsAsync<T>(string assetName, Action<T[]> completed) where T : Object
        {
            throw new NotImplementedException();
        }

        public void LoadSceneAsync(string sceneName)
        {
            throw new NotImplementedException();
        }
    }
}