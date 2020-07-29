// Copyright (c) 2020 Jon P Smith, GitHub: JonPSmith, web: http://www.thereformedprogrammer.net/
// Licensed under MIT license. See License.txt in the project root for license information.

using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace Test.Chapter11Listings.EfCode
{
    public class Notification2Entity : 
        INotifyPropertyChanged, 
        INotifyPropertyChanging //#A
    {
        public event PropertyChangedEventHandler PropertyChanged;
        public event PropertyChangingEventHandler PropertyChanging;

        protected void SetWithNotify<T>(T value, ref T field, 
            [CallerMemberName] string propertyName = "")
        {
            if (!Equals(field, value))
            {
                PropertyChanging?.Invoke(this, //#B
                    new PropertyChangingEventArgs(propertyName));
                field = value; //#C
                PropertyChanged?.Invoke(this,
                    new PropertyChangedEventArgs(propertyName));
            }
        }
    }
    /*****************************************************
    #A I have added the extra interface, INotifyPropertyChanging
    #B Now must to trigger an event before the property is changed
     * ******************************************************/
}