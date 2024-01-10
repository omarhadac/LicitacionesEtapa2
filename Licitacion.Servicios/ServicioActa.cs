using Licitacion.Servicios.Utiles;
using Licitaciones.BaseDato;
using Licitaciones.ViewModels;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;


namespace Licitacion.Servicios
{
    public class ServicioActa
    {
        private static readonly log4net.ILog log = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
        private static string ruta = System.Web.HttpRuntime.AppDomainAppPath + "\\bin\\log4net.config";

        public ActaViewModels buscarActaGenerica(int? idActa, int? idObra)
        {
            ActaViewModels unActa = new ActaViewModels();
            log4net.Config.XmlConfigurator.Configure(new FileInfo(ruta));
            log.Info("Buscar Acta Generica");
            try
            {
                using (db_meieEntities db = new db_meieEntities())
                {
                    var existe = db.LicActaGenerica
                        .Where(x => x.idLicActaGenerica == idActa)
                        .FirstOrDefault();
                    if (existe != null)
                    {
                        unActa.textoEncabezado = existe.actaEncabezado;
                    }
                    var unObra = (from lic in db.PryProyecto
                                  join edo in db.PryEstadoEtapa
                                  on lic.GrlTypeState_Id equals edo.Id
                                  join pyl in db.PryLicitacion
                                  on lic.Id equals pyl.IdPryProyecto
                                      into pylo
                                  from pyj in pylo.DefaultIfEmpty()
                                  where lic.Id == idObra
                                  select new licitacionGrillaViewModels
                                  {
                                      nombreObra = lic.Nombre,
                                      idObra = lic.Id,
                                      montoObra = pyj.MontoOficial,
                                      nroExpediente = lic.Expediente,
                                      nroLicitacion = lic.numeroLicitacion,
                                      plazo = pyj.Plazo,
                                      tipoPlazo = pyj.TipoPlazo
                                  }).FirstOrDefault();

                    if (unObra != null)
                    {
                        unActa.nombreObra = string.IsNullOrEmpty(unObra.nombreObra) ? "" : unObra.nombreObra;
                        unActa.nroExpediente = string.IsNullOrEmpty(unObra.nroExpediente) ? "" : unObra.nroExpediente;
                        unActa.nroLicitacion = unObra.nroLicitacion;
                        if (unObra.tipoPlazo.HasValue)
                        {
                            if (unObra.tipoPlazo.Value == 0)
                            {
                                unActa.plazoEjecucion = unObra.plazo + " Días ";
                            }
                            else
                            {
                                unActa.plazoEjecucion = unObra.plazo + " Mes/es ";
                            }
                        }
                        else
                        {
                            unActa.plazoEjecucion = "";
                        }
                        if (unObra.montoObra.HasValue)
                        {
                            unActa.montoObra = unObra.montoObra;
                        }
                        else
                        {
                            unActa.montoObra = 0;
                        }

                        unActa.textoEncabezado = unActa.textoEncabezado.Replace("#plazo", unActa.plazoEjecucion);
                        if (!string.IsNullOrEmpty(unActa.nombreObra))
                        {
                            unActa.textoEncabezado = unActa.textoEncabezado.Replace("#nombreObra", unActa.nombreObra);
                        }
                        else
                        {
                            unActa.textoEncabezado = unActa.textoEncabezado.Replace("#nombreObra", "");
                        }
                        if (!string.IsNullOrEmpty(unActa.nroLicitacion))
                        {
                            unActa.textoEncabezado = unActa.textoEncabezado.Replace("#nroLicitacion", unActa.nroLicitacion);
                        }
                        else
                        {
                            unActa.textoEncabezado = unActa.textoEncabezado.Replace("#nroLicitacion", "");
                        }
                        if (!string.IsNullOrEmpty(unActa.nroExpediente))
                        {
                            unActa.textoEncabezado = unActa.textoEncabezado.Replace("#nroExpediente", unActa.nroExpediente);
                        }
                        else
                        {
                            unActa.textoEncabezado = unActa.textoEncabezado.Replace("#nroExpediente", "");
                        }
                        if (!string.IsNullOrEmpty(unActa.montoObraString))
                        {
                            unActa.textoEncabezado = unActa.textoEncabezado.Replace("#montoPresupuesto ", unActa.montoObraString);
                        }
                        else
                        {
                            unActa.textoEncabezado = unActa.textoEncabezado.Replace("#montoPresupuesto ", unActa.montoObraString);
                        }


                        var unDomicilio = (from lic in db.LicProyecto
                                           where lic.idProyecto == idObra
                                           select new ActaFormalViewModels
                                           {
                                               domicilioApertura = lic.domicilioApertura
                                           }).FirstOrDefault();
                        if (unDomicilio != null)
                        {
                            if (!string.IsNullOrEmpty(unDomicilio.domicilioApertura))
                            {
                                unActa.textoEncabezado = unActa.textoEncabezado.Replace("#domicilioApertura", unDomicilio.domicilioApertura);
                            }
                        }
                        var listFecha = (from lic in db.LicProyectoFecha
                                         where lic.idProyecto == idObra && lic.esFracasada == null
                                         select new ActaFormalViewModels
                                         {
                                             idFecha = lic.idLicProyectoFecha,
                                             fechaApertura = lic.fechaApertura,
                                             horaApertura = lic.horaApertura,
                                         }).ToList();
                        if (listFecha != null && listFecha.Count > 0)
                        {
                            var fechaApertura = "";
                            var unFecha = listFecha.OrderByDescending(x => x.idFecha).FirstOrDefault();
                            if (unFecha.fechaApertura.HasValue)
                            {
                                var CurrentCulture = Thread.CurrentThread.CurrentCulture; //Store the current culture
                                Thread.CurrentThread.CurrentCulture = new System.Globalization.CultureInfo("es-AR"); //Set a new culture                 
                                unActa.textoEncabezado = unActa.textoEncabezado.Replace("#fechaActa", unFecha.fechaApertura.Value.ToLongDateString());
                                Thread.CurrentThread.CurrentCulture = CurrentCulture; //Restore the environment.
                            }
                            if (!string.IsNullOrEmpty(unFecha.horaApertura))
                            {
                                fechaApertura = fechaApertura + ", a las " + unFecha.horaApertura;
                            }
                            unActa.textoEncabezado = unActa.textoEncabezado.Replace("#fechaActa", fechaApertura);
                        }
                    }
                    #region auditoria
                    AuditoriaOficina aud = new AuditoriaOficina();
                    var usrID = Convert.ToInt32(System.Web.HttpContext.Current.Session["idUsuario"].ToString());

                    var nombreUsuario = db.SegUser.Where(x => x.Id == usrID).FirstOrDefault();
                    aud.AddLogCambios(2, 1, "El usuario " + nombreUsuario.FullName + " ha realizado una busqueda de Acta para la obra " + idObra + "- Búho Licitaciones", usrID, idObra);

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
                    aud.AddLogCambios(2, 1, "El usuario " + nombreUsuario.FullName + " ha generado un error al generar el acta de obra: " + idObra + "- Búho Licitaciones", usrID, idObra);

                    #endregion
                }
                log.Error("Error buscar Acta Generica " + ex.Message);
            }
            return unActa;
        }
        public ActaViewModels buscarActaFormal(int? idActa, int? idObra)
        {
            ActaViewModels unActa = new ActaViewModels();
            log4net.Config.XmlConfigurator.Configure(new FileInfo(ruta));
            log.Info("Buscar Acta Generica");
            try
            {
                using (db_meieEntities db = new db_meieEntities())
                {
                    string fecha = DateTime.Now.ToLongDateString();
                    var existe = db.LicActaGenerica
                        .Where(x => x.idLicActaGenerica == idActa)
                        .FirstOrDefault();
                    if (existe != null)
                    {
                        unActa.textoFormal = existe.actaFormal;
                        unActa.textoOferta = existe.actaOferta;
                        unActa.textoCierre = existe.actaCierre;
                    }
                    var unObra = (from lic in db.LicProyecto
                                  where lic.idProyecto == idObra
                                  select new ActaFormalViewModels
                                  {
                                      domicilioApertura = lic.domicilioApertura
                                  }).FirstOrDefault();

                    if (unObra != null)
                    {
                        //unActa.textoFormal = unActa.textoFormal.Replace("#fechaActa", fecha);
                        //unActa.textoFormal = unActa.textoFormal.Replace("#domicilioApertura", unObra.domicilioApertura);
                    }
                    unActa.textoCierre = unActa.textoCierre + " " + DateTime.Now.Hour + " Horas " + DateTime.Now.Minute + " Minutos";
                    var textoOferta = buscarOfertas(idObra);
                    var textoCuadoResumen = buscarCuadroResumen(idObra);
                    var textoCuadroOferta = buscarCuadroOferta(idObra);

                    var listaSobres = db.LicDocSobres.Where(x => x.idObra == idObra && x.esFracasada == null).ToList();
                    var sobres = "";
                    foreach (var item in listaSobres)
                    {
                        var unRequisito = db.LicDocObra.Where(x => x.nroSobre == item.numero && x.esFracasada == null).FirstOrDefault();
                        if(unRequisito != null)
                        {
                            if (item.nombre != null && item.nombre != "")
                            {
                                sobres += $"N° {item.numero} - {item.nombre}, ";
                            }
                            else
                            {
                                sobres += $"N° {item.numero}, ";
                            }
                        }
                    }
                    sobres = sobres.TrimEnd(',', ' ');
                    sobres += ":";

                    unActa.textoOferta = unActa.textoOferta.Replace("#sobres", sobres);
                    unActa.textoOferta += textoOferta;
                    unActa.cuadroOferta = textoCuadroOferta;
                    unActa.cuadroResumen = textoCuadoResumen;
                    #region auditoria
                    AuditoriaOficina aud = new AuditoriaOficina();
                    var usrID = Convert.ToInt32(System.Web.HttpContext.Current.Session["idUsuario"].ToString());

                    var nombreUsuario = db.SegUser.Where(x => x.Id == usrID).FirstOrDefault();
                    aud.AddLogCambios(2, 1, "El usuario " + nombreUsuario.FullName + " ha realizado una busqueda de Acta Formal para la obra " + idObra + "- Búho Licitaciones", usrID, idObra);

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
                    aud.AddLogCambios(2, 1, "El usuario " + nombreUsuario.FullName + " ha generado un error al generar el acta de obra: " + idObra + "- Búho Licitaciones", usrID, idObra);

                    #endregion
                }
                log.Error("Error buscar Acta Generica " + ex.Message);
            }
            return unActa;
        }
        private string buscarOfertas(int? idObra)
        {
            string value = "";
            ActaViewModels unActa = new ActaViewModels();
            log4net.Config.XmlConfigurator.Configure(new FileInfo(ruta));
            try
            {
                using (db_meieEntities db = new db_meieEntities())
                {
                    var listaOferta = db.LicOfertaEmpresa.Where(x => x.idObra == idObra && x.activa == 1 && x.esFracasada == null && x.tipoOferta == 1).ToList();

                    int nroOferta = 1;
                    foreach (var item in listaOferta)
                    {
                        using (DB_RACOPEntities db1 = new DB_RACOPEntities())
                        {
                            var unEmpresa = db1.rc_Empresa.Where(x => x.idEmpresa == item.idEmpresa).FirstOrDefault();
                            if (unEmpresa != null)
                            {
                                value = value + "<br><p><b>Oferta Nro " + nroOferta + ": " + unEmpresa.razonSocial + "</b></p>";
                                var listaRequisitos1 = db.LicDocObra.Where(x => x.idObra == idObra && x.nroSobre == 1 && x.esFracasada == null).ToList();
                                var sobre1 = db.LicDocSobres.Where(x => x.idObra == idObra && x.numero == 1 && x.esFracasada == null).FirstOrDefault();
                                if ((listaRequisitos1 != null) && (listaRequisitos1.Count > 0))
                                {
                                    value = value + "<b>Sobre N° 1 " + sobre1.nombre + ": </b><ul>";
                                }
                                foreach (var unSobre1 in listaRequisitos1)
                                {
                                    var aprobado = false;
                                    var archivoObservado = "";
                                    var unArchivo = db.LicArchivoEmpresa
                                        .Where(x => x.idRequisito == unSobre1.idLicDocObra && x.idObra == idObra && x.idEmpresa == unEmpresa.idEmpresa && x.esFracasada == null)
                                        .FirstOrDefault();
                                    if (unArchivo != null)
                                    {
                                        var unReq = db.LicDocGeneral.Where(x => x.idLicDocGeneral == unSobre1.idDocumentacion).FirstOrDefault();
                                        if (unReq != null)
                                        {
                                            if (unArchivo.idEstadoArchivo == 3)
                                            {
                                                archivoObservado = "<li>" + archivoObservado + unReq.nombre + "<b> Presentado </b>" + unArchivo.observaciones + " </li> ";
                                            }
                                            else
                                            {
                                                if (unArchivo.idEstadoArchivo == 5)
                                                {
                                                    archivoObservado = "<li>" + archivoObservado + unReq.nombre + "<b> Rechazado:</b> " + unArchivo.observaciones + "</li>";
                                                }
                                                else
                                                {
                                                    if (unArchivo.idEstadoArchivo == 4)
                                                    {
                                                        archivoObservado = "<li>" + archivoObservado + unReq.nombre + "<b> Observado:</b> " + unArchivo.observaciones + "</li>";
                                                    }
                                                    else
                                                    {
                                                        archivoObservado = "<li>" + archivoObservado + unReq.nombre + "<b> No Presentado </b>" + unArchivo.observaciones + "</li>";
                                                    }
                                                }
                                            }
                                        }
                                    }
                                    else
                                    {
                                        var unReq = db.LicDocGeneral.Where(x => x.idLicDocGeneral == unSobre1.idDocumentacion).FirstOrDefault();
                                        if (unReq != null)
                                        {
                                            archivoObservado = "<li>" + archivoObservado + unReq.nombre + " <b> No ha sido presentado</b>" + "</li>";
                                        }
                                    }
                                    value = value + archivoObservado;
                                }
                                value = value + "</ul>";
                                var listaRequisitos2 = db.LicDocObra.Where(x => x.idObra == idObra && x.nroSobre == 2 && x.esFracasada == null).ToList();
                                var sobre2 = db.LicDocSobres.Where(x => x.idObra == idObra && x.numero == 2 && x.esFracasada == null).FirstOrDefault();
                                if ((listaRequisitos2 != null) && (listaRequisitos2.Count > 0))
                                {
                                    value = value + "<b>Sobre N° 2 " + sobre2.nombre + ": </b><ul>";
                                }
                                foreach (var unSobre2 in listaRequisitos2)
                                {
                                    var archivoObservado = "";
                                    var unArchivo = db.LicArchivoEmpresa
                                        .Where(x => x.idRequisito == unSobre2.idLicDocObra && x.idObra == idObra && x.idEmpresa == unEmpresa.idEmpresa && x.esFracasada == null)
                                        .FirstOrDefault();
                                    if (unArchivo != null)
                                    {
                                        var unReq = db.LicDocGeneral.Where(x => x.idLicDocGeneral == unSobre2.idDocumentacion).FirstOrDefault();
                                        if (unReq != null)
                                        {
                                            if (unArchivo.idEstadoArchivo == 3)
                                            {
                                                archivoObservado = "<li>" + archivoObservado + unReq.nombre + " <b> Presentado </b>" + unArchivo.observaciones + "</li>";
                                            }
                                            else
                                            {
                                                if (unArchivo.idEstadoArchivo == 5)
                                                {
                                                    archivoObservado = "<li>" + archivoObservado + unReq.nombre + " <b> Rechazado:</b> " + unArchivo.observaciones + "</li>";
                                                }
                                                else
                                                {
                                                    if (unArchivo.idEstadoArchivo == 4)
                                                    {
                                                        archivoObservado = "<li>" + archivoObservado + unReq.nombre + " <b> Observado:</b> " + unArchivo.observaciones + "</li>";
                                                    }
                                                    else
                                                    {
                                                        archivoObservado = "<li>" + archivoObservado + unReq.nombre + " <b> No Presentado </b>" + unArchivo.observaciones + "</li>";
                                                    }
                                                }
                                            }
                                        }
                                    }
                                    else
                                    {
                                        var unReq = db.LicDocGeneral.Where(x => x.idLicDocGeneral == unSobre2.idDocumentacion).FirstOrDefault();
                                        if (unReq != null)
                                        {
                                            archivoObservado = "<li>" + archivoObservado + unReq.nombre + " <b> No ha sido presentado</b>" + "</li>";
                                        }
                                    }
                                    value = value + archivoObservado;
                                }
                                value = value + "</ul>";
                                var listaRequisitos3 = db.LicDocObra.Where(x => x.idObra == idObra && x.nroSobre == 3 && x.esFracasada == null).ToList();
                                var sobre3 = db.LicDocSobres.Where(x => x.idObra == idObra && x.numero == 3 && x.esFracasada == null).FirstOrDefault();
                                if ((listaRequisitos3 != null) && (listaRequisitos3.Count > 0))
                                {
                                    value = value + "<b>Sobre N° 3 " + sobre3.nombre + ": </b><ul>";
                                }
                                foreach (var unSobre3 in listaRequisitos3)
                                {
                                    var aprobado = false;
                                    var archivoObservado = "";
                                    var unArchivo = db.LicArchivoEmpresa
                                        .Where(x => x.idRequisito == unSobre3.idLicDocObra && x.idObra == idObra && x.idEmpresa == unEmpresa.idEmpresa && x.esFracasada == null)
                                        .FirstOrDefault();
                                    if (unArchivo != null)
                                    {
                                        var unReq = db.LicDocGeneral.Where(x => x.idLicDocGeneral == unSobre3.idDocumentacion).FirstOrDefault();
                                        if (unReq != null)
                                        {
                                            if (unArchivo.idEstadoArchivo == 3)
                                            {
                                                archivoObservado = "<li>" + archivoObservado + unReq.nombre + " <b> Presentado </b>" + unArchivo.observaciones + "</li>";
                                            }
                                            else
                                            {
                                                if (unArchivo.idEstadoArchivo == 5)
                                                {
                                                    archivoObservado = "<li>" + archivoObservado + unReq.nombre + " <b> Rechazado:</b> " + unArchivo.observaciones + "</li>";
                                                }
                                                else
                                                {
                                                    if (unArchivo.idEstadoArchivo == 4)
                                                    {
                                                        archivoObservado = "<li>" + archivoObservado + unReq.nombre + " <b> Observado:</b> " + unArchivo.observaciones + "</li>";
                                                    }
                                                    else
                                                    {
                                                        archivoObservado = "<li>" + archivoObservado + unReq.nombre + " <b> No Presentado </b>" + unArchivo.observaciones + "</li>";
                                                    }
                                                }
                                            }
                                        }
                                    }
                                    else
                                    {
                                        var unReq = db.LicDocGeneral.Where(x => x.idLicDocGeneral == unSobre3.idDocumentacion).FirstOrDefault();
                                        if (unReq != null)
                                        {
                                            archivoObservado = "<li>" + archivoObservado + unReq.nombre + " <b> No ha sido presentado</b>" + "</li>";
                                        }
                                    }

                                    value = value + archivoObservado;
                                }
                                value = value + "</ul>";
                                var listaRequisitos4 = db.LicDocObra.Where(x => x.idObra == idObra && x.nroSobre == 4 && x.esFracasada == null).ToList();
                                var sobre4 = db.LicDocSobres.Where(x => x.idObra == idObra && x.numero == 4 && x.esFracasada == null).FirstOrDefault();
                                if ((listaRequisitos4 != null) && (listaRequisitos4.Count > 0))
                                {
                                    value = value + "<b>Sobre N° 3 " + sobre4.nombre + ": </b><ul>";
                                }
                                foreach (var unSobre4 in listaRequisitos4)
                                {
                                    var aprobado = false;
                                    var archivoObservado = "";
                                    var unArchivo = db.LicArchivoEmpresa
                                        .Where(x => x.idRequisito == unSobre4.idLicDocObra && x.idObra == idObra && x.idEmpresa == unEmpresa.idEmpresa && x.esFracasada == null)
                                        .FirstOrDefault();
                                    if (unArchivo != null)
                                    {
                                        var unReq = db.LicDocGeneral.Where(x => x.idLicDocGeneral == unSobre4.idDocumentacion).FirstOrDefault();
                                        if (unReq != null)
                                        {
                                            if (unArchivo.idEstadoArchivo == 3)
                                            {
                                                archivoObservado = "<li>" + archivoObservado + unReq.nombre + " <b> Presentado </b>" + unArchivo.observaciones + "</li>";
                                            }
                                            else
                                            {
                                                if (unArchivo.idEstadoArchivo == 5)
                                                {
                                                    archivoObservado = "<li>" + archivoObservado + unReq.nombre + " <b> Rechazado:</b> " + unArchivo.observaciones + "</li>";
                                                }
                                                else
                                                {
                                                    if (unArchivo.idEstadoArchivo == 4)
                                                    {
                                                        archivoObservado = "<li>" + archivoObservado + unReq.nombre + " <b> Observado:</b> " + unArchivo.observaciones + "</li>";
                                                    }
                                                    else
                                                    {
                                                        archivoObservado = "<li>" + archivoObservado + unReq.nombre + " <b> No Presentado </b>" + unArchivo.observaciones + "</li>";
                                                    }
                                                }
                                            }
                                        }
                                    }
                                    else
                                    {
                                        var unReq = db.LicDocGeneral.Where(x => x.idLicDocGeneral == unSobre4.idDocumentacion).FirstOrDefault();
                                        if (unReq != null)
                                        {
                                            archivoObservado = "<li>" + archivoObservado + unReq.nombre + " <b> No ha sido presentado</b>" + "</li>";
                                        }
                                    }

                                    value = value + archivoObservado;
                                }
                            }
                            nroOferta = nroOferta + 1;
                        }
                    }
                    value = value + "<br>";
                    var listaEmpresaAgregada = db.LicEmpresaHabilitada.Where(x => x.idObra == idObra && x.observaciones == true && x.esFracasada == null).ToList();
                    foreach (var item in listaEmpresaAgregada)
                    {
                        #region ArchivosAgregados

                        value = value + "<br><p><b>Oferta Nro " + nroOferta + ": " + item.razonSocial + "</b></p>";
                        var listaRequisitos1 = db.LicDocObra.Where(x => x.idObra == idObra && x.nroSobre == 1 && x.esFracasada == null).ToList();
                        var sobre1 = db.LicDocSobres.Where(x => x.idObra == idObra && x.numero == 1 && x.esFracasada == null).FirstOrDefault();
                        if ((listaRequisitos1 != null) && (listaRequisitos1.Count > 0))
                        {
                            value = value + "<b>Sobre N° 1 " + sobre1.nombre + ": </b><ul>";
                        }
                        foreach (var unSobre1 in listaRequisitos1)
                        {
                            var archivoObservado = "";
                            var unArchivoObs = db.LicArchivoEmpresa
                                .Where(x => x.idRequisito == unSobre1.idLicDocObra && x.idObra == idObra && x.cuit == item.cuit && x.esFracasada == null)
                                .FirstOrDefault();
                            if (unArchivoObs != null)
                            {
                                var unReq = db.LicDocGeneral.Where(x => x.idLicDocGeneral == unSobre1.idDocumentacion).FirstOrDefault();
                                if (unReq != null)
                                {
                                    if (unArchivoObs.idEstadoArchivo == 3)
                                    {
                                        archivoObservado = "<li>" + archivoObservado + unReq.nombre + "<b> Presentado </b>" + unArchivoObs.observaciones + " </li> ";
                                    }
                                    else
                                    {
                                        if (unArchivoObs.idEstadoArchivo == 5)
                                        {
                                            archivoObservado = "<li>" + archivoObservado + unReq.nombre + "<b> Rechazado:</b> " + unArchivoObs.observaciones + "</li>";
                                        }
                                        else
                                        {
                                            if (unArchivoObs.idEstadoArchivo == 4)
                                            {
                                                archivoObservado = "<li>" + archivoObservado + unReq.nombre + "<b> Observado:</b> " + unArchivoObs.observaciones + "</li>";
                                            }
                                            else
                                            {
                                                archivoObservado = "<li>" + archivoObservado + unReq.nombre + "<b> No Presentado </b>" + unArchivoObs.observaciones + "</li>";
                                            }
                                        }
                                    }
                                }
                            }
                            else
                            {
                                var unReq = db.LicDocGeneral.Where(x => x.idLicDocGeneral == unSobre1.idDocumentacion).FirstOrDefault();
                                if (unReq != null)
                                {
                                    archivoObservado = "<li>" + archivoObservado + unReq.nombre + " <b> No ha sido presentado</b>" + "</li>";
                                }
                            }
                            value = value + archivoObservado;
                        }
                        value = value + "</ul>";
                        var listaRequisitos2 = db.LicDocObra.Where(x => x.idObra == idObra && x.nroSobre == 2 && x.esFracasada == null).ToList();
                        var sobre2 = db.LicDocSobres.Where(x => x.idObra == idObra && x.numero == 2 && x.esFracasada == null).FirstOrDefault();
                        if ((listaRequisitos2 != null) && (listaRequisitos2.Count > 0))
                        {
                            value = value + "<b>Sobre N° 2 " + sobre2.nombre + ": </b><ul>";
                        }
                        foreach (var unSobre2 in listaRequisitos2)
                        {
                            var archivoObservado = "";
                            var unArchivoObs = db.LicArchivoEmpresa
                                .Where(x => x.idRequisito == unSobre2.idLicDocObra && x.idObra == idObra && x.cuit == item.cuit && x.esFracasada == null)
                                .FirstOrDefault();
                            if (unArchivoObs != null)
                            {
                                var unReq = db.LicDocGeneral.Where(x => x.idLicDocGeneral == unSobre2.idDocumentacion).FirstOrDefault();
                                if (unReq != null)
                                {
                                    if (unArchivoObs.idEstadoArchivo == 3)
                                    {
                                        archivoObservado = "<li>" + archivoObservado + unReq.nombre + " <b> Presentado </b>" + unArchivoObs.observaciones + "</li>";
                                    }
                                    else
                                    {
                                        if (unArchivoObs.idEstadoArchivo == 5)
                                        {
                                            archivoObservado = "<li>" + archivoObservado + unReq.nombre + " <b> Rechazado:</b> " + unArchivoObs.observaciones + "</li>";
                                        }
                                        else
                                        {
                                            if (unArchivoObs.idEstadoArchivo == 4)
                                            {
                                                archivoObservado = "<li>" + archivoObservado + unReq.nombre + " <b> Observado:</b> " + unArchivoObs.observaciones + "</li>";
                                            }
                                            else
                                            {
                                                archivoObservado = "<li>" + archivoObservado + unReq.nombre + " <b> No Presentado </b>" + unArchivoObs.observaciones + "</li>";
                                            }
                                        }
                                    }
                                }
                            }
                            else
                            {
                                var unReq = db.LicDocGeneral.Where(x => x.idLicDocGeneral == unSobre2.idDocumentacion).FirstOrDefault();
                                if (unReq != null)
                                {
                                    archivoObservado = "<li>" + archivoObservado + unReq.nombre + " <b> No ha sido presentado</b>" + "</li>";
                                }
                            }
                            value = value + archivoObservado;
                        }
                        value = value + "</ul>";
                        var listaRequisitos3 = db.LicDocObra.Where(x => x.idObra == idObra && x.nroSobre == 3 && x.esFracasada == null).ToList();
                        var sobre3 = db.LicDocSobres.Where(x => x.idObra == idObra && x.numero == 3 && x.esFracasada == null).FirstOrDefault();
                        if ((listaRequisitos3 != null) && (listaRequisitos3.Count > 0))
                        {
                            value = value + "<b>Sobre N° 3 " + sobre3.nombre + ": </b><ul>";
                        }
                        foreach (var unSobre3 in listaRequisitos3)
                        {
                            var archivoObservado = "";
                            var unArchivoObs = db.LicArchivoEmpresa
                                .Where(x => x.idRequisito == unSobre3.idLicDocObra && x.idObra == idObra && x.cuit == item.cuit && x.esFracasada == null)
                                .FirstOrDefault();
                            if (unArchivoObs != null)
                            {
                                var unReq = db.LicDocGeneral.Where(x => x.idLicDocGeneral == unSobre3.idDocumentacion).FirstOrDefault();
                                if (unReq != null)
                                {
                                    if (unArchivoObs.idEstadoArchivo == 3)
                                    {
                                        archivoObservado = "<li>" + archivoObservado + unReq.nombre + " <b> Presentado </b>" + unArchivoObs.observaciones + "</li>";
                                    }
                                    else
                                    {
                                        if (unArchivoObs.idEstadoArchivo == 5)
                                        {
                                            archivoObservado = "<li>" + archivoObservado + unReq.nombre + " <b> Rechazado:</b> " + unArchivoObs.observaciones + "</li>";
                                        }
                                        else
                                        {
                                            if (unArchivoObs.idEstadoArchivo == 4)
                                            {
                                                archivoObservado = "<li>" + archivoObservado + unReq.nombre + " <b> Observado:</b> " + unArchivoObs.observaciones + "</li>";
                                            }
                                            else
                                            {
                                                archivoObservado = "<li>" + archivoObservado + unReq.nombre + " <b> No Presentado </b>" + unArchivoObs.observaciones + "</li>";
                                            }
                                        }
                                    }
                                }
                            }
                            else
                            {
                                var unReq = db.LicDocGeneral.Where(x => x.idLicDocGeneral == unSobre3.idDocumentacion).FirstOrDefault();
                                if (unReq != null)
                                {
                                    archivoObservado = "<li>" + archivoObservado + unReq.nombre + " <b> No ha sido presentado</b>" + "</li>";
                                }
                            }

                            value = value + archivoObservado;
                        }
                        value = value + "</ul>";
                        var listaRequisitos4 = db.LicDocObra.Where(x => x.idObra == idObra && x.nroSobre == 4 && x.esFracasada == null).ToList();
                        var sobre4 = db.LicDocSobres.Where(x => x.idObra == idObra && x.numero == 4 && x.esFracasada == null).FirstOrDefault();
                        if ((listaRequisitos4 != null) && (listaRequisitos4.Count > 0))
                        {
                            value = value + "<b>Sobre N° 3 " + sobre4.nombre + ": </b><ul>";
                        }
                        foreach (var unSobre4 in listaRequisitos4)
                        {
                            var archivoObservado = "";
                            var unArchivoObs = db.LicArchivoEmpresa
                                .Where(x => x.idRequisito == unSobre4.idLicDocObra && x.idObra == idObra && x.cuit == item.cuit && x.esFracasada == null)
                                .FirstOrDefault();
                            if (unArchivoObs != null)
                            {
                                var unReq = db.LicDocGeneral.Where(x => x.idLicDocGeneral == unSobre4.idDocumentacion).FirstOrDefault();
                                if (unReq != null)
                                {
                                    if (unArchivoObs.idEstadoArchivo == 3)
                                    {
                                        archivoObservado = "<li>" + archivoObservado + unReq.nombre + " <b> Presentado </b>" + unArchivoObs.observaciones + "</li>";
                                    }
                                    else
                                    {
                                        if (unArchivoObs.idEstadoArchivo == 5)
                                        {
                                            archivoObservado = "<li>" + archivoObservado + unReq.nombre + " <b> Rechazado:</b> " + unArchivoObs.observaciones + "</li>";
                                        }
                                        else
                                        {
                                            if (unArchivoObs.idEstadoArchivo == 4)
                                            {
                                                archivoObservado = "<li>" + archivoObservado + unReq.nombre + " <b> Observado:</b> " + unArchivoObs.observaciones + "</li>";
                                            }
                                            else
                                            {
                                                archivoObservado = "<li>" + archivoObservado + unReq.nombre + " <b> No Presentado </b>" + unArchivoObs.observaciones + "</li>";
                                            }
                                        }
                                    }
                                }
                            }
                            else
                            {
                                var unReq = db.LicDocGeneral.Where(x => x.idLicDocGeneral == unSobre4.idDocumentacion).FirstOrDefault();
                                if (unReq != null)
                                {
                                    archivoObservado = "<li>" + archivoObservado + unReq.nombre + " <b> No ha sido presentado</b>" + "</li>";
                                }
                            }

                            value = value + archivoObservado;
                        }


                        #endregion
                    }
                }
            }
            catch (Exception ex)
            {

            }
            return value;
        }
        private string buscarCuadroResumen(int? idObra)
        {
            string value = "<table class=\"table table-bordered\"><tbody><tr><td>SOBRES </td><td> REQUISITOS </td>";
            int indice = 1;

            List<Dictionary<int, string>> detalleSobre = new List<Dictionary<int, string>>();
            List<Dictionary<int, string>> columnas = new List<Dictionary<int, string>>();

            ActaViewModels unActa = new ActaViewModels();

            try
            {
                using (db_meieEntities db = new db_meieEntities())
                {
                    var listaOferta = db.LicOfertaEmpresa.Where(x => x.idObra == idObra && x.activa == 1 && x.esFracasada == null).ToList();

                    int nroOferta = 1;
                    foreach (var item in listaOferta)
                    {
                        using (DB_RACOPEntities db1 = new DB_RACOPEntities())
                        {
                            var unEmpresa = db1.rc_Empresa.Where(x => x.idEmpresa == item.idEmpresa).FirstOrDefault();
                            if (unEmpresa != null)
                            {
                                value = value + "<td>Oferta Nro " + nroOferta + ": " + unEmpresa.razonSocial + "</td>";


                            }
                            nroOferta = nroOferta + 1;
                        }
                    }

                    var listaEmpresaAgregada = db.LicEmpresaHabilitada.Where(x => x.idObra == idObra && x.observaciones == true && x.esFracasada == null).ToList();
                    foreach (var item in listaEmpresaAgregada)
                    {
                        #region ArchivosAgregados

                        value = value + "<td>Oferta Nro " + nroOferta + ": " + item.razonSocial + "</td>";
                        var listaRequisitos1 = db.LicDocObra.Where(x => x.idObra == idObra && x.nroSobre == 1 && x.esFracasada == null).ToList();
                        var sobre1 = db.LicDocSobres.Where(x => x.idObra == idObra && x.numero == 1 && x.esFracasada == null).FirstOrDefault();
                        if ((listaRequisitos1 != null) && (listaRequisitos1.Count > 0))
                        {
                            Dictionary<int, string> nuevoDiccionario = new Dictionary<int, string>();
                            nuevoDiccionario.Add(indice, "<td>Sobre N° 1 " + sobre1.nombre + "</td>");
                            detalleSobre.Add(nuevoDiccionario);
                        }
                        var cuantosRequisitos = listaRequisitos1.Count();
                        foreach (var unSobre1 in listaRequisitos1)
                        {

                            var archivoObservado = "";
                            var unArchivoObs = db.LicArchivoEmpresa
                                .Where(x => x.idRequisito == unSobre1.idLicDocObra && x.idObra == idObra && x.cuit == item.cuit && x.esFracasada == null)
                                .FirstOrDefault();
                            if (unArchivoObs != null)
                            {
                                var unReq = db.LicDocGeneral.Where(x => x.idLicDocGeneral == unSobre1.idDocumentacion).FirstOrDefault();
                                if (unReq != null)
                                {
                                    Dictionary<int, string> req = new Dictionary<int, string>();
                                    req.Add(indice, "<td>" + unReq.nombre + "</td>");
                                    detalleSobre.Add(req);
                                    if (unArchivoObs.idEstadoArchivo == 3)
                                    {
                                        archivoObservado = "<td>Presentado</td>";
                                    }
                                    else
                                    {
                                        if (unArchivoObs.idEstadoArchivo == 5)
                                        {
                                            archivoObservado = "<td>Rechazado</td>";
                                        }
                                        else
                                        {
                                            if (unArchivoObs.idEstadoArchivo == 4)
                                            {
                                                archivoObservado = "<td>Observado</td>";
                                            }
                                            else
                                            {
                                                archivoObservado = "<td>No Presentado</td>";
                                            }
                                        }
                                    }
                                }
                            }
                            else
                            {
                                var unReq = db.LicDocGeneral.Where(x => x.idLicDocGeneral == unSobre1.idDocumentacion).FirstOrDefault();
                                if (unReq != null)
                                {
                                    Dictionary<int, string> req = new Dictionary<int, string>();
                                    req.Add(indice, "<td>" + unReq.nombre + "</td>");
                                    detalleSobre.Add(req);
                                    archivoObservado = "<td>No ha sido presentado</td>";
                                }
                                else
                                {
                                    Dictionary<int, string> req = new Dictionary<int, string>();
                                    req.Add(indice, "<td>" + "</td>");
                                    detalleSobre.Add(req);
                                    archivoObservado = "<td>No ha sido presentado</td>";
                                }
                            }
                            Dictionary<int, string> nuevoDiccionario = new Dictionary<int, string>();
                            nuevoDiccionario.Add(indice, archivoObservado);
                            detalleSobre.Add(nuevoDiccionario);

                            if (!unSobre1.Equals(listaRequisitos1.Last()))
                            {
                                indice++;
                                Dictionary<int, string> sobreVacio = new Dictionary<int, string>();
                                sobreVacio.Add(indice, "<td>" + "</td>");
                                detalleSobre.Add(sobreVacio);
                            }


                        }
                        indice++;
                        var listaRequisitos2 = db.LicDocObra.Where(x => x.idObra == idObra && x.nroSobre == 2 && x.esFracasada == null).ToList();
                        var sobre2 = db.LicDocSobres.Where(x => x.idObra == idObra && x.numero == 2 && x.esFracasada == null).FirstOrDefault();
                        if ((listaRequisitos2 != null) && (listaRequisitos2.Count > 0))
                        {
                            Dictionary<int, string> nuevoDiccionario = new Dictionary<int, string>();
                            nuevoDiccionario.Add(indice, "<td>Sobre N° 2 " + sobre2.nombre + ": </td>");
                            detalleSobre.Add(nuevoDiccionario);
                        }

                        var cuantosRequisitos2 = listaRequisitos2.Count();
                        foreach (var unSobre2 in listaRequisitos2)
                        {
                            var archivoObservado = "";
                            var unArchivoObs = db.LicArchivoEmpresa
                                .Where(x => x.idRequisito == unSobre2.idLicDocObra && x.idObra == idObra && x.cuit == item.cuit && x.esFracasada == null)
                                .FirstOrDefault();
                            if (unArchivoObs != null)
                            {
                                var unReq = db.LicDocGeneral.Where(x => x.idLicDocGeneral == unSobre2.idDocumentacion).FirstOrDefault();
                                if (unReq != null)
                                {
                                    Dictionary<int, string> req = new Dictionary<int, string>();
                                    req.Add(indice, "<td>" + unReq.nombre + "</td>");
                                    detalleSobre.Add(req);
                                    if (unArchivoObs.idEstadoArchivo == 3)
                                    {
                                        archivoObservado = "<td>Presentado </td>";
                                    }
                                    else
                                    {
                                        if (unArchivoObs.idEstadoArchivo == 5)
                                        {
                                            archivoObservado = "<td> Rechazado</td>";
                                        }
                                        else
                                        {
                                            if (unArchivoObs.idEstadoArchivo == 4)
                                            {
                                                archivoObservado = "<td>Observado</td>";
                                            }
                                            else
                                            {
                                                archivoObservado = "<td>No Presentado </td>";
                                            }
                                        }
                                    }
                                }
                            }
                            else
                            {
                                var unReq = db.LicDocGeneral.Where(x => x.idLicDocGeneral == unSobre2.idDocumentacion).FirstOrDefault();
                                if (unReq != null)
                                {
                                    Dictionary<int, string> req = new Dictionary<int, string>();
                                    req.Add(indice, "<td>" + unReq.nombre + "</td>");
                                    detalleSobre.Add(req);
                                    archivoObservado = "<td>No ha sido presentado</td>";
                                }
                                else
                                {
                                    Dictionary<int, string> req = new Dictionary<int, string>();
                                    req.Add(indice, "<td>" + "</td>");
                                    detalleSobre.Add(req);
                                    archivoObservado = "<td>No ha sido presentado</td>";
                                }
                            }
                            Dictionary<int, string> nuevoDiccionario = new Dictionary<int, string>();
                            nuevoDiccionario.Add(indice, archivoObservado);
                            detalleSobre.Add(nuevoDiccionario);


                            if (!unSobre2.Equals(listaRequisitos2.Last()))
                            {
                                indice++;
                                Dictionary<int, string> sobreVacio = new Dictionary<int, string>();
                                sobreVacio.Add(indice, "<td>" + "</td>");
                                detalleSobre.Add(sobreVacio);
                            }
                        }
                        indice++;
                        var listaRequisitos3 = db.LicDocObra.Where(x => x.idObra == idObra && x.nroSobre == 3 && x.esFracasada == null).ToList();
                        var sobre3 = db.LicDocSobres.Where(x => x.idObra == idObra && x.numero == 3 && x.esFracasada == null).FirstOrDefault();
                        if ((listaRequisitos3 != null) && (listaRequisitos3.Count > 0))
                        {
                            Dictionary<int, string> nuevoDiccionario = new Dictionary<int, string>();
                            nuevoDiccionario.Add(indice, "<td>Sobre N° 3 " + sobre1.nombre + "</td>");
                            detalleSobre.Add(nuevoDiccionario);
                        }
                        var cuantosRequisitos3 = listaRequisitos3.Count();
                        foreach (var unSobre3 in listaRequisitos3)
                        {
                            var archivoObservado = "";
                            var unArchivoObs = db.LicArchivoEmpresa
                                .Where(x => x.idRequisito == unSobre3.idLicDocObra && x.idObra == idObra && x.cuit == item.cuit && x.esFracasada == null)
                                .FirstOrDefault();
                            if (unArchivoObs != null)
                            {
                                var unReq = db.LicDocGeneral.Where(x => x.idLicDocGeneral == unSobre3.idDocumentacion).FirstOrDefault();
                                if (unReq != null)
                                {
                                    Dictionary<int, string> req = new Dictionary<int, string>();
                                    req.Add(indice, "<td>" + unReq.nombre + "</td>");
                                    detalleSobre.Add(req);
                                    if (unArchivoObs.idEstadoArchivo == 3)
                                    {
                                        archivoObservado = "<td>Presentado </td>";
                                    }
                                    else
                                    {
                                        if (unArchivoObs.idEstadoArchivo == 5)
                                        {
                                            archivoObservado = "<td>Rechazado</td>";
                                        }
                                        else
                                        {
                                            if (unArchivoObs.idEstadoArchivo == 4)
                                            {
                                                archivoObservado = "<td>Observado</td>";
                                            }
                                            else
                                            {
                                                archivoObservado = "<td>No Presentado</td>";
                                            }
                                        }
                                    }
                                }
                            }
                            else
                            {
                                var unReq = db.LicDocGeneral.Where(x => x.idLicDocGeneral == unSobre3.idDocumentacion).FirstOrDefault();
                                if (unReq != null)
                                {
                                    Dictionary<int, string> req = new Dictionary<int, string>();
                                    req.Add(indice, "<td>" + unReq.nombre + "</td>");
                                    detalleSobre.Add(req);
                                    archivoObservado = "<td>No ha sido presentado</td>";
                                }
                                else
                                {
                                    Dictionary<int, string> req = new Dictionary<int, string>();
                                    req.Add(indice, "<td>" + "</td>");
                                    detalleSobre.Add(req);
                                    archivoObservado = "<td>No ha sido presentado</td>";
                                }
                            }

                            Dictionary<int, string> nuevoDiccionario = new Dictionary<int, string>();
                            nuevoDiccionario.Add(indice, archivoObservado);
                            detalleSobre.Add(nuevoDiccionario);


                            if (!unSobre3.Equals(listaRequisitos3.Last()))
                            {
                                indice++;
                                Dictionary<int, string> sobreVacio = new Dictionary<int, string>();
                                sobreVacio.Add(indice, "<td>" + "</td>");
                                detalleSobre.Add(sobreVacio);
                            }
                        }
                        indice++;
                        var listaRequisitos4 = db.LicDocObra.Where(x => x.idObra == idObra && x.nroSobre == 4 && x.esFracasada == null).ToList();
                        var sobre4 = db.LicDocSobres.Where(x => x.idObra == idObra && x.numero == 4 && x.esFracasada == null).FirstOrDefault();
                        if ((listaRequisitos4 != null) && (listaRequisitos4.Count > 0))
                        {
                            Dictionary<int, string> nuevoDiccionario = new Dictionary<int, string>();
                            nuevoDiccionario.Add(indice, "<td>Sobre N° 4 " + sobre1.nombre + "</td>");
                            detalleSobre.Add(nuevoDiccionario);
                        }
                        foreach (var unSobre4 in listaRequisitos4)
                        {
                            var archivoObservado = "";
                            var unArchivoObs = db.LicArchivoEmpresa
                                .Where(x => x.idRequisito == unSobre4.idLicDocObra && x.idObra == idObra && x.cuit == item.cuit && x.esFracasada == null)
                                .FirstOrDefault();
                            if (unArchivoObs != null)
                            {
                                var unReq = db.LicDocGeneral.Where(x => x.idLicDocGeneral == unSobre4.idDocumentacion).FirstOrDefault();
                                if (unReq != null)
                                {
                                    Dictionary<int, string> req = new Dictionary<int, string>();
                                    req.Add(indice, "<td>" + unReq.nombre + "</td>");
                                    detalleSobre.Add(req);
                                    if (unArchivoObs.idEstadoArchivo == 3)
                                    {
                                        archivoObservado = "<td>Presentado </td>";
                                    }
                                    else
                                    {
                                        if (unArchivoObs.idEstadoArchivo == 5)
                                        {
                                            archivoObservado = "<td>Rechazado</td>";
                                        }
                                        else
                                        {
                                            if (unArchivoObs.idEstadoArchivo == 4)
                                            {
                                                archivoObservado = "<td>Observado</td>";
                                            }
                                            else
                                            {
                                                archivoObservado = "<td>No Presentado</td>";
                                            }
                                        }
                                    }
                                }
                            }
                            else
                            {
                                var unReq = db.LicDocGeneral.Where(x => x.idLicDocGeneral == unSobre4.idDocumentacion).FirstOrDefault();
                                if (unReq != null)
                                {
                                    Dictionary<int, string> req = new Dictionary<int, string>();
                                    req.Add(indice, "<td>" + unReq.nombre + "</td>");
                                    detalleSobre.Add(req);
                                    archivoObservado = "<td>No ha sido presentado</td>";
                                }
                                else
                                {
                                    Dictionary<int, string> req = new Dictionary<int, string>();
                                    req.Add(indice, "<td>" + "</td>");
                                    detalleSobre.Add(req);
                                    archivoObservado = "<td>No ha sido presentado</td>";
                                }
                            }

                            Dictionary<int, string> nuevoDiccionario = new Dictionary<int, string>();
                            nuevoDiccionario.Add(indice, archivoObservado);
                            detalleSobre.Add(nuevoDiccionario);

                            if (!unSobre4.Equals(listaRequisitos4.Last()))
                            {
                                indice++;
                                Dictionary<int, string> sobreVacio = new Dictionary<int, string>();
                                sobreVacio.Add(indice, "<td>" + "</td>");
                                detalleSobre.Add(sobreVacio);
                            }
                        }


                        #endregion
                    }
                    value = value + "</tr>";
                    if (detalleSobre.Count > 0)
                    {

                        var grupos = detalleSobre
                        .SelectMany(dict => dict)
                        .GroupBy(pair => pair.Key)
                        .OrderBy(group => group.Key)
                        .Select(group => new
                        {
                            Key = group.Key,
                            Valores = group.Select(pair => pair.Value).ToList()
                        });
                        foreach (var grupo in grupos)
                        {
                            value = value + "<tr>";
                            foreach (var valor in grupo.Valores)
                            {
                                value = value + valor;
                            }
                            value = value + "</tr>";
                        }
                    }
                }
                value = value + "</tbody></table>";
            }
            catch (Exception e)
            {
                log.Error("Error grabar Acta Generica - Cuadro resumen" + e.Message);
            }

            return value;
        }
        private string buscarCuadroOferta(int? idObra)
        {

            string value = "<table class=\"table table-bordered\"><tbody><tr><td>N° </td><td> OFERENTE </td><td>MONTO OFERTA BÁSICA </td> <td> ANTICIPO FINANCIERO </td><td> OFERTA VARIANTE </td></tr>";
            ActaViewModels unActa = new ActaViewModels();
            int indice = 1;
            log4net.Config.XmlConfigurator.Configure(new FileInfo(ruta));
            try
            {
                using (db_meieEntities db = new db_meieEntities())
                {
                    var listaOferta = db.LicOfertaEmpresa.Where(x => x.idObra == idObra && x.activa == 1 && x.esFracasada == null).ToList();

                    int nroOferta = 1;
                    foreach (var item in listaOferta)
                    {
                        using (DB_RACOPEntities db1 = new DB_RACOPEntities())
                        {
                            var unEmpresa = db1.rc_Empresa.Where(x => x.idEmpresa == item.idEmpresa).FirstOrDefault();
                            if (unEmpresa != null)
                            {
                                value = value + "<tr><td>" + nroOferta + "</td><td> " + unEmpresa.razonSocial + "</td>" + "<td>$</td>" + "<td>%</td>" + "<td>$</td></tr>";


                            }
                            nroOferta = nroOferta + 1;
                        }
                    }
                    value = value + "</tbody></table>";
                }
            }
            catch (Exception e)
            {
                log.Error("Error grabar Acta Generica - Error cuadro oferta " + e.Message);
            }
            return value;
        }
        public int grabarActaObra(ActaViewModels unActa)
        {
            log4net.Config.XmlConfigurator.Configure(new FileInfo(ruta));
            log.Info("Grabar Acta Obra");

            

            int value = 0;
            try
            {
                using (db_meieEntities db = new db_meieEntities())
                {
                    var existeActa = db.LicActaObra.Where(x => x.idObra == unActa.idObra).FirstOrDefault();
                    if (existeActa != null)
                    {
                        existeActa.fechaModif = DateTime.Now;
                        //if (!string.IsNullOrEmpty(unActa.textoEncabezado))
                        //{
                        //    unActa.textoEncabezado = unActa.textoEncabezado.Replace("<p><br></p>", "");
                        //    unActa.textoEncabezado = unActa.textoEncabezado.Replace("<p><br></p>", "");
                        //    unActa.textoEncabezado = unActa.textoEncabezado.Replace("<p><br></p>", "");
                        //    unActa.textoEncabezado = unActa.textoEncabezado.Replace("<p><br></p>", "");
                        //}
                        //if (!string.IsNullOrEmpty(unActa.textoCierre))
                        //{
                        //    unActa.textoCierre = unActa.textoCierre.Replace("<p><br></p>", "");
                        //    unActa.textoCierre = unActa.textoCierre.Replace("<p><br></p>", "");
                        //    unActa.textoCierre = unActa.textoCierre.Replace("<p><br></p>", "");
                        //    unActa.textoCierre = unActa.textoCierre.Replace("<p><br></p>", "");
                        //}
                        //if (!string.IsNullOrEmpty(unActa.textoOferta))
                        //{
                        //    unActa.textoOferta = unActa.textoOferta.Replace("<p><br></p>", "");
                        //    unActa.textoOferta = unActa.textoOferta.Replace("<p><br></p>", "");
                        //    unActa.textoOferta = unActa.textoOferta.Replace("<p><br></p>", "");
                        //    unActa.textoOferta = unActa.textoOferta.Replace("<p><br></p>", "");
                        //}
                        existeActa.actaCierre = unActa.textoCierre;
                        existeActa.actaEncabezado = unActa.textoEncabezado;
                        existeActa.actaFormal = unActa.textoFormal;
                        existeActa.actaOferta = unActa.textoOferta;
                        existeActa.cuadroOferta = unActa.cuadroOferta;
                        existeActa.cuadroResumen = unActa.cuadroResumen;
                        db.SaveChanges();
                        value = existeActa.idLicActaObra;
                    }
                    else
                    {
                        //if (!string.IsNullOrEmpty(unActa.textoEncabezado))
                        //{
                        //    unActa.textoEncabezado = unActa.textoEncabezado.Replace("<p><br></p>", "");
                        //    unActa.textoEncabezado = unActa.textoEncabezado.Replace("<p><br></p>", "");
                        //    unActa.textoEncabezado = unActa.textoEncabezado.Replace("<p><br></p>", "");
                        //    unActa.textoEncabezado = unActa.textoEncabezado.Replace("<p><br></p>", "");
                        //}
                        //if (!string.IsNullOrEmpty(unActa.textoCierre))
                        //{
                        //    unActa.textoCierre = unActa.textoCierre.Replace("<p><br></p>", "");
                        //    unActa.textoCierre = unActa.textoCierre.Replace("<p><br></p>", "");
                        //    unActa.textoCierre = unActa.textoCierre.Replace("<p><br></p>", "");
                        //    unActa.textoCierre = unActa.textoCierre.Replace("<p><br></p>", "");
                        //}
                        //if (!string.IsNullOrEmpty(unActa.textoOferta))
                        //{
                        //    unActa.textoOferta = unActa.textoOferta.Replace("<p><br></p>", "");
                        //    unActa.textoOferta = unActa.textoOferta.Replace("<p><br></p>", "");
                        //    unActa.textoOferta = unActa.textoOferta.Replace("<p><br></p>", "");
                        //    unActa.textoOferta = unActa.textoOferta.Replace("<p><br></p>", "");
                        //}
                        LicActaObra actaObra = new LicActaObra();
                        actaObra.actaCierre = unActa.textoCierre;
                        actaObra.actaEncabezado = unActa.textoEncabezado;
                        actaObra.actaFormal = unActa.textoFormal;
                        actaObra.actaOferta = unActa.textoOferta;
                        actaObra.fechaAlta = DateTime.Now;
                        actaObra.idObra = unActa.idObra;
                        actaObra.cuadroOferta = unActa.cuadroOferta;
                        actaObra.cuadroResumen = unActa.cuadroResumen;
                        db.LicActaObra.Add(actaObra);
                        db.SaveChanges();
                        value = actaObra.idLicActaObra;
                    }

                    #region auditoria
                    AuditoriaOficina aud = new AuditoriaOficina();
                    var usrID = Convert.ToInt32(System.Web.HttpContext.Current.Session["idUsuario"].ToString());

                    var nombreUsuario = db.SegUser.Where(x => x.Id == usrID).FirstOrDefault();
                    aud.AddLogCambios(2, 1, "El usuario " + nombreUsuario.FullName + " ha generado la Grabación del Acta Nro: " + unActa.idActa + " para la obra " + unActa.idObra +" - Búho Licitaciones", usrID, unActa.idObra);

                    #endregion
                }
            }
            catch (Exception ex)
            {
                #region auditoria
                using (db_meieEntities db = new db_meieEntities())
                {
                    AuditoriaOficina aud = new AuditoriaOficina();
                    var usrID = Convert.ToInt32(System.Web.HttpContext.Current.Session["idUsuario"].ToString());

                    var nombreUsuario = db.SegUser.Where(x => x.Id == usrID).FirstOrDefault();
                    aud.AddLogCambios(2, 1, "El usuario " + nombreUsuario.FullName + "ha tenido un error al generar la Grabación del Acta Nro: " + unActa.idActa + " para la obra " + unActa.idObra, usrID, unActa.idObra);
                }
                #endregion
                log.Error("Error grabar Acta Generica " + ex.Message);
            }
            return value;
        }
        public ActaViewModels buscarActaObra(int? idObra)
        {
            ActaViewModels unActa = new ActaViewModels();
            var textoCuadroResumen = buscarCuadroResumen(idObra);
            var textoCuadroOferta = buscarCuadroOferta(idObra);
            log4net.Config.XmlConfigurator.Configure(new FileInfo(ruta));
            log.Info("Buscar Acta Obra");
            try
            {
                using (db_meieEntities db = new db_meieEntities())
                {
                    var existe = db.LicActaObra
                        .Where(x => x.idObra == idObra)
                        .FirstOrDefault();
                    if (existe != null)
                    {
                        unActa.textoEncabezado = existe.actaEncabezado;
                        unActa.idObra = existe.idObra;
                        unActa.textoCierre = existe.actaCierre;
                        unActa.textoEncabezado = existe.actaEncabezado;
                        unActa.textoFormal = existe.actaFormal;
                        unActa.textoOferta = existe.actaOferta;
                        unActa.cuadroResumen = existe.cuadroResumen == null ? textoCuadroResumen : existe.cuadroResumen;
                        unActa.cuadroOferta = existe.cuadroOferta == null ? textoCuadroOferta : existe.cuadroOferta;
                    }
                    #region auditoria
                    AuditoriaOficina aud = new AuditoriaOficina();
                    var usrID = Convert.ToInt32(System.Web.HttpContext.Current.Session["idUsuario"].ToString());

                    var nombreUsuario = db.SegUser.Where(x => x.Id == usrID).FirstOrDefault();
                    aud.AddLogCambios(2, 1, "El usuario " + nombreUsuario.FullName + " ha realizado una busqueda de Acta para la obra " + idObra + "- Búho Licitaciones", usrID, idObra);

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
                    aud.AddLogCambios(2, 1, "El usuario " + nombreUsuario.FullName + " ha generado un error al generar el acta de obra: " + idObra + "- Búho Licitaciones", usrID, idObra);

                    #endregion
                }
                log.Error("Error buscar Acta Obra " + ex.Message);
            }
            if (unActa != null)
            {
                if (!string.IsNullOrEmpty(unActa.textoEncabezado))
                {
                    unActa.textoEncabezado = unActa.textoEncabezado.Replace("<p><br></p>", "");
                    unActa.textoEncabezado = unActa.textoEncabezado.Replace("<p><br></p>", "");
                    unActa.textoEncabezado = unActa.textoEncabezado.Replace("<p><br></p>", "");
                    unActa.textoEncabezado = unActa.textoEncabezado.Replace("<p><br></p>", "");
                }
                if (!string.IsNullOrEmpty(unActa.textoCierre))
                {
                    unActa.textoCierre = unActa.textoCierre.Replace("<p><br></p>", "");
                    unActa.textoCierre = unActa.textoCierre.Replace("<p><br></p>", "");
                    unActa.textoCierre = unActa.textoCierre.Replace("<p><br></p>", "");
                    unActa.textoCierre = unActa.textoCierre.Replace("<p><br></p>", "");
                }
                if (!string.IsNullOrEmpty(unActa.textoOferta))
                {
                    unActa.textoOferta = unActa.textoOferta.Replace("<p><br></p>", "");
                    unActa.textoOferta = unActa.textoOferta.Replace("<p><br></p>", "");
                    unActa.textoOferta = unActa.textoOferta.Replace("<p><br></p>", "");
                    unActa.textoOferta = unActa.textoOferta.Replace("<p><br></p>", "");
                }
            }
            return unActa;
        }
        public void grabarArchivo(int? idObra, int? idCategoria, string nombreArchivo, string ruta)
        {
            ActaViewModels unActa = new ActaViewModels();
            log4net.Config.XmlConfigurator.Configure(new FileInfo(ruta));
            log.Info("Grabar Archivo Acta Obra");
            try
            {
                using (db_meieEntities db = new db_meieEntities())
                {
                    var existe = db.LicArchivoObra.Where(x => x.idCategoria == idCategoria && x.idObra == idObra && x.esFracasada == null).FirstOrDefault();
                    if (existe != null)
                    {
                        existe.nombreArchivo = nombreArchivo;
                        existe.rutaArchivo = ruta;
                        db.SaveChanges();
                    }
                    else
                    {
                        LicArchivoObra archivoObra = new LicArchivoObra();
                        archivoObra.idObra = idObra;
                        archivoObra.idCategoria = idCategoria;
                        archivoObra.nombreArchivo = nombreArchivo;
                        archivoObra.rutaArchivo = ruta;
                        db.LicArchivoObra.Add(archivoObra);
                        db.SaveChanges();
                    }
                    #region auditoria
                    AuditoriaOficina aud = new AuditoriaOficina();
                    var usrID = Convert.ToInt32(System.Web.HttpContext.Current.Session["idUsuario"].ToString());

                    var nombreUsuario = db.SegUser.Where(x => x.Id == usrID).FirstOrDefault();
                    aud.AddLogCambios(2, 1, "El usuario " + nombreUsuario.FullName + "ha Grabado Archivo Acta Nro: " + unActa.idActa + " para la obra " + unActa.idObra, usrID, unActa.idObra);

                    #endregion
                }
            }
            catch (Exception ex)
            {
                #region auditoria
                using (db_meieEntities db = new db_meieEntities())
                {
                    AuditoriaOficina aud = new AuditoriaOficina();
                    var usrID = Convert.ToInt32(System.Web.HttpContext.Current.Session["idUsuario"].ToString());

                    var nombreUsuario = db.SegUser.Where(x => x.Id == usrID).FirstOrDefault();
                    aud.AddLogCambios(2, 1, "El usuario " + nombreUsuario.FullName + "ha tenido un Error grabar Archivo Acta Nro: " + unActa.idActa + " para la obra " + unActa.idObra + "- Búho Licitaciones", usrID, unActa.idObra);
                }
                #endregion
                log.Error("Error grabar Archivo Acta " + ex.Message);
            }
        }
        public ArchivoViewModels buscarArchivo(int? idObra, int? idCategoria)
        {
            ArchivoViewModels value = new ArchivoViewModels();
            try
            {
                using (db_meieEntities db = new db_meieEntities())
                {
                    var unArchivo = db.LicArchivoObra.Where(x => x.idCategoria == idCategoria && x.idObra == idObra && x.esFracasada == null).FirstOrDefault();
                    if (unArchivo != null)
                    {
                        value.ruta = unArchivo.rutaArchivo;
                        value.nombre = unArchivo.nombreArchivo;
                    }
                }
            }
            catch (Exception ex)
            {

            }
            return value; ;
        }
        public int buscarTieneActa(int? idObra)
        {
            var value = 0;
            try
            {
                using (db_meieEntities db = new db_meieEntities())
                {
                    var existe = db.LicActaObra.Where(x => x.idObra == idObra).FirstOrDefault();
                    if (existe != null)
                    {
                        return 1;
                    }
                }
            }
            catch (Exception ex)
            {

            }
            return value;
        }
        private static string HtmlToPlainText(string html)
        {
            const string tagWhiteSpace = @"(>|$)(\W|\n|\r)+<";//matches one or more (white space or line breaks) between '>' and '<'
            const string stripFormatting = @"<[^>]*(>|$)";//match any character between '<' and '>', even when end tag is missing
            const string lineBreak = @"<(br|BR)\s{0,1}\/{0,1}>";//matches: <br>,<br/>,<br />,<BR>,<BR/>,<BR />
            var lineBreakRegex = new Regex(lineBreak, RegexOptions.Multiline);
            var stripFormattingRegex = new Regex(stripFormatting, RegexOptions.Multiline);
            var tagWhiteSpaceRegex = new Regex(tagWhiteSpace, RegexOptions.Multiline);

            var text = html;
            //Decode html specific characters
            text = System.Net.WebUtility.HtmlDecode(text);
            //Remove tag whitespace/line breaks
            text = tagWhiteSpaceRegex.Replace(text, "><");
            //Replace <br /> with line breaks
            text = lineBreakRegex.Replace(text, Environment.NewLine);
            //Strip formatting
            text = stripFormattingRegex.Replace(text, string.Empty);

            return text;
        }
    }
}
