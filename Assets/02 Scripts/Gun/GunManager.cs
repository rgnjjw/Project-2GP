using _02_Scripts.Core.ModuleSystem;
using UnityEngine;

namespace _02_Scripts.Gun
{
    public class GunManager : MonoBehaviour, IModule
    {
        [SerializeField] private Gun[] weapons;
        private Gun _currentWeapon;
        private int _currentIndex;

        //총발사하다가 갑자가 Idle에서 갖히는거 고치기
        public void Initialize(ModuleOwner owner)
        {
            if (owner is Player.Player player)
            {
                SwapWeapon(1);
            }
        }

        public void Fire() => _currentWeapon.Fire();

        //나중에 인풋이랑 연결하기 (휠)
        public void SwapWeapon(int index)
        {
            if (index < 0 || index >= weapons.Length) return;
        
            _currentIndex = index;
            _currentWeapon = weapons[_currentIndex];
            _currentWeapon.Equip();
        }

        public void SwapNext() => SwapWeapon((_currentIndex + 1) % weapons.Length);
        public void SwapPrev() => SwapWeapon((_currentIndex - 1 + weapons.Length) % weapons.Length);
    }
}