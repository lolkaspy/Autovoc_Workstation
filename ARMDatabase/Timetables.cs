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
    
    public partial class Timetables
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public Timetables()
        {
            this.TimetableList = new ObservableCollection<TimetableList>();
        }
    
        public System.DateTime DATE { get; set; }
        public string NAME { get; set; }
    
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ObservableCollection<TimetableList> TimetableList { get; set; }
    }
}