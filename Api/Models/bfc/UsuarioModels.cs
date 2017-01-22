using Api.DBUtils;
using Api.POCOS;
using log4net;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;

namespace Api.Models.bfc
{
    public class UsuarioModels
    {
        internal static readonly ILog Log = LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
        public List<Usuario> getAll(int idEmpresa)
        {
            List<Usuario> result = null;
            Usuario nUsuario = null;
            DataSet dsUsuario = null;
            SqlUtils su = new SqlUtils();
            dsUsuario = su.GetAllUsuarios();
            if (DBUtils.DataSetUtils.hasRows(dsUsuario))
            {
                EmpresaModels em = new EmpresaModels();

                List<Empresa> empresas = em.GetEmpresas();
                List<FormaCobro> formaCobro = new FormaCobroModels().GetFormaCobro();
                List<EmpresaFormaCobro> empresaFormaCobro = new List<EmpresaFormaCobro>()
                {
                    new EmpresaFormaCobro() { Id = 1, Nombre = "Metro Cúbico" },
                    new EmpresaFormaCobro() { Id = 2, Nombre = "Metro Cuadrado" },
                    new EmpresaFormaCobro() { Id = 3, Nombre = "Posición Palet" }
                };
                List<Rol> roles = new RolModels().GetRoles();
                List<UsuarioRol> usuarioRol = new UsuarioRolModels().GetUsuarioRol();

                result = new List<Usuario>();
                foreach (DataRow dr in dsUsuario.Tables[0].Rows)
                {
                    nUsuario = new Usuario();
                    nUsuario.Apellidos = dr["Apellidos"].ToString();
                    nUsuario.Celular = dr["Celular"].ToString();
                    nUsuario.Direccion = dr["Direccion"].ToString();
                    nUsuario.Email = dr["Email"].ToString();
                    if (empresas != null && empresas.Count > 0)
                    {
                        foreach (Empresa empresa in empresas)
                        {
                            if (Convert.ToInt32(dr["Empresa"].ToString()) == empresa.Id)
                            {
                                nUsuario.Empresa = empresa;
                                if (empresaFormaCobro != null)
                                {
                                    foreach (EmpresaFormaCobro efc in empresaFormaCobro)
                                    {
                                        //if (efc.IdEmpresa == nUsuario.Empresa.Id)
                                        //{
                                            nUsuario.Empresa.IdFormaCobro = efc.Id;
                                            //nUsuario.Empresa.Valor = Convert.ToDouble(efc.Valor);
                                            foreach (FormaCobro fc in formaCobro)
                                            {
                                                if (nUsuario.Empresa.IdFormaCobro == fc.Id)
                                                {
                                                    nUsuario.Empresa.NombreFormaCobro = fc.Nombre;
                                                }
                                            }
                                        //}
                                    }
                                }                               
                            }
                        }                        
                    }
                                        
                    nUsuario.Id = dr["Id"].ToString();
                    nUsuario.IsActivo = Convert.ToBoolean(dr["IsActivo"]);
                    nUsuario.Nombres = dr["Nombres"].ToString();
                    if (usuarioRol != null)
                    {
                        foreach (UsuarioRol uRol in usuarioRol)
                        {
                            if (uRol.IdUsuario == nUsuario.Id)
                            {
                                nUsuario.Rol = new Rol();
                                nUsuario.Rol.Id = uRol.IdRol;
                                
                                if (roles!= null)
                                {
                                    foreach (Rol rol in roles)
                                    {
                                        if (nUsuario.Rol.Id == rol.Id)
                                        {
                                            nUsuario.Rol.Name = rol.Name;
                                        }
                                    }
                                }
                            }
                        }                        
                    }
                                        
                    nUsuario.Telefono = dr["PhoneNumber"].ToString();

                    result.Add(nUsuario);
                }

            }
            return result;
        }
    }
}
