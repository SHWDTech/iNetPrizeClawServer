﻿using System;
using System.Threading;
using System.Threading.Tasks;
using NetWorkCore.IpcObjects;

namespace NetWorkCore
{
    public class MachineStutas
    {
        public MachineStutas(TcpSocketClient client)
        {
            _tcpSocketClient = client;
        }

        public string ClientCode { get; set; }

        public bool IsCoinReady { get; private set; }

        private readonly TcpSocketClient _tcpSocketClient;

        public void CoinReady()
        {
            IsCoinReady = true;
        }

        public void GameStart()
        {
            IsCoinReady = false;
            Task.Factory.StartNew(() =>
            {
                Thread.Sleep(TimeSpan.FromSeconds(15));
                _tcpSocketClient.SendCommand(ControlCommand.Catch);
            });
        }

        public MachineOperateResult ExecuteOperate(MachineOperate operate)
        {
            var result = new MachineOperateResult
            {
                ClientCode = ClientCode
            };
            switch (operate)
            {
                case MachineOperate.GameStart:
                    GameStart();
                    result.IsOperateResultOk = true;
                    break;
                case MachineOperate.GetCoinStatus:
                    result.IsOperateResultOk = IsCoinReady;
                    break;
            }

            result.IsOperateSuccess = true;
            return result;
        }
    }
}