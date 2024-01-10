
using Licitaciones.BaseDato;
using Licitaciones.ViewModels;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Threading;
using System.Configuration;
using System;
using System.Security.Principal;
using System.Text.RegularExpressions;
using MySqlX.XDevAPI.Relational;
using Licitacion.Servicios.Utiles;
using System.Data.Entity.Core.Metadata.Edm;

namespace Licitacion.Servicios
{
    public class ServicioLicitacion
    {
        private static readonly log4net.ILog log = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
        private static string ruta = System.Web.HttpRuntime.AppDomainAppPath + "\\bin\\log4net.config";
        public List<licitacionGrillaViewModels> listaLicitaciones
        (ref int recordsTotal, string sortColumn, string sortColumnDir, string searchValue,
            string nroExpediente, string nombreObra, string qObra, int? idEstado, DateTime? fechaPub, int? idOrganismo, int? nroObra,
            int pageSize = 0, int skip = 0)
        {
            log4net.Config.XmlConfigurator.Configure(new FileInfo(ruta));
            log.Info("Grilla Licitaciones Administrador");
            List<licitacionGrillaViewModels> lista = new List<licitacionGrillaViewModels>();
            try
            {
                using (db_meieEntities db = new db_meieEntities())
                {

                    var query = from lic in db.PryProyecto
                                join edo in db.PryEstadoEtapa
                                on lic.GrlTypeState_Id equals edo.Id
                                join pyl in db.PryLicitacion
                                on lic.Id equals pyl.IdPryProyecto                               
                                    into pylo
                                from pyj in pylo.DefaultIfEmpty()
                                join org in db.PryOrganismosEjecutores
                                on lic.PryOrganismoEjecutor_Id equals org.Id
                                    into orgo
                                from orgj in orgo.DefaultIfEmpty()

                                join licProy in db.LicProyecto
                                    on lic.Id equals licProy.idProyecto
                                        into licProyo
                                from licProyj in licProyo.DefaultIfEmpty()                               
                                where edo.PryStage_Id == 49 && lic.Eliminado == false
                                select new licitacionGrillaViewModels
                                {
                                    nombreEtapa = edo.Name,
                                    idEtapa = edo.Id,
                                    nombreObra = lic.Nombre,
                                    idObra = lic.Id,
                                    fechaApertura = pyj.FechaApertura,
                                    fechaPublicacion = pyj.FechaPublicacionDesde,
                                    montoObra = pyj.MontoOficial,
                                    nroExpediente = lic.Expediente,
                                    nombreOrganismo = orgj.NombreOrganismo,
                                    idOrganismo = lic.PryOrganismoEjecutor_Id,
                                    Eliminado = lic.Eliminado,
                                    idOrgano = lic.ufiOrgano_Id,
                                    idOffice = lic.oficinaCarga,
                                    idOrganoReceptor = lic.ufiOrganoReceptor_Id,
                                    tieneActaBool = licProyj.tieneActa
                                };

                    if (!string.IsNullOrEmpty(qObra))
                    {
                        if(qObra != "*")
                        {
                            var array = qObra.Split(',');
                            List<int?> listaObra = new List<int?>();
                            foreach (var item in array)
                            {
                                listaObra.Add(Convert.ToInt32(item));
                            }
                            query = query.Where(x => listaObra.Contains(x.idObra));
                        }
                    }
                    if (idOrganismo != 0)
                    {
                        query = query.Where(x => x.idOrganismo == idOrganismo);
                    }
                    if (idEstado != 0)
                    {
                        query = query.Where(x => x.idEtapa == idEstado);
                    }
                    if (!string.IsNullOrEmpty(nombreObra))
                    {
                        query = query.Where(x => x.nombreObra.Contains(nombreObra));
                    }
                    if (nroObra.HasValue)
                    {
                        if(nroObra != 0)
                        {
                            query = query.Where(x => x.idObra == nroObra);
                        }
                    }
                    if (!string.IsNullOrEmpty(nroExpediente))
                    {
                        query = query.Where(x => x.nroExpediente.Contains(nroExpediente));
                    }
                    if (fechaPub.HasValue)
                    {
                        query = query.Where(x => x.fechaPublicacion == fechaPub);
                    }                    
                    if (!(string.IsNullOrEmpty(sortColumn) && string.IsNullOrEmpty(sortColumnDir)))
                    {
                        if ((sortColumn == "nombreObra") && (sortColumnDir == "desc"))
                        {
                            query = query.OrderByDescending(x => x.nombreObra);
                        }
                        if ((sortColumn == "nombreObra") && (sortColumnDir == "asc"))
                        {
                            query = query.OrderBy(x => x.nombreObra);
                        }
                        if ((sortColumn == "nroExpedienteString") && (sortColumnDir == "desc"))
                        {
                            query = query.OrderByDescending(x => x.nroExpediente);
                        }
                        if ((sortColumn == "nroExpedienteString") && (sortColumnDir == "asc"))
                        {
                            query = query.OrderBy(x => x.nroExpediente);
                        }
                        if ((sortColumn == "fechaPublicacionString") && (sortColumnDir == "desc"))
                        {
                            query = query.OrderByDescending(x => x.fechaPublicacion);
                        }
                        if ((sortColumn == "fechaPublicacionString") && (sortColumnDir == "asc"))
                        {
                            query = query.OrderBy(x => x.fechaPublicacion);
                        }
                        if ((sortColumn == "nombreOrganismoString") && (sortColumnDir == "asc"))
                        {
                            query = query.OrderBy(x => x.nombreOrganismo);
                        }
                        if ((sortColumn == "nombreOrganismoString") && (sortColumnDir == "desc"))
                        {
                            query = query.OrderByDescending(x => x.nombreOrganismo);
                        }
                        if ((sortColumn == "montoObraString") && (sortColumnDir == "asc"))
                        {
                            query = query.OrderBy(x => x.montoObra);
                        }
                        if ((sortColumn == "montoObraString") && (sortColumnDir == "desc"))
                        {
                            query = query.OrderByDescending(x => x.montoObra);
                        }
                        if ((sortColumn == "nombreEtapa") && (sortColumnDir == "asc"))
                        {
                            query = query.OrderBy(x => x.nombreEtapa);
                        }
                        if ((sortColumn == "nombreEtapa") && (sortColumnDir == "desc"))
                        {
                            query = query.OrderByDescending(x => x.nombreEtapa);
                        }
                        if ((sortColumn == "accion") && (sortColumnDir == "asc"))
                        {
                            query = query.OrderBy(x => x.nombreObra);
                        }
                        if ((sortColumn == "accion") && (sortColumnDir == "desc"))
                        {
                            query = query.OrderByDescending(x => x.nombreObra);
                        }
                        if ((sortColumn == "idObra") && (sortColumnDir == "asc"))
                        {
                            query = query.OrderBy(x => x.idObra);
                        }
                        if ((sortColumn == "idObra") && (sortColumnDir == "desc"))
                        {
                            query = query.OrderByDescending(x => x.idObra);
                        }

                    }
                    else
                    {
                        query = query = query.OrderBy(x => x.nombreObra);
                    }

                    query = query.Take(500);
                    var algo = query.ToList();
                    var oficina = System.Web.HttpContext.Current.Session["idOficina"].ToString();
                    if (Convert.ToInt32(oficina) > 1)
                    {
                        var usuario = System.Web.HttpContext.Current.Session["idUsuario"].ToString();
                        query = ProyectFilterByOffice(Convert.ToInt32(oficina), Convert.ToInt32(usuario), query);
                        var lst = query.Skip(skip).Take(pageSize);
                        lista = lst.ToList();
                    }
                    else if (Convert.ToInt32(oficina) == 1)
                    {
                        var lst = query.Skip(skip).Take(pageSize);
                        lista = lst.ToList();
                    }
                    recordsTotal = query.Count();

                    return lista;
                }
            }
            catch (Exception ex)
            {
                var a = ex.InnerException;
                var e = ex.Data;
                log.Error("Error en Grilla Licitaciones Administrador", ex);
            }
            return lista;
        }
        public tarjetaLicViewModels calcularTarjeta
        (string nroExpediente, string nombreObra, string qObra, DateTime? fechaPub, int? idOrganismo, int? nroObra)
        {
            log4net.Config.XmlConfigurator.Configure(new FileInfo(ruta));
            log.Info("Tarjeta Licitaciones");
            tarjetaLicViewModels tarjeta = new tarjetaLicViewModels();
            tarjeta.qAbierta = 0;
            tarjeta.qAdjudicada = 0;
            tarjeta.qAPublicar = 0;
            tarjeta.qDesierta = 0;
            tarjeta.qFirmada = 0;
            tarjeta.qFracasada = 0;
            tarjeta.qPreAdjudicada = 0;
            tarjeta.qPublicada = 0;
            try
            {
                using (db_meieEntities db = new db_meieEntities())
                {
                    var query = from lic in db.PryProyecto
                                join edo in db.PryEstadoEtapa
                                on lic.GrlTypeState_Id equals edo.Id
                                join pyl in db.PryLicitacion
                                on lic.Id equals pyl.IdPryProyecto
                                    into pylo
                                from pyj in pylo.DefaultIfEmpty()
                                join org in db.PryOrganismosEjecutores
                                on lic.PryOrganismoEjecutor_Id equals org.Id
                                    into orgo
                                from orgj in orgo.DefaultIfEmpty()
                                where edo.PryStage_Id == 49 && lic.Eliminado == false
                                select new licitacionGrillaViewModels
                                {
                                    nombreEtapa = edo.Name,
                                    idEtapa = edo.Id,
                                    nombreObra = lic.Nombre,
                                    idObra = lic.Id,
                                    fechaApertura = pyj.FechaApertura,
                                    fechaPublicacion = pyj.FechaPublicacionDesde,
                                    montoObra = pyj.MontoOficial,
                                    nroExpediente = lic.Expediente,
                                    nombreOrganismo = orgj.NombreOrganismo,
                                    idOrganismo = lic.PryOrganismoEjecutor_Id,
                                    Eliminado = lic.Eliminado,
                                    idOrgano = lic.ufiOrgano_Id,
                                    idOffice = lic.oficinaCarga,
                                    idOrganoReceptor = lic.ufiOrganoReceptor_Id
                                };

                    if (!string.IsNullOrEmpty(qObra))
                    {
                        if (qObra != "*")
                        {
                            var array = qObra.Split(',');
                            List<int?> listaObra = new List<int?>();
                            foreach (var item in array)
                            {
                                listaObra.Add(Convert.ToInt32(item));
                            }
                            query = query.Where(x => listaObra.Contains(x.idObra));
                        }
                    }
                    if (idOrganismo != 0)
                    {
                        query = query.Where(x => x.idOrganismo == idOrganismo);
                    }
                    if (!string.IsNullOrEmpty(nombreObra))
                    {
                        query = query.Where(x => x.nombreObra.Contains(nombreObra));
                    }
                    if (!string.IsNullOrEmpty(nroExpediente))
                    {
                        query = query.Where(x => x.nroExpediente.Contains(nroExpediente));
                    }
                    if (fechaPub.HasValue)
                    {
                        query = query.Where(x => x.fechaPublicacion == fechaPub);
                    }
                    if (nroObra.HasValue)
                    {
                        if (nroObra != 0)
                        {
                            query = query.Where(x => x.idObra == nroObra);
                        }
                    }
                    var oficina = System.Web.HttpContext.Current.Session["idOficina"].ToString();
                    if (Convert.ToInt32(oficina) > 1)
                    {
                        var usuario = System.Web.HttpContext.Current.Session["idUsuario"].ToString();
                        query = ProyectFilterByOffice(Convert.ToInt32(oficina), Convert.ToInt32(usuario), query);
                    }
                        tarjeta.qAPublicar = query.Where(x => x.idEtapa == 5).Count();
                        tarjeta.qPublicada = query.Where(x => x.idEtapa == 6).Count();
                        tarjeta.qAbierta = query.Where(x => x.idEtapa == 7).Count();
                        tarjeta.qFirmada = query.Where(x => x.idEtapa == 8).Count();
                        tarjeta.qAdjudicada = query.Where(x => x.idEtapa == 14).Count();
                        tarjeta.qPreAdjudicada = query.Where(x => x.idEtapa == 15).Count();
                        tarjeta.qFracasada = query.Where(x => x.idEtapa == 16).Count();
                        tarjeta.qDesierta = query.Where(x => x.idEtapa == 17).Count();
                    
                                       
                }
            }
            catch (Exception ex)
            {
                log.Error("Error en Tarjeta Licitaciones", ex);
            }
            return tarjeta;
        }
        public unLicitacionViewModels buscarUna(int? idObra)
        {
            log4net.Config.XmlConfigurator.Configure(new FileInfo(ruta));
            log.Info("buscar Una Licitacion");
            unLicitacionViewModels unLicitacion = new unLicitacionViewModels();
            try
            {
                using (db_meieEntities db = new db_meieEntities())
                {
                    var query = from lic in db.PryProyecto
                                join edo in db.PryEstadoEtapa
                                on lic.GrlTypeState_Id equals edo.Id

                                join dp in db.GrlDepartament
                                on lic.GrlDepartament_Id equals dp.Id
                                    into dpo
                                from dpj in dpo.DefaultIfEmpty()

                                join pyl in db.PryLicitacion
                                on lic.Id equals pyl.IdPryProyecto
                                    into pylo
                                from pyj in pylo.DefaultIfEmpty()

                                join ff in db.PryFinanciacion
                                    on lic.PryFinanciacion_Id equals ff.IdPryFinanciacion
                                        into ffo
                                from ffj in ffo.DefaultIfEmpty()

                                join org in db.PryOrganismosEjecutores
                                on lic.PryOrganismoEjecutor_Id equals org.Id
                                    into orgo
                                from orgj in orgo.DefaultIfEmpty()

                                join enti in db.UfiOrgano
                                on lic.ufiOrganoReceptor_Id equals enti.idUfiOrgano
                                    into entio
                                from entij in entio.DefaultIfEmpty()

                                join licpry in db.LicProyecto
                                on lic.Id equals licpry.idProyecto                                 
                                   into licpryo
                                from licpryj in licpryo.DefaultIfEmpty()
                                where lic.Id == idObra && lic.Eliminado == false
                                select new unLicitacionViewModels
                                {
                                    nombreEtapa = edo.Name,
                                    idEtapa = edo.Id,
                                    nombreObra = lic.Nombre,
                                    idObra = lic.Id,
                                    fechaApertura = pyj.FechaApertura,
                                    fechaPublicacion = pyj.FechaPublicacionDesde,
                                    montoObra = pyj.MontoOficial,
                                    nroExpediente = lic.Expediente,
                                    nombreOrganismo = orgj.NombreOrganismo,
                                    idOrganismo = lic.PryOrganismoEjecutor_Id,
                                    caratula = lic.CaratulaExpediente,
                                    descripcion = lic.descripcion,
                                    plazo = pyj.Plazo,
                                    latitud = lic.Latitud,
                                    longitud = lic.Longitud,
                                    domicilio = lic.Dirección,
                                    valorPliego = pyj.ValorPliego,
                                    horaApertura = pyj.Hora,
                                    idTipoContratacion = pyj.TipoDeContratacion_Id,
                                    idMoneda = licpryj.moneda,
                                    domicilioApertura = licpryj.domicilioApertura,
                                    domicilioPresentacion = licpryj.domicilioPresentacion,
                                    urlPliego = licpryj.urlPliego,
                                    mailConsulta = licpryj.mailConsulta,
                                    fechaCierre = licpryj.fechaCierre,
                                    lugarVisita = licpryj.lugarVisita,
                                    fechaVisita = licpryj.fechaVisita,
                                    fechaConsulta = licpryj.limiteConsulta,
                                    departamento = dpj.Name,
                                    idFinanciamiento = lic.PryFinanciacion_Id,
                                    nombreFuenteFinanciamiento = ffj.nombrePryFinanciacion,
                                    idTipoLicitacion = pyj.Id,
                                    unidadPlazo = pyj.TipoPlazo,
                                    rutaVirtual= licpryj.domicilioVirtual,
                                    textoFinanciamiento = lic.financiamiento,
                                    nroLicitacion = lic.numeroLicitacion,
                                    fechaVtoObs = licpryj.fechaVtoObs,
                                    horaVtoObs = licpryj.horaVtoObs,
                                    entidadReceptora=entij.nombreOrgano
                                };
                    unLicitacion = query.FirstOrDefault();
                    var listaFecha = buscarListaFechas(idObra);
                    if ((listaFecha != null)&&(listaFecha.Count > 0)){
                        var unFecha = listaFecha.OrderByDescending(x => x.idDetalle).FirstOrDefault();
                        if (unFecha.fechaApertura.HasValue)
                        {
                            unLicitacion.fechaApertura = unFecha.fechaApertura;
                        }
                        if (unFecha.fechaCierre.HasValue)
                        {
                            unLicitacion.fechaCierre = unFecha.fechaCierre;
                        }
                        if (unFecha.fechaConsulta.HasValue)
                        {
                            unLicitacion.fechaConsulta = unFecha.fechaConsulta;
                        }
                        if (unLicitacion.fechaPublicacion.HasValue)
                        {
                            unLicitacion.fechaPublicacion = unFecha.fechaPublicacion;
                        }
                        if (!string.IsNullOrEmpty(unFecha.horaApertura))
                        {
                            unLicitacion.horaApertura = unFecha.horaApertura;
                        }                        
                    }
                }
            }
            catch (Exception ex)
            {
                log.Error("Error buscar Una Licitacion", ex);
            }
            if(unLicitacion != null) 
            {
                if(unLicitacion.idMoneda == 1)
                {
                    unLicitacion.nombreMoneda = "ARG";
                }
                else
                {
                    if (unLicitacion.idMoneda == 2)
                    {
                        unLicitacion.nombreMoneda = "USD";
                    }
                }               
            }
            return unLicitacion;
        }
        public async Task<int> grabarDetalleLicitacion(unLicitacionViewModels unLicitacion)
        {
            int value = 0;
            unLicitacion = validarDetalleLicitacion(unLicitacion);
            try
            {
                using (db_meieEntities db = new db_meieEntities())
                {
                    var existe = db.LicProyecto.Where(x => x.idProyecto == unLicitacion.idObra).FirstOrDefault();

                    if (existe != null)
                    {
                        existe.domicilioApertura = unLicitacion.domicilioApertura;
                        existe.domicilioPresentacion = unLicitacion.domicilioPresentacion;
                        existe.limiteConsulta = unLicitacion.fechaConsulta;
                        existe.idProyecto = unLicitacion.idObra.Value;
                        existe.mailConsulta = unLicitacion.mailConsulta;
                        existe.urlPliego = unLicitacion.urlPliego;
                        existe.moneda = unLicitacion.idMoneda;
                        existe.lugarVisita = unLicitacion.lugarVisita;
                        existe.fechaCierre = unLicitacion.fechaCierre;
                        existe.fechaVisita = unLicitacion.fechaVisita;
                        existe.fechaModif = DateTime.Now;
                        existe.domicilioVirtual = unLicitacion.rutaVirtual;
                        await db.SaveChangesAsync();
                    }
                    else
                    {
                        LicProyecto licProyecto = new LicProyecto();
                        licProyecto.domicilioApertura = unLicitacion.domicilioApertura;
                        licProyecto.domicilioPresentacion = unLicitacion.domicilioPresentacion;
                        licProyecto.limiteConsulta = unLicitacion.fechaConsulta;
                        licProyecto.idProyecto = unLicitacion.idObra.Value;
                        licProyecto.mailConsulta = unLicitacion.mailConsulta;
                        licProyecto.urlPliego = unLicitacion.urlPliego;
                        licProyecto.moneda = unLicitacion.idMoneda;
                        licProyecto.lugarVisita = unLicitacion.lugarVisita;
                        licProyecto.fechaCierre = unLicitacion.fechaCierre;
                        licProyecto.fechaVisita = unLicitacion.fechaVisita;
                        licProyecto.fechaModif = DateTime.Now;
                        licProyecto.domicilioVirtual = unLicitacion.rutaVirtual;
                        db.LicProyecto.Add(licProyecto);
                        await db.SaveChangesAsync();
                    }
                }
            }
            catch (Exception ex)
            {
                log.Error("Error grabar Detalle Licitacion", ex);
            }
            return value;
        }
        private unLicitacionViewModels validarDetalleLicitacion(unLicitacionViewModels unLicitacion)
        {
            try
            {
                if (string.IsNullOrEmpty(unLicitacion.domicilioApertura))
                {
                    unLicitacion.domicilioApertura = string.Empty;
                }
                if (string.IsNullOrEmpty(unLicitacion.domicilioPresentacion))
                {
                    unLicitacion.domicilioPresentacion = string.Empty;
                }
                if (string.IsNullOrEmpty(unLicitacion.mailConsulta))
                {
                    unLicitacion.mailConsulta = string.Empty;
                }
            }
            catch (Exception ex)
            {
                log.Error("Error validar Detalle Licitacion", ex);
            }
            return unLicitacion;
        }
        public async Task<int> grabarDetalleProyecto(unLicitacionViewModels unLicitacion)
        {
            int value = 0;
            try
            {
                using (db_meieEntities db = new db_meieEntities())
                {
                    var existe = db.PryProyecto.Where(x => x.Id == unLicitacion.idObra).FirstOrDefault();

                    if (existe != null)
                    {
                        //existe.GrlTypeState_Id = unLicitacion.idEtapa;
                        existe.Plazo = unLicitacion.unidadPlazo;
                        existe.Dias = unLicitacion.plazo;
                        existe.financiamiento = unLicitacion.textoFinanciamiento;
                        existe.numeroLicitacion = unLicitacion.nroLicitacion;
                        await db.SaveChangesAsync();
                    }
                    var existe2 = db.PryLicitacion.Where(x => x.IdPryProyecto == unLicitacion.idObra).FirstOrDefault();

                    if (existe2 != null)
                    {
                        existe2.Plazo = unLicitacion.plazo;
                        existe2.TipoPlazo = unLicitacion.unidadPlazo;
                        await db.SaveChangesAsync();
                    }
                }
            }
            catch (Exception ex)
            {
                log.Error("Error grabar Detalle Proyecto", ex);
            }
            return value;
        }
        public async Task<int> grabarLicitacion(unLicitacionViewModels unLicitacion)
        {
            log4net.Config.XmlConfigurator.Configure(new FileInfo(ruta));
            log.Info("Modifica una Licitacion ");
            int value = 0;
            unLicitacion = validarDetalleLicitacion(unLicitacion);
            try
            {
                using (db_meieEntities db = new db_meieEntities())
                {
                    var existe = db.PryLicitacion.Where(x => x.IdPryProyecto == unLicitacion.idObra).FirstOrDefault();

                    if (existe != null)
                    {
                        existe.MontoOficial = unLicitacion.montoObra;
                        existe.FechaApertura = unLicitacion.fechaApertura;
                        existe.Hora = unLicitacion.horaApertura;
                        existe.Plazo = unLicitacion.plazo;
                        existe.ValorPliego = unLicitacion.valorPliego;
                        existe.FechaPublicacionDesde = unLicitacion.fechaPublicacion;
                        existe.TipoDeContratacion_Id = unLicitacion.idTipoContratacion;
                        await db.SaveChangesAsync();
                    }
                    else
                    {
                        PryLicitacion pry = new PryLicitacion();
                        pry.MontoOficial = unLicitacion.montoObra;
                        pry.FechaApertura = unLicitacion.fechaApertura;
                        pry.Hora = unLicitacion.horaApertura;
                        pry.FechaPublicacionDesde = unLicitacion.fechaPublicacion;
                        pry.TipoDeContratacion_Id = unLicitacion.idTipoContratacion;
                        pry.Plazo = unLicitacion.plazo;
                        pry.ValorPliego = unLicitacion.valorPliego;
                        db.PryLicitacion.Add(pry);
                        await db.SaveChangesAsync();
                    }

                    var existe2 = db.PryProyectoPlanificacion.Where(x => x.PryProyecto_Id == unLicitacion.idObra).FirstOrDefault();
                    if(existe2 != null)
                    {
                        existe2.MontoOficial = unLicitacion.montoObra;
                        //existe2.MontoContratado = unLicitacion.montoObra;
                        await db.SaveChangesAsync();
                    }
                    //var existe3 = db.PryProyectoExpedientes
                    //    .Where(x => x.idPryProyecto == unLicitacion.idObra && x.nroExpediente == unLicitacion.nroExpediente && unLicitacion.nroExpediente == unLicitacion.nroExpediente && x.caratulaExpediente == unLicitacion.caratula)
                    //    .FirstOrDefault();
                    //if(existe3 == null)
                    //{
                    //    PryProyectoExpediente proyectoExpediente = new PryProyectoExpediente();
                    //    proyectoExpediente.idPryProyecto = unLicitacion.idObra.Value;
                    //    proyectoExpediente.idPryTipoLicitacion = 2;
                    //    proyectoExpediente.nroExpediente = unLicitacion.nroExpediente;
                    //    proyectoExpediente.caratulaExpediente = unLicitacion.caratula;
                    //    db.PryProyectoExpedientes.Add(proyectoExpediente);
                    //    await db.SaveChangesAsync();
                    //}
                    #region auditoria
                    AuditoriaOficina aud = new AuditoriaOficina();
                    var usrID = Convert.ToInt32(System.Web.HttpContext.Current.Session["idUsuario"].ToString());

                    var nombreUsuario = db.SegUser.Where(x => x.Id == usrID).FirstOrDefault();
                    aud.AddLogCambios(2, 1, "El usuario " + nombreUsuario.FullName + " ha generado Modificación una Licitacion para la obra " + unLicitacion.idObra +"- Búho Licitaciones", usrID, unLicitacion.idObra);

                    #endregion
                }
            }
            catch (Exception ex)
            {
                using (db_meieEntities db = new db_meieEntities())
                {
                    #region auditoria
                    AuditoriaOficina aud = new AuditoriaOficina();
                    var usrID = Convert.ToInt32(System.Web.HttpContext.Current.Session["idUsuario"].ToString());

                    var nombreUsuario = db.SegUser.Where(x => x.Id == usrID).FirstOrDefault();
                    aud.AddLogCambios(2, 1, "El usuario " + nombreUsuario.FullName + " ha generado un error en Modificación una Licitacion para la obra " + unLicitacion.idObra + "- Búho Licitaciones", usrID, unLicitacion.idObra);

                    #endregion
                }
                log.Error("Error grabar Licitacion", ex);
            }
            return value;
        }
        public async Task<int> grabarFechas(fechaViewModels fechaLicitacion)
        {
            log4net.Config.XmlConfigurator.Configure(new FileInfo(ruta));
            log.Info("Modifica una fecha de la licitacion ");
            int value = 0;
            try
            {
                using (db_meieEntities db = new db_meieEntities())
                {
                    var existe = db.LicProyectoFecha
                        .Where(x => x.idProyecto == fechaLicitacion.idObra && x.fechaApertura == fechaLicitacion.fechaApertura && x.fechaCierre == fechaLicitacion.fechaCierre && x.fechaPublicacion == fechaLicitacion.fechaPublicacion && x.horaApertura == fechaLicitacion.horaApertura && x.esFracasada == null)
                        .FirstOrDefault();
                    if(existe == null)
                    {
                        LicProyectoFecha proyectoFecha = new LicProyectoFecha();
                        proyectoFecha.fechaApertura = fechaLicitacion.fechaApertura;
                        proyectoFecha.fechaCierre = fechaLicitacion.fechaCierre;
                        proyectoFecha.fechaPublicacion = fechaLicitacion.fechaPublicacion;
                        proyectoFecha.horaApertura = fechaLicitacion.horaApertura;
                        proyectoFecha.idProyecto = fechaLicitacion.idObra;
                        db.LicProyectoFecha.Add(proyectoFecha);
                        await db.SaveChangesAsync();
                    }
                    #region auditoria
                    AuditoriaOficina aud = new AuditoriaOficina();
                    var usrID = Convert.ToInt32(System.Web.HttpContext.Current.Session["idUsuario"].ToString());

                    var nombreUsuario = db.SegUser.Where(x => x.Id == usrID).FirstOrDefault();
                    aud.AddLogCambios(2, 1, "El usuario " + nombreUsuario.FullName + " ha generado Modificación en las fechas de una Licitacion para la obra " + fechaLicitacion.idObra + "- Búho Licitaciones", usrID, fechaLicitacion.idObra);

                    #endregion
                }
            }
            catch (Exception ex)
            {
                using (db_meieEntities db = new db_meieEntities())
                {
                    #region auditoria
                    AuditoriaOficina aud = new AuditoriaOficina();
                    var usrID = Convert.ToInt32(System.Web.HttpContext.Current.Session["idUsuario"].ToString());

                    var nombreUsuario = db.SegUser.Where(x => x.Id == usrID).FirstOrDefault();
                    aud.AddLogCambios(2, 1, "El usuario " + nombreUsuario.FullName + " ha generado un error en Modificación de Fechas de una Licitacion para la obra " + fechaLicitacion.idObra + "- Búho Licitaciones", usrID, fechaLicitacion.idObra);

                    #endregion
                }
                log.Error("Error grabar Fechas Licitacion", ex);
            }
            return value;
        }
        public async Task<int> grabarEspecialidad(especialidadViewModels unEspecialidad)
        {
            log4net.Config.XmlConfigurator.Configure(new FileInfo(ruta));
            log.Info("Grabar Especialidad");
            int value = 0;
            try
            {
                using (db_meieEntities db = new db_meieEntities())
                {
                    List<int> lista = new List<int>();
                    foreach(var item in unEspecialidad.listaEspecialidad)
                    {
                        if (!string.IsNullOrEmpty(item))
                        {
                            var id = Convert.ToInt32(item);
                            lista.Add(id);
                        }
                    }
                    
                    if(lista.Count > 0)
                    {
                        var listaEspecialidad = db.PryProyectoEspecialidad.Where(x => x.idPryProyecto == unEspecialidad.idObra).ToList();
                        foreach (var item in listaEspecialidad)
                        {
                            db.PryProyectoEspecialidad.Remove(item);
                            db.SaveChanges();
                        }
                        foreach(var item in lista)
                        {
                            PryProyectoEspecialidad proyectoEspecialidad = new PryProyectoEspecialidad();
                            proyectoEspecialidad.idPryProyecto = unEspecialidad.idObra.Value;
                            proyectoEspecialidad.idPryEspecialidad = item;
                            proyectoEspecialidad.porcentaje100to400 = unEspecialidad.porcentaje100;
                            proyectoEspecialidad.porcentaje500 = unEspecialidad.porcentaje400;
                            db.PryProyectoEspecialidad.Add(proyectoEspecialidad);
                            await db.SaveChangesAsync();
                        }
                    }
                    #region auditoria
                    AuditoriaOficina aud = new AuditoriaOficina();
                    var usrID = Convert.ToInt32(System.Web.HttpContext.Current.Session["idUsuario"].ToString());

                    var nombreUsuario = db.SegUser.Where(x => x.Id == usrID).FirstOrDefault();
                    aud.AddLogCambios(2, 1, "El usuario " + nombreUsuario.FullName + " ha realizado una Grabación de Especialidad para la obra " + unEspecialidad.idObra + "- Búho Licitaciones", usrID, unEspecialidad.idObra);

                    #endregion
                }
            }
            catch (Exception ex)
            {
                using (db_meieEntities db = new db_meieEntities())
                {
                    #region auditoria
                    AuditoriaOficina aud = new AuditoriaOficina();
                    var usrID = Convert.ToInt32(System.Web.HttpContext.Current.Session["idUsuario"].ToString());

                    var nombreUsuario = db.SegUser.Where(x => x.Id == usrID).FirstOrDefault();
                    aud.AddLogCambios(2, 1, "El usuario " + nombreUsuario.FullName + " ha generado un error al realizar una Grabación de Especialidad para la obra " + unEspecialidad.idObra + "- Búho Licitaciones", usrID, unEspecialidad.idObra);

                    #endregion
                }
                log.Error("Error grabar Especialidad", ex);
            }
            return value;
        }
        public List<fechaViewModels> buscarListaFechas(int? idObra)
        {
            log4net.Config.XmlConfigurator.Configure(new FileInfo(ruta));
            log.Info("buscar Lista de Fechas");
            List<fechaViewModels> lista = new List<fechaViewModels>();
            try
            {
                using (db_meieEntities db = new db_meieEntities())
                {
                    var query = from lic in db.LicProyectoFecha
                                where lic.idProyecto == idObra && lic.esFracasada == null
                                select new fechaViewModels
                                {
                                    idObra = lic.idProyecto,
                                    fechaApertura = lic.fechaApertura,
                                    fechaPublicacion = lic.fechaPublicacion,
                                    horaApertura = lic.horaApertura,
                                    fechaCierre = lic.fechaCierre,
                                    idDetalle = lic.idLicProyectoFecha
                                };
                    lista = query.OrderByDescending(x => x.idDetalle).ToList();
                }
            }
            catch (Exception ex)
            {
                log.Error("Error buscar Lista de Fechas", ex);
            }
            return lista;
        }
        public List<licitacionGrillaViewModels> listarLicitacionesReportes
        (ref int recordsTotal, string sortColumn, string sortColumnDir, string searchValue,
            string nroExpediente, string nombreObra,string qObra, int? idEstado, DateTime? fechaPub, int? idOrganismo, int? nroObra, bool? obrasEjecucion,
            int pageSize = 0, int skip = 0)
        {
            var idObras = "";
            log4net.Config.XmlConfigurator.Configure(new FileInfo(ruta));
            log.Info("Grilla Licitaciones Administrador");
           
                var listaFiltrada = new List<licitacionGrillaViewModels>();
            List<licitacionGrillaViewModels> lista = new List<licitacionGrillaViewModels>();
            try
            {
                using (db_meieEntities db = new db_meieEntities())
                {

                    var query = from lic in db.PryProyecto
                                join edo in db.PryEstadoEtapa
                                on lic.GrlTypeState_Id equals edo.Id
                                join pyl in db.PryLicitacion
                                on lic.Id equals pyl.IdPryProyecto
                                    into pylo
                                from pyj in pylo.DefaultIfEmpty()
                                join org in db.PryOrganismosEjecutores
                                on lic.PryOrganismoEjecutor_Id equals org.Id
                                    into orgo
                                from orgj in orgo.DefaultIfEmpty()
                                join re in db.UfiOrgano
                                on lic.ufiOrganoReceptor_Id equals re.idUfiOrgano
                                join fi in db.PryFinanciacion                                    
                                on lic.PryFinanciacion_Id equals fi.IdPryFinanciacion   
                                join de in db.GrlDepartament
                                on lic.GrlDepartament_Id equals de.Id
                                where ((obrasEjecucion == true && edo.Id == 1) || edo.PryStage_Id == 49) && lic.Eliminado == false                                
                                select new licitacionGrillaViewModels
                                {
                                    nombreEtapa = edo.Name,
                                    idEtapa = edo.Id,
                                    nombreObra = lic.Nombre,
                                    idObra = lic.Id,                                    
                                    fechaPublicacion = pyj.FechaPublicacionDesde,
                                    fechaApertura = pyj.FechaApertura,
                                    montoObra = pyj.MontoOficial,
                                    nroExpediente = lic.Expediente,
                                    nombreOrganismo = orgj.NombreOrganismo,
                                    idOrganismo = lic.PryOrganismoEjecutor_Id,
                                    entidadReceptora = re.nombreOrgano,
                                    fechaAdjudicacionString = lic.fechaNormaAdjudicacion,
                                    tipoFinanciamiento = fi.nombrePryFinanciacion,
                                    plazo = pyj.Plazo,
                                    departamento = de.Name,                                    

                                };

                    if (!string.IsNullOrEmpty(qObra))
                    {
                        if (qObra != "*")
                        {
                            var array = qObra.Split(',');
                            List<int?> listaObra = new List<int?>();
                            foreach (var item in array)
                            {
                                listaObra.Add(Convert.ToInt32(item));
                            }
                            query = query.Where(x => listaObra.Contains(x.idObra));
                        }
                    }
                    if (idOrganismo != 0)
                    {
                        query = query.Where(x => x.idOrganismo == idOrganismo);
                    }
                    if (idEstado != 0)
                    {
                        query = query.Where(x => x.idEtapa == idEstado);
                    }
                    if (!string.IsNullOrEmpty(nombreObra))
                    {
                        query = query.Where(x => x.nombreObra.Contains(nombreObra));
                    }
                    if (nroObra.HasValue)
                    {
                        if (nroObra != 0)
                        {
                            query = query.Where(x => x.idObra == nroObra);
                        }
                    }
                    if (!string.IsNullOrEmpty(nroExpediente))
                    {
                        query = query.Where(x => x.nroExpediente.Contains(nroExpediente));
                    }
                    if (fechaPub.HasValue)
                    {
                        query = query.Where(x => x.fechaPublicacion == fechaPub);
                    }
                    if (!(string.IsNullOrEmpty(sortColumn) && string.IsNullOrEmpty(sortColumnDir)))
                    {
                        if ((sortColumn == "nombreObra") && (sortColumnDir == "desc"))
                        {
                            query = query.OrderByDescending(x => x.nombreObra);
                        }
                        if ((sortColumn == "nombreObra") && (sortColumnDir == "asc"))
                        {
                            query = query.OrderBy(x => x.nombreObra);
                        }
                        if ((sortColumn == "nroExpedienteString") && (sortColumnDir == "desc"))
                        {
                            query = query.OrderByDescending(x => x.nroExpediente);
                        }
                        if ((sortColumn == "nroExpedienteString") && (sortColumnDir == "asc"))
                        {
                            query = query.OrderBy(x => x.nroExpediente);
                        }
                        if ((sortColumn == "fechaPublicacionString") && (sortColumnDir == "desc"))
                        {
                            query = query.OrderByDescending(x => x.fechaPublicacion);
                        }
                        if ((sortColumn == "fechaPublicacionString") && (sortColumnDir == "asc"))
                        {
                            query = query.OrderBy(x => x.fechaPublicacion);
                        }
                        if ((sortColumn == "nombreOrganismoString") && (sortColumnDir == "asc"))
                        {
                            query = query.OrderBy(x => x.nombreOrganismo);
                        }
                        if ((sortColumn == "nombreOrganismoString") && (sortColumnDir == "desc"))
                        {
                            query = query.OrderByDescending(x => x.nombreOrganismo);
                        }
                        if ((sortColumn == "montoObraString") && (sortColumnDir == "asc"))
                        {
                            query = query.OrderBy(x => x.montoObra);
                        }
                        if ((sortColumn == "montoObraString") && (sortColumnDir == "desc"))
                        {
                            query = query.OrderByDescending(x => x.montoObra);
                        }
                        if ((sortColumn == "nombreEtapa") && (sortColumnDir == "asc"))
                        {
                            query = query.OrderBy(x => x.nombreEtapa);
                        }
                        if ((sortColumn == "nombreEtapa") && (sortColumnDir == "desc"))
                        {
                            query = query.OrderByDescending(x => x.nombreEtapa);
                        }
                        if ((sortColumn == "accion") && (sortColumnDir == "asc"))
                        {
                            query = query.OrderBy(x => x.nombreObra);
                        }
                        if ((sortColumn == "accion") && (sortColumnDir == "desc"))
                        {
                            query = query.OrderByDescending(x => x.nombreObra);
                        }

                    }
                    else
                    {
                        query = query.OrderBy(x => x.nombreObra);
                    }                   
                    
                    listaFiltrada = query.ToList();
                    //agrego empresas presentadas
                    foreach (var item in listaFiltrada)
                    {
                        List<string> nombresEmpresas = new List<string>();
                        var empresasQuery = from of in db.LicOfertaEmpresa
                                            where of.idObra == item.idObra && of.activa == 1 && of.esFracasada == null
                                            select new licitacionGrillaViewModels
                                            {
                                                idEmpresaPresentada = of.idEmpresa,
                                            };

                        var empresas = empresasQuery.ToList();

                        using (DB_RACOPEntities _db = new DB_RACOPEntities())
                        {
                            foreach (var item2 in empresas)
                            {
                                var nombreEmpresa = _db.rc_Empresa
                                                    .Where(x => x.idEmpresa == item2.idEmpresaPresentada)
                                                    .Select(x => x.razonSocial)
                                                    .FirstOrDefault();

                                if (nombreEmpresa != null)
                                {
                                    nombresEmpresas.Add(nombreEmpresa);
                                }
                            }
                        }

                        item.empresasPresentadas = string.Join(", ", nombresEmpresas);
                    }

                    return listaFiltrada;
                }
                
            }
           
            catch (Exception ex)
            {
                log.Error("Error en Grilla Licitaciones Administrador", ex);
            }

            return listaFiltrada;
            
        }
       
