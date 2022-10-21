using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.UI;

namespace Platformer.UI
{
    /// <summary>
    /// A simple controller for switching between UI panels.
    /// </summary>
    public class MainUIController : MonoBehaviour
    {
        public GameObject[] panels;
        [Header("Buttons")]
        [SerializeField] private Button serverBtn;
        [SerializeField] private Button hostBtn;
        [SerializeField] private Button clientBtn;

        private void Awake()
        {
            serverBtn.onClick.AddListener(() =>
            {
                NetworkManager.Singleton.StartServer();
            });
            hostBtn.onClick.AddListener(() =>
            {
                NetworkManager.Singleton.StartHost();
            });
            clientBtn.onClick.AddListener(() =>
            {
                NetworkManager.Singleton.StartClient();
            });
        }

        public void SetActivePanel(int index)
        {
            for (var i = 0; i < panels.Length; i++)
            {
                var active = i == index;
                var g = panels[i];
                if (g.activeSelf != active) g.SetActive(active);
            }
        }

        void OnEnable()
        {
            SetActivePanel(0);
        }
    }
}