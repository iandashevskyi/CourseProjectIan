namespace Prog.Model
{
    public class ArrayModel
    {
        public int array { get; set; }
    }

    public class ArrayRequest
    {
        public int[] Array { get; set; }
    }

    public class ArrayResponse
    {
        public string Message { get; set; }
        public int[] Array { get; set; }
    }

    public class AddElementsRequest
    {
        public int[] Elements { get; set; }
        public int? Index { get; set; } // Для вставки после указанного индекса
    }

    public class SortPartRequest
    {
        public int StartIndex { get; set; }
        public int EndIndex { get; set; }
    }

    public class GenerateArrayRequest
    {
        public int Size { get; set; }
    }

    public class ClearArrayResponse
    {
        public string Message { get; set; }
    }
}