using UnityEngine;
using UnityEngine.SceneManagement;

/// <summary>
/// Calls Game Ready when the first interactive scene is up.
/// Keeps Auto GRA off so LoadingScene does not mark the game ready too early.
/// </summary>
public class YandexGameBootstrap : MonoBehaviour
{
    private static bool created;
    private static bool gameReadySent;

    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterSceneLoad)]
    private static void Init()
    {
        if (created) return;
        created = true;

        GameObject go = new GameObject(nameof(YandexGameBootstrap));
        DontDestroyOnLoad(go);
        go.AddComponent<YandexGameBootstrap>();
    }

    private void OnEnable()
    {
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    private void OnDisable()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    private void Start()
    {
        TrySendGameReady(SceneManager.GetActiveScene());
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        TrySendGameReady(scene);
    }

    private void TrySendGameReady(Scene scene)
    {
        if (gameReadySent) return;
        if (scene.name == Loader.Scene.LoadingScene.ToString()) return;

        gameReadySent = true;
        YandexAdsService.NotifyGameReady();
    }
}
