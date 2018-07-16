# Best Practices Documentation

This document describes an overview of the implementation of various aspects.

## Solution architecture

1. RtlTvMazeScraper.Core -- this contains the business logic of the application. It contains things like:
    * Model classes to use in the database interaction.
    * Services that implement business logic.
    * Interfaces of repositories etc. The actual instances are to be injected into the services.
2. RtlTvMazeScraper.Infrastructure -- the connection to the outside world.
    * Repositories.
3. RtlTvMazeScraper.UI -- the UI project hosting an ASP.Net Core MVC application.
    * Controllers and Views, static web content would be here.
    * Initialization of the Dependency Injection container etc.
4. RtlTvMazeScraper.Core.Test -- a unittest project to test the Core.
    * Mock versions of Infrastructure components.


## Dependecy Injection

The native dependecy injection of .net core is used.

The constructors of the various controllers and services state the (interfaces of) the various components they need. DI provides the correct implementations, based on the configuration.

## Entity Framework

The strategy "Code First with existing database" was used. The models and DbContext are defined in Core/Model.

Future enhancement: use "Migrations", so the database in the installed version is correctly set up.

## Additional setup

* "Code Analysis" nalyzer is used for all projects (except the unittest project), using the Recommended Rules.
* StyleCop code analyzers are added to the various projects to enforce a consistent coding style.
* "Polly" (https://github.com/App-vNext/Polly) is used to retry request when the server was too busy. 
