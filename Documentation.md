# Best Practices Documentation

This document describes how the various aspects are implemented.

## Solution architecture

1. RtlTvMazeScraper.Core -- this contains the business logic of the application. It contains things like:
    * Model classes to use in the database interaction.
    * Services that implement business logic.
    * Interfaces of repositories etc. The actual instances are to be injected into the services.
2. RtlTvMazeScraper.Infrastructure -- the connection to the outside world.
    * Repositories.
    * DbContext implementation to supply Entity Framework.
3. RtlTvMazeScraper -- the UI project hosting an ASP.Net MVC application.
    * Controllers and Views, static web content.
    * Initialization of the Dependency Injection container.
4. RtlTvMazeScraper.Core -- a unittest project to test the Core.
    * Mock versions of Infrastructure components.


## Dependecy Injection

Autofac is used to provide a DI container. The setup is in App_Start/AutofacConfig.cs, where it hooks into the standard ASP.Net Dependency Injection infrastructure.

The constructors of the various controllers and services state the (interfaces of) the various components they need. DI provides the correct implementations, based on the configuration.

## Entity Framework

The strategy "Code First with existing database" was used. The models are defined in Core/Model. The DbContext is defined in Infrastructure/Data, based on an interface from Core/Interfaces.
The unittest project defines its own `MockDbContext`, implementing the same interface.

Possible enhancement: use "Migrations", so the database in the installed version is correctly set up.

## Additional setup

* "Code Analysis" is switched on for all projects (except the unittest project), using the Recommended Rules.
* StyleCop code analyzers are added to the various projects to enforce a consistent coding style.
 