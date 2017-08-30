using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using SouchProd.EFCore.Firebird.AspNetSample.Models;
using Microsoft.EntityFrameworkCore;

namespace SouchProd.EFCore.Firebird.AspNetSample.Controllers
{
    [Route("api/[controller]")]
    public class Authors2Controller : Controller
    {
        private SampleModelsContext _dbContext;
        
        /// <summary>
        /// Controller constructor
        /// </summary>
        /// <param name="dbContext">Database context from the Dependency injection</param>
        public Authors2Controller(SampleModelsContext dbContext)
        {
            _dbContext = dbContext;
            
            // Perform the pending migrations automatically
            _dbContext.Database.Migrate();
        }

        // GET api/authors
        [HttpGet]
        public async Task<IEnumerable<Author>> GetAsync()
        {
            return await _dbContext.Authors.AsNoTracking().ToListAsync();
        }

        // GET api/authors/5
        [HttpGet("{id}")]
        public async Task<Author> Get(int id)
        {
            return await _dbContext.Authors.FirstOrDefaultAsync(x => x.AuthorId.Equals(id));
        }

        // POST api/authors
        [HttpPost]
        public void Post([FromBody]Author value)
        {
            _dbContext.Authors.Add(value);
            _dbContext.SaveChanges();
        }

        // PUT api/authors/5
        [HttpPut("{id}")]
        public void Put(int id, [FromBody]Author value)
        {
            _dbContext.Authors.Update(value);
            _dbContext.SaveChanges();
        }

        // DELETE api/authors/5
        [HttpDelete("{id}")]
        public void Delete(int id)
        {
            var entity = _dbContext.Authors.FirstOrDefault(x => x.AuthorId.Equals(id));
            if (entity != null)
            {
                _dbContext.Authors.Remove(entity);
                _dbContext.SaveChanges();
            }
        }
    }
}
