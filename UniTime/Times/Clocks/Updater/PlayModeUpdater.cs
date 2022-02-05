using System;
using UnityEditor;
using UnityEngine;

namespace Kratorg.Internal.Times
{
    internal sealed class PlayModeUpdater : Clock
    {
        public PlayModeUpdater()
        {
            EditorApplication.playModeStateChanged += OnPlayModeStateChanged;
        }

        void OnPlayModeStateChanged(PlayModeStateChange playModeState)
        {
            if (playModeState == PlayModeStateChange.EnteredPlayMode)
            {
                InstanciateInGameKeeper();
            }
        }

        void InstanciateInGameKeeper()
        {
            MonoBehaviourTimeKeeper timeKeeper = new GameObject().AddComponent<MonoBehaviourTimeKeeper>();
            timeKeeper.gameObject.name = "TimeKeeper";
            timeKeeper.update += Update;
        }

        public override void OnDispose()
        {
            EditorApplication.playModeStateChanged -= OnPlayModeStateChanged;
            base.OnDispose();
        }
    }

    internal sealed class MonoBehaviourTimeKeeper : MonoBehaviour
    {
        public event Action<double> update;

        private void Awake() => DontDestroyOnLoad(this);

        private void FixedUpdate()
        {
            update?.Invoke(UnityEngine.Time.fixedDeltaTime);
        }

        void OnDestroy()
        {
            update = null;
        }
    }
}