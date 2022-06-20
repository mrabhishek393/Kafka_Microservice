using Database.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Database
{

    /// <summary>
    /// A class to perform database operations.
    /// </summary>
    public class DatabaseServices : IDatabaseServices
    {
        private readonly TestdbContext? _context;

        public DatabaseServices(TestdbContext context)
        {
            _context = context;
        }

        public async Task<List<GroupModel>> GetAllAsync()
        {
            return await _context.GroupInfos.ToListAsync();
        }
        /// <summary>
        /// Returns the corresponding row from database if exists otherwise null.
        /// </summary>
        /// <param name="Id"> Guid as a parameter</param>
        /// <returns>Task of type of single database row</returns>
        public async Task<GroupModel> GetbyIdAsync(Guid? Id)
        {
            return await _context.GroupInfos.FindAsync(Id);
        }

        /// <summary>
        ///  Adds the new row to the database and save the changes.
        /// </summary>
        /// <param name="newvalue">a new value(new database row) as a parameter</param>
        /// <returns>Task</returns>
        public async Task InsertAysnc(GroupModel newvalue)
        {
            try
            {
                await _context.GroupInfos.AddAsync(newvalue);
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateException ex)
            {
                Console.WriteLine("Dublicate Id found:  " + ex.ToString());
                throw;
            }
        }

        /// <summary>
        /// Replaces all the update value in the newvalue to the corresponding row in database.
        /// </summary>
        /// <param name="oldvalue">Current value in Database which needs to be update by new value</param>
        /// <param name="newvalue">New value</param>
        /// <returns>Bool implicating that whether newvalue is same as old or not</returns>
        public async Task UpdateAsync(GroupModel oldvalue, GroupModel newvalue)
        {
            try
            {
                var fromdb = await GetbyIdAsync(oldvalue.Id);

                _context.Entry<GroupModel>(fromdb).CurrentValues.SetValues(newvalue);
                await _context.SaveChangesAsync();
            }
            catch (Exception)
            {
                throw;
            }

        }
        public TestdbContext? _Context
        {
            get { return _context; }
        }
    }
}
