using UnityEngine;

public class PlayerStats : CharacterStats
{
    private Player player;

    protected override void Start()
    {
        base.Start();

        player = GetComponent<Player>();
    }

    public override void TakeDamage(int _damage)
    {
        base.TakeDamage(_damage);

        player.Damage();   //플래시 및 넉백 효과
    }

    protected override void Die()
    {
        base.Die();

        player.Die();
    }
}
