using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using ReSTore.Messages.Events;
using StructureMap;
using StructureMap.Pipeline;
using Topshelf;

namespace ReSTore
{
    public interface IService
    {
        void Start();
        void Stop();
    }

    class Program
    {
        static void Main(string[] args)
        {
            var container = new Container(x =>
                {
                    x.AddRegistry<DomainRegistry>();
                    x.AddRegistry<ServiceBusRegistry>();

                    x.For<IService>().Use<DomainService>();
                });

            HostFactory.Run(x =>
                {

                    x.Service<IService>(c =>
                        {
                            c.ConstructUsing(() => container.GetInstance<IService>());
                            c.WhenStarted(s => s.Start());
                            c.WhenStopped(s => s.Stop());
                        });

                    x.RunAsLocalSystem();
                    x.SetServiceName("ReSTore.Orders");
                    x.SetDisplayName("ReSTore Orders service");
                    x.SetDescription("Main service for the ReSTore order domain");
                });

        }
    }
}
