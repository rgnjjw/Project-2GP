using System;
using _02_Scripts.Core.Utility;

namespace _02_Scripts.Manager
{
    public class CurrencyManager : MonoSingleton<CurrencyManager>
    {
        public int Currency { get; private set; }
        public event Action<int> OnCurrencyChanged;

        public void AddCurrency(int amount)
        {
            if (amount <= 0) return;
            Currency += amount;
            OnCurrencyChanged?.Invoke(Currency);
        }

        public bool TrySpend(int amount)
        {
            if (amount <= 0 || Currency < amount) return false;
            Currency -= amount;
            OnCurrencyChanged?.Invoke(Currency);
            return true;
        }
    }
}
