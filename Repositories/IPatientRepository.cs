using APIClinica.Models;

namespace APIClinica.Repositories
{
    public interface IPatientRepository : IRepository<Patient>
    {
        Task<IEnumerable<Patient>> GetPatientsWithInsuranceAsync();
        Task<Patient?> GetPatientWithInsuranceByIdAsync(int id);
        Task<IEnumerable<Patient>> SearchPatientsAsync(string searchTerm);
    }
}