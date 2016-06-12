using UnityEngine;
using DarkRift;
using Warlock.Common;
using CommunicationLayer;
using CommunicationLayer.CommunicationModels;
using CommunicationLayer.CommunicationModels.Responses;
using System.Collections;

namespace Warlock.Networking
{
    public class WarlockServerManager : MonoBehaviour
    {
        public static DarkRiftConnection Connection
        {
            get
            {
                return m_Connection;
            }
        }
        public static Configuration Configuration { get; private set; }

        private static DarkRiftConnection m_Connection;
        
        [SerializeField]
        private Configuration m_Configuration;

        // Use this for initialization
        void Start()
        {
            Configuration = m_Configuration;
            m_Connection = new DarkRiftConnection();
            m_Connection.workInBackground = false;
            m_Connection.Connect(m_Configuration.ServerIP, m_Configuration.ServerPort);
            Debug.Log("Connected to Server!");

            m_Connection.onData += OnDataReceived;

            //test login
            StartCoroutine(Wait());
        }

        IEnumerator Wait()
        {
            if (m_Connection.isConnected)
            {
                var loginPayload = new GenericPayload<TryLoginRequestModel>
                {
                    Value = new TryLoginRequestModel
                    {
                        Username = "DrNerf",
                        Password = "qwerty12"
                    }
                };
                m_Connection.SendMessageToServer((int)UsersPluginRequestTags.TryLoginRequest
                                        , (int)UsersPluginRequestTags.TryLoginRequest, loginPayload);
                Debug.Log("Request send!");
            }
            else
            {
                yield return new WaitForSeconds(1);
            }
        }

        private void OnDataReceived(byte tag, ushort subject, object data)
        {
            if(tag == (int)UsersPluginResponseTags.TryLoginResponse)
            {
                var payload = data as GenericPayload<TryLoginResponseModel>;
                Debug.Log(payload.Value.IsSuccess + " " + payload.Value.Username);
            }
        }

        void OnApplicationQuit()
        {
            m_Connection.Disconnect();
        }
    }

}