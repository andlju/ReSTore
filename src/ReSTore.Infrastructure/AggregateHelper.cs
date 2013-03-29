using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace ReSTore.Infrastructure
{
    public abstract class Aggregate
    {
        private IList<object> _uncommittedEvents = new List<object>();

        public void Publish(object evt)
        {
            this.Apply(evt);
            _uncommittedEvents.Add(evt);
        }

        public IEnumerable<object> GetUncommittedEvents()
        {
            var events = _uncommittedEvents.ToArray();
            _uncommittedEvents.Clear();
            return events;
        }
    }

    public static class AggregateHelper
    {
        private static readonly Dictionary<Type, Func<IEnumerable<object>, object>> Builders = new Dictionary<Type, Func<IEnumerable<object>, object>>();
        private static readonly Dictionary<Type, Dictionary<Type, Action<object, object>>> Handlers = new Dictionary<Type, Dictionary<Type, Action<object, object>>>();

        private static void Register(Type aggregateType)
        {
            RegisterHandlers(aggregateType);
            RegisterBuilder(aggregateType);
        }

        private static void RegisterBuilder(Type t)
        {
            var ctor = t.GetConstructor(Type.EmptyTypes);
            if (ctor == null)
            {
                throw new InvalidOperationException("Aggregate must have constructor with no parameters");
            }
            Builders[t] = (evts) =>
                                       {
                                           var agg = (Aggregate)ctor.Invoke(new object[0]);
                                           foreach (var evt in evts)
                                           {
                                               agg.Apply(evt);
                                           }
                                           return agg;
                                       };
        }

        private static void RegisterHandlers(Type type)
        {
            var applyMethods = type.GetMethods(BindingFlags.Instance | BindingFlags.NonPublic).Where(m => m.Name == "Apply");

            var tmp = new Dictionary<Type, Action<object, object>>();
            foreach (var am in applyMethods)
            {
                var applyMethod = am;
                tmp.Add(applyMethod.GetParameters().First().ParameterType, (agg, e) => applyMethod.Invoke(agg, new object[] { e }));
            }
            Handlers[type] = tmp;
        }

        public static void Apply(this Aggregate agg, object evt)
        {
            Dictionary<Type, Action<object, object>> handlers;
            if (!Handlers.TryGetValue(agg.GetType(), out handlers))
            {
                Register(agg.GetType());
                Apply(agg, evt);
                return;
            }

            Action<object, object> handler;
            if (!handlers.TryGetValue(evt.GetType(), out handler))
            {
                // Nothing to see, move along
                // throw new InvalidOperationException(string.Format("No Apply method defined for event of type {0} in aggregate of type {1}", evt.GetType(), agg.GetType()));
                return;
            }
            handler(agg, evt);
        }

        public static T Build<T>(IEnumerable<object> events) where T : Aggregate, new()
        {
            Func<IEnumerable<object>, object> builder;
            if (!Builders.TryGetValue(typeof (T), out builder))
            {
                Register(typeof(T));
                return Build<T>(events); // Try again!
            }

            return (T)builder(events);
        }
    }
}