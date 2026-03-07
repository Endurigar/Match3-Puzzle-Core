using System;
#if !NOT_UNITY3D
using UnityEngine;
#endif

namespace Zenject
{
    public enum ValidationErrorResponses
    {
        Log,
        Throw
    }

    public enum RootResolveMethods
    {
        NonLazyOnly,
        All
    }

    public enum SignalDefaultSyncModes
    {
        Synchronous,
        Asynchronous
    }

    public enum SignalMissingHandlerResponses
    {
        Ignore,
        Throw,
        Warn
    }

    [Serializable]
    [ZenjectAllowDuringValidation]
    [NoReflectionBaking]
    public class ZenjectSettings
    {
        public static ZenjectSettings Default = new ZenjectSettings();

#if !NOT_UNITY3D
        [SerializeField]
#endif
        bool _ensureDeterministicDestructionOrderOnApplicationQuit;

#if !NOT_UNITY3D
        [SerializeField]
#endif
        bool _displayWarningWhenResolvingDuringInstall;

#if !NOT_UNITY3D
        [SerializeField]
#endif
        RootResolveMethods _validationRootResolveMethod;

#if !NOT_UNITY3D
        [SerializeField]
#endif
        ValidationErrorResponses _validationErrorResponse;

#if !NOT_UNITY3D
        [SerializeField]
#endif
        SignalSettings _signalSettings;

        public ZenjectSettings(
            ValidationErrorResponses validationErrorResponse,
            RootResolveMethods validationRootResolveMethod = RootResolveMethods.NonLazyOnly,
            bool displayWarningWhenResolvingDuringInstall = true,
            bool ensureDeterministicDestructionOrderOnApplicationQuit = false,
            SignalSettings signalSettings = null)
        {
            _validationErrorResponse = validationErrorResponse;
            _validationRootResolveMethod = validationRootResolveMethod;
            _displayWarningWhenResolvingDuringInstall = displayWarningWhenResolvingDuringInstall;
            _ensureDeterministicDestructionOrderOnApplicationQuit =ensureDeterministicDestructionOrderOnApplicationQuit;
            _signalSettings = signalSettings ?? SignalSettings.Default;
        }

        public ZenjectSettings()
            : this(ValidationErrorResponses.Log)
        {
        }

        public SignalSettings Signals
        {
            get { return _signalSettings; }
        }

        public ValidationErrorResponses ValidationErrorResponse
        {
            get { return _validationErrorResponse; }
        }

        public RootResolveMethods ValidationRootResolveMethod
        {
            get { return _validationRootResolveMethod; }
        }

        public bool DisplayWarningWhenResolvingDuringInstall
        {
            get { return _displayWarningWhenResolvingDuringInstall; }
        }

        public bool EnsureDeterministicDestructionOrderOnApplicationQuit
        {
            get { return _ensureDeterministicDestructionOrderOnApplicationQuit; }
        }

        [Serializable]
        public class SignalSettings
        {
            public static SignalSettings Default = new SignalSettings();

#if !NOT_UNITY3D
            [SerializeField]
#endif
            SignalDefaultSyncModes _defaultSyncMode;

#if !NOT_UNITY3D
            [SerializeField]
#endif
            SignalMissingHandlerResponses _missingHandlerDefaultResponse;

#if !NOT_UNITY3D
            [SerializeField]
#endif
            bool _requireStrictUnsubscribe;

#if !NOT_UNITY3D
            [SerializeField]
#endif
            int _defaultAsyncTickPriority;

            public SignalSettings(
                SignalDefaultSyncModes defaultSyncMode,
                SignalMissingHandlerResponses missingHandlerDefaultResponse = SignalMissingHandlerResponses.Warn,
                bool requireStrictUnsubscribe = false,
                int defaultAsyncTickPriority = 1)
            {
                _defaultSyncMode = defaultSyncMode;
                _missingHandlerDefaultResponse = missingHandlerDefaultResponse;
                _requireStrictUnsubscribe = requireStrictUnsubscribe;
                _defaultAsyncTickPriority = defaultAsyncTickPriority;
            }

            public SignalSettings()
                : this(SignalDefaultSyncModes.Synchronous)
            {
            }

            public int DefaultAsyncTickPriority
            {
                get { return _defaultAsyncTickPriority; }
            }

            public SignalDefaultSyncModes DefaultSyncMode
            {
                get { return _defaultSyncMode; }
            }

            public SignalMissingHandlerResponses MissingHandlerDefaultResponse
            {
                get { return _missingHandlerDefaultResponse; }
            }

            public bool RequireStrictUnsubscribe
            {
                get { return _requireStrictUnsubscribe; }
            }
        }
    }
}
