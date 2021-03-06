﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace ModuloDeSeguridad.Vista
{
    public partial class frmIniciarSesion : Form, Logica.Interfaces.ISesionObserver
    {
        private Logica.SesionBL sesionBL;
        public frmIniciarSesion()
        {
            InitializeComponent();
            CheckForIllegalCrossThreadCalls = false;
            sesionBL = Logica.SesionBL.ObtenerInstancia();
        }

        public void Actualizar(bool isFirst)
        {
            if (isFirst)
            {
                MessageBox.Show("Su sesión se cerrará automaticamente");
            }
            sesionBL.Desuscribir(this);
            this.Show();
        }

        private void BtnIniciarSesion_Click(object sender, EventArgs e)
        {
            if (String.IsNullOrWhiteSpace(txtUsername.Text) || String.IsNullOrWhiteSpace(txtContrasena.Text))
            {
                MessageBox.Show("Debe completar todos los campos");
                return;
            }
            try
            {
                int userId = sesionBL.ValidarUsuario(txtUsername.Text, txtContrasena.Text);
                if (userId != -1)
                {
                    sesionBL.Suscribir(this);

                    var sesion = Modelo.Sesion.ObtenerInstancia();
                    sesion.Usuario = sesionBL.ConsultarUsuario(userId);
                    sesion.LogIn = DateTime.Now;
                    sesionBL.IniciarSesion();

                    if (sesionBL.NeedNewPassword(userId))
                    {
                        frmCambiarContrasena cContrasena = new frmCambiarContrasena(sesion.Usuario.ID, true);
                        cContrasena.ShowDialog();
                        sesion.Usuario = sesionBL.ConsultarUsuario(userId);
                    }
                    
                    frmInicio inicio = new frmInicio();
                    this.Hide();
                    DialogResult result = inicio.ShowDialog();
                }
                else
                {
                    MessageBox.Show("Username o Contraseña incorrectos");
                    return;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
                return;
            }
        }

        private void LlblOlvidasteTuContrasena_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            frmRecuperarContrasena recuperar = new frmRecuperarContrasena();
            recuperar.ShowDialog();
        }
    }
}
