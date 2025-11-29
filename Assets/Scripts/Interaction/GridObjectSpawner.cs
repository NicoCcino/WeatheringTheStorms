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
        Vector2Int coordinates = ev.Coordinates;
        if (!GridManager.Instance.IsCoordinatesAvailables(ev.Coordinates))
        {
            coordinates = GridManager.Instance.FindClosestAvailablesPosition(ev.Coordinates);
        }

        GameObject spawnedGameObject = SimplePool.Spawn(PrefabEventClickable);
        spawnedGameObject.transform.parent = transform;
        EventOnGrid eventOnGrid = spawnedGameObject.GetComponent<EventOnGrid>();
        eventOnGrid.Init(ev);

        GridManager.Instance.DisplayObjectOnGrid(spawnedGameObject, coordinates);
    }
    private void OnPromptTriggeredCallback(Prompt prompt)
    {
        Vector2Int coordinates = prompt.Coordinates;
        if (!GridManager.Instance.IsCoordinatesAvailables(prompt.Coordinates))
        {
            coordinates = GridManager.Instance.FindClosestAvailablesPosition(prompt.Coordinates);
        }

        GameObject spawnedGameObject = SimplePool.Spawn(PrefabPromptClickable);
        spawnedGameObject.transform.parent = transform;
        PromptClickable promptClickable = spawnedGameObject.GetComponent<PromptClickable>();
        promptClickable.Init(prompt);
        GridManager.Instance.DisplayObjectOnGrid(spawnedGameObject, coordinates);
    }
    private void OnComputePowerLootSpawnCallback(ComputePowerLootData data)
    {
        GameObject spawnedGameObject = SimplePool.Spawn(PrefabComputePowerClickable);
        spawnedGameObject.transform.parent = transform;
        ComputePowerClickable computePowerClickable = spawnedGameObject.GetComponent<ComputePowerClickable>();
        computePowerClickable.Init(data);
        computePowerClickable.LinkedGridObject.Coordinates = GridManager.Instance.GetAvailableRandomPositionOnGrid();
        GridManager.Instance.DisplayObjectOnGrid(spawnedGameObject, computePowerClickable.LinkedGridObject.Coordinates);
    }

}
