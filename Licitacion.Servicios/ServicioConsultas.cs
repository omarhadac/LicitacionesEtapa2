
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
using System.Security.Policy;

namespace Licitacion.Servicios
{
    public class ServicioConsultas
    {
        private static readonly log4net.ILog log = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
        private static string ruta = System.Web.HttpRuntime.AppDomainAppPath + "\\bin\\log4net.config";      
      
        public List<consultasViewModels> listarPreguntasRespuestas(int? idObra)
        {
            log4net.Config.XmlConfigurator.Configure(new FileInfo(ruta));
            log.Info("buscar Lista de Fechas");
            List<consultasViewModels> lista = new List<consultasViewModels>();
            try
            {
                using (db_meieEntities db = new db_meieEntities())
                {
                    var query = from co in db.LicConsultas
                                where co.idObra == idObra && co.esFracasada == null
                                select new consultasViewModels
                                {
                                    idConsulta = co.idConsulta,
                                    idObra = co.idObra,
                                    idEmpresa = co.idEmpresa,
                                    fecha = co.fecha,
                                    razonSocial = co.razonSocial,
                                    detalle = co.detalle,
                                    idPregunta = co.idPregunta,
                                    
                                };
                    var tmp = query.ToList();
                    var tmpPreguntas = tmp.Where(x => x.idPregunta == null).ToList();
                    var tmpRespuestas = tmp.Where(x => x.idPregunta != null).ToList();
                    

                    // Recorrer grupos y agregar registros a la lista
                    foreach (var pregunta in tmpPreguntas)
                    {
                        lista.Add(pregunta);

                        foreach (var respuesta in tmpRespuestas)
                        {
                            if (pregunta.idConsulta == respuesta.idPregunta)
                            {
                                lista.Add(respuesta);
                            }
                        }
                    }

                }
            }
            catch (Exception ex)
            {
                log.Error("Error buscar Lista de consultas", ex);
            }
            return lista;
        }
        public List<consultasViewModels> listarPreguntasRespuestasEmpresa(int? idObra, int? idEmpresa)
        {
            log4net.Config.XmlConfigurator.Configure(new FileInfo(ruta));
            log.Info("buscar Lista de Fechas");
            List<consultasViewModels> lista = new List<consultasViewModels>();
            try
            {
                using (db_meieEntities db = new db_meieEntities())
                {
                    var query = from co in db.LicConsultas
                                where co.idObra == idObra && co.idEmpresa == idEmpresa && co.esFracasada == null
                                select new consultasViewModels
                                {
                                    idConsulta = co.idConsulta,
                                    idObra = co.idObra,
                                    idEmpresa = co.idEmpresa,
                                    fecha = co.fecha,
                                    razonSocial = co.razonSocial,
                                    detalle = co.detalle,
                                    idPregunta = co.idPregunta,

                                };
                    var tmp = query.ToList();
                    var tmpPreguntas = tmp.Where(x => x.idPregunta == null).ToList();
                    var tmpRespuestas = tmp.Where(x => x.idPregunta != null).ToList();


                    // Recorrer grupos y agregar registros a la lista
                    foreach (var pregunta in tmpPreguntas)
                    {
                        lista.Add(pregunta);

                        foreach (var respuesta in tmpRespuestas)
                        {
                            if (pregunta.idConsulta == respuesta.idPregunta)
                            {
                                lista.Add(respuesta);
                            }
                        }
                    }

                }
            }
            catch (Exception ex)
            {
                log.Error("Error buscar Lista de consultas", ex);
            }
            return lista;
        }
        public async Task<int> grabarPregunta(consultasViewModels unaConsulta)
        {
            log4net.Config.XmlConfigurator.Configure(new FileInfo(ruta));
            log.Info("Guarda Pregunta");
            int value = 0;
            try
            {                
                rc_Empresa unaEmpresa = null;
                using (DB_RACOPEntities db = new DB_RACOPEntities())
                {
                   unaEmpresa = db.rc_Empresa.Where(x => x.idEmpresa == unaConsulta.idEmpresa).FirstOrDefault();
                }
                using (db_meieEntities db = new db_meieEntities())
                {
                    
                    SegUser unUsuario = null;
                    if(unaConsulta.idUsuario != null)
                    {
                        unUsuario = db.SegUser.Where(x => x.Id == unaConsulta.idUsuario).FirstOrDefault();
                    }
                    
                    
                    LicConsultas nuevaConsulta = new LicConsultas();
                    nuevaConsulta.fecha = DateTime.Now;
                    nuevaConsulta.idObra = unaConsulta.idObra;
                    nuevaConsulta.idEmpresa = unaConsulta.idEmpresa;                   
                    nuevaConsulta.razonSocial = (unUsuario?.FullName != null ? unUsuario.FullName + " - " : "") + (unaEmpresa?.razonSocial ?? "");


                    nuevaConsulta.detalle = unaConsulta.detalle;

                    db.LicConsultas.Add(nuevaConsulta);
                    await db.SaveChangesAsync();
                    //mail a oficina sobre la consulta de la empresa
                    enviarNotificacionConsulta(unaConsulta.idObra, 7, unaConsulta.idEmpresa);
                    //mail a empresa recibido de la oficina
                    enviarNotificacionConsulta(unaConsulta.idObra, 8, unaConsulta.idEmpresa);
                }
            }
            catch (Exception ex)
            {
                log.Error("Error al grabar consulta", ex);
            }
            return value;
        }
        public async Task<int> grabarRespuesta(consultasViewModels unaRespuesta)
        {
            log4net.Config.XmlConfigurator.Configure(new FileInfo(ruta));
            log.Info("Guarda Respuesta");
            int value = 0;
            try
            {               
                using (db_meieEntities db = new db_meieEntities())
                {
                    
                    LicConsultas nuevaRespuesta = new LicConsultas();
                    nuevaRespuesta.fecha = DateTime.Now;
                    nuevaRespuesta.idObra = unaRespuesta.idObra;
                    nuevaRespuesta.idEmpresa = unaRespuesta.idEmpresa;
                    nuevaRespuesta.detalle = unaRespuesta.detalle;
                    nuevaRespuesta.idPregunta = unaRespuesta.idPregunta;

                    db.LicConsultas.Add(nuevaRespuesta);
                    await db.SaveChangesAsync();
                    //mail a empresa respuesta oficina y copia a oficina
                    enviarNotificacionConsulta(unaRespuesta.idObra, 9, unaRespuesta.idEmpresa);
                }
            }
            catch (Exception ex)
            {
                log.Error("Error al grabar respuesta", ex);
            }
            return value;
        }
        public async Task enviarNotificacionConsulta(int? idObra, int mail, int? idEmpresa)
        {
            log4net.Config.XmlConfigurator.Configure(new FileInfo(ruta));
            log.Info("Envia mail Notificacion con Adjunto");
            var nombreObra = "";
            var nombreOrganismo = "";
            var nombreEmpresa = "";
            int? idOrganismo = 0;
            var mailEmpresa = "";
            var mailOrganismo = "";
          
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
                            nombreOrganismo = unOrganismo.NombreOrganismo;
                            log.Info("Envia mail Notificacion Organismo: " + mailOrganismo);
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
                                    nombreEmpresa = unEmpresa.razonSocial;
                                    log.Info("Envia mail Notificacion a: " + mailEmpresa);
                                }
                            }
                        }
                    
                }
               
            }

            ServicioEmail servicioEmail = new ServicioEmail();
            var emailViewModels = servicioEmail.buscarMail(mail);
           
            if (!string.IsNullOrEmpty(mailEmpresa))
            {
                emailViewModels.destino = mailEmpresa;
            }
            else
            {
                emailViewModels.destino = mailOrganismo;
            }
            if(mail == 8)
            {
                emailViewModels.copiaMail = mailOrganismo;
            }
            emailViewModels.cuerpo = emailViewModels.cuerpo.Replace("#Empresa", nombreEmpresa);
            emailViewModels.cuerpo = emailViewModels.cuerpo.Replace("#NombreObra", nombreObra);
            emailViewModels.cuerpo = emailViewModels.cuerpo.Replace("#Organismo", nombreOrganismo);

            // Enviar Mail
            await servicioEmail.enviarEmail(emailViewModels);
           
        }
    }
}

