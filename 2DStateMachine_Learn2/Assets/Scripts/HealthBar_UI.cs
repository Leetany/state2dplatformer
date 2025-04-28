using UnityEngine;
using UnityEngine.UI;

public class HealthBar_UI : MonoBehaviour
{
    private Entity entity; //Entity 컴포넌트 참조
    private CharacterStats myStats; // CharacterStats 컴포넌트 참조
    private RectTransform myTransform; // RectTransform 컴포넌트 참조
    private Slider slider; // Slider 컴포넌트 참조

    private void Start()
    {
        entity = GetComponentInParent<Entity>();
        myTransform = GetComponent<RectTransform>(); // RectTransform 컴포넌트 가져오기
        slider = GetComponent<Slider>(); // Slider 컴포넌트 가져오기
        myStats = GetComponentInParent<CharacterStats>();

        entity.onFlipped += FlipUI; // Entity의 onFlipped 이벤트에 FlipUI 메서드 등록
        myStats.onHealthChanged += UpdateHealthUI; // 체력이 변경될 때만 연산 델리게이트 용이성

        UpdateHealthUI(); // 초기 체력 UI 업데이트

        Debug.Log("체력바를 부르고 잇다");
    }

    private void UpdateHealthUI()
    {
        slider.maxValue = myStats.GetMaxHealth();
        slider.value = myStats.currentHealth; // 현재 체력 값을 슬라이더에 설정
    }

    private void FlipUI() => myTransform.Rotate(0, 180, 0); // UI를 180도 회전시켜서 반대 방향으로 표시

    private void OnDisable()
    {
        entity.onFlipped -= FlipUI; // Entity의 onFlipped 이벤트에서 FlipUI 메서드 등록 해제
        myStats.onHealthChanged -= UpdateHealthUI; // 체력 변경 이벤트에서 UpdateHealthUI 메서드 등록 해제
    }
}
