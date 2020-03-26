// Copyright (c) 2020 Jon P Smith, GitHub: JonPSmith, web: http://www.thereformedprogrammer.net/
// Licensed under MIT license. See License.txt in the project root for license information.

using System;
using System.Collections.Generic;
using System.Linq;
using Xunit;
using Xunit.Extensions.AssertExtensions;

namespace Test.UnitTests.TestSupportCode
{
    public class Ch02_Linq
    {
        class Review
        {
            public string VoterName { get; set; }
            public int NumStars { get; set; }
            public string Comment { get; set; }
        }

        List<Review> ReviewsList = new List<Review>
        {
            new Review
            {
                VoterName = "Jack",
                NumStars = 5,
                Comment = "Great book!"
            },
            new Review
            {
                VoterName = "Jill",
                NumStars = 1,
                Comment = "I hated it!"
            }
        };

        [Theory]
        [InlineData(true, new[] {4, 5})]
        [InlineData(false, new[] {5, 4})]
        public void LinqExampleWithExtensionMethod(bool ascending, int[] expectedResult)
        {
            //SETUP
            var numsQ = new[] {1, 5, 4, 2, 3}
                .AsQueryable(); //#A

            var result = numsQ
                .MyOrder(ascending) //#B
                .Where(x => x > 3) //#C
                .ToArray(); //#D
            /**************************************************
            A# This turns an array of integers into a queryable object
            B# I call the MyOrder extension method 
            c# Then we filter out all the numbers 3 and below
            D# This executes the IQueryable and turns the result into an array. The result is an array of ints { 4, 5 }
             * *************************************************/
            //VERIFY
            result.ShouldEqual(expectedResult);
        }

        public IQueryable<int>
            FilterAndOrder(IQueryable<int> original)
        {
            return original
                .Where(n => n > 2)
                .OrderBy(n => n);
        }

        public int[] MySpecialMethod()
        {
            var numsQ = new[] {1, 5, 4, 2, 3}
                .AsQueryable(); //#A

            //ATTEMPT
            var part1 = FilterAndOrder(numsQ); //#B
            var part2 = part1.Skip(1); //#C

            var result = part2.ToArray(); //#D

            return result;
        }

        public List<Tuple<int, string>> SortStringsAndLen()
        {
            var strQ = new[]
                    {"short", "very long", "larger"}
                .AsQueryable();

            var query = from str in strQ //#A
                let len = str.Length //#B
                orderby len //#C
                select new
                    Tuple<int, string>(len, str); //#D
            return query.ToList(); //#E
        }

        [Fact]
        public void MethodSyntaxLinqExample()
        {
            //SETUP
            int[] nums = new[] {1, 5, 4, 2, 3, 0}; //#A

            int[] result = nums //#B
                .Where(x => x > 3) //#C
                .OrderBy(x => x) //#D
                .ToArray(); //#E
            /**************************************************
            #A I create an array of integers from 0 to 5, but in a random order
            #B I am going to apply some LINQ commands and then return a new array of integers
            #C I first filter out all the integers 3 and below
            #D Now I order the numbers
            #E This turns the query back into an array. The result is an array of ints { 4, 5 }
             * *************************************************/
            //VERIFY
            result.ShouldEqual(new[] {4, 5});
        }

        [Fact]
        public void MethodSyntaxWithLetLinqExample()
        {
            //SETUP
            int[] nums = new[] {1, 5, 4, 2, 3, 0}; //#A
            string[] numLookop = new[]
                {"zero", "one", "two", "three", "four", "five"}; //#B

            IEnumerable<int> result = nums //#C
                .Select(num => new //#D
                {
                    //#D
                    num, //#D
                    numString = numLookop[num] //#D
                }) //#D
                .Where(r => r.numString.Length > 3) //#E
                .OrderBy(r => r.numString) //#F
                .Select(r => r.num); //#G
            /**************************************************
            #A I create an array of integers from 0 to 5, but in a random order
            #B This is a lookup to convert a number to its word format
            #C The result returns here is an IEnumerable<int>
            #D Here I use a anonymous type to hold the original integer value, and my numString  word lookup 
            #E I first filter out all the numbers where the word is shorter than three letters
            #F Now I order the number by the word form 
            #G I finally apply another select to choose what I want. The result is an IEnumerable<int> containing { 5,4,3,0 }
             * *************************************************/
            //VERIFY
            result.ToArray().ShouldEqual(new[] {5, 4, 3, 0});
        }

