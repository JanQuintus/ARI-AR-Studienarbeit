using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.MagicLeap;


public class ProgramBlockSpawner : MonoBehaviour
{
    public GameObject ProgramBlockMenu;
    public ButtonGroup ProgramBlockList;

    private bool _visible = false;
    private MLInputController _controller;

    private void Start()
    {
        if (!(MLInput.Start().IsOk))
        {
            Debug.LogError("Error starting MLInput, disabling script.");
            return;
        }

        _controller = MLInput.GetController(MLInput.Hand.Left);
        _controller.OnButtonDown += OnBumperPressed;

        ProgramBlockMenu.SetActive(false);
        ProgramBlockList.Enabled = false;
    }

    private void OnDestroy()
    {
        if (_controller.Connected)
        {
            MLInput.Stop();
            _controller.OnButtonDown -= OnBumperPressed;
        }
    }

    public void SpawnPB(GameObject pb)
    {
        Vector3 position = transform.position + transform.forward;
        Instantiate(pb, position, Quaternion.identity);
    }

    private void OnBumperPressed(byte controllerId, MLInputControllerButton button)
    {
        if (button == MLInputControllerButton.Bumper)
            Toggle();
    }

    private void Toggle()
    {
        if (_visible)
        {
            
            ProgramBlockMenu.SetActive(false);
            ProgramBlockList.Enabled = false;
        }
        else
        {
            ProgramBlockMenu.SetActive(true);
            ProgramBlockList.Enabled = true;
        }

        _visible = !_visible;
    }
}
