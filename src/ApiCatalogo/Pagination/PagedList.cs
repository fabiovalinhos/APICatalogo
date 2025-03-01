namespace ApiCatalogo.Pagination
{
    public class PagedList<T> : List<T> where T : class
    {
        public int CurrentPage { get; private set; }

        public int TotalPages { get; private set; }

        public int PageSize { get; private set; }

        public int TotalCount { get; private set; }


        ////// Property expression-bodied members
        public bool HasPreview => CurrentPage > 1;

        public bool HasNext => CurrentPage < TotalPages;

        //public bool HasPreview
        //{
        //    get { return CurrentPage > 1; }
        //}

        //public bool HasNext
        //{
        //    get { return CurrentPage < TotalPages; }
        //}
        ////

        public PagedList(List<T> items, int count, int pageNumber, int pageSize)
        {
            TotalCount = count;
            PageSize = pageSize;
            CurrentPage = pageNumber;
            TotalPages = (int)Math.Ceiling(count / (double)pageSize);

            // eu poderia criar uma propriedade List<T> ao invés de herdar a List<T>
            // para mim ficaria menos sugar syntax
            AddRange(items);
        }

        public static PagedList<T> ToPagedList(IQueryable<T> source, int pageNumber,
            int pageSize)
        {
            var count = source.Count();
            var items = source.Skip((pageNumber - 1) * pageSize).Take(pageSize).ToList();


            return new PagedList<T>(items, count, pageNumber, pageSize);
        }
    }
}