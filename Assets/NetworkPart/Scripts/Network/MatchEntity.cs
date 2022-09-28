using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace GameNetwork
{
    public class MatchEntity : NetworkBehaviour
    {
        [SerializeField] private Transform _spawnPoint; 
        
        public override void OnNetworkSpawn()
        {
            if(!IsServer) return;
            base.OnNetworkSpawn();
            
            NetworkManager.SceneManager.OnLoadEventCompleted += OnSceneLoaded;
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

        private void OnSceneLoaded(string sceneName, LoadSceneMode loadSceneMode, List<ulong> clientsCompleted, List<ulong> clientsTimedOut)
        {
            NetworkManager.SceneManager.OnLoadEventCompleted -= OnSceneLoaded;

            SpawnPlayerObjects();
        }

        private void SpawnPlayerObjects()
        {
            // Spawn player objects for every player
            foreach (var item in NetworkManager.Singleton.ConnectedClients)
            {
                PlayerEntity entity = item.Value.PlayerObject.GetComponent<PlayerEntity>();
                entity.SpawnPlayerObject(_spawnPoint.position);
            }
        }
    }
}