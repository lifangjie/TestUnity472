/**
 * MainThreadManager.cs
 * Created by: lfj20
 * Created on: 2017/12/29
 */

using System;
using System.Collections.Generic;
using UnityEngine;

namespace _Scripts.Util {
    public class MainThreadManager : MonoBehaviour {
        public delegate void UpdateEvent();

        public static UpdateEvent UnityUpdate = null;
        private static readonly UpdateEvent UnityFixedUpdate = null;

        /// <summary>
        /// The singleton instance of the Main Thread Manager
        /// </summary>
        private static MainThreadManager _instance;

        public static MainThreadManager Instance {
            get {
                if (_instance == null)
                    Create();

                return _instance;
            }
        }

        /// <summary>
        /// This will create a main thread manager if one is not already created
        /// </summary>
        public static void Create() {
            if (_instance != null)
                return;

            ThreadManagement.Initialize();

            if (!ReferenceEquals(_instance, null))
                return;

            new GameObject("Main Thread Manager").AddComponent<MainThreadManager>();
        }

        /// <summary>
        /// A list of functions to run
        /// </summary>
        private static readonly Queue<Action> MainThreadActions = new Queue<Action>();

        private static readonly Queue<Action> MainThreadActionsRunner = new Queue<Action>();

        // Setup the singleton in the Awake
        private void Awake() {
            // If an instance already exists then delete this copy
            if (_instance != null) {
                Destroy(gameObject);
                return;
            }

            // Assign the static reference to this object
            _instance = this;

            // This object should move through scenes
            DontDestroyOnLoad(gameObject);
        }

        public void Execute(Action action) {
            Run(action);
        }

        /// <summary>
        /// Add a function to the list of functions to call on the main thread via the Update function
        /// </summary>
        /// <param name="action">The method that is to be run on the main thread</param>
        public static void Run(Action action) {
            // Only create this object on the main thread
#if UNITY_WEBGL
			if (ReferenceEquals(Instance, null)) {
#else
            if (ReferenceEquals(Instance, null) && ThreadManagement.IsMainThread) {
#endif
                Create();
            }

            // Make sure to lock the mutex so that we don't override
            // other threads actions
            lock (MainThreadActions) {
                MainThreadActions.Enqueue(action);
            }
        }

        private void HandleActions() {
            lock (MainThreadActions) {
                // Flush the list to unlock the thread as fast as possible
                if (MainThreadActions.Count > 0) {
                    while (MainThreadActions.Count > 0)
                        MainThreadActionsRunner.Enqueue(MainThreadActions.Dequeue());
                }
            }

            // If there are any functions in the list, then run
            // them all and then clear the list
            if (MainThreadActionsRunner.Count > 0) {
                while (MainThreadActionsRunner.Count > 0)
                    MainThreadActionsRunner.Dequeue()();
            }
        }

        private void FixedUpdate() {
            HandleActions();

            if (UnityFixedUpdate != null)
                UnityFixedUpdate();
        }

#if WINDOWS_UWP
		public static async void ThreadSleep(int length) {
#else
        public static void ThreadSleep(int length) {
#endif 
#if WINDOWS_UWP
			await System.Threading.Tasks.Task.Delay(System.TimeSpan.FromSeconds(length));
#else
            System.Threading.Thread.Sleep(length);
#endif
        }
    }
}