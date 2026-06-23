using DG.Tweening;
using _02_Scripts.Agent;
using _02_Scripts.Manager;
using _02_Scripts.Player.FSM;
using _02_Scripts.UI;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;
using EventBus = _02_Scripts.Core.Utility.EventBus;

namespace _02_Scripts.Player
{
    public class Player : Agent.Agent
    {
        [field: SerializeField] public PlayerInputSO PlayerInputSO { get; private set; }
        public bool IsWallRiding { get; set; }
        public event System.Action OnWallRideEnded;
        public void FireWallRideEnded() => OnWallRideEnded?.Invoke();
        [SerializeField] private BarUI barUI;
        [SerializeField] private Image redPanel;
        [SerializeField] private RecoilEvent recoilEvent;

        [Header("사망 시")]
        [Tooltip("죽었을 때 보여줄 패널('메인으로 가기' 버튼 포함). 버튼엔 SceneChangeButton(Title) 연결.")]
        [SerializeField] private GameObject deathPanel;

        [Tooltip("HP가 0이 된 뒤 사망 처리(패널 표시/게임 정지)까지의 지연(초). 체력바가 0으로 바뀌는 걸 보여줄 시간.")]
        [SerializeField] private float deathDelay = 2f;

        private Controls _controls;
        private GunManager _gunManager;
        private AgentHealth _playerHealth;

        private Tween _damageTween;

        protected override void Awake()
        {
            // 이전 세션(다시하기 전)의 죽은 입력 구독을 먼저 정리한다.
            // base.Awake()에서 모듈들이 다시 구독하므로 그 전에 비워야 한다.
            if (PlayerInputSO != null)
                PlayerInputSO.ClearAllEvents();

            base.Awake();

            _controls = new Controls();
            _controls.Player.SetCallbacks(PlayerInputSO);
            _controls.Player.Enable();

            _gunManager = GetModule<GunManager>();
            _playerHealth = GetModule<AgentHealth>();

            _playerHealth.CurrentHp.OnValueChanged += OnHpChanged;

            Color color = redPanel.color;
            color.a = 0f;
            redPanel.color = color;

            ChangeState(PlayerStateEnum.IDLE);
        }

        private void OnHpChanged(int before, int current)
        {
            barUI.SetFill((float)current / _playerHealth.MaxHp);

            if (current < before)
                PlayDamageEffect();
            
            EventBus.Publish(recoilEvent);
        }

        private void PlayDamageEffect()
        {
            _damageTween?.Kill();

            Color color = redPanel.color;
            color.a = 0.4f;
            redPanel.color = color;

            _damageTween = redPanel
                .DOFade(0f, 0.3f)
                .SetEase(Ease.OutQuad);
        }

        protected override void OnDead()
        {
            // HP가 0이 된 직후 바로 멈추면 체력바가 0으로 바뀌는 게 안 보인다.
            // deathDelay만큼 기다린 뒤 사망 처리(정지/패널)를 한다.
            StartCoroutine(DeathSequence());
        }

        private System.Collections.IEnumerator DeathSequence()
        {
            // 대기 중엔 timeScale이 정상(1)이라 체력바가 0으로 갱신되는 걸 보여줄 수 있다.
            if (deathDelay > 0f)
                yield return new WaitForSecondsRealtime(deathDelay);

            // 플레이어 사망 → 게임 멈추고 죽음 패널 표시. 이동은 패널의 '메인으로' 버튼이 처리.
            Time.timeScale = 0f;

            if (deathPanel != null)
                deathPanel.SetActive(true);

            if (CursorManager.Instance != null)
                CursorManager.Instance.SetCursorVisible(true);
        }

        protected void OnDestroy()
        {
            _playerHealth.CurrentHp.OnValueChanged -= OnHpChanged;

            // Controls는 C# 객체라 Player가 파괴돼도 InputSystem에 등록된 채 살아남는다.
            // Disable/Dispose하지 않으면, 씬 재시작 때마다 이전 세션의 Controls가
            // 그대로 살아 같은 PlayerInputSO 콜백을 계속 호출한다. → 키 한 번에
            // 점프/대쉬 이벤트가 N번 발생(점프가 비정상적으로 세지고, 대쉬가 한 번에 다 소모됨).
            if (_controls != null)
            {
                _controls.Player.Disable();
                _controls.Player.RemoveCallbacks(PlayerInputSO);
                _controls.Dispose();
                _controls = null;
            }
        }

        public void ChangeState(PlayerStateEnum nextState)
            => stateMachine.ChangeState((int)nextState);
    }
}