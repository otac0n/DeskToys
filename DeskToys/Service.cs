using System;

namespace DeskToys
{
    public abstract class Service
    {
        protected Service(string name, Type type)
        {
            this.Name = name;
            this.Type = type;
        }

        public string Name { get; private set; }

        public Type Type { get; private set; }

        public abstract object Get();
    }

    public class Service<T> : Service
    {
        private readonly Func<T> factory;

        public Service(string name, Func<T> factory)
            : base(name, typeof(T))
        {
            this.factory = factory;
        }

        public override object Get()
        {
            return this.factory();
        }
    }
}
