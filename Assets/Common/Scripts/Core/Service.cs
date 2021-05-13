using UnityEngine;
using UnityEditor;

namespace MonsterWorld.Unity
{
    public abstract class Service<T> : MonoBehaviour where T : Service<T>
    {
        public static T Instance { get; private set; } = null;

        protected abstract void Initialize();
        protected abstract void Dispose();

        private void Start()
        {
            if (Instance != null)
            {
                Destroy(this);
                return;
            }
            DontDestroyOnLoad(this);
            Instance = (T)this;

            Initialize();
        }

        private void OnDestroy()
        {
            Dispose();
        }
    }
}