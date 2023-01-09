//----------------------------------------------------------------------------------
// <copyright file="DiContainer.cs" company="Siemens Healthcare GmbH">
// Copyright (C) Siemens Healthcare GmbH, 2017-2022. All Rights Reserved. Confidential.
// Author: Steffen Hanke
// </copyright>
//----------------------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Linq;

using Solid.Infrastructure.Diagnostics;

namespace Solid.Infrastructure.DiContainer.Impl
{
    /// <summary>
    /// TypeNotRegisteredException
    /// </summary>
    public class TypeNotRegisteredException : Exception
    {
        public TypeNotRegisteredException(string message)
            : base(message)
        {
        }
    }

    /// <summary>
    /// CircularDependencyException
    /// </summary>
    public class CircularDependencyException : Exception
    {
        public CircularDependencyException(string message)
            : base(message)
        {
        }
    }

    internal interface IRegisteredObjectInternal: IDisposable
    {
        Type TypeToResolve { get; }
        Type ConcreteType { get; }
        //object Instance { get; }
        object GetInstance(IResolverInternal resolver);
    }

    internal interface IResolverInternal : IDiResolve
    {
        object ResolveObject(Type typeToResolve);
        object TryResolveObject(Type typeToResolve);
        IList<IRegisteredObjectInternal> AncestorObjects { get; }
    }

    /// <summary>
    /// DiContainer
    /// </summary>
    public class DiContainer : IDiContainer
    {
        private readonly IList<IRegisteredObjectInternal> _registeredObjects = new List<IRegisteredObjectInternal>();

        public DiContainer()
        {
            RegisterInstance<IDiContainer>(this);
            RegisterInstance<IDiResolve>(this);
        }

        public void Dispose()
        {
            foreach (var registeredObj in _registeredObjects)
            {
                registeredObj?.Dispose();
            }
            _registeredObjects.Clear();
        }

        public void Register(IDiRegistrar registrar)
        {
            ConsistencyCheck.EnsureArgument(registrar).IsNotNull();
            registrar.Register(this);
        }

        public bool IsRegistered<TTypeToResolve>()
        {
            var type = typeof(TTypeToResolve);
            return _registeredObjects.Any(o => o.TypeToResolve == type);
        }

        public bool IsRegisteredAnyImplementing<TTypeToResolve>()
        {
            var type = typeof(TTypeToResolve);
            return _registeredObjects.Any(o =>
            {
                // 1) proof if registered type is of requested type
                if (o.TypeToResolve == type)
                {
                    return true;
                }
                // 2) proof if registered concrete type implements requested interface type
                var intf = o.ConcreteType.GetInterface(type.Name);
                if (intf != null && intf.FullName == type.FullName)
                {
                    return true;
                }
                // 3) proof if registered concrete type implements requested type (is assignable to type)
                return o.ConcreteType.IsAssignableTo(type);
            });
        }


        private void DisposeExistingRegistration(Type typeToResolve)
        {
            ConsistencyCheck.EnsureArgument(typeToResolve).IsNotNull();
            var registration = _registeredObjects.FirstOrDefault(o => o.TypeToResolve == typeToResolve);
            if (registration == null)
            {
                return;
            }
            _registeredObjects.Remove(registration);
            registration.Dispose();
        }

        public void RegisterInstance<TTypeToResolve>(object instance)
        {
            DisposeExistingRegistration(typeof(TTypeToResolve));
            _registeredObjects.Add(new RegisteredObject(typeof(TTypeToResolve), instance));
        }

        public void RegisterType<TTypeToResolve, TConcrete>()
        {
            DisposeExistingRegistration(typeof(TTypeToResolve));
            _registeredObjects.Add(new RegisteredObject(typeof(TTypeToResolve), typeof(TConcrete), LifeCycle.Singleton));
        }

        public void RegisterTypeAsTransient<TTypeToResolve, TConcrete>()
        {
            DisposeExistingRegistration(typeof(TTypeToResolve));
            _registeredObjects.Add(new RegisteredObject(typeof(TTypeToResolve), typeof(TConcrete), LifeCycle.Transient));
        }

