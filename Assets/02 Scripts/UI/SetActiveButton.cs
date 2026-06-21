using UnityEngine;

namespace _02_Scripts.UI
{
    public class SetActiveButton : MonoBehaviour
    {
        [SerializeField] private GameObject target;
        public void OnButtonPressed()
        {
            target.SetActive(true);
        }
    }
}