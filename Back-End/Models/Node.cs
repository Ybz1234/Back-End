namespace Back_End.Models
{
    public class Node<T>
    {
        public T value;
        public Node<T> next;
        public Node(T value)
        {
            this.value = value;
            this.next = null;
        }
        public Node(T value, Node<T> next)
        {
            this.value = value;
            this.next = next;
        }
        public T GetValue() { return this.value; }
        public Node<T> GetNext() { return this.next; }
        public bool HasNext() { return this.next != null; }
        public void SetValue(T value) { this.value = value; }
        public void SetNext(Node<T> next) { this.next = next; }
        public int NumberOfNodes()
        {
            int count = 0;
            Node<T> current = this;

            while (current != null)
            {
                count++;
                current = current.GetNext();
            }

            return count;
        }
        public override string ToString()
        {
            return this.GetNext() == null ? $"{this.GetValue()} -> null" : $"{this.GetValue()} -> {this.GetNext()}";
        }
    }
}
