﻿using System;
using System.Collections.Generic;
using System.Text;
using Unity.Netcode;
using UnityEngine;

namespace GameNetwork
{
    public class Lobby : NetworkBehaviour
    {
        private NetworkDictionary<ulong, bool> _playersReadiness;
        public NetworkDictionary<ulong, bool> PlayersReadiness => _playersReadiness;
        
        // ReSharper disable once SimplifyConditionalTernaryExpression
        public bool LocalPlayerReadiness => _playersReadiness.ContainsKey(NetworkManager.LocalClientId)
            ? _playersReadiness[NetworkManager.LocalClientId] 
            : false;

        public Action OnInitialized;

        private void Awake()
        {
            _playersReadiness = new NetworkDictionary<ulong, bool>(
                NetworkVariableReadPermission.Everyone, 
                NetworkVariableWritePermission.Server, 
                new Dictionary<ulong, bool>());
        }

        public override void OnNetworkSpawn()
        {
            if (IsServer)
            {
                _playersReadiness[NetworkManager.LocalClientId] = false;  
                
                NetworkManager.OnClientConnectedCallback += OnPlayerAdd;
                NetworkManager.OnClientDisconnectCallback += OnPlayerRemove;
            }

            _playersReadiness.OnDictionaryChanged += DebugLobby;

            base.OnNetworkSpawn();
            
            OnInitialized?.Invoke();
            OnInitialized = null;
            
            DebugLobby(default);
        }

        public override void OnNetworkDespawn()
        {
            if (IsServer)
            {
                NetworkManager.OnClientConnectedCallback -= OnPlayerAdd;
                NetworkManager.OnClientDisconnectCallback -= OnPlayerRemove;
            }
            
            _playersReadiness.OnDictionaryChanged -= DebugLobby;
            
            base.OnNetworkDespawn();
        }

        public void RequestSwitchReadyStatus(bool status)
        {
            if (IsServer)
            {
                _playersReadiness[NetworkManager.LocalClientId] = status;
            }
            else
            {
                SwitchStatusRpcServerRpc(status);
            }
        }

        [ServerRpc(RequireOwnership = false)]
        private void SwitchStatusRpcServerRpc(bool status, ServerRpcParams rpcParams = default)
        {
            _playersReadiness[rpcParams.Receive.SenderClientId] = status;
        }
        
        private void OnPlayerAdd(ulong clientId)
        {
            if(!_playersReadiness.ContainsKey(clientId))
                _playersReadiness.Add(clientId, false);
        }

        private void OnPlayerRemove(ulong clientId)
        {
            if(_playersReadiness.ContainsKey(clientId))
                _playersReadiness.Remove(clientId);
        }
        
        private void DebugLobby(NetworkDictionaryEvent<ulong, bool> @event)
        {
            string mode = NetworkManager.IsHost ? "Host" :
                NetworkManager.IsServer ? "Server" : "Client";

            StringBuilder builder = new StringBuilder(64);
            builder.AppendLine("[Lobby] Mode: " + mode);
            builder.AppendLine("Current lobby list: ");
            foreach (var item in _playersReadiness)
            {
                builder.AppendFormat("{0} {1}\n", item.Key, item.Value);
            }
            Debug.Log(builder.ToString());
        }
    }
}