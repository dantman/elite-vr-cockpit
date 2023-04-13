namespace EVRC.Core
{
    public class Ref<T>
    {
        public T current;

        public Ref() { }
        public Ref(T value)
        {
            current = value;
        }
    }
}
