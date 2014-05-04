namespace ReSTore.Views.Builders
{
    public interface IModelUpdateNotifier
    {
        void Notify<TModel>(string id, TModel model);
    }
}