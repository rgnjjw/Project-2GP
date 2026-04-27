using UnityEngine.InputSystem;

namespace _02_Scripts.Chip
{
    public interface IChip
    {
        ChipEnum ChipType { get; }
        string Name { get; }
        string Description { get; }
        void Execute(InputAction.CallbackContext context);
    }
}