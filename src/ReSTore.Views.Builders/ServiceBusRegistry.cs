using System;
using System.Configuration;
using EasyNetQ;
using StructureMap;
using StructureMap.Configuration.DSL;

namespace ReSTore.Views.Builders
{
    public class ServiceBusRegistry : Registry
    {
        public ServiceBusRegistry()
        {
            For<IBus>().Use(CreateMessageBus);
        }

        public static IBus CreateMessageBus(IContext context)
        {
            var connectionString = ConfigurationManager.ConnectionStrings["EasyNetQ"];
            if (connectionString == null || connectionString.ConnectionString == string.Empty)
            {
                throw new Exception("EasyNetQ connection string is missing or empty");
            }

            var bus = RabbitHutch.CreateBus(connectionString.ConnectionString);

            return bus;
        }
    }
}