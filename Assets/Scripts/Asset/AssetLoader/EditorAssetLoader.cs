using System;
using UnityEditor;
using Object = UnityEngine.Object;

namespace Match3Game.Asset.AssetLoader
{
    public class EditorAssetLoader : IAssetLoader
    {
        public T LoadAsset<T>(string assetName) where T : Object
        {
            return AssetDatabase.LoadAssetAtPath<T>($"Assets/Arts/{assetName}");
        }

        public void LoadScene(string sceneName)
        {
            throw new NotImplementedException();
        }

        public T LoadSubAsset<T>(string assetName) where T : Object
        {
            //1.查找表 sub-parent 获取父级
            AssetDatabase.LoadAllAssetsAtPath($"Assets/Arts/");
            throw new NotImplementedException();
        }

        public T[] LoadAssetWithSubAssets<T>(string assetName) where T : Object
        {
            throw new NotImplementedException();
        }

        public void LoadAssetAsync<T>(string assetName, Action<T> completed) where T : Object
        {
            throw new NotImplementedException();
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