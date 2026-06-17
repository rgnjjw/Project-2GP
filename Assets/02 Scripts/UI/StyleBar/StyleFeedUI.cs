using UnityEngine;

namespace _02_Scripts.UI.StyleBar
{
    public class StyleFeedUI : MonoBehaviour
    {
        [SerializeField] private StyleFeedItem feedItemPrefab;
        [SerializeField] private Transform container;

        public void AddFeed(string message)
        {
            StyleFeedItem item = Instantiate(feedItemPrefab, container);
            item.transform.SetAsFirstSibling();
    
            RectTransform rt = item.GetComponent<RectTransform>();
            rt.localPosition = new Vector3(rt.localPosition.x, rt.localPosition.y, 0f);
    
            item.Setup(message, 1f);
        }   
    }
}
