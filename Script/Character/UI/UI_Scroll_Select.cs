using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class UI_Scroll_Select : MonoBehaviour
{
    [SerializeField] GameObject currentSelected;
    [SerializeField] GameObject previosSelected;
    [SerializeField] RectTransform currentSelectedTransform;
    
    [SerializeField] RectTransform contentPanel;
    [SerializeField] ScrollRect scrollRect;

    private void Update()
    {
        currentSelected = EventSystem.current.currentSelectedGameObject;

        if(currentSelected != null)
        {
            if (currentSelected != previosSelected)
            {
                previosSelected = currentSelected;
                currentSelectedTransform = currentSelected.GetComponent<RectTransform>();
                SnapTo(currentSelectedTransform);
            }
        }
    }

    private void SnapTo(RectTransform target)
    {
        Canvas.ForceUpdateCanvases();

        Vector2 newPosition = 
        (Vector2)scrollRect.transform.InverseTransformPoint(contentPanel.position) - (Vector2)scrollRect.transform.InverseTransformPoint(target.position);

        newPosition.x = 0;

        contentPanel.anchoredPosition = newPosition;
    }
}