        public void RegisterCreator<TTypeToResolve>(Func<IDiResolve, object> creator)
        {
            DisposeExistingRegistration(typeof(TTypeToResolve));
            _registeredObjects.Add(new RegisteredObject(typeof(TTypeToResolve), (resolver, parentType) => creator(resolver), LifeCycle.Singleton));
        }
        
        public void RegisterCreatorAsTransient<TTypeToResolve>(Func<IDiResolve, object> creator)
        {
            DisposeExistingRegistration(typeof(TTypeToResolve));
            _registeredObjects.Add(new RegisteredObject(typeof(TTypeToResolve), (resolver, parentType) => creator(resolver), LifeCycle.Transient));
        }

        public void RegisterCreator<TTypeToResolve>(Func<IDiResolve, Type, object> creator)
        {
            DisposeExistingRegistration(typeof(TTypeToResolve));
            _registeredObjects.Add(new RegisteredObject(typeof(TTypeToResolve), creator, LifeCycle.Singleton));
        }

        public void RegisterCreatorAsTransient<TTypeToResolve>(Func<IDiResolve, Type, object> creator)
        {
            DisposeExistingRegistration(typeof(TTypeToResolve));
            _registeredObjects.Add(new RegisteredObject(typeof(TTypeToResolve), creator, LifeCycle.Transient));
        }


        public TTypeToResolve Resolve<TTypeToResolve>()
        {
            return (new ResolveContext(_registeredObjects).Resolve<TTypeToResolve>());
        }

        public TTypeToResolve TryResolve<TTypeToResolve>()
        {
            return (new ResolveContext(_registeredObjects).TryResolve<TTypeToResolve>());
        }

        public IEnumerable<TTypeToResolve> ResolveAllImplementing<TTypeToResolve>()
        {
            return (new ResolveContext(_registeredObjects).ResolveAllImplementing<TTypeToResolve>());
        }

        internal class ResolveContext : IDiResolve, IResolverInternal
        {
            private readonly IList<IRegisteredObjectInternal> _registrations;
            private readonly IList<IRegisteredObjectInternal> _ancestors;

            internal ResolveContext(IEnumerable<IRegisteredObjectInternal> registrations)
            {
                _registrations = registrations?.ToList() ?? throw new ArgumentNullException(nameof(registrations));
                _ancestors = new List<IRegisteredObjectInternal>();
            }

            IList<IRegisteredObjectInternal> IResolverInternal.AncestorObjects => _ancestors;

            public TTypeToResolve Resolve<TTypeToResolve>()
            {
                return (TTypeToResolve)(this as IResolverInternal).ResolveObject(typeof(TTypeToResolve));
            }

            public TTypeToResolve TryResolve<TTypeToResolve>()
            {
                return (TTypeToResolve)(this as IResolverInternal).TryResolveObject(typeof(TTypeToResolve));
            }

            public IEnumerable<TTypeToResolve> ResolveAllImplementing<TTypeToResolve>()
            {
                Type typeToResolve = typeof(TTypeToResolve);

                // resolve all where registered interface implements requestes type
                //var matchingRegistrations = _registrations.Where(o => typeToResolve.IsAssignableFrom(o.TypeToResolve));

                // resolve all where registered concrete type implements requestes type
                var matchingRegistrations = _registrations.Where(o =>
                {
                    // 1) proof if registered type is of requested type
                    if (o.TypeToResolve == typeToResolve)
                    {
                        return true;
                    }

                    // 2) proof if registered concrete type implements requested interface type
                    var intf = o.ConcreteType.GetInterface(typeToResolve.Name);
                    return intf != null && intf.FullName == typeToResolve.FullName;
                });

                return matchingRegistrations
                    .Select(o => (TTypeToResolve)o.GetInstance(this))
                    .ToList(); //important for immediate LINQ execution
            }

