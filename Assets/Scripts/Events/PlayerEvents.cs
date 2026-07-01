using System;
using UnityEngine;

public static class PlayerEvents
{
    public static Action OnFreezePlayer;
    public static Action OnUnfreezePlayer;

    public static void RaiseOnFreezePlayer()
    {
        OnFreezePlayer?.Invoke();
    }

    public static void RaiseOnUnfreezePlayer()
    {
        OnUnfreezePlayer?.Invoke();
    }
}
