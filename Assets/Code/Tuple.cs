namespace JoyLib.Code
{
    public class Tuple<K, L>
    {
        public Tuple(K first, L second)
        {
            First = first;
            Second = second;
        }

        public K First
        {
            get;
            set;
        }

        public L Second
        {
            get;
            set;
        }
    }
}
