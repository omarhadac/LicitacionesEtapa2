using Ionic.Zip;
using Licitacion.Servicios.Utiles;
using Licitaciones.BaseDato;
using Licitaciones.ViewModels;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Licitacion.Servicios
{
    public class ServicioArchivo
    {
        private static readonly log4net.ILog log = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
        private static string ruta = System.Web.HttpRuntime.AppDomainAppPath + "\\bin\\log4net.config";
        public bool nuevo
            (string ruta, string nombre, string nombreVirtual, string tipo, int? idObra,
            int? idEmpresa, int? idRequisito, int? idSobre, string rutaInicio, string stringEncriptar)
        {
            log4net.Config.XmlConfigurator.Configure(new FileInfo(ruta));
            ServicioEmpresa servicioEmpresa = new ServicioEmpresa();
            if (servicioEmpresa.puedeSubirOferta(idObra))
            {
                log.Info("Graba Archivo de Empresa");
                try
                {
                    using (db_meieEntities db = new db_meieEntities())
                    {
                        if (debeEncriptar(idObra, stringEncriptar))
                        {
                            nombreVirtual = Encrypt.Encriptar(nombreVirtual);
                            nombre = Encrypt.Encriptar(nombre);
                        }
                        var existe = db.LicArchivoEmpresa
                            .Where(x => x.idEmpresa == idEmpresa && x.idObra == idObra && x.idRequisito == idRequisito && x.esFracasada == null)
                            .FirstOrDefault();
                        var sobre = db.LicDocSobres
                            .Where(x => x.idLicDocSobres == idSobre && x.esFracasada == null)
                            .FirstOrDefault();
                        if (existe == null)
                        {
                            //// GRABO ARCHIVO
                            LicArchivoEmpresa unArchivo = new LicArchivoEmpresa();
                            unArchivo.nombreArchivo = nombre;
                            unArchivo.idEmpresa = idEmpresa;
                            unArchivo.ruta = ruta;
                            //unArchivo.ruta = ruta;
                            unArchivo.idEstadoArchivo = 1;
                            unArchivo.idObra = idObra;
                            unArchivo.idRequisito = idRequisito;
                            unArchivo.nombreArchivoEnc = nombreVirtual;
                            unArchivo.fecha = DateTime.Now.Date;
                            unArchivo.hora = DateTime.Now.ToString("hh:mm tt");
                            unArchivo.nroSobre = sobre.numero;
                            unArchivo.tipoArchivo = tipo;
                            unArchivo.observaciones = string.Empty;
                            unArchivo.tipoOferta = 1; // 1-Oferta 2-Observaciones 3-Mejora
                            db.LicArchivoEmpresa.Add(unArchivo);
                            db.SaveChanges();
                        }
                        else
                        {
                            existe.nombreArchivo = nombre;
                            existe.idEmpresa = idEmpresa;
                            existe.ruta = ruta;
                            existe.idEstadoArchivo = 1;
                            existe.idObra = idObra;
                            existe.idRequisito = idRequisito;
                            existe.nombreArchivoEnc = nombreVirtual;
                            existe.fecha = DateTime.Now.Date;
                            existe.nroSobre = sobre.numero;
                            existe.tipoArchivo = tipo;
                            existe.hora = DateTime.Now.ToString("hh:mm tt");
                            existe.observaciones = string.Empty;
                            db.SaveChanges();
                        }
                    }
                    if (debeEncriptar(idObra, stringEncriptar))
                    {
                        var nombreZip = idObra.Value.ToString() + ".zip";
                        desComprimirDirectorio(ruta, rutaInicio, nombreZip, idObra );
                        comprimirDirectorio(ruta, rutaInicio, nombreZip);
                    }
                    return true;
                    #region auditoria
                    AuditoriaRacop aud = new AuditoriaRacop();
                    var usrID = Convert.ToInt32(System.Web.HttpContext.Current.Session["idUsuario"].ToString());
                    //var idEmpresa = Convert.ToInt32(System.Web.HttpContext.Current.Session["idEmpresa"].ToString());
                    aud.AddLogCreate(usrID, idEmpresa, " ha generado la Grabación del Archivo Empresa en obra Nro" +idObra.ToString()  + "- Búho Licitaciones");

                    #endregion
                }
                catch (Exception ex)
                {
                    log.Error("Error Graba Archivo de Empresa " + ex.Message);
                    #region auditoria
                    AuditoriaRacop aud = new AuditoriaRacop();
                    var usrID = Convert.ToInt32(System.Web.HttpContext.Current.Session["idUsuario"].ToString());
                    //var idEmpresa = Convert.ToInt32(System.Web.HttpContext.Current.Session["idEmpresa"].ToString());
                    aud.AddLogCreate(usrID, idEmpresa, " hay un error en la Grabación del Archivo Empresa en obra Nro" + idObra.ToString() + "- Búho Licitaciones");

                    #endregion
                }
            }
            else
            {
                #region auditoria
                AuditoriaRacop aud = new AuditoriaRacop();
                var usrID = Convert.ToInt32(System.Web.HttpContext.Current.Session["idUsuario"].ToString());
                //var idEmpresa = Convert.ToInt32(System.Web.HttpContext.Current.Session["idEmpresa"].ToString());
                aud.AddLogCreate(usrID, idEmpresa, " No puede subir Archivo de Empresa Fecha de cierre vencida en obra Nro " + idObra.ToString() + "- Búho Licitaciones");

                #endregion
                log.Info("No puede subir Archivo de Empresa Fecha de cierre vencida");
            }
            return false;
        }
        public void nuevoObs
           (string ruta, string nombre, string nombreVirtual, string tipo, int? idObra,
           int? idEmpresa, int? idRequisito, int? idSobre, string rutaInicio, string stringEncriptar)
        {
            log4net.Config.XmlConfigurator.Configure(new FileInfo(ruta));
            ServicioEmpresa servicioEmpresa = new ServicioEmpresa();
            log.Info("Graba Archivo Observaciones de Empresa");
            try
            {
                //if (debeEncriptar(idObra, stringEncriptar))
                //{
                //    nombreVirtual = Encrypt.Encriptar(nombreVirtual);
                //    nombre = Encrypt.Encriptar(nombre);
                //}
                using (DB_RACOPEntities db2 = new DB_RACOPEntities())     
                    using (db_meieEntities db = new db_meieEntities())
                {
                    LicArchivoEmpresa existe = new LicArchivoEmpresa();
                    string cuit = string.Empty;
                    var unaEmpresa = db2.rc_Empresa.Where(x => x.idEmpresa == idEmpresa).FirstOrDefault();
                    if (unaEmpresa != null)
                    {
                        cuit = unaEmpresa.cuit;
                    }
                    existe = db.LicArchivoEmpresa
                        .Where(x => x.idEmpresa == idEmpresa && x.idObra == idObra && x.idRequisito == idRequisito && x.tipoOferta == 2)
                        .FirstOrDefault();
                    if (existe == null)
                    {
                        existe = db.LicArchivoEmpresa
                        .Where(x => x.cuit == cuit && x.idObra == idObra && x.idRequisito == idRequisito && x.tipoOferta == 2)
                        .FirstOrDefault();
                    }
                    if (existe == null)
                    {
                        existe = db.LicArchivoEmpresa
                            .Where(x => x.idEmpresa == idEmpresa && x.idObra == idObra && x.idRequisito == idRequisito && x.tipoOferta == 1)
                            .FirstOrDefault();
                    }
                    if (existe == null)
                    {
                        existe = db.LicArchivoEmpresa
                        .Where(x => x.cuit == cuit && x.idObra == idObra && x.idRequisito == idRequisito && x.tipoOferta == 1)
                        .FirstOrDefault();
                    }
                    //var sobre = db.LicDocSobres
                    //    .Where(x => x.idLicDocSobres == idSobre)
                    //    .FirstOrDefault();
                    if (existe == null)
                    {
                        //// GRABO ARCHIVO
                        LicArchivoEmpresa unArchivo = new LicArchivoEmpresa();
                        unArchivo.nombreArchivo = nombre;
                        unArchivo.idEmpresa = idEmpresa;
                        unArchivo.ruta = ruta;
                        unArchivo.idEstadoArchivo = 1;
                        unArchivo.idObra = idObra;
                        unArchivo.idRequisito = idRequisito;
                        unArchivo.nombreArchivoEnc = nombreVirtual;
                        unArchivo.fecha = DateTime.Now.Date;
                        unArchivo.hora = DateTime.Now.ToString("hh:mm tt");
                        unArchivo.tipoOferta = 2;                        
                        unArchivo.tipoArchivo = tipo;
                        unArchivo.observaciones = string.Empty;
                        if (unaEmpresa != null) {
                            unArchivo.cuit = unaEmpresa.cuit;
                        }
                        db.LicArchivoEmpresa.Add(unArchivo);
                        db.SaveChanges();
                    }
                    else
                    {
                        if(existe.tipoOferta == 2)
                        {
                            existe.nombreArchivo = nombre;
                            existe.ruta = ruta;
                            existe.idEstadoArchivo = 1;
                            existe.idRequisito = idRequisito;
                            existe.nombreArchivoEnc = nombreVirtual;
                            existe.fecha = DateTime.Now.Date;
                            existe.tipoArchivo = tipo;
                            existe.tipoOferta = 2;
                            existe.hora = DateTime.Now.ToString("hh:mm tt");
                            existe.observaciones = string.Empty;
                            if (unaEmpresa != null)
                            {
                                existe.cuit = unaEmpresa.cuit;
                            }
                            db.SaveChanges();
                        }
                        else
                        {
                            //// GRABO ARCHIVO
                            LicArchivoEmpresa unArchivo = new LicArchivoEmpresa();
                            unArchivo.nombreArchivo = nombre;
                            unArchivo.idEmpresa = idEmpresa;
                            unArchivo.ruta = ruta;
                            unArchivo.idEstadoArchivo = 1;
                            unArchivo.idObra = idObra;
                            unArchivo.idRequisito = idRequisito;
                            unArchivo.nombreArchivoEnc = nombreVirtual;
                            unArchivo.fecha = DateTime.Now.Date;
                            unArchivo.hora = DateTime.Now.ToString("hh:mm tt");
                            unArchivo.tipoOferta = 2;
                            unArchivo.nroSobre = existe.nroSobre;
                            unArchivo.tipoArchivo = tipo;
                            unArchivo.observaciones = string.Empty;
                            if (unaEmpresa != null)
                            {
                                unArchivo.cuit = unaEmpresa.cuit;
                            }
                            db.LicArchivoEmpresa.Add(unArchivo);
                            db.SaveChanges();
                        }
                    }
                }
                #region auditoria
                AuditoriaRacop aud = new AuditoriaRacop();
                var usrID = Convert.ToInt32(System.Web.HttpContext.Current.Session["idUsuario"].ToString());
                //var idEmpresa = Convert.ToInt32(System.Web.HttpContext.Current.Session["idEmpresa"].ToString());
                aud.AddLogCreate(usrID, idEmpresa, " se Graba Archivo Observaciones de Empresa en obra Nro: " + idObra.ToString() + "- Búho Licitaciones");

                #endregion
                //if (debeEncriptar(idObra, stringEncriptar))
                //{
                //    var nombreZip = idObra.Value.ToString() + ".zip";
                //    desComprimirDirectorio(ruta, rutaInicio, nombreZip, idObra);
                //    comprimirDirectorio(ruta, rutaInicio, nombreZip);
                //}
            }
            catch (Exception ex)
            {
                #region auditoria
                AuditoriaRacop aud = new AuditoriaRacop();
                var usrID = Convert.ToInt32(System.Web.HttpContext.Current.Session["idUsuario"].ToString());
                //var idEmpresa = Convert.ToInt32(System.Web.HttpContext.Current.Session["idEmpresa"].ToString());
                aud.AddLogCreate(usrID, idEmpresa, " tuvo un error al Grabar Archivo Observaciones de Empresa en obra Nro: " + idObra.ToString() + "- Búho Licitaciones");

                #endregion
                log.Error("Error Graba Archivo Observaciones de Empresa " + ex.Message);
            }
        }
        public void eliminarArchivo(int? idArchivo, string rutaInicio, string stringEncriptar)
        {
            log4net.Config.XmlConfigurator.Configure(new FileInfo(ruta));
            log.Info("Elimina Archivo de Empresa");
            try
            {
                using (db_meieEntities db = new db_meieEntities())
                {
                    var existe = db.LicArchivoEmpresa
                        .Where(x => x.idArchivo == idArchivo && x.esFracasada == null)
                        .FirstOrDefault();
                    if (existe != null)
                    {
                        var ruta = "";
                        if (debeEncriptar(existe.idObra, stringEncriptar))
                        {
                            ruta = existe.ruta + '\\' + Encrypt.Desencriptar(existe.nombreArchivoEnc);
                        }
                        else
                        {
                            ruta = existe.ruta + '\\' + existe.nombreArchivoEnc;
                        }
                        var nombreZip = existe.idObra.Value.ToString() + ".zip";
                        if (debeEncriptar(existe.idObra, stringEncriptar))
                        {                            
                            desComprimirDirectorio(existe.ruta, rutaInicio, nombreZip, existe.idObra);
                        }
                        if (System.IO.File.Exists(ruta))
                        {
                            System.IO.File.Delete(ruta);
                        }
                        db.LicArchivoEmpresa.Remove(existe);
                        db.SaveChanges();
                        if (debeEncriptar(existe.idObra, stringEncriptar))
                        {
                            comprimirDirectorio(existe.ruta, rutaInicio, nombreZip);
                        }
                    }
                    #region auditoria
                    AuditoriaRacop aud = new AuditoriaRacop();
                    var usrID = Convert.ToInt32(System.Web.HttpContext.Current.Session["idUsuario"].ToString());
                    var idEmpresa = Convert.ToInt32(System.Web.HttpContext.Current.Session["idEmpresa"].ToString());
                    aud.AddLogCreate(usrID, idEmpresa, " tuvo un error al Eliminar Archivo de Empresa en obra Nro: " + existe.idObra.ToString() + "- Búho Licitaciones");

                    #endregion
                }
            }
            catch (Exception ex)
            {
                #region auditoria
                AuditoriaRacop aud = new AuditoriaRacop();
                var usrID = Convert.ToInt32(System.Web.HttpContext.Current.Session["idUsuario"].ToString());
                var idEmpresa = Convert.ToInt32(System.Web.HttpContext.Current.Session["idEmpresa"].ToString());
                aud.AddLogCreate(usrID, idEmpresa, " tuvo un error al Eliminar Archivo de Empresa"  + "- Búho Licitaciones");

                #endregion
                log.Error("Error Elimina Archivo de Empresa " + ex.Message);
            }
        }
        public void eliminarArchivoObserv(int? idArchivo, string rutaInicio, string stringEncriptar)
        {
            log4net.Config.XmlConfigurator.Configure(new FileInfo(ruta));
            log.Info("Elimina Archivo de Empresa");
            try
            {
                using (db_meieEntities db = new db_meieEntities())
                {
                    var existe = db.LicArchivoEmpresa
                        .Where(x => x.idArchivo == idArchivo)
                        .FirstOrDefault();
                    if (existe != null)
                    {
                        var ruta = "";
                        ruta = existe.ruta + '\\' + existe.nombreArchivoEnc;
                        //if (debeEncriptar(existe.idObra, stringEncriptar))
                        //{
                        //    ruta = existe.ruta + '\\' + Encrypt.Desencriptar(existe.nombreArchivoEnc);
                        //}
                        //else
                        //{
                        //    ruta = existe.ruta + '\\' + existe.nombreArchivoEnc;
                        //}
                        //var nombreZip = existe.idObra.Value.ToString() + ".zip";
                        //if (debeEncriptar(existe.idObra, stringEncriptar))
                        //{
                        //    desComprimirDirectorio(existe.ruta, rutaInicio, nombreZip, existe.idObra);
                        //}
                        if (System.IO.File.Exists(ruta))
                        {
                            System.IO.File.Delete(ruta);
                        }
                        db.LicArchivoEmpresa.Remove(existe);
                        db.SaveChanges();
                        //if (debeEncriptar(existe.idObra, stringEncriptar))
                        //{
                        //    comprimirDirectorio(existe.ruta, rutaInicio, nombreZip);
                        //}
                        existe.ruta = string.Empty;
                        existe.rutaVirtual = string.Empty;
                        existe.nombreArchivoEnc = string.Empty;
                        existe.nombreArchivo = string.Empty;
                        existe.fecha = null;
                        existe.hora = null;
                        db.SaveChanges();
                    }
                    #region auditoria
                    AuditoriaRacop aud = new AuditoriaRacop();
                    var usrID = Convert.ToInt32(System.Web.HttpContext.Current.Session["idUsuario"].ToString());
                    var idEmpresa = Convert.ToInt32(System.Web.HttpContext.Current.Session["idEmpresa"].ToString());
                    aud.AddLogCreate(usrID, idEmpresa, " Elimina Archivo de Empresa obra Nro: " + existe.idObra.ToString() + "- Búho Licitaciones");

                    #endregion
                }
            }
            catch (Exception ex)
            {
                log.Error("Error Elimina Archivo de Empresa " + ex.Message);
                #region auditoria
                AuditoriaRacop aud = new AuditoriaRacop();
                var usrID = Convert.ToInt32(System.Web.HttpContext.Current.Session["idUsuario"].ToString());
                var idEmpresa = Convert.ToInt32(System.Web.HttpContext.Current.Session["idEmpresa"].ToString());
                aud.AddLogCreate(usrID, idEmpresa, " Error al Eliminar Archivo de Empresa " + "- Búho Licitaciones");

                #endregion
            }
        }
        public ArchivoViewModels buscarArchivo(int? idArchivo, string rutaInicio, string stringEncriptar)
        {
            ArchivoViewModels unArchivo = new ArchivoViewModels();
            log4net.Config.XmlConfigurator.Configure(new FileInfo(ruta));
            log.Info("Buscar Archivo de Empresa");
            try
            {
                using (db_meieEntities db = new db_meieEntities())
                {
                    var existe = db.LicArchivoEmpresa
                        .Where(x => x.idArchivo == idArchivo && x.esFracasada == null)
                        .FirstOrDefault();
                    if (existe != null)
                    {
                        if(existe.tipoOferta == 1)
                        {
                            if (debeEncriptar(existe.idObra, stringEncriptar))
                            {
                                var nombreZip = existe.idObra.Value.ToString() + ".zip";
                                desComprimirDirectorio(existe.ruta, rutaInicio, nombreZip, existe.idObra);
                                unArchivo.ruta = existe.ruta + '\\' + Encrypt.Desencriptar(existe.nombreArchivoEnc);
                                unArchivo.nombre = Encrypt.Desencriptar(existe.nombreArchivo);
                            }
                            else
                            {
                                unArchivo.ruta = existe.ruta + '\\' + existe.nombreArchivoEnc;
                                unArchivo.nombre = existe.nombreArchivo;
                            }
                        }
                        else
                        {
                            unArchivo.ruta = existe.ruta + '\\' + existe.nombreArchivoEnc;
                            unArchivo.nombre = existe.nombreArchivo;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                log.Error("Error buscar Archivo de Empresa " + ex.Message);
            }
            return unArchivo;
        }
        public void nuevoObra
            (string ruta, string nombre, int? idObra, int? idCategoria, string descripcion)
        {
            log4net.Config.XmlConfigurator.Configure(new FileInfo(ruta));
            log.Info("Graba Archivo de Obra");
            try
            {
                using (db_meieEntities db = new db_meieEntities())
                {
                    var existe = db.LicArchivoObra
                        .Where(x => x.idObra == idObra && x.idObra == idObra && x.idCategoria == idCategoria && x.esFracasada == null)
                        .FirstOrDefault();
                    if (existe == null)
                    {
                        //// GRABO ARCHIVO
                        LicArchivoObra unArchivo = new LicArchivoObra();
                        unArchivo.nombreArchivo = nombre;
                        unArchivo.idCategoria = idCategoria;
                        unArchivo.rutaArchivo = ruta;
                        unArchivo.idObra = idObra;
                        unArchivo.nombreArchivo = nombre;
                        unArchivo.fecha = DateTime.Now;
                        unArchivo.descripcion = descripcion;
                        db.LicArchivoObra.Add(unArchivo);
                        db.SaveChanges();
                    }
                    else
                    {
                        existe.nombreArchivo = nombre;
                        existe.idCategoria = idCategoria;
                        existe.rutaArchivo = ruta;
                        existe.idObra = idObra;
                        existe.nombreArchivo = nombre;
                        existe.fecha = DateTime.Now;
                        existe.descripcion = descripcion;
                        
                        db.SaveChanges();
                    }
                    grabarCategoriaArchivo(idObra, idCategoria);
                    #region auditoria
                    AuditoriaRacop aud = new AuditoriaRacop();
                    var usrID = Convert.ToInt32(System.Web.HttpContext.Current.Session["idUsuario"].ToString());
                    var idEmpresa = Convert.ToInt32(System.Web.HttpContext.Current.Session["idEmpresa"].ToString());
                    aud.AddLogCreate(usrID, idEmpresa, " Graba Archivo de Obra: " + idObra.ToString() + "- Búho Licitaciones");

                    #endregion
                }
            }
            catch (Exception ex)
            {
                #region auditoria
                AuditoriaRacop aud = new AuditoriaRacop();
                var usrID = Convert.ToInt32(System.Web.HttpContext.Current.Session["idUsuario"].ToString());
                var idEmpresa = Convert.ToInt32(System.Web.HttpContext.Current.Session["idEmpresa"].ToString());
                aud.AddLogCreate(usrID, idEmpresa, "Error al Grabar Archivo de Obra: " + idObra.ToString() + "- Búho Licitaciones");

                #endregion
                log.Error("Error Graba Archivo de Obra " + ex.Message);
            }
        }
        public void nuevoObraVarios
            (string ruta, string nombre, int? idObra, int? idCategoria, string descripcion)
        {
            log4net.Config.XmlConfigurator.Configure(new FileInfo(ruta));
            log.Info("Graba Archivo de Obra");
            try
            {
                using (db_meieEntities db = new db_meieEntities())
                {                    
                        //// GRABO ARCHIVO
                        LicArchivoObra unArchivo = new LicArchivoObra();
                        unArchivo.nombreArchivo = nombre;
                        unArchivo.idCategoria = idCategoria;
                        unArchivo.rutaArchivo = ruta;
                        unArchivo.idObra = idObra;
                        unArchivo.nombreArchivo = nombre;
                        unArchivo.fecha = DateTime.Now;
                        unArchivo.descripcion = descripcion;
                        db.LicArchivoObra.Add(unArchivo);
                        db.SaveChanges();
                  
                    grabarCategoriaArchivo(idObra, idCategoria);
                    #region auditoria
                    AuditoriaRacop aud = new AuditoriaRacop();
                    var usrID = Convert.ToInt32(System.Web.HttpContext.Current.Session["idUsuario"].ToString());
                    var idEmpresa = Convert.ToInt32(System.Web.HttpContext.Current.Session["idEmpresa"].ToString());
                    aud.AddLogCreate(usrID, idEmpresa, " Graba Archivo de Obra: " + idObra.ToString() + "- Búho Licitaciones");

                    #endregion
                }
            }
            catch (Exception ex)
            {
                #region auditoria
                AuditoriaRacop aud = new AuditoriaRacop();
                var usrID = Convert.ToInt32(System.Web.HttpContext.Current.Session["idUsuario"].ToString());
                var idEmpresa = Convert.ToInt32(System.Web.HttpContext.Current.Session["idEmpresa"].ToString());
                aud.AddLogCreate(usrID, idEmpresa, "Error al Grabar Archivo de Obra: " + idObra.ToString() + "- Búho Licitaciones");

                #endregion
                log.Error("Error Graba Archivo de Obra " + ex.Message);
            }
        }
        public ArchivoViewModels buscarArchivoObra(int? idLicArchivoObra)
        {
            ArchivoViewModels value = new ArchivoViewModels();
            try
            {
                using (db_meieEntities db = new db_meieEntities())
                {
                    var unArchivo = db.LicArchivoObra.Where(x => x.idLicArchivoObra == idLicArchivoObra && x.esFracasada == null).FirstOrDefault();
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
        private void grabarCategoriaArchivo(int? idObra, int? idCategoria)
        {
            try
            {
                using (db_meieEntities db = new db_meieEntities())
                {
                    var unProyecto = db.LicProyecto.Where(x => x.idProyecto == idObra).FirstOrDefault();
                    if(unProyecto != null)
                    {
                        if (idCategoria == 2)
                        {
                            unProyecto.tieneContrato = true;
                            db.SaveChanges();
                        }
                        if (idCategoria == 1)
                        {
                            unProyecto.tieneActa = true;
                            db.SaveChanges();
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                log.Error("Error Graba Categoria de Archivo " + ex.Message);
            }
        }
        public void eliminarArchivoObra(int? idLicArchivoObra, string rutaInicio)
        {
            log4net.Config.XmlConfigurator.Configure(new FileInfo(ruta));
            log.Info("Elimina Archivo de Obra");
            try
            {
                using (db_meieEntities db = new db_meieEntities())
                {
                    var unArchivo = db.LicArchivoObra.Where(x => x.idLicArchivoObra == idLicArchivoObra && x.esFracasada == null).FirstOrDefault();
                    if (unArchivo != null)
                    {    
                        var ruta = "";
                       
                            ruta = unArchivo.rutaArchivo;   

                        if (System.IO.File.Exists(ruta))
                        {
                            System.IO.File.Delete(ruta);
                        }
                        db.LicArchivoObra.Remove(unArchivo);
                        db.SaveChanges();

                    }
                    #region auditoria
                    AuditoriaRacop aud = new AuditoriaRacop();
                    var usrID = Convert.ToInt32(System.Web.HttpContext.Current.Session["idUsuario"].ToString());
                    var idEmpresa = Convert.ToInt32(System.Web.HttpContext.Current.Session["idEmpresa"].ToString());
                    aud.AddLogCreate(usrID, idEmpresa, " Elimina Archivo de Obra id Archivo: " + idLicArchivoObra.ToString() + "- Búho Licitaciones");

                    #endregion
                }
            }
            catch (Exception ex)
            {
                #region auditoria
                AuditoriaRacop aud = new AuditoriaRacop();
                var usrID = Convert.ToInt32(System.Web.HttpContext.Current.Session["idUsuario"].ToString());
                var idEmpresa = Convert.ToInt32(System.Web.HttpContext.Current.Session["idEmpresa"].ToString());
                aud.AddLogCreate(usrID, idEmpresa, " Error al eliminar Archivo de Obra id Archivo: " + idLicArchivoObra.ToString() + "- Búho Licitaciones");

                #endregion
                log.Error("Error Elimina Archivo de Obra " + ex.Message);
            }
        }
        public List<ArchivoObraViewModels> listarArchivoObra(int? idObra)
        {
            List<ArchivoObraViewModels> lista = new List<ArchivoObraViewModels>();
            try
            {
                using (db_meieEntities db = new db_meieEntities())
                {
                    var query = from tl in db.LicArchivoObra
                                join tl2 in db.LicArchivoCategoria on tl.idCategoria equals tl2.idCategoria
                                where tl.idObra == idObra && tl.esFracasada == null
                                select new ArchivoObraViewModels
                                {
                                    idLicArchivoObra = tl.idLicArchivoObra,
                                    nombreArchivo = tl.nombreArchivo,
                                    nombreCategoria = tl2.nombreCategoria,
                                    idDetalle = tl.idLicArchivoObra,
                                    idCategoria = tl.idCategoria,
                                    idObra = tl.idObra,
                                    fechaArchivo = tl.fecha,
                                    descripcion = tl.descripcion != null ? tl.descripcion : string.Empty
                                };       
                    query = query.OrderBy(x => x.fechaArchivo);
                    var resultados = query.ToList();

                    //muestro nombre de archivo sin los tres numeros ni el _
                    foreach (var resultado in resultados)
                    {
                        int indiceGuionBajo = resultado.nombreArchivo.IndexOf('_');
                        resultado.nombre = indiceGuionBajo >= 0 && indiceGuionBajo + 1 < resultado.nombreArchivo.Length
                            ? resultado.nombreArchivo.Substring(indiceGuionBajo + 1)
                            : resultado.nombreArchivo;
                    }
                   
                    lista = resultados;

                }
            }
            catch (Exception ex)
            {
                log.Error("Error listarArchivoObra " + ex.Message);
            }
            return lista;
        }
        public List<ArchivoObraViewModels> listarArchivoObraEmpresa(int? idObra, int? idEmpresa, string stringEncriptar)
        {
            List<ArchivoObraViewModels> lista = new List<ArchivoObraViewModels>();
            try
            {
                using (db_meieEntities db = new db_meieEntities())
                {
                    var query = from tl in db.LicArchivoEmpresa
                                where tl.idObra == idObra && tl.idEmpresa == idEmpresa && tl.nombreArchivo != null && tl.tipoOferta == 1
                                select new ArchivoObraViewModels
                                {
                                    nombre = tl.nombreArchivoEnc,
                                    ruta = tl.ruta,
                                    nombreArchivo = tl.nombreArchivoEnc
                                };
                    var listaTmp = query.ToList();
                    foreach (var item in listaTmp)
                    {
                        ArchivoObraViewModels archivoObraView = new ArchivoObraViewModels();
                        if (debeEncriptar(idObra, stringEncriptar))
                        {
                            archivoObraView.nombre = Encrypt.Desencriptar(item.nombre);
                            archivoObraView.nombreArchivo = Encrypt.Desencriptar(item.nombreArchivo);
                        }
                        else
                        {
                            archivoObraView.nombre = item.nombre;
                            archivoObraView.nombreArchivo = item.nombreArchivo;
                        }
                        if (System.IO.File.Exists(ruta))
                        {
                            System.IO.File.Delete(ruta);
                        }
                        archivoObraView.ruta = item.ruta + "\\" + archivoObraView.nombreArchivo;
                        lista.Add(archivoObraView);
                    }
                }
            }
            catch (Exception ex)
            {
                log.Error("Error listarArchivoObraEmpresa " + ex.Message);
            }
            return lista;
        }
        public List<ArchivoObraViewModels> listarArchivoObraEmpresaObs(int? idObra, int? idEmpresa, string cuit,string stringEncriptar)
        {
            List<ArchivoObraViewModels> lista = new List<ArchivoObraViewModels>();
            try
            {
                using (db_meieEntities db = new db_meieEntities())
                {
                    var query = from tl in db.LicArchivoEmpresa
                                where tl.idObra == idObra && (tl.idEmpresa == idEmpresa || tl.cuit == cuit) &&
                                      tl.tipoOferta == 2
                                select new ArchivoObraViewModels
                                {
                                    nombre = tl.nombreArchivoEnc,
                                    ruta = tl.ruta,
                                    idEstado = tl.idEstadoArchivo,
                                    nombreArchivo = tl.nombreArchivoEnc
                                };
                    //query = query.Where(x => x.idEstado == 4 || x.idEstado == 6);
                    var listaTmp = query.ToList();
                    foreach (var item in listaTmp)
                    {
                        ArchivoObraViewModels archivoObraView = new ArchivoObraViewModels();
                        //if (debeEncriptar(idObra, stringEncriptar))
                        //{
                        //    archivoObraView.nombre = Encrypt.Desencriptar(item.nombre);
                        //    archivoObraView.nombreArchivo = Encrypt.Desencriptar(item.nombreArchivo);
                        //}
                        //else
                        //{
                        //    archivoObraView.nombre = item.nombre;
                        //    archivoObraView.nombreArchivo = item.nombreArchivo;
                        //}
                        if (!string.IsNullOrEmpty(item.nombreArchivo))
                        {
                            archivoObraView.nombre = item.nombre;
                            archivoObraView.nombreArchivo = item.nombreArchivo;
                            if (System.IO.File.Exists(ruta))
                            {
                                System.IO.File.Delete(ruta);
                            }
                            archivoObraView.ruta = item.ruta + "\\" + archivoObraView.nombreArchivo;
                            lista.Add(archivoObraView);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                log.Error("Error listarArchivoObraEmpresaObs " + ex.Message);
            }
            return lista;
        }
        public bool debeEncriptar(int? idObra, string stringEncriptar)
        {
            try
            {
                using (db_meieEntities db = new db_meieEntities())
                {
                    if (stringEncriptar == "false")
                    {
                        return false;
                    }
                    var unProyecto = db.PryLicitacion.Where(x => x.IdPryProyecto == idObra).FirstOrDefault();
                    if (unProyecto != null)
                    {
                        if (unProyecto.FechaPublicacionDesde.HasValue)
                        {
                            DateTime fechaControl = Convert.ToDateTime("2023-12-01");
                            if (unProyecto.FechaPublicacionDesde > fechaControl)
                            {
                                return true;
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                log.Error("Error debeEncriptar " + ex.Message);
            }
            return false;
        }
        public bool debeEncriptarOld(int? idObra)
        {
            return false;
        }
        public void comprimirDirectorio(string rutaDestino, string rutaDirectorio, string nombreZip)
        {
            try
            {
                if (File.Exists(rutaDirectorio + "\\" + nombreZip))
                {
                    File.Delete(rutaDestino + "\\" + nombreZip);
                }
                using (ZipFile zip = new ZipFile())
                {
                    zip.Password = "mypassword";
                    zip.Encryption = EncryptionAlgorithm.PkzipWeak;
                    zip.AddDirectory(rutaDestino);
                    zip.Save(rutaDirectorio + "\\" + nombreZip);
                }
                if (Directory.Exists(rutaDestino)) Directory.Delete(rutaDestino, true);
            }
            catch(Exception ex)
            {
                log.Error("Error comprimirDirectorio " + ex.Message);
            }
        }
        public void desComprimirDirectorio(string rutaDestino, string rutaDirectorio, string nombreZip, int? idObra)
        {
            try
            {
                var rutaTemporal = rutaDirectorio + "\\"+ idObra.Value.ToString() +  "\\tmp\\";
                var zipFile = rutaDirectorio + "\\" + nombreZip;
                if (File.Exists(zipFile))
                {
                    using (Ionic.Zip.ZipFile zip = Ionic.Zip.ZipFile.Read(zipFile))
                    {
                        zip.Password = "mypassword";
                        zip.Encryption = EncryptionAlgorithm.PkzipWeak;
                        zip.ExtractAll(rutaTemporal, Ionic.Zip.ExtractExistingFileAction.OverwriteSilently);
                    }
                }
                string origenFile = rutaTemporal;
                string destFile = rutaDestino;

                string[] files = Directory.GetFiles(origenFile);
                foreach (var item in files)
                {
                    File.Copy(item, destFile + "\\" + Path.GetFileName(item));
                }
                if (Directory.Exists(rutaTemporal)) Directory.Delete(rutaTemporal, true);
            }
            catch (Exception ex)
            {
                log.Error("Error desComprimirDirectorio " + ex.Message);
            }
        }
        public int existeArchivo(int? idObra, int? idEmpresa, int? idRequisito, int? tipoOferta)
        {
            try
            {
                using (db_meieEntities db = new db_meieEntities())
                {
                    var existe = db.LicArchivoEmpresa
                        .Where(x => x.idEmpresa == idEmpresa && x.idObra == idObra && x.idRequisito == idRequisito && x.tipoOferta == tipoOferta && x.esFracasada == null)
                        .FirstOrDefault();
                    if(existe != null)
                    {
                        return 1;
                    }
                }
            }
            catch(Exception ex)
            {
                log.Error("Error existeArchivo " + ex.Message);
            }
            return 0;
        }
    }
}
