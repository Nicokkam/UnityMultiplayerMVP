using System.Collections;
using System.Collections.Generic;
using Unity.Collections;
using Unity.Netcode;
using UnityEngine;

public class PlayerNetworking : NetworkBehaviour
{
    [SerializeField] private Transform spawnedObjectPrefab;
    private Transform spawnedObjectTransform;

    public bool isOwner;

    private NetworkVariable<MyCustomData> randomNumber = new NetworkVariable<MyCustomData>(
        new MyCustomData {
            _int = 56,
            _bool = true,
        }, NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Owner);
    public struct MyCustomData : INetworkSerializable
    {
        public int _int;
        public bool _bool;
        public FixedString128Bytes message;

        public void NetworkSerialize<T>(BufferSerializer<T> serializer) where T : IReaderWriter
        {
            serializer.SerializeValue(ref _int);
            serializer.SerializeValue(ref _bool);
            serializer.SerializeValue(ref message);
        }
    }

    public override void OnNetworkSpawn()
    {
        randomNumber.OnValueChanged += (MyCustomData previousValue, MyCustomData newValue) =>
            Debug.Log($"{OwnerClientId} // {newValue._int} // {newValue._bool} // {newValue.message}");
    }

    void Update()
    {

        isOwner = IsOwner;

        if (isOwner == false) return;

        if (Input.GetKeyDown(KeyCode.T))
        {
            spawnedObjectTransform = Instantiate(spawnedObjectPrefab);
            spawnedObjectTransform.GetComponent<NetworkObject>().Spawn(true);

            //TestClientRpc(new ClientRpcParams { Send = new ClientRpcSendParams { TargetClientIds = new List<ulong> { 1 } } } ); //Send an RPC for a specific client that we want

            //TestServerRpc(new ServerRpcParams());

            //randomNumber.Value = new MyCustomData
            //{
            //    _int = Random.Range(0, 100),
            //    _bool = true ? false : true,
            //    message = "Testing the Networking System!",
            //};
        }

        if (Input.GetKeyDown(KeyCode.Y))
        {
            spawnedObjectTransform.GetComponent<NetworkObject>().Despawn(true);
            Destroy(spawnedObjectTransform.gameObject);
        }
    }

    [ServerRpc]
    private void TestServerRpc(ServerRpcParams serveRpcParams)
    {
        Debug.Log($"TestServerRPC || {OwnerClientId} || {serveRpcParams.Receive.SenderClientId}");
    }

    [ClientRpc]
    private void TestClientRpc(ClientRpcParams clientRpcParams)
    {
        Debug.Log($"TestClientRpc");
    }
}
