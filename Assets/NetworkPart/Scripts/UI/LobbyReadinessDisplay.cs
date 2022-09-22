using GameNetwork;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace GameUI
{
    [RequireComponent(typeof(Button))]
    public class LobbyReadinessDisplay : MonoBehaviour
    {
        [SerializeField] private Lobby _lobby;

        [Header("Appearance")] [SerializeField] [ColorUsage(false)]
        private Color _readyColor;

        [SerializeField] [ColorUsage(false)] private Color _notReadyColor;
        [SerializeField] private string _readyText = "Ready";
        [SerializeField] private string _notReadyText = "Not ready";

        private Button _button;
        private TMP_Text _text;

        private void Start()
        {
            _button = GetComponent<Button>();
            _text = GetComponentInChildren<TMP_Text>();

            SetReadyColor(false);
        }

        public void ChangeReadiness()
        {
            bool currentState = _lobby.LocalPlayerReadiness;
            _lobby.RequestSwitchReadyStatus(!currentState);
            SetReadyColor(!currentState);
        }

        private void SetReadyColor(bool isReady)
        {
            _button.image.color = isReady ? _readyColor : _notReadyColor;

            if (_text != null)
                _text.text = isReady ? _readyText : _notReadyText;
        }
    }
}