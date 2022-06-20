using API.DTOs;
using Database.Models;

namespace API
{
    public interface IAPIServices
    {
        Task<List<GroupModel>> GetAllRecords();
        Task<GroupModel> GetSingleRowById(Guid? id);
        Task<GroupModel> InsertAsync(AddDataDTO newvalue);
        Task<GroupModel> UpdateAsync(UpdateDTO newvalue);

        Task AddEntitlements(PatchDTO request);
    }
}