using Platformer.Mechanics;
using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class SpawnEnemy : NetworkBehaviour
{
    [SerializeField] GameObject enemyPrefabType1;
    [SerializeField] Transform enemyContainer;
    [SerializeField] PatrolPath enemyPath;

    public NetworkObject spawnedNetworkObject;

    public override void OnNetworkSpawn()
    {
        SpawnEnemies();
    }
    public void SpawnEnemies()
    {
        if (IsServer == false) return;

        Vector3 spawnPosition = transform.position;

        var additonalInstance = Instantiate(enemyPrefabType1, spawnPosition, Quaternion.identity, enemyContainer);
        if (additonalInstance.TryGetComponent<NetworkObject>(out var networkObject))
        {
            if (networkObject.IsSpawned)
                Debug.Log($"Network object {networkObject.NetworkObjectId} has already spawned for {additonalInstance.name}");
            else
            {
                spawnedNetworkObject = networkObject;
                if (enemyPath != null)
                    additonalInstance.GetComponent<EnemyController>().path = enemyPath;
                networkObject.Spawn();
            }
        }
    }
}