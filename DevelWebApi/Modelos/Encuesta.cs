using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;
using System.Data;
using System.Diagnostics;
using System.Reflection;

namespace DevelWebApi.Modelos
{
    public class OperacionEncuesta
    {
        public static List<Encuesta> GetEncuesta(IConfiguration configuration, List<long> pListadoEncuesta)
        {
            SqlTransaction transaccion = null;
            List<Encuesta> listadoEncuesta = new List<Encuesta>();

            try
            {
                string query = @"SELECT *
                                from fn_ConsultarEncuesta()";

                using (SqlConnection conexion = new SqlConnection(configuration.GetConnectionString("CadenaSQL")))
                {
                    conexion.Open();

                    using (transaccion = conexion.BeginTransaction(IsolationLevel.ReadUncommitted))
                    {
                        using (SqlCommand cmd = new SqlCommand(query, conexion, transaccion))
                        {
                            using (SqlDataAdapter lDTA = new SqlDataAdapter(cmd))
                            {
                                DataTable tablaDatos = new DataTable();
                                lDTA.Fill(tablaDatos);

                                foreach (DataRow dr in tablaDatos.Rows)
                                {
                                    listadoEncuesta.Add(new Encuesta()
                                    {
                                        EncuestaId = Convert.ToInt64(dr["EncuestaId"]),
                                        EncuestaDescripcion = dr["EncuestaDescripcion"].ToString(),
                                        Campos = GetCamposEncuesta(configuration, Convert.ToInt64(dr["EncuestaId"]))
                                    });
                                }
                                transaccion.Commit();
                            }
                        }
                        conexion.Close();
                    }
                }

                if (listadoEncuesta.Count > 0 && pListadoEncuesta.Count > 0)
                {
                    listadoEncuesta = listadoEncuesta.Where(encuesta => pListadoEncuesta.Contains(encuesta.EncuestaId)).ToList();
                }

                return listadoEncuesta;
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

        public static List<TipoCampo> GetTipoCampo(IConfiguration configuration)
        {
            SqlTransaction transaccion = null;
            List<TipoCampo> listadoTipoCampo = new List<TipoCampo>();

            try
            {
                string query = @"SELECT *
                                from fn_ConsultarTipoCampo()";

                using (SqlConnection conexion = new SqlConnection(configuration.GetConnectionString("CadenaSQL")))
                {
                    conexion.Open();

                    using (transaccion = conexion.BeginTransaction(IsolationLevel.ReadUncommitted))
                    {
                        using (SqlCommand cmd = new SqlCommand(query, conexion, transaccion))
                        {
                            using (SqlDataAdapter lDTA = new SqlDataAdapter(cmd))
                            {
                                DataTable tablaDatos = new DataTable();
                                lDTA.Fill(tablaDatos);

                                foreach (DataRow dr in tablaDatos.Rows)
                                {
                                    listadoTipoCampo.Add(new TipoCampo()
                                    {
                                        TipoCampoId = Convert.ToInt64(dr["TipoCampoId"]),
                                        TipoCampoDescripcion = dr["TipoCampoDescripcion"].ToString(),
                                        TipoCampoNombre = dr["TipoCampoNombre"].ToString()
                                    });
                                }
                                transaccion.Commit();
                            }
                        }
                        conexion.Close();
                    }
                }

                return listadoTipoCampo;
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

        public static void PostEncuesta(IConfiguration configuration, string pEncuestaDescripcion, List<pCampos> campos)
        {
            SqlTransaction transaccion = null;
            long idGenerado = -1;

            try
            {
                using (SqlConnection conexion = new SqlConnection(configuration.GetConnectionString("CadenaSQL")))
                {
                    conexion.Open();

                    using (transaccion = conexion.BeginTransaction(IsolationLevel.ReadUncommitted))
                    {
                        using (SqlCommand cmd = new SqlCommand("sp_InsertarEncuesta", conexion, transaccion))
                        {
                            cmd.CommandType = CommandType.StoredProcedure;
                            cmd.Parameters.AddWithValue("@pEncuestaDescripcion", pEncuestaDescripcion);

                            var identity = cmd.ExecuteScalar();

                            
                            idGenerado = Convert.ToInt64(identity);
                        }
                        transaccion.Commit();
                    }
                    conexion.Close();
                }

                if (idGenerado > 0 && campos.Count > 0)
                {
                    foreach (pCampos item in campos)
                    {
                        PostCampos(configuration, idGenerado, item);
                    }
                }
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

        public static void PostCampos(IConfiguration configuration, long pEncuestaId, pCampos pCampos)
        {
            SqlTransaction transaccion = null;

            try
            {
                using (SqlConnection conexion = new SqlConnection(configuration.GetConnectionString("CadenaSQL")))
                {
                    conexion.Open();

                    using (transaccion = conexion.BeginTransaction(IsolationLevel.ReadUncommitted))
                    {
                        using (SqlCommand cmd = new SqlCommand("sp_InsertarCampoEncuesta", conexion, transaccion))
                        {
                            cmd.CommandType = CommandType.StoredProcedure;
                            cmd.Parameters.AddWithValue("@pEncuestaId", pEncuestaId);
                            cmd.Parameters.AddWithValue("@pCampoNombre", pCampos.CampoNombre);
                            cmd.Parameters.AddWithValue("@pCampoTitulo", pCampos.CampoTitulo);
                            cmd.Parameters.AddWithValue("@pCampoEsRequerido", pCampos.CampoEsRequerido);
                            cmd.Parameters.AddWithValue("@pIdTipoCampo", pCampos.TipoCampo);
                            cmd.ExecuteNonQuery();
                        }
                        transaccion.Commit();
                    }
                    conexion.Close();
                }
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

        public static void PutEncuesta(IConfiguration configuration, long pEncuestaId, string pEncuestaDescripcion)
        {
            SqlTransaction transaccion = null;

            try
            {
                using (SqlConnection conexion = new SqlConnection(configuration.GetConnectionString("CadenaSQL")))
                {
                    conexion.Open();

                    using (transaccion = conexion.BeginTransaction(IsolationLevel.ReadUncommitted))
                    {
                        using (SqlCommand cmd = new SqlCommand("sp_ModificarEncuesta", conexion, transaccion))
                        {
                            cmd.CommandType = CommandType.StoredProcedure;
                            cmd.Parameters.AddWithValue("@pEncuestaId", pEncuestaId);
                            cmd.Parameters.AddWithValue("@pEncuestaDescripcion", pEncuestaDescripcion);
                            cmd.ExecuteNonQuery();
                        }
                        transaccion.Commit();
                    }
                    conexion.Close();
                }
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

        public static void PutCampo(IConfiguration configuration, long campoId, pCampos campo)
        {
            SqlTransaction transaccion = null;

            try
            {
                using (SqlConnection conexion = new SqlConnection(configuration.GetConnectionString("CadenaSQL")))
                {
                    conexion.Open();

                    using (transaccion = conexion.BeginTransaction(IsolationLevel.ReadUncommitted))
                    {
                        using (SqlCommand cmd = new SqlCommand("sp_ModificarCampo", conexion, transaccion))
                        {
                            cmd.CommandType = CommandType.StoredProcedure;
                            cmd.Parameters.AddWithValue("@pCampoId", campoId);
                            cmd.Parameters.AddWithValue("@pCampoNombre", campo.CampoNombre);
                            cmd.Parameters.AddWithValue("@pCampoTitulo", campo.CampoTitulo);
                            cmd.Parameters.AddWithValue("@pCampoEsRequerido", campo.CampoEsRequerido);
                            cmd.Parameters.AddWithValue("@pTipoCampo", campo.TipoCampo);
                            cmd.ExecuteNonQuery();
                        }
                        transaccion.Commit();
                    }
                    conexion.Close();
                }
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

        public static void DeleteEncuesta(IConfiguration configuration, long encuestaId)
        {
            SqlTransaction transaccion = null;

            try
            {
                using (SqlConnection conexion = new SqlConnection(configuration.GetConnectionString("CadenaSQL")))
                {
                    conexion.Open();

                    using (transaccion = conexion.BeginTransaction(IsolationLevel.ReadUncommitted))
                    {
                        using (SqlCommand cmd = new SqlCommand("sp_EliminarEncuesta", conexion, transaccion))
                        {
                            cmd.CommandType = CommandType.StoredProcedure;
                            cmd.Parameters.AddWithValue("@pEncuestaId", encuestaId);
                            cmd.ExecuteNonQuery();
                        }
                        transaccion.Commit();
                    }
                    conexion.Close();
                }
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

        public static List<Campos> GetCamposEncuesta(IConfiguration configuration, long pEncuestaId)
        {
            SqlTransaction transaccion = null;
            List<Campos> listadoCampos = new List<Campos>();

            try
            {
                string query = @"SELECT *
                                from fn_ConsultarCamposEncuesta(@encuestaId)";

                using (SqlConnection conexion = new SqlConnection(configuration.GetConnectionString("CadenaSQL")))
                {
                    conexion.Open();

                    using (transaccion = conexion.BeginTransaction(IsolationLevel.ReadUncommitted))
                    {
                        using (SqlCommand cmd = new SqlCommand(query, conexion, transaccion))
                        {
                            cmd.Parameters.AddWithValue("@encuestaId", pEncuestaId);

                            using (SqlDataAdapter lDTA = new SqlDataAdapter(cmd))
                            {
                                DataTable tablaDatos = new DataTable();
                                lDTA.Fill(tablaDatos);

                                foreach (DataRow dr in tablaDatos.Rows)
                                {
                                    listadoCampos.Add(new Campos()
                                    {
                                        CampoId = Convert.ToInt64(dr["CampoId"]),
                                        CampoNombre = dr["CampoNombre"].ToString(),
                                        CampoTitulo = dr["CampoTitulo"].ToString(),
                                        CampoEsRequerido = Convert.ToBoolean(dr["CampoEsRequerido"].ToString()),
                                        IdEncuesta = Convert.ToInt64(dr["IdEncuesta"]),
                                        TipoCampo = dr["TipoCampo"].ToString(),
                                    });
                                }
                                transaccion.Commit();
                            }
                        }
                        conexion.Close();
                    }
                }
                return listadoCampos;
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



    public class Encuesta
    {
        public long EncuestaId { get; set; }
        public string EncuestaDescripcion { get; set; }
        public List<Campos> Campos { get; set; }
    }
    public class pCampos
    {
        public string CampoNombre { get; set; }
        public string CampoTitulo { get; set; }
        public bool CampoEsRequerido { get; set; }
        public byte TipoCampo { get; set; }
    }

    public class Campos
    {
        public long CampoId { get; set; }
        public string CampoNombre { get; set; }
        public string CampoTitulo { get; set; }
        public bool CampoEsRequerido { get; set; }
        public long IdEncuesta { get; set; }
        public string TipoCampo { get; set; }
    }

    public class TipoCampo
    {
        public long TipoCampoId { get; set;}
        public string TipoCampoDescripcion { get; set; }
        public string TipoCampoNombre { get; set; }
    }
}
