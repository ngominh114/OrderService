namespace OrderService.Application.DTOs;

public class InvoiceDto
{
    public int Id { get; set; }
    public int OrderId { get; set; }
    public string InvoiceNumber { get; set; } = string.Empty;
    public decimal Amount { get; set; }
    public DateTime IssuedDate { get; set; }
}
