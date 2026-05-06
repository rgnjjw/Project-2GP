using _02_Scripts.Core.ModuleSystem;
using _02_Scripts.Weapon.Gun.FireAction;
using UnityEngine;

namespace _02_Scripts.Weapon.Gun
{
    public class GunDataContainer : MonoBehaviour, IModule
    {
        [field: SerializeField] public GunDataSO GunDataSO {get; private set; }
        public IFire UseFireAction {get;set;}
        public int MagazineSize {get; set;}//탄장 총알수
        public int BulletDamage {get; set;}
        public float ReloadTime {get; set;}
        public float AttackSpeed {get; set;}


        public void Initialize(ModuleOwner moduleOwner)
        {
            MagazineSize = GunDataSO.MagazineSize;
            BulletDamage = GunDataSO.BulletDamage;
            ReloadTime = GunDataSO.ReloadTime;
            AttackSpeed = GunDataSO.AttackSpeed;
        }
    }
}