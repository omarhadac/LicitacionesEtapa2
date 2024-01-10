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
    public class ServicioChatBot
    {
        private static readonly log4net.ILog log = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
        private static string ruta = System.Web.HttpRuntime.AppDomainAppPath + "\\bin\\log4net.config";

        public List<ChatBotViewModels> buscarRespuestas(int? idOpcion)
        {
            List<ChatBotViewModels> respuestas = new List<ChatBotViewModels>();
            log4net.Config.XmlConfigurator.Configure(new FileInfo(ruta));
            log.Info("Buscar Respuestas ChatBot");
            var tipoEmpresa = System.Web.HttpContext.Current.Session["tipoEmpresa"].ToString();
            var idTipoEmpresa = Convert.ToInt32(tipoEmpresa);
            try
            {
                using (db_meieEntities db = new db_meieEntities())
                {
                    respuestas = (from opciones in db.LicChatbot
                                  where opciones.HijoDe == idOpcion && opciones.IdEscenario == idTipoEmpresa
                                  select new ChatBotViewModels
                                  {
                                      idChat = opciones.idLicChatbot,
                                      Opcion = opciones.Opcion,
                                      TituloPpal = opciones.TituloOpcion,
                                      MensajePpal = opciones.MensajePpal
                                  }).ToList();

                }
               
            }
            
            catch (Exception ex)
            {
                log.Error("Error Respuestas Chatbot " + ex.Message);
            }
            return respuestas;
        }
        public ChatBotViewModels buscarUnaPregunta(int? idOpcion)
        {
            ChatBotViewModels respuesta = new ChatBotViewModels();
            log4net.Config.XmlConfigurator.Configure(new FileInfo(ruta));
            log.Info("Buscar Respuestas ChatBot");
            try
            {
                if(idOpcion > 0)
                {
                    using (db_meieEntities db = new db_meieEntities())
                    {
                        respuesta = (from opciones in db.LicChatbot
                                     where opciones.idLicChatbot == idOpcion
                                     select new ChatBotViewModels
                                     {
                                         idChat = opciones.idLicChatbot,
                                         Opcion = opciones.Opcion,
                                         TituloPpal = opciones.TituloOpcion,
                                         MensajePpal = opciones.MensajePpal,
                                         idPadre = opciones.HijoDe,
                                         tipo = 3
                                     }).FirstOrDefault();
                        respuesta.esFin = db.LicChatbot.Where(h => h.HijoDe == idOpcion).ToList().Count;
                        if (respuesta.idPadre == null)
                        {
                            respuesta.tipo = 1; //soy escenario
                        }
                        if (respuesta.Opcion.Contains("button"))
                        {
                            respuesta.tipo = 2; //soy un botón ó topico
                        }
                        


                    }
                }
                             
                

            }
            catch (Exception ex)
            {
                log.Error("Error Respuestas Chatbot " + ex.Message);
            }
            return respuesta;
        }
        public List<EscenariosChatBotViewModels> buscarEscenarios()
        {
            List<EscenariosChatBotViewModels> escenarios = new List<EscenariosChatBotViewModels>();
            log4net.Config.XmlConfigurator.Configure(new FileInfo(ruta));
            log.Info("Buscar Escenarios");
            try
            {
                using (db_meieEntities db = new db_meieEntities())
                {
                    escenarios = (from opciones in db.LicEscenarioChatbot
                                    select new EscenariosChatBotViewModels
                                    {
                                      accion= "",
                                      activo = opciones.Activo.Value.ToString() == "1"? "Activo" : "Inactivo",
                                      descripcion = opciones.Descripcion,
                                      idEscenario = opciones.idLicEscenarioChatbot,
                                      sector = opciones.Sector,
                                      nombreEscenario = opciones.NombreEscenario
                                  }).ToList();

                }
            }
            catch (Exception ex)
            {
                log.Error("Error Respuestas Chatbot " + ex.Message);
            }
            return escenarios;
        }
        public int insetarComentario(ComentariosViewModels comentario)
        {
            List<EscenariosChatBotViewModels> escenarios = new List<EscenariosChatBotViewModels>();
            log4net.Config.XmlConfigurator.Configure(new FileInfo(ruta));
            log.Info("Insertar Comentario");
            try
            {
                using (db_meieEntities db = new db_meieEntities())
                {
                    var usuario = System.Web.HttpContext.Current.Session["idUsuario"].ToString();
                    var tipoEmpresa = System.Web.HttpContext.Current.Session["tipoEmpresa"].ToString();
                    LicCalificacionChatbot nuevoComentario = new LicCalificacionChatbot
                    {
                        Comentario = comentario.comentario,
                        FechaCalificacion = DateTime.Now,
                        IdEscenario = Convert.ToInt32(tipoEmpresa),
                        IdPregunta = Convert.ToInt32(usuario)

                    };
                    db.LicCalificacionChatbot.Add(nuevoComentario);
                    db.SaveChanges();
                    return 200;

                }
            }
            catch (Exception ex)
            {
                log.Error("Error Comentario Chatbot " + ex.Message);
                return 400;
            }
        }
        public List<ComentariosViewModels> buscarComentarios()
        {
            List<ComentariosViewModels> comentarios = new List<ComentariosViewModels>();
            log4net.Config.XmlConfigurator.Configure(new FileInfo(ruta));
            log.Info("Buscar Comentarios");
            try
            {
                using (db_meieEntities db = new db_meieEntities())
                {
                    comentarios = (from opciones in db.LicCalificacionChatbot
                                  join u in db.SegUser on opciones.IdPregunta equals u.Id 
                                  select new ComentariosViewModels
                                  {
                                      comentario = opciones.Comentario,
                                      fechaHora = opciones.FechaCalificacion,
                                      idComentario = opciones.idLicCalificacionChatbot,
                                      idUsuario = u.Id,
                                      nombreUsuario = u.FullName
                                  }).ToList();

                }
            }
            catch (Exception ex)
            {
                log.Error("Error Respuestas Chatbot " + ex.Message);
            }
            return comentarios;
        }
        public object obtenerEscenarioCompleto(int? idEscenario)
        {
            List<ChatBotViewModels> respuestas = new List<ChatBotViewModels>();
            var escenario = "";
            log4net.Config.XmlConfigurator.Configure(new FileInfo(ruta));
            log.Info("Buscar Escenarios");
            try
            {
                using (db_meieEntities db = new db_meieEntities())
                {
                    respuestas = (from opciones in db.LicChatbot
                                  where opciones.IdEscenario == idEscenario
                                  select new ChatBotViewModels
                                  {
                                      idChat = opciones.idLicChatbot,
                                      TituloOpcion = opciones.TituloOpcion,
                                      MensajePpal = opciones.MensajePpal,
                                      Opcion = opciones.Opcion,
                                      HijoDe = opciones.HijoDe,
                                      IdEscenario = opciones.HijoDe,
                                      IdLicChatbot = opciones.idLicChatbot
                                  }).ToList();

                    escenario = db.LicEscenarioChatbot.Where(x => x.idLicEscenarioChatbot == idEscenario).FirstOrDefault().Sector;
                }
                respuestas = ArmarArbol(respuestas);
                
                
            }
            catch (Exception ex)
            {
                log.Error("Error al obtener escenario completo " + ex.Message);
            }
            var mensajeSaludo = respuestas[0].MensajePpal;
            if(respuestas[0].MensajePpal != respuestas[0].Opcion)
            {
                mensajeSaludo +=  ", " + respuestas[0].Opcion;
            }

            var datosDesdeBackend = new
            {
                
                escenario = escenario + " - Saludo Inicial: " + mensajeSaludo,
                id = respuestas[0].idChat,
                topicos = respuestas[0].Hijos?.Select(topico => new
                {
                    topico = topico.TituloOpcion, 
                    id = topico.idChat,
                    preguntas = topico.Hijos?.Select(pregunta => new
                    {
                        id = pregunta.idChat,
                        pregunta = pregunta.TituloOpcion , 
                        respuesta = pregunta.Hijos.Count > 0 ? pregunta.Hijos[0]?.MensajePpal + pregunta.Hijos[0]?.Opcion : "Sin Respuestas registradas",
                        idHijoFinal = pregunta.Hijos.Count > 0 ? pregunta.Hijos[0]?.idChat: 0
                    }).ToList()
                }).ToList()
            };
            return datosDesdeBackend;
        }
        public List<ChatBotViewModels> ArmarArbol(List<ChatBotViewModels> licChatbots)
        {
            Dictionary<int, List<ChatBotViewModels>> parentToChild = new Dictionary<int, List<ChatBotViewModels>>();

            foreach (var licChatbot in licChatbots)
            {
                if (licChatbot.HijoDe != null)
                {
                    if (!parentToChild.ContainsKey(licChatbot.HijoDe.Value))
                    {
                        parentToChild[licChatbot.HijoDe.Value] = new List<ChatBotViewModels>();
                    }
                    parentToChild[licChatbot.HijoDe.Value].Add(licChatbot);
                }
            }

            List<ChatBotViewModels> topLevel = new List<ChatBotViewModels>();

            foreach (var licChatbot in licChatbots)
            {
                if (licChatbot.HijoDe == null)
                {
                    AsignarHijosRecursivo(licChatbot, parentToChild);
                    topLevel.Add(licChatbot);
                }
            }
            
            return topLevel;
        }

        private void AsignarHijosRecursivo(ChatBotViewModels parent, Dictionary<int, List<ChatBotViewModels>> parentToChild)
        {
            if (parentToChild.ContainsKey(parent.IdLicChatbot))
            {
                parent.Hijos = parentToChild[parent.IdLicChatbot];
                foreach (var child in parent.Hijos)
                {
                    AsignarHijosRecursivo(child, parentToChild);
                }
            }
        }
        public int arbolEscenarioActual()
        {
            var tipoEmpresa = System.Web.HttpContext.Current.Session["tipoEmpresa"].ToString();
            if(Convert.ToInt32(tipoEmpresa) == 2)
            {
                var escenarioActivo = buscarEscenarios().Where(x => x.sector == "Empresa" && x.activo == "Activo").Count();
                System.Web.HttpContext.Current.Session["chatBotActiveEmpresa"] = escenarioActivo;
                return escenarioActivo;
            }
            else
            {
                var escenarioActivo = buscarEscenarios().Where(x => x.sector == "Oficina" && x.activo == "Activo").Count();
                System.Web.HttpContext.Current.Session["chatBotActiveOficina"] = escenarioActivo;
                return escenarioActivo;
            }
           
        }
        public int guardarChat(ChatBotViewModels chat)
        {
            List<EscenariosChatBotViewModels> escenarios = new List<EscenariosChatBotViewModels>();
            log4net.Config.XmlConfigurator.Configure(new FileInfo(ruta));
            log.Info("Insertar chatbot");
            try
            {
                using (db_meieEntities db = new db_meieEntities())
                {
                    if(chat.idPadre > 0)
                    {
                        LicChatbot nuevo = new LicChatbot()
                        {
                            HijoDe = chat.idPadre,
                            IdEscenario = chat.IdEscenario
                        };
                        db.LicChatbot.Add(nuevo);
                        db.SaveChanges();
                        chat.idChat = nuevo.idLicChatbot;
                    }
                    if(chat.idChat > 0)
                    {
                        var chatdb = db.LicChatbot.Where(x => x.idLicChatbot == chat.idChat).FirstOrDefault();
                        chatdb.TituloOpcion = chat.MensajePpal;
                        var esFin = chat.esFin == 1 ? "true" : "false";
                        if(chat.tipo == 0)
                        {
                            //es boton
                            chatdb.Opcion = "<button class=\"option-button\" onclick=\"showResponse(" + chatdb.idLicChatbot + ",'" + chat.MensajePpal + "'," + esFin.ToString() + ")\">" + chat.MensajePpal + "</button>";
                        }
                        else
                        {
                            if(chatdb.Opcion == chat.MensajePpal)
                            {
                                chatdb.Opcion = "";
                            }
                            else
                            {
                                chatdb.Opcion = chat.MensajePpal;
                            }
                            
                        }
                        if(chat.tipoPregunta == 0)
                        {
                            string[] partes = chat.MensajePpal.Split('.');
                            if(partes.Count() > 1)
                            {
                                string primeraParte = partes[0].Trim();
                                string segundaParte = partes[1].Trim();
                                if (chat.tipo == 0)
                                {
                                    if (primeraParte != null)
                                    {
                                        chatdb.MensajePpal = primeraParte;
                                    }
                                    if (segundaParte != null)
                                    {
                                        chatdb.MensajePpal += segundaParte;
                                    }
                                }
                                else
                                {
                                    if (primeraParte != null)
                                    {
                                        chatdb.MensajePpal = primeraParte;
                                    }
                                    if (segundaParte != null)
                                    {
                                        chatdb.Opcion = segundaParte;
                                    }
                                }
                            }
                            else
                            {
                                chatdb.MensajePpal = chat.MensajePpal;
                            }
                            db.SaveChanges();
                        }
                        if(chat.tipoPregunta == 1)
                        {
                            if(chat.tipo == 0)
                            {
                                chatdb.MensajePpal = "En qué puedo ayudarte? (Seleccione una opción)";
                            }
                            else
                            {
                                string[] partes = chat.MensajePpal.Split('.');
                                if (partes.Count() > 1)
                                {
                                    string primeraParte = partes[0].Trim();
                                    string segundaParte = partes[1].Trim();
                                    if (chat.tipo == 0)
                                    {
                                        if (primeraParte != null)
                                        {
                                            chatdb.MensajePpal = primeraParte;
                                        }
                                        if (segundaParte != null)
                                        {
                                            chatdb.MensajePpal += segundaParte;
                                        }
                                    }
                                    else
                                    {
                                        if (primeraParte != null)
                                        {
                                            chatdb.MensajePpal = primeraParte;
                                        }
                                        if (segundaParte != null)
                                        {
                                            chatdb.Opcion = segundaParte;
                                        }
                                    }
                                }
                            }
                        }
                        if(chat.tipoPregunta == 2)
                        {
                            if(chat.tipo == 0)
                            {
                                var elPadre = db.LicChatbot.Where(x => x.idLicChatbot == chatdb.HijoDe).FirstOrDefault();
                                if (elPadre != null)
                                {
                                    chatdb.MensajePpal = "Conozco mucho sobre " + elPadre.TituloOpcion + ". Puedo ayudarte con: ";
                                }
                            }
                            else
                            {
                                string[] partes = chat.MensajePpal.Split('.');
                                if (partes.Count() > 1)
                                {
                                    string primeraParte = partes[0].Trim();
                                    string segundaParte = partes[1].Trim();
                                    if (primeraParte != null)
                                    {
                                        chatdb.MensajePpal = primeraParte;
                                    }
                                    if (segundaParte != null)
                                    {
                                        chatdb.Opcion = segundaParte;
                                    }
                                }
                            }

                            
                        }
                        if (chat.tipoPregunta == 3)
                        {
                            if (chat.tipo == 0)
                            {
                                var elPadre = db.LicChatbot.Where(x => x.idLicChatbot == chatdb.HijoDe).FirstOrDefault();
                                if (elPadre != null)
                                {
                                    chatdb.MensajePpal = "Puedo ayudarte con lo siguiente: ";
                                    elPadre.Opcion.Replace("False", "True");
                                    db.SaveChanges();
                                }
                            }
                            else
                            {
                                string[] partes = chat.MensajePpal.Split('.');
                                if (partes.Count() > 1)
                                {
                                    string primeraParte = partes[0].Trim();
                                    string segundaParte = partes[1].Trim();
                                    if (primeraParte != null)
                                    {
                                        chatdb.MensajePpal = primeraParte;
                                    }
                                    if (segundaParte != null)
                                    {
                                        chatdb.Opcion = segundaParte;
                                    }
                                }
                            }

                        }
                    }
                   
                    db.SaveChanges();
                    return 200;

                }
            }
            catch (Exception ex)
            {
                log.Error("Error insertar Chatbot " + ex.Message);
                return 400;
            }
        }
        public int activarDesactivar(int? idEscenario)
        {
            List<EscenariosChatBotViewModels> escenarios = new List<EscenariosChatBotViewModels>();
            log4net.Config.XmlConfigurator.Configure(new FileInfo(ruta));
            log.Info("Activar o desactivar escenarios");
            try
            {
                using (db_meieEntities db = new db_meieEntities())
                {
                    var escenario = db.LicEscenarioChatbot.Where(x => x.idLicEscenarioChatbot == idEscenario).FirstOrDefault();
                    escenario.Activo = (sbyte?)((escenario.Activo == '1') ? 0 : 1);

                    db.SaveChanges();
                    return 200;

                }
            }
            catch (Exception ex)
            {
                log.Error("Error al activar o desactivar escenarios " + ex.Message);
                return 400;
            }
        }
        public int eliminarChat(int? idChat)
        {
            List<EscenariosChatBotViewModels> escenarios = new List<EscenariosChatBotViewModels>();
            log4net.Config.XmlConfigurator.Configure(new FileInfo(ruta));
            log.Info("Activar o desactivar escenarios");
            try
            {
                using (db_meieEntities db = new db_meieEntities())
                {
                    var escenario = db.LicChatbot.Where(x => x.idLicChatbot == idChat).FirstOrDefault();
                    db.LicChatbot.Remove(escenario);

               
                    var listadosEsc = db.LicChatbot.Where(x => x.HijoDe == idChat).ToList();
                    foreach(var item in listadosEsc)
                    {
                        db.LicChatbot.Remove(item);
                    }
                    db.SaveChanges();
                    return 200;

                }
            }
            catch (Exception ex)
            {
                log.Error("Error al activar o desactivar escenarios " + ex.Message);
                return 400;
            }
        }
    }
}
