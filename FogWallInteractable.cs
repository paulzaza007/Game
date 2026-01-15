using UnityEngine;

public class FogWallInteractable : Interactable
{
    [Header("Fog")]
    [SerializeField] GameObject[] fogGameObjects;

    [Header("I.D")]
    public int fogWallID;

    public bool isActive = false;
    public bool IsActive
    {
        get => isActive;
        set
        {
            if (isActive == value) return;

            bool old = isActive;
            isActive = value;

            OnActive?.Invoke(old, value);
        }
    }

    public event System.Action<bool, bool> OnActive;

    protected override void Awake()
    {
        OnActive += OnIsActiveChanaged;
    }

    protected override void Start()
    {
        Onspawn();
    }

    public void Onspawn()
    {
        OnIsActiveChanaged(false,isActive);
        OnActive += OnIsActiveChanaged;
        WorldObjectManager.instance.AddFogWallTolist(this);
    }

    public void OnDespawn()
    {
        OnActive -= OnIsActiveChanaged;
        WorldObjectManager.instance.RemoveFogWallTolist(this);
    }

    private void OnIsActiveChanaged(bool oldBool, bool newBool)
    {
        if (isActive)
        {
            foreach(var fogObject in fogGameObjects)
            {
                fogObject.SetActive(true);
            }
        }
        else
        {
            foreach (var fogObject in fogGameObjects)
            {
                fogObject.SetActive(false);
            }
        }
    }
}
