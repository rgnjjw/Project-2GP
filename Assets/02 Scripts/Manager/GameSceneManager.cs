using System;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnitySceneManager = UnityEngine.SceneManagement.SceneManager;

namespace _02_Scripts.Manager
{
    public class GameSceneManager : MonoBehaviour
    {
        public static GameSceneManager Instance { get; private set; }

        private string _currentSceneName;

        public Func<Task> OnBeforeLoadAsync;
        public Func<Task> OnAfterLoadAsync;

        public bool IsLoading { get; private set; } = false;

        private void Awake()
        {
            if (Instance == null)
            {
                Instance = this;
                DontDestroyOnLoad(gameObject);
                UnitySceneManager.sceneLoaded += OnSceneChangedHandle;
            }
            else if (Instance != this)
            {
                Destroy(gameObject);
            }
        }

        private void Start()
        {
            _currentSceneName = UnitySceneManager.GetActiveScene().name;
        }

        public async Task LoadSceneAsync(string sceneName)
        {
            if (IsLoading) return;
            IsLoading = true;

            if (OnBeforeLoadAsync != null)
            {
                await OnBeforeLoadAsync.Invoke();
            }

            await UnitySceneManager.LoadSceneAsync(sceneName, LoadSceneMode.Additive).ToTask();
            await UnitySceneManager.UnloadSceneAsync(_currentSceneName).ToTask();

            _currentSceneName = sceneName;

            if (OnAfterLoadAsync != null)
            {
                await OnAfterLoadAsync.Invoke();
            }

            IsLoading = false;
        }

        public async Task LoadOneSceneAsync(string sceneName)
        {
            if (IsLoading) return;
            IsLoading = true;

            if (OnBeforeLoadAsync != null)
            {
                await OnBeforeLoadAsync.Invoke();
            }

            await UnitySceneManager.LoadSceneAsync(sceneName, LoadSceneMode.Single).ToTask();

            _currentSceneName = sceneName;

            if (OnAfterLoadAsync != null)
            {
                await OnAfterLoadAsync.Invoke();
            }

            IsLoading = false;
        }

        public async Task AddSceneAsync(string sceneName)
        {
            await UnitySceneManager.LoadSceneAsync(sceneName, LoadSceneMode.Additive).ToTask();
        }

        public async Task UnloadSceneAsync(string sceneName)
        {
            await UnitySceneManager.UnloadSceneAsync(sceneName).ToTask();
        }

        private void OnSceneChangedHandle(Scene scene, LoadSceneMode mode)
        {
            Debug.Log($"[GameSceneManager] Scene Loaded: {scene.name} ({mode})");
        }

        private void OnDestroy()
        {
            if (Instance == this)
            {
                UnitySceneManager.sceneLoaded -= OnSceneChangedHandle;
            }
        }
    }

    public static class AsyncOperationExtensions
    {
        public static Task ToTask(this AsyncOperation operation)
        {
            var tcs = new TaskCompletionSource<bool>();
            if (operation.isDone)
            {
                tcs.SetResult(true);
                return tcs.Task;
            }

            operation.completed += _ => tcs.SetResult(true);
            return tcs.Task;
        }
    }
}