// Copyright (c) 2020 Jon P Smith, GitHub: JonPSmith, web: http://www.thereformedprogrammer.net/
// Licensed under MIT license. See License.txt in the project root for license information.

using System;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using Test.Chapter08Listings.EfClasses;
using Test.Chapter08Listings.EFCode;
using TestSupport.EfHelpers;
using Xunit;
using Xunit.Extensions.AssertExtensions;

namespace Test.UnitTests.TestDataLayer
{
    public class Ch08_DeleteBehaviour
    {
        [Fact]
        public void TestCreateDeletePrincipalNoDependentsOk()
        {
            //SETUP
            var options = SqliteInMemory.CreateOptions<Chapter08DbContext>();
            using (var context = new Chapter08DbContext(options))
            {
                context.Database.EnsureCreated();

                //ATTEMPT
                var entity = new DeletePrincipal();
                context.Add(entity);
                context.SaveChanges();

                //VERIFY
                context.DeletePrincipals.Count().ShouldEqual(1);
            }
        }

        [Fact]
        public void TestDeletePrincipalCascadeOk()
        {
            //SETUP
            var options = SqliteInMemory.CreateOptions<Chapter08DbContext>();
            using (var context = new Chapter08DbContext(options))
            {
                context.Database.EnsureCreated();
                var dependent = new DeleteDependentCascade();
                var entity = new DeletePrincipal {DependentCascade = dependent};
                context.Add(entity);
                context.SaveChanges();
            }
            using (var context = new Chapter08DbContext(options))
            {
                //ATTEMPT
                context.Remove(context.DeletePrincipals.Single());
                context.SaveChanges();

                //VERIFY
                context.DeletePrincipals.Count().ShouldEqual(0);
                context.Set<DeleteDependentCascade>().Count().ShouldEqual(0);
            }
        }

        [Fact]
        public void TestDeletePrincipalClientCascadeNoIncludedOk()
        {
            //SETUP
            var options = SqliteInMemory.CreateOptions<Chapter08DbContext>();
            using (var context = new Chapter08DbContext(options))
            {
                context.Database.EnsureCreated();
                var dependent = new DeleteDependentClientCascade();
                var entity = new DeletePrincipal {DependentClientCascade = dependent};
                context.Add(entity);
                context.SaveChanges();
            }

            using (var context = new Chapter08DbContext(options))
            {
                //ATTEMPT
                var entity = context.DeletePrincipals.Single();
                context.Remove(entity);
                var ex = Assert.Throws<DbUpdateException>(() => context.SaveChanges());

                //VERIFY
                ex.InnerException.Message.ShouldEqual("SQLite Error 19: 'FOREIGN KEY constraint failed'.");
            }
        }

        [Fact]
        public void TestDeletePrincipalClientCascadeIncludedOk()
        {
            //SETUP
            var options = SqliteInMemory.CreateOptions<Chapter08DbContext>();
            using (var context = new Chapter08DbContext(options))
            {
                context.Database.EnsureCreated();
                var dependent = new DeleteDependentClientCascade();
                var entity = new DeletePrincipal { DependentClientCascade = dependent };
                context.Add(entity);
                context.SaveChanges();
            }

            using (var context = new Chapter08DbContext(options))
            {
                //ATTEMPT
                var entity = context.DeletePrincipals
                    .Include(x => x.DependentClientCascade).Single();
                context.Remove(entity);
                context.SaveChanges();

                //VERIFY
                context.DeletePrincipals.Count().ShouldEqual(0);
                context.Set<DeleteDependentClientCascade>().Count().ShouldEqual(0);
            }
        }

        [Fact]
        public void TestDeletePrincipalDefaultNoIncludeOk()
        {
            //SETUP
            var options = SqliteInMemory.CreateOptions<Chapter08DbContext>();
            using (var context = new Chapter08DbContext(options))
            {
                context.Database.EnsureCreated();

                var entity = new DeletePrincipal {DependentDefault = new DeleteDependentDefault()};
                //Guard test - check the default delete behaviour for nullable key is ClientSetNull
                context.Model.FindEntityType(entity.DependentDefault.GetType().FullName)
                    .GetForeignKeys().Single().DeleteBehavior.ShouldEqual(DeleteBehavior.ClientSetNull);
                context.Add(entity);
                context.SaveChanges();
            }
            using (var context = new Chapter08DbContext(options))
            {
                //ATTEMPT
                var entity = context.DeletePrincipals 
                    .Single(p => p.DeletePrincipalId == 1);
                context.Remove(entity); 
                var ex = Assert.Throws<DbUpdateException>(() => context.SaveChanges());

                //VERIFY
                ex.InnerException.Message.ShouldEqual("SQLite Error 19: 'FOREIGN KEY constraint failed'.");             
            }
        }

