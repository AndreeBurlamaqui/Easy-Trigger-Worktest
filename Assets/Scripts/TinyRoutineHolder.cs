using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TinyCacto.Utils
{
    public class TinyRoutineHolder : MonoBehaviour
    {
        #region SINGLETON

        private static TinyRoutineHolder _instance;

        public static TinyRoutineHolder Instance
        {
            get
            {
                if (_instance == null)
                {
                    _instance = FindObjectOfType<TinyRoutineHolder>();
                }

                if (_instance == null)
                {
                    // If still null, create a the new instance

                    GameObject routineHolder = new GameObject("TinyRoutineHolder");
                    routineHolder.AddComponent<TinyRoutineHolder>();

                    _instance = routineHolder.GetComponent<TinyRoutineHolder>();
                }

                return _instance;
            }
        }

        private void Awake()
        {
            DontDestroyOnLoad(gameObject);
        }

        #endregion

    }
}