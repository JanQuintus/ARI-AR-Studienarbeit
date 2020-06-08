using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.MagicLeap;

public class Button3DInteractionHandler : MonoBehaviour
{
    public LayerMask Layer;

    private bool _readyToPressTrigger = false;
    private MLInputController _controller;

    private void Start()
    {
        if (!(MLInput.Start().IsOk))
        {
            Debug.LogError("Error starting MLInput, disabling script.");
            return;
        }

        _controller = MLInput.GetController(MLInput.Hand.Left);
    }

    private void OnDestroy()
    {
        if (_controller.Connected)
            MLInput.Stop();
    }

    private void Update()
    {
        if (!_controller.Connected)
            return;

        if (UpdateTrigger())
        {
            if (Physics.Raycast(transform.position, transform.forward, out RaycastHit hit, 10, Layer))
                hit.collider.gameObject.GetComponent<Button3D>().Press();
        }
    }

    private bool UpdateTrigger()
    {
        if (_controller == null) return false;
        if (_controller.TriggerValue >= 0.6f && _readyToPressTrigger)
        {
            _readyToPressTrigger = false;
            return true;
        }
        else if (_controller.TriggerValue <= 0.2f)
        {
            _readyToPressTrigger = true;
            return false;
        }
        return false;
    }
}
