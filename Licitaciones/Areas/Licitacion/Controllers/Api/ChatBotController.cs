using Licitacion.Servicios;
using Licitaciones.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Formatting;
using System.Web;
using System.Web.Http;
using System.Web.Mvc;
using Newtonsoft.Json.Linq;

namespace Licitaciones.Areas.Licitacion.Controllers.Api
{
    public class ChatBotController : ApiController
    {
        // GET: Licitacion/ChatBot
        [System.Web.Http.Route("api/ChatBot/buscaRespuestas")]
        [System.Web.Http.ActionName("buscaRespuestas")]
        [System.Web.Http.HttpGet]
        public List<ChatBotViewModels> buscaRespuestas(int? idPadre)
        {
            List<ChatBotViewModels> respuestas = new List<ChatBotViewModels>();
            
            try
            {
                ServicioChatBot servicio = new ServicioChatBot();
                respuestas = servicio.buscarRespuestas(idPadre);
                
            }
            catch (Exception ex)
            {
                //respuesta.codigo = 0;
                //respuesta.mensaje = "Ocurrio un error " + ex.Message;
            }
            return respuestas;
        }
        [System.Web.Http.Route("api/ChatBot/buscarEscenarios")]
        [System.Web.Http.ActionName("buscarEscenarios")]
        [System.Web.Http.HttpPost]
        public HttpResponseMessage buscarEscenarios(FormDataCollection form)
        {
            List<EscenariosChatBotViewModels> escenarios = new List<EscenariosChatBotViewModels>();

            try
            {
                var draw = form.Get("draw");
                var start = form.Get("start");
                var length = form.Get("length");
                var sortColumn = (form.Get("columns[" + form.Get("order[0][column]").FirstOrDefault() + "][data]").ToString()).ToString();
                var sortColumnDir = form.Get("order[0][dir]").ToString();
                var searchValue = form.Get("search[value]").ToString();

                int pageSize = length != null ? Convert.ToInt32(length) : 0;
                int skip = start != null ? Convert.ToInt32(start) : 0;
                int recordsTotal = 0;

                ServicioChatBot servicio = new ServicioChatBot();
                escenarios = servicio.buscarEscenarios();
                var json = Json(new { draw = draw, recordsFiltered = recordsTotal, recordsTotal = recordsTotal, data = escenarios });
                return Request.CreateResponse(HttpStatusCode.OK, new { draw = draw, recordsFiltered = recordsTotal, recordsTotal = recordsTotal, data = escenarios }, Configuration.Formatters.JsonFormatter);


            }
            catch (Exception ex)
            {
                return Request.CreateResponse(HttpStatusCode.InternalServerError, Configuration.Formatters.JsonFormatter);

                //respuesta.codigo = 0;
                //respuesta.mensaje = "Ocurrio un error " + ex.Message;
            }

        }
        [System.Web.Http.Route("api/ChatBot/guardarComentario")]
        [System.Web.Http.ActionName("guardarComentario")]
        [System.Web.Http.HttpPost]
        public int guardarComentario(ComentariosViewModels comentario)
        {
            var tmp = 400;
            try
            {
              
                ServicioChatBot servicio = new ServicioChatBot();
                return servicio.insetarComentario(comentario);
            }
            catch (Exception ex)
            {
            }
            return tmp;
        }
        [System.Web.Http.Route("api/ChatBot/guardarChat")]
        [System.Web.Http.ActionName("guardarChat")]
        [System.Web.Http.HttpPost]
        public int guardarChat(ChatBotViewModels chat)
        {
            var tmp = 400;
            try
            {

                ServicioChatBot servicio = new ServicioChatBot();
                return servicio.guardarChat(chat);
            }
            catch (Exception ex)
            {
            }
            return tmp;
        }
        [System.Web.Http.Route("api/ChatBot/buscarComentarios")]
        [System.Web.Http.ActionName("buscarComentarios")]
        [System.Web.Http.HttpPost]
        public HttpResponseMessage buscarComentarios(FormDataCollection form)
        {
            List<ComentariosViewModels> comentarios = new List<ComentariosViewModels>();

            try
            {
                var draw = form.Get("draw");
                var start = form.Get("start");
                var length = form.Get("length");
                var sortColumn = (form.Get("columns[" + form.Get("order[0][column]").FirstOrDefault() + "][data]").ToString()).ToString();
                var sortColumnDir = form.Get("order[0][dir]").ToString();
                var searchValue = form.Get("search[value]").ToString();

                int pageSize = length != null ? Convert.ToInt32(length) : 0;
                int skip = start != null ? Convert.ToInt32(start) : 0;
                int recordsTotal = 0;

                ServicioChatBot servicio = new ServicioChatBot();
                comentarios = servicio.buscarComentarios();
                recordsTotal = comentarios.Count;
                
                var json = Json(new { draw = draw, recordsFiltered = recordsTotal, recordsTotal = recordsTotal, data = comentarios });
                return Request.CreateResponse(HttpStatusCode.OK, new { draw = draw, recordsFiltered = recordsTotal, recordsTotal = recordsTotal, data = comentarios }, Configuration.Formatters.JsonFormatter);


            }
            catch (Exception ex)
            {
                return Request.CreateResponse(HttpStatusCode.InternalServerError, Configuration.Formatters.JsonFormatter);

                //respuesta.codigo = 0;
                //respuesta.mensaje = "Ocurrio un error " + ex.Message;
            }

        }
        [System.Web.Http.Route("api/ChatBot/obtenerEscenarioCompleto")]
        [System.Web.Http.ActionName("obtenerEscenarioCompleto")]
        [System.Web.Http.HttpPost]
        public object obtenerEscenarioCompleto(int? idEscenario)
        {
            List<ChatBotViewModels> comentarios = new List<ChatBotViewModels>();

