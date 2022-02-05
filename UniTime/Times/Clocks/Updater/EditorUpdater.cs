using System;
using UnityEditor;

namespace Kratorg.Internal.Times
{
    internal sealed class EditorUpdater : Clock
    {
        DateTime _lastFrame;

        public EditorUpdater()
        {
            _lastFrame = DateTime.Now;
            EditorApplication.update += OnEditorUpdate;
        }

        private void OnEditorUpdate()
        {
            TimeSpan past = DateTime.Now - _lastFrame;
            Update(past.TotalSeconds);
            _lastFrame = DateTime.Now;
        }

        public override void OnDispose()
        {
            EditorApplication.update -= OnEditorUpdate;
            base.OnDispose();
        }
    }
}