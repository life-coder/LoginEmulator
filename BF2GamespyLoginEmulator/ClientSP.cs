﻿using System;
using System.Threading;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net;
using System.Net.Sockets;

namespace Gamespy
{
    class ClientSP : IDisposable
    {
        public bool Disposed { get; protected set; }
        private ClientStream Stream;
        private TcpClient Client;
        private Thread iThread;
        private bool BF_15 = false;
        private string clientNick;
        private string clientPID;

        public ClientSP(TcpClient client)
        {
            // Set disposed to false!
            this.Disposed = false;

            // Set the client variable
            this.Client = client;

            // Init a new client stream class
            Stream = new ClientStream(client);

            iThread = new Thread(new ThreadStart(Start));
            iThread.IsBackground = true;
            iThread.Start();
        }

        /// <summary>
        /// Destructor
        /// </summary>
        ~ClientSP()
        {
            this.Dispose(true);
        }

        //This becomes private void Dispose( bool Disposing ) on sealed classes.
        protected virtual void Dispose(bool Disposing)
        {
            //If we've already been disposed, don't call again.
            if (this.Disposed)
                return;

            if (Disposing)
            {
                //Dispose of all managed resources here, for example a Windows.Forms.Control, Component or other object on the Framework.
            }

            //Done.
            this.Disposed = true;
        }

        public void Dispose()
        {
            //Clean up everything.
            this.Dispose(true);
        }

        public void Start()
        {
            Console.WriteLine(" - <GPSP> Client Connected: {0}", Client.Client.RemoteEndPoint);

            while (Client.Client.IsConnected())
            {
                Update();
            }

            Console.WriteLine(" - <GPSP> Client Disconnected: {0}", Client.Client.RemoteEndPoint);
            Dispose();
        }

        public void Update()
        {
            if (Stream.HasData())
            {
                // TODO: process the 'getprofile' (returned at this point) data
                string message = Stream.Read();
                Console.WriteLine(message);
                string[] recv = message.Split('\\');

                switch (recv[1])
                {
                    case "nicks":
                        int L = recv.Length;
                        if (L == 15)
                            BF_15 = true;

                        // TODO Validate that the account exists!
                        // GetUser(GetParamenterValue(recv, "email"), GetParamenterValue(recv, "pass"));
                        clientNick = "Wilson212";
                        clientPID = "101249154";
                        SendGPSP();
                        break;
                    case "check":
                        SendCheck();
                        break;
                }
            }
        }

        private void SendGPSP()
        {
            string message = String.Format("\\nr\\1\\nick\\{0}\\uniquenick\\{1}\\ndone\\\\final\\", clientNick, clientNick);
            Stream.Write(message);
        }

        private void SendCheck()
        {
            string message = String.Format("\\cur\\0\\pid\\{0}\\final\\", clientPID);
            Stream.Write(message);
        }

        /// <summary>
        /// A simple method of getting the value of the passed parameter key,
        /// from the returned array of data from the client
        /// </summary>
        /// <param name="parts">The array of data from the client</param>
        /// <param name="parameter">The parameter</param>
        /// <returns>The value of the paramenter key</returns>
        private string GetParameterValue(string[] parts, string parameter)
        {
            bool next = false;
            foreach (string part in parts)
            {
                if (next)
                    return part;
                else if (part == parameter)
                    next = true;
            }
            return "";
        }
    }
}
