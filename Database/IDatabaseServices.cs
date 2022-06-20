using Database.Models;

namespace Database
{
    public interface IDatabaseServices
    {
        TestdbContext? _Context { get; }

        Task<List<GroupModel>> GetAllAsync();
        Task<GroupModel> GetbyIdAsync(Guid? Id);
        Task InsertAysnc(GroupModel newvalue);
        Task UpdateAsync(GroupModel oldvalue, GroupModel newvalue);
    }
}