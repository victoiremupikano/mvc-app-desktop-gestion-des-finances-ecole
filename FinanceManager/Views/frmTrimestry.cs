﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using FinanceManager.Properties;
using MySql.Data.MySqlClient;


namespace FinanceManager.Views
{
    public partial class frmTrimestry : Form
    {
        public string id;

        Services.convertDate valid = new Services.convertDate();
        Services.DtgvServices dgv = new Services.DtgvServices();

        public frmTrimestry()
        {
            InitializeComponent();
        }

        //interface servant à verifier si un frm est dejà active
        public static bool IsFormOpen(Type formType)
        {
            foreach (Form form in Application.OpenForms)
            {
                if (form.GetType().Name == formType.Name)
                {
                    return true;
                }
            }
            return false;
        }

        private void frmArrivage_Load(object sender, EventArgs e)
        {
            //remplissage ethiquette
            EthiquetteArr();

            btnToModify.Enabled = false;
            btnSaved.Enabled = false;
            btnDelete.Enabled = false;
        }

        private void pictureBoxMinimize_Click(object sender, EventArgs e)
        {
            this.WindowState = FormWindowState.Minimized;
        }

        private void pictureBoxClose_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        //
        //Debut du code pour la trimestry .;
        //
        #region trimestry

        private void viderLesTxt()
        {
            txtWording.Text = string.Empty;
            txtMt_to_pay.Text = "0";
        }

        //fonction nous aidant ethiquetter
        private void EthiquetteArr()
        {
            txtWording.Text = "Nom du trimestre";
        }

        /// <summary>
        /// Controle trimestry
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        /// 
        private void migrate()
        {
            if (dgvData.Rows.Count > 0 && dgvData.SelectedRows.Count > 0)
            {
                viderLesTxt();

                id = dgvData.CurrentRow.Cells[0].Value.ToString();
                txtWording.Text = dgvData.CurrentRow.Cells[1].Value.ToString();
                txtMt_to_pay.Text = dgvData.CurrentRow.Cells[2].Value.ToString().Replace(".", ",");
            }
        }

        private void modify()
        {
            Services.MsgFRM msg = new Services.MsgFRM();
            if (string.IsNullOrEmpty(id) || string.IsNullOrEmpty(txtMt_to_pay.Text) || string.IsNullOrEmpty(txtWording.Text) || txtWording.Text == "Nom du trimestre")
            {
                msg.getAttention("Erreur, veiller remplir tous les champs ?");
            }
            else
            {
                if (msg.getDialog("Etes-vous sûr de vouloir modifier ?"))
                {
                    Dictionary<string, string> fields = new Dictionary<string, string>{
                        {"id", id},
                        {"wording", txtWording.Text},
                        {"mt_to_pay", txtMt_to_pay.Text.Replace(",", ".")},
                        {"fk_year", Services.Session.ExerciselSession["id"]},
                        {"fk_user", Services.Session.UserSession["id"]},
                    };

                    //on passe les donnees dans le controllers
                    Controllers.CTrimestry obj = new Controllers.CTrimestry(fields);
                    obj.update(obj);

                    if (obj.message["type"] == "success")
                    {
                        msg.getInfo(obj.message["message"]);

                        loard();
                    }
                    else if (obj.message["type"] == "failure")
                    {
                        msg.getError(obj.message["message"]);
                    }
                    else
                    {
                        msg.getError(obj.message["message"]);
                    }
                    btnDelete.Enabled = true;
                    btnToModify.Enabled = true;
                    btnSaved.Enabled = false;
                }
            }
        }

        private void delete()
        {
            Services.MsgFRM msg = new Services.MsgFRM();
            if (msg.getDialog("Etes-vous sûr de vouloir supprimer ?"))
            {
                Dictionary<string, string> fields = new Dictionary<string, string>{
                    {"id", id},
                };
                //on passe les donnees dans le controllers
                Controllers.CTrimestry obj = new Controllers.CTrimestry(fields);
                obj.delete(obj);

                if (obj.message["type"] == "success")
                {
                    msg.getInfo(obj.message["message"]);

                    loard();
                }
                else if (obj.message["type"] == "failure")
                {
                    msg.getError(obj.message["message"]);
                }
                else
                {
                    msg.getError(obj.message["message"]);
                }
                btnDelete.Enabled = true;
                btnToModify.Enabled = true;
                btnSaved.Enabled = false;
            }
        }

