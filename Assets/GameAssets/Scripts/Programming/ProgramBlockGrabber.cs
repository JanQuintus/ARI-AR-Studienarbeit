using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.MagicLeap;

[RequireComponent(typeof(ControllerLine))]
public class ProgramBlockGrabber : MonoBehaviour
{
    public LayerMask Layer;

    private MLInputController _controller;
    private ControllerLine _controllerLine;
    private GameObject _grabbedObject;
    public static ProgramBlockGrabber Instace;

    private void Start()
    {
        Instace = this;

        if (!(MLInput.Start().IsOk))
        {
            Debug.LogError("Error starting MLInput, disabling script.");
            return;
        }
        _controller = MLInput.GetController(MLInput.Hand.Left);

        _controllerLine = GetComponent<ControllerLine>();
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

        if (UpdateTrigger()) {
            if (_grabbedObject == null && Physics.Raycast(transform.position, transform.forward, out RaycastHit hit, 10, Layer))
            {
                if (hit.transform.gameObject.tag != "ProgramBlock")
                    return;
                GameObject go = hit.collider.gameObject;
                ProgramSpace.Instance?.RemoveFromProgramSpace(go.GetComponent<AProgramBlock>());
                go.GetComponent<AProgramBlock>().SetState(AProgramBlock.BlockState.GRABBING);
                _grabbedObject = go;
                _controllerLine.Target = go.transform;
            }
            if (_grabbedObject != null)
            {
                _grabbedObject.transform.position = transform.position + transform.forward;
                Vector3 fromTo = transform.position - _grabbedObject.transform.position;
                fromTo.y = _grabbedObject.transform.position.y;
                _grabbedObject.transform.LookAt(fromTo, Vector3.down);
            }
        }
        else
        {
            if (_grabbedObject != null)
            {
                _grabbedObject.GetComponent<AProgramBlock>().SetState(AProgramBlock.BlockState.DEFAULT);
                _grabbedObject = null;
                _controllerLine.Target = null;
            }
        }
    }

    private bool UpdateTrigger()
    {
        if (_controller == null) return false;

        if (_controller.TriggerValue >= 0.6f)
            return true;
        else if (_controller.TriggerValue <= 0.2f)
            return false;
        
        return false;
    }
}
