using UnityEngine;

public class ShockwaveTrigger : StateMachineBehaviour
{
    override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        PlayerShockwave shockwave = animator.GetComponentInParent<PlayerShockwave>();
        if (shockwave != null)
        {
            shockwave.StartShockwaveEffect();
            shockwave.startBlinking();
        }
    }

    override public void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        PlayerCombat playerCombat = animator.GetComponentInParent<PlayerCombat>();
        if (playerCombat != null) playerCombat.resetSoloTimer();

        PlayerShockwave shockwave = animator.GetComponentInParent<PlayerShockwave>();
        if (shockwave != null) shockwave.stopBlinking();
    }
}