        private void save()
        {
            Services.MsgFRM msg = new Services.MsgFRM();
            if (string.IsNullOrEmpty(txtWording.Text) || string.IsNullOrEmpty(txtMt_to_pay.Text) || txtWording.Text == "Nom du trimestre")
            {
                msg.getAttention("Erreur, veiller remplir tous les champs ?");
            }
            else
            {
                Dictionary<string, string> fields = new Dictionary<string, string>{
                    {"wording", txtWording.Text},
                    {"mt_to_pay", txtMt_to_pay.Text.Replace(",", ".")},
                    {"fk_year", Services.Session.ExerciselSession["id"]},
                    {"fk_user", Services.Session.UserSession["id"]},
                };

                //on passe les donnees dans le controllers
                Controllers.CTrimestry obj = new Controllers.CTrimestry(fields);
                obj.add(obj);

                if (obj.message["type"] == "success")
                {
                    msg.getInfo(obj.message["message"]);

                    loard();
                }
                else if (obj.message["type"] == "failure")
                {
                    msg.getError(obj.message["message"]);
                }
                else
                {
                    msg.getError(obj.message["message"]);
                }
                btnDelete.Enabled = true;
                btnToModify.Enabled = true;
                btnSaved.Enabled = false;
            }
        }

        private void loard()
        {
            Models.MTrimestry obj = new Models.MTrimestry();
            obj.get();
            if (obj.callback["type"] == "success")
            {
                //on vide la dgv
                dgvData.Rows.Clear();
                MySqlDataReader dr = Apps.Query.DR;

                while (dr.Read())
                {
                    dgvData.Rows.Add(dr[0].ToString(), dr[1].ToString(), dr[2].ToString());
                }
                Apps.Query.DR.Close();
            }
            else if (obj.callback["type"] == "failure")
            {
                Services.MsgFRM msg = new Services.MsgFRM();
                msg.getError(obj.callback["message"]);
            }
            else
            {
                Services.MsgFRM msg = new Services.MsgFRM();
                msg.getError(obj.callback["message"]);
            }
            btnDelete.Enabled = true;
            btnToModify.Enabled = true;
            btnSaved.Enabled = false;
        }

        private void search(string param)
        {
            Models.MTrimestry obj = new Models.MTrimestry();
            obj.reseach(param);
            if (obj.callback["type"] == "success")
            {
                //on vide la dgv
                dgvData.Rows.Clear();
                MySqlDataReader dr = Apps.Query.DR;

                while (dr.Read())
                {
                    dgvData.Rows.Add(dr[0].ToString(), dr[1].ToString(), dr[2].ToString());
                }
                Apps.Query.DR.Close();
            }
            else if (obj.callback["type"] == "failure")
            {
                Services.MsgFRM msg = new Services.MsgFRM();
                msg.getError(obj.callback["message"]);
            }
            else
            {
                Services.MsgFRM msg = new Services.MsgFRM();
                msg.getError(obj.callback["message"]);
            }
        }

        #endregion

        #region eventsClicktrimestry
        private void txtdesignArriv_Click(object sender, EventArgs e)
        {
            if (txtWording.Text == "Nom du trimestre")
            {
                txtWording.Text = string.Empty;
            }
        }

        private void txtdescription_Click(object sender, EventArgs e)
        {
            if (txtWording.Text == string.Empty)
            {
                txtWording.Text = "Nom du trimestre";
            }
        }

        private void txtRechercheArriv_Click(object sender, EventArgs e)
        {
            if (txtRechercheArriv.Text == "Taper ici le text à rechercher")
            {
                txtRechercheArriv.Text = "";
            }
        }

        #endregion

        #region btnCtrl trimestry
        private void btnCharger_Click(object sender, EventArgs e)
        {
            loard();
        }

        private void btnAjouter_Click(object sender, EventArgs e)
        {
            save();
        }

        private void btnNewUser_Click(object sender, EventArgs e)
        {
            EthiquetteArr();
            
            btnDelete.Enabled = false;
            btnSaved.Enabled = true;
            btnToModify.Enabled = false;
        }

        private void btnModifier_Click(object sender, EventArgs e)
        {
            modify();
        }

        private void btnSupp_Click(object sender, EventArgs e)
        {
            delete();
        }

        private void dgvDataAddArrival_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            migrate();
        }

        private void dgvDataAddArrival_SelectionChanged(object sender, EventArgs e)
        {
            migrate();
        }

        private void toolStripMenuItem1_Click(object sender, EventArgs e)
        {
            migrate();
        }

        #endregion
    }
}
