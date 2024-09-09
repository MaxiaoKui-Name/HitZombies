namespace Session
{
    public partial class ErrorCode
    {
        public const int ERR_Success = 0;

        public const int ERR_HttpSuccess = 200;

        public const int ERR_NOTINWHITELIST = 403;//不在白名单

        public const int ERR_ClientSelfClose = 1000;//客户端主动关闭
        public const int ERR_SocketError = 1001;//Socket错误
        public const int ERR_SocketConnectError = 1002;//Socket错误
        public const int ERR_ClientExceptionClose = 1100;//客户端主动关闭


        public const int ERR_FatalError = 101120;//致命错误

        public const int ERR_NetWorkError = 200002;//网络错误

    }
}
