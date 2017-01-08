using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using DG.Tweening;

public static class UIAnimationHelper
{
    public static void ScaleEmphasisShort(RectTransform target, UnityAction callback = null)
    {
        ScaleEmphasis(target, 0.1f, callback);
    }

    public static void ScaleEmphasisLong(RectTransform target, UnityAction callback = null)
    {
        ScaleEmphasis(target, 0.5f, callback);
    }

    private static void ScaleEmphasis(RectTransform target, float duration, UnityAction callback = null)
    {
        target.DOKill();
        var tween = target.DOScale(1.1f, duration);
        tween.SetEase(Ease.OutExpo);
        tween.OnComplete(() =>
        {
            tween = target.DOScale(1f, duration / 2f);
            tween.SetEase(Ease.Linear);
            tween.OnComplete(() =>
            {
                if (callback != null) callback();
            });
        });
    }
}
