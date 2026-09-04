using DTSWindowsForm.Extensions;
using DTSWindowsForm.UI.FattureWeb.dtos;
using DTSWindowsForm.UI.UserControls;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Security.Cryptography;
using System.Text;


namespace DTSWindowsForm.UI.FattureWeb
{
    public partial class FattureWebForm : Form
    {
        List<Dado> _dados = new List<Dado>();
        string _token = string.Empty;

        private string _ultimaColunaOrdenada = string.Empty;
        private SortOrder _direcaoOrdenacao = SortOrder.None;
        private FiltrosFaturasDto _filtros = new FiltrosFaturasDto();
        private LoadingControl _loadingControl;
        private List<Dado> _dadosFiltrados = new List<Dado>();
        private bool _gravarLog = false;
        private readonly string URL_FATTUREWEB = "https://api.fattureweb.com.br/";
        private static readonly HttpClient HttpClient = new()
        {
            Timeout = TimeSpan.FromMinutes(10)
        };
        private CancellationTokenSource? _carregamentoCts;
        private List<FaturasProdutoViewDto> _cacheProdutos = new();
        private List<FaturasProdutoViewDto> _produtosFiltrados = new();
        private const int TamanhoPaginaGrid = 500;
        private int _paginaAtual = 1;
        private int _totalFaturasCarregadas;
        private static readonly string DiretorioCache = Path.Combine(
            Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData),
            "DevToolsShare",
            "FattureWeb");
        private readonly Dictionary<Control, ComboBox> _operadoresFiltro = new();
        private readonly Dictionary<Control, (Color Fundo, Color Texto)> _coresOriginais = new();
        private bool _modoEscuro = true;
        private string _usuarioCacheAtual = string.Empty;
        private CheckBox _separarProdutosCheckBox = null!;
        private Label _estadoVazioLabel = null!;
        private DarkScrollBar _rolagemHorizontal = null!;
        private DarkScrollBar _rolagemVertical = null!;
        private Panel _preenchimentoAbas = null!;
        private static readonly Color DarkBackground = Color.FromArgb(15, 18, 22);
        private static readonly Color DarkSurface = Color.FromArgb(24, 29, 35);
        private static readonly Color DarkSurfaceRaised = Color.FromArgb(31, 37, 44);
        private static readonly Color DarkBorder = Color.FromArgb(58, 67, 77);
        private static readonly Color DarkText = Color.FromArgb(232, 235, 239);
        private static readonly Color DarkMutedText = Color.FromArgb(166, 176, 188);
        private static readonly Color DarkAccent = Color.FromArgb(76, 141, 255);

        public FattureWebForm()
        {
            InitializeComponent();
            ConfigurarOperadoresFiltro();
            ConfigurarModoEscuro();
            _loadingControl = new LoadingControl();

            foreach (DataGridViewColumn column in gridFaturas.Columns)
            {
                column.SortMode = DataGridViewColumnSortMode.Automatic;
            }
            gridFaturas.AllowUserToOrderColumns = true;
            gridFaturas.ColumnHeaderMouseClick += dataGridView_ColumnHeaderMouseClick;

            if (Program.AppSettings != null)
            {
                txtUsuario.Text = Program.AppSettings.Usuario;
                txtSenha.Text = Program.AppSettings.Senha;
            }

            // Criar a primeira coluna de botão (Lupa - Visualizar Fatura)
            DataGridViewButtonColumn btnLupa = new DataGridViewButtonColumn();
            btnLupa.Name = "VisualizarFatura";
            btnLupa.HeaderText = "⬇️";
            btnLupa.Text = "🔍"; // Apenas um símbolo para diferenciar
            btnLupa.UseColumnTextForButtonValue = true; // Garante que o botão mostra o texto
            btnLupa.Width = 30;
            btnLupa.FlatStyle = FlatStyle.Flat;

            // Criar a segunda coluna de botão (Chaves - Visualizar JSON)
            DataGridViewButtonColumn btnJson = new DataGridViewButtonColumn();
            btnJson.Name = "VisualizarJson";
            btnJson.HeaderText = "⬇️";
            btnJson.Text = "{ }"; // Representação do JSON
            btnJson.UseColumnTextForButtonValue = true;
            btnJson.Width = 30;
            btnJson.FlatStyle = FlatStyle.Flat;

            // Adicionar as colunas no DataGridView
            gridFaturas.Columns.Insert(0, btnJson);
            gridFaturas.Columns.Insert(0, btnLupa);

            // Adicionar ToolTips
            gridFaturas.CellMouseEnter += (s, e) =>
            {
                if (e.RowIndex >= 0)
                {
                    if (e.ColumnIndex == gridFaturas.Columns["VisualizarFatura"].Index)
                        gridFaturas.Rows[e.RowIndex].Cells[e.ColumnIndex].ToolTipText = "Baixar Fatura";
                    else if (e.ColumnIndex == gridFaturas.Columns["VisualizarJson"].Index)
                        gridFaturas.Rows[e.RowIndex].Cells[e.ColumnIndex].ToolTipText = "Baixar JSON";
                }
            };

            // Capturar clique nos botões            
            gridFaturas.CellClick += (s, e) =>
            {
                if (e.RowIndex >= 0)
                {
                    // Obtém o objeto vinculado à linha
                    var item = (FaturasProdutoViewDto)gridFaturas.Rows[e.RowIndex].DataBoundItem;
                    if (item == null) return;

                    if (e.ColumnIndex == gridFaturas.Columns["VisualizarFatura"].Index)
                        BaixarPdf(item);
                    else if (e.ColumnIndex == gridFaturas.Columns["VisualizarJson"].Index)
                        BaixarJson(item);
                }
            };

            btnBuscarFaturas.MouseUp += (sender, e) =>
            {
                if (e.Button == MouseButtons.Right)
                {
                    cmsLog.Show(btnBuscarFaturas, e.Location);
                }
            };

            checkLog.Click += (sender, e) =>
            {
                _gravarLog = checkLog.Checked;
            };
        }

        private void ConfigurarOperadoresFiltro()
        {
            AdicionarOperador(txbInstalacao, 3);
            AdicionarOperador(txbDistribuidora, 63);
            AdicionarOperador(txbMesRef, 122);
            AdicionarOperador(txbDescricaoProdutos, 185);
            AdicionarOperador(txbDescricoesOriginais, 245);
            AdicionarOperador(txbModeloFw, 302);
            AdicionarOperador(txbClasseConsumo, 361);
            AdicionarOperador(txbSubgrupo, 420);
        }

        private void AdicionarOperador(Control filtro, int y)
        {
            filtro.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
            filtro.Width = pnlFiltros.ClientSize.Width - 12;
            var operador = new DarkComboBox
            {
                DropDownStyle = ComboBoxStyle.DropDownList,
                Location = new Point(
                    pnlFiltros.ClientSize.Width - 106,
                    Math.Max(2, filtro.Top - 25)),
                Size = new Size(100, 23),
                Font = new Font("Segoe UI", 8F),
                Anchor = AnchorStyles.Top | AnchorStyles.Right,
                DrawMode = DrawMode.OwnerDrawFixed
            };
            operador.Items.AddRange(new object[] { "Igual a", "Diferente de", "Contém", "Não contém" });
            operador.SelectedIndex = 0;
            operador.DrawItem += (_, e) =>
            {
                if (e.Index < 0)
                    return;
                Color fundo = _modoEscuro ? DarkSurfaceRaised : Color.White;
                Color texto = _modoEscuro ? DarkText : SystemColors.ControlText;
                using var fundoBrush = new SolidBrush(fundo);
                using var textoBrush = new SolidBrush(texto);
                e.Graphics.FillRectangle(fundoBrush, e.Bounds);
                e.Graphics.DrawString(
                    operador.Items[e.Index]?.ToString(),
                    operador.Font,
                    textoBrush,
                    e.Bounds.Left + 4,
                    e.Bounds.Top + 2);
            };
            pnlFiltros.Controls.Add(operador);
            operador.BringToFront();
            _operadoresFiltro[filtro] = operador;
        }

        private bool AplicarOperador(Control filtro, bool corresponde)
        {
            return _operadoresFiltro.TryGetValue(filtro, out var operador)
                && operador.SelectedItem?.ToString() is "Diferente de" or "Não contém"
                    ? !corresponde
                    : corresponde;
        }

