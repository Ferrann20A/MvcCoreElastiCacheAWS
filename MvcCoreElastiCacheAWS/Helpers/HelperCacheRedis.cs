using StackExchange.Redis;

namespace MvcCoreElastiCacheAWS.Helpers
{
    public class HelperCacheRedis
    {
        private static Lazy<ConnectionMultiplexer> CreateConnection = new Lazy<ConnectionMultiplexer>(() =>
        {
            //AQUI IRA NUESTRA CADENA DE CONEXION
            string connectionString = "cache-coches.j3wol5.ng.0001.use1.cache.amazonaws.com:6379";
            return ConnectionMultiplexer.Connect(connectionString);
        });

        public static ConnectionMultiplexer Connection
        {
            get
            {
                return CreateConnection.Value;
            }
        }
    }
}
