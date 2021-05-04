using ZenExtended;

namespace ZenListView
{
    public abstract class ListElementBase<TData, TClass> : MonoSpawnable<TClass>
        where TClass : ListElementBase<TData, TClass>
    {
        public abstract void UpdateContent(TData data);
    }
}