using System;
using ReSTore.Domain.Services;
using ReSTore.Infrastructure;
using ReSTore.Messages.Commands;

namespace ReSTore.Domain.CommandHandlers
{
    public class AddItemToOrderHandler : ICommandHandler<AddItemToOrder>
    {
        private readonly IRepository<Guid> _repository;
        private readonly IPricingService _pricingService;

        public AddItemToOrderHandler(IRepository<Guid> repository, IPricingService pricingService)
        {
            _repository = repository;
            _pricingService = pricingService;
        }

        public void Handle(AddItemToOrder command)
        {
            var order = _repository.GetAggregate<Order>(command.OrderId);
            if (order == null)
                throw new InvalidOperationException(string.Format("No order found with an OrderId of {0}", command.OrderId));

            order.AddItem(command.ProductId, _pricingService);
            _repository.Store(command.OrderId, order);
        }
    }
}