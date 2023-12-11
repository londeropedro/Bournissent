using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using static System.Windows.Forms.VisualStyles.VisualStyleElement;

namespace Bournissent
{
    public partial class frmPrincipal : Form
    {
        #region Instanciacion
        private Conexion ConexionDB = new Conexion();
        SqlCommand comando = new SqlCommand();
        SqlDataReader leer;
        DataTable tabla = new DataTable();
        DataTable proveedores = new DataTable();
        DataTable marcasAutos = new DataTable();
        DataTable rubros = new DataTable();
        DataTable provincias = new DataTable();
        DataTable ciudades = new DataTable();
        string carpetaPath = @"C:\BournissentRepuestosImagenes";
        #endregion

        #region Principal
        public frmPrincipal()
        {
            InitializeComponent();
        }

        private void frmPrincipal_Load(object sender, EventArgs e)
        {
            #region CreacionCarpetaImagenes
            try
            {
                // Verificar si la carpeta ya existe antes de intentar crearla
                if (!Directory.Exists(carpetaPath))
                {
                    // Crear la carpeta
                    Directory.CreateDirectory(carpetaPath);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Ha ocurrido un error al generar la carpeta para guardar imágenes: ", ex.ToString());
            }
            #endregion

            LeerCombos();
            LeerProductos();
            LeerProveedores();
            LeerMarcasAutos();
            LeerRubros();
            LeerProvincias();
            LeerCiudades();
        }
        #endregion

        #region Combos

        public void LeerCombos()
        {
            string[] combos = { "Rubros", "Medidas", "Proveedores", "MarcasAutos", "Ciudades", "Provincias" };

            foreach (string combo in combos)
            {
                switch (combo)
                {
                    case "Rubros":
                        cmbRubros.DataSource = ObtenerCombos(combo);
                        cmbRubros.DisplayMember = "Descripcion";
                        cmbRubros.ValueMember = "Id";
                        break;
                    case "Medidas":
                        cmbMedidas.DataSource = ObtenerCombos(combo);
                        cmbMedidas.DisplayMember = "Descripcion";
                        cmbMedidas.ValueMember = "Id";
                        break;
                    case "Proveedores":
                        chkProveedores.DataSource = ObtenerCombos(combo);
                        chkProveedores.DisplayMember = "Descripcion";
                        chkProveedores.ValueMember = "Id";
                        break;
                    case "MarcasAutos":
                        chkMarcasAutos.DataSource = ObtenerCombos(combo);
                        chkMarcasAutos.DisplayMember = "Descripcion";
                        chkMarcasAutos.ValueMember = "Id";
                        break;
                    case "Ciudades":
                        cmbPCiudad.DataSource = ObtenerCombos(combo);
                        cmbPCiudad.DisplayMember = "Descripcion";
                        cmbPCiudad.ValueMember = "Id";
                        break;
                    case "Provincias":
                        cmbCProvincia.DataSource = ObtenerCombos(combo);
                        cmbCProvincia.DisplayMember = "Descripcion";
                        cmbCProvincia.ValueMember = "Id";
                        break;

                }
            }
        }
        private DataTable ObtenerCombos(string combo)
        {
            try
            {
                tabla.Clear();

                comando.Connection = ConexionDB.AbrirConexion();
                comando.CommandText = "sp_Combos";
                comando.Parameters.Clear();

                comando.Parameters.Add("@Combo", SqlDbType.Char);
                comando.Parameters["@Combo"].Value = combo;
                comando.CommandType = CommandType.StoredProcedure;
                using (SqlDataAdapter da = new SqlDataAdapter(comando))
                {
                    DataTable dt = new DataTable();
                    da.Fill(dt);
                    ConexionDB.CerrarConexion();
                    return dt;
                }

            }
            catch (Exception e)
            {
                throw e;
            }
        }


        #endregion

        #region Productos
        public void LeerProductos()
        {
            griProductos.DataSource = ObtenerProductos();

        }
        private DataTable ObtenerProductos()
        {

            try
            {
                tabla.Clear();

                comando.Connection = ConexionDB.AbrirConexion();
                comando.CommandText = "sp_Producto";
                comando.Parameters.Clear();

                comando.Parameters.Add("@Opcion", SqlDbType.Char);
                comando.Parameters["@Opcion"].Value = 'S';
                comando.CommandType = CommandType.StoredProcedure;
                leer = comando.ExecuteReader();
                tabla.Load(leer);
                ConexionDB.CerrarConexion();

                return tabla;


            }
            catch (Exception e)
            {
                throw e;
            }
        }
     
        private void griProductos_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {
           
        }

        private void txtAgregar_Click(object sender, EventArgs e)
        {
            AgregarModificarEliminar("Agregar");
        }

        public void AgregarModificarEliminar(string operacion)
        {
            ConexionDB = new Conexion();
            try
            {
                comando.Connection = ConexionDB.AbrirConexion();
                comando.CommandText = "sp_Producto";
                comando.Parameters.Clear();

                if (operacion == "Eliminar")
                {
                    comando.Parameters.Add("@Opcion", SqlDbType.Char);
                    comando.Parameters["@Opcion"].Value = 'D';
                    comando.Parameters.Add("@Id", SqlDbType.Int);
                    comando.Parameters["@Id"].Value = Convert.ToInt16(txtId.Text);

                    comando.CommandType = CommandType.StoredProcedure;
                    comando.ExecuteNonQuery();
                    ConexionDB.CerrarConexion();

                    MessageBox.Show("Los datos se eliminaron exitosamente!");

                    LeerProductos();
                }
                else
                {
                    if (operacion == "Modificar")
                    {
                        comando.Parameters.Add("@Opcion", SqlDbType.Char);
                        comando.Parameters["@Opcion"].Value = 'U';
                        comando.Parameters.Add("@Id", SqlDbType.Int);
                        comando.Parameters["@Id"].Value = Convert.ToInt16(txtId.Text);

                    }
                    if (operacion == "Agregar")
                    {
                        comando.Parameters.Add("@Opcion", SqlDbType.Char);
                        comando.Parameters["@Opcion"].Value = 'I';
                    }
                    comando.Parameters.Add("@Nombre", SqlDbType.VarChar);
                    comando.Parameters["@Nombre"].Value = txtPrdNombre.Text;
                    comando.Parameters.Add("@RubroId", SqlDbType.Int);
                    comando.Parameters["@RubroId"].Value = Convert.ToInt16(cmbRubros.SelectedValue);
                    comando.Parameters.Add("@MedidaId", SqlDbType.Int);
                    comando.Parameters["@MedidaId"].Value = Convert.ToInt16(cmbMedidas.SelectedValue);

                    comando.CommandType = CommandType.StoredProcedure;
                    comando.ExecuteNonQuery();
                    ConexionDB.CerrarConexion();

                    MessageBox.Show("Los datos se guardaron exitosamente!");

                    LeerProductos();

                }

            }
            catch (Exception)
            {
                throw;
            }
        }

        private void txtModificar_Click(object sender, EventArgs e)
        {
            AgregarModificarEliminar("Modificar");
        }

        private void txtEliminar_Click(object sender, EventArgs e)
        {
            string mensaje = "Está seguro de Eliminar el Producto: " + griProductos.CurrentRow.Cells["Nombre"].Value.ToString();
            if (MessageBox.Show(mensaje, "Eliminación", MessageBoxButtons.YesNo, MessageBoxIcon.Question, MessageBoxDefaultButton.Button1) == DialogResult.Yes)
                AgregarModificarEliminar("Eliminar");
        }

        private void griProductos_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            txtId.Text = griProductos.CurrentRow.Cells["Id"].Value.ToString();
            txtPrdNombre.Text = griProductos.CurrentRow.Cells["Nombre"].Value.ToString();
            cmbRubros.Text = griProductos.CurrentRow.Cells["Rubro"].Value.ToString();
            cmbMedidas.Text = griProductos.CurrentRow.Cells["Medida"].Value.ToString();
            if (!Directory.Exists(nombreCarpetaId))
            {
                // Crear la carpeta
                Directory.CreateDirectory(nombreCarpetaId);
            }
        }

