using UnityEngine;

public class Clone_Skill : Skill
{
    [Header("클론 정보")]
    [SerializeField] private GameObject clonePrefabs;
    [SerializeField] private float cloneDuration;
    [Space]
    [SerializeField] private bool canAttack;


    public void CreateClone(Transform _clonePosition)
    {
        GameObject newClone = Instantiate(clonePrefabs);

        newClone.GetComponent<Clone_Skill_Controller>().SetupClone(_clonePosition, cloneDuration, canAttack);
    }

}

