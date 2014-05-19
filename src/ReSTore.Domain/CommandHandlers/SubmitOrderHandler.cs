using System;
using ReSTore.Infrastructure;
using ReSTore.Messages.Commands;

namespace ReSTore.Domain.CommandHandlers
{
    public class SubmitOrderHandler : ICommandHandler<SubmitOrder>
    {
        private readonly IRepository<Guid> _repository;

        public SubmitOrderHandler(IRepository<Guid> repository)
        {
            _repository = repository;
        }

        public void Handle(SubmitOrder command)
        {
            var order = _repository.GetAggregate<Order>(command.OrderId);
            if (order == null)
                throw new InvalidOperationException(string.Format("No order found with an OrderId of {0}", command.OrderId));
            order.Submit();
            _repository.Store(command.OrderId, order, command.ApplyCommandHeaders());
        }
    }
}