//------------------------------------------------------------------------------
// <auto-generated>
//     Этот код создан по шаблону.
//
//     Изменения, вносимые в этот файл вручную, могут привести к непредвиденной работе приложения.
//     Изменения, вносимые в этот файл вручную, будут перезаписаны при повторном создании кода.
// </auto-generated>
//------------------------------------------------------------------------------

namespace ARMDatabase
{
    using System;
    using System.Collections.ObjectModel;
    
    public partial class TimetableTemp
    {
        public int TEMP_ID { get; set; }
        public System.DateTime DEPARTURE_TIME { get; set; }
        public System.DateTime ARRIVAL_TIME { get; set; }
        public int ROUTE_ID_FK { get; set; }
    
        public virtual Routes Routes { get; set; }
    }
}
