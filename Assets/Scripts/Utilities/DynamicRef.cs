namespace EVRC
{
    public class DynamicRef<T>
    {
        public delegate T Getter();

        private Getter getter;
        public T Current
        {
            get {
                return getter();
            }
        }

        public DynamicRef(Getter getter)
        {
            this.getter = getter;
        }
    }
}
