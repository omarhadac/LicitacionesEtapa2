
using Licitacion.Servicios.Utiles;
using Licitaciones.BaseDato;
using Licitaciones.ViewModels;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Licitacion.Servicios
{
    public class ServicioEmpresa
    {
        private static readonly log4net.ILog log = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
        private static string ruta = System.Web.HttpRuntime.AppDomainAppPath + "\\bin\\log4net.config";
        public List<licitacionGrillaViewModels> listaLicitaciones
        (ref int recordsTotal, string sortColumn, string sortColumnDir, string searchValue,
            string nroExpediente, string nombreObra, string qObra, int? idEmpresa, int? idFavorito, int? idEtapa, DateTime? fechaPub, int? idOrganismo,
            int pageSize = 0, int skip = 0)
        {
            log4net.Config.XmlConfigurator.Configure(new FileInfo(ruta));
            log.Info("Grilla Licitaciones Empresa");
            List<licitacionGrillaViewModels> lista = new List<licitacionGrillaViewModels>();
            try
            {
                using (db_meieEntities db = new db_meieEntities())
                {
                    IQueryable<licitacionGrillaViewModels> query = from lic in db.PryProyecto
                                join edo in db.PryEstadoEtapa
                                on lic.GrlTypeState_Id equals edo.Id
                                join pyl in db.PryLicitacion
                                on lic.Id equals pyl.IdPryProyecto
                                    into pylo
                                from pyj in pylo.DefaultIfEmpty()
                                join tcont in db.GrlType
                                 on pyj.TipoDeContratacion_Id equals tcont.Id 
                                   into tconto 
                                   from tcontj in tconto.DefaultIfEmpty()
                                join org in db.PryOrganismosEjecutores
                                on lic.PryOrganismoEjecutor_Id equals org.Id
                                    into orgo
                                from orgj in orgo.DefaultIfEmpty()
                                join empO in db.LicEmpresaObra.Where(x=>x.idEmpresa == idEmpresa)
                                on lic.Id equals empO.idObra
                                    into empOo
                                from empOj in empOo.DefaultIfEmpty()
                                join ofe in db.LicOfertaEmpresa.Where(x => x.idEmpresa == idEmpresa && x.activa == 1)
                                on lic.Id equals ofe.idObra
                                    into ofeo
                                from ofej in ofeo.DefaultIfEmpty()
                                    //where edo.PryStage_Id == 49 && lic.Eliminado == false && edo.Id < 8 && edo.Id > 5                                   
                                    where edo.PryStage_Id == 49 && lic.Eliminado == false && pyj.FechaApertura != null && pyj.FechaPublicacionDesde != null 
                                    && edo.Id < 8 && edo.Id > 5 || (edo.Id == 8 && ofej.idEmpresa == idEmpresa)
                                    
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
                                    idContratacion = pyj.TipoDeContratacion_Id,
                                    nombreContratacion = tcontj.Name,
                                    idFavorito = empOj.esFavorito,
                                    fechaOferta=ofej.fecha,                                  
                                };

                    if (!string.IsNullOrEmpty(qObra))
                    {
                        if (qObra != "*")
                        {
                            var array = qObra.Split(',');
                            List<int?> listaObra = new List<int?>();
                            foreach(var item in array)
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
                    if (idFavorito != 0)
                    {
                        query = query.Where(x => x.idFavorito == idFavorito);
                    }
                    if (idEtapa != 0)
                    {
                        query = query.Where(x => x.idEtapa == idEtapa);
                    }
                    if (!string.IsNullOrEmpty(nombreObra))
                    {
                        query = query.Where(x => x.nombreObra.Contains(nombreObra));
                    }
                    if (!string.IsNullOrEmpty(nroExpediente))
                    {
                        query = query.Where(x => x.nroExpediente.Contains(nroExpediente));
                    }
                    //if ((monto != null)&&(monto != 0))
                    //{
                    //    query = query.Where(x => x.montoObra == monto);
                    //}
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
                        if ((sortColumn == "fechaAperturaString") && (sortColumnDir == "desc"))
                        {
                            query = query.OrderByDescending(x => x.fechaApertura);
                        }
                        if ((sortColumn == "fechaAperturaString") && (sortColumnDir == "asc"))
                        {
                            query = query.OrderBy(x => x.fechaApertura);
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
                        if ((sortColumn == "estadoOferta") && (sortColumnDir == "asc"))
                        {
                            query = query.OrderBy(x => x.fechaOferta);
                        }
                        if ((sortColumn == "estadoOferta") && (sortColumnDir == "desc"))
                        {
                            query = query.OrderByDescending(x => x.fechaOferta);
                        }
                    }
                    else
                    {
                        query = query = query.OrderBy(x => x.nombreObra);
                    }

                    query = query.Take(500);
                    recordsTotal = query.Count();

                    var lst = query.Skip(skip).Take(pageSize);
                    lista = lst.ToList();
                    return lista;
                }
            }
            catch (Exception ex)
            {
                log.Error("Error en Grilla Licitaciones Empresa", ex);
            }
            return lista;
        }
        public unLicitacionViewModels buscarUna(int? idObra)
        {
            log4net.Config.XmlConfigurator.Configure(new FileInfo(ruta));
            log.Info("Una Licitacion");
            unLicitacionViewModels unLicitacion = new unLicitacionViewModels();
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
                                    mailConsulta=licpryj.mailConsulta,
                                    fechaCierre = licpryj.fechaCierre,
                                    lugarVisita = licpryj.lugarVisita,
                                    fechaVisita = licpryj.fechaVisita,
                                    fechaConsulta = licpryj.limiteConsulta,
                                    rutaVirtual = licpryj.domicilioVirtual
                                };
                    unLicitacion = query.FirstOrDefault();
                }
            }
            catch (Exception ex)
            {
                log.Error("Error grabar Buscar Una Licitacion Empresa", ex);
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
                    
                    if(existe != null)
                    {
                        existe.domicilioApertura = unLicitacion.domicilioApertura;
                        existe.domicilioPresentacion = unLicitacion.domicilioPresentacion;
                        existe.limiteConsulta = unLicitacion.fechaConsulta;
                        existe.idProyecto = unLicitacion.idObra.Value;
                        existe.mailConsulta = unLicitacion.mailConsulta;
                        existe.urlPliego = unLicitacion.urlPliego;
                        existe.moneda = unLicitacion.idMoneda;
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
                        db.LicProyecto.Add(licProyecto);
                        await db.SaveChangesAsync();
                    }
                }
            }
            catch(Exception ex)
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
            catch(Exception ex)
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
                        existe.GrlTypeState_Id = unLicitacion.idEtapa;
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
                }
            }
            catch (Exception ex)
            {
                log.Error("Error grabar Licitacion", ex);
            }
            return value;
        }
        public async Task<int> marcarFavoritas(obraFavoritaViewModels favorito)
        {
            log4net.Config.XmlConfigurator.Configure(new FileInfo(ruta));
            log.Info("Marcar Favorita");
            int value = 1;
            try
            {
                using (db_meieEntities db = new db_meieEntities())
                {
                    var unFavorito = db.LicEmpresaObra
                        .Where(x => x.idEmpresa == favorito.idEmpresa && x.idObra == favorito.idObra)
                        .FirstOrDefault();
                    if(unFavorito != null)
                    {
                        if(unFavorito.esFavorito == 0)
                        {
                            unFavorito.esFavorito = 1;
                        }
                        else
                        {
                            unFavorito.esFavorito = 0;
                        }
                        await db.SaveChangesAsync();
                    }
                    else
                    {
                        LicEmpresaObra licEmpresa = new LicEmpresaObra();
                        licEmpresa.esFavorito = 1;
                        licEmpresa.idObra = favorito.idObra.Value;
                        licEmpresa.idEmpresa = favorito.idEmpresa;
                        db.LicEmpresaObra.Add(licEmpresa);
                        await db.SaveChangesAsync();
                    }
                }
            }
            catch(Exception ex)
            {
                value = 0;
                log.Error("Error Marcar Favorita Empresa", ex);
            }
            return value;
        }
        public async Task grabarOferta(int? idObra, int? idEmpresa, string ruta, int? tipoOferta)
        {
            try
            {
                using (db_meieEntities db= new db_meieEntities())
                {
                    var unOferta = db.LicOfertaEmpresa
                        .Where(x => x.idEmpresa == idEmpresa && x.idObra == idObra && x.activa == 1)
                        .FirstOrDefault();

                    if (unOferta != null)
                    {
                        unOferta.activa = 0;
                        await db.SaveChangesAsync();
                    }

                    LicOfertaEmpresa ofertaEmpresa = new LicOfertaEmpresa();
                    ofertaEmpresa.idEmpresa = idEmpresa;
                    ofertaEmpresa.idObra = idObra;
                    ofertaEmpresa.fecha = DateTime.Now;
                    ofertaEmpresa.rutaComprobante = ruta;
                    ofertaEmpresa.tipoOferta = tipoOferta;
                    ofertaEmpresa.activa = 1;
                    db.LicOfertaEmpresa.Add(ofertaEmpresa);
                    await db.SaveChangesAsync();
                }
            }
            catch(Exception ex)
            {

            }
        }
        public async Task<int> enviarOferta(empresaOfertaViewModels oferta)
        {
            log4net.Config.XmlConfigurator.Configure(new FileInfo(ruta));
            log.Info("Enviar Oferta");
            int value = 1;
            try
            {
                using (db_meieEntities db = new db_meieEntities())
                {
                    List<RequisitoViewModels> lista = new List<RequisitoViewModels>();
                    var query = (from tb1 in db.LicDocObra
                                 join tb2 in db.LicDocGeneral
                                    on tb1.idDocumentacion equals tb2.idLicDocGeneral
                                 where tb1.idObra == oferta.idObra && tb1.esFracasada == null
                                 select new RequisitoViewModels
                                 {
                                     idPadre = tb2.idPadre,
                                     nombre = tb2.nombre,
                                     idRequisito = tb1.idDocumentacion,
                                     nroSobre = tb1.nroSobre,
                                     id = tb1.idLicDocObra,
                                     horaArchivo = "",
                                     idArchivo = 0
                                 });
                    lista = query.ToList();
                    foreach (var item in lista)
                    {
                        var tieneArchivo = db.LicArchivoEmpresa
                                              .Where(x => x.idEmpresa == oferta.idEmpresa  && x.idObra == oferta.idObra && x.idRequisito == item.id && x.esFracasada == null)
                                              .FirstOrDefault();
                        if (tieneArchivo == null)
                        {
                            LicArchivoEmpresa archivoEmpresa = new LicArchivoEmpresa();
                            archivoEmpresa.fecha = DateTime.Now.Date;
                            archivoEmpresa.hora = DateTime.Now.ToString("hh:mm"); ;
                            archivoEmpresa.idObra = oferta.idObra;
                            archivoEmpresa.idEmpresa = oferta.idEmpresa;
                            archivoEmpresa.idEstadoArchivo = 6;
                            archivoEmpresa.tipoOferta = 1;
                            archivoEmpresa.idRequisito = item.id;
                            archivoEmpresa.nroSobre = item.nroSobre;
                            db.LicArchivoEmpresa.Add(archivoEmpresa);
                            await db.SaveChangesAsync();
                        }
                    }

                }
                #region auditoria
                AuditoriaRacop aud = new AuditoriaRacop();
                var usrID = Convert.ToInt32(System.Web.HttpContext.Current.Session["idUsuario"].ToString());
                var idEmpresa = Convert.ToInt32(System.Web.HttpContext.Current.Session["idEmpresa"].ToString());
                aud.AddLogCreate(usrID, idEmpresa, " ha enviado oferta: " + oferta.idObra.ToString() + "- Búho Licitaciones");

                #endregion
            }
            catch (Exception ex)
            {
                value = 0;
                #region auditoria
                AuditoriaRacop aud = new AuditoriaRacop();
                var usrID = Convert.ToInt32(System.Web.HttpContext.Current.Session["idUsuario"].ToString());
                var idEmpresa = Convert.ToInt32(System.Web.HttpContext.Current.Session["idEmpresa"].ToString());
                aud.AddLogCreate(usrID, idEmpresa, " ha generado un error al oferta: " + oferta.idObra.ToString() + "- Búho Licitaciones");

                #endregion
                log.Error("Error enviarOferta Empresa", ex);
            }
            return value;
        }
        public async Task<string> enviarOfertaObservacion(empresaOfertaViewModels oferta)
        {
            log4net.Config.XmlConfigurator.Configure(new FileInfo(ruta));
            log.Info("Enviar Oferta Observada");
            string value = "";
            try
            {
                using (db_meieEntities db = new db_meieEntities())
                {
                    List<RequisitoViewModels> lista = new List<RequisitoViewModels>();
                    var query = (from tb1 in db.LicDocObra
                                 join tb2 in db.LicDocGeneral
                                    on tb1.idDocumentacion equals tb2.idLicDocGeneral
                                 where tb1.idObra == oferta.idObra
                                 select new RequisitoViewModels
                                 {
                                     idPadre = tb2.idPadre,
                                     nombre = tb2.nombre,
                                     idRequisito = tb1.idDocumentacion,
                                     nroSobre = tb1.nroSobre,
                                     id = tb1.idLicDocObra,
                                     horaArchivo = "",
                                     idArchivo = 0
                                 });
                    lista = query.ToList();
                    foreach (var item in lista)
                    {
                        DateTime fechaControl = DateTime.Now.Date;
                        var existeArchivo = db.LicArchivoEmpresa
                                  .Where(x => x.idEmpresa == oferta.idEmpresa && x.idObra == oferta.idObra && x.idRequisito == item.id && x.fecha == fechaControl && x.tipoOferta == 1 && x.idEstadoArchivo >3 )
                                  .FirstOrDefault();
                        if(existeArchivo != null)
                        {
                            var tieneArchivo = db.LicArchivoEmpresa
                      .Where(x => x.idEmpresa == oferta.idEmpresa && x.idObra == oferta.idObra && x.idRequisito == item.id && x.fecha == fechaControl && x.tipoOferta == 2)
                      .FirstOrDefault();
                            if (tieneArchivo == null)
                            {
                                LicArchivoEmpresa archivoEmpresa = new LicArchivoEmpresa();
                                archivoEmpresa.fecha = DateTime.Now.Date;
                                archivoEmpresa.hora = DateTime.Now.ToString("hh:mm"); ;
                                archivoEmpresa.idObra = oferta.idObra;
                                archivoEmpresa.idEmpresa = oferta.idEmpresa;
                                archivoEmpresa.idEstadoArchivo = 6;
                                archivoEmpresa.idRequisito = item.id;
                                archivoEmpresa.nroSobre = item.nroSobre;
                                archivoEmpresa.tipoOferta = 2;
                                db.LicArchivoEmpresa.Add(archivoEmpresa);
                                await db.SaveChangesAsync();
                            }
                        }
                    }

                }
                using (DB_RACOPEntities db = new DB_RACOPEntities())
                {
                    DatosEmpresaViewModels unEmpresa = new DatosEmpresaViewModels();
                    var query = db.rc_Empresa.Where(x => x.idEmpresa == oferta.idEmpresa).FirstOrDefault();
                    if (query != null)
                    {
                        unEmpresa.emailEmpresa = query.mailContacto;
                        unEmpresa.emailCopiaEmpresa = query.mailEspecial;
                        unEmpresa.idEmpresa = query.idEmpresa;
                        unEmpresa.idObra = oferta.idObra;
                        unEmpresa.nombreEmpresa = query.razonSocial;
                        unEmpresa.cuitEmpresa = query.cuit;
                        var nombreEmpresa = query.razonSocial;
                        value = await generarComprobanteOfertaObs(unEmpresa);
                    }
                }
                #region auditoria
                AuditoriaRacop aud = new AuditoriaRacop();
                var usrID = Convert.ToInt32(System.Web.HttpContext.Current.Session["idUsuario"].ToString());
                var idEmpresa = Convert.ToInt32(System.Web.HttpContext.Current.Session["idEmpresa"].ToString());
                aud.AddLogCreate(usrID, idEmpresa, " ha enviado observación de oferta: " + oferta.idObra.ToString() + "- Búho Licitaciones");

                #endregion
            }
            catch (Exception ex)
            {
                value = "NoOk";
                #region auditoria
                AuditoriaRacop aud = new AuditoriaRacop();
                var usrID = Convert.ToInt32(System.Web.HttpContext.Current.Session["idUsuario"].ToString());
                var idEmpresa = Convert.ToInt32(System.Web.HttpContext.Current.Session["idEmpresa"].ToString());
                aud.AddLogCreate(usrID, idEmpresa, " ha generado un error al enviar oferta: " + oferta.idObra.ToString() + "- Búho Licitaciones");

                #endregion
                log.Error("Error enviarOferta Empresa", ex);
            }
            return value;
        }
        public string buscarNombreEmpresa(int? idEmpresa, string cuit)
        {
            log4net.Config.XmlConfigurator.Configure(new FileInfo(ruta));
            log.Info("Una buscarNombreEmpresa");
            string nombreEmpresa = string.Empty;
            try
            {
                using (DB_RACOPEntities db = new DB_RACOPEntities())
                using (db_meieEntities _db = new db_meieEntities())
                {
                    var query = db.rc_Empresa.Where(x => x.idEmpresa == idEmpresa).FirstOrDefault();
                    if(query != null)
                    {
                        nombreEmpresa = query.razonSocial;
                    }
                    else
                    {
                        var empresasHabilitadas = _db.LicEmpresaHabilitada.Where(x => x.cuit == cuit).FirstOrDefault();
                        if(empresasHabilitadas != null)
                        {
                            nombreEmpresa = empresasHabilitadas.razonSocial;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                log.Error("Error buscarNombreEmpresa", ex);
            }
            return nombreEmpresa;
        }
        public List<licitacionGrillaViewModels> listarObraEmpresaExpotar
        (ref int recordsTotal, string sortColumn, string sortColumnDir, string searchValue,
            string nroExpediente, string nombreObra,string qObra, int? idEmpresa, int? idFavorito, DateTime? fechaPub, int? idOrganismo,
            int pageSize = 0, int skip = 0)
        {
            log4net.Config.XmlConfigurator.Configure(new FileInfo(ruta));
            log.Info("Grilla Licitaciones Empresa");
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
                                join tcont in db.GrlType
                                 on pyj.TipoDeContratacion_Id equals tcont.Id
                                   into tconto
                                from tcontj in tconto.DefaultIfEmpty()
                                join org in db.PryOrganismosEjecutores
                                on lic.PryOrganismoEjecutor_Id equals org.Id
                                    into orgo
                                from orgj in orgo.DefaultIfEmpty()
                                join empO in db.LicEmpresaObra.Where(x => x.idEmpresa == idEmpresa)
                                on lic.Id equals empO.idObra
                                    into empOo
                                from empOj in empOo.DefaultIfEmpty()
                                join ofe in db.LicOfertaEmpresa.Where(x => x.idEmpresa == idEmpresa && x.activa == 1)
                                on lic.Id equals ofe.idObra
                                    into ofeo
                                from ofej in ofeo.DefaultIfEmpty()
                                where edo.PryStage_Id == 49 && lic.Eliminado == false && edo.Id < 9 && edo.Id > 5
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
                                    idContratacion = pyj.TipoDeContratacion_Id,
                                    nombreContratacion = tcontj.Name,
                                    idFavorito = empOj.esFavorito,
                                    fechaOferta = ofej.fecha,
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
                    if (idFavorito != 0)
                    {
                        query = query.Where(x => x.idFavorito == idFavorito);
                    }
                    if (!string.IsNullOrEmpty(nombreObra))
                    {
                        query = query.Where(x => x.nombreObra.Contains(nombreObra));
                    }
                    if (!string.IsNullOrEmpty(nroExpediente))
                    {
                        query = query.Where(x => x.nroExpediente.Contains(nroExpediente));
                    }
                    //if ((monto != null)&&(monto != 0))
                    //{
                    //    query = query.Where(x => x.montoObra == monto);
                    //}
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
                        if ((sortColumn == "fechaAperturaString") && (sortColumnDir == "desc"))
                        {
                            query = query.OrderByDescending(x => x.fechaApertura);
                        }
                        if ((sortColumn == "fechaAperturaString") && (sortColumnDir == "asc"))
                        {
                            query = query.OrderBy(x => x.fechaApertura);
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
                        if ((sortColumn == "estadoOferta") && (sortColumnDir == "asc"))
                        {
                            query = query.OrderBy(x => x.fechaOferta);
                        }
                        if ((sortColumn == "estadoOferta") && (sortColumnDir == "desc"))
                        {
                            query = query.OrderByDescending(x => x.fechaOferta);
                        }
                    }
                    else
                    {
                        query = query = query.OrderBy(x => x.nombreObra);
                    }

                    query = query.Take(500);
                    recordsTotal = query.Count();

                    var lst = query.Skip(skip).Take(pageSize);
                    lista = lst.ToList();
                    listaFiltrada = query.ToList();

                    return listaFiltrada;
                }
            }
            catch (Exception ex)
            {
                log.Error("Error en Grilla Licitaciones Empresa", ex);
            }
            return listaFiltrada;
        }
        public ReporteExcelVM generarExcelGrilla(List<licitacionGrillaViewModels> obras, bool? nombre, bool? expediente, bool? publicacion, bool? organismo, bool? monto, bool? etapa, bool? apertura, bool? oferta)
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
                        detalle.estadoOferta = item.estadoOferta;
                        excel.filas.Add(detalle);
                    }
                }
            }
            ExcelUtility excelUtility = new ExcelUtility();
            excel = excelUtility.GenerarExcelGrillaEmpresa(excel, nombre, expediente, publicacion, organismo, monto, etapa, apertura, oferta);
            return excel;
        }
        public List<EmpresaOfertaViewModels> listarEmpresaOferta(int? idObra)
        {
            List<EmpresaOfertaViewModels> lista = new List<EmpresaOfertaViewModels>();
            try
            {
                using (db_meieEntities db = new db_meieEntities())
                {
                    var query = from of in db.LicOfertaEmpresa
                                where of.idObra == idObra && of.activa == 1
                                select new EmpresaOfertaViewModels
                                {
                                    idEmpresa = of.idEmpresa,
                                    idObra = of.idObra,
                                    esGanadora = of.esGanadora
                                };
                    lista = query.OrderByDescending(x => x.esGanadora).ToList();
                }
                using(DB_RACOPEntities db1 = new DB_RACOPEntities())
                {
                    foreach (var item in lista)
                    {
                        var unEmpresa = db1.rc_Empresa.Where(x => x.idEmpresa == item.idEmpresa).FirstOrDefault();
                        if(unEmpresa != null)
                        {
                            item.nombreEmpresa = unEmpresa.razonSocial;
                        }
                    }
                }
            }
            catch (Exception ex)
            {

            }
            return lista;
        }
        public int marcarGanadora(int? idObra, int? idEmpresa)
        {
            var value = 0;
            try
            {
                using (db_meieEntities db = new db_meieEntities())
                {
                    var existe = db.LicOfertaEmpresa.Where(x => x.idEmpresa == idEmpresa && x.idObra == idObra && x.activa == 1).FirstOrDefault();
                    if (existe != null)
                    {
                        ServicioLicitacion servicioLicitacion = new ServicioLicitacion();
                        value = servicioLicitacion.cambiarEtapa(idObra, 14); //PreAdjudicada;
                        if (value == 1)
                        {
                            existe.esGanadora = 1;
                            db.SaveChanges();
                        }
                    }
                }
            }
            catch (Exception ex)
            {

            }
            return value;
        }
        public DatosEmpresaViewModels buscarDatosEmpresa(int? idEmpresa)
        {
            log4net.Config.XmlConfigurator.Configure(new FileInfo(ruta));
            log.Info("Una buscarDatosEmpresa");
            DatosEmpresaViewModels unEmpresa = new DatosEmpresaViewModels();
            try
            {
                using (DB_RACOPEntities db = new DB_RACOPEntities())
                {
                    var query = db.rc_Empresa.Where(x => x.idEmpresa == idEmpresa).FirstOrDefault();
                    if (query != null)
                    {
                        unEmpresa.nombreEmpresa = query.razonSocial;
                        unEmpresa.idEmpresa = query.idEmpresa;
                        unEmpresa.emailEmpresa = query.mailContacto;
                    }
                }
            }
            catch (Exception ex)
            {
                log.Error("Error buscarDatosEmpresa", ex);
            }
            return unEmpresa;
        }
        public async Task<string> grabarEmailEmpresa(DatosEmpresaViewModels unEmpresa)
        {
            string value = "";
            log4net.Config.XmlConfigurator.Configure(new FileInfo(ruta));
            log.Info("Una grabarEmailEmpresa");
            try
            {
                using (DB_RACOPEntities db = new DB_RACOPEntities())
                {
                    var query = db.rc_Empresa.Where(x => x.idEmpresa == unEmpresa.idEmpresa).FirstOrDefault();
                    if (query != null)
                    {
                        query.mailContacto = unEmpresa.emailEmpresa;
                        //await db.SaveChangesAsync();
                        unEmpresa.nombreEmpresa = query.razonSocial;
                        value = await generarComprobanteOferta(unEmpresa);

                    }
                }
            }
            catch (Exception ex)
            {
                log.Error("Error grabarEmailEmpresa", ex);
                return value;
            }
            return value;
        }
        public async Task<string> generarComprobanteOferta(DatosEmpresaViewModels unEmpresa)
        {
            string filePath = "";
            string nombre = "";
            log4net.Config.XmlConfigurator.Configure(new FileInfo(ruta));
            log.Info("enviarMailOferta");
            try
            {
                using (DB_RACOPEntities db = new DB_RACOPEntities())
                {
                    // Buscar Oferta 
                    ServicioDocumentacion servicioDocumentacion = new ServicioDocumentacion();
                    List<RequisitoViewModels> listaArchivo = new List<RequisitoViewModels>();
                    listaArchivo = servicioDocumentacion.listarRequisitoPorObraEmpresa(unEmpresa.idEmpresa, unEmpresa.idObra);
                    ServicioLicitacion servicioLicitacion = new ServicioLicitacion();
                    var unObra = servicioLicitacion.buscarUna(unEmpresa.idObra);
                    if(unObra != null)
                    {
                        unEmpresa.nombreObra = unObra.nombreObra;
                    }
                    // Buscar Tipo Mail
                    ServicioEmail servicioEmail = new ServicioEmail();
                    EmailViewModels emailViewModels = new EmailViewModels();
                    emailViewModels = servicioEmail.buscarMail(1);
                    emailViewModels.destino = unEmpresa.emailEmpresa;
                    emailViewModels.copiaMail = unEmpresa.emailCopiaEmpresa;
                    var lista = string.Empty;
                    lista = "<ul>";
                    foreach(var item in listaArchivo)
                    {
                        lista = lista + "<li>";
                        lista = lista + "Requisito " + item.nombre + " Archivo: " + item.nombreArchivo;
                        lista = lista + "</li>";
                    }
                    lista = lista + "</ul>";
                    emailViewModels.cuerpo = emailViewModels.cuerpo.Replace("#ListaArchivos", lista);
                    emailViewModels.cuerpo = emailViewModels.cuerpo.Replace("#Empresa", unEmpresa.nombreEmpresa);
                    emailViewModels.cuerpo = emailViewModels.cuerpo.Replace("#NombreObra", unEmpresa.nombreObra);
                    ServicioPDF servicio = new ServicioPDF();
                    filePath = ConfigurationSettings.AppSettings["repositorioComprobante"].ToString();
                    //filePath = ConfigurationSettings.AppSettings["repositorioFiles"].ToString();
                    nombre = "Oferta_" + unEmpresa.idObra + unEmpresa.idEmpresa + DateTime.Now.TimeOfDay.Ticks + ".pdf";
                    filePath = filePath + nombre;
                    servicio.generarComprobanteOferta(unEmpresa.idEmpresa, unEmpresa.idObra, filePath,unObra.nombreObra);
                    await grabarOferta(unEmpresa.idObra, unEmpresa.idEmpresa, filePath,1);
                    //await actualizarArchivoOferta(unEmpresa.idEmpresa, unEmpresa.idObra, filePath);
                    // Enviar Mail
                    //await servicioEmail.enviarEmail(emailViewModels);
                }
                #region auditoria
                AuditoriaRacop aud = new AuditoriaRacop();
                var usrID = Convert.ToInt32(System.Web.HttpContext.Current.Session["idUsuario"].ToString());
                var idEmpresa = Convert.ToInt32(System.Web.HttpContext.Current.Session["idEmpresa"].ToString());
                aud.AddLogCreate(usrID, idEmpresa, " ha generado el comprobante de oferta: " + unEmpresa.idObra.ToString() + "- Búho Licitaciones");

                #endregion
            }
            catch (Exception ex)
            {
                #region auditoria
                AuditoriaRacop aud = new AuditoriaRacop();
                var usrID = Convert.ToInt32(System.Web.HttpContext.Current.Session["idUsuario"].ToString());
                var idEmpresa = Convert.ToInt32(System.Web.HttpContext.Current.Session["idEmpresa"].ToString());
                aud.AddLogCreate(usrID, idEmpresa, " ha generado un error al crear el comprobante de oferta: " + unEmpresa.idObra.ToString() + "- Búho Licitaciones");

                #endregion
                log.Error("enviarMailOferta", ex);
                return nombre;
            }
            return nombre;
        }        
        public async Task actualizarArchivoOferta(int? idEmpresa, int? idObra, string ruta)
        {
            try
            {
                using (db_meieEntities db=new db_meieEntities())
                {
                    var unOferta = db.LicOfertaEmpresa.Where(x => x.idEmpresa == idEmpresa && x.idObra == idObra && x.activa == 1).FirstOrDefault();
                    if(unOferta != null)
                    {
                        unOferta.rutaComprobante = ruta;
                        await db.SaveChangesAsync();
                        #region auditoria
                        AuditoriaRacop aud = new AuditoriaRacop();
                        var usrID = Convert.ToInt32(System.Web.HttpContext.Current.Session["idUsuario"].ToString());
                        //var idEmpresa = Convert.ToInt32(System.Web.HttpContext.Current.Session["idEmpresa"].ToString());
                        aud.AddLogCreate(usrID, idEmpresa, " ha actualizado un archivo oferta: " + idObra.ToString() + "- Búho Licitaciones");

                        #endregion
                    }
                }
            }
            catch (Exception)
            {
                #region auditoria
                AuditoriaRacop aud = new AuditoriaRacop();
                var usrID = Convert.ToInt32(System.Web.HttpContext.Current.Session["idUsuario"].ToString());
                //var idEmpresa = Convert.ToInt32(System.Web.HttpContext.Current.Session["idEmpresa"].ToString());
                aud.AddLogCreate(usrID, idEmpresa, " ha generado un error al actualizar un archivo oferta: " + idObra.ToString() + "- Búho Licitaciones");

                #endregion
            }
        }
        public ArchivoViewModels buscarOferta(int? idEmpresa, int? idObra)
        {
            ArchivoViewModels unArchivo = new ArchivoViewModels();
            log4net.Config.XmlConfigurator.Configure(new FileInfo(ruta));
            log.Info("Buscar Oferta de Empresa");
            try
            {
                using (db_meieEntities db = new db_meieEntities())
                {
                    var existe = db.LicOfertaEmpresa
                        .Where(x => x.idEmpresa == idEmpresa && x.idObra == idObra && x.activa == 1)
                        .FirstOrDefault();
                    if (existe != null)
                    {
                        unArchivo.ruta = existe.rutaComprobante;
                    }
                }
            }
            catch (Exception ex)
            {
                log.Error("Error buscar Archivo de Empresa " + ex.Message);
            }
            return unArchivo;
        }
        public bool puedeSubirOferta(int? idObra)
        {
            try
            {
                using(db_meieEntities db=new db_meieEntities())
                {
                    var unObra = db.LicProyectoFecha.Where(x => x.idProyecto == idObra).OrderByDescending(x=>x.idLicProyectoFecha).FirstOrDefault();
                    if(unObra != null)
                    {
                        var fechaControl = DateTime.Now.Date;
                        var fechaComparar = DateTime.Now.Date;
                        var horaControl = DateTime.Now.TimeOfDay;
                        var horaComprar = DateTime.Now.TimeOfDay;
                        if (unObra.fechaCierre.HasValue)
                        {
                            fechaComparar = unObra.fechaCierre.Value;
                            fechaComparar = fechaComparar.AddDays(1);
                        }
                        else
                        {
                            if (unObra.fechaApertura.HasValue)
                            {
                                fechaComparar = unObra.fechaApertura.Value;
                            }
                        }
                        if (fechaControl >= fechaComparar)
                        {
                            return false;
                        }
                        return true;
                    }
                }
            }
            catch(Exception ex)
            {

            }
            return false;
        }    
        /// <summary>
        /// Grabo en la tabla, las empresas que se agreguen para poder subir documentación
        /// </summary>
        /// <param name="datosEmpresa"></param>
        /// <returns></returns>
        public async Task<string> grabarEmpresaHabilitada(EmpresaHabilitadaViewModels datosEmpresa) 
        {
            string value = "";
            log4net.Config.XmlConfigurator.Configure(new FileInfo(ruta));
            log.Info("Una grabarEmpresaHabilidada");
            //faltaria validar si el cuit esta en las empresas de racop con documentacion digital en la obra?
            try
            {
                using (DB_RACOPEntities _db = new DB_RACOPEntities())
                using (db_meieEntities db = new db_meieEntities())
                {
                    var query = db.LicEmpresaHabilitada.Where(x => x.idObra == datosEmpresa.idObra && x.cuit == datosEmpresa.cuit).FirstOrDefault();
                    var existe = _db.rc_Empresa.Where(x => x.cuit == datosEmpresa.cuit).FirstOrDefault();
                    if (query == null)  //agrego empresa habilitada
                    {
                        LicEmpresaHabilitada empresaHabilitada = new LicEmpresaHabilitada();                       
                        empresaHabilitada.fecha = DateTime.Now;
                        empresaHabilitada.cuit = Regex.Replace(datosEmpresa.cuit, "[^0-9]", "");
                        empresaHabilitada.razonSocial = datosEmpresa.razonSocial;
                        empresaHabilitada.idObra = datosEmpresa.idObra;
                        empresaHabilitada.observaciones = true;                     
            
                        db.LicEmpresaHabilitada.Add(empresaHabilitada);
                        await db.SaveChangesAsync();  

                        LicArchivoEmpresa archivosEmpresa = new LicArchivoEmpresa();
                        var archivos = db.LicDocObra.Where(x => x.idObra == datosEmpresa.idObra && x.esFracasada == null).ToList();

                        foreach( var item  in archivos)  //agrego archivos sin presentar
                        {
                            archivosEmpresa.idObra = datosEmpresa.idObra;                            
                            archivosEmpresa.cuit = datosEmpresa.cuit;
                            archivosEmpresa.idRequisito = item.idLicDocObra;
                            archivosEmpresa.nroSobre = item.nroSobre;
                            archivosEmpresa.idEstadoArchivo = 4;
                            archivosEmpresa.tipoOferta = 1;
                            archivosEmpresa.observaciones = "Debe presentar Oferta digital";
                            if (existe != null)
                            {
                                archivosEmpresa.idEmpresa = existe.idEmpresa;
                            }
                            db.LicArchivoEmpresa.Add(archivosEmpresa);
                            await db.SaveChangesAsync();
                        }
                    }
                    #region auditoria
                    AuditoriaOficina aud = new AuditoriaOficina();
                    var usrID = Convert.ToInt32(System.Web.HttpContext.Current.Session["idUsuario"].ToString());

                    var nombreUsuario = db.SegUser.Where(x => x.Id == usrID).FirstOrDefault();
                    aud.AddLogCambios(2, 1, "El usuario " + nombreUsuario.FullName + " ha grabado una empresa habilitada en obra:" + datosEmpresa.idObra + "- Búho Licitaciones", usrID, datosEmpresa.idObra);

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
                    aud.AddLogCambios(2, 1, "El usuario " + nombreUsuario.FullName + " ha generado un error al grabar empresa habilitada en obra: " + datosEmpresa.idObra + "- Búho Licitaciones", usrID, datosEmpresa.idObra);

                    #endregion
                }
                log.Error("Error grabar empresa habilitada", ex);
                return value;
            }
            return value;
        }
        public async Task<string> habilitarSubaMejora(EmpresaHabilitadaViewModels datosEmpresa) 
        {
            string value = "";
            log4net.Config.XmlConfigurator.Configure(new FileInfo(ruta));
            log.Info("Una submejora");
            try
            {
                using (db_meieEntities db = new db_meieEntities())
                using (DB_RACOPEntities _db = new DB_RACOPEntities())
                {
                    var unaEmpresa = _db.rc_Empresa.Where(x => x.idEmpresa == datosEmpresa.idEmpresa).FirstOrDefault();
                    var query = db.LicEmpresaHabilitada.Where(x => x.idObra == datosEmpresa.idObra && x.cuit == datosEmpresa.cuit).FirstOrDefault();
                    
                    if (query != null)
                    {
                        if(datosEmpresa.idEmpresa != null)
                        {
                            query.idEmpresa = datosEmpresa.idEmpresa;
                        }
                        query.mejoraoferta = datosEmpresa.mejoraOferta;
                        db.SaveChangesAsync();
                    }
                    else
                    {
                        LicEmpresaHabilitada empresaHabilitada = new LicEmpresaHabilitada();
                        empresaHabilitada.fecha = DateTime.Now;                       
                        empresaHabilitada.idObra = datosEmpresa.idObra;                       
                        empresaHabilitada.mejoraoferta = datosEmpresa.mejoraOferta;

                        if(unaEmpresa != null)
                        {
                            empresaHabilitada.idEmpresa = unaEmpresa.idEmpresa;
                            empresaHabilitada.cuit = unaEmpresa.cuit;
                            empresaHabilitada.razonSocial = unaEmpresa.razonSocial;
                        }

                        db.LicEmpresaHabilitada.Add(empresaHabilitada);
                        await db.SaveChangesAsync();
                    }
                    #region auditoria
                    AuditoriaOficina aud = new AuditoriaOficina();
                    var usrID = Convert.ToInt32(System.Web.HttpContext.Current.Session["idUsuario"].ToString());

                    var nombreUsuario = db.SegUser.Where(x => x.Id == usrID).FirstOrDefault();
                    aud.AddLogCambios(2, 1, "El usuario " + nombreUsuario.FullName + " ha grabado una submejora:" + datosEmpresa.idObra + "- Búho Licitaciones", usrID, datosEmpresa.idObra);

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
                    aud.AddLogCambios(2, 1, "El usuario " + nombreUsuario.FullName + " ha generado un error al grabar una submejora en obra: " + datosEmpresa.idObra + "- Búho Licitaciones", usrID, datosEmpresa.idObra);

                    #endregion
                }
                log.Error("Error submejora", ex);
                return value;
            }
            return value;
        }
        public ArchivoViewModels descargarArchivoComprobante(int? idArchivo)
        {
            ArchivoViewModels unArchivo = new ArchivoViewModels();
            log4net.Config.XmlConfigurator.Configure(new FileInfo(ruta));
            log.Info("Buscar Oferta de Empresa");
            try
            {
                using (db_meieEntities db = new db_meieEntities())
                {
                    var existe = db.LicOfertaEmpresa
                        .Where(x => x.idOferta == idArchivo)
                        .FirstOrDefault();
                    if (existe != null)
                    {
                        unArchivo.ruta = existe.rutaComprobante;
                    }
                }
            }
            catch (Exception ex)
            {
                log.Error("Error buscar Archivo de Empresa " + ex.Message);
            }
            return unArchivo;
        }
        public async Task<string> generarComprobanteOfertaObs(DatosEmpresaViewModels unEmpresa)
        {
            string filePath = "";
            string nombre = "";
            log4net.Config.XmlConfigurator.Configure(new FileInfo(ruta));
            log.Info("enviarMailOferta");
            try
            {
                using (DB_RACOPEntities db = new DB_RACOPEntities())
                {
                    // Buscar Oferta 
                    ServicioDocumentacion servicioDocumentacion = new ServicioDocumentacion();
                    //List<RequisitoViewModels> listaArchivo = new List<RequisitoViewModels>();
                    //listaArchivo = servicioDocumentacion.listarArchivoPorObraEmpresaObs(unEmpresa.idEmpresa, unEmpresa.idObra, 0, unEmpresa.cuitEmpresa);
                    ServicioLicitacion servicioLicitacion = new ServicioLicitacion();
                    var unObra = servicioLicitacion.buscarUna(unEmpresa.idObra);
                    if (unObra != null)
                    {
                        unEmpresa.nombreObra = unObra.nombreObra;
                    }
                    // Buscar Tipo Mail
                    ServicioEmail servicioEmail = new ServicioEmail();
                    EmailViewModels emailViewModels = new EmailViewModels();
                    emailViewModels = servicioEmail.buscarMail(1);
                    emailViewModels.destino = unEmpresa.emailEmpresa;
                    emailViewModels.copiaMail = unEmpresa.emailCopiaEmpresa;
                    //var lista = string.Empty;
                    //lista = "<ul>";
                    //foreach (var item in listaArchivo)
                    //{
                    //    lista = lista + "<li>";
                    //    lista = lista + "Requisito " + item.nombre + " Archivo: " + item.nombreArchivo;
                    //    lista = lista + "</li>";
                    //}
                    //lista = lista + "</ul>";
                    //emailViewModels.cuerpo = emailViewModels.cuerpo.Replace("#ListaArchivos", lista);
                    emailViewModels.cuerpo = emailViewModels.cuerpo.Replace("#Empresa", unEmpresa.nombreEmpresa);
                    emailViewModels.cuerpo = emailViewModels.cuerpo.Replace("#NombreObra", unEmpresa.nombreObra);
                    ServicioPDF servicio = new ServicioPDF();
                    filePath = ConfigurationSettings.AppSettings["repositorioComprobante"].ToString();
                    //filePath = ConfigurationSettings.AppSettings["repositorioFiles"].ToString();
                    nombre = "Oferta_" + unEmpresa.idObra + unEmpresa.idEmpresa + DateTime.Now.TimeOfDay.Ticks + ".pdf";
                    filePath = filePath + nombre;
                    servicio.generarComprobanteOfertaObser(unEmpresa.idEmpresa, unEmpresa.idObra, filePath, unObra.nombreObra);
                    await grabarOferta(unEmpresa.idObra, unEmpresa.idEmpresa, filePath, 2); //tipoOferta 1- Oferta 2-Observaciones 3-MejoraOferta
                    //await actualizarArchivoOferta(unEmpresa.idEmpresa, unEmpresa.idObra, filePath);
                    // Enviar Mail
                    await enviarNotificacionObservacionAdjunto(unEmpresa.idObra, unEmpresa.idEmpresa);
                    #region auditoria
                    AuditoriaRacop aud = new AuditoriaRacop();
                    var usrID = Convert.ToInt32(System.Web.HttpContext.Current.Session["idUsuario"].ToString());
                    var idEmpresa = Convert.ToInt32(System.Web.HttpContext.Current.Session["idEmpresa"].ToString());
                    aud.AddLogCreate(usrID, idEmpresa, " Genera comprobante Oferta obs: " + unEmpresa.idObra.ToString() + "- Búho Licitaciones");

                    #endregion
                }
            }
            catch (Exception ex)
            {
                #region auditoria
                AuditoriaRacop aud = new AuditoriaRacop();
                var usrID = Convert.ToInt32(System.Web.HttpContext.Current.Session["idUsuario"].ToString());
                var idEmpresa = Convert.ToInt32(System.Web.HttpContext.Current.Session["idEmpresa"].ToString());
                aud.AddLogCreate(usrID, idEmpresa, " error al generar comprobante Oferta obs: " + unEmpresa.idObra.ToString() + "- Búho Licitaciones");

                #endregion
                log.Error("enviarMailOferta", ex);
                return nombre;
            }
            return nombre;
        }
        public async Task enviarNotificacionObservacionAdjunto(int? idObra, int? idEmpresa)
        {
            log4net.Config.XmlConfigurator.Configure(new FileInfo(ruta));
            log.Info("Envia mail Observaciones Notificacion con Adjunto");
            var nombreObra = "";
            int? idOrganismo = 0;
            var mailEmpresa = "";
            var mailOrganismo = "";
            var archivoAdjunto = "";
            using (db_meieEntities db = new db_meieEntities())
            {
                var unObra = db.PryProyecto.Where(x => x.Id == idObra).FirstOrDefault();
                if (unObra != null)
                {
                    nombreObra = unObra.Nombre;
                    idOrganismo = unObra.PryOrganismoEjecutor_Id;

                    var unOrganismo = db.PryOrganismosEjecutores.Where(x => x.Id == idOrganismo).FirstOrDefault();
                    if (unOrganismo != null)
                    {
                        if (!string.IsNullOrEmpty(unOrganismo.mailOrganisimo))
                        {
                            mailOrganismo = unOrganismo.mailOrganisimo;
                            log.Info("Envia mail Notificacion con Adjunto a Organismo: " + mailOrganismo);
                        }
                    }
                    using (DB_RACOPEntities db2 = new DB_RACOPEntities())
                    {
                        var unEmpresa = db2.rc_Empresa.Where(x => x.idEmpresa == idEmpresa).FirstOrDefault();
                        if (unEmpresa != null)
                        {
                            if (!string.IsNullOrEmpty(unEmpresa.mailEspecial))
                            {
                                mailEmpresa = mailEmpresa + unEmpresa.mailEspecial + ";";
                                log.Info("Envia mail Notificacion con Adjunto a: " + mailEmpresa);
                            }
                        }
                    }
                }
                var unAdjunto = db.LicOfertaEmpresa.Where(x => x.idEmpresa == idEmpresa && x.idObra == idObra && x.tipoOferta == 2 && x.activa == 1).FirstOrDefault();
                if (unAdjunto != null)
                {
                    archivoAdjunto = unAdjunto.rutaComprobante;
                }
            }

            ServicioEmail servicioEmail = new ServicioEmail();
            var emailViewModels = servicioEmail.buscarMail(6);
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
        }

    }

}