            try
            {
               
                ServicioChatBot servicio = new ServicioChatBot();
                return servicio.obtenerEscenarioCompleto(idEscenario);

            }
            catch (Exception ex)
            {
                return comentarios;
                //respuesta.codigo = 0;
                //respuesta.mensaje = "Ocurrio un error " + ex.Message;
            }

        }
        [System.Web.Http.Route("api/ChatBot/buscarUnaPregunta")]
        [System.Web.Http.ActionName("buscarUnaPregunta")]
        [System.Web.Http.HttpGet]
        public ChatBotViewModels buscarUnaPregunta(int? idPregunta)
        {
            ChatBotViewModels comentarios = new ChatBotViewModels();

            try
            {

                ServicioChatBot servicio = new ServicioChatBot();
                return servicio.buscarUnaPregunta(idPregunta);

            }
            catch (Exception ex)
            {
                return comentarios;
                //respuesta.codigo = 0;
                //respuesta.mensaje = "Ocurrio un error " + ex.Message;
            }

        }
        [System.Web.Http.Route("api/ChatBot/buscarArbolActivo")]
        [System.Web.Http.ActionName("buscarArbolActivo")]
        [System.Web.Http.HttpGet]
        public int buscarArbolActivo()
        {

            try
            {

                ServicioChatBot servicio = new ServicioChatBot();
                
                return servicio.arbolEscenarioActual();

            }
            catch (Exception ex)
            {
                return 0;
                //respuesta.codigo = 0;
                //respuesta.mensaje = "Ocurrio un error " + ex.Message;
            }

        }
        [System.Web.Http.Route("api/ChatBot/activarDesactivar")]
        [System.Web.Http.ActionName("activarDesactivar")]
        [System.Web.Http.HttpPost]
        public int activarDesactivar(int? idEscenario)
        {
            
            try
            {

                ServicioChatBot servicio = new ServicioChatBot();
                return servicio.activarDesactivar(idEscenario);

            }
            catch (Exception ex)
            {
                return 400;
                //respuesta.codigo = 0;
                //respuesta.mensaje = "Ocurrio un error " + ex.Message;
            }

        }
        [System.Web.Http.Route("api/ChatBot/eliminarChat")]
        [System.Web.Http.ActionName("eliminarChat")]
        [System.Web.Http.HttpPost]
        public int eliminarChat(int? idChat)
        {
          

            try
            {
                if(idChat > 0)
                {
                    ServicioChatBot servicio = new ServicioChatBot();
                    return servicio.eliminarChat(idChat);
                }
                return 400;

            }
            catch (Exception ex)
            {
                return 400;
                //respuesta.codigo = 0;
                //respuesta.mensaje = "Ocurrio un error " + ex.Message;
            }

        }
    }
}