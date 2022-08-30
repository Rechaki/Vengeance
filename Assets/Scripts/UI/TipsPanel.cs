using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TipsPanel : UIPanel
{
    [SerializeField]
    Animator _animator;

    protected override void Show()
    {
        GlobalMessenger.AddListener(EventMsg.BossComing, PlayBossIn);
    }

    protected override void Hide()
    {
        GlobalMessenger.RemoveListener(EventMsg.BossComing, PlayBossIn);
    }

    void Update()
    {
        AnimatorStateInfo stateInfo = _animator.GetCurrentAnimatorStateInfo(0);
        if (stateInfo.fullPathHash == Animator.StringToHash("Base Layer.BossIconOut") && _animator.IsInTransition(0))
        {
            //GlobalMessenger.Launch(EventMsg.BossBattleStart);
        }
    }

    void PlayBossIn()
    {
        _animator.SetTrigger("BossIn");
        GlobalMessenger.Launch(EventMsg.BossBattleStart);
    }
}
