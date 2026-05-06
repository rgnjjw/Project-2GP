using System.Collections;
using _02_Scripts.Core.ModuleSystem;
using _02_Scripts.Weapon.Gun.FireAction;
using UnityEngine;
using UnityEngine.InputSystem;

namespace _02_Scripts.Weapon.Gun
{
    public class Gun : ModuleOwner
    {
        [SerializeField] private LayerMask layerMask;
        [SerializeField] private Transform fireTrm;
        [SerializeField] private Transform muzzleTrm;
        [SerializeField] private LineRenderer lineRenderer;
        
        private GunDataContainer _gunDataContainer;
        private int _currentMagazineAmmo;
        private bool _isCanFire = true ;
        
        public int CurrentMagazineAmmo
        {
            get => _currentMagazineAmmo;
            protected set
            {
                _currentMagazineAmmo = value;
                if (_currentMagazineAmmo == 0)
                {
                    _isCanFire = false;
                    Reload();
                }
            }
        }

        private void Update()
        {
            //test
            if (Mouse.current.leftButton.wasPressedThisFrame)
            {
                if (_isCanFire)
                {
                    Fire();
                    
                }

            }

            if (Keyboard.current.rKey.wasPressedThisFrame)
            {
                Reload();
            }
        }

        protected override void Awake()
        {
            base.Awake();
            Initialized();
        }

        protected virtual void Initialized()
        {
            _gunDataContainer = GetModule<GunDataContainer>();
            CurrentMagazineAmmo = _gunDataContainer.MagazineSize;
            _gunDataContainer.UseFireAction = new HitScanFireAction();
            lineRenderer.enabled = false; 
        }

        public void Reload()
        {
            //애니메이션 종료후 실행 애니메이션 종료 이벤트에 구독
            CurrentMagazineAmmo = _gunDataContainer.MagazineSize;
            _isCanFire = true;
        }

        public void Fire()
        {
            if (Camera.main != null)
            {
                Ray ray = Camera.main.ScreenPointToRay(new Vector3(Screen.width / 2f, Screen.height / 2f, 0));
                Vector3 hitPoint = Physics.Raycast(ray, out RaycastHit hit, layerMask) ? hit.point : ray.origin + ray.direction * 1000f;

                _gunDataContainer.UseFireAction.Fire(fireTrm, layerMask, _gunDataContainer.BulletDamage);
                StartCoroutine(DrawLine(muzzleTrm.position, hitPoint));
                CurrentMagazineAmmo -= 1;
            }
        }

        private IEnumerator DrawLine(Vector3 start, Vector3 end)
        {
            lineRenderer.enabled = true;
            lineRenderer.SetPosition(0, start);  // 총구
            lineRenderer.SetPosition(1, end);    // 조준점 히트 포인트
    
            yield return new WaitForSeconds(0.01f);
    
            lineRenderer.enabled = false;
        }
    }   
}