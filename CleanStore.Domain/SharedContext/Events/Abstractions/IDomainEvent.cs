using MediatR;
using System;
using System.Collections.Generic;
using System.Text;

namespace CleanStore.Domain.SharedContext.Events.Abstractions
{
    public interface IDomainEvent : INotification;
}
