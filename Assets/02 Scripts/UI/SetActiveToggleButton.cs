using UnityEngine;
using UnityEngine.UI;

namespace _02_Scripts.UI
{
    [RequireComponent(typeof(Button))]
    public class SetActiveToggleButton : MonoBehaviour
    {
        public enum PanelAction
        {
            Toggle,
            Open,
            Close
        }

        [SerializeField] private SlidePanel slidePanel;
        [SerializeField] private PanelAction action = PanelAction.Toggle;

        private void Awake()
        {
            GetComponent<Button>().onClick.AddListener(Invoke);
        }

        public void Invoke()
        {
            if (slidePanel == null) return;

            switch (action)
            {
                case PanelAction.Open:
                    slidePanel.Open();
                    break;
                case PanelAction.Close:
                    slidePanel.Close();
                    break;
                default:
                    slidePanel.Toggle();
                    break;
            }
        }

        public void Toggle() => slidePanel.Toggle();
        public void Open() => slidePanel.Open();
        public void Close() => slidePanel.Close();
    }
}
