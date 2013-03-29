using System;

namespace ReSTore.Web.Controllers
{
    public interface ICommandDispatcher
    {
        void Dispatch(Guid commandId, object command);
    }
}