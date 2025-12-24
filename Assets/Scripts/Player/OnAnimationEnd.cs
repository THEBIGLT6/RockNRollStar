using UnityEngine;

public class OnAnimationEnd : StateMachineBehaviour
{
    override public void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        var parent = animator.transform.parent;
        if (parent != null)
        {
            parent.SendMessage("OnAnimationFinished", SendMessageOptions.DontRequireReceiver);
        }
    }
}