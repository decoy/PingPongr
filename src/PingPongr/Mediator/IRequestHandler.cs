﻿namespace PingPongr.Mediator
{
    public interface IRequestHandler<in TRequest, TResponse>
        where TRequest : IRequest<TResponse>
    {
        TResponse Handle(TRequest message);
    }
}
