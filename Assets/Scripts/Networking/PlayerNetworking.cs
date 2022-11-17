using Cinemachine;
using Platformer.Mechanics;
using Platformer.UI;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.Collections;
using Unity.Netcode;
using UnityEngine;

public class PlayerNetworking : NetworkBehaviour
{
    [SerializeField] private Transform spawnedObjectPrefab;
    private Transform spawnedObjectTransform;

    public bool isOwner;

    public GameObject GameControllerPrefab;

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
        if (IsSpawned && IsOwnedByServer)
            ChangeHostColor();

        if (IsOwner)
        {
            Camera c = Camera.main;
            CinemachineVirtualCamera vc = c.GetComponent<CinemachineVirtualCamera>();
            vc.Follow = transform;
            vc.LookAt = transform;
            spawnGameController();
        }

        randomNumber.OnValueChanged += (MyCustomData previousValue, MyCustomData newValue) =>
            Debug.Log($"{OwnerClientId} // {newValue._int} // {newValue._bool} // {newValue.message}");

    }

    private void ChangeHostColor()
    {
        transform.GetComponent<SpriteRenderer>().color = new Color32(207, 253, 46, 255);
    }

    private void spawnGameController()
    {
        var gameController = Instantiate(GameControllerPrefab);
        var gameControllerCS = gameController.GetComponent<GameController>();
        var metaGameControllerCS = gameController.GetComponent<MetaGameController>();
        var token1 = GameObject.FindWithTag("SpawnPoint1").transform;
        var token2 = GameObject.FindWithTag("SpawnPoint2").transform;

        gameControllerCS.model.virtualCamera = Camera.main.GetComponent<CinemachineVirtualCamera>();
        metaGameControllerCS.mainMenu = GameObject.FindWithTag("Canvas").GetComponent<MainUIController>();
        gameControllerCS.model.player = gameObject.GetComponent<PlayerController>();
        if (IsOwnedByServer)
            gameControllerCS.model.spawnPoint = token1;
        else
            gameControllerCS.model.spawnPoint = token2;

        metaGameControllerCS.ToggleMainMenu(true);
        metaGameControllerCS.ToggleMainMenu(false);
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
