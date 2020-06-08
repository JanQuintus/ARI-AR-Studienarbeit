using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.MagicLeap;
using UnityEngine.EventSystems;
using UnityEngine.UI;

[RequireComponent(typeof(LineRenderer))]
public class ControllerLine : MonoBehaviour
{
    #region Public Variables
    public LayerMask RaycastLayerMask;
    public Color StartColor;
    public Color EndColor;
    [HideInInspector] public Transform Target;

    #endregion

    #region Private Variables
    private LineRenderer _lineRenderer;
    private MLInputController _controller;

    #endregion

    private void Start()
    {
        if (!(MLInput.Start().IsOk))
        {
            Debug.LogError("Error starting MLInput, disabling script.");
            return;
        }
        _controller = MLInput.GetController(MLInput.Hand.Left);
        _lineRenderer = GetComponent<LineRenderer>();
        _lineRenderer.useWorldSpace = true;
        _lineRenderer.startColor = StartColor;
        _lineRenderer.endColor = EndColor;
    }

    // Update is called once per frame
    void Update()
    {
        if (!_controller.Connected)
            return;

        transform.position = _controller.Position;
        transform.rotation = _controller.Orientation;
        _lineRenderer.SetPosition(0, transform.position + transform.forward * 0.05f);

        if(Target != null)
        {
            _lineRenderer.SetPosition(1, Target.position);
            return;
        }

        if (Physics.Raycast(transform.position, transform.forward, out RaycastHit hit, 10f, RaycastLayerMask))
            _lineRenderer.SetPosition(1, hit.point);
        else
            _lineRenderer.SetPosition(1, transform.forward * 3f);
    }

    private void OnDestroy()
    {
        if (_controller.Connected)
            MLInput.Stop();
    }
}
