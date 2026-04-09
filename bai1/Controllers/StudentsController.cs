using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ConnectDB.Data;
using ConnectDB.Models;

namespace ConnectDB.Controllers;

[Route("api/[controller]")]
[ApiController]
public class StudentsController : ControllerBase
{
    private readonly AppDbContext _context;

    public StudentsController(AppDbContext context)
    {
        _context = context;
    }

    // GET: api/Students
    [HttpGet]
    public async Task<ActionResult<IEnumerable<Student>>> GetStudents()
    {
        return await _context.Students.ToListAsync();
    }

    // GET: api/Students/5
    [HttpGet("{id}")]
    public async Task<ActionResult<Student>> GetStudent(int id)
    {
        var student = await _context.Students.FindAsync(id);

        if (student == null)
            return NotFound();

        return student;
    }

    // POST: api/Students
    [HttpPost]
    public async Task<ActionResult<Student>> PostStudent(Student student)
    {
        _context.Students.Add(student);
        await _context.SaveChangesAsync();

        return CreatedAtAction(nameof(GetStudent), new { id = student.Id }, student);
    }
    [HttpGet("advanced")]
    public async Task<IActionResult> Advanced(string keyword, int page = 1)
    {
        int pageSize = 5;

        var query = _context.Students.AsQueryable();

        if (!string.IsNullOrEmpty(keyword))
        {
            query = query.Where(s => s.FullName.Contains(keyword));
        }

        var result = await query
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync();

        return Ok(result);
    }

    // PUT: api/Students/5
    [HttpPut("{id}")]
    public async Task<IActionResult> PutStudent(int id, Student student)
    {
        if (id != student.Id)
            return BadRequest();

        _context.Entry(student).State = EntityState.Modified;
        await _context.SaveChangesAsync();

        return NoContent();
    }

    // DELETE: api/Students/5
    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteStudent(int id)
    {
        var student = await _context.Students.FindAsync(id);

        if (student == null)
            return NotFound();

        _context.Students.Remove(student);
        await _context.SaveChangesAsync();

        return NoContent();
    }

    // SEARCH
    [HttpGet("search")]
    public async Task<IActionResult> Search(string keyword)
    {
        var result = await _context.Students
            .Where(s => s.FullName.Contains(keyword))
            .ToListAsync();

        return Ok(result);
    }
    // sắp xếp
    [HttpGet("sort")]
    public async Task<IActionResult> Sort(string type)
    {
        var data = _context.Students.AsQueryable();

        if (type == "name")
        {
            data = data.OrderBy(s => s.FullName);
        }
        else if (type == "date")
        {
            data = data.OrderBy(s => s.Birthday);
        }

        return Ok(await data.ToListAsync());
    }
}