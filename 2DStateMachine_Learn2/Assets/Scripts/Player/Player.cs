using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem.LowLevel;
using UnityEngine.Rendering;

public class Player : Entity
{
    [Header("공격 디테일")]
    public Vector2[] attackMovement;
    public float counterAttackDuration = 0.2f;

    public bool isBusy { get; private set; }
    [Header("이동 정보")]
    public float moveSpeed = 12f;
    public float jumpForce;

    [Header("대시 정보")]
    //[SerializeField] private float dashCoolDown;
    //private float dashUsageTimer;
    public float dashSpeed;
    public float dashDuration;
    public float dashDir { get; private set; }

    public SkillManager skill { get; private set; }

    #region States
    public PlayerStateMachine stateMachine { get; private set; }

    public PlayerIdleState idleState { get; private set; }
    public PlayerMoveState moveState { get; private set; }
    public PlayerJumpState jumpState { get; private set; }
    public PlayerAirState airState { get; private set; }
    public PlayerDashState dashState { get; private set; }
    public PlayerWallSlideState wallslideState { get; private set; }
    public PlayerWallJumpState wallJumpState { get; private set; }
    public PlayerPrimaryAttack primaryAttack { get; private set; }
    public PlayerCounterAttackState counterAttackState { get; private set; }
    public PlayerAimState aimSword { get; private set; }
    public PlayerCatchSwordState catchSwrod { get; private set; }

    #endregion

    protected override void Awake()
    {
        stateMachine = new PlayerStateMachine();
        idleState = new PlayerIdleState(this, stateMachine, "Idle");
        moveState = new PlayerMoveState(this, stateMachine, "Move");
        jumpState = new PlayerJumpState(this, stateMachine, "Jump");
        airState = new PlayerAirState(this, stateMachine, "Jump");
        dashState = new PlayerDashState(this, stateMachine, "Dash");
        wallslideState = new PlayerWallSlideState(this, stateMachine, "WallSlide");
        wallJumpState = new PlayerWallJumpState(this, stateMachine, "Jump");
        primaryAttack = new PlayerPrimaryAttack(this, stateMachine, "Attack");
        counterAttackState = new PlayerCounterAttackState(this, stateMachine, "CounterAttack");
        aimSword = new PlayerAimState(this, stateMachine, "AimSword");
        catchSwrod = new PlayerCatchSwordState(this, stateMachine, "CatchSwrod");
    }

    protected override void Start()
    {
        base.Start();

        skill = SkillManager.instance;

        stateMachine.Initialize(idleState);
    }

    protected override void Update()
    {
        base.Update();

        stateMachine.currentState.Update();
        CheckForDashInput();
    }

    public IEnumerator BusyFor(float _seconds)
    {
        isBusy = true;
        yield return new WaitForSeconds(_seconds);
        isBusy = false;
    }

    public void AnimationTrigger() => stateMachine.currentState.AnimationFinishTrigger();

    private void CheckForDashInput()
    {
        if (isWallDetected())
            return;


        if (Input.GetKeyDown(KeyCode.LeftShift) && SkillManager.instance.dash.CanUseSkill())
        {
            dashDir = Input.GetAxisRaw("Horizontal");

            if (dashDir == 0)
                dashDir = facingDir;

            stateMachine.ChangeState(dashState);
        }
    }
}
