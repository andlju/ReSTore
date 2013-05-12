using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using StructureMap;
using Topshelf;
using Topshelf.Runtime;

namespace ReSTore.Views.Builders
{
    public interface IService
    {
        void Start();
        void Stop();
    }

    public class ViewBuilderData
    {
        public long CommitPosition { get; set; }
        public long PreparePosition { get; set; }
    }

    class Program
    {
        static void Main(string[] args)
        {
            var container = new Container(x =>
                                              {
                                                  x.AddRegistry<RavenRegistry>();
                                                  x.AddRegistry<ServiceBusRegistry>();
                                                  x.AddRegistry<EventStoreRegistry>();
                                                  
                                                  x.For<IModelUpdateNotifier>().Use<ServiceBusUpdateNotifier>();
                                                  
                                                  x.For<IService>().Use<ViewBuilderService>();
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
                x.SetServiceName("ReSTore.ViewBuilders");
                x.SetDisplayName("ReSTore View Builders service");
                x.SetDescription("Service for building the views for ReSTore");
            });
        }
    }
}