        private void txtLimpiar_Click(object sender, EventArgs e)
        {
            txtPrdNombre.Text = "";
            txtId.Text = "";

        }

        private void txtImagen_Click(object sender, EventArgs e)
        {
            // Generar Carpeta por Id del Producto si no existe
            if (txtId.Text == "")
            {
                MessageBox.Show("Debe seleccionar un Repuesto para cargar una imagen");
            }
            else
            {
                // Abre el cuadro de diálogo para seleccionar un archivo
                OpenFileDialog openFileDialog = new OpenFileDialog();
                openFileDialog.Title = "Selecciona un archivo";
                openFileDialog.Filter = "Archivos de texto (*.jpg)|*.jpg|Todos los archivos (*.*)|*.*";

                if (openFileDialog.ShowDialog() == DialogResult.OK)
                {
                
                    string nombreCarpetaId = carpetaPath + @"\" + txtId.Text;

                    if (!Directory.Exists(nombreCarpetaId))
                    {
                        // Crear la carpeta
                        Directory.CreateDirectory(nombreCarpetaId);
                    }

                    // Obtiene la ruta completa del archivo seleccionado
                    string archivoOrigen = openFileDialog.FileName;

                    // Copia el archivo seleccionado a la nueva carpeta
                    File.Copy(archivoOrigen, Path.Combine(nombreCarpetaId, txtId.Text + @".jpg"), true);
                    MessageBox.Show("Imagen guardada exitosamente");

                    // Asigno imagen seleccionada al pictureBox
                    Image imagen = Image.FromFile(nombreCarpetaId + @"\" + txtId.Text + @".jpg");
                    pbxImagen.Image = imagen;

                }
            }
        }
        #endregion

        #region Proveedores
        public void LeerProveedores()
        {
            griProveedores.DataSource = ObtenerProveedores();

        }
        private DataTable ObtenerProveedores()
        {

            try
            {
                proveedores.Clear();

                comando.Connection = ConexionDB.AbrirConexion();
                comando.CommandText = "sp_Proveedores";
                comando.Parameters.Clear();

                comando.Parameters.Add("@Opcion", SqlDbType.Char);
                comando.Parameters["@Opcion"].Value = 'S';
                comando.CommandType = CommandType.StoredProcedure;
                leer = comando.ExecuteReader();
                proveedores.Load(leer);
                ConexionDB.CerrarConexion();

                return proveedores;


            }
            catch (Exception e)
            {
                throw e;
            }
        }
        private void btnAgregarProveedor_Click(object sender, EventArgs e)
        {
            AgregarModificarProveedores("Agregar");
        }

        public void AgregarModificarProveedores(string operacion)
        {
            ConexionDB = new Conexion();
            try
            {
                comando.Connection = ConexionDB.AbrirConexion();
                comando.CommandText = "sp_Proveedores";
                comando.Parameters.Clear();

                if (operacion == "Modificar")
                {
                    comando.Parameters.Add("@Opcion", SqlDbType.Char);
                    comando.Parameters["@Opcion"].Value = 'U';
                    comando.Parameters.Add("@Id", SqlDbType.Int);
                    comando.Parameters["@Id"].Value = Convert.ToInt16(txtPId.Text);

                }
                if (operacion == "Agregar")
                {
                    comando.Parameters.Add("@Opcion", SqlDbType.Char);
                    comando.Parameters["@Opcion"].Value = 'I';
                }
                comando.Parameters.Add("@Descripcion", SqlDbType.VarChar);
                comando.Parameters["@Descripcion"].Value = txtPNombre.Text;

                comando.Parameters.Add("@Domicilio", SqlDbType.VarChar);
                comando.Parameters["@Domicilio"].Value = txtPDomicilio.Text;
                
                comando.Parameters.Add("@Telefono", SqlDbType.VarChar);
                comando.Parameters["@Telefono"].Value = txtPTelefono.Text;
                
                comando.Parameters.Add("@PaginaWeb", SqlDbType.VarChar);
                comando.Parameters["@PaginaWeb"].Value = txtPSitio.Text;

                comando.Parameters.Add("@Mail", SqlDbType.VarChar);
                comando.Parameters["@Mail"].Value = txtPEmail.Text;

                comando.Parameters.Add("@CiuidadId", SqlDbType.Int);
                comando.Parameters["@CiuidadId"].Value = Convert.ToInt16(cmbPCiudad.SelectedValue);
                
                
                comando.CommandType = CommandType.StoredProcedure;
                comando.ExecuteNonQuery();
                ConexionDB.CerrarConexion();

                MessageBox.Show("Los datos se guardaron exitosamente!");

                LeerProveedores();
                LeerCombos();

            }
            catch (Exception)
            {
                throw;
            }
        }
        private void btnModificarProveedor_Click(object sender, EventArgs e)
        {
            AgregarModificarProveedores("Modificar");
        }

        private void griProveedores_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            txtPId.Text = griProveedores.CurrentRow.Cells["Id"].Value.ToString();
            txtPNombre.Text = griProveedores.CurrentRow.Cells["Proveedor"].Value.ToString();
            txtPDomicilio.Text = griProveedores.CurrentRow.Cells["Domicilio"].Value.ToString();
            txtPTelefono.Text = griProveedores.CurrentRow.Cells["Telefono"].Value.ToString();
            txtPSitio.Text = griProveedores.CurrentRow.Cells["PaginaWeb"].Value.ToString();
            txtPEmail.Text = griProveedores.CurrentRow.Cells["Mail"].Value.ToString();
            cmbPCiudad.Text = griProveedores.CurrentRow.Cells["Ciudad"].Value.ToString();
        }

