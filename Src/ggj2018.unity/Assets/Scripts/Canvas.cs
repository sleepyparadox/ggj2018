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

        public UnityEngine.UI.Text LobbyText;

        public UnityEngine.UI.Text CarsScoreText;
        public UnityEngine.UI.Text ConductorScoreText;
        public UnityEngine.UI.Text TimerText;

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

            CarsScoreText.text = "";
            ConductorScoreText.text = "";
        }


        public IEnumerator RunLobby()
        {
            SetScreen(Screen.Lobby);

            while(true)
            {
                if(HasEnoughPlayers() == false)
                {
                    LobbyText.text = ReadyPlayers() + " cars";
                    yield return null;
                }

                var endAt = Time.time + 10f;
                while (HasEnoughPlayers() && Time.time < endAt)
                {
                    var remaining = endAt - Time.time;
                    LobbyText.text = "" + remaining.ToString("00.0");

                    yield return null;
                }

                if (HasEnoughPlayers() == false)
                {
                    // try again
                    continue;
                }

                // Done!
                break;
            }
        }

        bool HasEnoughPlayers()
        {
            return ReadyPlayers() >= 2;
        }

        int ReadyPlayers()
        {
            return MainApp.S.Devices.Values.Count(d => d.Role != DeviceRole.Wait && d.Connected);
        }

        public void UpdateScores(int carScore, int conductorScore, float timerRemaining)
        {
            CarsScoreText.text = carScore.ToString();
            ConductorScoreText.text = conductorScore.ToString();
            TimerText.text = string.Format("time remaining {0}", timerRemaining.ToString("00.00"));
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
