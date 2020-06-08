using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.MagicLeap;

public class ButtonGroup : MonoBehaviour
{
    public enum ControllerButton
    {
        TRIGGER,
        BUMPER,
        TOUCHPAD
    }

    #region Public Variables
    public ControllerButton ClickButton;
    public bool Enabled = true;
    #endregion

    #region Private Variables
    private OutlineButton[] _buttons;
    private MLInputController _controller;
    private bool _readyToPressTrigger = false;
    private OutlineButton _selectedButton;
    #endregion

    private void Start()
    {
        if (!(MLInput.Start().IsOk))
        {
            Debug.LogError("Error starting MLInput, disabling script.");
            return;
        }

        _controller = MLInput.GetController(MLInput.Hand.Left);
        _controller.OnTouchpadGestureEnd += OnGestureEnd;

        if (ClickButton == ControllerButton.TOUCHPAD)
            _controller.OnTouchpadGestureStart += OnGestureStart;

        if (ClickButton == ControllerButton.BUMPER)
            _controller.OnButtonDown += OnBumperPressed;

        _buttons = GetComponentsInChildren<OutlineButton>();
        if (_buttons.Length > 0)
        {
            _buttons[0].Select();
            _selectedButton = _buttons[0];
        }
    }

    private void OnDestroy()
    {
        if (_controller.Connected)
        {
            MLInput.Stop();
            _controller.OnTouchpadGestureEnd -= OnGestureEnd;
            if (ClickButton == ControllerButton.BUMPER)
                _controller.OnButtonDown -= OnBumperPressed;
            if (ClickButton == ControllerButton.TOUCHPAD)
                _controller.OnTouchpadGestureStart -= OnGestureStart;
        }
    }

    private void Update()
    {
        if (!_controller.Connected)
            return;

        if (ClickButton == ControllerButton.TRIGGER)
            UpdateTrigger();
    }

    private void UpdateTrigger()
    {
        if (!Enabled) return;

        if (_controller == null) return;
        if (_controller.TriggerValue >= 0.6f && _readyToPressTrigger)
        {
            _readyToPressTrigger = false;
            _selectedButton?.Press();
        }
        else if (_controller.TriggerValue <= 0.2f)
        {
            _readyToPressTrigger = true;
        }
    }

    private void OnBumperPressed(byte controllerId, MLInputControllerButton button)
    {
        if (!Enabled) return;

        if (button == MLInputControllerButton.Bumper)
            _selectedButton?.Press();
    }

    private void OnGestureEnd(byte controllerId, MLInputControllerTouchpadGesture touchpadGesture)
    {
        if (!Enabled) return;

        if (_buttons.Length == 0)
            return;

        if (_selectedButton == null && _buttons.Length > 0)
            _selectedButton = _buttons[0];

        if (touchpadGesture.Type == MLInputControllerTouchpadGestureType.Swipe)
        {
            OutlineButton newButton = null;
            switch (touchpadGesture.Direction)
            {
                case MLInputControllerTouchpadGestureDirection.Left:
                    newButton = _selectedButton.GetNeigborInDirection(OutlineButton.NeighborDirection.LEFT).Target;
                    break;
                case MLInputControllerTouchpadGestureDirection.Right:
                    newButton = _selectedButton.GetNeigborInDirection(OutlineButton.NeighborDirection.RIGHT).Target;
                    break;
                case MLInputControllerTouchpadGestureDirection.Up:
                    newButton = _selectedButton.GetNeigborInDirection(OutlineButton.NeighborDirection.TOP).Target;
                    break;
                case MLInputControllerTouchpadGestureDirection.Down:
                    newButton = _selectedButton.GetNeigborInDirection(OutlineButton.NeighborDirection.BOTTOM).Target;
                    break;
            }
            if (newButton)
            {
                _selectedButton?.Deselect();
                newButton.Select();
                _selectedButton = newButton;
            }
        }


    }

    private void OnGestureStart(byte controllerId, MLInputControllerTouchpadGesture touchpadGesture)
    {
        if (!Enabled) return;

        if (touchpadGesture.Type == MLInputControllerTouchpadGestureType.Tap)
            _selectedButton?.Press();
    }
}
