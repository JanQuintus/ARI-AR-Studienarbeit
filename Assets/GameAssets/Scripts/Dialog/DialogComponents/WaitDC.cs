using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WaitDC : ADialogComponent
{
    public float WaitForSec = 1f;

    private bool _isFinished = false;

    public override void Perform()
    {
        _isFinished = false;
        StartCoroutine(WaitCor());
    }

    public override void Cancel()
    {
        StopAllCoroutines();
    }

    public override bool IsFinished()
    {
        return _isFinished;
    }

    private IEnumerator WaitCor()
    {
        yield return new WaitForSeconds(WaitForSec);
        _isFinished = true;
    }
}
