
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ARIDialog : MonoBehaviour
{
    public static ARIDialog Instance;

    public AudioSource SpeakSource;
    public Animator ARIAnimator;

    private Dialog _currentDialog;
    private int _dcIndex = 0;


    private void Awake()
    {
        if (Instance != null)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
    }

    public void StartDialog(Dialog dialog)
    {
        CancelDialog();

        _dcIndex = 0;
        _currentDialog = dialog;
        StartCoroutine(RunDialog());
    }

    public void CancelDialog()
    {
        if (!_currentDialog) return;

        StopAllCoroutines();
        CancelSpeak();
        StopAnimations();
        if (_dcIndex < _currentDialog.DialogComponents.Length)
        {
            _currentDialog.DialogComponents[_dcIndex].Cancel();
        }
    }

    public void Speak(AudioClip clip)
    {
        SpeakSource?.PlayOneShot(clip);
    }

    public bool IsSpeaking() {
        if (SpeakSource != null)
            return SpeakSource.isPlaying;

        return false;
    }

    public void StartSmile() => ARIAnimator.SetBool("Smile", true);
    public void StopSmile() => ARIAnimator.SetBool("Smile", false);
    public void PointLeft() { ARIAnimator.SetBool("PointLeft", true);  ARIAnimator.SetBool("PointRight", false); }
    public void PointRight() { ARIAnimator.SetBool("PointRight", true); ARIAnimator.SetBool("PointLeft", false); }
    public void StopPoint() { ARIAnimator.SetBool("PointRight", false); ARIAnimator.SetBool("PointLeft", false); }

    private void CancelSpeak()
    {
        SpeakSource?.Stop();
    }

    private void StopAnimations()
    {
        StopSmile();
        StopPoint();
    }

    private IEnumerator RunDialog()
    {
        while (_dcIndex < _currentDialog.DialogComponents.Length)
        {
            _currentDialog.DialogComponents[_dcIndex].Perform();
            if(!_currentDialog.DialogComponents[_dcIndex].Parallel)
                yield return new WaitUntil(() => _currentDialog.DialogComponents[_dcIndex].IsFinished());
            _dcIndex++;
        }
    }

}
