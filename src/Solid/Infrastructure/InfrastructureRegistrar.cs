//----------------------------------------------------------------------------------
// <copyright file="InfrastructureRegistrar.cs" company="Siemens Healthcare GmbH">
// Copyright (C) Siemens Healthcare GmbH, 2022-2023. All Rights Reserved. Confidential.
// Author: Steffen Hanke
// </copyright>
//----------------------------------------------------------------------------------

using Solid.Infrastructure.Diagnostics;
using Solid.Infrastructure.DiContainer;
using Solid.Infrastructure.Environment;
using Solid.Infrastructure.EventAggregator;
using Solid.Infrastructure.StateMachine;

namespace Solid.Infrastructure
{
    /// <summary>
    /// InfrastructureRegistrar
    /// 
    /// registers basic infrastructure components like:
    /// - EventAggregator
    /// - StateMachineCreator
    /// - FolderProvider
    /// - Tracing infrastucure (but initially does not trace without further configuration unless envronment varaiables TraceTarget and TraceLevel are defines approriate)
    /// 
    /// remark:
    /// there happens no actual creation of any of the registerted components
    /// this is just the definition which type is to be instanciated or which creator func is to be executed when a specific interfaces is queried at the DiContainer
    /// 
    /// example usage:
    /// // 1) create empty DI-Container
    /// using var diContainer = new DiContainer();
    /// // 2) register basic infrastucture components (e.g. basic tracing components, folder provider ...)
    /// diContainer.Register(new InfrastructureRegistrar());
    ///
    /// </summary>
    public class InfrastructureRegistrar : IDiRegistrar
    {
        public void Register(IDiContainer container)
        {
            ConsistencyCheck.EnsureArgument(container).IsNotNull();


            // a) register SOLID infrastructure components
            container.RegisterType<IEventAggregator, EventAggregator.Impl.EventAggregator>();
            container.RegisterType<IStateMachineCreator, StateMachine.Impl.StateMachineCreator>();


            // b) register basic infrastructure components
            container.RegisterTypeAsTransient<IFolderProvider, Environment.Impl.FolderProvider>();
            container.RegisterType<IMultiThreadingHelper, Environment.Impl.MultiThreadingHelper>();


            // c) register basic trace environment
            container.RegisterCreatorAsTransient<ITracer>((resolver, creatingType) 
                => resolver.Resolve<IMultiTracer>().CreateBaseDomainTracer(creatingType));

            container.RegisterType<IMultiTracer, Diagnostics.Impl.MultiTracer>();

            // register existing tracer implementations for potential later usage
            container.RegisterTypeAsTransient<IConsoleTracer, Diagnostics.Impl.ConsoleTracer>();
            container.RegisterTypeAsTransient<IFileTracer, Diagnostics.Impl.FileTracer>();

            // register trace configuration interface
            container.RegisterType<ITraceConfiguration, Diagnostics.Impl.TraceConfiguration>();
        }
    }
}
