namespace Prog.Services
{
    public class HeapSort
    {
        private readonly int[] _array;
        private readonly List<int> _sortedArray;

        public HeapSort(int[] array)
        {
            _array = array;
            _sortedArray = new List<int>();
            Sort();
        }

        private void Sort()
        {
            if (_array == null || _array.Length == 0)
                return;

            // Копируем массив, чтобы не изменять исходный
            int[] arrayCopy = new int[_array.Length];
            Array.Copy(_array, arrayCopy, _array.Length);

            // Выполняем пирамидальную сортировку
            HeapSortAlgorithm(arrayCopy);

            // Сохраняем отсортированный массив
            _sortedArray.AddRange(arrayCopy);
        }

        private void HeapSortAlgorithm(int[] array)
        {
            int n = array.Length;

            // Построение max-кучи
            for (int i = n / 2 - 1; i >= 0; i--)
            {
                Heapify(array, n, i);
            }

            // Извлечение элементов из кучи
            for (int i = n - 1; i > 0; i--)
            {
                // Перемещаем текущий корень в конец
                Swap(array, 0, i);

                // Вызываем Heapify на уменьшенной куче
                Heapify(array, i, 0);
            }
        }

        private void Heapify(int[] array, int n, int i)
        {
            int largest = i; // Инициализируем наибольший элемент как корень
            int left = 2 * i + 1; // Левый дочерний элемент
            int right = 2 * i + 2; // Правый дочерний элемент

            // Если левый дочерний элемент больше корня
            if (left < n && array[left] > array[largest])
            {
                largest = left;
            }

            // Если правый дочерний элемент больше, чем самый большой элемент на данный момент
            if (right < n && array[right] > array[largest])
            {
                largest = right;
            }

            // Если самый большой элемент не корень
            if (largest != i)
            {
                Swap(array, i, largest);

                // Рекурсивно преобразуем в кучу затронутое поддерево
                Heapify(array, n, largest);
            }
        }

        private void Swap(int[] array, int i, int j)
        {
            int temp = array[i];
            array[i] = array[j];
            array[j] = temp;
        }

        public List<int> GetSortedArray()
        {
            return _sortedArray;
        }

        public string GetSortedArrayAsString()
        {
            return string.Join(", ", _sortedArray);
        }
    }
}