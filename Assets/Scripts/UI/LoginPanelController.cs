using UnityEngine;
using System.Collections;
using DarkRift;
using Warlock.Networking;
using CommunicationLayer;
using CommunicationLayer.CommunicationModels;
using System;
using UnityEngine.SceneManagement;
using UnityEngine.Events;

namespace Warlock.UI
{
    public class LoginPanelController : MonoBehaviour
    {
        public string ServerStatus = "Connecting...";
        [HideInInspector]
        public string Username;
        [HideInInspector]
        public string Password;
        [HideInInspector]
        public bool IsLoginBtnInteractable = true;
        public UnityEvent OnLogin;

        private DarkRiftConnection m_Connection;

        void Start()
        {
            m_Connection = WarlockServerManager.Connection;
            m_Connection.onData += OnDataReceived;
        }

        private void OnDataReceived(byte tag, ushort subject, object data)
        {
            if(tag == (int)UsersPluginResponseTags.TryLoginResponse)
            {
                var payload = data as GenericPayload<TryLoginResponseModel>;
                if(data != null)
                {
                    Debug.Log(payload.Value.IsSuccess);
                    if (payload.Value.IsSuccess)
                    {
                        //SceneManager.LoadSceneAsync("DevArena");
                        OnLogin.Invoke();
                    }
                }
                IsLoginBtnInteractable = true;
            }
        }

        void Update()
        {
            if (m_Connection.isConnected && ServerStatus != "Connected!")
                ServerStatus = "Connected!";

            if (Input.GetKeyDown(KeyCode.KeypadEnter) || Input.GetKeyDown(KeyCode.Return))
            {
                TryLogin();
            }
        }

        public void TryLogin()
        {
            if (m_Connection.isConnected && IsLoginBtnInteractable)
            {
                IsLoginBtnInteractable = false;
                var payload = new GenericPayload<TryLoginRequestModel>
                {
                    Value = new TryLoginRequestModel
                    {
                        Username = Username,
                        Password = Password
                    }
                };

                m_Connection.SendMessageToServer((int)UsersPluginRequestTags.TryLoginRequest
                                , 0
                                , payload);
            }
        }
    } 
}
