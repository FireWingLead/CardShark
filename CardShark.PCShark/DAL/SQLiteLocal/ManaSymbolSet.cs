//------------------------------------------------------------------------------
// <auto-generated>
//    This code was generated from a template.
//
//    Manual changes to this file may cause unexpected behavior in your application.
//    Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace CardShark.PCShark.DAL.SQLiteLocal
{
    using System;
    using System.Collections.Generic;
    
    public partial class ManaSymbolSet
    {
        public ManaSymbolSet()
        {
            this.ManaSymbols = new HashSet<ManaSymbol>();
            this.Sets = new HashSet<Set>();
        }
    
        public long Id { get; set; }
    
        public virtual ICollection<ManaSymbol> ManaSymbols { get; set; }
        public virtual ICollection<Set> Sets { get; set; }
    }
}