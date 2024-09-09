namespace Session
{
    public partial class ErrorCode
    {
        public const int ERR_Success = 0;

        public const int ERR_HttpSuccess = 200;

        public const int ERR_NOTINWHITELIST = 403;//���ڰ�����

        public const int ERR_ClientSelfClose = 1000;//�ͻ��������ر�
        public const int ERR_SocketError = 1001;//Socket����
        public const int ERR_SocketConnectError = 1002;//Socket����
        public const int ERR_ClientExceptionClose = 1100;//�ͻ��������ر�


        public const int ERR_FatalError = 101120;//��������

        public const int ERR_NetWorkError = 200002;//�������

    }
}
