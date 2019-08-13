using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DatingApp.Data;
using DatingApp.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace DatingApp.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ValuesController : ControllerBase
    {
        private readonly ApplicationDbContext _context;
        public ValuesController(ApplicationDbContext context)
        {
            _context = context;
        }
        // GET api/values

        [HttpGet]
        public async Task<ActionResult> Get()
        {
           var values= await _context.Values.ToListAsync();
            return Ok(values);
        }

        // GET api/values/5
        [AllowAnonymous]
        [HttpGet("{id}")]
        public async Task<ActionResult> Get(int id)
        {
            var value = await _context.Values.FirstOrDefaultAsync(x=>x.Id==id);
            if (value==null)
            {
                return BadRequest();
            }
            return Ok(value);
        }

        // POST api/values
        [HttpPost]
        public ActionResult Post( Value value)
        {
            _context.Values.Add(value);
            _context.SaveChanges();
            return Ok(value);
        }

        // PUT api/values/5
        [HttpPut("{id}")]
        public void Put(int id, [FromBody] string value)
        {
        }

        // DELETE api/values/5
        [HttpDelete("{id}")]
        public void Delete(int id)
        {
        }
    }
}
