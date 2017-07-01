namespace NetWorkCore.IpcObjects
{
    public enum ControlCommand : ushort
    {
        /// <summary>
        /// 机械臂前进
        /// </summary>
        Forward = 0x00,

        /// <summary>
        /// 机械臂后退
        /// </summary>
        Backward = 0x01,

        /// <summary>
        /// 机械臂左移
        /// </summary>
        Left = 0x02,

        /// <summary>
        /// 机械臂右移
        /// </summary>
        Right = 0x03,

        /// <summary>
        /// 机械臂抓取
        /// </summary>
        Catch = 0x04,

        /// <summary>
        /// 机械臂停止
        /// </summary>
        Stop = 0x05,

        /// <summary>
        /// 机器投币
        /// </summary>
        CoinIn = 0x06,

        /// <summary>
        /// 投币完成
        /// </summary>
        CoinGet = 0x07
    }
}
