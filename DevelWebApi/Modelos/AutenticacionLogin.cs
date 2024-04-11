using Microsoft.Data.SqlClient;
using System.Data;
using System.Diagnostics;
using System.Reflection;

namespace DevelWebApi.Modelos
{
    public class AutenticacionLogin
    {
        public static List<login> GetUsuarios(IConfiguration configuration, string usuario, string contrasenia)
        {
            SqlTransaction transaccion = null;
            List<login> listadoUsuarios = new List<login>();

            try
            {
                string query = @"SELECT *
                                from fn_ConsultarUsuario( @Usuario, @Contrasenia)";

                using (SqlConnection conexion = new SqlConnection(configuration.GetConnectionString("CadenaSQL")))
                {
                    conexion.Open();

                    using (transaccion = conexion.BeginTransaction(IsolationLevel.ReadUncommitted))
                    {
                        using (SqlCommand cmd = new SqlCommand(query, conexion, transaccion))
                        {
                            cmd.Parameters.AddWithValue("@Usuario", usuario);
                            cmd.Parameters.AddWithValue("@Contrasenia", contrasenia);

                            using (SqlDataAdapter lDTA = new SqlDataAdapter(cmd))
                            {
                                DataTable tablaDatos = new DataTable();
                                lDTA.Fill(tablaDatos);

                                foreach (DataRow dr in tablaDatos.Rows)
                                {
                                    listadoUsuarios.Add(new login()
                                    {
                                        usuarioId = Convert.ToInt64(dr["UsuarioId"]),
                                        usuario = dr["Usuario"].ToString(),
                                        contrasenia = dr["Contrasenia"].ToString()
                                    });
                                }
                                transaccion.Commit();
                            }
                        }
                        conexion.Close();
                    }
                }

                return listadoUsuarios;
            }
            catch (SqlException ex)
            {
                if (transaccion is not null)
                    transaccion.Rollback();

                var st = new StackTrace();
                var sf = st.GetFrame(0);
                MethodBase nombreMetodo = null;
                if (sf != null) { nombreMetodo = sf.GetMethod(); }

                string error = string.Format("{0} {1}", nombreMetodo, ex.Message);

                throw new Exception(error);
            }
        }

        public static login GetUsuariosId(IConfiguration configuration, long usuarioId)
        {
            SqlTransaction transaccion = null;
            login usuario = new login();

            try
            {
                string query = @"SELECT *
                                from fn_ConsultarUsuarioPorId( @UsuarioId )";

                using (SqlConnection conexion = new SqlConnection(configuration.GetConnectionString("CadenaSQL")))
                {
                    conexion.Open();

                    using (transaccion = conexion.BeginTransaction(IsolationLevel.ReadUncommitted))
                    {
                        using (SqlCommand cmd = new SqlCommand(query, conexion, transaccion))
                        {
                            cmd.Parameters.AddWithValue("@UsuarioId", usuarioId);

                            using (SqlDataAdapter lDTA = new SqlDataAdapter(cmd))
                            {
                                DataTable tablaDatos = new DataTable();
                                lDTA.Fill(tablaDatos);

                                foreach (DataRow dr in tablaDatos.Rows)
                                {
                                    usuario.usuarioId = Convert.ToInt64(dr["UsuarioId"]);
                                    usuario.usuario = dr["Usuario"].ToString();
                                    usuario.contrasenia = dr["Contrasenia"].ToString();
                                }
                                transaccion.Commit();
                            }
                        }
                        conexion.Close();
                    }
                }

                return usuario;
            }
            catch (SqlException ex)
            {
                if (transaccion is not null)
                    transaccion.Rollback();

                var st = new StackTrace();
                var sf = st.GetFrame(0);
                MethodBase nombreMetodo = null;
                if (sf != null) { nombreMetodo = sf.GetMethod(); }

                string error = string.Format("{0} {1}", nombreMetodo, ex.Message);

                throw new Exception(error);
            }
        }
    }

    public class login
    {
        public long usuarioId { get; set; }
        public string usuario { get; set; }
        public string contrasenia { get; set; }
    }
}
