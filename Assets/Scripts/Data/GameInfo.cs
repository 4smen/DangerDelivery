using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;
using System;
using Multiplayer.Runtime;
using UnityEngine.Events;

public class GameInfo : NetworkBehaviour
{
    public static GameInfo Singletone { get; private set; } = null;

    [Header("Info")]
    public NetworkVariable<int> planDaysLeft = new NetworkVariable<int>();
    public NetworkVariable<int> planCount = new NetworkVariable<int>();
    public NetworkVariable<int> plan = new NetworkVariable<int>();
    List<string> planetsWeather = new List<string>();

    //2 - Testorios

    public NetworkVariable<int> choosedPlanet = new NetworkVariable<int>();

    public static event UnityAction<int> OnPlanetChoosedCallback;

    public static event UnityAction OnShipStartedCallback;
    public static event UnityAction OnShipLandedCallback;

    public static event UnityAction<int> OnPlanetChangingCallback;
    //public static event UnityAction OnCallback;

    private void Awake()
    {
        if (Singletone == null)
            Singletone = this;
        else
        {
            Destroy(gameObject);
            return;
        }

        DontDestroyOnLoad(gameObject);
    }

    void Start()
    {
        RandomizePlanetsWeather();
    }

    void RandomizePlanetsWeather()
    {

    }

    internal void InvokeOnPlanetChoosedCallback(int planetSceneId)
    {
        OnPlanetChoosedCallback?.Invoke(planetSceneId);
        choosedPlanet.Value = planetSceneId;
    }
    internal void InvokeOnShipStartedCallback()
    {
        OnShipStartedCallback?.Invoke();
    }
    internal void InvokeOnShipLandedCallback()
    {
        OnShipLandedCallback?.Invoke();
    }
    internal void InvokeOnPlanetChangingCallback(int planetSceneId)
    {
        OnPlanetChangingCallback?.Invoke(planetSceneId);
    }

    //internal void InvokeOnCallback()
    //{
    //    OnCallback?.Invoke();
    //}
}