            object IResolverInternal.ResolveObject(Type typeToResolve)
            {
                var resolvedObject = (this as IResolverInternal).TryResolveObject(typeToResolve);
                if (resolvedObject == null)
                {
                    throw new TypeNotRegisteredException($"The type {typeToResolve.Name} has not been registered");
                }
                return resolvedObject;
            }

            object IResolverInternal.TryResolveObject(Type typeToResolve)
            {
                // all resolutions paths end up here (also the recursions due resolving ctor paramaters)
                var registeredObject = _registrations.FirstOrDefault(o => o.TypeToResolve == typeToResolve);
                if (registeredObject == null)
                {
                    return null;
                }

                // we maintain here the list of ancestors for detecting circular dependecies
                // -> add current registration object to head of ancestors
                (this as IResolverInternal).AncestorObjects.Insert(0, registeredObject);

                // we pass the ancestor history information to the regististration object (via this implementing IResolverInternal)
                var instance = registeredObject.GetInstance(this);

                // we cleanup afterwards
                // -> remove resolved registration object from head of ancestors
                (this as IResolverInternal).AncestorObjects.RemoveAt(0);

                return instance;
            }
        }

        internal enum LifeCycle
        {
            ExternInstance,
            Singleton,
            Transient
        }

        private class RegisteredObject : IRegisteredObjectInternal
        {
            private readonly Type _typeToResolve;
            private Type _concreteType;
            private object _instance;
            private readonly LifeCycle _lifeCycle;
            private readonly Func<IDiResolve, Type, object> _creator;
            private readonly IList<object> _instanceList = new List<object>();

            internal RegisteredObject(Type typeToResolve, Type concreteType, LifeCycle lifeCycle)
            {
                ConsistencyCheck.EnsureArgument(typeToResolve).IsNotNull();
                ConsistencyCheck.EnsureArgument(concreteType).IsNotNull();
                _typeToResolve = typeToResolve;
                _concreteType = concreteType;
                _lifeCycle = lifeCycle;
            }

            internal RegisteredObject(Type typeToResolve, object instance)
            {
                ConsistencyCheck.EnsureArgument(typeToResolve).IsNotNull();
                ConsistencyCheck.EnsureArgument(instance).IsNotNull();
                _typeToResolve = typeToResolve;
                _instance = instance;
                _lifeCycle = LifeCycle.ExternInstance;
                // update _conreteType from given instance (for ancestor info requests)
                _concreteType = instance.GetType();
            }

            internal RegisteredObject(Type typeToResolve, Func<IDiResolve, Type, object> creator, LifeCycle lifeCycle)
            {
                ConsistencyCheck.EnsureArgument(typeToResolve).IsNotNull();
                ConsistencyCheck.EnsureArgument(creator).IsNotNull();
                _typeToResolve = typeToResolve;
                _creator = creator;
                _lifeCycle = lifeCycle;
            }

            // expose attributes of registeration
            Type IRegisteredObjectInternal.TypeToResolve => _typeToResolve;
            Type IRegisteredObjectInternal.ConcreteType => _concreteType;
            //object IRegisteredObjectInternal.Instance => _instance;
            //LifeCycle IRegisteredObjectInternal.LifeCycle => _lifeCycle;
            //Func<IDiResolve, Type, object> IRegisteredObjectInternal.Creator => _creator;

            public void Dispose()
            {
                if (this._lifeCycle == LifeCycle.ExternInstance)
                {
                    // we must not dispose external created instances
                    // this also applies to the automatically added registrations of the DiContainer itslef (as IDiResolve and IDicontainer)
                    return;
                }

                if (this._lifeCycle == LifeCycle.Singleton)
                {
                    (_instance as IDisposable)?.Dispose();
                }

                if (this._lifeCycle == LifeCycle.Transient)
                {
                    // in case of 'Transient' registrations we have to dispose all created instances
                    foreach (var instance in _instanceList)
                    {
                        (instance as IDisposable)?.Dispose();
                    }
                }
                _instanceList.Clear();
            }

            object IRegisteredObjectInternal.GetInstance(IResolverInternal resolver)
            {
                if (_instance == null || _lifeCycle == LifeCycle.Transient)
                {
                    _instance = CreateInstance(resolver);
                    _instanceList.Add(_instance);
                }
                return _instance;
            }

