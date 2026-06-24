using TownHubPeople.Application.Admin.Organization;
using TownHubPeople.Application.Common.Models;
using TownHubPeople.Domain.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace TownHubPeople.Application.Admin.Services;

using OrganizationEntity = Domain.Entities.Admin.Organization;

public class OrganizationService : IOrganizationService
{
    private readonly IUnitOfWork _unitOfWork;

    public OrganizationService(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<OrganizationDto> GetAsync(CancellationToken ct = default)
    {
        var entity = await _unitOfWork.Repository<OrganizationEntity>().QueryNoTracking()
            .OrderBy(x => x.Id)
            .FirstOrDefaultAsync(ct);

        if (entity is null)
            return new OrganizationDto { Name = "Your Company" };

        return new OrganizationDto
        {
            Id = entity.Id,
            Name = entity.Name,
            LegalName = entity.LegalName,
            AddressLine1 = entity.AddressLine1,
            AddressLine2 = entity.AddressLine2,
            City = entity.City,
            State = entity.State,
            PostalCode = entity.PostalCode,
            Country = entity.Country,
            Phone = entity.Phone,
            Email = entity.Email,
            TaxId = entity.TaxId,
            CurrencyCode = entity.CurrencyCode,
            CurrencySymbol = entity.CurrencySymbol,
            LogoUrl = entity.LogoUrl
        };
    }

    public async Task<Result> UpdateAsync(OrganizationDto dto, CancellationToken ct = default)
    {
        var entity = await _unitOfWork.Repository<OrganizationEntity>().Query()
            .OrderBy(x => x.Id)
            .FirstOrDefaultAsync(ct);

        if (entity is null)
        {
            entity = new OrganizationEntity();
            _unitOfWork.Repository<OrganizationEntity>().Add(entity);
        }

        entity.Name = dto.Name;
        entity.LegalName = dto.LegalName;
        entity.AddressLine1 = dto.AddressLine1;
        entity.AddressLine2 = dto.AddressLine2;
        entity.City = dto.City;
        entity.State = dto.State;
        entity.PostalCode = dto.PostalCode;
        entity.Country = dto.Country;
        entity.Phone = dto.Phone;
        entity.Email = dto.Email;
        entity.TaxId = dto.TaxId;
        entity.CurrencyCode = dto.CurrencyCode;
        entity.CurrencySymbol = dto.CurrencySymbol;
        entity.LogoUrl = dto.LogoUrl;

        await _unitOfWork.SaveChangesAsync(ct);
        return Result.Success();
    }
}