        #endregion

        #region MarcasAutos
        public void LeerMarcasAutos()
        {
            griMarcasAutos.DataSource = ObtenerMarcasAutos();

        }
        private DataTable ObtenerMarcasAutos()
        {

            try
            {
                marcasAutos.Clear();

                comando.Connection = ConexionDB.AbrirConexion();
                comando.CommandText = "sp_MarcasAutos";
                comando.Parameters.Clear();

                comando.Parameters.Add("@Opcion", SqlDbType.Char);
                comando.Parameters["@Opcion"].Value = 'S';
                comando.CommandType = CommandType.StoredProcedure;
                leer = comando.ExecuteReader();
                marcasAutos.Load(leer);
                ConexionDB.CerrarConexion();

                return marcasAutos;


            }
            catch (Exception e)
            {
                throw e;
            }
        }
     
        public void AgregarModificarMarcasAutos(string operacion)
        {
            ConexionDB = new Conexion();
            try
            {
                comando.Connection = ConexionDB.AbrirConexion();
                comando.CommandText = "sp_MarcasAutos";
                comando.Parameters.Clear();

                if (operacion == "Modificar")
                {
                    comando.Parameters.Add("@Opcion", SqlDbType.Char);
                    comando.Parameters["@Opcion"].Value = 'U';
                    comando.Parameters.Add("@Id", SqlDbType.Int);
                    comando.Parameters["@Id"].Value = Convert.ToInt16(txtMarcasAutosId.Text);

                }
                if (operacion == "Agregar")
                {
                    comando.Parameters.Add("@Opcion", SqlDbType.Char);
                    comando.Parameters["@Opcion"].Value = 'I';
                }
                comando.Parameters.Add("@Descripcion", SqlDbType.VarChar);
                comando.Parameters["@Descripcion"].Value = txtANombre.Text;

                comando.CommandType = CommandType.StoredProcedure;
                comando.ExecuteNonQuery();
                ConexionDB.CerrarConexion();

                MessageBox.Show("Los datos se guardaron exitosamente!");

                LeerMarcasAutos();
                LeerCombos();

            }
            catch (Exception)
            {
                throw;
            }
        }
       
