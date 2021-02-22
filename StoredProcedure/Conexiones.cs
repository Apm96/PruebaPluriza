using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;

namespace ProjectSales
{    
    public class Conexiones
    {
        public Conexiones() { }

        public IConfiguration configuration_ConnectionString;

        public Conexiones(IConfiguration configuration)
        {
            configuration_ConnectionString = configuration;
        }

        public SqlConnection Conexion;
        private String CadenaConexion;

        #region "METODOS DE CONEXION"


        //'Crear nueva Conexion
        private void NuevaConexionBaseDeDatos(string Cadena)
        {
            if (Cadena != "")
            {
                CadenaConexion = Cadena;
                Conexion = new SqlConnection(CadenaConexion);
            }
        }

        //'Abrir conexion
        private void ConectarBaseDeDatos()
        {
            Conexion.Open();
        }

        //'Cerrar conexion
        private void DesconectarBaseDeDatos()
        {
            if (Conexion.State == ConnectionState.Open)
            {
                Conexion.Close();
                SqlConnection.ClearPool(Conexion);
            }
        }
        //'Metodo publico conectar
        public void Conectar(string Cadena)
        {
            NuevaConexionBaseDeDatos(Cadena);
            ConectarBaseDeDatos();
        }

        //'Metodo publico desconectar 
        public void Desconectar()
        {
            DesconectarBaseDeDatos();
        }
        #endregion

        /// Autor: Andres Peinado
        /// Fecha: 2020/02/21
        /// <summary>
        /// Consultar Usuario
        /// </summary>
        /// <param name="usuario"></param>
        /// <param name="contrasenia"></param>
        /// <returns></returns>
        public DataTable IniciarSesion(string usuario, string contrasenia)
        {
            Conexiones Conexion = new Conexiones();
            SqlCommand cmd = new SqlCommand();
            DataTable tabla = new DataTable();
            SqlDataAdapter sda = new SqlDataAdapter();
            try
            {
                Conexion.Conectar(configuration_ConnectionString.GetConnectionString("WideWorldImportersDatabase"));
                if (Conexion.Conexion.State == ConnectionState.Open)
                {
                    cmd.Connection = Conexion.Conexion;
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.CommandText = "Sp_IniciarSesion";
                    cmd.Parameters.AddWithValue("@usuario", usuario);
                    cmd.Parameters.AddWithValue("@contra", contrasenia);
                    sda.SelectCommand = cmd;
                    sda.Fill(tabla);
                }
                Conexion.Desconectar();
            }
            catch (Exception ex)
            {
                Conexion.Desconectar();
            }
            return tabla;
        }
    }
}
