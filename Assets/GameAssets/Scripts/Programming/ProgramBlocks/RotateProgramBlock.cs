using UnityEngine;

public class RotateProgramBlock : AProgramBlock
{
    [Range(-1, 1)]
    public int Direction = 1;

    private bool _isExecuting = false;

    private void Update()
    {
        if (_isExecuting)
            _isExecuting = ARI.Instance.Mover.InAction();
    }

    public override void Execute()
    {
        ARI.Instance.Mover.Rotate(Direction);
        _isExecuting = true;
    }

    public override bool IsExecuting()
    {
        return _isExecuting;
    }

    public override void CancelExecution()
    {
        _isExecuting = false;
    }

    public override ProgramError CheckBlock()
    {
        return new ProgramError();
    }
}
