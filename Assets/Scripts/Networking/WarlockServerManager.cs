using UnityEngine;
using DarkRift;
using Warlock.Common;
using CommunicationLayer;
using CommunicationLayer.CommunicationModels;
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
        void Awake()
        {
            Configuration = m_Configuration;
            m_Connection = new DarkRiftConnection();
            m_Connection.workInBackground = m_Configuration.NetworkingAsync;
            m_Connection.Connect(m_Configuration.ServerIP, m_Configuration.ServerPort);
            Debug.Log("Connected to Server!");
        }

        void Update()
        {
            m_Connection.Receive();
        }

        void OnApplicationQuit()
        {
            m_Connection.Disconnect();
        }
    }

}