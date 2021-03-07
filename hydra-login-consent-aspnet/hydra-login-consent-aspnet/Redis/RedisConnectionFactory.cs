using Microsoft.Extensions.Logging;
using StackExchange.Redis;
using System;

namespace hydra_login_consent_aspnet.Redis
{
    public class RedisConnectionFactory : IRedisConnectionFactory
    {
        private readonly ConnectionMultiplexer connection;

        private RedisConnectionFactory(
            ConnectionMultiplexer connection)
        {
            this.connection = connection;
        }

        public static RedisConnectionFactory Create(ILogger<RedisConnectionFactory> logger, string connectionString)
        {
            try
            {
                var connection = ConnectionMultiplexer.Connect(connectionString);

                connection.ConnectionFailed += (sender, args) => logger.LogError("Database is offline.");

                connection.ConnectionRestored += (sender, args) => logger.LogError("Database is back online");

                return new RedisConnectionFactory(connection);
            }
            catch (Exception e)
            {
                logger.LogError($"Database initialization failed: {e}");
            }

            return new RedisConnectionFactory(null);
        }


        public IDatabase GetDatabase()
        {
            if (this.connection == null)
            {
                return null;
            }

            return this.connection.GetDatabase();
        }
    }

    public interface IRedisConnectionFactory
    {
        IDatabase GetDatabase();
    }
}
