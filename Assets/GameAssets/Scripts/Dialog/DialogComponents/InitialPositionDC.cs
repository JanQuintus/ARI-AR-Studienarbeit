using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InitialPositionDC : ADialogComponent
{
    public Transform InitialTransform;

    public override void Perform()
    {
        ARIDialog.Instance?.transform.SetPositionAndRotation(InitialTransform.position, InitialTransform.rotation);
    }

    public override void Cancel(){}

    public override bool IsFinished() => true;
}
