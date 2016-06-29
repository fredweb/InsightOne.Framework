// insightOne.SB1.Framework.Framework
// Copyright (c) 2014
// Provider Consultoria
// Autor: Marvin Mendes

using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;

namespace insightOne.SB1.Framework.Exceptions
{
    /// <summary>
    /// Class minimalist container from STACKOVERFLOW
    /// See: http://stackoverflow.com/questions/2515124/whats-the-simplest-ioc-container-for-c/2598839#2598839
    /// </summary>
    class ServiceLocator
    {
        protected readonly Dictionary<string, Func<object>> _services = new Dictionary<string, Func<object>>();
        protected readonly Dictionary<Type, string> _serviceNames = new Dictionary<Type, string>();

        protected Dictionary<string, Func<object>> Services {
            get { return _services; }
        }

        protected Dictionary<Type, string> ServiceNames {
            get { return _serviceNames; }
        }

        public DependencyManager Register<S, C>() where C : S {
            return Register<S, C>(Guid.NewGuid().ToString());
        }

        public DependencyManager Register<S, C>(string name) where C : S {
            if (!_serviceNames.ContainsKey(typeof(S))) {
                _serviceNames[typeof(S)] = name;
            }
            return new DependencyManager(this, name, typeof(C));
        }

        public DependencyManager Register(Type cls, Type itf, string name) {
            if (!_serviceNames.ContainsKey(itf)) {
                _serviceNames[itf] = name;
            }
            return new DependencyManager(this, name, cls);
        }

        public T Resolve<T>(string name) where T : class {
            return (T)_services[name]();
        }

        public T TryResolve<T>(string name) where T : class {
            Func<object> outValue;
            if (_services.TryGetValue(name, out outValue))
                return (T)outValue();
            else
                return null;
        }

        public T Resolve<T>() where T : class {
            return Resolve<T>(_serviceNames[typeof(T)]);
        }

        internal class DependencyManager
        {
            private readonly ServiceLocator _container;
            private readonly Dictionary<string, Func<object>> args;
            private readonly string name;

            internal DependencyManager(ServiceLocator container, string name, Type type) {
                this._container = container;
                this.name = name;

                ConstructorInfo c = type.GetConstructors().First();
                args = c.GetParameters()
                    .ToDictionary<ParameterInfo, string, Func<object>>(
                    x => x.Name,
                    x => (() => _container.Services[container.ServiceNames[x.ParameterType]]())
                    );

                container.Services[name] = () => c.Invoke(args.Values.Select(x => x()).ToArray());
            }

            public DependencyManager AsSingleton() {
                object value = null;
                Func<object> service = _container.Services[name];
                _container.Services[name] = () => value ?? (value = service());
                return this;
            }

            public DependencyManager WithDependency(string parameter, string component) {
                args[parameter] = () => _container.Services[component]();
                return this;
            }

            public DependencyManager WithValue(string parameter, object value) {
                args[parameter] = () => value;
                return this;
            }
        }
    }
}
