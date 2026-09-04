using ClosedXML.Excel;
using System.Collections;
using System.Data;
using System.Linq;

namespace DTSWindowsForm.Extensions
{
    public static class DataGridViewExtensions
    {
        /// <summary>
        /// Exporta os dados de um DataGridView para um arquivo Excel (.xlsx) com colunas formatadas.
        /// Aplica estilos de cabeçalhos em verde claro com texto em negrito e preto, ajusta automaticamente
        /// a largura das colunas e adiciona filtros nos cabeçalhos.
        /// </summary>
        /// <param name="grid">DataGridView contendo os dados a serem exportados.</param>
        /// <param name="prefixFileName">Prefixo do nome do arquivo gerado. Se vazio, será utilizado "DevToolsShare".</param>
        /// <param name="outputPath">Diretório onde o arquivo será salvo. Se vazio, será salvo em uma pasta "output" no diretório do executável.</param>
        /// <returns>True se o arquivo foi exportado com sucesso, False em caso de erro.</returns>
        /// <exception cref="ArgumentException">Lançada se o DataGridView estiver vazio ou nulo.</exception>
        public static bool ExportToXls(
            this DataGridView grid,
            string prefixFileName,
            string outputPath,
            IEnumerable? dataItems = null)
        {
            try
            {
                string fileName = $"{prefixFileName}_{DateTime.Now:yyyyMMdd_HHmmss}.xlsx";
                MessageBox.Show($"O arquivo será exportado. \n{fileName}");
                if (grid == null || (grid.Rows.Count == 0 && dataItems == null))
                {
                    throw new ArgumentException("O DataGridView está vazio ou nulo.");
                }

                if (string.IsNullOrWhiteSpace(prefixFileName))
                {
                    prefixFileName = "DevToolsShare";
                }

                if (string.IsNullOrWhiteSpace(outputPath))
                {
                    outputPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "output");

                    if (!Directory.Exists(outputPath))
                    {
                        Directory.CreateDirectory(outputPath);
                    }
                }

                string fullPath = Path.Combine(outputPath, fileName);


                using (var workbook = new XLWorkbook())
                {
                    var orderedColumns = grid.Columns.Cast<DataGridViewColumn>()
                                             .Where(c => c.Visible && c is not DataGridViewButtonColumn)
                                             .OrderBy(c => c.DisplayIndex)
                                             .ToList();

                    var dataTable = new DataTable(prefixFileName);
                    foreach (var column in orderedColumns)
                    {
                        string baseName = string.IsNullOrWhiteSpace(column.HeaderText)
                            ? (!string.IsNullOrWhiteSpace(column.DataPropertyName)
                                ? column.DataPropertyName
                                : column.Name)
                            : column.HeaderText;
                        string uniqueName = baseName;
                        int suffix = 2;
                        while (dataTable.Columns.Contains(uniqueName))
                            uniqueName = $"{baseName} ({suffix++})";
                        dataTable.Columns.Add(uniqueName);
                    }

                    if (dataItems != null)
                    {
                        foreach (object item in dataItems)
                        {
                            var values = orderedColumns.Select(c =>
                                string.IsNullOrWhiteSpace(c.DataPropertyName)
                                    ? null
                                    : item.GetType().GetProperty(c.DataPropertyName)?.GetValue(item))
                                .ToArray();
                            dataTable.Rows.Add(values);
                        }
                    }
                    else
                    {
                        foreach (DataGridViewRow row in grid.Rows)
                        {
                            var values = orderedColumns.Select(c => row.Cells[c.Index].Value).ToArray();
                            dataTable.Rows.Add(values);
                        }
                    }

                    var worksheet = workbook.Worksheets.Add(dataTable, prefixFileName);
                    var headerStyle = worksheet.Range(1, 1, 1, dataTable.Columns.Count).Style;
                    headerStyle.Font.Bold = true;
                    headerStyle.Font.FontColor = XLColor.White;
                    headerStyle.Fill.BackgroundColor = XLColor.BlueGray;

                    worksheet.Columns().AdjustToContents();
                    workbook.SaveAs(fullPath);
                }

                MessageBox.Show("Exportação concluída com sucesso!", "Sucesso", MessageBoxButtons.OK, MessageBoxIcon.Information);

                DialogResult result = MessageBox.Show("Deseja abrir o diretório onde o arquivo foi salvo?", "Abrir Diretório", MessageBoxButtons.YesNo, MessageBoxIcon.Question);

                if (result == DialogResult.Yes)
                {
                    System.Diagnostics.Process.Start("explorer.exe", Path.GetDirectoryName(fullPath));
                }

                return true;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Erro ao exportar para Excel: {ex.Message}", "Erro", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return false;
            }
        }
    }
}
