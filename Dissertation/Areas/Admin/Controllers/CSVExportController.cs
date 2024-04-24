using Dissertation.Data;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Globalization;
using CsvHelper;
using Microsoft.EntityFrameworkCore;

namespace Dissertation.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = "Admin")]
    public class CSVExportController : Controller
    {
        private readonly ApplicationDbContext _context;

        public CSVExportController(ApplicationDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        public async Task<IActionResult> ExportToCsv()
        {
            var data = await _context.Items.ToListAsync(); // Fetch your data from database

            var stream = new MemoryStream();
            var writer = new StreamWriter(stream);
            var csvWriter = new CsvWriter(writer, CultureInfo.InvariantCulture);

            csvWriter.WriteRecords(data);  // Write the data to the CSV file
            await writer.FlushAsync();      // Flushes the written data into the MemoryStream
            stream.Position = 0;           // Rewind the stream for reading

            return File(stream, "text/csv", "export.csv");
        }
    }
}