            private object CreateInstance(IResolverInternal resolver)
            {
                if (_creator != null)
                {
                    // the first in the AncestorObjects list is ourslef, we need the info of the second (our creator)
                    var ancestorType = resolver.AncestorObjects.Count > 1 ? resolver.AncestorObjects[1].ConcreteType : null;
                    var instance = _creator(resolver, ancestorType);
                    // update _conreteType from created instance (for ancestor info requests)
                    _concreteType = instance?.GetType();
                    return instance;
                }

                if (_concreteType != null)
                {
                    var parameters = ResolveConstructorParameters(resolver);

                    return Activator.CreateInstance(_concreteType, 
                        System.Reflection.BindingFlags.CreateInstance |
                        System.Reflection.BindingFlags.Public |
                        System.Reflection.BindingFlags.Instance |
                        System.Reflection.BindingFlags.OptionalParamBinding,
                        binder: null, 
                        args: parameters,
                        culture: System.Globalization.CultureInfo.CurrentCulture);
                }

                return null;
            }

            private object[] ResolveConstructorParameters(IResolverInternal resolver)
            {
                // prepare list of ancestors for detecting circular dependecies (add self to head)
                var ancestors = resolver.AncestorObjects;
                // we try to find a constructor where all parameters can be resolved
                // we start with the biggest constructor
                IEnumerable<string> missingParameters = Array.Empty<string>();
                var constructorInfos = _concreteType.GetConstructors().OrderByDescending(x => x.GetParameters().Count()).ToList();
                if (!constructorInfos.Any())
                {
                    throw new TypeNotRegisteredException($"The type '{_concreteType.Name}' cannot be instantiated since there is no public ctor available");
                }

                var circularDependecies = false;
                foreach (var cinfo in constructorInfos)
                {
                    // extract infos about parameters of current ctor
                    var ctorParameterInfos = cinfo.GetParameters().Where(p => p != null).ToList();
                    
                    var parameterInfos = ctorParameterInfos.Select(p => new
                    {
                        type = p.ParameterType,
                        name = p.Name,
                        optional = p.IsOptional, //p.GetCustomAttributes(typeof(System.Runtime.InteropServices.OptionalAttribute), inherit: false).Any(),
                        defaultValue = p.RawDefaultValue ?? Type.Missing,
                        circularDependency = ancestors.Any(a => a.TypeToResolve == p.ParameterType || a.ConcreteType == p.ParameterType)
                    }).ToList();

                    // try to resolve all parameters
                    var parameters = parameterInfos.Select(p => new 
                    {
                        info = p,
                        instance = p.circularDependency 
                            ? (p.optional ? p.defaultValue : null)
                            : (resolver.TryResolveObject(p.type) ?? (p.optional ? p.defaultValue : null))
                    }).ToList();

                    // return resolved parameters for creation of asked type
                    if (parameters.All(p => p.instance != null))
                    {
                        return parameters.Select(p => p.instance).ToArray();
                    }

                    // update missingParameters as argument for TypeNotRegisteredException
                    var missingCtorParams = parameters.Where(p => p.instance == null)
                        .Select(p => p.info.optional 
                        ? $"'{p.info.type.Name} {p.info.name} = {p.info.defaultValue}'"
                        : $"'{p.info.type.Name} {p.info.name}'"
                        ).Distinct();
                    
                    missingParameters = missingParameters.Union(missingCtorParams).Distinct();

                    circularDependecies |= parameters.Any(p => p.instance == null && p.info.circularDependency);
                }
                if (circularDependecies) 
                {
                    throw new CircularDependencyException($"The type '{_concreteType.Name}' cannot be instantiated since there are only ctors that have a circular dependency to itself");
                }
                throw new TypeNotRegisteredException($"The type '{_concreteType.Name}' cannot be instantiated since all ctors have unresolvable parameters. \nmissing parameters are: {string.Join(", ", missingParameters)}");
            }
        }
    }
}