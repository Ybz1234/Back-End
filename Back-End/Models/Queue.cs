namespace Back_End.Models
{
    public class Queue<T>
    {
        private Node<T> first;
        private Node<T> last;

        public Queue()
        {
            first = null;
            last = null;
        }
        public bool IsEmpty()
        {
            return first == null;
        }
        public void Insert(T x)
        {
            Node<T> newNode = new Node<T>(x);

            if (IsEmpty())
            {
                first = newNode;
                last = newNode;
            }
            else
            {
                last.SetNext(newNode);
                last = newNode;
            }
        }
        public T Remove()
        {
            if (IsEmpty())
            {
                throw new InvalidOperationException("Queue is empty.");
            }

            T removedData = first.GetValue();
            first = first.GetNext();

            if (first == null)
            {
                last = null;
            }

            return removedData;
        }
        public T Head()
        {
            if (IsEmpty())
            {
                throw new InvalidOperationException("Queue is empty.");
            }

            return first.GetValue();
        }
        public override string ToString()
        {
            if (IsEmpty())
            {
                return "Queue is empty.";
            }

            string result = "";
            Node<T> current = first;

            while (current != null)
            {
                result += current.GetValue();
                if (current.HasNext())
                {
                    result += " -> ";
                }
                current = current.GetNext();
            }

            return result;
        }
        public int Counter()
        {
            int count = 0;
            Node<T> current = first;

            while (current != null)
            {
                count++;
                current = current.GetNext();
            }

            return count;
        }
    }
}
