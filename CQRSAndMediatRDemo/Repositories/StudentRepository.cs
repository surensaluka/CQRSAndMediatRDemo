using CQRSAndMediatRDemo.Data;
using CQRSAndMediatRDemo.Models;
using Microsoft.EntityFrameworkCore;
using System.Net;

namespace CQRSAndMediatRDemo.Repositories
{
    public class StudentRepository : IStudentRepository
    {
        private readonly DbContextClass _dbContext;

        public StudentRepository(DbContextClass dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<StudentDetails> AddStudentAsync(StudentDetails studentDetails)
        {
            var result = await _dbContext.Students.AddAsync(studentDetails);
            await _dbContext.SaveChangesAsync();
            return result.Entity;
        }

        public async Task<int> DeleteStudentAsync(int Id)
        {
            var student = await _dbContext.Students.FirstOrDefaultAsync(x => x.Id == Id);
            if (student == null) throw new ArgumentException(HttpStatusCode.NotFound.ToString());

            _dbContext.Students.Remove(student);
            return await _dbContext.SaveChangesAsync();
        }

        public async Task<StudentDetails> GetStudentByIdAsync(int Id)
        {
            var student = await _dbContext.Students.FirstOrDefaultAsync(x => x.Id == Id);
            if (student == null) throw new ArgumentException(HttpStatusCode.NotFound.ToString());
            return student;
        }

        public async Task<List<StudentDetails>> GetStudentListAsync()
        {
            return await _dbContext.Students.ToListAsync();
        }

        public async Task<int> UpdateStudentAsync(StudentDetails studentDetails)
        {
            _dbContext.Students.Update(studentDetails);
            return await _dbContext.SaveChangesAsync();
        }
    }
}