        [Fact]
        public void QuerySyntaxLinqExample()
        {
            //SETUP
            int[] nums = new[] {1, 5, 4, 2, 3, 0}; //#A

            IOrderedEnumerable<int> result = //#B
                from num in nums //#C
                where num > 3 //#D
                orderby num //#E
                select num; //#F
            /**************************************************
            #A I create an array of integers from 0 to 5, but in a random order
            #B The result returns here is an IOrderedEnumerable<int>
            #C The query syntax starts with a from <item> in <collection>
            #D I first filter out all the integers 3 and below
            #E Now I order the numbers
            #F I finally apply a select to choose what I want. The result is an IOrderedEnumerable<int> containing { 4, 5 }
             * *************************************************/
            //VERIFY
            result.ToArray().ShouldEqual(new[] {4, 5});
        }

        [Fact]
        public void QuerySyntaxWithLetLinqExample()
        {
            //SETUP
            int[] nums = new[] {1, 5, 4, 2, 3, 0}; //#A
            string[] numLookop = new[]
                {"zero", "one", "two", "three", "four", "five"}; //#B

            IEnumerable<int> result = //#C
                from num in nums //#D
                let numString = numLookop[num] //#E
                where numString.Length > 3 //#F
                orderby numString //#G
                select num; //#H
            /**************************************************
            #A I create an array of integers from 0 to 5, but in a random order
            #B This is a lookup to convert a number to its word format
            #C The result returns here is an IEnumerable<int>
            #D The query syntax starts with a from <item> in <collection>
            #E This is the let syntax that allows you to calculate a value once, and use it multiple times in the query
            #F I first filter out all the numbers where the word is shorter than three letters
            #G Now I order the number by the word form 
            #H I finally apply a select to choose what I want. The result is an IEnumerable<int> containing { 5,4,3,0 }
             * *************************************************/
            //VERIFY
            result.ToArray().ShouldEqual(new[] {5, 4, 3, 0});
        }

        [Fact]
        public void ReviewLinqExamples()
        {
            //SETUP

            //ATTEMPT
            string[] voters = ReviewsList
                .Select(p => p.VoterName).ToArray();
            double aveVotes = ReviewsList
                .Average(p => p.NumStars);
            string firstVoter = ReviewsList
                .First().VoterName;
            bool any5stars = ReviewsList
                .Any(p => p.NumStars == 1);


            //VERIFY
            voters.ShouldEqual(new string[] {"Jack", "Jill"});
            aveVotes.ShouldEqual(3);
            firstVoter.ShouldEqual("Jack");
            any5stars.ShouldBeTrue();
        }

        [Fact]
        public void SimpleLinqExampleChapter01()
        {
            //SETUP
            var numsQ = new[] {1, 5, 4, 2, 3}
                .AsQueryable(); //#A

            var result = numsQ
                .OrderBy(x => x) //#B
                .Where(x => x > 3) //#C
                .ToArray(); //#D
            /**************************************************
            A# This turns an array of integers into a queryable object
            B# First we order the numbers
            c# Then we filter out all the numbers 3 and below
            D# This turns the query back into an array. The result is an array of ints { 4, 5 }
             * *************************************************/
            //VERIFY
            result.ShouldEqual(new[] {4, 5});
        }


        [Fact]
        public void TestMySpecialMethod()
        {
            MySpecialMethod().ShouldEqual(new[] {4, 5});
        }
        /*****************************************************************
         #A The array is turned into IQueryable<int>
         #B LINQ commands to filter and sort are added, and returned as IQueryable<int>
         #C This adds a command to skip the first item after the previous commands
         #D The ToArray causes the LINQ commands to be executed, and the returns an int array
         ****************************************************************/

        [Fact]
        public void TestSortStringsAndLen()
        {
            SortStringsAndLen().ShouldEqual(new List<Tuple<int, string>>
            {
                new Tuple<int, string>(5, "short"),
                new Tuple<int, string>(6, "larger"),
                new Tuple<int, string>(9, "very long"),
            });
        }

        /****************************************************
        #A This is a Standard Query’ style LINQ command
        #B The 'let' keyword allows you to hold a emp variable
        #C We order the results by the length of the string
        #D We also put the lenfth of the string in the result
        #E As with the lambda style we need a command to execute the query
         * *************************************************/
    }
}