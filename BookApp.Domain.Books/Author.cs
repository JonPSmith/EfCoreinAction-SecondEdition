// Copyright (c) 2019 Jon P Smith, GitHub: JonPSmith, web: http://www.thereformedprogrammer.net/
// Licensed under MIT license. See License.txt in the project root for license information.

using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using BookApp.Domain.Books.DomainEvents;
using BookApp.Domain.Books.SupportTypes;
using GenericEventRunner.DomainParts;

namespace BookApp.Domain.Books
{
    public class Author : EventsAndCreatedUpdated  //#A
    {
        private string _name;   //#B
        private HashSet<BookAuthor> _booksLink;

        public Author(string name, string email)
        {                                       
            _name = name;                       
            Email = email;                      
        }                                       

        public int AuthorId { get;  private set; }

        [Required(AllowEmptyStrings = false)]
        [MaxLength(100)]
        public string Name
        {
            get => _name;
            set   //#C
            {
                if (value != _name &&    //#D
                    AuthorId != default) //#D
                {
                    AddEvent(                          //#D
                        new AuthorNameUpdatedEvent()); //#D
                    AddEvent( //#D
                        new AuthorNameUpdatedEvent(), EventToSend.DuringSave);
                }
                _name = value;
            }
        }

        [MaxLength(100)]
        public string Email { get; private set; }

        //------------------------------
        //Relationships

        public ICollection<BookAuthor> BooksLink => _booksLink?.ToList();
    }
    /*******************************************************
    #A Adding the EntityEventsBase will provide the methods to send an event
    #B This is the backing field for the Name property. EF Core will read/write this
    #C You make the setting public and  override the setter to add the event test/send
    #D If the Name has changes and it's not a new Author then it sends a domain event
     *******************************************************/

}