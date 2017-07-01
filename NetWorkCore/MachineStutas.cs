using NetWorkCore.IpcObjects;

namespace NetWorkCore
{
    public class MachineStutas
    {
        public string ClientCode { get; set; }

        public bool IsCoinReady { get; private set; }

        public void CoinReady()
        {
            IsCoinReady = true;
        }

        public void GameStart()
        {
            IsCoinReady = false;
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

            result.IsOperateResultOk = true;
            return result;
        }
    }
}
