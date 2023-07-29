using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using LookOn.Integrations.Haravan.Components.Repositories.Interfaces;
using LookOn.Integrations.Haravan.Models.ValueObjects;

namespace LookOn.Integrations.Haravan;

public class HaravanCustomerManager : LookOnManager
{
    private readonly IHaravanCustomerRepository _haravanCustomerRepository;

    public HaravanCustomerManager(IHaravanCustomerRepository haravanCustomerRepository)
    {
        _haravanCustomerRepository = haravanCustomerRepository;
    }

    public async Task<List<HaravanCustomerPhoneNoAndEmail>> GetCusPhoneNosAndEmails(IList<long> haravanCustomerIds)
    {
        return await _haravanCustomerRepository.GetCusPhoneNosAndEmails(haravanCustomerIds);
    }
}