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
        public RectTransform LobbyIconSpace;


        public UnityEngine.UI.Text CarsScoreText;
        public UnityEngine.UI.Text ConductorScoreText;
        public UnityEngine.UI.Text TimerText;

        const int MinPlayersNeed = 2;

        bool _firstTimeLobby;
        Dictionary<Device, PlayerIcon> _playerIcons = new Dictionary<Device, PlayerIcon>();

        const float FirstLobbyDuration = 60f;
        const float QuickLobbyDuration = 30f;
        //const float FirstLobbyDuration = 6f;
        //const float QuickLobbyDuration = 3f;

        void Awake()
        {
            S = this;
            _firstTimeLobby = true;
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
                    var needed = MinPlayersNeed - ReadyPlayers();
                    LobbyText.text = string.Format("{0} MORE PLAYERS NEEDED", needed);

                    UpdatePlayerIcons();
                    yield return null;
                }

                var endAt = Time.time + (_firstTimeLobby ? FirstLobbyDuration : QuickLobbyDuration);
                _firstTimeLobby = false;

                while (HasEnoughPlayers() && Time.time < endAt)
                {
                    var remaining = endAt - Time.time;
                    LobbyText.text = string.Format("STARTING {0}", remaining.ToString("00.0"));

                    UpdatePlayerIcons();
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

            foreach (var icon in _playerIcons.Values)
                PlayerIcon.Destroy(icon.gameObject);
            _playerIcons.Clear();
        }

        void UpdatePlayerIcons()
        {
            foreach (var device in MainApp.S.Devices.Values)
            {
                if (device.Connected == false
                    || device.Role != DeviceRole.Ready
                    || _playerIcons.ContainsKey(device))
                    continue;

                var icon = PlayerIcon.Instantiate<PlayerIcon>(Prefabs.S.PlayerIcon);
                icon.transform.SetParent(LobbyIconSpace, false);
                icon.transform.localPosition = new Vector3(UnityEngine.Random.Range(-100f, 100f), UnityEngine.Random.Range(-100f, 100f), 0);

                icon.NameText.text = device.Name;
                icon.ScoreText.text = device.Score.ToString();

                icon.Space = LobbyIconSpace;
                icon.IconsCollection = _playerIcons;

                _playerIcons.Add(device, icon);
            }

            foreach (var icon in _playerIcons.Values)
            {
                icon.Update();
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