        [Fact]
        public void TestDeletePrincipalDefaultWithIncludeOk()
        {
            //SETUP
            var options = SqliteInMemory.CreateOptions<Chapter08DbContext>();
            using (var context = new Chapter08DbContext(options))
            {
                context.Database.EnsureCreated();

                var entity = new DeletePrincipal { DependentDefault = new DeleteDependentDefault() };
                //Guard test - check the default delete behaviour for nullable key is ClientSetNull
                context.Model.FindEntityType(entity.DependentDefault.GetType().FullName)
                    .GetForeignKeys().Single().DeleteBehavior.ShouldEqual(DeleteBehavior.ClientSetNull);
                context.Add(entity);
                context.SaveChanges();
            }
            using (var context = new Chapter08DbContext(options))
            {
                //ATTEMPT
                var entity = context.DeletePrincipals //#A
                    .Include(p => p.DependentDefault) //#B
                    .Single(p => p.DeletePrincipalId == 1);
                context.Remove(entity); //#C
                context.SaveChanges(); //#D
                /*******************************************
                #A I read in the principal entity
                #B I include the dependent entity that has the default delete behaviour of ClientSetNull
                #C I make the principal entity for deletion
                #C I then call SaveChanges, which, because the dependent entity is tracked, then sets its foreign key to null
                 * *******************************************/

                //VERIFY
                entity.DependentDefault.DeletePrincipalId.ShouldBeNull();
                context.DeletePrincipals.Count().ShouldEqual(0);
                context.Set<DeleteDependentDefault>().Count().ShouldEqual(1);
            }
        }

        [Fact]
        public void TestDeletePrincipalNonNullDefaultNoIncludeOk()
        {
            //SETUP
            var options = SqliteInMemory.CreateOptions<Chapter08DbContext>();
            using (var context = new Chapter08DbContext(options))
            {
                context.Database.EnsureCreated();

                var entity = new DeletePrincipal { DependentNonNullDefault = new DeleteNonNullDefault() };
                //Guard test - check the deafault delete behaviour for non-nullable key is Cascade
                context.Model.FindEntityType(entity.DependentNonNullDefault.GetType().FullName)
                    .GetForeignKeys().Single().DeleteBehavior.ShouldEqual(DeleteBehavior.Cascade);
                context.Add(entity);
                context.SaveChanges();
            }
            using (var context = new Chapter08DbContext(options))
            {
                //ATTEMPT
                var entity = context.DeletePrincipals
                    .Single(p => p.DeletePrincipalId == 1);
                context.Remove(entity);
                context.SaveChanges();

                //VERIFY
                context.Set<DeleteNonNullDefault>().Count().ShouldEqual(0);
            }
        }

        [Fact]
        public void TestDeletePrincipalRestrictOk()
        {
            //SETUP
            var options = SqliteInMemory.CreateOptions<Chapter08DbContext>();
            using (var context = new Chapter08DbContext(options))
            {
                context.Database.EnsureCreated();
                var entity = new DeletePrincipal {DependentRestrict = new DeleteDependentRestrict()};
                context.Add(entity);
                context.SaveChanges();
            }
            using (var context = new Chapter08DbContext(options))
            {
                //ATTEMPT
                var entity = context.DeletePrincipals.Single();
                context.Remove(entity);
                var ex = Assert.Throws<DbUpdateException>(() => context.SaveChanges());

                //VERIFY
                ex.InnerException.Message.ShouldEqual("SQLite Error 19: 'FOREIGN KEY constraint failed'.");
            }
        }

        [Fact]
        public void TestDeletePrincipalSetNullOk()
        {
            //SETUP
            var options = SqliteInMemory.CreateOptions<Chapter08DbContext>();
            using (var context = new Chapter08DbContext(options))
            {
                context.Database.EnsureCreated();
                var entity = new DeletePrincipal { DependentSetNull = new DeleteDependentSetNull() };
                context.Add(entity);
                context.SaveChanges();

                //ATTEMPT
                context.Remove(entity);
                context.SaveChanges();

                //VERIFY
                entity.DependentSetNull.DeletePrincipalId.ShouldBeNull();
                context.DeletePrincipals.Count().ShouldEqual(0);
                context.Set<DeleteDependentSetNull>().Count().ShouldEqual(1);
            }
        }
    }
}