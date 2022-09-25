using Unity.Netcode;
using UnityEngine;

namespace GameNetwork
{
    public class MatchEntity : NetworkBehaviour
    {
        [SerializeField] private Transform _spawnPoint; 
        
        public override void OnNetworkSpawn()
        {
            if(!IsServer) return;
            base.OnNetworkSpawn();

            // Spawn player objects for every player
            foreach (var item in NetworkManager.Singleton.ConnectedClients)
            {
                PlayerEntity entity = item.Value.PlayerObject.GetComponent<PlayerEntity>();
                entity.SpawnPlayerObject(_spawnPoint.position);
                // entity.UnlinkOtherPlayersInputClientRpc();
            }
        }

        public override void OnNetworkDespawn()
        {
            if(!IsServer) return;
            base.OnNetworkDespawn();


            foreach (var item in NetworkManager.Singleton.ConnectedClients)
            {
                NetworkObject playerObject =
                    GetNetworkObject(item.Value.PlayerObject.GetComponent<PlayerEntity>().PlayerObjectId);
                
                playerObject.Despawn();
            }
        }
    }
}