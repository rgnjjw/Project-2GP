using UnityEngine;

namespace _02_Scripts
{
    public class CharacterRotator : MonoBehaviour
    {
        [SerializeField] private float rotateSpeed = 30f;
    
        void Update()
        {
            transform.Rotate(Vector3.up, rotateSpeed * Time.deltaTime);
        }
    }
}