using KLog.DataModel.Entities;
using Microsoft.AspNetCore.WebUtilities;
using System;
using System.Collections.Generic;
using System.Linq;

namespace KLog.Api.Core.Queries
{
    public class PaginatedResult<T> where T : KLogBase
    {
        public int CurrentPage { get; set; }
        public int TotalPages { get; set; }
        public int ItemsPerPage { get; set; }
        public int TotalItems { get; set; }
        public string NextPageUrl { get; set; }
        public string PreviousPageUrl { get; set; }
        public IEnumerable<T> Items { get; private set; }

        private bool HasPreviousPage
        {
            get { return CurrentPage > 1; }
        }

        private bool HasNextPage
        {
            get { return CurrentPage < TotalPages; }
        }

        public PaginatedResult(
            IEnumerable<T> items, 
            int itemCount, 
            int pageNumber, 
            int itemsPerPage,
            string requestUrl
        )
        {
            Items = items;
            TotalItems = itemCount;
            ItemsPerPage = itemsPerPage;
            CurrentPage = pageNumber;
            TotalPages = (int)Math.Ceiling(itemCount / (double)itemsPerPage);

            if (CurrentPage > 1)
                PreviousPageUrl = GenerateRequestUrl(requestUrl);

            if (HasNextPage)
                NextPageUrl = GenerateRequestUrl(requestUrl, true);
        }

        public static PaginatedResult<T> ToPaginatedResult(
            IQueryable<T> items, 
            int pageNumber, 
            int itemsPerPage,
            string requestUrl
        )
        {
            int totalItems = items.Count();
            IEnumerable<T> pageItems = items
                .Skip((pageNumber - 1) * itemsPerPage)
                .Take(itemsPerPage);

            return new PaginatedResult<T>(
                pageItems, 
                totalItems, 
                pageNumber, 
                itemsPerPage, 
                requestUrl
            );
        } 

        private string GenerateRequestUrl(string currentUrl, bool next = false)
        {
            string[] urlParts = currentUrl.Split("?");
            string url = urlParts[0];
            string queryString = urlParts[1];
            var queryParams = QueryHelpers.ParseQuery(queryString);

            if (!next && HasPreviousPage)
            {
                int.TryParse(queryParams["page"], out int currentPage);
                --currentPage;

                if (currentPage > TotalPages)
                    currentPage = TotalPages;

                queryParams["page"] = currentPage.ToString();
                return QueryHelpers.AddQueryString(url, queryParams);
            }
            else if (next && HasNextPage)
            {
                int.TryParse(queryParams["page"], out int currentPage);
                ++currentPage;

                if (currentPage <= 1)
                    currentPage = CurrentPage + 1;

                queryParams["page"] = currentPage.ToString();
                return QueryHelpers.AddQueryString(url, queryParams);
            }
            else 
                return null;
        }
    }
}
