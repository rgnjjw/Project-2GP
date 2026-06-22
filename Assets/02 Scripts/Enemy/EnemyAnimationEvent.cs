using System;
using _02_Scripts.Core.ModuleSystem;
using UnityEngine;

namespace _02_Scripts.Enemy
{
    public class EnemyAnimationEvent : MonoBehaviour, IModule
    {
        public event Action OnAttackEnd;
        public event Action OnAttack;
        public event Action OnPrepare;
        public event Action OnDeath;

        public void Initialize(ModuleOwner owner) { }

        public void AttackEnd() => OnAttackEnd?.Invoke();
        public void Attack() => OnAttack?.Invoke();
        public void Prepare() => OnPrepare?.Invoke();
        public void Death() => OnDeath?.Invoke();

        // 공격이 OnAttack까지 도달하기 전에 중단(상태 전환)되면, 스킬이 등록해 둔
        // 익명 핸들러가 OnAttack/OnPrepare/OnAttackEnd에 그대로 남는다. 다음 공격 때 새 핸들러가
        // 더해져 한 번의 OnAttack에 데미지가 여러 번 적용된다(플레이어가 돌연사하는 원인).
        // 공격 진입 시점에 이 이벤트들을 깨끗이 비워 중복 누적을 막는다.
        // (이 세 이벤트의 구독자는 EnemyAttackState와 스킬뿐이며, 둘 다 공격마다 새로 등록된다.)
        public void ClearAttackEvents()
        {
            OnAttack = null;
            OnPrepare = null;
            OnAttackEnd = null;
        }
    }
}