        private void ConfigurarModoEscuro()
        {
            _estadoVazioLabel = new Label
            {
                Text = "Nenhum resultado encontrado\nRevise os filtros e tente novamente.",
                AutoSize = false,
                TextAlign = ContentAlignment.MiddleCenter,
                Dock = DockStyle.Fill,
                Font = new Font("Segoe UI", 12F),
                Visible = false
            };
            panel3.Controls.Add(_estadoVazioLabel);
            _estadoVazioLabel.BringToFront();

            _separarProdutosCheckBox = new CheckBox
            {
                Text = "Separar produtos em linhas",
                AutoSize = true,
                Location = new Point(6, txbSubgrupo.Bottom + 48),
                Checked = false
            };
            _separarProdutosCheckBox.CheckedChanged += async (_, _) =>
                await AtualizarVisualizacaoAsync();
            pnlFiltros.Controls.Add(_separarProdutosCheckBox);
            _separarProdutosCheckBox.BringToFront();

            var alternarTema = new CheckBox
            {
                Text = "Modo escuro",
                AutoSize = true,
                Location = new Point(6, txbSubgrupo.Bottom + 78),
                Checked = true
            };
            alternarTema.CheckedChanged += (_, _) =>
            {
                _modoEscuro = alternarTema.Checked;
                AplicarTema(this);
            };
            pnlFiltros.Controls.Add(alternarTema);
            alternarTema.BringToFront();
            chbFaturasDuplicadas.Location = new Point(6, txbSubgrupo.Bottom + 18);
            chbFaturasDuplicadas.BringToFront();
            OrganizarPainelFiltros(alternarTema);
            ConfigurarRolagemEscura();
            ConfigurarFundoAbas();

            tabControl1.DrawMode = TabDrawMode.OwnerDrawFixed;
            tabControl1.DrawItem += (_, e) =>
            {
                var pagina = tabControl1.TabPages[e.Index];
                bool selecionada = e.Index == tabControl1.SelectedIndex;
                Color fundo = _modoEscuro
                    ? (selecionada ? DarkSurfaceRaised : DarkBackground)
                    : SystemColors.Control;
                Color texto = _modoEscuro ? DarkText : SystemColors.ControlText;
                using var brushFundo = new SolidBrush(fundo);
                using var brushTexto = new SolidBrush(texto);
                e.Graphics.FillRectangle(brushFundo, e.Bounds);
                var formato = new StringFormat
                {
                    Alignment = StringAlignment.Center,
                    LineAlignment = StringAlignment.Center
                };
                e.Graphics.DrawString(pagina.Text, Font, brushTexto, e.Bounds, formato);
                if (_modoEscuro && selecionada)
                {
                    using var destaque = new Pen(DarkAccent, 2);
                    e.Graphics.DrawLine(destaque, e.Bounds.Left + 4, e.Bounds.Bottom - 1,
                        e.Bounds.Right - 4, e.Bounds.Bottom - 1);
                }
            };
            AplicarTema(this);
        }

        private void OrganizarPainelFiltros(CheckBox alternarTema)
        {
            pnlFiltros.SuspendLayout();
            pnlFiltros.BorderStyle = BorderStyle.None;
            tpgFaturas.Padding = Padding.Empty;
            panel10.Height = 44;
            btnFiltrar.Width = 120;
            btnLimparFiltros.Width = 120;

            var fluxo = new FlowLayoutPanel
            {
                Dock = DockStyle.Fill,
                FlowDirection = FlowDirection.TopDown,
                WrapContents = false,
                AutoScroll = true,
                Padding = new Padding(12, 10, 12, 12),
                Margin = Padding.Empty,
                Name = "flowFiltros"
            };

            pnlFiltros.Controls.Clear();
            pnlFiltros.Controls.Add(fluxo);
            pnlFiltros.Controls.Add(panel10);
            panel10.Dock = DockStyle.Bottom;
            panel10.BringToFront();

            var grupos = new[]
            {
                CriarGrupoFiltro("Instalação", txbInstalacao),
                CriarGrupoFiltro("Distribuidora", txbDistribuidora),
                CriarGrupoFiltro("Mês de referência", txbMesRef),
                CriarGrupoFiltro("Descrição dos produtos", txbDescricaoProdutos),
                CriarGrupoFiltro("Descrições originais", txbDescricoesOriginais),
                CriarGrupoFiltro("Modelo FattureWeb", txbModeloFw),
                CriarGrupoFiltro("Classe de consumo", txbClasseConsumo),
                CriarGrupoFiltro("Subgrupo", txbSubgrupo)
            };

            foreach (var grupo in grupos)
                fluxo.Controls.Add(grupo);

            var opcoes = new Panel
            {
                Height = 100,
                Margin = new Padding(0, 8, 0, 0),
                Name = "pnlOpcoesFiltro"
            };
            chbFaturasDuplicadas.Location = new Point(4, 4);
            _separarProdutosCheckBox.Location = new Point(4, 34);
            alternarTema.Location = new Point(4, 64);
            opcoes.Controls.Add(chbFaturasDuplicadas);
            opcoes.Controls.Add(_separarProdutosCheckBox);
            opcoes.Controls.Add(alternarTema);
            fluxo.Controls.Add(opcoes);

            void AjustarLarguras()
            {
                int largura = Math.Max(220, fluxo.ClientSize.Width - fluxo.Padding.Horizontal - 4);
                foreach (Control grupo in fluxo.Controls)
                    grupo.Width = largura;
            }

            fluxo.SizeChanged += (_, _) => AjustarLarguras();
            AjustarLarguras();
            pnlFiltros.ResumeLayout();
        }

        private Panel CriarGrupoFiltro(string titulo, TextBox campo)
        {
            var operador = _operadoresFiltro[campo];
            var grupo = new Panel
            {
                Width = 260,
                Height = 68,
                Margin = new Padding(0, 0, 0, 8),
                Name = $"grupo{campo.Name}"
            };
            var rotulo = new Label
            {
                Text = titulo,
                AutoSize = true,
                Location = new Point(1, 4),
                Font = new Font("Segoe UI", 9F, FontStyle.Bold)
            };

            operador.Parent = grupo;
            operador.Location = new Point(grupo.Width - 104, 0);
            operador.Anchor = AnchorStyles.Top | AnchorStyles.Right;
            campo.Parent = grupo;
            campo.Location = new Point(1, 34);
            campo.Width = grupo.Width - 2;
            campo.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
            grupo.Controls.Add(rotulo);
            operador.BringToFront();
            return grupo;
        }

        private void ConfigurarFundoAbas()
        {
            _preenchimentoAbas = new Panel
            {
                Height = 24,
                BackColor = DarkBackground,
                Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right
            };
            // TabControl aceita somente TabPages como filhos diretos. A faixa é
            // sobreposta no mesmo container do TabControl.
            tabControl1.Parent!.Controls.Add(_preenchimentoAbas);

            void Ajustar()
            {
                int inicio = tabControl1.TabCount == 0
                    ? 0
                    : tabControl1.GetTabRect(tabControl1.TabCount - 1).Right;
                _preenchimentoAbas.SetBounds(
                    tabControl1.Left + inicio,
                    tabControl1.Top,
                    Math.Max(0, tabControl1.ClientSize.Width - inicio),
                    24);
                _preenchimentoAbas.Visible = _modoEscuro;
                _preenchimentoAbas.BringToFront();
            }

            tabControl1.Resize += (_, _) => Ajustar();
            tabControl1.SelectedIndexChanged += (_, _) => Ajustar();
            Ajustar();
        }

        private void ConfigurarRolagemEscura()
        {
            gridFaturas.ScrollBars = ScrollBars.None;
            _rolagemHorizontal = new DarkScrollBar(Orientation.Horizontal)
            {
                Dock = DockStyle.Bottom,
                Height = 15
            };
            _rolagemVertical = new DarkScrollBar(Orientation.Vertical)
            {
                Dock = DockStyle.Right,
                Width = 15
            };
            gridFaturas.Controls.Add(_rolagemHorizontal);
            gridFaturas.Controls.Add(_rolagemVertical);
            _rolagemHorizontal.BringToFront();
            _rolagemVertical.BringToFront();

            _rolagemVertical.ValueChanged += (_, _) =>
            {
                if (gridFaturas.RowCount > 0)
                    gridFaturas.FirstDisplayedScrollingRowIndex =
                        Math.Min(_rolagemVertical.Value, gridFaturas.RowCount - 1);
            };
            _rolagemHorizontal.ValueChanged += (_, _) =>
                gridFaturas.HorizontalScrollingOffset = _rolagemHorizontal.Value;

            void Atualizar()
            {
                int linhasVisiveis = Math.Max(1, gridFaturas.DisplayedRowCount(false));
                _rolagemVertical.Configure(
                    Math.Max(0, gridFaturas.RowCount - linhasVisiveis), linhasVisiveis);
                int larguraColunas = gridFaturas.Columns.Cast<DataGridViewColumn>()
                    .Where(c => c.Visible).Sum(c => c.Width);
                _rolagemHorizontal.Configure(
                    Math.Max(0, larguraColunas - gridFaturas.ClientSize.Width + 15),
                    Math.Max(1, gridFaturas.ClientSize.Width));
                _rolagemHorizontal.Visible = _modoEscuro && _rolagemHorizontal.Maximum > 0;
                _rolagemVertical.Visible = _modoEscuro && _rolagemVertical.Maximum > 0;
                _rolagemHorizontal.BringToFront();
                _rolagemVertical.BringToFront();
            }

            gridFaturas.DataBindingComplete += (_, _) => Atualizar();
            gridFaturas.Resize += (_, _) => Atualizar();
            gridFaturas.ColumnWidthChanged += (_, _) => Atualizar();
            gridFaturas.MouseWheel += (_, e) =>
                _rolagemVertical.Value = Math.Clamp(
                    _rolagemVertical.Value - Math.Sign(e.Delta) * 3,
                    0, _rolagemVertical.Maximum);
            Atualizar();
        }

