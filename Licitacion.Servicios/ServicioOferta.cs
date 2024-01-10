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
    public class ServicioOferta
    {
        private static readonly log4net.ILog log = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
        private static string ruta = System.Web.HttpRuntime.AppDomainAppPath + "\\bin\\log4net.config";

        public List<EmpresaOfertaViewModels> listarEmpresasPorObra(int? idObra)
        {
            log4net.Config.XmlConfigurator.Configure(new FileInfo(ruta));
            log.Info("Listar Empresas que han ofertado en la obra");

            List<EmpresaOfertaViewModels> lista = new List<EmpresaOfertaViewModels>();
            List<EmpresaOfertaViewModels> listaFinal = new List<EmpresaOfertaViewModels>();
            try
            {
                using (db_meieEntities db = new db_meieEntities())
                {
                    var query = (from tb1 in db.LicOfertaEmpresa.Where(x=>x.activa ==1 && x.esFracasada == null)
                                 where tb1.idObra == idObra
                                 join tb2 in db.LicEmpresaHabilitada
                                 on tb1.idEmpresa equals tb2.idEmpresa into empresaHabilitadaGroup
                                 from tb2 in empresaHabilitadaGroup.DefaultIfEmpty()
                                 select new EmpresaOfertaViewModels
                                 {
                                     idObra = tb1.idObra,
                                     idEmpresa = tb1.idEmpresa,
                                     fechaOferta = tb1.fecha,
                                     mejoraOferta = tb2 != null ? tb2.mejoraoferta : false                                     
                                 });
                    lista = query.ToList();
                    
                }
                using (DB_RACOPEntities db1 = new DB_RACOPEntities())
                {
                    foreach (var item in lista)
                    {
                        var unEmpresa = db1.rc_Empresa.Where(x => x.idEmpresa == item.idEmpresa).FirstOrDefault();
                        if (unEmpresa != null)
                        {
                            item.nombreEmpresa = unEmpresa.razonSocial;
                            item.cuit = unEmpresa.cuit;
                        }
                    }
                }
                using (db_meieEntities db = new db_meieEntities())
                {                    

                    var query2 = (from hab in db.LicEmpresaHabilitada
                                  where hab.idObra == idObra && hab.idEmpresa == null && hab.esFracasada == null
                                  select new EmpresaOfertaViewModels
                                  {
                                      idObra = hab.idObra,
                                      idEmpresa = null,
                                      fechaOferta = null,
                                      mejoraOferta = hab.mejoraoferta,
                                      nombreEmpresa = hab.razonSocial,
                                      cuit = hab.cuit,
                                  });

                    var query2List = query2.ToList();


                    foreach (var item in query2List)
                    {
                        var empresaEnLista = lista.FirstOrDefault(e => e.idEmpresa == item.idEmpresa && e.idObra == idObra);
                        if (empresaEnLista != null)
                        {
                            empresaEnLista.mejoraOferta = item.mejoraOferta;
                        }
                    }
                    
                    lista = lista.Concat(query2List).ToList();
                    foreach(var item in lista)
                    {
                        var existe = listaFinal.Where(x => x.cuit == item.cuit).FirstOrDefault();
                        if(existe == null)
                        {
                            EmpresaOfertaViewModels empresaOferta = new EmpresaOfertaViewModels();
                            empresaOferta.cuit = item.cuit;
                            empresaOferta.fechaOferta = item.fechaOferta;
                            empresaOferta.idEmpresa = item.idEmpresa;
                            empresaOferta.idObra = item.idObra;
                            empresaOferta.mejoraOferta = item.mejoraOferta;
                            empresaOferta.nombreEmpresa = item.nombreEmpresa;
                            listaFinal.Add(empresaOferta);
                        }
                        else
                        {
                            if (item.mejoraOferta.Value)
                            {
                                existe.mejoraOferta = true;
                            }
                        }

                    }

                }
            }
            catch (Exception ex)
            {
                log.Error("Error en listarEmpresasPorObra", ex);
            }
            return listaFinal;
        }
        public List<ComprobanteOfertaViewModels> listarComprobantesEmpresasPorObra(int? idObra, int? idEmpresa)
        {
            log4net.Config.XmlConfigurator.Configure(new FileInfo(ruta));
            log.Info("Listar Empresas que han ofertado en la obra");

            List<ComprobanteOfertaViewModels> lista = new List<ComprobanteOfertaViewModels>();
            try
            {
                using (db_meieEntities db = new db_meieEntities())
                {
                    var query = (from tb1 in db.LicOfertaEmpresa
                                 where tb1.idObra == idObra && tb1.idEmpresa == idEmpresa && tb1.esFracasada == null
                                 select new ComprobanteOfertaViewModels
                                 {
                                     idArchivo = tb1.idOferta,
                                     fechaOferta = tb1.fecha,
                                     tipoOferta = tb1.tipoOferta
                                 });
                    lista = query.ToList();

                }
            }
            catch (Exception ex)
            {
                log.Error("Error en listarComprobantesEmpresasPorObra", ex);
            }
            return lista;
        }
    }
}
