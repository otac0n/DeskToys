using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace DreamCheeky.MissileLauncher
{
    public static class Launchers
    {
        public static ILauncher Create()
        {
            return Create<ILauncher>();
        }

        public static T Create<T>() where T : ILauncher
        {
            return FindServices()
                .Where(s => typeof(T).IsAssignableFrom(s.Type))
                .Select(s => (T)s.Get())
                .FirstOrDefault();
        }

        private static IEnumerable<Service> FindServices()
        {
            var implementations = from t in Assembly.GetExecutingAssembly().GetTypes()
                                  where typeof(ILauncher).IsAssignableFrom(t)
                                  where t.IsClass
                                  where !t.IsAbstract
                                  select t;

            var services = from i in implementations
                           let enumerate = i.GetMethod("Enumerate", BindingFlags.Static)
                           from s in (IEnumerable<Service>)enumerate.Invoke(null, new object[0])
                           select s;

            return services;
        }
    }
}
