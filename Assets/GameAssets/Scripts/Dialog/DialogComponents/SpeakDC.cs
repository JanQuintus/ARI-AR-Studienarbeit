using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpeakDC : ADialogComponent
{
    public AudioClip Clip;

    public override void Perform()
    {
        ARIDialog.Instance.Speak(Clip);
    }

    public override void Cancel()
    {
    }

    public override bool IsFinished()
    {
        return !ARIDialog.Instance.IsSpeaking();
    }
}
