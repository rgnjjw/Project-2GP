using UnityEngine;

namespace _02_Scripts.Weapon.Gun
{
    [CreateAssetMenu(fileName = "GunDataSO", menuName = "GunDataSO", order = 0)]
    public class GunDataSO : ScriptableObject
    {
        [field: SerializeField] public ParticleSystem FireParticle {get;private set;}
        [field: SerializeField] public int MagazineSize {get;private set;}//탄장 총알수
        [field: SerializeField] public int BulletDamage {get;private set;}
        [field: SerializeField] public float ReloadTime {get;private set;}
        [field: SerializeField] public float AttackSpeed {get;private set;}
        //애니메이션 추가하기
    }
}