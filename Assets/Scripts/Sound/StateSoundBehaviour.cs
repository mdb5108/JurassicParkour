using UnityEngine;

using System.Collections;

public class StateSoundBehaviour : StateMachineBehaviour
{

    public SoundBank soundBank;

    private bool rightLeg;

    // OnStateEnter is called when a transition starts and the state machine starts to evaluate this state
    override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        bool OnGround = animator.GetBool("OnGround");
        float jumpVal = animator.GetFloat("Jump");

        if(OnGround && jumpVal > -2)
        {
            //if(soundBank != null)
            //    soundBank.PlaySound("Land");
        }
    }

    // OnStateUpdate is called on each Update frame between OnStateEnter and OnStateExit callbacks
    override public void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        float jumpLeg = animator.GetFloat("JumpLeg");
        if((rightLeg && jumpLeg >=  .7f) || (!rightLeg && jumpLeg <= -.7f))
        {
            rightLeg = !rightLeg;
            if(soundBank != null)
                soundBank.PlaySound("FootStep");
        }
    }

    // OnStateExit is called when a transition ends and the state machine finishes evaluating this state
    //override public void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex) {
    //
    //}

    // OnStateMove is called right after Animator.OnAnimatorMove(). Code that processes and affects root motion should be implemented here
    //override public void OnStateMove(Animator animator, AnimatorStateInfo stateInfo, int layerIndex) {
    //
    //}

    // OnStateIK is called right after Animator.OnAnimatorIK(). Code that sets up animation IK (inverse kinematics) should be implemented here.
    //override public void OnStateIK(Animator animator, AnimatorStateInfo stateInfo, int layerIndex) {
    //
    //}
}
