using UnityEngine;

public abstract class AProgramBlock : MonoBehaviour
{
    public enum BlockState
    {
        DEFAULT,
        GRABBING,
        IN_SLOT
    }

    public string DisplayName;
    public string Description;

    public BlockState State { get; private set; }

    public abstract void Execute();
    public abstract bool IsExecuting();
    public abstract ProgramError CheckBlock();
    public abstract void CancelExecution();

    public void SetState(BlockState state)
    {
        Rigidbody rb = GetComponent<Rigidbody>();

        switch (state)
        {
            case BlockState.DEFAULT:
                rb.isKinematic = false;
                rb.useGravity = true;
                gameObject.layer = 0;
                break;
            case BlockState.GRABBING:
                rb.isKinematic = true;
                rb.useGravity = false;
                gameObject.layer = 8;
                break;
            case BlockState.IN_SLOT:
                rb.isKinematic = true;
                rb.useGravity = false;
                gameObject.layer = 8;
                break;
        }

        State = state;
    }
}
