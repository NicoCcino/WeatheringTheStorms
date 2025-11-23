using System;
using UnityEngine;

public class GridObjectSpawner : MonoBehaviour
{
    [Header("Spawnable Objects")]
    public GameObject PrefabEventClickable;
    public GameObject PrefabPromptClickable;
    public GameObject PrefabComputePowerClickable;


    private void OnEnable()
    {
        EventManager.Instance.OnEventTriggered += OnEventTriggeredCallback;
        PromptManager.Instance.OnPromptSpawned += OnPromptTriggeredCallback;
        ComputePower.Instance.OnComputePowerLootSpawn += OnComputePowerLootSpawnCallback;
    }

    private void OnDisable()
    {
        EventManager.Instance.OnEventTriggered -= OnEventTriggeredCallback;
        PromptManager.Instance.OnPromptSpawned -= OnPromptTriggeredCallback;
        ComputePower.Instance.OnComputePowerLootSpawn -= OnComputePowerLootSpawnCallback;
    }


    private void OnEventTriggeredCallback(Event ev)
    {
        if (!GridManager.Instance.IsCoordinatesAvailables(ev.Coordinates))
            return;

        GameObject spawnedGameObject = SimplePool.Spawn(PrefabEventClickable);
        spawnedGameObject.transform.parent = transform;
        EventOnGrid eventOnGrid = spawnedGameObject.GetComponent<EventOnGrid>();
        eventOnGrid.Init(ev);

        GridManager.Instance.DisplayObjectOnGrid(spawnedGameObject,ev.Coordinates);
    }
    private void OnPromptTriggeredCallback(Prompt prompt)
    {
        if (!GridManager.Instance.IsCoordinatesAvailables(prompt.Coordinates))
            return;

        GameObject spawnedGameObject = SimplePool.Spawn(PrefabPromptClickable);
        spawnedGameObject.transform.parent = transform;
        PromptClickable promptClickable = spawnedGameObject.GetComponent<PromptClickable>();
        promptClickable.Init(prompt);
        GridManager.Instance.DisplayObjectOnGrid(spawnedGameObject, promptClickable.LinkedGridObject.Coordinates);
    }
    private void OnComputePowerLootSpawnCallback(ComputePowerLootData data)
    {
        if (!GridManager.Instance.IsCoordinatesAvailables(data.Coordinates))
            return;

        GameObject spawnedGameObject = SimplePool.Spawn(PrefabComputePowerClickable);
        spawnedGameObject.transform.parent = transform;
        ComputePowerClickable computePowerClickable = spawnedGameObject.GetComponent<ComputePowerClickable>();
        computePowerClickable.Init(data);
        computePowerClickable.LinkedGridObject.Coordinates = GridManager.Instance.GetRandomPositionOnGrid();
        GridManager.Instance.DisplayObjectOnGrid(spawnedGameObject, computePowerClickable.LinkedGridObject.Coordinates);
    }
}
