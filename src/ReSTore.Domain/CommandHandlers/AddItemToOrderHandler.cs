using System;
using ReSTore.Infrastructure;
using ReSTore.Messages.Commands;

namespace ReSTore.Domain.CommandHandlers
{
    public class AddItemToOrderHandler : ICommandHandler<AddItemToOrder>
    {
        private readonly IRepository<Guid> _repository;

        public AddItemToOrderHandler(IRepository<Guid> repository)
        {
            _repository = repository;
        }

        public void Handle(AddItemToOrder command)
        {
            var order = _repository.GetAggregate<Order>(command.OrderId);
            if (order == null)
                throw new InvalidOperationException(string.Format("No order found with an OrderId of {0}", command.OrderId));

            order.AddItem(command.ItemId);
            _repository.Store(command.OrderId, order);
        }
    }
}