using System;
using UnityEngine;
#if InterstitialAdv_yg || RewardedAdv_yg
using YG;
#endif

/// <summary>
/// Thin ads wrapper around PluginYG2. Compiles without modules via stubs.
/// </summary>
public static class YandexAdsService
{
    public const string ExtraTimeRewardId = "extra_time";

    public static void ShowInterstitial()
    {
#if InterstitialAdv_yg
        YG2.InterstitialAdvShow();
#else
        Debug.Log("[Ads] InterstitialAdv module missing — stub call.");
#endif
    }

    public static void ShowRewarded(string rewardId, Action onRewarded)
    {
#if RewardedAdv_yg
        YG2.RewardedAdvShow(rewardId, onRewarded);
#else
        Debug.Log("[Ads] RewardedAdv module missing — granting reward in editor/stub.");
        onRewarded?.Invoke();
#endif
    }

    public static void NotifyGameReady()
    {
#if YandexGamesPlatform_yg
        YG2.GameReadyAPI();
#endif
    }

    public static void NotifyGameplayStart()
    {
#if YandexGamesPlatform_yg
        YG2.GameplayStart();
#endif
    }

    public static void NotifyGameplayStop()
    {
#if YandexGamesPlatform_yg
        YG2.GameplayStop();
#endif
    }
}
