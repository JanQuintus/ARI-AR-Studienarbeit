using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.MagicLeap;
using UnityEngine.SceneManagement;
using DG.Tweening;


public class RoomMapping : MonoBehaviour
{
    #region Public Variables
    public GameObject TitleGO;
    public GameObject SkipDialogGO;
    #endregion

    #region Private Variables
    private MLInputController _controller;
    private bool _showingSkipDialog = false;
    #endregion

    private void Start()
    {
        if (!(MLInput.Start().IsOk))
        {
            Debug.LogError("Error starting MLInput, disabling script.");
            return;
        }
        _controller = MLInput.GetController(MLInput.Hand.Left);
        _controller.OnButtonDown += OnButtonPressed;

        SkipDialogGO.SetActive(false);

    }

    private void OnDestroy()
    {
        if (_controller.Connected)
        {
            MLInput.Stop();
            _controller.OnButtonDown -= OnButtonPressed;
        }
    }

    public void OnButtonPressed(byte controllerId, MLInputControllerButton button)
    {
        if (button == MLInputControllerButton.HomeTap)
        {
            if (_showingSkipDialog)
                CloseSkipDialog();
            else
                ShowSkipDialog();

            _controller.StartFeedbackPatternVibe(MLInputControllerFeedbackPatternVibe.Click, MLInputControllerFeedbackIntensity.Medium);
        }
    }

    public void CloseSkipDialog()
    {
        SkipDialogGO.SetActive(false);
        TitleGO.SetActive(true);
        _showingSkipDialog = false;
    }

    private void ShowSkipDialog()
    {
        SkipDialogGO.SetActive(true);
        TitleGO.SetActive(false);
        _showingSkipDialog = true;
    }

    public void ProceedToGroundSelection()
    {
        SceneManager.LoadScene("SelectGround", LoadSceneMode.Single);
    }
}
