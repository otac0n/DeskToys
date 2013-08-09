using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace DeskToys
{
    public static class LedNotifiers
    {
        public static ILedNotifier Create()
        {
            return Create<ILedNotifier>();
        }

        public static T Create<T>() where T : ILedNotifier
        {
            return FindServices()
                .Where(s => typeof(T).IsAssignableFrom(s.Type))
                .Select(s => (T)s.Get())
                .FirstOrDefault();
        }

        private static IEnumerable<Service> FindServices()
        {
            var implementations = from t in Assembly.GetExecutingAssembly().GetTypes()
                                  where typeof(ILedNotifier).IsAssignableFrom(t)
                                  where t.IsClass
                                  where !t.IsAbstract
                                  select t;

            var services = from i in implementations
                           let enumerate = i.GetMethod("Enumerate", BindingFlags.Static | BindingFlags.Public)
                           from s in (IEnumerable<Service>)enumerate.Invoke(null, new object[0])
                           select s;

            return services;
        }
    }
}
