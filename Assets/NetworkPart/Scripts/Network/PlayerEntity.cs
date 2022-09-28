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

        public override void OnNetworkSpawn()
        {
            base.OnNetworkSpawn();
            gameObject.name = $"Player_{OwnerClientId}";
        }

        public void SpawnPlayerObject(Vector3 position)
        {
            Assert.IsTrue(IsServer, "It is supposed that server spawns everything!");
            
            NetworkObject playerObject = Instantiate(_playerObjectPrefab);
            playerObject.transform.position = position;
            playerObject.SpawnWithOwnership(OwnerClientId, true);
            playerObject.name = $"PlayerObject_{OwnerClientId}";
            _playerObject.Value = playerObject;
        }
    }
}