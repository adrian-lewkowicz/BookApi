using BooksApi.Models;
using BooksApi.Service;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Text;
using static System.Reflection.Metadata.BlobBuilder;

namespace BooksApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class BooksController : ControllerBase
    {
        private readonly BookService _bookService;

        public BookService BookService => _bookService;

        private ILogger<BooksController> _logger;
        public BooksController(BookService bookService, ILogger<BooksController> logger)
        {
            _bookService = bookService;
            _logger = logger;
        }

        // GET: api/books?page=1&pageSize=5
        [HttpGet]
        public async Task<ActionResult<List<Book>>> Get(int page = 1, int pageSize = 5)
        {
            try
            {
                page = page < 1 ? 1 : page;
                pageSize = pageSize < 1 ? 5 : pageSize;
                var skip = (page - 1) * pageSize;

                var books = await _bookService.GetAsync(skip, pageSize);

                return Ok(books);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while fetching books for page={Page} with pageSize={PageSize}", page, pageSize);
                return StatusCode(500, "Internal server error.");
            }
        }



        // GET: api/books/{id}
        [HttpGet("{id:length(24)}")]
        public async Task<ActionResult<Book>> Get(string id)
        {
            try
            {
                var book = await _bookService.GetAsync(id);

                if (book == null)
                {
                    return NotFound();
                }

                return book;
            }catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred GET: api/books/{Id}", id);
                return StatusCode(500, "Internal server error.");
            }
        }

        // POST: api/books
        [HttpPost]
        public async Task<IActionResult> Create(Book newBook)
        {
            try
            {
                _logger.LogInformation("Creating new book");
                await _bookService.CreateAsync(newBook);
                return CreatedAtAction(nameof(Get), new { id = newBook.Id }, newBook);
            }catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred POST: api/books");
                return StatusCode(500, "Internal server error.");
            }
        }

        // PUT: api/books/{id}
        [HttpPut("{id:length(24)}")]
        public async Task<IActionResult> Update(string id, Book updatedBook)
        {
            try
            {
                _logger.LogInformation("Received UPDATE for id = {Id}", id);
                var book = await _bookService.GetAsync(id);

                if (book == null)
                {
                    _logger.LogWarning("No books found for id = {Id}", id);
                    return NotFound();
                }

                updatedBook.Id = book.Id;

                await _bookService.UpdateAsync(id, updatedBook);

                return NoContent();
            }catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred PUT: api/books/{id}",id);
                return StatusCode(500, "Internal server error.");
            }
        }

        // DELETE: api/books/{id}
        [HttpDelete("{id:length(24)}")]
        public async Task<IActionResult> Delete(string id)
        {
            try
            {
                _logger.LogInformation("Received UPDATE for id = {Id}", id);
                var book = await _bookService.GetAsync(id);

                if (book == null)
                {
                    _logger.LogWarning("No books found for id = {Id}", id);
                    return NotFound();
                }

                await _bookService.DeleteAsync(id);

                return NoContent();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred DELETE: api/books/{id}", id);
                return StatusCode(500, "Internal server error.");
            }
        }

        // GET: api/books/download
        [HttpGet("download")]
        public IActionResult DownloadBooks()
        {
            try
            {
                _logger.LogInformation("Received download book list");
                var books = _bookService.GetAsync();
                var csvData = new StringBuilder();
                csvData.AppendLine("Id;Title;Author;PublicationDate;Pages;Genre;Rating;Price;ISBN;IsAvailable");

                foreach (var book in books.Result)
                {
                    csvData.AppendLine($"{book.Id};{book.Title};{book.Author};{book.PublicationDate};{book.Pages};{book.Genre};{book.Rating};{book.Price};{book.ISBN};{book.IsAvailable}");
                }

                var fileBytes = Encoding.UTF8.GetBytes(csvData.ToString());
                var fileName = "books.csv";

                return File(fileBytes, "text/csv", fileName);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred DELETE: api/books/download");
                return StatusCode(500, "Internal server error.");
            }
        }

    }

}
