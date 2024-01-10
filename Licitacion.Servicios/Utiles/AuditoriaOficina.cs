using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Licitaciones.BaseDato;
using Licitaciones.ViewModels;

namespace Licitacion.Servicios.Utiles
{
    public class AuditoriaOficina
    {
        public void AddLogEdit(int accessId, int accionId, string data, SegUser usuario, int idObra)
        {
            using (db_meieEntities db = new db_meieEntities())
            {
                //IPHostEntry IPAddress = Dns.GetHostEntry(Dns.GetHostName());
                System.Web.HttpContext contexts = System.Web.HttpContext.Current;
                //var IpAddress = HttpContext.Current.Session["IpData"].ToString();
                var IpAddress = "";


                //IPAddress.AddressList[2].ToString();

                AudLogAccion la = new AudLogAccion
                {
                    Date = DateTime.Now,
                    User_Id = usuario.Id,
                    Accion_Id = accionId,
                    Access_Id = accessId,
                    Data = "El usuario " + usuario.FullName + " con el id= " + usuario.Id + " ha modificado " + data,
                    Ip = IpAddress,
                    ProyectoId = idObra

                };
                db.AudLogAccion.Add(la);
                db.SaveChanges();
            }
            

        }
        public void AddLogCambios(int accessId, int accionId, string data, int usuario, int? idObra)
        {
            //IPHostEntry IPAddress = Dns.GetHostEntry(Dns.GetHostName());
            System.Web.HttpContext contexts = System.Web.HttpContext.Current;
            //var IpAddress = HttpContext.Current.Session["IpData"].ToString();
            var IpAddress = " ";

            //IPAddress.AddressList[2].ToString();
            using (db_meieEntities db = new db_meieEntities())
            {
                AudLogAccion la = new AudLogAccion
                {
                    Date = DateTime.Now,
                    User_Id = usuario,
                    Accion_Id = accionId,
                    Access_Id = accessId,
                    Data = data,
                    Ip = IpAddress,
                    ProyectoId = Convert.ToInt32(idObra)

                };
                db.AudLogAccion.Add(la);
                db.SaveChanges();
            }
              

        }
    }
}
