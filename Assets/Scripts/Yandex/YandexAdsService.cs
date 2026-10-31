using System;
using UnityEngine;
#if InterstitialAdv_yg || RewardedAdv_yg || StickyAdv_yg
using YG;
#endif

/// <summary>
/// Thin ads wrapper around PluginYG2. Compiles without modules via stubs.
///
/// Placement strategy (Yandex criteria § 1.12, § 1.16):
///   • StickyAdv   — shown on Main Menu and Game Over; hidden during gameplay/countdown.
///   • Interstitial — shown on logical pauses only: Game Over (before result screen),
///                    transitions back to Main Menu. Timer enforced by the SDK.
///   • Rewarded    — "+25 sec" offer on Game Over screen; reward granted only AFTER ad closes.
/// </summary>
public static class YandexAdsService
{
    public const string ExtraTimeRewardId = "extra_time";

    // ——— Sticky Banner ——————————————————————————————————————————————————————————

    /// <summary>Show the sticky banner (Main Menu / Game Over screens).</summary>
    public static void ShowStickyBanner()
    {
#if StickyAdv_yg
        YG2.StickyAdActivity(true);
#else
        Debug.Log("[Ads] StickyAdv module missing — stub ShowStickyBanner.");
#endif
    }

    /// <summary>Hide the sticky banner (during gameplay and countdown).</summary>
    public static void HideStickyBanner()
    {
#if StickyAdv_yg
        YG2.StickyAdActivity(false);
#else
        Debug.Log("[Ads] StickyAdv module missing — stub HideStickyBanner.");
#endif
    }

    // ——— Interstitial ——————————————————————————————————————————————————————————

    /// <summary>
    /// Request a full-screen interstitial ad (only shown when the SDK internal
    /// timer is ready; safe to call on every logical pause transition).
    /// If <paramref name="onClosed"/> is provided, it is invoked after the ad closes
    /// or immediately if no ad was shown.
    /// </summary>
    public static void ShowInterstitial(Action onClosed = null)
    {
#if InterstitialAdv_yg
        if (onClosed != null)
        {
            if (YG2.isTimerAdvCompleted && !YG2.nowAdsShow)
            {
                Action closeHandler = null;
                closeHandler = () =>
                {
                    YG2.onCloseInterAdv -= closeHandler;
                    YG2.onErrorInterAdv -= closeHandler;
                    onClosed.Invoke();
                };
                YG2.onCloseInterAdv += closeHandler;
                YG2.onErrorInterAdv += closeHandler;
                YG2.InterstitialAdvShow();
                return;
            }
            else
            {
                onClosed.Invoke();
                return;
            }
        }
        YG2.InterstitialAdvShow();
#else
        Debug.Log("[Ads] InterstitialAdv module missing — stub ShowInterstitial.");
        onClosed?.Invoke();
#endif
    }

    // ——— Rewarded ——————————————————————————————————————————————————————————————

    /// <summary>
    /// Show a rewarded video. The <paramref name="onRewarded"/> callback is
    /// invoked AFTER the ad window closes to avoid timeScale conflicts with
    /// PauseGameYG (Yandex SDK sets timeScale=0 while the ad is open).
    /// If reward fails or is skipped, <paramref name="onFailed"/> is called.
    /// </summary>
    public static void ShowRewarded(string rewardId, Action onRewarded, Action onFailed = null)
    {
#if RewardedAdv_yg
        bool gotReward = false;
        bool adClosed  = false;

        Action tryApplyReward = () =>
        {
            if (gotReward && adClosed)
            {
                Time.timeScale = 1f;
                onRewarded?.Invoke();
                Time.timeScale = 1f;
            }
            else if (!gotReward && adClosed)
            {
                onFailed?.Invoke();
            }
        };

        Action rewardHandler = () =>
        {
            gotReward = true;
            tryApplyReward();
        };

        Action closeHandler = null;
        closeHandler = () =>
        {
            YG2.onCloseRewardedAdv  -= closeHandler;
            YG2.onErrorRewardedAdv  -= closeHandler;
            adClosed = true;
            tryApplyReward();
        };

        YG2.onCloseRewardedAdv  += closeHandler;
        YG2.onErrorRewardedAdv  += closeHandler;
        YG2.RewardedAdvShow(rewardId, rewardHandler);
#else
        Debug.Log("[Ads] RewardedAdv module missing — granting reward in editor/stub.");
        Time.timeScale = 1f;
        onRewarded?.Invoke();
        Time.timeScale = 1f;
#endif
    }

    // ——— Gameplay API ——————————————————————————————————————————————————————————

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