        private void txtAgregarAuto_Click(object sender, EventArgs e)
        {
            AgregarModificarMarcasAutos("Agregar");
        }

        private void txtModificarAuto_Click(object sender, EventArgs e)
        {
            AgregarModificarMarcasAutos("Modificar");
        }
        private void griMarcasAutos_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {
            
        }
        private void griMarcasAutos_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            txtMarcasAutosId.Text = griMarcasAutos.CurrentRow.Cells["Id"].Value.ToString();
            txtANombre.Text = griMarcasAutos.CurrentRow.Cells["Descripcion"].Value.ToString();
        }

        #endregion

        #region Rubros
        public void LeerRubros()
        {
            griRubros.DataSource = ObtenerRubros();

        }
        private DataTable ObtenerRubros()
        {

            try
            {
                rubros.Clear();

                comando.Connection = ConexionDB.AbrirConexion();
                comando.CommandText = "sp_Rubros";
                comando.Parameters.Clear();

                comando.Parameters.Add("@Opcion", SqlDbType.Char);
                comando.Parameters["@Opcion"].Value = 'S';
                comando.CommandType = CommandType.StoredProcedure;
                leer = comando.ExecuteReader();
                rubros.Load(leer);
                ConexionDB.CerrarConexion();

                return rubros;


            }
            catch (Exception e)
            {
                throw e;
            }
        }

