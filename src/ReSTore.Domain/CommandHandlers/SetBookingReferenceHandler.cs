using System;
using ReSTore.Infrastructure;
using ReSTore.Messages.Commands;

namespace ReSTore.Domain.CommandHandlers
{
    public class SetBookingReferenceHandler: ICommandHandler<SetBookingReference>
    {
        private readonly IRepository<Guid> _repository;

        public SetBookingReferenceHandler(IRepository<Guid> repository)
        {
            _repository = repository;
        }

        public void Handle(SetBookingReference command)
        {
            var order = _repository.GetAggregate<Order>(command.OrderId);
            if (order == null)
                throw new InvalidOperationException(string.Format("No order found with an OrderId of {0}", command.OrderId));

            order.SetBookingReference(command.BookingReference);
            _repository.Store(command.OrderId, order, command.ApplyCommandHeaders());
        }
    }
}