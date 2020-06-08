using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SmileDC : ADialogComponent
{
    [Header("Always Parallel!")]
    public bool EnableSmile;

    public override void Perform()
    {
        if (EnableSmile)
            ARIDialog.Instance.StartSmile();
        else
            ARIDialog.Instance.StopSmile();
    }

    public override void Cancel()
    {
    }

    public override bool IsFinished()
    {
        return true;
    }
}
