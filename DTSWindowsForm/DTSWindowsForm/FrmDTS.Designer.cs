using DTSWindowsForm.UI.FattureWeb.dtos;

namespace DTSWindowsForm
{
    partial class FrmDTS
    {
        /// <summary>
        ///  Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        ///  Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        ///  Required method for Designer support - do not modify
        ///  the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            components = new System.ComponentModel.Container();
            pnlForm = new Panel();
            btnDashboard = new Button();
            btnBancoLocal = new Button();
            btnFattureWeb = new Button();
            conteudoBindingSource = new BindingSource(components);
            pnlForm.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)conteudoBindingSource).BeginInit();
            SuspendLayout();
            // 
            // pnlForm
            // 
            pnlForm.Controls.Add(btnDashboard);
            pnlForm.Controls.Add(btnBancoLocal);
            pnlForm.Controls.Add(btnFattureWeb);
            pnlForm.Dock = DockStyle.Fill;
            pnlForm.Location = new Point(0, 0);
            pnlForm.Name = "pnlForm";
            pnlForm.Size = new Size(442, 70);
            pnlForm.TabIndex = 0;
            // 
            // btnDashboard
            // 
            btnDashboard.Location = new Point(286, 12);
            btnDashboard.Name = "btnDashboard";
            btnDashboard.Size = new Size(131, 40);
            btnDashboard.TabIndex = 2;
            btnDashboard.Text = "Dashboard";
            btnDashboard.UseVisualStyleBackColor = true;
            btnDashboard.Click += btnDashboard_Click;
            // 
            // btnBancoLocal
            // 
            btnBancoLocal.Location = new Point(149, 12);
            btnBancoLocal.Name = "btnBancoLocal";
            btnBancoLocal.Size = new Size(131, 40);
            btnBancoLocal.TabIndex = 1;
            btnBancoLocal.Text = "Banco Local";
            btnBancoLocal.UseVisualStyleBackColor = true;
            btnBancoLocal.Click += btnBancoLocal_Click;
            // 
            // btnFattureWeb
            // 
            btnFattureWeb.Location = new Point(12, 12);
            btnFattureWeb.Name = "btnFattureWeb";
            btnFattureWeb.Size = new Size(131, 40);
            btnFattureWeb.TabIndex = 0;
            btnFattureWeb.Text = "FattureWeb";
            btnFattureWeb.UseVisualStyleBackColor = true;
            btnFattureWeb.Click += btnFattureWeb_Click;
            // 
            // conteudoBindingSource
            // 
            conteudoBindingSource.DataSource = typeof(Conteudo);
            // 
            // FrmDTS
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(442, 70);
            Controls.Add(pnlForm);
            MaximumSize = new Size(458, 109);
            MinimumSize = new Size(458, 109);
            Name = "FrmDTS";
            StartPosition = FormStartPosition.CenterScreen;
            Text = "DevTools Share";
            pnlForm.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)conteudoBindingSource).EndInit();
            ResumeLayout(false);
        }

        #endregion

        private Panel pnlForm;
        private DataGridViewTextBoxColumn leituraDataGridViewTextBoxColumn;
        private DataGridViewTextBoxColumn produtosDataGridViewTextBoxColumn;
        private DataGridViewTextBoxColumn tributosDataGridViewTextBoxColumn;
        private DataGridViewTextBoxColumn indicadoresDataGridViewTextBoxColumn;
        private DataGridViewTextBoxColumn totalPagarDataGridViewTextBoxColumn;
        private DataGridViewTextBoxColumn dataEmissaoDataGridViewTextBoxColumn;
        private DataGridViewTextBoxColumn totalFaturaDataGridViewTextBoxColumn;
        private DataGridViewTextBoxColumn numeroFaturaDataGridViewTextBoxColumn;
        private DataGridViewTextBoxColumn mesReferenciaDataGridViewTextBoxColumn;
        private DataGridViewTextBoxColumn dataVencimentoDataGridViewTextBoxColumn;
        private DataGridViewTextBoxColumn dataApresentacaoDataGridViewTextBoxColumn;
        private DataGridViewTextBoxColumn bandeirasTarifariasDataGridViewTextBoxColumn;
        private DataGridViewTextBoxColumn historicoFaturamentoDataGridViewTextBoxColumn;
        private DataGridViewTextBoxColumn devolucaoGeracaoDataGridViewTextBoxColumn;
        private DataGridViewTextBoxColumn composicaoDataGridViewTextBoxColumn;
        private Button btnBancoLocal;
        private Button btnFattureWeb;
        private BindingSource conteudoBindingSource;
        private Button btnDashboard;
    }
}
