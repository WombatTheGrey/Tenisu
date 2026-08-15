namespace Tenisu.Application.Entity
{
    public record Page<T>
    {
        public int PageNum { get; init; }
        public int PageSize { get; init; }
        public int PageCount { get; init; }
        public IReadOnlyCollection<T> Items { get; init; }

        public Page(int pageNum, int pageSize, int pageCount, IReadOnlyCollection<T> items)
        {
            PageNum = pageNum;
            PageSize = pageSize;
            PageCount = pageCount;
            Items = items;
        }
    }
}
