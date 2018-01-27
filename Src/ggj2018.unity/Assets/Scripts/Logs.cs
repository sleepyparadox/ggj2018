using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace Assets.Scripts
{
    public class Logs : MonoBehaviour
    {
        List<string> _logs = new List<string>();
        static Logs S;

        void Awake()
        {
            S = this;
        }

        public static void Log(string msg)
        {
            S.LogInternal(msg);
        }

        void LogInternal(string msg)
        {
            _logs.Insert(0, msg);

            if (_logs.Count > 1000)
                _logs.RemoveAt(_logs.Count - 1);
        }

        void Update() { /* Get enable toggle */ }

        void OnGUI()
        {
            if(this.gameObject.activeInHierarchy)
            {
                GUI.Window(0, new Rect(0, 0, UnityEngine.Screen.width, UnityEngine.Screen.height), DrawLogs, "Logs");
            }
        }

        void DrawLogs(int id)
        {
            foreach (var log in _logs)
            {
                GUILayout.Label(log);
            }
        }
    }
}
