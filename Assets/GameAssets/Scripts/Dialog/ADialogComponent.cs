using UnityEngine;

public abstract class ADialogComponent : MonoBehaviour
{
    public bool Parallel = false;

    public abstract void Perform();
    public abstract void Cancel();
    public abstract bool IsFinished();
}
