using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Licitaciones.BaseDato;
using Licitaciones.ViewModels;

namespace Licitacion.Servicios.Utiles
{
    public class AuditoriaRacop
    {
        public void AddLogLogin(int UserEmpresa)
        {
            using (var db = new DB_RACOPEntities ())
            {
                UsuarioAudit user = (from emus in db.rc_EmpresaUser
                                     where emus.id == UserEmpresa
                                     join us in db.rc_User on emus.idUser
                                    equals us.idUser
                                     join emp in db.rc_Empresa on emus.idEmpresa
                                     equals emp.idEmpresa
                                     select new UsuarioAudit
                                     {
                                         nombreApellido = us.nombreApellido,
                                         nombreEmpresa = emp.razonSocial,
                                         IdEmpresa = emp.idEmpresa,
                                         idUser = us.idUser
                                     }

                                 ).FirstOrDefault();

        rc_Audit au = new rc_Audit
        {
            FechaHora = DateTime.Now,
            IdUserEmpresa = UserEmpresa,
            Data = "El usuario " + user.nombreApellido + " con id: " + user.idUser + ", se logueo con la empresa: " + user.nombreEmpresa + " de id: " + user.IdEmpresa,
            idAuditActions = 7
        };
        db.rc_Audit.Add(au);
                db.SaveChanges();
            }
        }
        public void AddLogCreate(int User, int? empresa, String Data)
        {
            using (var db = new DB_RACOPEntities())
            {
                UsuarioAudit user = (from emus in db.rc_EmpresaUser
                                     where emus.idUser == User && emus.idEmpresa == empresa
                                     join us in db.rc_User on emus.idUser
                                     equals us.idUser
                                     join emp in db.rc_Empresa on emus.idEmpresa
                                     equals emp.idEmpresa
                                     select new UsuarioAudit
                                     {
                                         IdEmpresaUser = emus.id,
                                         nombreApellido = us.nombreApellido,
                                         nombreEmpresa = emp.razonSocial,
                                         IdEmpresa = emp.idEmpresa,
                                     }

                                 ).FirstOrDefault();

                if(user != null)
                {
                    rc_Audit au = new rc_Audit
                    {
                        FechaHora = DateTime.Now,
                        IdUserEmpresa = user.IdEmpresaUser,
                        Data = "El usuario " + user.nombreApellido + Data + ". En la empresa: " + user.nombreEmpresa + " de id: " + user.IdEmpresa,
                        idAuditActions = 1
                    };
                    db.rc_Audit.Add(au);

                    db.SaveChanges();
                }
            }
        }
        public void AddLogEnvioTramite(int User, int? empresa, String Data)
        {
            using (var db = new DB_RACOPEntities())
            {
                UsuarioAudit user = (from emus in db.rc_EmpresaUser
                                     where emus.idUser == User && emus.idEmpresa == empresa
                                     join us in db.rc_User on emus.idUser
                                     equals us.idUser
                                     join emp in db.rc_Empresa on emus.idEmpresa
                                     equals emp.idEmpresa
                                     select new UsuarioAudit
                                     {
                                         IdEmpresaUser = emus.id,
                                         nombreApellido = us.nombreApellido,
                                         nombreEmpresa = emp.razonSocial,
                                         IdEmpresa = emp.idEmpresa,
                                     }

                                 ).FirstOrDefault();

                rc_Audit au = new rc_Audit
                {
                    FechaHora = DateTime.Now,
                    IdUserEmpresa = user.IdEmpresaUser,
                    Data = "El usuario " + user.nombreApellido + Data + ". En la empresa: " + user.nombreEmpresa + " de id: " + user.IdEmpresa,
                    idAuditActions = 1
                };
                db.rc_Audit.Add(au);

                db.SaveChanges();
            }
        }
    }
}
