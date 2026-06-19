using _02_Scripts.Agent;
using _02_Scripts.Gun.Skill;
using UnityEngine;

namespace _02_Scripts.Gun.G_MachineGun
{
    public class MachineGun : Gun
    {
        [SerializeField] private Transform muzzleRight;
        [SerializeField] private Transform muzzleLeft;
        [SerializeField] private ParticleSystem fireEffectRight;
        [SerializeField] private ParticleSystem fireEffectLeft;
        [SerializeField] private MachineGunSkillDataSO skillData;
        [SerializeField] private GunAnimationEvent animEvent;

        [Header("Beam (LineRenderer)")]
        [SerializeField] private LineRenderer fireBeam;

        public override bool IsAutoFire => true;
        public bool LastFiredLeft { get; private set; }
        [field:SerializeField] public bool IsSkillActive { get; private set; }

        private bool _isLeft;
        private int _skillLevel = 1;
        private float _skillCooldownRemaining;//쿨타임
        private float _skillTimeRemaining;//스킬의 지속시간
    
        public bool IsSkillReady => _skillCooldownRemaining <= 0f;

        public override void SetSkillLevel(int level) => _skillLevel = level;

        private void Awake()
        {
            if (animEvent != null)
                animEvent.OnSkillFire += SkillFireShot;
        }

        private void OnDestroy()
        {
            if (animEvent != null)
                animEvent.OnSkillFire -= SkillFireShot;
        }

        public override void OnSkillPressed()
        {
            if (skillData == null || !IsSkillReady || IsSkillActive) return;

            _skillTimeRemaining = skillData.GetLevel(_skillLevel).Duration;//시간
            IsSkillActive = true;//실행중이다
            _isLeft = false; //스킬 애니메이션은 항상 오른쪽 먼저 왼쪽 순서로 고정되어 있으므로 시작 시 초기화함
            FireSkillStart();
        }

        public override void TickSkill(float deltaTime)
        {
            if (_skillCooldownRemaining > 0f)
                _skillCooldownRemaining -= deltaTime;

            if (!IsSkillActive) return;
            _skillTimeRemaining -= deltaTime;
            if (_skillTimeRemaining <= 0f)
            {
                IsSkillActive = false;
                FireSkillEnd();
                if (skillData != null)
                    _skillCooldownRemaining = skillData.GetLevel(_skillLevel).Cooldown;
            }
        }

        private void SkillFireShot()
        {
            if (!IsSkillActive) return;
            FireShot();
            PlayFireFeedbackOnly();
        }

        public override void Fire()
        {
            if (IsSkillActive) return;
            if (Time.time < nextFireTime) return;
            nextFireTime = Time.time + fireDelay;

            FireShot();
            base.Fire();
        }

        private void FireShot()
        {
            if (Camera.main == null) return;

            LastFiredLeft = _isLeft;
            Transform currentMuzzle = _isLeft ? muzzleLeft : muzzleRight;
            _isLeft = !_isLeft;

            Ray ray = Camera.main.ScreenPointToRay(new Vector3(Screen.width * 0.5f, Screen.height * 0.5f, 0));

            Vector3 endPoint = ray.origin + ray.direction * 1000f;
            if (Physics.Raycast(ray, out RaycastHit hit, Mathf.Infinity, layerMask))
            {
                endPoint = hit.point;

                hitEffect.transform.position = hit.point;
                hitEffect.transform.forward = hit.normal;
                hitEffect.Emit(1);

                if (hit.transform.TryGetComponent<Enemy.Enemy>(out var enemy))
                    DealDamage(enemy.GetModule<AgentHealth>(), bulletDamage);
            }

            // 발사한 총구 → 끝점(조준점/벽)을 잇는 빔
            ShowBeam(fireBeam, currentMuzzle.position, endPoint);
        }

        protected override void PlayFireEffect()
        {
            (LastFiredLeft ? fireEffectLeft : fireEffectRight)?.Play();
        }
    }
}