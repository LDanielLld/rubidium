using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Configuration;
using System.Data.SqlClient;
using System.Security.Cryptography;

namespace REVIREPanels
{
    public partial class PanelPass : Form
    {
        internal class Helper
        {
            public static string EncodePassword(string originalPassword)
            {
                SHA1 sha1 = new SHA1CryptoServiceProvider();

                byte[] inputBytes = (new UnicodeEncoding()).GetBytes(originalPassword);
                byte[] hash = sha1.ComputeHash(inputBytes);
                return Convert.ToBase64String(hash);
            }
        }

        public PanelPass()
        {
            InitializeComponent();
        }     

        public bool Autenticar(string usuario, string password)
        {
            try
            {
                String connectionString = "Data Source = (LocalDB)\\MSSQLLocalDB; " +
                   "AttachDbFilename = " +
                   "D:\\UnityProjects\\DeveloperVisualCSharp\\REVIRE\\REVIRE\\REVIREDataBase.mdf;" +
                   " Integrated Security = True";
                               
                string sql = @"SELECT COUNT(*)
                                FROM Autenticacion
                                WHERE NombreLogin = @nombre AND PASSWORD = @password";
                // string cnn = ConfigurationManager.ConnectionStrings["name"].ConnectionString;

                using (SqlConnection conexion = new SqlConnection(connectionString))
                {
                    conexion.Open();
                    SqlCommand command = new SqlCommand(sql, conexion);
                    command.Parameters.AddWithValue("@nombre", usuario);
                    

                    string hash = Helper.EncodePassword(string.Concat(usuario,password));
                    command.Parameters.AddWithValue("@password", hash);

                    int count = Convert.ToInt32(command.ExecuteScalar());

                    conexion.Close();
                    conexion.Dispose();

                    if (count == 0)
                        return false;
                    else
                        return true;
                }
            }

            catch(Exception e)
            {
                MessageBox.Show(e.ToString());
                return false;
            }
        }

        public void Insert(string usuario, string password)
        {
            try
            {
                String connectionString = "Data Source = (LocalDB)\\MSSQLLocalDB; " +
                   "AttachDbFilename = " +
                   "D:\\UnityProjects\\DeveloperVisualCSharp\\REVIRE\\REVIRE\\REVIREDataBase.mdf;" +
                   " Integrated Security = True";

                string sql = @"INSERT INTO Autenticacion (                           
                          NombreLogin
                          ,Password)
                      VALUES (                            
                            @NombreLogin,
                            @Password)";
             //   string cnn = ConfigurationManager.ConnectionStrings["REVIRE.Properties.Settings.REVIREDataBaseConnectionString"].ConnectionString;
                //string cnn = ConfigurationManager.ConnectionStrings["REVIREPanels.Properties.Settings.REVIREDataBaseConnectionString"].ConnectionString;
                using (SqlConnection conexion = new SqlConnection(connectionString))
                {
                    SqlCommand command = new SqlCommand(sql, conexion);                    
                    command.Parameters.AddWithValue("NombreLogin", usuario);

                    string password_encr = Helper.EncodePassword(string.Concat(usuario, password));
                    command.Parameters.AddWithValue("Password", password_encr);
                    
                    conexion.Open();
                    command.ExecuteScalar();
                    conexion.Close();
                    conexion.Dispose();                    
                }
            }

            catch (Exception e)
            {
                MessageBox.Show(e.ToString());                
            }
        }

        public bool UpdatePass(string usuario, string password, string oldpassword)
        {
            try
            {
                String connectionString = "Data Source = (LocalDB)\\MSSQLLocalDB; " +
                   "AttachDbFilename = " +
                   "D:\\UnityProjects\\DeveloperVisualCSharp\\REVIRE\\REVIRE\\REVIREDataBase.mdf;" +
                   " Integrated Security = True";

                string sql = @"UPDATE Autenticacion SET " +
                                "NombreLogin = @NombreLogin, Password = @newPassword " +
                                "WHERE NombreLogin = @oldNombre AND PASSWORD = @oldPassword";
                // string cnn = ConfigurationManager.ConnectionStrings["name"].ConnectionString;

                using (SqlConnection conexion = new SqlConnection(connectionString))
                {
                    
                    SqlCommand command = new SqlCommand(sql, conexion);
                    command.Parameters.AddWithValue("@NombreLogin", usuario);
                    command.Parameters.AddWithValue("@oldNombre", usuario);

                    string hash = Helper.EncodePassword(string.Concat(usuario, password));
                    string oldhash = Helper.EncodePassword(string.Concat(usuario, oldpassword));
                    command.Parameters.AddWithValue("@newPassword", hash);
                    command.Parameters.AddWithValue("@oldPassword", oldhash);

                    conexion.Open();
                    command.ExecuteScalar();
                    conexion.Close();
                    conexion.Dispose();

                    return true;
                }
            }

            catch (Exception e)
            {
                MessageBox.Show(e.ToString());
                return false;
            }
        }

        private void btnChange_Click(object sender, EventArgs e)
        {
            //Insertar contraseña y usuario nuevo
            //Insert("usuario", "pass");

            string currentPass = txtCurrentPass.Text;
            string newPass = txtNexPass.Text;
            string writeNewPass = txtWriteNewPass.Text;

            //Primer comprueba que se hayan introducido los datos
            bool st1 = string.IsNullOrEmpty(currentPass);
            bool st2 = string.IsNullOrEmpty(newPass);
            bool st3 = string.IsNullOrEmpty(writeNewPass);

            if (st1 || st2 || st3)
            {
                MessageBox.Show("Faltan datos de entrada.");
                return;
            }

            //Compara la contraseña introducida con la de la base de datos
            bool passIsEqual = Autenticar("admin", currentPass);

            if (!passIsEqual)
            {
                MessageBox.Show("Contraseña incorrecta.");
                return;
            }       

            if (newPass != writeNewPass)
            {
                MessageBox.Show("La nueva contraseña no coincide.");
                return;
            }

            //Actualizar contraseña
            bool updating = UpdatePass("admin", writeNewPass, currentPass);
            if(updating)
                MessageBox.Show("Contraseña actualizada.");
            else
                MessageBox.Show("Error al actualizar la contraseña.");

            this.Close();
           
        }
    }
}
