namespace ThuocGiaThatAdmin.Server.Configurations
{
    public class RedisConfigurationOption
    {
        public int ConnectionTimeout { get; set; }
        public int SyncTImeout { get; set; }
        public int AsyncTImeout { get; set; }
        public bool AdminAllow { get; set; }
        public bool AbortOnConnectFail { get; set; }
        public int ConnectRetry { get; set; }
        public int ReconnectRetryPolicy { get; set; }
        public int KeepAlive { get; set; }
    }
}
