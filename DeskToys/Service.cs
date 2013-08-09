using System;

namespace DeskToys
{
    internal abstract class Service
    {
        public abstract Type Type { get; }

        public abstract object Get();
    }

    internal class Service<T> : Service
    {
        private readonly Func<T> factory;

        public Service(Func<T> factory)
        {
            this.factory = factory;
        }

        public override Type Type
        {
            get { return typeof(T); }
        }

        public override object Get()
        {
            return this.factory();
        }
    }
}