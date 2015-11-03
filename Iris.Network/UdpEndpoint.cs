﻿using System;
using System.Linq;
using System.Net;
using System.Net.Sockets;

namespace Iris.Network
{

    public delegate void DataReceivedEventHandler(byte[] data);

    class UdpEndpoint
    {
        private readonly UdpClient _client = new UdpClient(9867);

        private IPEndPoint _broadcastEndPoint = new IPEndPoint(IPAddress.Broadcast, 9867);

        private IPEndPoint _anyEndPoint = new IPEndPoint(IPAddress.Any, 9867);

        public void Initialize()
        {
            _client.DontFragment = true;

            _client.EnableBroadcast = true;

            _client.BeginReceive(ReceiveAsync, null);
        }

        public void Send(byte[] data)
        {
            _client.SendAsync(data, data.Length, _broadcastEndPoint);
        }

        private void ReceiveAsync(IAsyncResult asyncResult)
        {
            byte[] data = _client.EndReceive(asyncResult, ref _anyEndPoint);

            ProcessMessage(data);

            _client.BeginReceive(ReceiveAsync, null);
        }

        private void ProcessMessage(byte[] data)
        {
            DataReceived?.BeginInvoke(data, EndDataReceived, null);
        }

        private void EndDataReceived(IAsyncResult result)
        {
            DataReceived?.EndInvoke(result);
        }

        public event DataReceivedEventHandler DataReceived;
    }
}
