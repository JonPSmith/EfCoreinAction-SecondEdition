// Copyright (c) 2020 Jon P Smith, GitHub: JonPSmith, web: http://www.thereformedprogrammer.net/
// Licensed under MIT license. See License.txt in the project root for license information.


using System;
using System.Linq;
using Microsoft.EntityFrameworkCore;

namespace Test.Chapter13Listings.Repositories
{
    public class GenericRepository<TEntity> //#A
        where TEntity : class
    {
        protected readonly DbContext Context;    //#B

        public GenericRepository(DbContext context) //#B
        {
            Context = context;                   //#B
        }

        public IQueryable<TEntity> GetEntities() //#C
        {                                        //#C
            return Context.Set<TEntity>();       //#C  
        }                                        //#C

        public TEntity FindEntity(int id)  //#D
        {
            var entity = Context.Find<TEntity>(id);     //#D
            if (entity == null)                               //#E
                throw new Exception("Could not find entity"); //#E
            return entity; //#F
        }

        public void PersistData()    //#G
        {                            //#G
            Context.SaveChanges();   //#G
        }                            //#G
    }
    /**************************************************************
    #A The generic repository will work with any entity class
    #B The repository needs the DbContext of the database
    #C This returns an IQueryable query of the entity type
    #D This finds an entity via its single, integer primary key
    #E This a rudimentary check that the entity was found
    #F The found entity is returned
    #G This calls SaveChanges to update the database
     ***************************************************************/
}