namespace ReSTore.Domain
{
    public interface ICommandHandler
    {
        
    }

    public interface ICommandHandler<T> : ICommandHandler
    {
        void Handle(T command);
    }
}