        public ReporteExcelVM generarExcelGrilla(List<licitacionGrillaViewModels> obras, bool? nombre, bool? expediente, bool? publicacion, bool? organismo, 
            bool? monto, bool? etapa, bool? fechaApertura, bool? fechaAdjudicacion, bool? entidadReceptora, bool? departamento, bool? tipoFinanciamiento, bool? plazoMeses, bool? empresasPresentadas)
        {
            ReporteExcelVM excel = new ReporteExcelVM();
            excel.filas = new List<DetalleExcelVM>();
           
            using (db_meieEntities db = new db_meieEntities())
            {
                foreach (var item in obras)
                {
                    excel.encabezado = new List<string>();                    
                    if (obras != null)
                    {                        
                        DetalleExcelVM detalle = new DetalleExcelVM();
                        excel.encabezado.Add("Grilla Obras");   
                        detalle.nombre = item.nombreObra;
                        detalle.expediente = item.nroExpedienteString;
                        detalle.publicacion = item.fechaPublicacionString;   
                        detalle.organismo = item.nombreOrganismo;
                        detalle.monto = item.montoObraString;                
                        detalle.etapa = item.nombreEtapa;
                        detalle.fechaApertura = item.fechaAperturaString;
                        detalle.fechaAdjudicacion = item.fechaAdjudicacionString;                       
                        detalle.entidadReceptora = item.entidadReceptora;
                        detalle.tipoFinanciamiento = item.tipoFinanciamiento;
                        detalle.departamento = item.departamento;
                        detalle.plazoMeses = Convert.ToString(item.plazo / 30);
                        detalle.empresasPresentadas = item.empresasPresentadas;
                        
                        
                        excel.filas.Add(detalle);
                    }
                }
            }
            ExcelUtility excelUtility = new ExcelUtility();
            excel = excelUtility.GenerarExcelGrilla(excel, nombre, expediente, publicacion, organismo, monto, etapa,
                fechaApertura, fechaAdjudicacion, entidadReceptora, departamento, tipoFinanciamiento, plazoMeses, empresasPresentadas);
            return excel;
        }
        public IQueryable<licitacionGrillaViewModels> ProyectFilterByOffice(int currentIdOffice, int currentIdUser, IQueryable<licitacionGrillaViewModels> listaDatos)
        {
            
            var oficinaUsuario = new SegUserVewModels();
            using (var userData = new db_meieEntities())
            {
                oficinaUsuario = (from u in userData.SegUser
                                  where u.Id == currentIdUser
                                  select new SegUserVewModels
                                  {
                                      idOffice = (int)u.SegOffice_Id,
                                      segvisualiza_id = (int)u.SegVisualiza_Id

                                  }).FirstOrDefault();
            }
            var oficina = new SegUserVewModels();
            using (var userData = new db_meieEntities())
            {
                oficina = (from o in userData.SegOffice
                           where o.Id == currentIdOffice
                           select new SegUserVewModels
                           {
                               idVM = o.Id,
                               idOfficePad = (int)o.idOfficePadre
                           }).FirstOrDefault();
            }
            var organo = new SegUserVewModels();
            using (var userData = new db_meieEntities())
            {
                organo = (from o in userData.UfiOrgano
                          where o.idUfiOrgano == oficina.idOfficePad
                          select new SegUserVewModels
                          {
                              idUfiOrg = o.idUfiOrgano
                          }).FirstOrDefault();
            }

            //
            var hijos = new List<SegUserVewModels>();
            var cargarHijos = new List<SegUserVewModels>();
            using (var userData = new db_meieEntities())
            {
                hijos = (from o in userData.SegOffice
                             //where o.Eliminado == 0
                         select new SegUserVewModels
                         {
                             idVM = o.Id,

                             idOfficePad = o.idOfficePadre ?? 0
                         }).ToList();
            }
            int?[] num1 = new int?[100000];
            int i = 0;
            foreach (var item in hijos)
            {
                if (item.idOfficePad == oficina.idOfficePad)
                {
                    num1[i++] = item.idVM;
                    cargarHijos.Add(item);
                }
            }

            //
            var cargar = new List<SegUserVewModels>();
            using (var userData = new db_meieEntities())
            {
                cargar = (from o in userData.SegOffice
                          where o.idOfficePadre == oficina.idOfficePad
                          select new SegUserVewModels
                          {
                              idVM = o.Id,

                              idOfficePad = o.idOfficePadre ?? 0
                          }).ToList();
            }
           
            if (oficinaUsuario.segvisualiza_id == 1)
            {
                listaDatos = listaDatos.Where(x => x.Eliminado == false);

            }
            else
            if (oficinaUsuario.segvisualiza_id == 2)
            {
                listaDatos = listaDatos.Where(x => x.Eliminado == false
                         && (x.idOffice == currentIdOffice || x.idOrgano == oficina.idOfficePad || x.idOrganoReceptor == oficina.idOfficePad));
            }
            else if (oficinaUsuario.segvisualiza_id == 3)
            {
                listaDatos = listaDatos.Where(x => x.Eliminado == false &&
                 (x.idOrgano == oficina.idOfficePad || x.idOrganoReceptor == oficina.idOfficePad
                 || (x.idOffice == currentIdOffice && x.fechaPublicacion != null)));

            }
            //var s = listaDatos.ToList();
                return listaDatos;
        }
        public int cambiarEtapa(int? idObra, int? idEtapa)
        {
            var value = 0;
            try
            {
                if (validarCambiarEtapa(idObra, idEtapa))
                {
                    using (db_meieEntities db = new db_meieEntities())
                    {
                        var existe = db.PryProyecto.Where(x => x.Id == idObra).FirstOrDefault();

                        if (existe != null)
                        {
                            existe.GrlTypeState_Id = idEtapa;
                            db.SaveChanges();
                            value = 1;                           
                        }
                        #region auditoria
                        AuditoriaOficina aud = new AuditoriaOficina();
                        var usrID = Convert.ToInt32(System.Web.HttpContext.Current.Session["idUsuario"].ToString());

                        var nombreUsuario = db.SegUser.Where(x => x.Id == usrID).FirstOrDefault();
                        aud.AddLogCambios(2, 1, "El usuario " + nombreUsuario.FullName + " ha cambiado la etapa para la obra " + idObra + "- Búho Licitaciones", usrID, idObra);

                        #endregion
                    }
                }
            }
            catch (Exception ex)
            {
                using (db_meieEntities db = new db_meieEntities())
                {
                    #region auditoria
                    AuditoriaOficina aud = new AuditoriaOficina();
                    var usrID = Convert.ToInt32(System.Web.HttpContext.Current.Session["idUsuario"].ToString());

                    var nombreUsuario = db.SegUser.Where(x => x.Id == usrID).FirstOrDefault();
                    aud.AddLogCambios(2, 1, "El usuario " + nombreUsuario.FullName + " ha generado un error al cambiar la etapa para la obra " + idObra + "- Búho Licitaciones", usrID, idObra);

                    #endregion
                }
            }
            return value;
        }
        public bool validarCambiarEtapa(int? idObra, int? idEtapa)
        {
            var value = false;
            try
            {
                using (db_meieEntities db = new db_meieEntities())
                {
                    var existe = db.LicArchivoCategoria.Where(x => x.idEtapa == idEtapa).FirstOrDefault();

                    if (existe != null)
                    {
                        var existeArchivo = db.LicArchivoObra.Where(x => x.idCategoria == existe.idCategoria && x.idObra == idObra && x.esFracasada == null).FirstOrDefault();
                        if (existeArchivo != null)
                        {
                            return true;
                        }
                    }
                    else
                    {
                        value = true;
                    }
                }
            }
            catch (Exception ex)
            {

            }
            return value;
        }
        public string buscarTipoContratacion(int? idTipoContratacion)
        {
            string value = string.Empty;
            try
            {
                using (db_meieEntities db= new db_meieEntities())
                {
                    var tmp = db.GrlType.Where(x => x.Id == idTipoContratacion).FirstOrDefault();
                    if(tmp != null)
                    {
                        value = tmp.Name;
                    }
                }
            }
            catch(Exception ex)
            {

            }
            return value;
        }
        public async Task<int> cambioEtapa(cambioEtapaViewModels unLicitacion, string rutaInicio)
        {
            int value = 1;
            try
            {
                using (db_meieEntities db = new db_meieEntities())
                {
                    var existe = db.PryProyecto.Where(x => x.Id == unLicitacion.idObra).FirstOrDefault();

                    if (existe != null)
                    {
                        var rutaDirectorio = rutaInicio + "\\" + unLicitacion.idObra.Value.ToString();
                        var unFecha = db.PryLicitacion.Where(x => x.IdPryProyecto == unLicitacion.idObra).FirstOrDefault();
                        var unFechaProyecto = db.LicProyectoFecha.Where(x => x.idProyecto == unLicitacion.idObra).OrderByDescending(x => x.idLicProyectoFecha).FirstOrDefault();
                        var requisitos = db.LicDocObra.Where(x => x.idObra == unLicitacion.idObra && x.esFracasada == null).FirstOrDefault();
                        if (unLicitacion.idEtapa == 7)
                        {

                            if (unFecha != null)
                            {
                                var fechaControl = DateTime.Now.Date;
                                var horaControl = DateTime.Now.TimeOfDay;
                                if (unFecha.FechaApertura < fechaControl)
                                {
                                    existe.GrlTypeState_Id = unLicitacion.idEtapa;
                                    await db.SaveChangesAsync();
                                    ServicioArchivo servicioArchivo = new ServicioArchivo();
                                    var nombreZip = unLicitacion.idObra.Value.ToString() + ".zip";
                                    servicioArchivo.desComprimirDirectorio(rutaDirectorio, rutaInicio, nombreZip, unLicitacion.idObra);
                                    await grabarHisotialEtapas(unLicitacion.idObra, unLicitacion.idEtapa);
                                }
                                else
                                {
                                    if ((unFecha.FechaApertura == fechaControl))
                                    {
                                        if (!string.IsNullOrEmpty(unFecha.Hora))
                                        {
                                            var horaApertura = TimeSpan.Parse(unFecha.Hora);
                                            if (horaApertura <= horaControl)
                                            {
                                                existe.GrlTypeState_Id = unLicitacion.idEtapa;
                                                await db.SaveChangesAsync();
                                                ServicioArchivo servicioArchivo = new ServicioArchivo();
                                                var nombreZip = unLicitacion.idObra.Value.ToString() + ".zip";
                                                servicioArchivo.desComprimirDirectorio(rutaDirectorio, rutaInicio, nombreZip, unLicitacion.idObra);
                                                await grabarHisotialEtapas(unLicitacion.idObra, unLicitacion.idEtapa);
                                            }
                                            else
                                            {
                                                return 0;
                                            }
                                        }
                                    }
                                    else
                                    {
                                        return 0;
                                    }
                                }
                            }
                        }
                        else
                        {
                            if (unLicitacion.idEtapa == 6)
                            {
                                if (unFecha != null)
                                {
                                    if (unFecha.FechaPublicacionDesde == null || unFecha.FechaApertura == null || string.IsNullOrEmpty(unFecha.Hora) || unFechaProyecto.fechaCierre == null)
                                    {
                                        return -1;
                                    }
                                    else
                                    {
                                        if (requisitos == null)
                                        {
                                            return -3;
                                        }
                                        else
                                        {
                                            existe.GrlTypeState_Id = unLicitacion.idEtapa;
                                            await db.SaveChangesAsync();
                                            await grabarHisotialEtapas(unLicitacion.idObra, unLicitacion.idEtapa);
                                        }                                            
                                    }
                                }
                            }
                            else
                            {
                                if (unLicitacion.idEtapa == 8 || unLicitacion.idEtapa == 1)
                                {
                                    if (tieneContrato(unLicitacion.idObra))
                                    {
                                        existe.GrlTypeState_Id = unLicitacion.idEtapa;
                                        await db.SaveChangesAsync();
                                        await grabarHisotialEtapas(unLicitacion.idObra, unLicitacion.idEtapa);
                                    }
                                    else
                                    {
                                        return -2;
                                    }
                                }
                                else
                                {

                                    if (unLicitacion.idEtapa == 15)
                                    {
                                        if (tieneActa(unLicitacion.idObra))
                                        {
                                            existe.GrlTypeState_Id = unLicitacion.idEtapa;
                                            await db.SaveChangesAsync();
                                            await grabarHisotialEtapas(unLicitacion.idObra, unLicitacion.idEtapa);
                                        }
                                        else
                                        {
                                            return -4;
                                        }
                                    }
                                    else
                                    {
                                        existe.GrlTypeState_Id = unLicitacion.idEtapa;
                                        await db.SaveChangesAsync();
                                        await grabarHisotialEtapas(unLicitacion.idObra, unLicitacion.idEtapa);
                                    }
                                }
                            }
                        }
                    }
                    #region auditoria
                    AuditoriaOficina aud = new AuditoriaOficina();
                    var usrID = Convert.ToInt32(System.Web.HttpContext.Current.Session["idUsuario"].ToString());

                    var nombreUsuario = db.SegUser.Where(x => x.Id == usrID).FirstOrDefault();
                    aud.AddLogCambios(2, 1, "El usuario " + nombreUsuario.FullName + " ha realizado una Grabación de cambio Etapa para la obra " + unLicitacion.idObra + "- Búho Licitaciones", usrID, unLicitacion.idObra);

                    #endregion
                }
            }
            catch (Exception ex)
            {
                using (db_meieEntities db = new db_meieEntities())
                {
                    #region auditoria
                    AuditoriaOficina aud = new AuditoriaOficina();
                    var usrID = Convert.ToInt32(System.Web.HttpContext.Current.Session["idUsuario"].ToString());

                    var nombreUsuario = db.SegUser.Where(x => x.Id == usrID).FirstOrDefault();
                    aud.AddLogCambios(2, 1, "El usuario " + nombreUsuario.FullName + " ha generado un error al realizar una Grabación de cambio Etapa para la obra " + unLicitacion.idObra + "- Búho Licitaciones", usrID, unLicitacion.idObra);

                    #endregion
                }
                log.Error("Error grabar cambioEtapa", ex);
            }
            return value;
        }
        public async Task<int> cambioEtapaAnterior(cambioEtapaViewModels unLicitacion)
        {
            int value = 1;
            try
            {
                using (db_meieEntities db = new db_meieEntities())
                {
                    var existe = db.PryProyecto.Where(x => x.Id == unLicitacion.idObra).FirstOrDefault();
                    int? idEtapaAnterior = 5;
                    if (existe != null)
                    {
                        if(unLicitacion.idEtapa == 6)
                        {
                            idEtapaAnterior = 5;
                            await grabarHisotialEtapas(unLicitacion.idObra, idEtapaAnterior);
                        }
                        if (unLicitacion.idEtapa == 7)
                        {
                            //var unFecha = db.PryLicitacion.Where(x => x.IdPryProyecto == unLicitacion.idObra).FirstOrDefault();
                            //if (unFecha != null)
                            //{
                            //    var fechaControl = DateTime.Now.Date;
                            //    var horaControl = DateTime.Now.TimeOfDay;
                            //    if (unFecha.FechaApertura <= fechaControl)
                            //    {
                            //        if (!string.IsNullOrEmpty(unFecha.Hora))
                            //        {
                            //            var horaApertura = TimeSpan.Parse(unFecha.Hora);
                            //            if (horaApertura <= horaControl)
                            //            {
                            //                idEtapaAnterior = 6;
                            //                grabarHisotialEtapas(unLicitacion.idObra, idEtapaAnterior);
                            //            }
                            //        }
                            //    }
                            //}
                            idEtapaAnterior = 6;
                            grabarHisotialEtapas(unLicitacion.idObra, idEtapaAnterior);
                        }
                        if (unLicitacion.idEtapa == 8)
                        {
                            idEtapaAnterior = 14;
                            grabarHisotialEtapas(unLicitacion.idObra, idEtapaAnterior);
                        }
                        if (unLicitacion.idEtapa == 14)
                        {
                            idEtapaAnterior = 15;
                            grabarHisotialEtapas(unLicitacion.idObra, idEtapaAnterior);
                        }
                        if (unLicitacion.idEtapa == 15)
                        {
                            idEtapaAnterior = 7;
                            grabarHisotialEtapas(unLicitacion.idObra, idEtapaAnterior);
                        }
                        if (unLicitacion.idEtapa == 16)
                        {
                            idEtapaAnterior = 5;
                            grabarHisotialEtapas(unLicitacion.idObra, idEtapaAnterior);
                        }
                        if (unLicitacion.idEtapa == 17)
                        {
                            idEtapaAnterior = 5;
                            grabarHisotialEtapas(unLicitacion.idObra, idEtapaAnterior);
                        }
                        existe.GrlTypeState_Id = idEtapaAnterior;
                        await db.SaveChangesAsync();                      
                    }
                    #region auditoria
                    AuditoriaOficina aud = new AuditoriaOficina();
                    var usrID = Convert.ToInt32(System.Web.HttpContext.Current.Session["idUsuario"].ToString());

                    var nombreUsuario = db.SegUser.Where(x => x.Id == usrID).FirstOrDefault();
                    aud.AddLogCambios(2, 1, "El usuario " + nombreUsuario.FullName + " ha realizado una Grabación de cambio Etapa para la obra " + unLicitacion.idObra + "- Búho Licitaciones", usrID, unLicitacion.idObra);

                    #endregion
                }
            }
            catch (Exception ex)
            {
                using (db_meieEntities db = new db_meieEntities())
                {
                    #region auditoria
                    AuditoriaOficina aud = new AuditoriaOficina();
                    var usrID = Convert.ToInt32(System.Web.HttpContext.Current.Session["idUsuario"].ToString());

                    var nombreUsuario = db.SegUser.Where(x => x.Id == usrID).FirstOrDefault();
                    aud.AddLogCambios(2, 1, "El usuario " + nombreUsuario.FullName + " ha generado un error al realizar una Grabación de cambio Etapa para la obra " + unLicitacion.idObra + "- Búho Licitaciones", usrID, unLicitacion.idObra);

                    #endregion
                }
                log.Error("Error grabar cambioEtapa", ex);
            }
            return value;
        }
        public async Task<int> grabarHisotialEtapas(int? idObra, int? idEtapa)
        {            
            try
            {
                using (db_meieEntities db = new db_meieEntities())
                {
                    var existe = db.PryProyecto.Where(x => x.Id == idObra).FirstOrDefault();
                    if(existe != null)
                    {
                        //agrego historial etapa
                        PryProyectoEstado etapa = new PryProyectoEstado();
                        etapa.PryProyecto_Id = idObra;
                        etapa.FechaModificación = DateTime.Now;
                        etapa.GrlTypeState_Id = idEtapa;
                        db.PryProyectoEstado.Add(etapa);
                        await db.SaveChangesAsync();
                    }
                    #region auditoria
                    AuditoriaOficina aud = new AuditoriaOficina();
                    var usrID = Convert.ToInt32(System.Web.HttpContext.Current.Session["idUsuario"].ToString());

                    var nombreUsuario = db.SegUser.Where(x => x.Id == usrID).FirstOrDefault();
                    aud.AddLogCambios(2, 1, "El usuario " + nombreUsuario.FullName + " ha generado Historial de Etapa para la obra " + idObra + "- Búho Licitaciones", usrID, idObra);

                    #endregion

                }
            }
            catch (Exception ex)
            {
                using (db_meieEntities db = new db_meieEntities())
                {
                    #region auditoria
                    AuditoriaOficina aud = new AuditoriaOficina();
                    var usrID = Convert.ToInt32(System.Web.HttpContext.Current.Session["idUsuario"].ToString());

                    var nombreUsuario = db.SegUser.Where(x => x.Id == usrID).FirstOrDefault();
                    aud.AddLogCambios(2, 1, "El usuario " + nombreUsuario.FullName + " ha generado un error al realizar una Grabación de historial Etapa para la obra " + idObra + "- Búho Licitaciones", usrID, idObra);

                    #endregion
                }
                log.Error("Error grabar historial etapa", ex);
            }
            return 0;
        }      
        public async Task enviarNotificacionAdjunto(int? idObra, int? idTipoMail, int? idCategoria)
        {
            log4net.Config.XmlConfigurator.Configure(new FileInfo(ruta));
            log.Info("Envia mail Notificacion con Adjunto");
            var nombreObra = "";
            int? idOrganismo = 0;
            var mailEmpresa = "";
            var mailOrganismo = "";
            var archivoAdjunto = "";
            using (db_meieEntities db = new db_meieEntities())
            {
                var unObra = db.PryProyecto.Where(x => x.Id == idObra).FirstOrDefault();
                if(unObra != null)
                {
                    nombreObra = unObra.Nombre;
                    idOrganismo = unObra.PryOrganismoEjecutor_Id;

                    var listaEmpresa = db.LicOfertaEmpresa.Where(x => x.idObra == idObra && x.activa == 1 && x.esFracasada == null).ToList();
                    var unOrganismo = db.PryOrganismosEjecutores.Where(x => x.Id == idOrganismo).FirstOrDefault();
                    if(unOrganismo != null)
                    {
                        if (!string.IsNullOrEmpty(unOrganismo.mailOrganisimo))
                        {
                            mailOrganismo = unOrganismo.mailOrganisimo;
                            log.Info("Envia mail Notificacion con Adjunto a Organismo: " + mailOrganismo);
                        }
                    }
                    foreach(var item in listaEmpresa)
                    {
                        using (DB_RACOPEntities db2 = new DB_RACOPEntities())
                        {
                            var unEmpresa = db2.rc_Empresa.Where(x => x.idEmpresa == item.idEmpresa).FirstOrDefault();
                            if(unEmpresa != null)
                            {
                                if (!string.IsNullOrEmpty(unEmpresa.mailEspecial))
                                {
                                    mailEmpresa = mailEmpresa + unEmpresa.mailEspecial + ";";
                                    log.Info("Envia mail Notificacion con Adjunto a: " + mailEmpresa);
                                }
                            }
                        }
                    }
                }
                var unAdjunto = db.LicArchivoObra.Where(x => x.idCategoria == idCategoria && x.idObra == idObra && x.esFracasada == null).FirstOrDefault();
                if(unAdjunto != null)
                {
                    archivoAdjunto = unAdjunto.rutaArchivo;
                }
            }
            
            ServicioEmail servicioEmail = new ServicioEmail();
            var emailViewModels = servicioEmail.buscarMail(5);
            emailViewModels.copiaMail = mailOrganismo;
            if (!string.IsNullOrEmpty(mailEmpresa))
            {
                emailViewModels.destino = mailEmpresa;
            }
            else
            {
                emailViewModels.destino = mailOrganismo;
            }

            emailViewModels.cuerpo = emailViewModels.cuerpo.Replace("#NombreObra", nombreObra);
            // Enviar Mail
            await servicioEmail.enviarEmailAdjunto(emailViewModels, archivoAdjunto);
            await marcarObraConActa(idObra);
        }
        public async Task<int> grabarVencimientoObs(unLicitacionViewModels unLicitacion)
        {
            log4net.Config.XmlConfigurator.Configure(new FileInfo(ruta));
            log.Info("Grabar grabarVencimientoObs");
            int value = 0;
            try
            {
                using (db_meieEntities db = new db_meieEntities())
                {
                    var existe = db.LicProyecto.Where(x => x.idProyecto == unLicitacion.idObra).FirstOrDefault();
                    if (existe != null)
                    {
                        existe.fechaVtoObs = unLicitacion.fechaVtoObs;
                        existe.horaVtoObs = unLicitacion.horaVtoObs;
                    }
                   
                    await db.SaveChangesAsync();
                    #region auditoria
                    AuditoriaOficina aud = new AuditoriaOficina();
                    var usrID = Convert.ToInt32(System.Web.HttpContext.Current.Session["idUsuario"].ToString());

                    var nombreUsuario = db.SegUser.Where(x => x.Id == usrID).FirstOrDefault();
                    aud.AddLogCambios(2, 1, "El usuario " + nombreUsuario.FullName + " ha realizado una Grabación de vencimiento obs: " + unLicitacion.idObra + "- Búho Licitaciones", usrID, unLicitacion.idObra);

                    #endregion
                }
            }
            catch (Exception ex)
            {
                using (db_meieEntities db = new db_meieEntities())
                {
                    #region auditoria
                    AuditoriaOficina aud = new AuditoriaOficina();
                    var usrID = Convert.ToInt32(System.Web.HttpContext.Current.Session["idUsuario"].ToString());

                    var nombreUsuario = db.SegUser.Where(x => x.Id == usrID).FirstOrDefault();
                    aud.AddLogCambios(2, 1, "El usuario " + nombreUsuario.FullName + " ha generado un error al realizar una Grabación de vencimiento obs: " + unLicitacion.idObra + "- Búho Licitaciones", usrID, unLicitacion.idObra);

                    #endregion
                }
                log.Error("Error grabar vencimiento observaciones", ex);
            }
            return value;
        }
        /// <summary>
        /// busca si la empresa esta habilitada para observaciones y mejora si las fechas estan ok
        /// </summary>
        /// <param name="idObra"></param>
        /// <param name="idEmpresa"></param>
        /// <returns></returns>
        public EmpresaHabilitadaViewModels buscarEmpresaHabilitada(int? idObra, int? idEmpresa)
        {
            log4net.Config.XmlConfigurator.Configure(new FileInfo(ruta));
            log.Info("buscar Lista de Fechas");
            var cuit = "";
            using (DB_RACOPEntities db2 = new DB_RACOPEntities())
            {
                var unaEmpresa = db2.rc_Empresa.Where(x => x.idEmpresa == idEmpresa).FirstOrDefault();
                cuit = unaEmpresa.cuit;
            }
            
                EmpresaHabilitadaViewModels unaEmpresaHabilitada = new EmpresaHabilitadaViewModels();
            try
            {
                using (db_meieEntities db = new db_meieEntities())
                {
                    var query = from hab in db.LicEmpresaHabilitada                 //busca en tabla empresa habilitada y carga observaciones y mejora
                                where hab.idObra == idObra && hab.esFracasada == null && (hab.idEmpresa == idEmpresa || hab.cuit == cuit)
                                select new EmpresaHabilitadaViewModels
                                {
                                    idObra = hab.idObra,
                                    cuit = hab.cuit,
                                    idEmpresa = hab.idEmpresa,
                                    razonSocial = hab.razonSocial,
                                    observaciones = hab.observaciones,
                                    mejoraOferta = hab.mejoraoferta,
                                };
                    unaEmpresaHabilitada = query.FirstOrDefault();
                    if(unaEmpresaHabilitada != null)                         //si esta habilidada verifica si tiene archivos observados y mejora
                    {
                        bool tieneEstado4 = db.LicArchivoEmpresa.Any(x => x.idEmpresa == unaEmpresaHabilitada.idEmpresa && x.idObra == idObra && x.idEstadoArchivo == 4 && x.esFracasada == null);

                        if (tieneEstado4)
                        {
                            unaEmpresaHabilitada.observaciones = true;
                        }
                    }
                    else
                    {
                        var query2 = from hab in db.LicArchivoEmpresa           //si la empresa no está en la tabla verifica si tiene archivos observados
                                     where hab.idObra == idObra && hab.idEmpresa == idEmpresa && hab.esFracasada == null
                                    select new EmpresaHabilitadaViewModels
                                    {
                                        idObra = hab.idObra,
                                        cuit = cuit,
                                        idEmpresa = hab.idEmpresa,
                                        razonSocial = "",
                                        observaciones = false,
                                        mejoraOferta = false,
                                        idEstadoArchivo = hab.idEstadoArchivo,
                                    };
                        unaEmpresaHabilitada = query2.FirstOrDefault();
                        var tmp = query2.ToList();
                        if(unaEmpresaHabilitada != null)
                        {
                            foreach (var item in tmp)
                            {
                                if(item.idEstadoArchivo == 4)
                                {
                                    unaEmpresaHabilitada.observaciones = true;
                                }

                            }
                        }
                    }
                    
                }
            }
            catch (Exception ex)
            {
                log.Error("Error buscar empresa habilitada", ex);
            }
            return unaEmpresaHabilitada;
        }
        private async Task marcarObraConActa(int? idObra)
        {
            log4net.Config.XmlConfigurator.Configure(new FileInfo(ruta));
            log.Info("Grabar marcarObraConActa");
            try
            {
                using (db_meieEntities db = new db_meieEntities())
                {
                    var unProyecto = db.LicProyecto.Where(x => x.idProyecto == idObra).FirstOrDefault();
                    if(unProyecto != null)
                    {
                        unProyecto.tieneActa = true;
                        await db.SaveChangesAsync();
                    }
                    else
                    {
                        LicProyecto licProyecto = new LicProyecto();
                        licProyecto.idProyecto = idObra.Value;
                        licProyecto.tieneActa = true;
                        db.LicProyecto.Add(licProyecto);
                        await db.SaveChangesAsync();
                    }
                    #region auditoria
                    AuditoriaOficina aud = new AuditoriaOficina();
                    var usrID = Convert.ToInt32(System.Web.HttpContext.Current.Session["idUsuario"].ToString());

                    var nombreUsuario = db.SegUser.Where(x => x.Id == usrID).FirstOrDefault();
                    aud.AddLogCambios(2, 1, "El usuario " + nombreUsuario.FullName + " ha marcado la obra: " + idObra + " con acta - Búho Licitaciones", usrID, idObra);

                    #endregion
                }
            }
            catch(Exception ex)
            {
                using (db_meieEntities db = new db_meieEntities())
                {
                    #region auditoria
                    AuditoriaOficina aud = new AuditoriaOficina();
                    var usrID = Convert.ToInt32(System.Web.HttpContext.Current.Session["idUsuario"].ToString());

                    var nombreUsuario = db.SegUser.Where(x => x.Id == usrID).FirstOrDefault();
                    aud.AddLogCambios(2, 1, "El usuario " + nombreUsuario.FullName + " ha generado un error al marcar la obra " + idObra + " con acta. - Búho Licitaciones", usrID, idObra);

                    #endregion
                }
                log.Error("Error marcarObraConActa", ex);
            }
        }
        ///Licitación Fracasada
        public int nuevoLlamado(int? idObra)
        {
            var value = 0;
            log4net.Config.XmlConfigurator.Configure(new FileInfo(ruta));
            log.Info("Grabar nuevoLlamado");
            try
            {
                using (db_meieEntities db = new db_meieEntities())
                {
                    // Consulta para cada entidad individual
                    var proyecto = db.PryProyecto.Where(x => x.Id == idObra).FirstOrDefault();
                    var licitacion = db.LicProyecto.Where(x => x.idProyecto == idObra).FirstOrDefault();                   
                    var archivosEmpresa = db.LicArchivoEmpresa.Where(x => x.idObra == idObra && x.esFracasada == null).ToList();
                    var archivosObra = db.LicArchivoObra.Where(x => x.idObra == idObra && x.esFracasada == null).ToList();
                    var documentosSobres = db.LicDocSobres.Where(x => x.idObra == idObra && x.esFracasada == null).ToList();
                    var documentosObra = db.LicDocObra.Where(x => x.idObra == idObra && x.esFracasada == null).ToList();
                    var empresasHabilitadas = db.LicEmpresaHabilitada.Where(x => x.idObra == idObra && x.esFracasada == null).ToList();
                    var empresaObra = db.LicEmpresaObra.Where(x => x.idObra == idObra && x.esFracasada == null).ToList();
                    var ofertaEmpresa = db.LicOfertaEmpresa.Where(x => x.idObra == idObra && x.esFracasada == null).ToList();
                    var fechasObra = db.LicProyectoFecha.Where(x => x.idProyecto == idObra && x.esFracasada == null).ToList();
                    var listaConsulta = db.LicConsultas.Where(x => x.idObra == idObra && x.esFracasada == null).ToList();
                    var unActa = db.LicActaObra.Where(x => x.idObra == idObra).FirstOrDefault();

                    var numeroFracaso = db.LicDocSobres
                        .Where(x => x.idObra == idObra)
                        .Max(x => x.esFracasada) + 1;
                    if (!numeroFracaso.HasValue)
                    {
                        numeroFracaso = 1;
                    }
                    // Iterar y asignar esFracasada segun el numero anterior // copio sobres y requisitos  
                    //foreach (var documentoSobres in documentosSobres)
                    //{
                    //    documentoSobres.esFracasada = numeroFracaso;

                    //    LicDocSobres licDocSobres = new LicDocSobres();
                    //    licDocSobres.idObra = documentoSobres.idObra;
                    //    licDocSobres.nombre = documentoSobres.nombre;
                    //    licDocSobres.numero = documentoSobres.numero;
                    //    licDocSobres.activo = documentoSobres.activo;
                    //    db.LicDocSobres.Add(licDocSobres);
                    //    db.SaveChangesAsync();
                    //}
                    //foreach (var documentoObra in documentosObra)
                    //{
                    //    documentoObra.esFracasada = numeroFracaso;

                    //    LicDocObra licDocObra = new LicDocObra();
                    //    licDocObra.idObra = documentoObra.idObra;
                    //    licDocObra.idDocumentacion = documentoObra.idDocumentacion;
                    //    licDocObra.nroSobre = documentoObra.nroSobre;
                    //    db.LicDocObra.Add(licDocObra);
                    //    db.SaveChangesAsync();
                    //}
                    if(unActa != null)
                    {
                        db.LicActaObra.Remove(unActa);
                        db.SaveChangesAsync();
                    }
                    foreach (var archivoEmpresa in archivosEmpresa)
                    {                       
                        archivoEmpresa.esFracasada = numeroFracaso;
                    }
                    foreach (var archivoObra in archivosObra)
                    {
                        archivoObra.esFracasada = numeroFracaso;
                    }    
                    foreach (var empresaHabilitada in empresasHabilitadas)
                    {
                        //empresaHabilitada.esFracasada = numeroFracaso;
                        db.LicEmpresaHabilitada.Remove(empresaHabilitada);
                        db.SaveChangesAsync();
                    }
                    foreach (var empresa in empresaObra)
                    {
                        empresa.esFracasada = numeroFracaso;
                    }
                    foreach (var oferta in ofertaEmpresa)
                    {
                        oferta.esFracasada = numeroFracaso;
                    }
                    foreach (var fechaObra in fechasObra)
                    {
                        fechaObra.esFracasada = numeroFracaso;
                    }     
                    if(licitacion != null)
                    {
                        licitacion.tieneActa = null;
                        licitacion.tieneContrato = null;
                    }
                    foreach(var cons in listaConsulta)
                    {
                        db.LicConsultas.Remove(cons);
                        db.SaveChangesAsync();
                    }
                    db.SaveChanges();
                    #region auditoria
                    AuditoriaOficina aud = new AuditoriaOficina();
                    var usrID = Convert.ToInt32(System.Web.HttpContext.Current.Session["idUsuario"].ToString());

                    var nombreUsuario = db.SegUser.Where(x => x.Id == usrID).FirstOrDefault();
                    aud.AddLogCambios(2, 1, "El usuario " + nombreUsuario.FullName + " ha generado un nuevo llamado para la obra: " + idObra + "- Búho Licitaciones", usrID, idObra);

                    #endregion
                }
            }
            catch (Exception ex)
            {
                using (db_meieEntities db = new db_meieEntities())
                {
                    #region auditoria
                    AuditoriaOficina aud = new AuditoriaOficina();
                    var usrID = Convert.ToInt32(System.Web.HttpContext.Current.Session["idUsuario"].ToString());

                    var nombreUsuario = db.SegUser.Where(x => x.Id == usrID).FirstOrDefault();
                    aud.AddLogCambios(2, 1, "El usuario " + nombreUsuario.FullName + " ha generado un error ha generado un nuevo llamado para la obra " + idObra + "- Búho Licitaciones", usrID, idObra);

                    #endregion
                }
                log.Error("Error nuevo llamado", ex);
            }
            return value;
        }
        public bool tieneContrato(int? idObra)
        {
            log4net.Config.XmlConfigurator.Configure(new FileInfo(ruta));
            log.Info("Grabar tieneContrato");
            try
            {
                using (db_meieEntities db = new db_meieEntities())
                {
                    var unProyecto = db.LicProyecto.Where(x => x.idProyecto == idObra).FirstOrDefault();
                    if (unProyecto != null)
                    {
                        return unProyecto.tieneContrato.HasValue?unProyecto.tieneContrato.Value:false;
                    }
                    return true;
                }
            }
            catch(Exception ex)
            {
                log.Error("Error tieneContrato", ex);
            }
            return false;
        }
        public bool tieneActa(int? idObra)
        {
            log4net.Config.XmlConfigurator.Configure(new FileInfo(ruta));
            log.Info("Grabar tieneActa");
            try
            {
                using (db_meieEntities db = new db_meieEntities())
                {
                    var unProyecto = db.LicProyecto.Where(x => x.idProyecto == idObra).FirstOrDefault();
                    if (unProyecto != null)
                    {
                        return unProyecto.tieneActa.HasValue ? unProyecto.tieneActa.Value : false;
                    }
                    return true;
                }
            }
            catch (Exception ex)
            {
                log.Error("Error tieneActa", ex);
            }
            return false;
        }
    }
}

