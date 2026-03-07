using TMPro;
using UnityEngine;
using Zenject;

namespace Assets.Scripts.Game.UI.Base
{
    [RequireComponent(typeof(TextMeshProUGUI))]
    public abstract class TextSignalUI<TSignal> : MonoBehaviour
    {
        private TextMeshProUGUI _textField;
        private SignalBus _signalBus;

        [Inject]
        public void Construct(SignalBus signalBus)
        {
            _signalBus = signalBus;
        }

        protected virtual void Awake()
        {
            _textField = GetComponent<TextMeshProUGUI>();
        }

        protected virtual void Start()
        {
            _signalBus.Subscribe<TSignal>(OnSignalReceived);
        }

        protected virtual void OnDestroy()
        {
            _signalBus.Unsubscribe<TSignal>(OnSignalReceived);
        }

        private void OnSignalReceived(TSignal signal)
        {
            if (_textField != null)
            {
                UpdateText(signal, _textField);
            }
        }

        protected abstract void UpdateText(TSignal signal, TextMeshProUGUI textField);
    }
}