﻿using NetWorkCore.IpcObjects;

namespace NetWorkCore
{
    public class MachineStutas
    {
        public string ClientCode { get; set; }

        public bool IsCoinReady { get; private set; }

        public bool IsCatchSuccess { get; private set; }

        public bool IsGameOver { get; private set; } = true;

        public void CoinReady()
        {
            IsCoinReady = true;
            IsGameOver = false;
        }

        public void GameStart()
        {
            IsCoinReady  = IsCatchSuccess = false;
        }

        public void GameOver()
        {
            IsGameOver = true;
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
                case MachineOperate.GetCatchCount:
                    result.IsOperateResultOk = IsCatchSuccess;
                    break;
            }

            result.IsOperateSuccess = true;
            return result;
        }

        public void CatchSuccessed()
        {
            IsCatchSuccess = true;
        }
    }
}
