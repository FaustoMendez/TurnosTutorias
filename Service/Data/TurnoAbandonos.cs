//------------------------------------------------------------------------------
// <auto-generated>
//     Este código se generó a partir de una plantilla.
//
//     Los cambios manuales en este archivo pueden causar un comportamiento inesperado de la aplicación.
//     Los cambios manuales en este archivo se sobrescribirán si se regenera el código.
// </auto-generated>
//------------------------------------------------------------------------------

namespace Data
{
    using System;
    using System.Collections.Generic;
    
    public partial class TurnoAbandonos
    {
        public int AbandonoId { get; set; }
        public int TurnoId { get; set; }
        public System.DateTime FechaAbandono { get; set; }
        public System.TimeSpan HoraAbandono { get; set; }
        public string Razon { get; set; }
    
        public virtual Turnos Turnos { get; set; }
    }
}
