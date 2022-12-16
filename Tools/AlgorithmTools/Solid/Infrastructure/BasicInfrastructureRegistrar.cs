//----------------------------------------------------------------------------------
// <copyright file="BasicInfrastructureRegistrar.cs" company="Siemens Healthcare GmbH">
// Copyright (C) Siemens Healthcare GmbH, 2022. All Rights Reserved. Confidential.
// Author: Steffen Hanke
// </copyright>
//----------------------------------------------------------------------------------

using Solid.Infrastructure.Diagnostics;
using Solid.Infrastructure.DiContainer;
using Solid.Infrastructure.Environment;
using Solid.Infrastructure.EventAggregator;

namespace Solid.Infrastructure
{
    /// <summary>
    /// BasicInfrastructureRegistrar
    /// 
    /// registers basic infrastructure components like:
    /// - EventAggregator
    /// - FolderProvider
    /// - Tracing infrastucure (but initially does not trace without further configuration
    /// 
    /// remark:
    /// there happens no actual creation of any of the registerted components
    /// this is just the definition which type is to be instanciated or which creator func is to be executed when a specific interfaces is queried at the Dicontainer
    /// 
    /// example usage:
    /// // 1) create empty DI-Container
    /// using var diContainer = new DiContainer();
    /// // 2) register basic infrastucture components (e.g. basic tracing components, folder provider ...)
    /// diContainer.Register(new BasicInfrastructureRegistrar());
    ///
    /// </summary>
    public class BasicInfrastructureRegistrar : IRegistrar
    {
        public void Register(IDiContainer container)
        {
            ConsistencyCheck.EnsureArgument(container).IsNotNull();

            // a) register SOLID infrastructure components
            container.RegisterType<IEventAggregator, EventAggregator.Impl.EventAggregator>();

            
            // b) register basic infrastructure components
            container.RegisterTypeAsTransient<IFolderProvider, Solid.Infrastructure.Environment.Impl.FolderProvider>();

            
            // c) register basic trace environment
            container.RegisterCreatorAsTransient<ITracer>((resolver, creatingType) => resolver.Resolve<IMultiTracer>().CreateBaseDomainTracer(creatingType));

            container.RegisterType<IMultiTracer, Diagnostics.Impl.MultiTracer>();
            //container.RegisterCreator<IMultiTracer>(resolver =>
            //{
            //    var multiTracer = new Diagnostics.Impl.MultiTracer();
            //    // configure TraceTarget(s) and TraceLevel(s) as desired (e.g. contolled by command line argument switches or by environment variables)
            //    multiTracer.AddTracer(resolver.Resolve<IFileTracer>());
            //    //multiTracer.AddTracer(resolver.Resolve<IConsoleTracer>().SetTraceLevel(INFO));
            //    //multiTracer.SetTraceLevel(ERROR|WARNING|DEBUG|INFO|INOUT);
            //    return multiTracer;
            //});

            // register existing tracer implementations for potential later usage
            container.RegisterType<IConsoleTracer, Diagnostics.Impl.ConsoleTracer>();
            container.RegisterTypeAsTransient<IFileTracer, Solid.Infrastructure.Diagnostics.Impl.FileTracer>();

            // register trace configuration interface
            container.RegisterType<ITraceConfiguration, Diagnostics.Impl.TraceConfiguration>();
        }
    }
}
