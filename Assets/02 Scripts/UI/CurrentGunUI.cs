using UnityEngine;
using UnityEngine.UI;

namespace _02_Scripts.UI
{
    public class CurrentGunUI : MonoBehaviour
    {
        [SerializeField] private Image pistolImage;
        [SerializeField] private Image shotGunImage;

        public void SetShotGunImage()
        {
            pistolImage.gameObject.SetActive(false);
            shotGunImage.gameObject.SetActive(true);
        }

        public void SetPistolImage()
        {
            shotGunImage.gameObject.SetActive(false);
            pistolImage.gameObject.SetActive(true);
        }
    }
}