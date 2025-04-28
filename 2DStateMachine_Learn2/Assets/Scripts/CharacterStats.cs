using System;
using Unity.VisualScripting;
using UnityEngine;
using Random = UnityEngine.Random;

public class CharacterStats : MonoBehaviour
{
    [Header("기본적인 스탯")]
    public Stat strength;      //힘
    public Stat agility;       //회피
    public Stat intelligence;  //마법데미지 
    public Stat vitality;      //체력

    [Header("공격관련 스탯")]
    public Stat damage;
    public Stat critChance;
    public Stat critPower;


    [Header("방어관련 스탯")]
    public Stat maxHealth;
    public Stat armor;
    public Stat evasion;       //회피
    public Stat magicResistance;

    [Header("마법 스탯")]
    public Stat fireDamage;
    public Stat iceDamage;
    public Stat lighteningDamage;

    public bool isIgnited;
    public bool isChilled;
    public bool isShocked;

    private float ignitedTimer;  //화염 상태 지속 시간
    private float chilledTimer;  //냉기 상태 지속 시간
    private float shockedTimer;  //전격 상태 지속 시간

    private float igniteDamageCooldown = 0.3f; //화염 상태 지속 시간 쿨타임
    private float ignitedamageTimer;  //화염 상태 지속 타이머

    private int igniteDamage;  // 화염 상태 데미지

    public int currentHealth; // 현재 체력 값
    public Action onHealthChanged; //체력 변경 시 호출되는 델리게이트

    protected virtual void Start()
    {
        // 치명타 배율 기본값을 150으로 설정
        critPower.SetDefaultValue(150);

        // 현재 체력을 최대 체력 값으로 초기화
        currentHealth = GetMaxHealth();
        Debug.Log("캐릭터 스탯을 부르고 잇다");
    }

    protected virtual void Update()
    {
        ignitedTimer -= Time.deltaTime;   //화염상태 지속시간 감소
        chilledTimer -= Time.deltaTime;  //냉기 상태 지속 시간 감소
        shockedTimer -= Time.deltaTime;  //감전 상태 지속 시간 감소

        ignitedamageTimer -= Time.deltaTime;  //화염 상태 데미지 쿨타임 감소

        if (ignitedTimer < 0)
        {
            isIgnited = false;
        }

        if (chilledTimer < 0)
        {
            isChilled = false;
        }

        if (shockedTimer < 0)
        {
            isShocked = false;
        }

        if (ignitedamageTimer < 0 && isIgnited)
        {
            Debug.Log("화염 상태 데미지 적용됨" + igniteDamage);

            DecreaseHealth(igniteDamage);

            if (currentHealth < 0)
                Die();
            ignitedamageTimer = igniteDamageCooldown; //화염 상태 지속 시간 쿨타임 초기화
        }
    }

    public virtual void DoDamage(CharacterStats _targetStats)
    {
        // 대상이 공격을 회피할 수 있는지 확인, 회피 가능하면 함수 종료
        if (CanAvoidAttack(_targetStats))
            return;

        // 기본 데미지와 힘 스탯을 더해 총 물리 데미지 계산
        int totalDamage = damage.GetValue() + strength.GetValue();

        // 치명타 여부를 확인하고 치명타 데미지를 계산
        if (CanCrit())
        {
            CalculateCriticalDmage(totalDamage);
        }

        // 대상의 방어력을 고려하여 데미지를 감소
        totalDamage = CheckTargetArmor(_targetStats, totalDamage);

        // 계산된 물리 데미지를 대상에게 적용
        _targetStats.TakeDamage(totalDamage);

        // 마법 데미지를 추가로 적용
        DoMagicalDamage(_targetStats);
    }

