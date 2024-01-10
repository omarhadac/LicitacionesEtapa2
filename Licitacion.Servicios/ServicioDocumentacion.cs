using Licitacion.Servicios.Utiles;
using Licitaciones.BaseDato;
using Licitaciones.ViewModels;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Licitacion.Servicios
{
    public class ServicioDocumentacion
    {
        private static readonly log4net.ILog log = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
        private static string ruta = System.Web.HttpRuntime.AppDomainAppPath + "\\bin\\log4net.config";
        public List<selectViewModels> listarReferencia()
        {
            log4net.Config.XmlConfigurator.Configure(new FileInfo(ruta));
            log.Info("Listar Referencias");

            List<selectViewModels> lista = new List<selectViewModels>();
            try
            {
                using (db_meieEntities db = new db_meieEntities())
                {
                    var tmp = from tbl in db.LicDocGeneral
                              where tbl.esPadre == 1 && tbl.activo == 1
                              select new selectViewModels
                              {
                                  nombre = tbl.nombre,
                                  id = tbl.idLicDocGeneral
                              };
                    lista = tmp.ToList();
                }
            }
            catch (Exception ex)
            {
                log.Error("Error en Listar Referencias", ex);
            }
            return lista;
        }
        public int nuevaReferencia(ReferenciaViewModels unReferencia)
        {
            log4net.Config.XmlConfigurator.Configure(new FileInfo(ruta));
            log.Info("Nueva Referencia");

            List<selectViewModels> lista = new List<selectViewModels>();
            try
            {
                using (db_meieEntities db = new db_meieEntities())
                {
                    LicDocGeneral docGeneral = new LicDocGeneral();
                    docGeneral.activo = 1;
                    docGeneral.esPadre = 1;
                    docGeneral.nombre = unReferencia.nombre;
                    db.LicDocGeneral.Add(docGeneral);
                    db.SaveChanges();

                }
            }
            catch (Exception ex)
            {
                log.Error("Error en Nueva Referencia", ex);
                return 0;
            }
            return 1;
        }
        public int nuevoRequisito(RequisitoViewModels unRequisito)
        {
            log4net.Config.XmlConfigurator.Configure(new FileInfo(ruta));
            log.Info("Nuevo Requisito");
            try
            {
                using (db_meieEntities db = new db_meieEntities())
                {
                    LicDocGeneral docGeneral = new LicDocGeneral();
                    docGeneral.activo = 1;
                    docGeneral.esPadre = 0;
                    docGeneral.idPadre = unRequisito.idPadre;
                    docGeneral.nombre = unRequisito.nombre;
                    db.LicDocGeneral.Add(docGeneral);
                    db.SaveChanges();

                }
            }
            catch (Exception ex)
            {
                log.Error("Error en Nuevo Requisito", ex);
                return 0;
            }
            return 1;
        }
        public List<RequisitoViewModels> listarRequisitoPorPadre(int? idPadre)
        {
            log4net.Config.XmlConfigurator.Configure(new FileInfo(ruta));
            log.Info("Listar Requisitos Filtrado por padre");

            List<RequisitoViewModels> lista = new List<RequisitoViewModels>();
            try
            {
                using (db_meieEntities db = new db_meieEntities())
                {
                    var query = (from tb1 in db.LicDocGeneral
                                 where tb1.idPadre == idPadre && tb1.activo == 1
                                 select new RequisitoViewModels
                                 {
                                     idPadre = tb1.idPadre,
                                     nombre = tb1.nombre,
                                     idRequisito = tb1.idLicDocGeneral
                                 });
                    lista = query.ToList();
                }
            }
            catch (Exception ex)
            {
                log.Error("Error en Listar Requisitos", ex);
            }
            return lista;
        }       
        public List<RequisitoViewModels> listarRequisitoPorObra(int? idPadre, int? idObra, int? idSobre)
        {
            log4net.Config.XmlConfigurator.Configure(new FileInfo(ruta));
            log.Info("Listar Requisitos Filtrado por idObr ay nroSobre");

            List<RequisitoViewModels> lista = new List<RequisitoViewModels>();
            try
            {
                using (db_meieEntities db = new db_meieEntities())
                {
                    var query = (from tb1 in db.LicDocObra
                                 join tb2 in db.LicDocGeneral
                                 on tb1.idDocumentacion equals tb2.idLicDocGeneral
                                 join tb3 in db.LicDocSobres
                                 on tb1.nroSobre equals tb3.numero
                                 //where tb2.idPadre == idPadre && tb1.nroSobre == nroSobre && tb1.idObra == idObra
                                 where tb3.idLicDocSobres == idSobre && tb1.idObra == idObra && tb2.activo == 1 && tb1.esFracasada == null
                                 select new RequisitoViewModels
                                 {
                                     idPadre = tb2.idPadre,
                                     nombre = tb2.nombre,
                                     idRequisito = tb1.idDocumentacion,
                                     nroSobre = tb1.nroSobre,
                                     id = tb1.idLicDocObra
                                 });
                    lista = query.ToList();
                }
            }
            catch (Exception ex)
            {
                log.Error("Error en Listar Requisitos", ex);
            }
            return lista;
        }       
        public int nuevoRequisitoObra(RequisitoObraViewModels unRequisito)
        {
            log4net.Config.XmlConfigurator.Configure(new FileInfo(ruta));
            log.Info("Nuevo Requisito para una Obra");
            try
            {
                using (db_meieEntities db = new db_meieEntities())
                {
                    var existe = db.LicDocObra
                        .Where(x => x.idDocumentacion == unRequisito.idRequisito.Value && x.idObra == unRequisito.idObra && x.nroSobre == x.nroSobre && x.esFracasada == null)
                        .FirstOrDefault();
                    var unSobre = db.LicDocSobres
                        .Where(x => x.idLicDocSobres == unRequisito.idSobre && x.esFracasada == null)
                        .FirstOrDefault();
                    if (existe == null)
                    {
                        LicDocObra docGeneral = new LicDocObra();
                        docGeneral.idDocumentacion = unRequisito.idRequisito.Value;
                        docGeneral.idObra = unRequisito.idObra;
                        docGeneral.nroSobre = unSobre.numero;
                        db.LicDocObra.Add(docGeneral);
                        db.SaveChanges();
                        log.Info("Se grabo el requisito en la obra");
                        #region auditoria
                        AuditoriaOficina aud = new AuditoriaOficina();
                        var usrID = Convert.ToInt32(System.Web.HttpContext.Current.Session["idUsuario"].ToString());

                        var nombreUsuario = db.SegUser.Where(x => x.Id == usrID).FirstOrDefault();
                        aud.AddLogCambios(2, 1, "El usuario " + nombreUsuario.FullName + " ha grabado el requisito en la obra " + unRequisito.idObra + "- Búho Licitaciones", usrID, unRequisito.idObra);

                        #endregion
                    }
                    else
                    {
                        #region auditoria
                        AuditoriaOficina aud = new AuditoriaOficina();
                        var usrID = Convert.ToInt32(System.Web.HttpContext.Current.Session["idUsuario"].ToString());

                        var nombreUsuario = db.SegUser.Where(x => x.Id == usrID).FirstOrDefault();
                        aud.AddLogCambios(2, 1, "El usuario " + nombreUsuario.FullName + " ha intentado grabar un requisito en la obra " + unRequisito.idObra + ", pero el requisito ya existe. - Búho Licitaciones", usrID, unRequisito.idObra);

                        #endregion
                        log.Info("No se carga el requisito porque ya existe");
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
                    aud.AddLogCambios(2, 1, "El usuario " + nombreUsuario.FullName + " ha generado un error grabar un requisito en la obra: " + unRequisito.idObra + "- Búho Licitaciones", usrID, unRequisito.idObra);

                    #endregion
                }
                log.Error("Error en Nuevo Requisito para una Obra", ex);
                return 0;
            }
            return 1;
        }
        public int eliminarRequisitoObra(int? idRequisito)
        {
            log4net.Config.XmlConfigurator.Configure(new FileInfo(ruta));
            log.Info("Eliminar Requisito para una Obra");
            try
            {
                using (db_meieEntities db = new db_meieEntities())
                {
                    var existe = db.LicDocObra.Where(x => x.idLicDocObra == idRequisito && x.esFracasada == null).FirstOrDefault();
                    if (existe != null)
                    {
                        db.LicDocObra.Remove(existe);
                        db.SaveChanges();
                        #region auditoria
                        AuditoriaOficina aud = new AuditoriaOficina();
                        var usrID = Convert.ToInt32(System.Web.HttpContext.Current.Session["idUsuario"].ToString());

                        var nombreUsuario = db.SegUser.Where(x => x.Id == usrID).FirstOrDefault();
                        aud.AddLogCambios(2, 1, "El usuario " + nombreUsuario.FullName + " ha eliminado un requisito en la obra " + existe.idObra + ", pero el requisito ya existe. - Búho Licitaciones", usrID, existe.idObra);

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
                    aud.AddLogCambios(2, 1, "El usuario " + nombreUsuario.FullName + " ha generado Error en Eliminar Requisito: " + idRequisito + "- Búho Licitaciones", usrID, 0);

                    #endregion
                }
                log.Error("Error en Eliminar Requisito para una Obra", ex);
                return 0;
            }
            return 1;
        }
        public List<selectViewModels> listarRequisitoPorObra(int? idObra, int? idSobre)
        {
            log4net.Config.XmlConfigurator.Configure(new FileInfo(ruta));
            log.Info("Listar Requisitos Filtrado por padre");

            List<selectViewModels> lista = new List<selectViewModels>();
            try
            {               
                using (db_meieEntities db = new db_meieEntities())
                {
                    var query = (from tb1 in db.LicDocObra
                                 join tb2 in db.LicDocGeneral
                                    on tb1.idDocumentacion equals tb2.idLicDocGeneral
                                 join tb3 in db.LicDocSobres
                                on tb1.nroSobre equals tb3.numero
                                 where tb3.idLicDocSobres == idSobre && tb1.idObra == idObra && tb1.esFracasada == null
                                 select new selectViewModels
                                 {
                                     nombre = tb2.nombre,
                                     id = tb1.idLicDocObra
                                 });
                    lista = query.ToList();
                }
            }
            catch (Exception ex)
            {
                log.Error("Error en Listar Requisitos", ex);
            }
            return lista;
        }
        public List<RequisitoViewModels> listarRequisitoPorObraEmpresa(int? idEmpresa, int? idObra, int? idSobre, int? idEstado, string stringEncriptar)
        {
            log4net.Config.XmlConfigurator.Configure(new FileInfo(ruta));
            log.Info("Listar Requisitos Filtrado por idObra, nroSobre y Empresa");

            List<RequisitoViewModels> lista = new List<RequisitoViewModels>();
            List<RequisitoViewModels> tmp = new List<RequisitoViewModels>();
            try
            {
                using (db_meieEntities db = new db_meieEntities())
                {
                    var query = (from tb1 in db.LicDocObra
                                 join tb2 in db.LicDocGeneral
                                    on tb1.idDocumentacion equals tb2.idLicDocGeneral
                                 join tb3 in db.LicDocSobres
                                 on tb1.nroSobre equals tb3.numero
                                 where tb3.idLicDocSobres == idSobre && tb1.idObra == idObra && tb1.esFracasada == null
                                 // //where tb2.idPadre == idPadre && tb1.nroSobre == nroSobre && tb1.idObra == idObra
                                 // where tb1.nroSobre == nroSobre && tb1.idObra == idObra
                                 select new RequisitoViewModels
                                 {
                                     idPadre = tb2.idPadre,
                                     nombre = tb2.nombre,
                                     idRequisito = tb1.idDocumentacion,
                                     nroSobre = tb1.nroSobre,
                                     id = tb1.idLicDocObra
                                 });
                    tmp = query.ToList();
                    foreach (var item in tmp)
                    {
                        var existe = db.LicArchivoEmpresa
                            .Where(x => x.idRequisito == item.id && x.idEmpresa == idEmpresa && x.nroSobre == item.nroSobre && x.esFracasada == null)
                            .FirstOrDefault();
                        if (existe != null)
                        {
                            ServicioArchivo servicioArchivo = new ServicioArchivo();
                            if (servicioArchivo.debeEncriptar(idObra, stringEncriptar))
                            {
                                item.nombreArchivo = Encrypt.Desencriptar(existe.nombreArchivo);
                            }
                            else
                            {
                                item.nombreArchivo = existe.nombreArchivo;
                            }

                            item.completo = 1;
                            item.fecha = existe.fecha;
                            item.horaArchivo = existe.hora;
                            item.idArchivo = existe.idArchivo;
                        }
                        else
                        {
                            item.completo = 0;
                        }
                        if(idEstado == 0)
                        {
                            lista.Add(item);
                        }
                        else
                        {
                            if(existe != null)
                            {
                                if (existe.idEstadoArchivo == idEstado)
                                {
                                    lista.Add(item);
                                }
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                log.Error("Error en listarRequisitoPorObraEmpresa", ex);
            }
            return lista;
        }
        public List<RequisitoViewModels> listarArchivoPorObraEmpresa(int? idEmpresa, int? idObra, int? idSobre, string cuit)
        {
            log4net.Config.XmlConfigurator.Configure(new FileInfo(ruta));
            log.Info("Listar Archivos Filtrado por idObra, nroSobre y Empresa");

            List<RequisitoViewModels> lista = new List<RequisitoViewModels>();
            try
            {
                using (db_meieEntities db = new db_meieEntities())
                {
                    var query = (from tb1 in db.LicDocObra
                                 join tb2 in db.LicDocGeneral
                                    on tb1.idDocumentacion equals tb2.idLicDocGeneral
                                 join tb3 in db.LicDocSobres
                                    on tb1.nroSobre equals tb3.numero
                                 where tb3.idLicDocSobres == idSobre && tb1.idObra == idObra && tb1.esFracasada == null
                                 select new RequisitoViewModels
                                 {
                                     idPadre = tb2.idPadre,
                                     nombre = tb2.nombre,
                                     idRequisito = tb1.idDocumentacion,
                                     nroSobre = tb1.nroSobre,
                                     id = tb1.idLicDocObra,
                                     horaArchivo="",
                                     idArchivo = 0,
                                     tieneArchivo = true
                                 });
                    lista = query.ToList();
                    foreach (var item in lista)
                    {
                        var tieneArchivo = default(LicArchivoEmpresa);

                        if (idEmpresa != null)
                        {
                            tieneArchivo = db.LicArchivoEmpresa
                                              .Where(x => x.idEmpresa == idEmpresa && x.nroSobre == item.nroSobre && x.idObra == idObra && x.idRequisito == item.id && x.esFracasada == null && x.tipoOferta == 1)
                                              .FirstOrDefault();
                        }
                        else
                        {
                            if(cuit != null)
                            {
                                tieneArchivo = db.LicArchivoEmpresa
                                                   .Where(x => x.cuit == cuit && x.nroSobre == item.nroSobre && x.idObra == idObra && x.idRequisito == item.id && x.esFracasada == null)
                                                   .FirstOrDefault();
                            }
                        }
                        if (tieneArchivo != null)
                        {
                            item.idArchivo = tieneArchivo.idArchivo;
                            item.idEstado = tieneArchivo.idEstadoArchivo;
                            item.observacion = string.IsNullOrEmpty(tieneArchivo.observaciones) ? "" : tieneArchivo.observaciones;
                            item.horaArchivo = string.IsNullOrEmpty(tieneArchivo.hora)?"":tieneArchivo.hora;
                            item.fecha = tieneArchivo.fecha;
                            if (string.IsNullOrEmpty(tieneArchivo.nombreArchivo))
                            {
                                item.tieneArchivo = false;
                            }
                        }
                        else
                        {
                            item.tieneArchivo = false;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                log.Error("Error en listarArchivoPorObraEmpresa", ex);
            }
            return lista;
        }
        public int cambiarEstadoArchivo(int? idArchivo, string observaciones, int? idEstado)
        {
            log4net.Config.XmlConfigurator.Configure(new FileInfo(ruta));
            log.Info("Cambia Estado Archivos de la Oferta");

            try
            {
                using (db_meieEntities db = new db_meieEntities())
                {
                    var unArchivo = db.LicArchivoEmpresa.Where(x => x.idArchivo == idArchivo && x.esFracasada == null).FirstOrDefault();
                    if (unArchivo != null)
                    {
                        unArchivo.idEstadoArchivo = idEstado;
                        unArchivo.observaciones = observaciones;
                        db.SaveChanges();
                        #region auditoria
                        AuditoriaOficina aud = new AuditoriaOficina();
                        var usrID = Convert.ToInt32(System.Web.HttpContext.Current.Session["idUsuario"].ToString());

                        var nombreUsuario = db.SegUser.Where(x => x.Id == usrID).FirstOrDefault();
                        aud.AddLogCambios(2, 1, "El usuario " + nombreUsuario.FullName + " ha realizado cambio del Estado Archivos de la Oferta " + unArchivo.idObra + "- Búho Licitaciones", usrID, unArchivo.idObra);

                        #endregion
                    }
                }
                return 1;
            }
            catch (Exception ex)
            {
                using (db_meieEntities db = new db_meieEntities())
                {
                    #region auditoria
                    AuditoriaOficina aud = new AuditoriaOficina();
                    var usrID = Convert.ToInt32(System.Web.HttpContext.Current.Session["idUsuario"].ToString());

                    var nombreUsuario = db.SegUser.Where(x => x.Id == usrID).FirstOrDefault();
                    aud.AddLogCambios(2, 1, "El usuario " + nombreUsuario.FullName + " ha generado un error al Cambia Estado Archivos de la Oferta " + idArchivo + "- Búho Licitaciones", usrID, 0);

                    #endregion
                }
                log.Error("Error en listarArchivoPorObraEmpresa", ex);
            }
            return 0;
        }
        public List<RequisitoViewModels> listarRequisitoPorObraEmpresa(int? idEmpresa, int? idObra)
        {
            log4net.Config.XmlConfigurator.Configure(new FileInfo(ruta));
            log.Info("Listar Requisitos Filtrado por idObra y Empresa");

            List<RequisitoViewModels> lista = new List<RequisitoViewModels>();
            List<RequisitoViewModels> tmp = new List<RequisitoViewModels>();
            try
            {
                using (db_meieEntities db = new db_meieEntities())
                {
                    var query = (from tb1 in db.LicDocObra
                                 join tb2 in db.LicDocGeneral
                                    on tb1.idDocumentacion equals tb2.idLicDocGeneral
                                 where tb1.idObra == idObra && tb1.esFracasada == null
                                 select new RequisitoViewModels
                                 {
                                     idPadre = tb2.idPadre,
                                     nombre = tb2.nombre,
                                     idRequisito = tb1.idDocumentacion,
                                     nroSobre = tb1.nroSobre,
                                     id = tb1.idLicDocObra,
                                 });
                    tmp = query.ToList();
                    foreach (var item in tmp)
                    {
                        var existe = db.LicArchivoEmpresa
                            .Where(x => x.idRequisito == item.id && x.idEmpresa == idEmpresa && x.idObra == idObra && x.esFracasada == null)
                            .FirstOrDefault();
                        if (existe != null)
                        {
                            item.completo = 1;
                            item.idArchivo = existe.idArchivo;
                            item.nombreArchivo = Encrypt.Desencriptar(existe.nombreArchivo);
                            item.fecha = existe.fecha;
                        }
                        else
                        {
                            item.completo = 0;
                            item.nombreArchivo = "No se subió ningún archivo";
                        }
                        lista.Add(item);
                    }
                }
            }
            catch (Exception ex)
            {
                log.Error("Error en listarRequisitoPorObraEmpresa", ex);
            }
            return lista;
        }
        public List<SobresViewModels> listarSobres(int? idObra)
        {
            log4net.Config.XmlConfigurator.Configure(new FileInfo(ruta));
            log.Info("Listar Sobres");

            List<SobresViewModels> lista = new List<SobresViewModels>();
            try
            {
                agregarSobres(idObra);
                using (db_meieEntities db = new db_meieEntities())
                {
                    var tmp = from s in db.LicDocSobres
                              where s.activo == true && s.idObra == idObra && s.esFracasada == null
                              select new SobresViewModels
                              {
                                  idLicDocSobres = s.idLicDocSobres,
                                  numero = s.numero,
                                  nombre = "Nro " + s.numero + (s.nombre != null ? " - " + s.nombre : ""),
                                  idObra = s.idObra,
                              };
                    lista = tmp.ToList();
                }
            }
            catch (Exception ex)
            {
                log.Error("Error en Listar Referencias", ex);
            }
            return lista;
        }
        public List<SobresViewModels> listarSobresEmpresa(int? idObra)
        {
            log4net.Config.XmlConfigurator.Configure(new FileInfo(ruta));
            log.Info("Listar Sobres");

            List<SobresViewModels> lista = new List<SobresViewModels>();
            try
            {
                agregarSobres(idObra);
                using (db_meieEntities db = new db_meieEntities())
                {
                    var tmp = from s in db.LicDocSobres
                              where s.activo == true && s.idObra == idObra && s.esFracasada == null &&
                                    db.LicDocObra.Any(d => d.nroSobre == s.numero && d.idObra == idObra)
                              select new SobresViewModels
                              {
                                  idLicDocSobres = s.idLicDocSobres,
                                  numero = s.numero,
                                  nombre = "Nro " + s.numero + (s.nombre != null ? " - " + s.nombre : ""),
                                  idObra = s.idObra,
                              };
                    lista = tmp.ToList();
                }
            }
            catch (Exception ex)
            {
                log.Error("Error en Listar Referencias", ex);
            }
            return lista;
        }
        public int agregarSobres(int? idObra)
        {
            log4net.Config.XmlConfigurator.Configure(new FileInfo(ruta));
            log.Info("Nuevos Sobres");

            try
            {
                using (db_meieEntities db = new db_meieEntities())
                {
                    var existe = db.LicDocSobres
                           .Where(x => x.idObra == idObra && x.activo == true && x.esFracasada == null)
                           .FirstOrDefault();
                    LicDocSobres docSobres = new LicDocSobres();
                    if (existe == null)
                    {
                        int i = 1;
                        for (i = 1; i <= 2; i++)
                        {
                            docSobres.numero = i;
                            docSobres.idObra = idObra;
                            docSobres.activo = true;
                            db.LicDocSobres.Add(docSobres);
                            db.SaveChanges();
                        }
                    }
                    db.SaveChanges();
                    #region auditoria
                    //AuditoriaOficina aud = new AuditoriaOficina();
                    //var usrID = Convert.ToInt32(System.Web.HttpContext.Current.Session["idUsuario"].ToString());
                    //var nombreUsuario = db.SegUser.Where(x => x.Id == usrID).FirstOrDefault();
                    //aud.AddLogCambios(2, 1, "El usuario " + nombreUsuario.FullName + " ha creado un nuevo sobre en la obra: " + idObra + "- Búho Licitaciones", usrID, idObra);

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
                    aud.AddLogCambios(2, 1, "El usuario " + nombreUsuario.FullName + " ha generado un error al generar el sobre de la obra: " + idObra + "- Búho Licitaciones", usrID, idObra);

                    #endregion
                }
                log.Error("Error en Nuevas Sobres", ex);
                return 0;
            }
            return 1;
        }
        public int? ultimoSobre(int? idObra)
        {
            log4net.Config.XmlConfigurator.Configure(new FileInfo(ruta));
            log.Info("ultimo");
            int? numeroUltimoSobre = 0;
            try
            {
                using (db_meieEntities db = new db_meieEntities())
                {
                    var ultimoSobre = db.LicDocSobres
                         .Where(x => x.idObra == idObra && x.activo == true && x.esFracasada == null)
                         .OrderByDescending(x => x.numero)
                         .FirstOrDefault();

                    if (ultimoSobre != null)
                    {
                        numeroUltimoSobre = ultimoSobre.numero;
                    }
                   
                }
            }
            catch (Exception ex)
            {
                log.Error("Error en Carga ultimo Sobres", ex);
            }
            return numeroUltimoSobre + 1;
        }
        public int nuevoSobre(SobresViewModels unSobre)
        {
            log4net.Config.XmlConfigurator.Configure(new FileInfo(ruta));
            log.Info("Nuevo Sobre");
            try
            {
                using (db_meieEntities db = new db_meieEntities())
                {
                    var ultimoSobre = db.LicDocSobres
                            .Where(x => x.idObra == unSobre.idObra && x.activo == true && x.esFracasada == null)
                            .OrderByDescending(x => x.numero)
                            .FirstOrDefault();

                    LicDocSobres docSobre = new LicDocSobres();
                   
                    docSobre.nombre = string.IsNullOrWhiteSpace(unSobre.nombre) ? null : unSobre.nombre;
                    docSobre.idObra = unSobre.idObra;
                    docSobre.activo = true;
                    if (ultimoSobre != null)  //pongo esto por las dudas 
                    {
                        docSobre.numero = ultimoSobre.numero + 1;
                    } else
                        {
                            docSobre.numero = 1;
                        }
                    db.LicDocSobres.Add(docSobre);
                    db.SaveChanges();
                    #region auditoria
                    AuditoriaOficina aud = new AuditoriaOficina();
                    var usrID = Convert.ToInt32(System.Web.HttpContext.Current.Session["idUsuario"].ToString());

                    var nombreUsuario = db.SegUser.Where(x => x.Id == usrID).FirstOrDefault();
                    aud.AddLogCambios(2, 1, "El usuario " + nombreUsuario.FullName + " ha creado un nuevo sobre en la obra: " + unSobre.idObra + "- Búho Licitaciones", usrID, unSobre.idObra);

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
                    aud.AddLogCambios(2, 1, "El usuario " + nombreUsuario.FullName + " ha generado un error al generar el sobre de la obra: " + unSobre.idObra + "- Búho Licitaciones", usrID, unSobre.idObra);

                    #endregion
                }
                log.Error("Error en Nuevo Sobre", ex);
                return 0;
            }
            return 1;
        }
        public SobresViewModels buscarSobre(int? idObra, int? idSobre)
        {
            log4net.Config.XmlConfigurator.Configure(new FileInfo(ruta));
            log.Info("buscar Un Sobre");
            SobresViewModels unSobre = new SobresViewModels();
            try
            {
                using (db_meieEntities db = new db_meieEntities())
                {
                    var tmp = from s in db.LicDocSobres
                              where s.activo == true && s.idLicDocSobres == idSobre && s.esFracasada == null
                              select new SobresViewModels
                              {
                                  idLicDocSobres = s.idLicDocSobres,
                                  numero = s.numero,
                                  nombre = s.nombre != null ? s.nombre : "",
                                  idObra = s.idObra,
                              };
                    unSobre = tmp.FirstOrDefault();

                }
            }
            catch (Exception ex)
            {
                log.Error("Error buscar Una Licitacion", ex);
            }

            return unSobre;
        }
        public int editarSobre(SobresViewModels unSobre)
        {
            log4net.Config.XmlConfigurator.Configure(new FileInfo(ruta));
            log.Info("Editar Sobre");
            try
            {
                using (db_meieEntities db = new db_meieEntities())
                {
                    var sobre = db.LicDocSobres
                            .Where(x => x.idLicDocSobres == unSobre.idLicDocSobres && x.esFracasada == null)                            
                            .FirstOrDefault();

                    sobre.nombre = unSobre.nombre;

                    db.SaveChanges();
                    #region auditoria
                    AuditoriaOficina aud = new AuditoriaOficina();
                    var usrID = Convert.ToInt32(System.Web.HttpContext.Current.Session["idUsuario"].ToString());

                    var nombreUsuario = db.SegUser.Where(x => x.Id == usrID).FirstOrDefault();
                    aud.AddLogCambios(2, 1, "El usuario " + nombreUsuario.FullName + " ha editado un sobre en la obra: " + unSobre.idObra + "- Búho Licitaciones", usrID, unSobre.idObra);

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
                    aud.AddLogCambios(2, 1, "El usuario " + nombreUsuario.FullName + " ha generado un error al editar el sobre de la obra: " + unSobre.idObra + "- Búho Licitaciones", usrID, unSobre.idObra);

                    #endregion
                }
                log.Error("Error en Nuevo Sobre", ex);
                return 0;
            }
            return 1;
        }
        public int eliminarSobre(SobresViewModels unSobre)
        {
            log4net.Config.XmlConfigurator.Configure(new FileInfo(ruta));
            log.Info("Editar Sobre");
            try
            {
                using (db_meieEntities db = new db_meieEntities())
                {
                    var sobre = db.LicDocSobres
                            .Where(x => x.idLicDocSobres == unSobre.idLicDocSobres && x.esFracasada == null)                            
                            .FirstOrDefault();
                    var requisitosSobre = db.LicDocObra 
                           .Where(x => x.idObra == unSobre.idObra && x.nroSobre == sobre.numero && x.esFracasada == null)
                           .ToList();
                    foreach(var item in requisitosSobre) //elimino los requisitos
                    {
                        db.LicDocObra.Remove(item);
                    }
                    db.LicDocSobres.Remove(sobre);      //elimino el sobre
                    db.SaveChanges();
                    #region auditoria
                    AuditoriaOficina aud = new AuditoriaOficina();
                    var usrID = Convert.ToInt32(System.Web.HttpContext.Current.Session["idUsuario"].ToString());

                    var nombreUsuario = db.SegUser.Where(x => x.Id == usrID).FirstOrDefault();
                    aud.AddLogCambios(2, 1, "El usuario " + nombreUsuario.FullName + " ha eliminado un sobre en la obra: " + unSobre.idObra + "- Búho Licitaciones", usrID, unSobre.idObra);

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
                    aud.AddLogCambios(2, 1, "El usuario " + nombreUsuario.FullName + " ha generado un error al eliminar el sobre de la obra: " + unSobre.idObra + "- Búho Licitaciones", usrID, unSobre.idObra);

                    #endregion
                }
                log.Error("Error en Nuevo Sobre", ex);
                return 0;
            }
            return 1;
        }
        public List<RequisitoViewModels> listarArchivoPorObraEmpresaObs(int? idEmpresa, int? idObra, int? idSobre, string cuit)
        {
            log4net.Config.XmlConfigurator.Configure(new FileInfo(ruta));
            log.Info("Listar Archivos Filtrado por idObra, nroSobre y Empresa");

            List<RequisitoViewModels> lista = new List<RequisitoViewModels>();
            List<RequisitoViewModels> listaFinal = new List<RequisitoViewModels>();
            try
            {
                using (db_meieEntities db = new db_meieEntities())
                {
                    var query = (from tb1 in db.LicDocObra
                                 join tb2 in db.LicDocGeneral
                                    on tb1.idDocumentacion equals tb2.idLicDocGeneral
                                 join tb3 in db.LicDocSobres.Where(x=>x.idObra == idObra)
                                    on tb1.nroSobre equals tb3.numero
                                 where tb1.idObra == idObra && tb1.esFracasada == null
                                 select new RequisitoViewModels
                                 {
                                     idPadre = tb2.idPadre,
                                     nombre = tb2.nombre,
                                     idRequisito = tb1.idDocumentacion,
                                     nroSobre = tb1.nroSobre,
                                     id = tb1.idLicDocObra,
                                     horaArchivo = "",
                                     idArchivo = 0,
                                     idSobre = tb3.idLicDocSobres,
                                     tieneArchivo = true
                                 });
                    if (idSobre != 0)
                    {
                        query = query.Where(x => x.idSobre == idSobre);
                    }
                    lista = query.ToList();

                    foreach (var item in lista)
                    {
                        var tieneArchivo = default(LicArchivoEmpresa);

                        if (idEmpresa != null)
                        {
                            tieneArchivo = db.LicArchivoEmpresa
                                              .Where(x => x.idEmpresa == idEmpresa && x.nroSobre == item.nroSobre && x.idObra == idObra && x.idRequisito == item.id && x.esFracasada == null && x.tipoOferta == 2)
                                              .FirstOrDefault();
                        }
                        else //si no tiene idEmpresa buscar por cuit                       
                        {
                            if(cuit != null)
                            {
                                tieneArchivo = db.LicArchivoEmpresa
                                                  .Where(x => x.cuit == cuit && x.nroSobre == item.nroSobre && x.idObra == idObra && x.idRequisito == item.id && x.esFracasada == null)
                                                  .FirstOrDefault();
                            }
                        }
                        if (tieneArchivo != null)
                        {
                            item.idArchivo = tieneArchivo.idArchivo;
                            item.idEstado = tieneArchivo.idEstadoArchivo;
                            item.observacion = string.IsNullOrEmpty(tieneArchivo.observaciones) ? "" : tieneArchivo.observaciones;
                            item.horaArchivo = string.IsNullOrEmpty(tieneArchivo.hora) ? "" : tieneArchivo.hora;
                            item.fecha = tieneArchivo.fecha;
                            if (string.IsNullOrEmpty(tieneArchivo.nombreArchivo))
                            {
                                item.tieneArchivo = false;
                            }
                            listaFinal.Add(item);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                log.Error("Error en listarArchivoPorObraEmpresa", ex);
            }
            return listaFinal;
        }
        /// <summary>
        /// lista requisitos de observaciones
        /// </summary>
        /// <param name="idEmpresa"></param>
        /// <param name="idObra"></param>
        /// <param name="idSobre"></param>
        /// <param name="idEstado"></param>
        /// <returns></returns>
        public List<RequisitoViewModels> listarRequisitoPorObraEmpresaObs(int? idEmpresa, int? idObra)
        {
            log4net.Config.XmlConfigurator.Configure(new FileInfo(ruta));
            log.Info("Listar Requisitos Filtrado por idObra y Empresa");

            List<RequisitoViewModels> lista = new List<RequisitoViewModels>();
            List<RequisitoViewModels> tmp = new List<RequisitoViewModels>();
            try
            {
                using (db_meieEntities db = new db_meieEntities())
                {
                    var fechaControl = DateTime.Now.Date;
                    var query = (from tb1 in db.LicDocObra
                                 join tb2 in db.LicDocGeneral
                                    on tb1.idDocumentacion equals tb2.idLicDocGeneral
                                 where tb1.idObra == idObra
                                 select new RequisitoViewModels
                                 {
                                     idPadre = tb2.idPadre,
                                     nombre = tb2.nombre,
                                     idRequisito = tb1.idDocumentacion,
                                     nroSobre = tb1.nroSobre,
                                     id = tb1.idLicDocObra,
                                 });
                    tmp = query.ToList();
                    foreach (var item in tmp)
                    {
                        if (idEmpresa != null)
                        {
                            var tieneArchivo = db.LicArchivoEmpresa
                                              .Where(x => x.idEmpresa == idEmpresa && x.idObra == idObra && x.idRequisito == item.id && x.tipoOferta == 2)
                                              .FirstOrDefault();
                            if (tieneArchivo != null)
                            {
                                if (string.IsNullOrEmpty(tieneArchivo.nombreArchivo))
                                {
                                    item.completo = 0;
                                }
                                else
                                {
                                    item.completo = 1;
                                    item.nombreArchivo = tieneArchivo.nombreArchivo;
                                    item.fecha = tieneArchivo.fecha;
                                }

                                item.idArchivo = tieneArchivo.idArchivo;
                                lista.Add(item);
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                log.Error("Error en listarRequisitoPorObraEmpresa", ex);
            }
            return lista;
        }
        public List<selectViewModels> listarRequisitoPorObraObser(int? idObra, int? idEmpresa, int? idSobre)
        {
            log4net.Config.XmlConfigurator.Configure(new FileInfo(ruta));
            log.Info("Listar Requisitos Filtrado por padre");

            List<selectViewModels> lista = new List<selectViewModels>();
            try
            {
                using (db_meieEntities db = new db_meieEntities())
                {
                    var query = (from tb1 in db.LicDocObra
                                 join tb2 in db.LicDocGeneral
                                    on tb1.idDocumentacion equals tb2.idLicDocGeneral
                                 //join tb3 in db.LicDocSobres
                                 //   on tb1.nroSobre equals tb3.numero
                                 where tb1.idObra == idObra //&& tb3.idLicDocSobres == idSobre
                                 select new selectViewModels
                                 {
                                     nombre = tb2.nombre,
                                     id = tb1.idLicDocObra,
                                 });
                    var listaTmp = query.ToList();

                    foreach (var item in listaTmp)
                    {

                        if (idEmpresa != null)
                        {
                            var tieneArchivo = db.LicArchivoEmpresa
                                              .Where(x => x.idEmpresa == idEmpresa && x.idObra == idObra && x.idRequisito == item.id && (x.idEstadoArchivo == 4 || x.idEstadoArchivo == 6 || x.idEstadoArchivo == 5))
                                              .FirstOrDefault();
                            if (tieneArchivo != null)
                            {
                                lista.Add(item);
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                log.Error("Error en listarArchivoPorObraEmpresa", ex);
            }
            return lista;
        }
        
        /// <summary>
        /// lista requisitos de observaciones
        /// </summary>
        /// <param name="idEmpresa"></param>
        /// <param name="idObra"></param>
        /// <param name="idSobre"></param>
        /// <param name="idEstado"></param>
        /// <returns></returns>
        public List<RequisitoViewModels> listarRequisitoPorObraEmpresaObs(int? idEmpresa, int? idObra, int? idSobre, int? idEstado)
        {
            log4net.Config.XmlConfigurator.Configure(new FileInfo(ruta));
            log.Info("Listar Requisitos Filtrado por idObra, nroSobre y Empresa");

            List<RequisitoViewModels> lista = new List<RequisitoViewModels>();
            List<RequisitoViewModels> tmp = new List<RequisitoViewModels>();
            try
            {
                using (db_meieEntities db = new db_meieEntities())
                {
                    var query = (from tb1 in db.LicDocObra
                                 join tb2 in db.LicDocGeneral
                                    on tb1.idDocumentacion equals tb2.idLicDocGeneral
                                 //join tb3 in db.LicDocSobres
                                 //on tb1.nroSobre equals tb3.numero
                                 //where tb3.idLicDocSobres == idSobre && tb1.idObra == idObra
                                 where tb1.idObra == idObra
                                 select new RequisitoViewModels
                                 {
                                     idPadre = tb2.idPadre,
                                     nombre = tb2.nombre,
                                     idRequisito = tb1.idDocumentacion,
                                     nroSobre = tb1.nroSobre,
                                     id = tb1.idLicDocObra
                                 });
                    tmp = query.ToList();
                    foreach (var item in tmp)
                    {

                        if (idEmpresa != null)
                        {
                            var tieneArchivo = db.LicArchivoEmpresa
                                  .Where(x => x.idEmpresa == idEmpresa && x.idObra == idObra && x.idRequisito == item.id && x.tipoOferta == 2)
                                  .FirstOrDefault();
                            if(tieneArchivo == null)
                            {
                                tieneArchivo = db.LicArchivoEmpresa
                                  .Where(x => x.idEmpresa == idEmpresa && x.idObra == idObra && x.idRequisito == item.id && (x.idEstadoArchivo == 4 || x.idEstadoArchivo == 6 || x.idEstadoArchivo == 5))
                                  .FirstOrDefault();
                            }
                            if (tieneArchivo != null)
                            {
                                //if (string.IsNullOrEmpty(tieneArchivo.nombreArchivo))
                                //{
                                //    item.completo = 0;
                                //}
                                //else
                                //{
                                //    item.completo = 1;
                                //}
                                if (tieneArchivo.tipoOferta == 2)
                                {
                                    if (string.IsNullOrEmpty(tieneArchivo.nombreArchivo))
                                    {
                                        item.completo = 0;
                                    }
                                    else
                                    {
                                        item.completo = 1;
                                    }
                                }
                                else
                                {
                                    item.completo = 0;
                                }
                                item.idArchivo = tieneArchivo.idArchivo;
                                lista.Add(item);
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                log.Error("Error en listarRequisitoPorObraEmpresaObs", ex);
            }
            return lista;
        }
    }
}