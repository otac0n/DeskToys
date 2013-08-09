using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace DeskToys
{
    public static class Toys
    {
        public static IButton CreateButton()
        {
            return Create<IButton>();
        }

        public static ILedNotifier CreateLedNotifier()
        {
            return Create<ILedNotifier>();
        }

        public static IResettableLauncher CreateLauncher()
        {
            return Create<IResettableLauncher>();
        }

        public static T Create<T>()
        {
            return FindAll<T>()
                .Select(s => (T)s.Get())
                .FirstOrDefault();
        }

        public static IEnumerable<Service> FindAll<T>()
        {
            var implementations = from t in Assembly.GetExecutingAssembly().GetTypes()
                                  where typeof(T).IsAssignableFrom(t)
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
