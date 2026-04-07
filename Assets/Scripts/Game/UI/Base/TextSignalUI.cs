using TMPro;
using UnityEngine;
using Zenject;

namespace Assets.Scripts.Game.UI.Base
{
    /// <summary>
    /// Abstract base class for UI elements that update their text based on a Zenject signal.
    /// Automatically handles subscription and unsubscription to the signal.
    /// </summary>
    /// <typeparam name="TSignal">The type of signal to listen for.</typeparam>
    [RequireComponent(typeof(TextMeshProUGUI))]
    public abstract class TextSignalUI<TSignal> : MonoBehaviour
    {
        private TextMeshProUGUI _textField;
        private SignalBus _signalBus;

        /// <summary>
        /// Injects the Zenject signal bus dependency.
        /// </summary>
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

        /// <summary>
        /// Derived classes must implement this to update the text field with data from the signal.
        /// </summary>
        /// <param name="signal">The received signal instance.</param>
        /// <param name="textField">The TextMeshProUGUI component to update.</param>
        protected abstract void UpdateText(TSignal signal, TextMeshProUGUI textField);
    }
}
 village