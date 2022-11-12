using Platformer.Mechanics;
using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class SpawnTokens : NetworkBehaviour
{
    [SerializeField] private List<Transform> tokenPositions;
    [SerializeField] private Transform tokenContainer;
    public GameObject tokenPrefab;

    public NetworkObject spawnedNetworkObject;

    //private void Awake()
    //{
    //    foreach(Transform child in transform)
    //    {
    //        tokenPositions.Add(child.position);
    //    }
    //}

    public override void OnNetworkSpawn()
    {
        SpawnTheTokens();
    }
    public void SpawnTheTokens()
    {
        if (IsServer == false) return;

        foreach(Transform tokenTransform in tokenPositions)
        {
            Vector3 tokenPos = tokenTransform.position;
            GameObject tokenInstance = Instantiate(tokenPrefab, tokenPos, Quaternion.identity, tokenContainer);
            if (tokenInstance.TryGetComponent<NetworkObject>(out var networkObject))
            {
                if (networkObject.IsSpawned)
                    Debug.Log($"Network object {networkObject.NetworkObjectId} has already spawned for {tokenInstance.name}");
                else
                {
                    spawnedNetworkObject = networkObject;
                    //tokenInstance.GetComponent<TokenInstance>().startTokenAnimation();
                    networkObject.Spawn();
                }
            }
        }
    }
}
