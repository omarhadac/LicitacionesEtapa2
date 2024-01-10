﻿using Licitacion.Servicios;
using Licitaciones.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace Licitaciones.Areas.Licitacion.Controllers.Api 
{
    public class EtapaController : ApiController
    {
        /// <summary>
        /// Lista las Etapas disponibles solo para Licitacion
        /// </summary>
        /// <returns>Lista total de Etapas filtradas</returns>
        /// <response code="200">Ok devuelve lista</response>
        [Route("api/Etapa/Listar")]
        [System.Web.Http.ActionName("Listar")]
        [System.Web.Http.HttpGet]
        public Respuesta Listar()
        {
            Respuesta respuesta = new Respuesta();
            ServicioEtapa servicio = new ServicioEtapa();
            var lista = servicio.listar();
            respuesta.codigo = 1;
            respuesta.replay = lista;
            return respuesta;
        }
        [Route("api/Etapa/listarEtapaEmpresa")]
        [System.Web.Http.ActionName("listarEtapaEmpresa")]
        [System.Web.Http.HttpGet]
        public Respuesta listarEtapaEmpresa()
        {
            Respuesta respuesta = new Respuesta();
            ServicioEtapa servicio = new ServicioEtapa();
            var lista = servicio.listarEtapaEmpresa();
            respuesta.codigo = 1;
            respuesta.replay = lista;
            return respuesta;
        }
    }
}
