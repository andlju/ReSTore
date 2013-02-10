namespace ReSTore.Domain
{
    public interface ICommandHandler<T>
    {
        void Handle(T command);
    }
}