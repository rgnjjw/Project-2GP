using UnityEngine;
using UnityEngine.UI;

namespace _02_Scripts.UI
{
    public class CurrentGunUI : MonoBehaviour
    {
        [SerializeField] private Image pistolImage;
        [SerializeField] private Image shotGunImage;
        [SerializeField] private Image machineGunImage;

        public void SetPistolImage()
        {
            pistolImage.gameObject.SetActive(true);
            shotGunImage.gameObject.SetActive(false);
            machineGunImage.gameObject.SetActive(false);
        }

        public void SetShotGunImage()
        {
            pistolImage.gameObject.SetActive(false);
            shotGunImage.gameObject.SetActive(true);
            machineGunImage.gameObject.SetActive(false);
        }

        public void SetMachineGunImage()
        {
            pistolImage.gameObject.SetActive(false);
            shotGunImage.gameObject.SetActive(false);
            machineGunImage.gameObject.SetActive(true);
        }
    }
}
