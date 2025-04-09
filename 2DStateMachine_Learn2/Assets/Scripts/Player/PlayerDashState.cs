using UnityEngine;

public class PlayerDashState : PlayerState
{
    public PlayerDashState(Player _player, PlayerStateMachine _stateMachine, string _animBoolName) : base(_player, _stateMachine, _animBoolName)
    {
    }

    public override void Enter()
    {
        base.Enter();

        stateTimer = player.dashDuration;
    }

    public override void Exit()
    {
        base.Exit();
        player.SetVelocity(0, rb.linearVelocityY);
    }

    public override void Update()
    {
        base.Update();

        player.SetVelocity(player.dashSpeed * player.dashDir, 0);

        if (!player.IsGroundedDetected() && player.isWallDetected())
            stateMachine.ChangeState(player.wallslideState);

        if (stateTimer < 0)
            stateMachine.ChangeState(player.idleState);
    }
}
