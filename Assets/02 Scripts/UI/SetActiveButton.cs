using UnityEngine;
using _02_Scripts.Manager;

namespace _02_Scripts.UI
{
    public class SetActiveButton : MonoBehaviour
    {
        [SerializeField] private GameObject panelRoot;

        public void Open()
        {
            panelRoot.SetActive(true);
            PauseManager.Instance.OpenSubPanel();
        }

        public void Close()
        {
            panelRoot.SetActive(false);
            PauseManager.Instance.CloseSubPanel();
        }

        private void OnDisable()
        {
            if (PauseManager.Instance != null)
            {
                PauseManager.Instance.CloseSubPanel();
            }
        }
    }
}