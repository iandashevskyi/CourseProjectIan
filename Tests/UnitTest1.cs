namespace Prog.Services.Tests

{
    using Xunit;
    using Prog.Services;
    public class HeapSortTests
    {
        [Fact]
        public void Sort_ArrayWithMultipleElements_SortsCorrectly()
        {
            // Arrange
            int[] array = { 12, 11, 13, 5, 6, 7 };
            var heapSort = new HeapSort(array);

            // Act
            var sortedArray = heapSort.GetSortedArray();

            // Assert
            Assert.Equal(new List<int> { 5, 6, 7, 11, 12, 13 }, sortedArray);
        }

        [Fact]
        public void Sort_EmptyArray_ReturnsEmptyList()
        {
            // Arrange
            int[] array = { };
            var heapSort = new HeapSort(array);

            // Act
            var sortedArray = heapSort.GetSortedArray();

            // Assert
            Assert.Empty(sortedArray);
        }

        [Fact]
        public void Sort_SingleElementArray_ReturnsSingleElementList()
        {
            // Arrange
            int[] array = { 1 };
            var heapSort = new HeapSort(array);

            // Act
            var sortedArray = heapSort.GetSortedArray();

            // Assert
            Assert.Equal(new List<int> { 1 }, sortedArray);
        }

        [Fact]
        public void Sort_ArrayWithNegativeElements_SortsCorrectly()
        {
            // Arrange
            int[] array = { -3, -1, -2, -4, -5 };
            var heapSort = new HeapSort(array);

            // Act
            var sortedArray = heapSort.GetSortedArray();

            // Assert
            Assert.Equal(new List<int> { -5, -4, -3, -2, -1 }, sortedArray);
        }

        [Fact]
        public void GetSortedArrayAsString_ReturnsCorrectStringRepresentation()
        {
            // Arrange
            int[] array = { 3, 1, 2 };
            var heapSort = new HeapSort(array);

            // Act
            var sortedArrayString = heapSort.GetSortedArrayAsString();

            // Assert
            Assert.Equal("1, 2, 3", sortedArrayString);
        }
    }
}