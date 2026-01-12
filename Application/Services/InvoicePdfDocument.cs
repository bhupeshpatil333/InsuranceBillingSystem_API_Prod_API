using InsuranceBillingSystem_API_Prod.Application.DTOs.Billing;
using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;

namespace InsuranceBillingSystem_API_Prod.Application.Services
{
    public class InvoicePdfDocument : IDocument
    {
        private readonly InvoiceDto _invoice;

        // ✅ CONSTRUCTOR (interfaces cannot have this, classes can)
        public InvoicePdfDocument(InvoiceDto invoice)
        {
            _invoice = invoice;
        }

        public DocumentMetadata GetMetadata()
            => DocumentMetadata.Default;

        public void Compose(IDocumentContainer container)
        {
            container.Page(page =>
            {
                page.Margin(30);

                page.Header()
                    .Text("INVOICE")
                    .FontSize(20)
                    .Bold()
                    .AlignCenter();

                page.Content().Column(col =>
                {
                    col.Spacing(10);

                    col.Item().Text($"Invoice No: {_invoice.InvoiceNumber}");
                    col.Item().Text($"Patient: {_invoice.PatientName}");
                    col.Item().Text($"Mobile: {_invoice.Mobile}");
                    col.Item().Text($"Date: {_invoice.BillDate:dd-MM-yyyy}");

                    col.Item().LineHorizontal(1);

                    col.Item().Table(table =>
                    {
                        table.ColumnsDefinition(columns =>
                        {
                            columns.RelativeColumn();
                            columns.ConstantColumn(60);
                            columns.ConstantColumn(100);
                        });

                        table.Header(header =>
                        {
                            header.Cell().Text("Service").Bold();
                            header.Cell().Text("Qty").Bold();
                            header.Cell().Text("Amount").Bold();
                        });

                        foreach (var item in _invoice.Items)
                        {
                            table.Cell().Text(item.ServiceName);
                            table.Cell().Text(item.Quantity.ToString());
                            table.Cell().Text(item.Amount.ToString("0.00"));
                        }
                    });

                    col.Item().LineHorizontal(1);

                    col.Item().Text($"Gross Amount: ₹{_invoice.GrossAmount}");
                    col.Item().Text($"Insurance Covered: ₹{_invoice.InsuranceAmount}");
                    col.Item().Text($"Net Payable: ₹{_invoice.NetPayable}");
                    col.Item().Text($"Total Paid: ₹{_invoice.TotalPaid}");
                    col.Item().Text($"Status: {_invoice.Status}").Bold();
                });

                page.Footer()
                    .AlignCenter()
                    .Text("Thank you for choosing our service");
            });
        }
    }
}