        public void AgregarModificarRubros(string operacion)
        {
            ConexionDB = new Conexion();
            try
            {
                comando.Connection = ConexionDB.AbrirConexion();
                comando.CommandText = "sp_Rubros";
                comando.Parameters.Clear();

                if (operacion == "Modificar")
                {
                    comando.Parameters.Add("@Opcion", SqlDbType.Char);
                    comando.Parameters["@Opcion"].Value = 'U';
                    comando.Parameters.Add("@Id", SqlDbType.Int);
                    comando.Parameters["@Id"].Value = Convert.ToInt16(txtRId.Text);

                }
                if (operacion == "Agregar")
                {
                    comando.Parameters.Add("@Opcion", SqlDbType.Char);
                    comando.Parameters["@Opcion"].Value = 'I';
                }
                comando.Parameters.Add("@Descripcion", SqlDbType.VarChar);
                comando.Parameters["@Descripcion"].Value = txtRNombre.Text;

                comando.CommandType = CommandType.StoredProcedure;
                comando.ExecuteNonQuery();
                ConexionDB.CerrarConexion();

                MessageBox.Show("Los datos se guardaron exitosamente!");

                LeerRubros();

            }
            catch (Exception)
            {
                throw;
            }
        }

        private void btnAgregarRubro_Click(object sender, EventArgs e)
        {
            AgregarModificarRubros("Agregar");
        }

        private void btnModificarRubro_Click(object sender, EventArgs e)
        {
            AgregarModificarRubros("Modificar");
        }

