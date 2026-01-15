using UnityEngine;
using UnityEngine.UI;


public class UI_StatBar : MonoBehaviour
{
    protected Slider slider;
    protected RectTransform rectTransform;

    [Header("Bar Options")]
    [SerializeField] protected bool scaleBarLengthWithStats = true;
    [SerializeField] protected float widthScaleMutiplier = 1;   

    protected virtual void Awake()
    {
        slider = GetComponent<Slider>();
        rectTransform = GetComponent<RectTransform>();
    }

    protected virtual void Start()
    {
        
    }

    public virtual void SetStat(int newValue)
    {
        slider.value = newValue;    
        Debug.Log(newValue);
    }

    public virtual void SetMaxStat(float maxValue)
    {
        slider.maxValue = maxValue;

        if (scaleBarLengthWithStats)
        {
            rectTransform.sizeDelta = new Vector2(maxValue * widthScaleMutiplier, rectTransform.sizeDelta.y);
            PlayerUIManager.instance.playerUIHUDManager.RefeshHUD();
        }
    }
}
