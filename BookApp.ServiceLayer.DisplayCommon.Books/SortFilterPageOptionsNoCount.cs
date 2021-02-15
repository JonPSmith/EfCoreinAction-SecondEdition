// Copyright (c) 2019 Jon P Smith, GitHub: JonPSmith, web: http://www.thereformedprogrammer.net/
// Licensed under MIT license. See License.txt in the project root for license information.

namespace BookApp.ServiceLayer.DisplayCommon.Books
{

    public class SortFilterPageOptionsNoCount : ISortFilterPage
    {
        public const int DefaultPageSize = 100;   //default page size is 100

        /// <summary>
        /// This holds the possible page sizes
        /// </summary>
        public int[] PageSizes = new[] {5, 10, 20, 50, 100, 500, 1000};

        public OrderByOptions OrderByOptions { get; set; }

        public BooksFilterBy FilterBy { get; set; }

        public string FilterValue { get; set; }

        //-----------------------------------------
        //Paging parts, which require the use of the method

        public int PageNum { get; set; } = 1;

        public int PageSize { get; set; } = DefaultPageSize;

        public bool NoCount { get; } = true;

        public bool PrevPageValid { get; private set; }
        public bool NextPageValid { get; private set; }

        /// <summary>
        /// This holds the state of the key parts of the SortFilterPage parts 
        /// </summary>
        public string PrevCheckState { get; set; }


        public void SetupRestOfDto(int numEntriesRead)
        {
            SetupRestOfDtoGivenCount(numEntriesRead);
        }

        //----------------------------------------
        //private methods

        private void SetupRestOfDtoGivenCount(int numEntriesRead)
        {
            var newCheckState = GenerateCheckState();
            if (PrevCheckState != newCheckState)
                PageNum = 1;

            NextPageValid = numEntriesRead == PageSize;
            PrevPageValid = PageNum > 1;
            PrevCheckState = newCheckState;
        }

        /// <summary>
        /// This returns a string containing the state of the SortFilterPage data
        /// that, if they change, should cause the PageNum to be set back to 0
        /// </summary>
        /// <returns></returns>
        private string GenerateCheckState()
        {
            return $"{(int) FilterBy},{FilterValue},{PageSize}";
        }
    }
}