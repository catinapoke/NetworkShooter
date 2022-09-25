using System;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.Assertions;

namespace GameNetwork
{
    public class PlayerEntity : NetworkBehaviour
    {
        [SerializeField] private NetworkObject _playerObjectPrefab;
        
        private NetworkVariable<NetworkObjectReference> _playerObject = new NetworkVariable<NetworkObjectReference>();
        
        private NetworkObject LocalPlayerObject => GetNetworkObject(_playerObject.Value.NetworkObjectId);
        

        public ulong PlayerObjectId => _playerObject.Value.NetworkObjectId;

        public void SpawnPlayerObject(Vector3 position)
        {
            Assert.IsTrue(IsServer, "It is supposed that server spawns everything!");
            SpawnPlayerServerRpc(position);
        }

        [ServerRpc(RequireOwnership = false)]
        private void SpawnPlayerServerRpc(Vector3 position)
        {
            NetworkObject playerObject = Instantiate(_playerObjectPrefab);
            playerObject.transform.position = position;
            playerObject.SpawnWithOwnership(OwnerClientId, true);
            _playerObject.Value = playerObject;
        }
    }
}