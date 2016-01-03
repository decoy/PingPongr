# PingPongr

An experimental, lightweight, RPC framework.

The framework itself is extremely lean, with minimal dependencies and most of the configuration burden pushed down to the IoC container and other Owin middlwares.  This allows for more flexibility when infrastructure concerns change without having to change your own business logic.

The core router concept is based off the infinitely useful [MediatR](https://github.com/jbogard/MediatR) by Jimmy Bogard.  See his blog post  ["Tackling cross-cutting concerns with a mediator pipeline"](https://lostechies.com/jimmybogard/2014/09/09/tackling-cross-cutting-concerns-with-a-mediator-pipeline/) for more information on using this pattern.

Also checkout the PingPongr.Sandbox example project to see how it can be used.
