using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace Assets.Scripts
{
    public class Canvas : MonoBehaviour
    {
        public static Canvas S { get; private set; }
        public Transform LobbyScreen;
        public Transform GameScreen;
        public Transform WinnerScreen;

        void Awake()
        {
            S = this;
        }

        public void SetScreen(Screen screen)
        {
            S = this;
            LobbyScreen.gameObject.SetActive(screen == Screen.Lobby);
            GameScreen.gameObject.SetActive(screen == Screen.Game);
            WinnerScreen.gameObject.SetActive(screen == Screen.Winner);
        }

        public IEnumerator RunLobby()
        {
            SetScreen(Screen.Lobby);

            yield return null;

            var endAt = Time.time + 10f;
            while (Time.time < endAt)
                yield return null;
        }

        public IEnumerator RunWinner()
        {
            SetScreen(Screen.Winner);

            yield return null;

            var endAt = Time.time + 10f;
            while (Time.time < endAt)
                yield return null;
        }
    }

    public enum Screen
    {
        Lobby,
        Game,
        Winner
    }
}