        private void griRubros_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            txtRId.Text = griRubros.CurrentRow.Cells["Id"].Value.ToString();
            txtRNombre.Text = griRubros.CurrentRow.Cells["Descripcion"].Value.ToString();
        }
        #endregion

        #region Provincias
        public void LeerProvincias()
        {
            griProvincias.DataSource = ObtenerProvincias();

        }
        private DataTable ObtenerProvincias()
        {

            try
            {
                provincias.Clear();

                comando.Connection = ConexionDB.AbrirConexion();
                comando.CommandText = "sp_Provincias";
                comando.Parameters.Clear();

                comando.Parameters.Add("@Opcion", SqlDbType.Char);
                comando.Parameters["@Opcion"].Value = 'S';
                comando.CommandType = CommandType.StoredProcedure;
                leer = comando.ExecuteReader();
                provincias.Load(leer);
                ConexionDB.CerrarConexion();

                return provincias;


            }
            catch (Exception e)
            {
                throw e;
            }
        }

        public void AgregarModificarProvincias(string operacion)
        {
            ConexionDB = new Conexion();
            try
            {
                comando.Connection = ConexionDB.AbrirConexion();
                comando.CommandText = "sp_Provincias";
                comando.Parameters.Clear();

                if (operacion == "Modificar")
                {
                    comando.Parameters.Add("@Opcion", SqlDbType.Char);
                    comando.Parameters["@Opcion"].Value = 'U';
                    comando.Parameters.Add("@Id", SqlDbType.Int);
                    comando.Parameters["@Id"].Value = Convert.ToInt16(txtProvId.Text);

                }
                if (operacion == "Agregar")
                {
                    comando.Parameters.Add("@Opcion", SqlDbType.Char);
                    comando.Parameters["@Opcion"].Value = 'I';
                }
                comando.Parameters.Add("@Descripcion", SqlDbType.VarChar);
                comando.Parameters["@Descripcion"].Value = txtProvNombre.Text;

                comando.CommandType = CommandType.StoredProcedure;
                comando.ExecuteNonQuery();
                ConexionDB.CerrarConexion();

                MessageBox.Show("Los datos se guardaron exitosamente!");

                LeerRubros();

            }
            catch (Exception)
            {
                throw;
            }
        }

        private void btnAgregarProvincia_Click(object sender, EventArgs e)
        {
            AgregarModificarProvincias("Agregar");
        }

        private void btnModificarProvincia_Click(object sender, EventArgs e)
        {
            AgregarModificarProvincias("Modificar");
        }

        private void griProvincias_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            txtProvId.Text = griProvincias.CurrentRow.Cells["Id"].Value.ToString();
            txtProvNombre.Text = griProvincias.CurrentRow.Cells["Descripcion"].Value.ToString();
        }
        #endregion

        #region Ciudades
        public void LeerCiudades()
        {
            griCiudades.DataSource = ObtenerCiudades();

        }
        private DataTable ObtenerCiudades()
        {

            try
            {
                ciudades.Clear();

                comando.Connection = ConexionDB.AbrirConexion();
                comando.CommandText = "sp_Ciudades";
                comando.Parameters.Clear();

                comando.Parameters.Add("@Opcion", SqlDbType.Char);
                comando.Parameters["@Opcion"].Value = 'S';
                comando.CommandType = CommandType.StoredProcedure;
                leer = comando.ExecuteReader();
                ciudades.Load(leer);
                ConexionDB.CerrarConexion();

                return ciudades;


            }
            catch (Exception e)
            {
                throw e;
            }
        }

        public void AgregarModificarCiudades(string operacion)
        {
            ConexionDB = new Conexion();
            try
            {
                comando.Connection = ConexionDB.AbrirConexion();
                comando.CommandText = "sp_Ciudades";
                comando.Parameters.Clear();

                if (operacion == "Modificar")
                {
                    comando.Parameters.Add("@Opcion", SqlDbType.Char);
                    comando.Parameters["@Opcion"].Value = 'U';
                    comando.Parameters.Add("@Id", SqlDbType.Int);
                    comando.Parameters["@Id"].Value = Convert.ToInt16(txtCiuId.Text);

                }
                if (operacion == "Agregar")
                {
                    comando.Parameters.Add("@Opcion", SqlDbType.Char);
                    comando.Parameters["@Opcion"].Value = 'I';
                }
                comando.Parameters.Add("@Descripcion", SqlDbType.VarChar);
                comando.Parameters["@Descripcion"].Value = txtCiuNombre.Text;

                comando.Parameters.Add("@ProvinciaId", SqlDbType.Int);
                comando.Parameters["@ProvinciaId"].Value = Convert.ToInt16(cmbCProvincia.SelectedValue);

                comando.CommandType = CommandType.StoredProcedure;
                comando.ExecuteNonQuery();
                ConexionDB.CerrarConexion();

                MessageBox.Show("Los datos se guardaron exitosamente!");

                LeerCiudades();

            }
            catch (Exception)
            {
                throw;
            }
        }

      
        private void btnAgregarCiudad_Click(object sender, EventArgs e)
        {
            AgregarModificarCiudades("Agregar");
        }

        private void btnModificarCiudad_Click(object sender, EventArgs e)
        {
            AgregarModificarCiudades("Modificar");
        }

        private void griCiudades_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            txtCiuId.Text = griProvincias.CurrentRow.Cells["Id"].Value.ToString();
            txtCiuNombre.Text = griProvincias.CurrentRow.Cells["Descripcion"].Value.ToString();
            cmbCProvincia.Text = griProductos.CurrentRow.Cells["Provincia"].Value.ToString();

        }


        #endregion

       
    }
}