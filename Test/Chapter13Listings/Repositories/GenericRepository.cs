// Copyright (c) 2020 Jon P Smith, GitHub: JonPSmith, web: http://www.thereformedprogrammer.net/
// Licensed under MIT license. See License.txt in the project root for license information.


using System;
using System.Linq;
using System.Threading.Tasks;
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

        public async Task<TEntity> FindEntityAsync(int id)  //#D
        {
            var entity = await Context.FindAsync<TEntity>(id); //#E

            if (entity == null)                               //#F
                throw new Exception("Could not find entity"); //#F 
            
            return entity; //#G
        }

        public Task PersistDataAsync()        //#H
        {                                     //#H
            return Context.SaveChangesAsync();//#H
        }                                     //#H
    }
    /**************************************************************
    #A The generic repository will work with any entity class
    #B The repository needs the DbContext of the database
    #C This returns an IQueryable query of the entity type
    #D This method finds and returns a entity with a integer primary key
    #E This finds an entity via its single, integer primary key
    #F This a rudimentary check that the entity was found
    #G The found entity is returned
    #H This calls SaveChanges to update the database
     ***************************************************************/
}