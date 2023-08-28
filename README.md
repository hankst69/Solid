# Solid


Introduction
------------

The Solid framework enables the development of .Net C\# applications based on the __SOLID__ design principles.

|Acronym|Principle|
|:---:|:---|
| __S__ |Single-responsibility principle|
| __O__ |Open–closed principle|
| __L__ |Liskov substitution principle|
| __I__ |Interface segregation principle|
| __D__ |Dependency inversion principle|



Components
----------

1. __DependencyInjectionContainer__
   * Implements constructor injection
   * Comes with high flexibility for registering the units at the container
   * It allows registration of existing object __instances__ but also the registering of a __type__ or a __creator func__.
   * In case of registering types or creator funcs, the lifecycle can be choosen between __Transient__ or __Singleton__.
   * Throws TypeNotRegisteredException when trying to resolve a type that was not registered.
   * Throws TypeNotRegisteredException when the type to resolve is registered but any of its dependencies is not registered.
   * Throws CircularDependencyException when the typ to resolve depends on arguments that (over multiple steps) redirect to type to reslove.

2. __Diagnostics__
   * Provides abstraction of Logging and Tracing
   * Comes with implementations for output onto Console or File
   * Comes with TraceConfiguration api that allows selection of TraceLevel and Output depending on Environment variables, CommandLineArgs or programmatically
   * Supports validation of function argument prerequisites via a fluent ConsitencyCheck implementation

3. __EventAggregator__
   * For loosely coupling of units but still based on strict types (no direct dependency of units, coupling based on events)

4. __StateMachine__
   * Flexible configuration of states, triggers. permissions, entry and leave handlers with fluent configuration api
   * Support of export into DotGraph format
   * Currently no support for sub states

5. __Dicom__
   * An abstraction that was introduced to operate on Dicom images independent on their technical source (files system vs memory instances)
   * This Dicom api enables also to:
     - operate on (MR)ImageAttributes on a semantical level (independent from concrete Dicom type like EnhancedMultiFrameImages vs. SingleDicomInages) for ordering Images or to parametrize algorithms
     - group images into Volume blocks
     - extract the pixel data and build volume blobs
     - convert the original pixelData into different .net target data types


License
-------

This software is distributed under the [MIT License](https://opensource.org/licenses/MIT).


Namespace layout
----------------

| Namespace | Content |
|:----------|:--------|
|Solid.Infrastructure|InfrastructureRegistrar, InfrastructureDiContainer|
|Solid.Infrastructure.BootStrapper|IBootStrapper, IBootable|
|Solid.Infrastructure.BootStrapper.Impl|BootStrapper|
|Solid.Infrastructure.Diagnostics|ConsistencyCheck, ILogger, ITracer, ITraceConfiguration|
|Solid.Infrastructure.Diagnostics.Impl|FileTracer, ConsoleTracer, NullTracer, MultiTracer, TraceConfiguration|
|Solid.Infrastructure.DiContainer|IDiContainer, IDiResolve, IDiRegister, IDiRegistrar, IDiIsRegistered|
|Solid.Infrastructure.DiContainer.Impl|DiContainer|
|Solid.Infrastructure.EventAggregator|IEventAggregator, IEvent, IHandleEvent|
|Solid.Infrastructure.EventAggregator.Impl|EventAggregator|
|Solid.Infrastructure.StateMachine|IStateMachine, IStateMachineCreator, IStateMachineStateConfiguration, IStateMachineInfo|
|Solid.Infrastructure.StateMachine.Impl|StateMachine, StateMachineCreator, StateMachineStateConfiguration, StateMachineInfo. StateMachineDotGraphFormatter|
|Solid.Infrastructure.RuntimeTypeExtensions|...|
|Solid.Infrastructure_uTest|Unit tests|
|||
|__Namespace__|__Content__|
|Solid.Dicom|...|
|Solid.Dicom.Impl|...|
|Solid.Dicom_uTest|Unit tests|
|||
|__Namespace__|__Content__|
|Solid.DicomAdapters.FoDicom|...|
|Solid.DicomAdapters.FoDicom.Impl|...|
|Solid.DicomAdapters.FoDicom_uTest|Unit tests|
|||
|__Namespace__|__Content__|
|Solid.TestInfrastructure|...|
|Solid.TestInfrastructure_uTest|Unit tests|

