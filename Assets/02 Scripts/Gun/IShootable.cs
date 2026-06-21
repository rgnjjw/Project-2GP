namespace _02_Scripts.Gun
{
    // 총에 맞으면 반응하는 오브젝트. 총의 Fire 레이캐스트가 명중하면 OnShot()이 호출된다.
    // (구현체에는 총 레이캐스트(layerMask)가 맞을 수 있도록 Collider가 있어야 한다)
    public interface IShootable
    {
        void OnShot();
    }
}
