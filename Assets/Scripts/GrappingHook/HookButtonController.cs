using UnityEngine;

public class HookButtonController : MonoBehaviour
{
    [SerializeField] HookController hookController;

    public void LaunchHook()
    {
        hookController.LaunchHook();
    }

    public void ReturnHook()
    {
        hookController.ReturnHook();
    }

    public void UpHook()
    {
        hookController.UpHook();
    }

    public void DownHook()
    {
        hookController.DownHook();
    }
}