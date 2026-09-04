namespace DTSWindowsForm.UI.FattureWeb
{
    partial class FattureWebForm
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
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
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            components = new System.ComponentModel.Container();
            panel1 = new Panel();
            panel5 = new Panel();
            panel3 = new Panel();
            gridFaturas = new DataGridView();
            panel4 = new Panel();
            btnPaginaAnterior = new Button();
            btnProximaPagina = new Button();
            lblPagina = new Label();
            btnDownloadCsv = new Button();
            btnFiltros = new Button();
            txtQtdFaturasFiltradas = new TextBox();
            label1 = new Label();
            txtQtdFaturas = new TextBox();
            lblFaturas = new Label();
            panel2 = new Panel();
            lblProgresso = new Label();
            btnCancelarBusca = new Button();
            btnBuscarFaturas = new Button();
            txtSenha = new TextBox();
            lblSenha = new Label();
            txtUsuario = new TextBox();
            lblUsuario = new Label();
            label6 = new Label();
            txbDescricoesOriginais = new TextBox();
            lblDescricoesOriginais = new Label();
            label2 = new Label();
            txbDescricaoProdutos = new TextBox();
            lblDescricaoProdutos = new Label();
            label4 = new Label();
            txbInstalacao = new TextBox();
            label3 = new Label();
            label5 = new Label();
            lblMesRef = new Label();
            txbMesRef = new TextBox();
            lblDistribuidora = new Label();
            txbDistribuidora = new TextBox();
            chbFaturasDuplicadas = new CheckBox();
            btnLimparFiltros = new Button();
            btnFiltrar = new Button();
            lblInstalacao = new Label();
            faturasViewDtoBindingSource1 = new BindingSource(components);
            faturasViewDtoBindingSource = new BindingSource(components);
            conteudoBindingSource = new BindingSource(components);
            bcwCarregaDados = new System.ComponentModel.BackgroundWorker();
            panel7 = new Panel();
            tabControl1 = new TabControl();
            tpgFaturas = new TabPage();
            panel9 = new Panel();
            pnlFiltros = new Panel();
            label8 = new Label();
            txbModeloFw = new TextBox();
            label9 = new Label();
            txbClasseConsumo = new TextBox();
            lblClasseConsumo = new Label();
            txbSubgrupo = new TextBox();
            lblSubgrupo = new Label();
            panel10 = new Panel();
            tpgCobrancas = new TabPage();
            cmsLog = new ContextMenuStrip(components);
            checkLog = new ToolStripMenuItem();
            panel1.SuspendLayout();
            panel5.SuspendLayout();
            panel3.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)gridFaturas).BeginInit();
            panel4.SuspendLayout();
            panel2.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)faturasViewDtoBindingSource1).BeginInit();
            ((System.ComponentModel.ISupportInitialize)faturasViewDtoBindingSource).BeginInit();
            ((System.ComponentModel.ISupportInitialize)conteudoBindingSource).BeginInit();
            panel7.SuspendLayout();
            tabControl1.SuspendLayout();
            tpgFaturas.SuspendLayout();
            panel9.SuspendLayout();
            pnlFiltros.SuspendLayout();
            panel10.SuspendLayout();
            cmsLog.SuspendLayout();
            SuspendLayout();
            // 
            // panel1
            // 
            panel1.Controls.Add(panel5);
            panel1.Controls.Add(panel4);
            panel1.Controls.Add(panel2);
            panel1.Dock = DockStyle.Fill;
            panel1.Location = new Point(0, 0);
            panel1.Name = "panel1";
            panel1.Size = new Size(643, 645);
            panel1.TabIndex = 0;
            // 
            // panel5
            // 
            panel5.Controls.Add(panel3);
            panel5.Dock = DockStyle.Fill;
            panel5.Location = new Point(0, 55);
            panel5.Name = "panel5";
            panel5.Size = new Size(643, 554);
            panel5.TabIndex = 3;
            // 
            // panel3
            // 
            panel3.AutoScroll = true;
            panel3.Controls.Add(gridFaturas);
            panel3.Dock = DockStyle.Fill;
            panel3.Location = new Point(0, 0);
            panel3.Name = "panel3";
            panel3.Size = new Size(643, 554);
            panel3.TabIndex = 1;
            // 
            // gridFaturas
            // 
            gridFaturas.AllowUserToOrderColumns = true;
            gridFaturas.ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            gridFaturas.Dock = DockStyle.Fill;
            gridFaturas.EditMode = DataGridViewEditMode.EditOnEnter;
            gridFaturas.Location = new Point(0, 0);
            gridFaturas.Name = "gridFaturas";
            gridFaturas.RowTemplate.ReadOnly = true;
            gridFaturas.SelectionMode = DataGridViewSelectionMode.CellSelect;
            gridFaturas.Size = new Size(643, 554);
            gridFaturas.TabIndex = 0;
            gridFaturas.DataSourceChanged += gridFaturas_DataSourceChanged;
            // 
            // panel4
            // 
            panel4.Controls.Add(btnPaginaAnterior);
            panel4.Controls.Add(btnProximaPagina);
            panel4.Controls.Add(lblPagina);
            panel4.Controls.Add(btnDownloadCsv);
            panel4.Controls.Add(btnFiltros);
            panel4.Controls.Add(txtQtdFaturasFiltradas);
            panel4.Controls.Add(label1);
            panel4.Controls.Add(txtQtdFaturas);
            panel4.Controls.Add(lblFaturas);
            panel4.Dock = DockStyle.Bottom;
            panel4.Location = new Point(0, 609);
            panel4.Name = "panel4";
            panel4.Size = new Size(643, 36);
            panel4.TabIndex = 2;
            // 
            // btnPaginaAnterior
            // 
            btnPaginaAnterior.Location = new Point(377, 5);
            btnPaginaAnterior.Name = "btnPaginaAnterior";
            btnPaginaAnterior.Size = new Size(30, 24);
            btnPaginaAnterior.TabIndex = 10;
            btnPaginaAnterior.Text = "<";
            btnPaginaAnterior.UseVisualStyleBackColor = true;
            btnPaginaAnterior.Click += btnPaginaAnterior_Click;
            // 
            // btnProximaPagina
            // 
            btnProximaPagina.Location = new Point(502, 5);
            btnProximaPagina.Name = "btnProximaPagina";
            btnProximaPagina.Size = new Size(30, 24);
            btnProximaPagina.TabIndex = 12;
            btnProximaPagina.Text = ">";
            btnProximaPagina.UseVisualStyleBackColor = true;
            btnProximaPagina.Click += btnProximaPagina_Click;
            // 
            // lblPagina
            // 
            lblPagina.Location = new Point(410, 9);
            lblPagina.Name = "lblPagina";
            lblPagina.Size = new Size(89, 15);
            lblPagina.TabIndex = 11;
            lblPagina.Text = "Página 1/1";
            lblPagina.TextAlign = ContentAlignment.MiddleCenter;
            // 
            // btnDownloadCsv
            // 
            btnDownloadCsv.BackColor = Color.DarkOrange;
            btnDownloadCsv.FlatStyle = FlatStyle.Popup;
            btnDownloadCsv.Font = new Font("Segoe UI", 9F);
            btnDownloadCsv.ForeColor = SystemColors.ControlText;
            btnDownloadCsv.Location = new Point(276, 5);
            btnDownloadCsv.Name = "btnDownloadCsv";
            btnDownloadCsv.Size = new Size(95, 24);
            btnDownloadCsv.TabIndex = 6;
            btnDownloadCsv.Text = "🔼 Exportar";
            btnDownloadCsv.UseVisualStyleBackColor = false;
            btnDownloadCsv.Click += btnDownloadCsv_Click;
            // 
            // btnFiltros
            // 
            btnFiltros.Dock = DockStyle.Right;
            btnFiltros.Location = new Point(568, 0);
            btnFiltros.Name = "btnFiltros";
            btnFiltros.Size = new Size(75, 36);
            btnFiltros.TabIndex = 1;
            btnFiltros.Text = "Filtros >>";
            btnFiltros.UseVisualStyleBackColor = true;
            btnFiltros.Click += btnFiltros_Click;
            // 
            // txtQtdFaturasFiltradas
            // 
            txtQtdFaturasFiltradas.Enabled = false;
            txtQtdFaturasFiltradas.Location = new Point(195, 7);
            txtQtdFaturasFiltradas.Name = "txtQtdFaturasFiltradas";
            txtQtdFaturasFiltradas.Size = new Size(75, 23);
            txtQtdFaturasFiltradas.TabIndex = 9;
            // 
            // label1
            // 
            label1.AutoSize = true;
            label1.Font = new Font("Segoe UI", 9F, FontStyle.Bold);
            label1.Location = new Point(132, 10);
            label1.Name = "label1";
            label1.Size = new Size(57, 15);
            label1.TabIndex = 8;
            label1.Text = "Filtrados:";
            // 
            // txtQtdFaturas
            // 
            txtQtdFaturas.Enabled = false;
            txtQtdFaturas.Location = new Point(51, 7);
            txtQtdFaturas.Name = "txtQtdFaturas";
            txtQtdFaturas.Size = new Size(75, 23);
            txtQtdFaturas.TabIndex = 7;
            // 
            // lblFaturas
            // 
            lblFaturas.AutoSize = true;
            lblFaturas.Font = new Font("Segoe UI", 9F, FontStyle.Bold);
            lblFaturas.Location = new Point(8, 10);
            lblFaturas.Name = "lblFaturas";
            lblFaturas.Size = new Size(37, 15);
            lblFaturas.TabIndex = 2;
            lblFaturas.Text = "Total:";
            // 
            // panel2
            // 
            panel2.BorderStyle = BorderStyle.Fixed3D;
            panel2.Controls.Add(lblProgresso);
            panel2.Controls.Add(btnCancelarBusca);
            panel2.Controls.Add(btnBuscarFaturas);
            panel2.Controls.Add(txtSenha);
            panel2.Controls.Add(lblSenha);
            panel2.Controls.Add(txtUsuario);
            panel2.Controls.Add(lblUsuario);
            panel2.Dock = DockStyle.Top;
            panel2.Location = new Point(0, 0);
            panel2.Name = "panel2";
            panel2.Size = new Size(643, 55);
            panel2.TabIndex = 0;
            // 
            // lblProgresso
            // 
            lblProgresso.AutoEllipsis = true;
            lblProgresso.Location = new Point(402, 3);
            lblProgresso.Name = "lblProgresso";
            lblProgresso.Size = new Size(230, 15);
            lblProgresso.TabIndex = 14;
            // 
            // btnCancelarBusca
            // 
            btnCancelarBusca.BackColor = Color.MistyRose;
            btnCancelarBusca.FlatStyle = FlatStyle.Popup;
            btnCancelarBusca.Location = new Point(402, 21);
            btnCancelarBusca.Name = "btnCancelarBusca";
            btnCancelarBusca.Size = new Size(96, 23);
            btnCancelarBusca.TabIndex = 13;
            btnCancelarBusca.Text = "Cancelar";
            btnCancelarBusca.UseVisualStyleBackColor = false;
            btnCancelarBusca.Visible = false;
            btnCancelarBusca.Click += btnCancelarBusca_Click;
            // 
            // btnBuscarFaturas
            // 
            btnBuscarFaturas.BackColor = Color.GreenYellow;
            btnBuscarFaturas.FlatStyle = FlatStyle.Popup;
            btnBuscarFaturas.Location = new Point(288, 21);
            btnBuscarFaturas.Name = "btnBuscarFaturas";
            btnBuscarFaturas.Size = new Size(108, 23);
            btnBuscarFaturas.TabIndex = 12;
            btnBuscarFaturas.Text = "\U0001f7e2 Buscar Faturas";
            btnBuscarFaturas.UseVisualStyleBackColor = false;
            btnBuscarFaturas.Click += btnBuscarFaturas_Click;
            // 
            // txtSenha
            // 
            txtSenha.Location = new Point(170, 21);
            txtSenha.Name = "txtSenha";
            txtSenha.PasswordChar = '●';
            txtSenha.Size = new Size(100, 23);
            txtSenha.TabIndex = 11;
            // 
            // lblSenha
            // 
            lblSenha.AutoSize = true;
            lblSenha.Font = new Font("Segoe UI", 9F, FontStyle.Bold);
            lblSenha.Location = new Point(170, 6);
            lblSenha.Name = "lblSenha";
            lblSenha.Size = new Size(41, 15);
            lblSenha.TabIndex = 10;
            lblSenha.Text = "Senha";
            // 
            // txtUsuario
            // 
            txtUsuario.Location = new Point(16, 21);
            txtUsuario.Name = "txtUsuario";
            txtUsuario.Size = new Size(148, 23);
            txtUsuario.TabIndex = 9;
            // 
            // lblUsuario
            // 
            lblUsuario.AutoSize = true;
            lblUsuario.Font = new Font("Segoe UI", 9F, FontStyle.Bold);
            lblUsuario.Location = new Point(16, 6);
            lblUsuario.Name = "lblUsuario";
            lblUsuario.Size = new Size(36, 15);
            lblUsuario.TabIndex = 8;
            lblUsuario.Text = "Email";
            // 
            // label6
            // 
            label6.AutoSize = true;
            label6.Font = new Font("Segoe UI", 7F);
            label6.ForeColor = SystemColors.ControlDarkDark;
            label6.Location = new Point(108, 293);
            label6.Name = "label6";
            label6.Size = new Size(72, 12);
            label6.TabIndex = 30;
            label6.Text = "separado por ';'";
            // 
            // txbDescricoesOriginais
            // 
            txbDescricoesOriginais.Location = new Point(6, 267);
            txbDescricoesOriginais.Name = "txbDescricoesOriginais";
            txbDescricoesOriginais.Size = new Size(189, 23);
            txbDescricoesOriginais.TabIndex = 29;
            // 
            // lblDescricoesOriginais
            // 
            lblDescricoesOriginais.AutoSize = true;
            lblDescricoesOriginais.Location = new Point(6, 248);
            lblDescricoesOriginais.Name = "lblDescricoesOriginais";
            lblDescricoesOriginais.Size = new Size(113, 15);
            lblDescricoesOriginais.TabIndex = 28;
            lblDescricoesOriginais.Text = "Descrições Originais";
            // 
            // label2
            // 
            label2.AutoSize = true;
            label2.Font = new Font("Segoe UI", 7F);
            label2.ForeColor = SystemColors.ControlDarkDark;
            label2.Location = new Point(108, 233);
            label2.Name = "label2";
            label2.Size = new Size(72, 12);
            label2.TabIndex = 27;
            label2.Text = "separado por ';'";
            // 
            // txbDescricaoProdutos
            // 
            txbDescricaoProdutos.Location = new Point(6, 207);
            txbDescricaoProdutos.Name = "txbDescricaoProdutos";
            txbDescricaoProdutos.Size = new Size(189, 23);
            txbDescricaoProdutos.TabIndex = 26;
            // 
            // lblDescricaoProdutos
            // 
            lblDescricaoProdutos.AutoSize = true;
            lblDescricaoProdutos.Location = new Point(6, 188);
            lblDescricaoProdutos.Name = "lblDescricaoProdutos";
            lblDescricaoProdutos.Size = new Size(109, 15);
            lblDescricaoProdutos.TabIndex = 25;
            lblDescricaoProdutos.Text = "Descrição Produtos";
            // 
            // label4
            // 
            label4.AutoSize = true;
            label4.Font = new Font("Segoe UI", 7F);
            label4.ForeColor = SystemColors.ControlDarkDark;
            label4.Location = new Point(108, 51);
            label4.Name = "label4";
            label4.Size = new Size(72, 12);
            label4.TabIndex = 24;
            label4.Text = "separado por ';'";
            // 
            // txbInstalacao
            // 
            txbInstalacao.Location = new Point(6, 25);
            txbInstalacao.Name = "txbInstalacao";
            txbInstalacao.Size = new Size(189, 23);
            txbInstalacao.TabIndex = 23;
            // 
            // label3
            // 
            label3.AutoSize = true;
            label3.Font = new Font("Segoe UI", 7F);
            label3.ForeColor = SystemColors.ControlDarkDark;
            label3.Location = new Point(108, 111);
            label3.Name = "label3";
            label3.Size = new Size(72, 12);
            label3.TabIndex = 22;
            label3.Text = "separado por ';'";
            // 
            // label5
            // 
            label5.AutoSize = true;
            label5.Font = new Font("Segoe UI", 7F);
            label5.ForeColor = SystemColors.ControlDarkDark;
            label5.Location = new Point(108, 170);
            label5.Name = "label5";
            label5.Size = new Size(72, 12);
            label5.TabIndex = 21;
            label5.Text = "separado por ';'";
            // 
            // lblMesRef
            // 
            lblMesRef.AutoSize = true;
            lblMesRef.Location = new Point(6, 125);
            lblMesRef.Name = "lblMesRef";
            lblMesRef.Size = new Size(100, 15);
            lblMesRef.TabIndex = 17;
            lblMesRef.Text = "Mês de referência";
            // 
            // txbMesRef
            // 
            txbMesRef.Location = new Point(6, 144);
            txbMesRef.Name = "txbMesRef";
            txbMesRef.Size = new Size(189, 23);
            txbMesRef.TabIndex = 16;
            // 
            // lblDistribuidora
            // 
            lblDistribuidora.AutoSize = true;
            lblDistribuidora.Location = new Point(6, 66);
            lblDistribuidora.Name = "lblDistribuidora";
            lblDistribuidora.Size = new Size(75, 15);
            lblDistribuidora.TabIndex = 15;
            lblDistribuidora.Text = "Distribuidora";
            // 
            // txbDistribuidora
            // 
            txbDistribuidora.Location = new Point(6, 85);
            txbDistribuidora.Name = "txbDistribuidora";
            txbDistribuidora.Size = new Size(189, 23);
            txbDistribuidora.TabIndex = 14;
            // 
            // chbFaturasDuplicadas
            // 
            chbFaturasDuplicadas.AutoSize = true;
            chbFaturasDuplicadas.Location = new Point(6, 483);
            chbFaturasDuplicadas.Name = "chbFaturasDuplicadas";
            chbFaturasDuplicadas.Size = new Size(124, 19);
            chbFaturasDuplicadas.TabIndex = 7;
            chbFaturasDuplicadas.Text = "Faturas duplicadas";
            chbFaturasDuplicadas.UseVisualStyleBackColor = true;
            // 
            // btnLimparFiltros
            // 
            btnLimparFiltros.BackColor = Color.PaleGoldenrod;
            btnLimparFiltros.Dock = DockStyle.Right;
            btnLimparFiltros.FlatStyle = FlatStyle.Popup;
            btnLimparFiltros.Location = new Point(6, 0);
            btnLimparFiltros.Name = "btnLimparFiltros";
            btnLimparFiltros.Size = new Size(95, 29);
            btnLimparFiltros.TabIndex = 4;
            btnLimparFiltros.Text = "Limpar Filtros";
            btnLimparFiltros.UseVisualStyleBackColor = false;
            btnLimparFiltros.Click += btnLimparFiltros_Click;
            // 
            // btnFiltrar
            // 
            btnFiltrar.BackColor = Color.GreenYellow;
            btnFiltrar.Dock = DockStyle.Right;
            btnFiltrar.FlatStyle = FlatStyle.Popup;
            btnFiltrar.Location = new Point(101, 0);
            btnFiltrar.Name = "btnFiltrar";
            btnFiltrar.Size = new Size(95, 29);
            btnFiltrar.TabIndex = 2;
            btnFiltrar.Text = "Aplicar Filtros";
            btnFiltrar.UseVisualStyleBackColor = false;
            btnFiltrar.Click += btnFiltrar_Click;
            // 
            // lblInstalacao
            // 
            lblInstalacao.AutoSize = true;
            lblInstalacao.Location = new Point(6, 6);
            lblInstalacao.Name = "lblInstalacao";
            lblInstalacao.Size = new Size(60, 15);
            lblInstalacao.TabIndex = 1;
            lblInstalacao.Text = "Instalação";
            // 
            // bcwCarregaDados
            // 
            bcwCarregaDados.DoWork += bcwCarregaDados_DoWork;
            bcwCarregaDados.RunWorkerCompleted += bcwCarregaDados_RunWorkerCompleted;
            // 
            // panel7
            // 
            panel7.Controls.Add(tabControl1);
            panel7.Dock = DockStyle.Fill;
            panel7.Location = new Point(0, 0);
            panel7.Name = "panel7";
            panel7.Size = new Size(857, 679);
            panel7.TabIndex = 1;
            // 
            // tabControl1
            // 
            tabControl1.Controls.Add(tpgFaturas);
            tabControl1.Controls.Add(tpgCobrancas);
            tabControl1.Dock = DockStyle.Fill;
            tabControl1.Location = new Point(0, 0);
            tabControl1.Multiline = true;
            tabControl1.Name = "tabControl1";
            tabControl1.SelectedIndex = 0;
            tabControl1.Size = new Size(857, 679);
            tabControl1.TabIndex = 1;
            // 
            // tpgFaturas
            // 
            tpgFaturas.Controls.Add(panel9);
            tpgFaturas.Controls.Add(pnlFiltros);
            tpgFaturas.Location = new Point(4, 24);
            tpgFaturas.Name = "tpgFaturas";
            tpgFaturas.Padding = new Padding(3);
            tpgFaturas.Size = new Size(849, 651);
            tpgFaturas.TabIndex = 0;
            tpgFaturas.Text = "Faturas";
            tpgFaturas.UseVisualStyleBackColor = true;
            // 
            // panel9
            // 
            panel9.Controls.Add(panel1);
            panel9.Dock = DockStyle.Fill;
            panel9.Location = new Point(3, 3);
            panel9.Name = "panel9";
            panel9.Size = new Size(643, 645);
            panel9.TabIndex = 2;
            // 
            // pnlFiltros
            // 
            pnlFiltros.BackColor = Color.Transparent;
            pnlFiltros.BorderStyle = BorderStyle.Fixed3D;
            pnlFiltros.Controls.Add(label8);
            pnlFiltros.Controls.Add(txbModeloFw);
            pnlFiltros.Controls.Add(label9);
            pnlFiltros.Controls.Add(txbClasseConsumo);
            pnlFiltros.Controls.Add(lblClasseConsumo);
            pnlFiltros.Controls.Add(txbSubgrupo);
            pnlFiltros.Controls.Add(lblSubgrupo);
            pnlFiltros.Controls.Add(chbFaturasDuplicadas);
            pnlFiltros.Controls.Add(panel10);
            pnlFiltros.Controls.Add(label6);
            pnlFiltros.Controls.Add(txbDescricoesOriginais);
            pnlFiltros.Controls.Add(lblInstalacao);
            pnlFiltros.Controls.Add(lblDescricoesOriginais);
            pnlFiltros.Controls.Add(txbInstalacao);
            pnlFiltros.Controls.Add(label2);
            pnlFiltros.Controls.Add(label4);
            pnlFiltros.Controls.Add(txbDistribuidora);
            pnlFiltros.Controls.Add(lblDistribuidora);
            pnlFiltros.Controls.Add(txbDescricaoProdutos);
            pnlFiltros.Controls.Add(lblDescricaoProdutos);
            pnlFiltros.Controls.Add(label5);
            pnlFiltros.Controls.Add(label3);
            pnlFiltros.Controls.Add(lblMesRef);
            pnlFiltros.Controls.Add(txbMesRef);
            pnlFiltros.Dock = DockStyle.Right;
            pnlFiltros.Location = new Point(646, 3);
            pnlFiltros.Name = "pnlFiltros";
            pnlFiltros.Size = new Size(300, 645);
            pnlFiltros.TabIndex = 1;
            // 
            // label8
            // 
            label8.AutoSize = true;
            label8.Font = new Font("Segoe UI", 7F);
            label8.ForeColor = SystemColors.ControlDarkDark;
            label8.Location = new Point(108, 350);
            label8.Name = "label8";
            label8.Size = new Size(72, 12);
            label8.TabIndex = 35;
            label8.Text = "separado por ';'";
            // 
            // txbModeloFw
            // 
            txbModeloFw.Location = new Point(6, 324);
            txbModeloFw.Name = "txbModeloFw";
            txbModeloFw.Size = new Size(189, 23);
            txbModeloFw.TabIndex = 34;
            // 
            // label9
            // 
            label9.AutoSize = true;
            label9.Location = new Point(6, 305);
            label9.Name = "label9";
            label9.Size = new Size(112, 15);
            label9.TabIndex = 33;
            label9.Text = "Modelo FattureWeb";
            // 
            // txbClasseConsumo
            // 
            txbClasseConsumo.Location = new Point(6, 383);
            txbClasseConsumo.Name = "txbClasseConsumo";
            txbClasseConsumo.Size = new Size(189, 23);
            txbClasseConsumo.TabIndex = 37;
            // 
            // lblClasseConsumo
            // 
            lblClasseConsumo.AutoSize = true;
            lblClasseConsumo.Location = new Point(6, 364);
            lblClasseConsumo.Name = "lblClasseConsumo";
            lblClasseConsumo.Size = new Size(101, 15);
            lblClasseConsumo.TabIndex = 36;
            lblClasseConsumo.Text = "Classe de consumo";
            // 
            // txbSubgrupo
            // 
            txbSubgrupo.Location = new Point(6, 442);
            txbSubgrupo.Name = "txbSubgrupo";
            txbSubgrupo.Size = new Size(189, 23);
            txbSubgrupo.TabIndex = 39;
            // 
            // lblSubgrupo
            // 
            lblSubgrupo.AutoSize = true;
            lblSubgrupo.Location = new Point(6, 423);
            lblSubgrupo.Name = "lblSubgrupo";
            lblSubgrupo.Size = new Size(59, 15);
            lblSubgrupo.TabIndex = 38;
            lblSubgrupo.Text = "Subgrupo";
            // 
            // panel10
            // 
            panel10.Controls.Add(btnLimparFiltros);
            panel10.Controls.Add(btnFiltrar);
            panel10.Dock = DockStyle.Bottom;
            panel10.Location = new Point(0, 612);
            panel10.Name = "panel10";
            panel10.Size = new Size(196, 29);
            panel10.TabIndex = 32;
            // 
            // tpgCobrancas
            // 
            tpgCobrancas.Location = new Point(4, 24);
            tpgCobrancas.Name = "tpgCobrancas";
            tpgCobrancas.Padding = new Padding(3);
            tpgCobrancas.Size = new Size(849, 651);
            tpgCobrancas.TabIndex = 1;
            tpgCobrancas.Text = "Cobrancas";
            tpgCobrancas.UseVisualStyleBackColor = true;
            // 
            // cmsLog
            // 
            cmsLog.Items.AddRange(new ToolStripItem[] { checkLog });
            cmsLog.Name = "cmsLog";
            cmsLog.Size = new Size(181, 48);
            // 
            // checkLog
            // 
            checkLog.CheckOnClick = true;
            checkLog.Name = "checkLog";
            checkLog.Size = new Size(180, 22);
            checkLog.Text = "Gravar Log";
            // 
            // FattureWebForm
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(1057, 679);
            Controls.Add(panel7);
            MinimumSize = new Size(1073, 718);
            Name = "FattureWebForm";
            ShowIcon = true;
            StartPosition = FormStartPosition.CenterScreen;
            Text = "DevTools Share — FattureWeb";
            panel1.ResumeLayout(false);
            panel5.ResumeLayout(false);
            panel3.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)gridFaturas).EndInit();
            panel4.ResumeLayout(false);
            panel4.PerformLayout();
            panel2.ResumeLayout(false);
            panel2.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)faturasViewDtoBindingSource1).EndInit();
            ((System.ComponentModel.ISupportInitialize)faturasViewDtoBindingSource).EndInit();
            ((System.ComponentModel.ISupportInitialize)conteudoBindingSource).EndInit();
            panel7.ResumeLayout(false);
            tabControl1.ResumeLayout(false);
            tpgFaturas.ResumeLayout(false);
            panel9.ResumeLayout(false);
            pnlFiltros.ResumeLayout(false);
            pnlFiltros.PerformLayout();
            panel10.ResumeLayout(false);
            cmsLog.ResumeLayout(false);
            ResumeLayout(false);
        }

        #endregion

        private Panel panel1;
        private Panel panel3;
        private Panel panel2;
        private Label lblFaturas;
        private DataGridView gridFaturas;
        private BindingSource conteudoBindingSource;
        private System.ComponentModel.BackgroundWorker bcwCarregaDados;
        private Button btnDownloadCsv;
        private Button btnPaginaAnterior;
        private Button btnProximaPagina;
        private Label lblPagina;
        private TextBox txtQtdFaturas;
        private DataGridViewTextBoxColumn faturaIdDataGridViewTextBoxColumn;
        private DataGridViewTextBoxColumn instalacaoDataGridViewTextBoxColumn;
        private DataGridViewTextBoxColumn mesReferenciaDataGridViewTextBoxColumn;
        private DataGridViewTextBoxColumn distribuidoraDataGridViewTextBoxColumn;
        private DataGridViewTextBoxColumn idInstalacaoDataGridViewTextBoxColumn;
        private DataGridViewTextBoxColumn dataEmissaoDataGridViewTextBoxColumn;
        private BindingSource faturasViewDtoBindingSource;
        private Panel panel4;
        private Panel panel5;
        private Button btnFiltrar;
        private Label lblInstalacao;
        private TextBox txtFiltroInstalacao;
        private BindingSource faturasViewDtoBindingSource1;
        private Label lblUsuario;
        private TextBox txtSenha;
        private Label lblSenha;
        private TextBox txtUsuario;
        private Button btnBuscarFaturas;
        private Button btnCancelarBusca;
        private Label lblProgresso;
        private Panel panel7;
        private TabControl tabControl1;
        private TabPage tpgFaturas;
        private TabPage tpgCobrancas;
        private Button btnLimparFiltros;
        private TextBox txtQtdFaturasFiltradas;
        private Label label1;
        private CheckBox chbFaturasDuplicadas;
        private Label lblDistribuidora;
        private TextBox txbDistribuidora;
        private Label lblMesRef;
        private TextBox txbMesRef;
        private Label label4;
        private TextBox txbInstalacao;
        private Label label3;
        private Label label5;
        private Label label2;
        private TextBox txbDescricaoProdutos;
        private Label lblDescricaoProdutos;
        private Label label6;
        private TextBox txbDescricoesOriginais;
        private Label lblDescricoesOriginais;
        private Panel panel9;
        private Panel pnlFiltros;
        private Panel panel10;
        private Label label8;
        private TextBox txbModeloFw;
        private Label label9;
        private TextBox txbClasseConsumo;
        private Label lblClasseConsumo;
        private TextBox txbSubgrupo;
        private Label lblSubgrupo;
        private Button btnFiltros;
        private ContextMenuStrip cmsLog;
        private ToolStripMenuItem checkLog;
    }
}
