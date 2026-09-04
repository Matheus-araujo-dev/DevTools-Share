namespace DTSWindowsForm.UI.FattureWeb.dtos;

public record FaturasProdutoViewDto
{
    public string? FaturaId { get; set; }
    public string? Instalacao { get; set; }
    public string? MesReferencia { get; set; }
    public string? Distribuidora { get; set; }
    public string? ModeloFattureWeb { get; set; }
    public string? ClasseConsumo { get; set; }
    public string? Subgrupo { get; set; }
    public string? IdInstalacao { get; set; }
    public string? DataEmissao { get; set; }
    public string? DataProcessamento { get; set; }
    public string? Descricao { get; set; }
    public double? Quantidade { get; set; }
    public double? ValorTotal { get; set; }
    public double? ValorSemImpostos { get; set; }
    public double? TarifaComImpostos { get; set; }
    public double? TarifaSemImpostos { get; set; }
    public string? TaxaDesconto { get; set; }
    public string? DescricoesOriginais { get; set; }
}
