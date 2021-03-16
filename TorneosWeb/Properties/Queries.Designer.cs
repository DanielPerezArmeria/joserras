﻿//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//     Runtime Version:4.0.30319.42000
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace TorneosWeb.Properties {
    using System;
    
    
    /// <summary>
    ///   A strongly-typed resource class, for looking up localized strings, etc.
    /// </summary>
    // This class was auto-generated by the StronglyTypedResourceBuilder
    // class via a tool like ResGen or Visual Studio.
    // To add or remove a member, edit your .ResX file then rerun ResGen
    // with the /str option, or rebuild your VS project.
    [global::System.CodeDom.Compiler.GeneratedCodeAttribute("System.Resources.Tools.StronglyTypedResourceBuilder", "15.0.0.0")]
    [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
    [global::System.Runtime.CompilerServices.CompilerGeneratedAttribute()]
    internal class Queries {
        
        private static global::System.Resources.ResourceManager resourceMan;
        
        private static global::System.Globalization.CultureInfo resourceCulture;
        
        [global::System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode")]
        internal Queries() {
        }
        
        /// <summary>
        ///   Returns the cached ResourceManager instance used by this class.
        /// </summary>
        [global::System.ComponentModel.EditorBrowsableAttribute(global::System.ComponentModel.EditorBrowsableState.Advanced)]
        internal static global::System.Resources.ResourceManager ResourceManager {
            get {
                if (object.ReferenceEquals(resourceMan, null)) {
                    global::System.Resources.ResourceManager temp = new global::System.Resources.ResourceManager("TorneosWeb.Properties.Queries", typeof(Queries).Assembly);
                    resourceMan = temp;
                }
                return resourceMan;
            }
        }
        
        /// <summary>
        ///   Overrides the current thread's CurrentUICulture property for all
        ///   resource lookups using this strongly typed resource class.
        /// </summary>
        [global::System.ComponentModel.EditorBrowsableAttribute(global::System.ComponentModel.EditorBrowsableState.Advanced)]
        internal static global::System.Globalization.CultureInfo Culture {
            get {
                return resourceCulture;
            }
            set {
                resourceCulture = value;
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to select top 1 t.fecha
        ///from torneos t, jugadores j, resultados r
        ///where r.torneo_id = t.id and r.jugador_id = j.id and j.id = &apos;{0}&apos;
        ///order by fecha desc.
        /// </summary>
        internal static string FindLastPlayedTournament {
            get {
                return ResourceManager.GetString("FindLastPlayedTournament", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to select j.nombre, elim.nombre as eliminado, sum(e.eliminaciones) as kos
        ///from knockouts e, jugadores j, jugadores elim, torneos t
        ///where e.jugador_id = j.id and e.eliminado_id = elim.id and t.id = e.torneo_id {0} group by j.nombre, elim.nombre order by kos desc.
        /// </summary>
        internal static string GetAllKOs {
            get {
                return ResourceManager.GetString("GetAllKOs", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to select tt.nombre, count(tt.posicion) as ult_lugares
        ///from
        ///(select j.nombre, dt.posicion, t.entradas
        ///from jugadores j, torneos t, resultados dt
        ///where dt.torneo_id = t.id and dt.jugador_id = j.id and dt.posicion = t.entradas {0}
        ///) as tt
        ///group by nombre.
        /// </summary>
        internal static string GetBundy {
            get {
                return ResourceManager.GetString("GetBundy", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to select j.nombre, count(dt.burbuja) as burbuja
        ///from jugadores j, torneos t, resultados dt
        ///where dt.torneo_id = t.id and dt.jugador_id = j.id and dt.burbuja = 1 {0} group by nombre.
        /// </summary>
        internal static string GetBurbuja {
            get {
                return ResourceManager.GetString("GetBurbuja", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to select detalles.id, detalles.nombre, detalles.costo_total, detalles.premios, detalles.podios, detalles.victorias, detalles.burbujas, detalles.kos,
        ///	detalles.torneos, detalles.rebuys
        ///from
        ///(select j.id, j.nombre, sum(t.precio_buyin + (d.rebuys * t.precio_rebuy)) as costo_total, count(t.id) as torneos, sum(d.kos) as kos,
        ///	sum(d.premio + d.premio_bounties) as premios, count(case when d.posicion = 1 then 1 end) as victorias, sum(d.rebuys) as rebuys,
        ///	count(case when d.podio = 1 then 1 end) as podios, count( [rest of string was truncated]&quot;;.
        /// </summary>
        internal static string GetDetalleJugador {
            get {
                return ResourceManager.GetString("GetDetalleJugador", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to select j.nombre, elim.nombre as eliminado, sum(e.eliminaciones) as eliminaciones from knockouts e, jugadores j, jugadores elim
        ///where j.id = &apos;{0}&apos; and e.jugador_id = j.id and e.eliminado_id = elim.id
        ///group by j.nombre, elim.nombre order by eliminaciones desc.
        /// </summary>
        internal static string GetKnockoutsByPlayer {
            get {
                return ResourceManager.GetString("GetKnockoutsByPlayer", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to select res.nombre, count(res.nombre) as podios_neg
        ///from
        ///(select t.fecha, j.nombre, sum(d.premio) as premio, sum((t.precio_rebuy * d.rebuys) + t.precio_buyin ) as costos
        ///from jugadores j, resultados d, torneos t
        ///where d.podio = 1 and j.id = d.jugador_id and t.id = d.torneo_id {0} group by t.fecha, j.nombre
        ///) as res
        ///where res.premio - res.costos &lt; 0
        ///group by res.nombre order by podios_neg desc.
        /// </summary>
        internal static string GetPodiosNegativos {
            get {
                return ResourceManager.GetString("GetPodiosNegativos", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to select detalles.id, detalles.nombre, detalles.costo_total, detalles.premios, detalles.podios, detalles.victorias, detalles.burbujas, detalles.kos,
        ///	detalles.torneos, detalles.rebuys
        ///from
        ///(select j.id, j.nombre, sum(t.precio_buyin + (d.rebuys * t.precio_rebuy)) as costo_total, count(t.id) as torneos,
        ///	sum(d.premio + d.premio_bounties) as premios, count(case when d.posicion = 1 then 1 end) as victorias, sum(d.rebuys) as rebuys,
        ///	count(case when d.podio = 1 then 1 end) as podios, count(case when d.burbuja [rest of string was truncated]&quot;;.
        /// </summary>
        internal static string GetStats {
            get {
                return ResourceManager.GetString("GetStats", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to select liga_fees.jugador_id, premios_liga.premios, fees
        ///from
        ///(select id as jugador_id, sum(total) as fees from
        ///(select j.id, l.fee, (sum(r.rebuys * l.fee) + count(tl.torneo_id) * l.fee) as total
        ///from torneos_liga tl, resultados r, jugadores j, ligas l
        ///where j.id = r.jugador_id and tl.torneo_id = r.torneo_id
        ///	and l.id = tl.liga_id
        ///group by j.id, l.fee) as total_fees
        ///group by id) as liga_fees,
        ///(
        ///	select j.id, coalesce(ptl.premios, 0) as premios from
        ///	(select j.id from jugadores j) as j left outer j [rest of string was truncated]&quot;;.
        /// </summary>
        internal static string GetTotalLigaProfits {
            get {
                return ResourceManager.GetString("GetTotalLigaProfits", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to select liga_fees.jugador_id, premios_liga.premios, fees
        ///from
        ///(select id as jugador_id, sum(total) as fees
        ///from
        ///(select j.id, l.fee, (sum(r.rebuys * l.fee) + count(tl.torneo_id) * l.fee) as total
        ///from torneos_liga tl, resultados r, jugadores j, ligas l
        ///where j.id = r.jugador_id and tl.torneo_id = r.torneo_id
        ///	and l.id = tl.liga_id and j.id = &apos;{0}&apos;
        ///group by j.id, l.fee) as total_fees
        ///group by id) as liga_fees,
        ///(select coalesce(sum(ptl.premio), 0) as premios from puntos_torneo_liga ptl
        ///where ptl.jug [rest of string was truncated]&quot;;.
        /// </summary>
        internal static string GetTotalLigaProfitsByPlayerId {
            get {
                return ResourceManager.GetString("GetTotalLigaProfitsByPlayerId", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to insert into torneos (fecha, precio_buyin, precio_rebuy, entradas, rebuys, bolsa, tipo, premio_x_bounty, premiacion) output INSERTED.ID
        ///values (&apos;{0}&apos;, {1}, {2}, {3}, {4}, {5}, &apos;{6}&apos;, {7}, &apos;{8}&apos; ).
        /// </summary>
        internal static string InsertTorneo {
            get {
                return ResourceManager.GetString("InsertTorneo", resourceCulture);
            }
        }
    }
}
