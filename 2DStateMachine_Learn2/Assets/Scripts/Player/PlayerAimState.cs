using UnityEngine;

public class PlayerAimState : PlayerState
{
    public PlayerAimState(Player _player, PlayerStateMachine _stateMachine, string _animBoolName) : base(_player, _stateMachine, _animBoolName)
    {
    }

    public override void Enter()
    {
        base.Enter();
    }

    public override void Exit()
    {
        base.Exit();
    }

    public override void Update()
    {
        base.Update();

        if (Input.GetKeyUp(KeyCode.Mouse1))
            stateMachine.ChangeState(player.idleState);
    }
}
