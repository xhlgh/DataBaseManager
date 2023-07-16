//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated from a template.
//
//     Manual changes to this file may cause unexpected behavior in your application.
//     Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace DataBaseManager
{
    using System;
    using System.Collections.ObjectModel;
    
    public partial class Produkty
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public Produkty()
        {
            this.Dostawy = new ObservableCollection<Dostawy>();
            this.Transakcje = new ObservableCollection<Transakcje>();
        }
    
        public int id_produktu { get; set; }
        public string nazwa { get; set; }
        public Nullable<decimal> ilosc { get; set; }
        public Nullable<decimal> cena_brutto { get; set; }
        public Nullable<int> id_kategorii { get; set; }
        public Nullable<int> id_dostawcy { get; set; }
    
        public virtual Dostawcy Dostawcy { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ObservableCollection<Dostawy> Dostawy { get; set; }
        public virtual Kategoria Kategoria { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ObservableCollection<Transakcje> Transakcje { get; set; }
    }
}
