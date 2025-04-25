using UnityEngine;

public class PlayerBlackholeState : PlayerState
{
    private float flyTime = 0.4f;
    private bool skillUsed;

    private float defaultGravity;

    public PlayerBlackholeState(Player _player, PlayerStateMachine _stateMachine, string _animBoolName) : base(_player, _stateMachine, _animBoolName)
    {
    }

    public override void Enter()
    {
        base.Enter();

        defaultGravity = player.rb.gravityScale;

        skillUsed = false;
        stateTimer = flyTime;
        rb.gravityScale = 0;
    }

    public override void Exit()
    {
        base.Exit();

        player.rb.gravityScale = defaultGravity;
        player.MakeTransparent(false);
    }

    public override void Update()
    {
        base.Update();

        if (stateTimer > 0)
            rb.linearVelocity = new Vector2(0, 15);

        if (stateTimer < 0)
        {
            rb.linearVelocity = new Vector2(0, -0.1f);

            if (!skillUsed)
            {
                if (player.skill.blackHole.CanUseSkill())
                    skillUsed = true;
            }
        }

        //스킬 종료 후 처리
        if (player.skill.blackHole.BlackholeFinished())
            stateMachine.ChangeState(player.airState);
    }
}
