using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class PointAtDC : ADialogComponent
{
    [Header("Always Parallel!")]
    public Transform Target;

    private Tweener _tweener;

    public override void Perform()
    {
        bool isRight = Vector3.SignedAngle(ARIDialog.Instance.transform.forward, Target.position - ARIDialog.Instance.transform.position, Vector3.up) > 0f;

        Vector3 targetRight = ARIDialog.Instance.transform.right;
        if (isRight)
        {
            targetRight = Target.position - ARIDialog.Instance.transform.position;
            ARIDialog.Instance.PointRight();
        }
        else
        {
            targetRight = -(Target.position - ARIDialog.Instance.transform.position);
            ARIDialog.Instance.PointLeft();
        }

        targetRight.y = 0;
        _tweener?.Kill(false);
        _tweener = DOTween.To(() => ARIDialog.Instance.transform.right, x => ARIDialog.Instance.transform.right = x, targetRight, .25f).SetEase(Ease.InOutCubic);
    }

    public override void Cancel()
    {
        _tweener?.Kill(false);
    }

    public override bool IsFinished()
    {
        return true;
    }
}
