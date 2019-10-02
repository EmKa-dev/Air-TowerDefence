using UnityEngine;

public class AnimationTriggerController : MonoBehaviour
{
    [SerializeField]
    private string _TriggerName;

    [SerializeField]
    private bool SetAnimationRateFromFireRate;

    public Animator _Animator;

    void Start()
    {
        transform.parent.GetComponentInChildren<ILauncher>().Launch += PlayLaunchAnimation;

        if (SetAnimationRateFromFireRate)
        {
            var f = GetFireRate();

            if (f < 1.0f)
            {
                f = 1.0f;
            }

            _Animator.SetFloat("FireRate", f);
        }
    }

    private float GetFireRate()
    {
        return transform.parent.GetComponentInChildren<AttackOrchestrator>().FireRate;
    }

    private void PlayLaunchAnimation()
    {
        _Animator.SetTrigger("OnLaunch");
    }
}
