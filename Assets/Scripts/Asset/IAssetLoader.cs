using System;
using Object = UnityEngine.Object;

namespace Match3Game.Asset
{
    public interface IAssetLoader
    {
        T LoadAsset<T>(string assetName) where T : Object;
        void LoadScene(string sceneName);
        T LoadSubAsset<T>(string assetName) where T : Object;
        T[] LoadAssetWithSubAssets<T>(string assetName) where T : Object;

        void LoadAssetAsync<T>(string assetName, Action<T> completed) where T : Object;
        void LoadSubAssetAsync<T>(string assetName, Action<T> completed) where T : Object;
        void LoadAssetWithSubAssetsAsync<T>(string assetName, Action<T[]> completed) where T : Object;
        void LoadSceneAsync(string sceneName);
    }
}