        private void AplicarTema(Control controle)
        {
            if (_modoEscuro)
            {
                _coresOriginais.TryAdd(controle, (controle.BackColor, controle.ForeColor));
                controle.ForeColor = DarkText;
                controle.BackColor = controle switch
                {
                    TextBox or RichTextBox => DarkBackground,
                    ComboBox => DarkSurfaceRaised,
                    Panel or TabPage => DarkSurface,
                    Button => DarkSurfaceRaised,
                    _ => DarkBackground
                };

                if (controle is Button botao)
                {
                    botao.FlatStyle = FlatStyle.Flat;
                    botao.FlatAppearance.BorderColor = DarkBorder;
                    botao.FlatAppearance.MouseOverBackColor = Color.FromArgb(43, 51, 61);
                    botao.FlatAppearance.MouseDownBackColor = DarkAccent;
                    botao.UseVisualStyleBackColor = false;
                }
                else if (controle is ComboBox combo)
                {
                    combo.FlatStyle = FlatStyle.Flat;
                }
                else if (controle is TextBox caixaTexto)
                {
                    caixaTexto.BorderStyle = BorderStyle.FixedSingle;
                }
                else if (controle is Label label && label.Font.Size <= 7.5F)
                {
                    label.ForeColor = DarkMutedText;
                }
            }
            else if (_coresOriginais.TryGetValue(controle, out var cores))
            {
                controle.BackColor = cores.Fundo;
                controle.ForeColor = cores.Texto;
                if (controle is Button botao)
                {
                    botao.FlatAppearance.BorderColor = SystemColors.ControlDark;
                    botao.FlatAppearance.MouseOverBackColor = Color.Empty;
                    botao.FlatAppearance.MouseDownBackColor = Color.Empty;
                }
            }

            foreach (Control filho in controle.Controls)
                AplicarTema(filho);

            AplicarEstiloGrid();
            if (controle == this && _modoEscuro)
            {
                panel2.BackColor = DarkSurfaceRaised;
                panel4.BackColor = DarkSurfaceRaised;
                pnlFiltros.BackColor = DarkSurface;
                if (pnlFiltros.Controls["flowFiltros"] is FlowLayoutPanel fluxo)
                {
                    fluxo.BackColor = DarkSurface;
                    foreach (Control grupo in fluxo.Controls)
                        grupo.BackColor = DarkSurfaceRaised;
                }
                btnBuscarFaturas.BackColor = Color.FromArgb(42, 157, 91);
                btnBuscarFaturas.ForeColor = Color.White;
                btnFiltrar.BackColor = DarkAccent;
                btnFiltrar.ForeColor = Color.White;
                btnLimparFiltros.BackColor = DarkSurfaceRaised;
                btnDownloadCsv.BackColor = Color.FromArgb(215, 139, 46);
                btnDownloadCsv.ForeColor = Color.White;
                _estadoVazioLabel.BackColor = DarkBackground;
                _estadoVazioLabel.ForeColor = DarkMutedText;
            }
            if (controle == this)
            {
                foreach (var combo in _operadoresFiltro.Values.OfType<DarkComboBox>())
                {
                    combo.DarkMode = _modoEscuro;
                    combo.Invalidate();
                }
                if (_preenchimentoAbas != null)
                {
                    _preenchimentoAbas.Visible = _modoEscuro;
                    _preenchimentoAbas.BackColor = DarkBackground;
                    _preenchimentoAbas.BringToFront();
                }
                if (_rolagemHorizontal != null)
                {
                    _rolagemHorizontal.DarkMode = _modoEscuro;
                    _rolagemHorizontal.Visible = _modoEscuro && _rolagemHorizontal.Maximum > 0;
                    _rolagemHorizontal.Invalidate();
                }
                if (_rolagemVertical != null)
                {
                    _rolagemVertical.DarkMode = _modoEscuro;
                    _rolagemVertical.Visible = _modoEscuro && _rolagemVertical.Maximum > 0;
                    _rolagemVertical.Invalidate();
                }
                gridFaturas.ScrollBars = _modoEscuro ? ScrollBars.None : ScrollBars.Both;
            }
            if (controle == this)
                AplicarTemaNativo(_modoEscuro);
            AplicarTemaBarraTitulo(_modoEscuro);
            tabControl1.Invalidate();
        }

        private void AplicarEstiloGrid()
        {
            gridFaturas.EnableHeadersVisualStyles = !_modoEscuro;
            gridFaturas.BackgroundColor = _modoEscuro ? DarkBackground : SystemColors.AppWorkspace;
            gridFaturas.GridColor = _modoEscuro ? DarkBorder : SystemColors.ControlDark;
            gridFaturas.BorderStyle = BorderStyle.None;
            gridFaturas.RowHeadersBorderStyle = DataGridViewHeaderBorderStyle.Single;
            gridFaturas.ColumnHeadersBorderStyle = DataGridViewHeaderBorderStyle.Single;
            gridFaturas.ColumnHeadersHeight = 30;
            gridFaturas.RowTemplate.Height = 26;

            gridFaturas.DefaultCellStyle.BackColor = _modoEscuro ? DarkSurface : Color.White;
            gridFaturas.DefaultCellStyle.ForeColor = _modoEscuro ? DarkText : SystemColors.ControlText;
            gridFaturas.AlternatingRowsDefaultCellStyle.BackColor =
                _modoEscuro ? DarkSurfaceRaised : Color.WhiteSmoke;
            gridFaturas.DefaultCellStyle.SelectionBackColor =
                _modoEscuro ? Color.FromArgb(43, 81, 138) : SystemColors.Highlight;
            gridFaturas.DefaultCellStyle.SelectionForeColor = Color.White;
            gridFaturas.ColumnHeadersDefaultCellStyle.BackColor =
                _modoEscuro ? Color.FromArgb(36, 44, 53) : SystemColors.Control;
            gridFaturas.ColumnHeadersDefaultCellStyle.ForeColor =
                _modoEscuro ? DarkText : SystemColors.ControlText;
            gridFaturas.RowHeadersDefaultCellStyle.BackColor =
                _modoEscuro ? DarkSurfaceRaised : SystemColors.Control;

            foreach (DataGridViewColumn coluna in gridFaturas.Columns)
            {
                if (coluna is DataGridViewButtonColumn botaoColuna)
                {
                    botaoColuna.FlatStyle = FlatStyle.Flat;
                    coluna.DefaultCellStyle.BackColor = _modoEscuro ? DarkSurfaceRaised : Color.White;
                    coluna.DefaultCellStyle.ForeColor = _modoEscuro ? DarkAccent : SystemColors.ControlText;
                    coluna.DefaultCellStyle.SelectionBackColor =
                        _modoEscuro ? Color.FromArgb(43, 81, 138) : SystemColors.Highlight;
                }
            }
        }

        private void AplicarTemaBarraTitulo(bool escuro)
        {
            if (!OperatingSystem.IsWindows() || !IsHandleCreated)
                return;
            int valor = escuro ? 1 : 0;
            if (DwmSetWindowAttribute(Handle, 20, ref valor, sizeof(int)) != 0)
                DwmSetWindowAttribute(Handle, 19, ref valor, sizeof(int));

            if (escuro)
            {
                int corFundo = ColorTranslator.ToWin32(DarkSurface);
                int corTexto = ColorTranslator.ToWin32(DarkText);
                int corBorda = ColorTranslator.ToWin32(DarkBorder);
                DwmSetWindowAttribute(Handle, 35, ref corFundo, sizeof(int));
                DwmSetWindowAttribute(Handle, 36, ref corTexto, sizeof(int));
                DwmSetWindowAttribute(Handle, 34, ref corBorda, sizeof(int));
            }
            else
            {
                int padrao = unchecked((int)0xFFFFFFFF);
                DwmSetWindowAttribute(Handle, 35, ref padrao, sizeof(int));
                DwmSetWindowAttribute(Handle, 36, ref padrao, sizeof(int));
                DwmSetWindowAttribute(Handle, 34, ref padrao, sizeof(int));
            }
        }

        private void AplicarTemaNativo(bool escuro)
        {
            if (!OperatingSystem.IsWindows() || !IsHandleCreated)
                return;

            string tema = escuro ? "DarkMode_Explorer" : "Explorer";
            AplicarTemaNativoRecursivo(this, tema);
            SetWindowTheme(gridFaturas.Handle, tema, null);
            if (pnlFiltros.Controls["flowFiltros"] is FlowLayoutPanel fluxo)
                SetWindowTheme(fluxo.Handle, tema, null);

            RedrawWindow(Handle, IntPtr.Zero, IntPtr.Zero,
                0x0080 | 0x0100 | 0x0001 | 0x0400);
        }

        private static void AplicarTemaNativoRecursivo(Control controle, string tema)
        {
            if (controle.IsHandleCreated &&
                controle is Button or ComboBox or CheckBox or TabControl
                    or DataGridView or ScrollableControl)
            {
                SetWindowTheme(controle.Handle, tema, null);
            }

            foreach (Control filho in controle.Controls)
                AplicarTemaNativoRecursivo(filho, tema);
        }

        [DllImport("dwmapi.dll")]
        private static extern int DwmSetWindowAttribute(
            IntPtr hwnd,
            int atributo,
            ref int valor,
            int tamanho);

        [DllImport("uxtheme.dll", CharSet = CharSet.Unicode)]
        private static extern int SetWindowTheme(
            IntPtr hwnd,
            string? subAppName,
            string? subIdList);

        [DllImport("user32.dll")]
        private static extern bool RedrawWindow(
            IntPtr hwnd,
            IntPtr updateRect,
            IntPtr updateRegion,
            uint flags);

