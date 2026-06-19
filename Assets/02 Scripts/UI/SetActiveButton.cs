using UnityEngine;

namespace _02_Scripts.UI
{
    public class SetActiveButton : MonoBehaviour
    {
        [SerializeField] private SlidePanel slidePanel;

        public void Toggle()
        {
            slidePanel.Toggle();
        }

        public void Close()
        {
            slidePanel.Close();
        }
    }
}