    public virtual void DoMagicalDamage(CharacterStats _targetStats)
    {
        // 각각의 마법 데미지 값 가져오기
        int _fireDamage = fireDamage.GetValue();
        int _iceDamage = iceDamage.GetValue();
        int _lighteningDamage = lighteningDamage.GetValue();

        // 총 마법 데미지를 계산 (화염, 얼음, 번개 데미지 + 지능 스탯)
        int totalMagicalDamage = _fireDamage + _iceDamage + _lighteningDamage + intelligence.GetValue();

        // 대상의 마법 저항력과 지능을 고려하여 마법 데미지를 감소
        totalMagicalDamage = CheckTargetResistance(_targetStats, totalMagicalDamage);

        // 계산된 마법 데미지를 대상에게 적용
        _targetStats.TakeDamage(totalMagicalDamage);

        if (Mathf.Max(_fireDamage, _iceDamage, _lighteningDamage) <= 0)
        {
            return;  //모든 속성 데미지가 0 이하일 경우 상태이상 적용하지 않음
        }

        // 가장 높은 마법 데미지 타입에 따라 상태 이상 효과 적용 가능 여부 확인
        bool canApplyIgnite = _fireDamage > _iceDamage && _fireDamage > _lighteningDamage; // 화염 효과
        bool canApplyChill = _iceDamage > _fireDamage && _iceDamage > _lighteningDamage;   // 냉기 효과
        bool canApplyShock = _lighteningDamage > _fireDamage && _lighteningDamage > _iceDamage; // 전격 효과

        while (!canApplyIgnite && !canApplyChill && !canApplyChill)
        {
            if (Random.value < 0.35f && _fireDamage > 0)
            {
                canApplyIgnite = true;
                _targetStats.ApplyAilments(canApplyIgnite, canApplyChill, canApplyShock); //상태이상 적용
                Debug.Log("점화 상태 적용됨");
                return;
            }

            if (Random.value < 0.25f && _iceDamage > 0)
            {
                canApplyChill = true;
                _targetStats.ApplyAilments(canApplyIgnite, canApplyChill, canApplyShock); //상태이상 적용
                Debug.Log("냉기 상태 적용됨");
                return;
            }

            if (Random.value < 0.15f && _lighteningDamage > 0)
            {
                canApplyShock = true;
                _targetStats.ApplyAilments(canApplyIgnite, canApplyChill, canApplyShock); //상태이상 적용
                Debug.Log("전격 상태 적용됨");
                return;
            }
        }

        if (canApplyIgnite)
            _targetStats.SetupIgniteDamage(Mathf.RoundToInt(_fireDamage * 0.2f));

        _targetStats.ApplyAilments(canApplyIgnite, canApplyChill, canApplyShock);
    }

    private static int CheckTargetResistance(CharacterStats _targetStats, int totalMagicalDamage)
    {
        // 대상의 마법 저항력과 지능에 따라 마법 데미지를 감소
        totalMagicalDamage -= _targetStats.magicResistance.GetValue() + (_targetStats.intelligence.GetValue() * 3);

        // 데미지가 음수가 되지 않도록 0으로 클램프
        totalMagicalDamage = Mathf.Clamp(totalMagicalDamage, 0, int.MaxValue);

        // 조정된 마법 데미지를 반환
        return totalMagicalDamage;
    }

    public void ApplyAilments(bool _ignite, bool _chill, bool _shock)
    {
        if (isIgnited || isChilled || isShocked)
        {
            return;
        }

        if (_ignite)
        {
            isIgnited = _ignite; //점화상태 적용
            ignitedTimer = 2f; //점화상태 지속 시간 설정
        }

        if (_chill)
        {
            isChilled = _chill; //냉기 상태 적용
            chilledTimer = 2f; //냉기 상태 지속 시간 설정
        }

        if (_shock)
        {
            isShocked = _shock; //전격 상태 적용
            shockedTimer = 2f; //전격 상태 지속 시간 설정
        }
    }

    public void SetupIgniteDamage(int _damage) => igniteDamage = _damage; //화염 상태 데미지 설정

    private static int CheckTargetArmor(CharacterStats _targetStats, int totalDamage)
    {
        if (_targetStats.isChilled)
            totalDamage -= Mathf.RoundToInt(_targetStats.armor.GetValue() * 0.8f);
        else
            totalDamage -= _targetStats.armor.GetValue();

        totalDamage = Mathf.Clamp(totalDamage, 0, int.MaxValue);
        return totalDamage;
    }

    private bool CanAvoidAttack(CharacterStats _targetStats)
    {
        int totalEvasion = _targetStats.evasion.GetValue() + _targetStats.agility.GetValue();

        if (isShocked)
        {
            totalEvasion += 20; // 전격 상태일 경우 회피 확률 증가
        }

        if (Random.Range(0, 100) < totalEvasion)
        {
            return true;
        }

        return false;
    }

    public virtual void TakeDamage(int _damage)
    {
        DecreaseHealth(_damage); //체력 감소

        if (currentHealth <= 0)
            Die();
    }

    protected virtual void DecreaseHealth(int _damage)
    {
        currentHealth -= _damage;
        if (onHealthChanged != null)
            onHealthChanged?.Invoke(); //체력 변경 시 호출되는 델리게이트 호출
    }

    protected virtual void Die()
    {

    }

    private bool CanCrit()
    {
        int totalCriticalChance = critChance.GetValue() + agility.GetValue();

        if (Random.Range(0, 100) <= totalCriticalChance)
        {
            return true;
        }

        return false;
    }

    private int CalculateCriticalDmage(int _damage)
    {
        float totalCritPower = (critPower.GetValue() + strength.GetValue()) * 0.01f;
        float critDamage = _damage * totalCritPower;

        return Mathf.RoundToInt(critDamage);
    }

    public int GetMaxHealth()
    {
        return maxHealth.GetValue() + vitality.GetValue() * 5;
    }
}
