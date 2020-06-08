using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class FlyToDC : ADialogComponent
{
    public Transform Target;
    public float MoveTime = 2f;
    public float RotateTime = 0.25f;
    public Ease MoveEase = Ease.InOutCubic;
    public Ease RotateEase = Ease.InOutCubic;

    private bool _isRunning = false;

    public override void Perform()
    {
        if (ARIDialog.Instance == null)
            return;

        _isRunning = true;

        Vector3 lookRot = Target.position - ARIDialog.Instance.transform.position;
        lookRot.y = ARIDialog.Instance.transform.position.y;
        ARIDialog.Instance.transform.DORotateQuaternion(Quaternion.LookRotation(lookRot), RotateTime).SetEase(RotateEase).OnComplete(() =>
        {
            ARIDialog.Instance.transform.DOMove(Target.position, MoveTime).SetEase(MoveEase).OnComplete(() =>
            {
                ARIDialog.Instance.transform.DORotateQuaternion(Target.rotation, RotateTime).SetEase(RotateEase).OnComplete(() =>
                {
                    _isRunning = false;
                });
            });
        });
    }

    public override void Cancel()
    {
        if (ARIDialog.Instance)
            ARIDialog.Instance.transform.DOKill(false);
        _isRunning = false;
    }

    public override bool IsFinished()
    {
        return !_isRunning;
    }
}
