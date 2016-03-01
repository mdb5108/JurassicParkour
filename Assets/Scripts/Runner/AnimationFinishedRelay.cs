using UnityEngine;
using System.Collections;

public class AnimationFinishedRelay : MonoBehaviour
{
  public RunnerController target;

  public void AnimationFinished()
  {
    target.AnimationFinished();
  }
}
