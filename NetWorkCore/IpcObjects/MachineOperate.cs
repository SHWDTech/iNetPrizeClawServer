namespace NetWorkCore.IpcObjects
{
    public enum MachineOperate : ushort
    {
        /// <summary>
        /// 获取投币状态
        /// </summary>
        GetCoinStatus = 0x00,

        /// <summary>
        /// 游戏开始
        /// </summary>
        GameStart = 0x01,

        /// <summary>
        /// 获取抓取成功总数
        /// </summary>
        GetCatchCount = 0x02
    }
}