        void BaixarPdf(FaturasProdutoViewDto fatura)
        {
            try
            {
                StartLoading();
                MessageBox.Show($"O download do PDF da fatura: {fatura.FaturaId} será iniciado.");

                using (var client = new HttpClient())
                {
                    var url = $"{URL_FATTUREWEB}faturas/{fatura.FaturaId}/arquivo";
                    var requestArquivo = new HttpRequestMessage(HttpMethod.Get, url);
                    requestArquivo.Headers.Add("Fatture-AuthToken", _token);

                    var responseArquivos = client.Send(requestArquivo);

                    if (responseArquivos.IsSuccessStatusCode)
                    {
                        string responseStringArquivos = responseArquivos.Content.ReadAsStringAsync().Result;
                        FattureWebArquivoResponse arquivos = JsonConvert.DeserializeObject<FattureWebArquivoResponse>(responseStringArquivos);

                        var linkPdf = arquivos.Dados.First().AwsPresignedUrl;

                        try
                        {
                            HttpResponseMessage response = client.GetAsync(linkPdf).Result;
                            response.EnsureSuccessStatusCode();

                            byte[] pdfFatura = response.Content.ReadAsByteArrayAsync().Result;
                            var outputPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Faturas");

                            if (!Directory.Exists(outputPath))
                            {
                                Directory.CreateDirectory(outputPath);
                            }

                            string fileName = $"FAT-{fatura.FaturaId}.pdf";
                            string fullPath = Path.Combine(outputPath, fileName);

                            File.WriteAllBytes(fullPath, pdfFatura);

                            DialogResult result = MessageBox.Show($"Fatura salva em:\n{fullPath}\n\nDeseja abrir o diretório onde o arquivo foi salvo?", "Download Concluído", MessageBoxButtons.YesNo, MessageBoxIcon.Question);

                            if (result == DialogResult.Yes)
                            {
                                System.Diagnostics.Process.Start("explorer.exe", Path.GetDirectoryName(fullPath));
                            }
                        }
                        catch (Exception ex)
                        {
                            MessageBox.Show($"Erro ao baixar o arquivo PDF: {ex.Message}", "Erro", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        }
                    }
                    else
                    {
                        MessageBox.Show($"Erro ao obter URL do PDF: {responseArquivos.ReasonPhrase}", "Erro", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Erro inesperado: {ex.Message}", "Erro", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            finally
            {
                StopLoading();
            }
        }

        void BaixarJson(FaturasProdutoViewDto fatura)
        {
            try
            {
                StartLoading();
                MessageBox.Show($"O download do JSON da fatura: {fatura.FaturaId} será iniciado.");

                string json = GetJsonFaturas(idFatura: fatura.FaturaId!);

                var outputPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Faturas");

                if (!Directory.Exists(outputPath))
                {
                    Directory.CreateDirectory(outputPath);
                }

                string fileName = $"FAT-{fatura.FaturaId}.json";
                string fullPath = Path.Combine(outputPath, fileName);

                // Tratamento de erro ao desserializar JSON
                try
                {
                    string formattedJson = JsonConvert.SerializeObject(JsonConvert.DeserializeObject(json), Formatting.Indented);
                    File.WriteAllText(fullPath, formattedJson);

                    DialogResult result = MessageBox.Show($"JSON salvo com sucesso em:\n{fullPath}\n\nDeseja abrir o diretório onde o arquivo foi salvo?", "Download Concluído", MessageBoxButtons.YesNo, MessageBoxIcon.Question);

                    if (result == DialogResult.Yes)
                    {
                        System.Diagnostics.Process.Start("explorer.exe", Path.GetDirectoryName(fullPath));
                    }
                }
                catch (JsonException)
                {
                    MessageBox.Show("Erro ao processar o JSON. O conteúdo pode estar corrompido.", "Erro de JSON", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
            catch (UnauthorizedAccessException)
            {
                MessageBox.Show("Erro: Permissão negada para salvar o arquivo. Tente executar o programa como administrador.", "Erro de Permissão", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            catch (IOException ex)
            {
                MessageBox.Show($"Erro ao acessar o arquivo: {ex.Message}", "Erro de Arquivo", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Erro inesperado: {ex.Message}", "Erro Desconhecido", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            finally
            {
                StopLoading();
            }

        }

        private void StartLoading()
        {
            _loadingControl.ShowLoading(this, "aguarde...");
        }

        private void StopLoading()
        {
            _loadingControl.HideLoading(this);
        }

        private void dataGridView_ColumnHeaderMouseClick(object sender, DataGridViewCellMouseEventArgs e)
        {
            string columnName = gridFaturas.Columns[e.ColumnIndex].DataPropertyName;
            if (string.IsNullOrWhiteSpace(columnName))
                return;

            if (_ultimaColunaOrdenada == columnName)
            {
                _direcaoOrdenacao = (_direcaoOrdenacao == SortOrder.Ascending) ? SortOrder.Descending : SortOrder.Ascending;
            }
            else
            {
                _direcaoOrdenacao = columnName == nameof(FaturasProdutoViewDto.MesReferencia)
                    ? SortOrder.Descending
                    : SortOrder.Ascending;
                _ultimaColunaOrdenada = columnName;
            }

            if (_direcaoOrdenacao == SortOrder.Ascending)
            {
                _produtosFiltrados = _produtosFiltrados
                    .OrderBy(d => GetSortValue(d, columnName))
                    .ToList();
            }
            else
            {
                _produtosFiltrados = _produtosFiltrados
                    .OrderByDescending(d => GetSortValue(d, columnName))
                    .ToList();
            }
            _paginaAtual = 1;
            ExibirPaginaAtual();
            gridFaturas.Columns[e.ColumnIndex].HeaderCell.SortGlyphDirection = _direcaoOrdenacao;
        }

        private object GetSortValue(FaturasProdutoViewDto item, string propertyName)
        {
            object? valor = item.GetType().GetProperty(propertyName)?.GetValue(item);
            if (propertyName == nameof(FaturasProdutoViewDto.MesReferencia))
                return ObterChaveMesReferencia(valor?.ToString());
            return valor ?? string.Empty;
        }

        private static int ObterChaveMesReferencia(string? mesReferencia)
        {
            if (string.IsNullOrWhiteSpace(mesReferencia))
                return 0;

            string digitos = new string(mesReferencia.Where(char.IsDigit).ToArray());
            if (digitos.Length != 6 ||
                !int.TryParse(digitos[..2], out int mes) ||
                !int.TryParse(digitos[2..], out int ano) ||
                mes is < 1 or > 12)
                return 0;

            return ano * 100 + mes;
        }
        private void IniciaBackground() => _ = CarregarDadosAsync();

        private void bcwCarregaDados_DoWork(object sender, System.ComponentModel.DoWorkEventArgs e) { }

        private void bcwCarregaDados_RunWorkerCompleted(object sender, System.ComponentModel.RunWorkerCompletedEventArgs e)
        {
            try
            {
                if (e.Error != null)
                {
                    MessageBox.Show(
                        $"Não foi possível carregar as faturas.\n{e.Error.Message}",
                        "Erro",
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Error);
                    return;
                }

                AjustarInterface();
            }
            finally
            {
                StopLoading();
            }
        }

        private void AjustarInterface()
        {
            PreencherGridFaturas();
        }

        private void PreencherGridFaturas()
        {
            var operadores = CapturarOperadores();
            IEnumerable<FaturasProdutoViewDto> origem = _separarProdutosCheckBox.Checked
                ? _cacheProdutos
                : AgruparPorFatura(_cacheProdutos);
            _produtosFiltrados = FiltrarCache(
                origem,
                _filtros,
                operadores,
                chbFaturasDuplicadas.Checked);
            _paginaAtual = 1;
            ExibirPaginaAtual();
        }

        private async Task AtualizarVisualizacaoAsync()
        {
            if (_cacheProdutos.Count == 0)
                return;

            var filtros = _filtros;
            var operadores = CapturarOperadores();
            bool somenteDuplicadas = chbFaturasDuplicadas.Checked;
            bool separarProdutos = _separarProdutosCheckBox.Checked;
            btnFiltrar.Enabled = false;
            btnLimparFiltros.Enabled = false;
            lblProgresso.Text = "Aplicando filtros...";
            Cursor = Cursors.WaitCursor;

            try
            {
                var resultado = await Task.Run(() =>
                {
                    IEnumerable<FaturasProdutoViewDto> origem = separarProdutos
                        ? _cacheProdutos
                        : AgruparPorFatura(_cacheProdutos);
                    return FiltrarCache(origem, filtros, operadores, somenteDuplicadas);
                });
                _produtosFiltrados = resultado;
                _paginaAtual = 1;
                ExibirPaginaAtual();
                lblProgresso.Text = $"{resultado.Count:N0} linhas encontradas";
            }
            finally
            {
                Cursor = Cursors.Default;
                btnFiltrar.Enabled = true;
                btnLimparFiltros.Enabled = true;
            }
        }

        private Dictionary<string, string> CapturarOperadores() =>
            _operadoresFiltro.ToDictionary(
                x => x.Key.Name,
                x => x.Value.SelectedItem?.ToString() ?? "Igual a");

        private List<FaturasProdutoViewDto> FiltrarCache(
            IEnumerable<FaturasProdutoViewDto> origem,
            FiltrosFaturasDto filtros,
            IReadOnlyDictionary<string, string> operadores,
            bool somenteDuplicadas)
        {
            IEnumerable<FaturasProdutoViewDto> query = origem;

            query = AplicarFiltroExato(query, filtros.Instalacao, x => x.Instalacao, txbInstalacao.Name, operadores);
            query = AplicarFiltroExato(query, filtros.MesReferencia, x => x.MesReferencia, txbMesRef.Name, operadores);
            query = AplicarFiltroExato(query, filtros.Distribuidora, x => x.Distribuidora, txbDistribuidora.Name, operadores);
            query = AplicarFiltroExato(query, filtros.ModelosFw, x => x.ModeloFattureWeb, txbModeloFw.Name, operadores);
            query = AplicarFiltroExato(query, filtros.ClassesConsumo, x => x.ClasseConsumo, txbClasseConsumo.Name, operadores);
            query = AplicarFiltroExato(query, filtros.Subgrupos, x => x.Subgrupo, txbSubgrupo.Name, operadores);
            query = AplicarFiltroParcial(query, filtros.DescricaoProdutos, x => x.Descricao, txbDescricaoProdutos.Name, operadores);
            query = AplicarFiltroParcial(query, filtros.DescricoesOriginais, x => x.DescricoesOriginais, txbDescricoesOriginais.Name, operadores);

            var resultado = query.ToList();
            if (!somenteDuplicadas)
                return resultado;

            var idsDuplicados = resultado
                .GroupBy(x => new { x.MesReferencia, x.Instalacao, x.Distribuidora })
                .Where(g => g.Select(x => x.FaturaId).Distinct().Count() > 1)
                .SelectMany(g => g.Select(x => x.FaturaId))
                .ToHashSet();
            return resultado.Where(x => idsDuplicados.Contains(x.FaturaId)).ToList();
        }

        private static IEnumerable<FaturasProdutoViewDto> AplicarFiltroExato(
            IEnumerable<FaturasProdutoViewDto> origem,
            List<string>? filtros,
            Func<FaturasProdutoViewDto, string?> valor,
            string nomeControle,
            IReadOnlyDictionary<string, string> operadores)
        {
            if (filtros?.Count > 0 != true)
                return origem;
            string operador = operadores.GetValueOrDefault(nomeControle) ?? "Igual a";
            bool usarContem = operador is "Contém" or "Não contém";
            bool negar = operador is "Diferente de" or "Não contém";
            return origem.Where(item =>
            {
                string valorItem = valor(item) ?? string.Empty;
                bool corresponde = filtros.Any(f => usarContem
                    ? valorItem.Contains(f, StringComparison.OrdinalIgnoreCase)
                    : string.Equals(f, valorItem, StringComparison.OrdinalIgnoreCase));
                return negar ? !corresponde : corresponde;
            });
        }

        private static IEnumerable<FaturasProdutoViewDto> AplicarFiltroParcial(
            IEnumerable<FaturasProdutoViewDto> origem,
            List<string>? filtros,
            Func<FaturasProdutoViewDto, string?> valor,
            string nomeControle,
            IReadOnlyDictionary<string, string> operadores)
        {
            return AplicarFiltroExato(origem, filtros, valor, nomeControle, operadores);
        }

        private static List<FaturasProdutoViewDto> AgruparPorFatura(
            IEnumerable<FaturasProdutoViewDto> produtos)
        {
            return produtos
                .GroupBy(x => x.FaturaId)
                .Select(g =>
                {
                    var primeiro = g.First();
                    return new FaturasProdutoViewDto
                    {
                        FaturaId = primeiro.FaturaId,
                        Instalacao = primeiro.Instalacao,
                        MesReferencia = primeiro.MesReferencia,
                        Distribuidora = primeiro.Distribuidora,
                        ModeloFattureWeb = primeiro.ModeloFattureWeb,
                        ClasseConsumo = primeiro.ClasseConsumo,
                        Subgrupo = primeiro.Subgrupo,
                        IdInstalacao = primeiro.IdInstalacao,
                        DataEmissao = primeiro.DataEmissao,
                        DataProcessamento = primeiro.DataProcessamento,
                        Descricao = string.Join(" | ", g.Select(x => x.Descricao)
                            .Where(x => !string.IsNullOrWhiteSpace(x)).Distinct()),
                        Quantidade = SomarSeExistir(g.Select(x => x.Quantidade)),
                        ValorTotal = SomarSeExistir(g.Select(x => x.ValorTotal)),
                        ValorSemImpostos = SomarSeExistir(g.Select(x => x.ValorSemImpostos)),
                        TarifaComImpostos = null,
                        TarifaSemImpostos = null,
                        TaxaDesconto = string.Empty,
                        DescricoesOriginais = string.Join(" | ", g.Select(x => x.DescricoesOriginais)
                            .Where(x => !string.IsNullOrWhiteSpace(x)).Distinct())
                    };
                })
                .ToList();
        }

        private static double? SomarSeExistir(IEnumerable<double?> valores)
        {
            var lista = valores.Where(x => x.HasValue).Select(x => x!.Value).ToList();
            return lista.Count == 0 ? null : lista.Sum();
        }

        private void ConstruirCacheProdutos()
        {
            _cacheProdutos = ConverterDadosEmProdutos(_dados);
        }

        private static List<FaturasProdutoViewDto> ConverterDadosEmProdutos(IEnumerable<Dado> dados)
        {
            return dados
                .Where(d => d?.Conteudo?.Fatura?.Produtos != null)
                .SelectMany(d =>
                {
                    var conteudo = d.Conteudo;
                    var fatura = conteudo.Fatura;
                    string mesReferencia = DateTime.TryParse(fatura.MesReferencia, out var data)
                        ? data.ToString("MMyyyy")
                        : fatura.MesReferencia ?? string.Empty;

                    return fatura.Produtos.Where(p => p != null).Select(produto => new FaturasProdutoViewDto
                    {
                        FaturaId = conteudo.FaturaId?.ToString() ?? string.Empty,
                        Instalacao = conteudo.UnidadeConsumidora?.Instalacao ?? string.Empty,
                        MesReferencia = mesReferencia,
                        Distribuidora = conteudo.Distribuidora ?? string.Empty,
                        ModeloFattureWeb = conteudo.ModeloFatura ?? string.Empty,
                        ClasseConsumo = conteudo.Outros?.ClasseConsumo ?? string.Empty,
                        Subgrupo = conteudo.UnidadeConsumidora?.Subgrupo ?? string.Empty,
                        IdInstalacao = d.InstalacaoId.ToString(),
                        DataEmissao = fatura.DataEmissao ?? string.Empty,
                        DataProcessamento = d.DataProcessamento.ToString(),
                        Descricao = produto.Descricao,
                        Quantidade = produto.Quantidade,
                        ValorTotal = produto.ValorTotal,
                        ValorSemImpostos = produto.ValorSemImpostos,
                        TarifaComImpostos = produto.TarifaComImpostos,
                        TarifaSemImpostos = produto.TarifaSemImpostos,
                        TaxaDesconto = produto.TaxaDesconto?.ToString(),
                        DescricoesOriginais = produto.DescricoesOriginais != null
                            ? string.Join(";", produto.DescricoesOriginais)
                            : string.Empty
                    });
                })
                .ToList();
        }

        private void ExibirPaginaAtual()
        {
            int totalPaginas = Math.Max(1, (int)Math.Ceiling(_produtosFiltrados.Count / (double)TamanhoPaginaGrid));
            _paginaAtual = Math.Clamp(_paginaAtual, 1, totalPaginas);
            var pagina = _produtosFiltrados
                .Skip((_paginaAtual - 1) * TamanhoPaginaGrid)
                .Take(TamanhoPaginaGrid)
                .ToList();

            gridFaturas.SuspendLayout();
            gridFaturas.DataSource = null;
            gridFaturas.DataSource = pagina;
            ConfigurarColunasGrid();
            AplicarEstiloGrid();
            gridFaturas.ResumeLayout();
            _estadoVazioLabel.Visible = pagina.Count == 0;
            if (_estadoVazioLabel.Visible)
                _estadoVazioLabel.BringToFront();
            else
                gridFaturas.BringToFront();
            lblPagina.Text = $"Página {_paginaAtual}/{totalPaginas}";
            btnPaginaAnterior.Enabled = _paginaAtual > 1;
            btnProximaPagina.Enabled = _paginaAtual < totalPaginas;
            AtualizarTotais();
        }

        private void ConfigurarColunasGrid()
        {
            var colunaDescricao = gridFaturas.Columns[nameof(FaturasProdutoViewDto.Descricao)];
            var colunaDescricoesOriginais =
                gridFaturas.Columns[nameof(FaturasProdutoViewDto.DescricoesOriginais)];

            if (colunaDescricao == null || colunaDescricoesOriginais == null)
                return;

            colunaDescricao.HeaderText = "Descrição dos produtos";
            colunaDescricao.AutoSizeMode = DataGridViewAutoSizeColumnMode.None;
            colunaDescricao.MinimumWidth = 160;
            colunaDescricao.Width = Math.Max(colunaDescricao.Width, 220);
            colunaDescricao.Resizable = DataGridViewTriState.True;

            colunaDescricoesOriginais.HeaderText = "Descrições originais";
            colunaDescricoesOriginais.AutoSizeMode = DataGridViewAutoSizeColumnMode.None;
            colunaDescricoesOriginais.MinimumWidth = 160;
            colunaDescricoesOriginais.Width = Math.Max(colunaDescricoesOriginais.Width, 240);
            colunaDescricoesOriginais.Resizable = DataGridViewTriState.True;
            colunaDescricoesOriginais.DisplayIndex = colunaDescricao.DisplayIndex + 1;
        }

        // ObtemDadosFatura removido: informações dos produtos agora são tratadas diretamente em PreencherGridFaturas.

        private List<Dado> FiltrarDados()
        {
            IEnumerable<Dado> query = _dados.Where(x =>
                x != null &&
                x.Conteudo != null &&
                x.Conteudo.Fatura != null);

            if (!string.IsNullOrEmpty(_filtros.FaturaId))
            {
                query = query.Where(x => x.Conteudo.FaturaId.ToString() == _filtros.FaturaId);
            }

            if (_filtros.Instalacao != null && _filtros.Instalacao.Any())
            {
                query = query.Where(x => AplicarOperador(
                    txbInstalacao,
                    _filtros.Instalacao.Any(o =>
                        string.Equals(o, x.Conteudo.UnidadeConsumidora?.Instalacao, StringComparison.OrdinalIgnoreCase))));
            }

            if (_filtros.MesReferencia != null && _filtros.MesReferencia.Any())
            {
                query = query.Where(x =>
                {
                    string mes = DateTime.TryParse(x.Conteudo.Fatura.MesReferencia, out var data)
                        ? data.ToString("MMyyyy")
                        : x.Conteudo.Fatura.MesReferencia ?? string.Empty;
                    return AplicarOperador(txbMesRef,
                        _filtros.MesReferencia.Any(f => string.Equals(f, mes, StringComparison.OrdinalIgnoreCase)));
                });
            }

            if (_filtros.Distribuidora != null && _filtros.Distribuidora.Any())
            {
                query = query.Where(x => AplicarOperador(
                    txbDistribuidora,
                    _filtros.Distribuidora.Any(f =>
                        string.Equals(f, x.Conteudo.Distribuidora, StringComparison.OrdinalIgnoreCase))));
            }

            if (_filtros.ClassesConsumo != null && _filtros.ClassesConsumo.Any())
            {
                query = query.Where(x => AplicarOperador(
                    txbClasseConsumo,
                    x.Conteudo.Outros != null &&
                    _filtros.ClassesConsumo.Any(filtro =>
                        string.Equals(filtro, x.Conteudo.Outros.ClasseConsumo, StringComparison.OrdinalIgnoreCase))));
            }

            if (_filtros.Subgrupos != null && _filtros.Subgrupos.Any())
            {
                query = query.Where(x => AplicarOperador(
                    txbSubgrupo,
                    x.Conteudo.UnidadeConsumidora != null &&
                    _filtros.Subgrupos.Any(filtro =>
                        string.Equals(filtro, x.Conteudo.UnidadeConsumidora.Subgrupo, StringComparison.OrdinalIgnoreCase))));
            }

            if (!string.IsNullOrEmpty(_filtros.IdInstalacao))
            {
                query = query.Where(x => x.InstalacaoId.ToString() == _filtros.IdInstalacao);
            }

            if (!string.IsNullOrEmpty(_filtros.ConsumoTotal))
            {
                query = query.Where(x => (x.Conteudo.Fatura.HistoricoFaturamento != null ? x.Conteudo.Fatura.HistoricoFaturamento.FirstOrDefault().EnergiaAtiva : 0).ToString() == _filtros.ConsumoTotal);
            }

            if (_filtros.DataEmissao.HasValue)
            {
                query = query.Where(x => DateTime.Parse(x.Conteudo.Fatura.DataEmissao) == _filtros.DataEmissao.Value);
            }

            if (_filtros.ModelosFw != null && _filtros.ModelosFw.Any())
            {
                query = query.Where(x => AplicarOperador(
                    txbModeloFw,
                    _filtros.ModelosFw.Any(m =>
                        string.Equals(m, x.Conteudo.ModeloFatura, StringComparison.OrdinalIgnoreCase))));
            }

            if (chbFaturasDuplicadas.Checked)
            {
                query = query
                    .GroupBy(x => new
                    {
                        x.Conteudo.Fatura.MesReferencia,
                        x.Conteudo.UnidadeConsumidora.Instalacao,
                        x.Conteudo.Distribuidora
                    })
                    .Where(g => g.Count() > 1)
                    .SelectMany(g => g);
            }

            return query.ToList();
        }

        public void CarregarDados()
        {
            _token = realizarLogin();
            if (!string.IsNullOrEmpty(_token))
            {
                var objeto = GetFaturasPaginadoWithLogAsync();
                if (objeto != null)
                {
                    _dados = objeto.Dados.ToList();
                }
            }
        }

        public string realizarLogin()
        {
            var clientToken = new HttpClient();
            var requestToken = new HttpRequestMessage(HttpMethod.Post, $"{URL_FATTUREWEB}auth/login");
            var jsonRequestToken = new JObject
            {
                { "email", txtUsuario.Text },
                { "senha", txtSenha.Text }
            };
            var contentToken = new StringContent(jsonRequestToken.ToString(), null, "application/json");
            requestToken.Content = contentToken;
            var response = clientToken.Send(requestToken);
            //response.EnsureSuccessStatusCode();
            var responseBody = response.Content.ReadAsStringAsync().Result;
            var jsonResponse = JObject.Parse(responseBody);

            if (response.StatusCode != System.Net.HttpStatusCode.OK)
            {
                MessageBox.Show("Não foi possível fazer autenticação com esse usuário/senha.", "Falha login", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return "";
            }
            else
            {
                var dados = jsonResponse["dados"];
                if (dados != null && dados.Any())
                {
                    var primeiroDado = dados.FirstOrDefault();
                    if (primeiroDado != null)
                    {
                        var token = primeiroDado["token"];
                        if (token != null)
                            return token.ToString();
                    }
                }
                return "";
            }
        }

        public Root? GetFaturasPaginadoWithLogAsync()
        {
            Root retorno = new Root("", "", new List<Dado>());
            const int tamanhoMaximoPagina = 1000;
            int skip = 0;
            bool continuar = true;
            object locker = new object();

            List<Task> tasks = new List<Task>();

            List<(int, string)> logEntries = new List<(int, string)>();

            Stopwatch totalStopwatch = Stopwatch.StartNew();

            logEntries.Add((0, $"Início do processo {DateTime.Now}"));

            while (continuar)
            {
                var localSkip = skip;
                skip += tamanhoMaximoPagina;

                Task task = Task.Run(() =>
                {
                    Stopwatch taskStopwatch = Stopwatch.StartNew();

                    try
                    {
                        string content = GetJsonFaturas(tamanhoMaximoPagina, localSkip);
                        var result = JsonConvert.DeserializeObject<Root>(content);

                        lock (locker)
                        {
                            logEntries.Add((
                                localSkip / tamanhoMaximoPagina + 1,
                                $"Requisição {localSkip / tamanhoMaximoPagina + 1} - Página: {localSkip / tamanhoMaximoPagina + 1} - Tempo: {taskStopwatch.ElapsedMilliseconds / 1000.0:F2}s - Sucesso"
                            ));
                        }

                        if (result?.Dados != null && result.Dados.Any())
                        {
                            lock (locker)
                            {
                                retorno.Dados.AddRange(result.Dados);
                            }

                            if (result.Dados.Count < tamanhoMaximoPagina)
                                continuar = false;
                        }
                        else
                        {
                            continuar = false;
                        }
                    }
                    catch (Exception ex)
                    {
                        lock (locker)
                        {
                            logEntries.Add((
                                localSkip / tamanhoMaximoPagina + 1,
                                $"Requisição {localSkip / tamanhoMaximoPagina + 1} - Página: {localSkip / tamanhoMaximoPagina + 1} - Erro: {ex.Message}"
                            ));
                        }
                        continuar = false;
                    }

                    taskStopwatch.Stop();
                });

                tasks.Add(task);

                if (tasks.Count >= 80)
                {
                    Task.WaitAll(tasks.ToArray());
                    tasks.Clear();
                }
            }

            Task.WaitAll(tasks.ToArray());

            totalStopwatch.Stop();
            logEntries.Add((logEntries.Count + 1, $"Processo concluído. Tempo total: {totalStopwatch.ElapsedMilliseconds / 1000.0:F2}s"));

            var sortedLogs = logEntries.OrderBy(entry => entry.Item1).ToList();

            if (_gravarLog)
            {
                string logFilePath = Path.Combine("logs", $"log{DateTime.Now:yyyyMMdd_HHmmss}.txt");

                if (!Directory.Exists("logs"))
                {
                    Directory.CreateDirectory("logs");
                }
                using (StreamWriter logFile = new StreamWriter(logFilePath, true))
                {
                    foreach (var log in sortedLogs)
                    {
                        logFile.WriteLine(log.Item2);
                    }
                }
            }

            return retorno;
        }

        private string GetJsonFaturas(int limit = 1000, int skip = 0, string idFatura = "")
        {
            using (HttpClient clientFaturas = new HttpClient())
            {
                clientFaturas.Timeout = Timeout.InfiniteTimeSpan;
                string url = $"{URL_FATTUREWEB}faturas?limit={limit}&skip={skip}";
                if (!string.IsNullOrEmpty(idFatura))
                {
                    url += $"&id={idFatura}";
                }

                var requestFaturas = new HttpRequestMessage(HttpMethod.Get, url);
                requestFaturas.Headers.Add("Fatture-AuthToken", _token);
                requestFaturas.Headers.Add(
                    "Fatture-SearchFields",
                    "id, instalacao_id, arquivo_id, status_fatura_id, status, data_criacao, data_atualizacao, processamento_id, usuario_id, email_fatura_id, data_processamento, erro_processamento, mes_referencia, data_vencimento, valor_total, conteudo"
                );

                var responseFaturas = clientFaturas.Send(requestFaturas);
                responseFaturas.EnsureSuccessStatusCode();

                var contentResponseFaturas = responseFaturas.Content.ReadAsStringAsync().Result;
                return contentResponseFaturas;
            }
        }

        private async Task CarregarDadosAsync()
        {
            _carregamentoCts?.Cancel();
            _carregamentoCts = new CancellationTokenSource();
            var cancellationToken = _carregamentoCts.Token;
            btnBuscarFaturas.Enabled = false;
            btnCancelarBusca.Visible = true;
            lblProgresso.Text = "Autenticando...";
            Cursor = Cursors.WaitCursor;
            var tempoTotal = Stopwatch.StartNew();

            try
            {
                _token = await RealizarLoginOtimizadoAsync(txtUsuario.Text, txtSenha.Text, cancellationToken);
                if (string.IsNullOrEmpty(_token))
                {
                    MessageBox.Show("Não foi possível autenticar com esse usuário/senha.", "Falha no login",
                        MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                _usuarioCacheAtual = NormalizarUsuarioCache(txtUsuario.Text);
                string arquivoCache = ObterArquivoCache(_usuarioCacheAtual);

                if (File.Exists(arquivoCache))
                {
                    DateTime atualizadoEm = File.GetLastWriteTime(arquivoCache);
                    var escolha = MessageBox.Show(
                        $"Existe um cache local atualizado em {atualizadoEm:dd/MM/yyyy HH:mm}.\n\n" +
                        "Sim: abrir o cache agora\nNão: atualizar todos os dados pela API\nCancelar: interromper",
                        "Cache do FattureWeb",
                        MessageBoxButtons.YesNoCancel,
                        MessageBoxIcon.Question);

                    if (escolha == DialogResult.Cancel)
                        return;

                    if (escolha == DialogResult.Yes)
                    {
                        lblProgresso.Text = "Carregando cache local...";
                        var cache = await Task.Run(
                            () => LerCache(arquivoCache, _usuarioCacheAtual, cancellationToken),
                            cancellationToken);
                        _cacheProdutos = cache.Produtos;
                        _totalFaturasCarregadas = cache.TotalFaturas;
                        await AtualizarVisualizacaoAsync();
                        tempoTotal.Stop();
                        lblProgresso.Text =
                            $"{_totalFaturasCarregadas:N0} faturas carregadas do cache em " +
                            $"{tempoTotal.Elapsed.TotalSeconds:F1}s";
                        return;
                    }
                }

                var progresso = new Progress<string>(texto => lblProgresso.Text = texto);
                var tempoDownload = Stopwatch.StartNew();
                var resultado = await BuscarFaturasOtimizadoAsync(progresso, cancellationToken);
                tempoDownload.Stop();
                _cacheProdutos = resultado.Produtos;
                _totalFaturasCarregadas = resultado.TotalFaturas;
                _dados.Clear();
                lblProgresso.Text = "Salvando cache local...";
                var tempoPreparacao = Stopwatch.StartNew();
                await Task.Run(
                    () => SalvarCache(arquivoCache, _usuarioCacheAtual, cancellationToken),
                    cancellationToken);
                tempoPreparacao.Stop();
                await AtualizarVisualizacaoAsync();
                tempoTotal.Stop();
                lblProgresso.Text =
                    $"{_totalFaturasCarregadas:N0} faturas em {tempoTotal.Elapsed.TotalSeconds:F1}s " +
                    $"(API {tempoDownload.Elapsed.TotalSeconds:F1}s / cache {tempoPreparacao.Elapsed.TotalSeconds:F1}s)";
            }
            catch (OperationCanceledException)
            {
                lblProgresso.Text = "Busca cancelada";
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Não foi possível carregar as faturas.\n{ex.Message}", "Erro",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
                lblProgresso.Text = "Falha no carregamento";
            }
            finally
            {
                Cursor = Cursors.Default;
                btnBuscarFaturas.Enabled = true;
                btnCancelarBusca.Visible = false;
            }
        }

        private async Task<string> RealizarLoginOtimizadoAsync(
            string usuario,
            string senha,
            CancellationToken cancellationToken)
        {
            using var request = new HttpRequestMessage(HttpMethod.Post, $"{URL_FATTUREWEB}auth/login");
            request.Content = new StringContent(
                new JObject { { "email", usuario }, { "senha", senha } }.ToString(),
                null,
                "application/json");
            using var response = await HttpClient.SendAsync(request, cancellationToken);
            string body = await response.Content.ReadAsStringAsync(cancellationToken);
            if (!response.IsSuccessStatusCode)
                return string.Empty;
            return JObject.Parse(body)["dados"]?.FirstOrDefault()?["token"]?.ToString() ?? string.Empty;
        }

        private static string NormalizarUsuarioCache(string usuario) =>
            usuario.Trim().ToLowerInvariant();

        private static string ObterArquivoCache(string usuarioNormalizado)
        {
            byte[] hash = SHA256.HashData(Encoding.UTF8.GetBytes(usuarioNormalizado));
            string identificador = Convert.ToHexString(hash)[..20].ToLowerInvariant();
            return Path.Combine(DiretorioCache, $"faturas-cache-v2-{identificador}.json");
        }

        private void SalvarCache(
            string arquivoCache,
            string usuarioNormalizado,
            CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();
            Directory.CreateDirectory(DiretorioCache);
            string arquivoTemporario = arquivoCache + ".tmp";
            var cache = new CacheFattureWeb
            {
                Usuario = usuarioNormalizado,
                CriadoEm = DateTime.Now,
                TotalFaturas = _totalFaturasCarregadas,
                Produtos = _cacheProdutos
            };

            using (var stream = new FileStream(
                arquivoTemporario,
                FileMode.Create,
                FileAccess.Write,
                FileShare.None,
                1024 * 1024))
            using (var writer = new StreamWriter(stream))
            using (var jsonWriter = new JsonTextWriter(writer))
            {
                var serializer = Newtonsoft.Json.JsonSerializer.CreateDefault();
                serializer.Serialize(jsonWriter, cache);
                jsonWriter.Flush();
            }

            cancellationToken.ThrowIfCancellationRequested();
            File.Move(arquivoTemporario, arquivoCache, true);
        }

        private static CacheFattureWeb LerCache(
            string arquivoCache,
            string usuarioNormalizado,
            CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();
            using var stream = new FileStream(
                arquivoCache,
                FileMode.Open,
                FileAccess.Read,
                FileShare.Read,
                1024 * 1024);
            using var reader = new StreamReader(stream);
            using var jsonReader = new JsonTextReader(reader);
            var serializer = Newtonsoft.Json.JsonSerializer.CreateDefault();
            return serializer.Deserialize<CacheFattureWeb>(jsonReader)
                ?? throw new InvalidDataException("O cache local está vazio ou inválido.");
        }

        private sealed class CacheFattureWeb
        {
            public string Usuario { get; set; } = string.Empty;
            public DateTime CriadoEm { get; set; }
            public int TotalFaturas { get; set; }
            public List<FaturasProdutoViewDto> Produtos { get; set; } = new();
        }

        private async Task<(List<FaturasProdutoViewDto> Produtos, int TotalFaturas)> BuscarFaturasOtimizadoAsync(
            IProgress<string> progresso,
            CancellationToken cancellationToken)
        {
            const int limite = 1000;
            const int concorrencia = 12;
            int skip = 0;
            var produtos = new List<FaturasProdutoViewDto>();
            int totalFaturas = 0;
            bool continuar;

            do
            {
                var inicioLote = Stopwatch.StartNew();
                var skips = Enumerable.Range(0, concorrencia).Select(i => skip + i * limite).ToArray();
                var tarefas = skips.Select(localSkip =>
                    BuscarPaginaComRetryAsync(limite, localSkip, cancellationToken));
                var paginas = await Task.WhenAll(tarefas);
                foreach (var pagina in paginas)
                {
                    totalFaturas += pagina.Dados.Count;
                    produtos.AddRange(ConverterDadosEmProdutos(pagina.Dados));
                    pagina.Dados.Clear();
                }

                progresso.Report(
                    $"{totalFaturas:N0} faturas recebidas — último lote em {inicioLote.Elapsed.TotalSeconds:F1}s");
                continuar = paginas.All(p => p.Dados.Count == limite);
                skip += concorrencia * limite;
            }
            while (continuar);

            return (produtos, totalFaturas);
        }

        private async Task<Root> BuscarPaginaComRetryAsync(
            int limite,
            int skip,
            CancellationToken cancellationToken)
        {
            const int maximoTentativas = 4;
            Exception? ultimoErro = null;

            for (int tentativa = 1; tentativa <= maximoTentativas; tentativa++)
            {
                try
                {
                    return await BuscarPaginaAsync(limite, skip, cancellationToken);
                }
                catch (Exception ex) when (
                    ex is HttpRequestException or IOException or TaskCanceledException)
                {
                    cancellationToken.ThrowIfCancellationRequested();
                    ultimoErro = ex;
                    if (tentativa < maximoTentativas)
                        await Task.Delay(TimeSpan.FromSeconds(tentativa * 2), cancellationToken);
                }
            }

            throw new HttpRequestException(
                $"Não foi possível baixar a página {skip / limite + 1} após {maximoTentativas} tentativas.",
                ultimoErro);
        }

        private async Task<Root> BuscarPaginaAsync(int limite, int skip, CancellationToken cancellationToken)
        {
            using var request = CriarRequestFaturas(limite, skip);
            using var response = await HttpClient.SendAsync(
                request,
                HttpCompletionOption.ResponseHeadersRead,
                cancellationToken);

            // O FattureWeb responde 404 quando o skip ultrapassa a última página.
            // Nesse contexto, 404 representa fim da paginação, não falha da busca.
            if (response.StatusCode == System.Net.HttpStatusCode.NotFound)
            {
                return new Root(string.Empty, string.Empty, new List<Dado>());
            }

            response.EnsureSuccessStatusCode();
            string body = await response.Content.ReadAsStringAsync(cancellationToken);
            return JsonConvert.DeserializeObject<Root>(body)
                ?? new Root(string.Empty, string.Empty, new List<Dado>());
        }

        private HttpRequestMessage CriarRequestFaturas(int limite, int skip)
        {
            var request = new HttpRequestMessage(
                HttpMethod.Get,
                $"{URL_FATTUREWEB}faturas?limit={limite}&skip={skip}");
            request.Headers.Add("Fatture-AuthToken", _token);
            request.Headers.Add(
                "Fatture-SearchFields",
                "id, instalacao_id, arquivo_id, status_fatura_id, status, data_criacao, data_atualizacao, processamento_id, usuario_id, email_fatura_id, data_processamento, erro_processamento, mes_referencia, data_vencimento, valor_total, conteudo");
            return request;
        }

        private void btnCancelarBusca_Click(object sender, EventArgs e) => _carregamentoCts?.Cancel();

        private void btnPaginaAnterior_Click(object sender, EventArgs e)
        {
            _paginaAtual--;
            ExibirPaginaAtual();
        }

        private void btnProximaPagina_Click(object sender, EventArgs e)
        {
            _paginaAtual++;
            ExibirPaginaAtual();
        }

        private void btnDownloadCsv_Click(object sender, EventArgs e)
        {
            try
            {
                if (_produtosFiltrados.Count > 0)
                {
                    StartLoading();
                    gridFaturas.ExportToXls("faturas", Program.OutputDir, _produtosFiltrados);
                }
                else
                {
                    MessageBox.Show("Não há dados para serem exportados.", "Alerta", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Erro ao exportar arquivo. {ex.Message}", "Erro", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            finally
            {
                StopLoading();
            }
        }

        private async void btnFiltrar_Click(object sender, EventArgs e)
        {
            try
            {
                _filtros = new FiltrosFaturasDto();
                if (!string.IsNullOrEmpty(txbDistribuidora.Text))
                    _filtros.Distribuidora = SepararFiltros(txbDistribuidora.Text);

                if (!string.IsNullOrEmpty(txbMesRef.Text))
                    _filtros.MesReferencia = SepararFiltros(txbMesRef.Text);

                if (!string.IsNullOrEmpty(txbInstalacao.Text))
                    _filtros.Instalacao = SepararFiltros(txbInstalacao.Text);

                if (!string.IsNullOrEmpty(txbDescricaoProdutos.Text))
                    _filtros.DescricaoProdutos = SepararFiltros(txbDescricaoProdutos.Text);

                if (!string.IsNullOrEmpty(txbDescricoesOriginais.Text))
                    _filtros.DescricoesOriginais = SepararFiltros(txbDescricoesOriginais.Text);

                if (!string.IsNullOrEmpty(txbModeloFw.Text))
                    _filtros.ModelosFw = SepararFiltros(txbModeloFw.Text);

                if (!string.IsNullOrEmpty(txbClasseConsumo.Text))
                    _filtros.ClassesConsumo = SepararFiltros(txbClasseConsumo.Text);

                if (!string.IsNullOrEmpty(txbSubgrupo.Text))
                    _filtros.Subgrupos = SepararFiltros(txbSubgrupo.Text);

                await AtualizarVisualizacaoAsync();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Erro ao filtrar faturas. \n{ex.Message}", "Erro", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void btnBuscarFaturas_Click(object sender, EventArgs e)
        {
            if (_dados == null || !_dados.Any() ||
                MessageBox.Show("Essa ação irá buscar todas as faturas no FattureWeb. \nQuer continuar?", "Deseja realizar uma nova busca?", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes
                )
            {
                IniciaBackground();
            }
        }

        private async void btnLimparFiltros_Click(object sender, EventArgs e)
        {
            await LimparFiltrosAsync();
        }

        private async Task LimparFiltrosAsync()
        {
            _filtros = new FiltrosFaturasDto();
            chbFaturasDuplicadas.Checked = false;
            txbDistribuidora.Text = string.Empty;
            txbDescricaoProdutos.Text = string.Empty;
            txbDescricoesOriginais.Text = string.Empty;
            txbInstalacao.Text = string.Empty;
            txbMesRef.Text = string.Empty;
            txbClasseConsumo.Text = string.Empty;
            txbSubgrupo.Text = string.Empty;
            foreach (var operador in _operadoresFiltro.Values)
                operador.SelectedIndex = 0;

            await AtualizarVisualizacaoAsync();
        }

        private static List<string> SepararFiltros(string texto) =>
            texto.Split(';', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries).ToList();

        private void gridFaturas_DataSourceChanged(object sender, EventArgs e)
        {
            AtualizarTotais();
        }

        private void AtualizarTotais()
        {
            txtQtdFaturas.Text = _totalFaturasCarregadas.ToString();
            txtQtdFaturasFiltradas.Text = _produtosFiltrados.Count.ToString();
        }

        private void btnFiltros_Click(object sender, EventArgs e)
        {
            if (btnFiltros.Text.Contains(">>"))
            {
                btnFiltros.Text = "Filtros <<";
                pnlFiltros.Visible = false;
            }
            else
            {
                btnFiltros.Text = "Filtros >>";
                pnlFiltros.Visible = true;
            }

        }

        private sealed class DarkComboBox : ComboBox
        {
            public bool DarkMode { get; set; }

            protected override void WndProc(ref Message m)
            {
                base.WndProc(ref m);
                if (!DarkMode || m.Msg != 0x000F || DropDownStyle == ComboBoxStyle.Simple)
                    return;

                using var graphics = Graphics.FromHwnd(Handle);
                var areaSeta = new Rectangle(ClientSize.Width - 22, 1, 21, ClientSize.Height - 2);
                using var fundo = new SolidBrush(DarkSurfaceRaised);
                using var borda = new Pen(DarkBorder);
                using var seta = new SolidBrush(DarkText);
                graphics.FillRectangle(fundo, areaSeta);
                graphics.DrawLine(borda, areaSeta.Left, areaSeta.Top,
                    areaSeta.Left, areaSeta.Bottom);

                int centroX = areaSeta.Left + areaSeta.Width / 2;
                int centroY = areaSeta.Top + areaSeta.Height / 2;
                graphics.FillPolygon(seta,
                new Point[]
                {
                    new Point(centroX - 4, centroY - 2),
                    new Point(centroX + 4, centroY - 2),
                    new Point(centroX, centroY + 3)
                });
                graphics.DrawRectangle(borda, 0, 0, ClientSize.Width - 1, ClientSize.Height - 1);
            }
        }

        private sealed class DarkScrollBar : Control
        {
            private readonly Orientation _orientacao;
            private bool _arrastando;
            private int _inicioArraste;
            private int _valorInicioArraste;
            private int _value;

            public bool DarkMode { get; set; } = true;
            public int Maximum { get; private set; }
            public int LargeChange { get; private set; } = 1;
            public event EventHandler? ValueChanged;

            public int Value
            {
                get => _value;
                set
                {
                    int novo = Math.Clamp(value, 0, Maximum);
                    if (_value == novo)
                        return;
                    _value = novo;
                    Invalidate();
                    ValueChanged?.Invoke(this, EventArgs.Empty);
                }
            }

            public DarkScrollBar(Orientation orientacao)
            {
                _orientacao = orientacao;
                SetStyle(ControlStyles.AllPaintingInWmPaint |
                    ControlStyles.OptimizedDoubleBuffer |
                    ControlStyles.UserPaint, true);
                Cursor = Cursors.Hand;
            }

            public void Configure(int maximum, int largeChange)
            {
                Maximum = Math.Max(0, maximum);
                LargeChange = Math.Max(1, largeChange);
                _value = Math.Clamp(_value, 0, Maximum);
                Invalidate();
            }

            protected override void OnPaint(PaintEventArgs e)
            {
                base.OnPaint(e);
                e.Graphics.Clear(DarkBackground);
                Rectangle trilho = ClientRectangle;
                trilho.Inflate(-2, -2);
                using var fundoTrilho = new SolidBrush(DarkSurfaceRaised);
                using var indicador = new SolidBrush(DarkMode ? DarkBorder : SystemColors.ScrollBar);
                e.Graphics.FillRectangle(fundoTrilho, trilho);
                e.Graphics.FillRectangle(indicador, ObterIndicador());
            }

            protected override void OnMouseDown(MouseEventArgs e)
            {
                base.OnMouseDown(e);
                Rectangle indicador = ObterIndicador();
                int posicao = _orientacao == Orientation.Horizontal ? e.X : e.Y;
                if (indicador.Contains(e.Location))
                {
                    _arrastando = true;
                    _inicioArraste = posicao;
                    _valorInicioArraste = Value;
                    Capture = true;
                }
                else
                {
                    int inicio = _orientacao == Orientation.Horizontal
                        ? indicador.Left : indicador.Top;
                    Value += posicao < inicio ? -LargeChange : LargeChange;
                }
            }

            protected override void OnMouseMove(MouseEventArgs e)
            {
                base.OnMouseMove(e);
                if (!_arrastando || Maximum == 0)
                    return;
                int comprimento = _orientacao == Orientation.Horizontal ? Width : Height;
                int tamanhoIndicador = _orientacao == Orientation.Horizontal
                    ? ObterIndicador().Width : ObterIndicador().Height;
                int faixa = Math.Max(1, comprimento - tamanhoIndicador - 4);
                int posicao = _orientacao == Orientation.Horizontal ? e.X : e.Y;
                Value = _valorInicioArraste +
                    (int)Math.Round((posicao - _inicioArraste) * Maximum / (double)faixa);
            }

            protected override void OnMouseUp(MouseEventArgs e)
            {
                base.OnMouseUp(e);
                _arrastando = false;
                Capture = false;
            }

            private Rectangle ObterIndicador()
            {
                int comprimento = _orientacao == Orientation.Horizontal ? Width : Height;
                int transversal = _orientacao == Orientation.Horizontal ? Height : Width;
                int tamanho = Maximum == 0
                    ? comprimento - 4
                    : Math.Max(28, (int)((comprimento - 4) *
                        LargeChange / (double)(Maximum + LargeChange)));
                tamanho = Math.Min(comprimento - 4, tamanho);
                int faixa = Math.Max(0, comprimento - tamanho - 4);
                int inicio = 2 + (Maximum == 0 ? 0 :
                    (int)Math.Round(faixa * Value / (double)Maximum));
                return _orientacao == Orientation.Horizontal
                    ? new Rectangle(inicio, 3, tamanho, Math.Max(4, transversal - 6))
                    : new Rectangle(3, inicio, Math.Max(4, transversal - 6), tamanho);
            }
        }
    }
}
