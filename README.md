# Solid.Net

Introduction
------------

The Solid.Net framework enables the development of .Net C\# applications based on the SOLID design principles.
Single-responsibility principle
Open–closed principle
Liskov substitution principle
Interface segregation principle
Dependency inversion principle

The main components are:
DependencyInjectionContainer that allows Construcor injection and has hight flecibility for registering the types at the container (register instances, types or even creator funcs)

Diagnostics api that comes with:
- an abstraction of Logging and Tracing
- implememntations for output onto Console or File
- configuration api that allows selection of TraceLevel and Output depending on Environment variables, CommandLineArgs or programmatically
- support for prerequisite checks of input argument in a fluent language way

EventAggregator for loosely coupling but still based on strict types (no direct dependency of units, coupling based on events)

free configurable StateMachine implementation

Dicom abstraction that was introduced to operate on Dicom images independent on their technical source (files system vs, syngo.via memory instances)

this Dicom api enables also to:
- operate on (MR)ImageAttributes on a semantical level (independent from concrete Dicom type like EnhancedMultiFrameImages vs. SingleDicomInages) for ordering Images or to parametrize algorithms
- group images into Volume blocks
- extract the pixel data and build volume blobs
- convert the original pixelData into different target .net data types


License
-------

This software is distributed under the [MIT License](https://opensource.org/licenses/MIT).


Namespace layout
----------------

| Namespace | Content |
|:----------------:|:-----------------------------|
|Solid.Net.Infrastructure|InfrastructureDiContainer, InfrastructureRegistrar|
|Solid.Net.Infrastructure.BootStrapper|IBootStrapper, IBootable|
|Solid.Net.Infrastructure.BootStrapper.Impl|BootStrapper|
|Solid.Net.Infrastructure.Diagnostics|ConsistencyCheck, ILogger, ITracer, ITraceConfiguration|
|Solid.Net.Infrastructure.Diagnostics.Impl|FileTracer, ConsoleTracer, NullTracer, MultiTracer, TraceConfiguration|
|Solid.Net.Infrastructure.DiContainer|IDiContainer, IDiResolve, IDiRegister, IDiRegistrar, IDiIsRegistered|
|Solid.Net.Infrastructure.DiContainer.Impl|DiContainer|
|Solid.Net.Infrastructure.EventAggregator|IEventAggregator, IEvent, IHandleEvent|
|Solid.Net.Infrastructure.EventAggregator.Impl|EventAggregator|
|Solid.Net.Infrastructure.StateMachine|IStateMachine, IStateMachineCreator, IStateMachineStateConfiguration, IStateMachineInfo|
|Solid.Net.Infrastructure.StateMachine.Impl|StateMachine, StateMachineCreator, StateMachineStateConfiguration, StateMachineInfo. StateMachineDotGraphFormatter|
|Solid.Net.Infrastructure.RuntimeTypeExtensions|...|
|Solid.Net.Infrastructure_uTest|Unit tests|

| Namespace | Content |
|:----------------:|:-----------------------------|
|Solid.Net.Dicom|...|
|Solid.Net.Dicom.Impl|...|
|Solid.Net.Dicom_uTest|Unit tests|

| Namespace | Content |
|:----------------:|:-----------------------------|
|Solid.Net.DicomAdapters.FoDicom|...|
|Solid.Net.DicomAdapters.FoDicom.Impl|...|
|Solid.Net.DicomAdapters.FoDicom_uTest|Unit tests|

| Namespace | Content |
|:----------------:|:-----------------------------|
|Solid.Net.TestInfrastructure|...|
|Solid.Net.TestInfrastructure_uTest|Unit tests|


Credits
-------
